using System;
using System.Collections.Generic;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
namespace PCIShield.Api
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Load .env file into configuration
                    LoadDotEnvFile(config, hostingContext.HostingEnvironment);
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseStartup<Startup>()
                    .UseUrls("https://localhost:52509/api")
                    .ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(52509, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            var loggerFactory = LoggerFactory.Create(builder =>
                            {
                                builder.AddConsole();
                            });
                            var logger = loggerFactory.CreateLogger<Program>();
                            logger.LogInformation("Configuring HTTPS with certificate");
                            try
                            {
                                listenOptions.UseHttps();
                                logger.LogInformation("Certificate loaded successfully");
                            }
                            catch (Exception ex)
                            {
                                logger.LogError($"Error loading certificate: {ex.Message}");
                                throw;
                            }
                        });
                    }))
                .UseSerilog((context, services, configuration) => configuration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console());
        public static void Main(string[] args)
        {
            var test = args;
            IHost host = CreateHostBuilder(args).Build();
            using (IServiceScope scope = host.Services.CreateScope())
            {
                try
                {
                    IServiceProvider services = scope.ServiceProvider;
                    IWebHostEnvironment hostEnvironment = services.GetRequiredService<IWebHostEnvironment>();
                    ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
                    logger.LogInformation($"Starting in environment {hostEnvironment.EnvironmentName}");
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    throw;
                }
            }
            try
            {
                host.Run();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Loads environment variables from .env file into configuration.
        /// Supports multiple .env files with priority: .env.{Environment} > .env > .env.example
        /// Searches current directory and parent directories up to solution root.
        /// </summary>
        private static void LoadDotEnvFile(IConfigurationBuilder config, IHostEnvironment env)
        {
            var rootPath = FindSolutionRoot() ?? Directory.GetCurrentDirectory();
            var envName = env.EnvironmentName?.ToLower() ?? "development";

            // Priority order for .env files
            var envFiles = new[]
            {
                Path.Combine(rootPath, $".env.{envName}"),      // Highest priority: .env.Development
                Path.Combine(rootPath, ".env"),                 // Standard .env
                Path.Combine(rootPath, ".env.example")          // Fallback to example (for CI/CD)
            };

            var loadedFiles = new List<string>();
            var envVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Load all .env files in priority order (later files don't override earlier ones)
            foreach (var envFile in envFiles)
            {
                if (File.Exists(envFile))
                {
                    try
                    {
                        var lines = File.ReadAllLines(envFile);
                        var parsedCount = 0;

                        foreach (var line in lines)
                        {
                            // Skip empty lines and comments
                            var trimmedLine = line.Trim();
                            if (string.IsNullOrWhiteSpace(trimmedLine) || 
                                trimmedLine.StartsWith("#") || 
                                trimmedLine.StartsWith("//"))
                            {
                                continue;
                            }

                            // Parse KEY=VALUE format
                            var separatorIndex = trimmedLine.IndexOf('=');
                            if (separatorIndex > 0)
                            {
                                var key = trimmedLine.Substring(0, separatorIndex).Trim();
                                var value = trimmedLine.Substring(separatorIndex + 1).Trim();

                                // Remove surrounding quotes if present
                                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                                    (value.StartsWith("'") && value.EndsWith("'")))
                                {
                                    value = value.Substring(1, value.Length - 2);
                                }

                                // Only add if not already set (priority to earlier files)
                                if (!envVariables.ContainsKey(key))
                                {
                                    envVariables[key] = value;
                                    parsedCount++;
                                }
                            }
                        }

                        loadedFiles.Add($"{Path.GetFileName(envFile)} ({parsedCount} vars)");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"⚠️  Warning: Failed to load {envFile}: {ex.Message}");
                    }
                }
            }

            // Add all loaded variables to configuration
            if (envVariables.Count > 0)
            {
                config.AddInMemoryCollection(envVariables);
                
                // IMPORTANT: Build intermediate configuration to enable ${VAR} expansion
                var intermediateConfig = config.Build();
                
                // Now expand any ${VAR} references in appsettings.json
                var expandedConfig = ExpandEnvironmentVariables(intermediateConfig);
                if (expandedConfig.Count > 0)
                {
                    config.AddInMemoryCollection(expandedConfig);
                }
                
                Console.WriteLine("✅ Environment Configuration Loaded:");
                Console.WriteLine($"   Files: {string.Join(", ", loadedFiles)}");
                Console.WriteLine($"   Total Variables: {envVariables.Count}");
                Console.WriteLine($"   Expanded Variables: {expandedConfig.Count}");
                Console.WriteLine($"   Environment: {envName}");
                
                // Validate critical variables
                ValidateCriticalEnvironmentVariables(envVariables);
            }
            else
            {
                Console.WriteLine("⚠️  Warning: No .env files found. Using system environment variables only.");
                Console.WriteLine($"   Searched paths:");
                foreach (var envFile in envFiles)
                {
                    Console.WriteLine($"   - {envFile}");
                }
            }
        }

        /// <summary>
        /// Validates that critical environment variables are set and not placeholder values.
        /// </summary>
        private static void ValidateCriticalEnvironmentVariables(Dictionary<string, string> envVariables)
        {
            var criticalVars = new[]
            {
                "PCISHIELDAPP_SECRET_KEY",
                "SQL_SERVER_PASSWORD",
                "MONGODB_PASSWORD",
                "REDIS_PASSWORD"
            };

            var missingVars = new List<string>();
            var placeholderVars = new List<string>();

            foreach (var varName in criticalVars)
            {
                if (!envVariables.TryGetValue(varName, out var value) || string.IsNullOrWhiteSpace(value))
                {
                    missingVars.Add(varName);
                }
                else if (value.Contains("YOUR_") || value.Contains("changeme") || value.Contains("your-"))
                {
                    placeholderVars.Add(varName);
                }
            }

            if (missingVars.Count > 0)
            {
                Console.Error.WriteLine("❌ CRITICAL: Missing required environment variables:");
                foreach (var varName in missingVars)
                {
                    Console.Error.WriteLine($"   - {varName}");
                }
                Console.Error.WriteLine("\n   Please check your .env file configuration.");
                Console.Error.WriteLine("   See SETUP_SECRETS.md for instructions.\n");
            }

            if (placeholderVars.Count > 0)
            {
                Console.WriteLine("⚠️  WARNING: Some variables still have placeholder values:");
                foreach (var varName in placeholderVars)
                {
                    Console.WriteLine($"   - {varName}");
                }
                Console.WriteLine("\n   These should be replaced with actual values before production use.\n");
            }

            if (missingVars.Count == 0 && placeholderVars.Count == 0)
            {
                Console.WriteLine("✅ All critical environment variables validated successfully.");
            }
        }

        /// <summary>
        /// Finds the solution root directory by searching for .sln or .git files.
        /// Searches current directory and up to 5 parent directories.
        /// </summary>
        private static string FindSolutionRoot()
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            var maxLevels = 5;
            var currentLevel = 0;

            while (currentDir != null && currentLevel < maxLevels)
            {
                // Check for solution file
                if (currentDir.GetFiles("*.sln").Length > 0)
                {
                    Console.WriteLine($"📁 Found solution root: {currentDir.FullName}");
                    return currentDir.FullName;
                }

                // Check for .git directory (alternative indicator)
                if (currentDir.GetDirectories(".git").Length > 0)
                {
                    Console.WriteLine($"📁 Found git root: {currentDir.FullName}");
                    return currentDir.FullName;
                }

                // Check for .env file (direct indicator)
                if (currentDir.GetFiles(".env").Length > 0 || currentDir.GetFiles(".env.example").Length > 0)
                {
                    Console.WriteLine($"📁 Found .env location: {currentDir.FullName}");
                    return currentDir.FullName;
                }

                currentDir = currentDir.Parent;
                currentLevel++;
            }

            // Fallback to current directory
            Console.WriteLine($"⚠️  Could not find solution root, using: {Directory.GetCurrentDirectory()}");
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Expands ${VARIABLE} references in configuration values using loaded environment variables.
        /// This enables appsettings.json to use ${MONGODB_HOST} syntax.
        /// </summary>
        private static Dictionary<string, string> ExpandEnvironmentVariables(IConfiguration config)
        {
            var expanded = new Dictionary<string, string>();
            
            foreach (var item in config.AsEnumerable())
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                    
                // Check if value contains ${VAR} pattern
                var value = item.Value;
                if (value.Contains("${"))
                {
                    // Replace all ${VAR} with actual values
                    var pattern = @"\$\{([^}]+)\}";
                    var expandedValue = System.Text.RegularExpressions.Regex.Replace(value, pattern, match =>
                    {
                        var varName = match.Groups[1].Value;
                        var varValue = config[varName];
                        
                        if (!string.IsNullOrEmpty(varValue))
                        {
                            return varValue;
                        }
                        
                        // Keep original if variable not found
                        Console.WriteLine($"⚠️  Variable not found for expansion: {varName}");
                        return match.Value;
                    });
                    
                    if (expandedValue != value)
                    {
                        expanded[item.Key] = expandedValue;
                        Console.WriteLine($"🔄 Expanded: {item.Key}");
                    }
                }
            }
            
            return expanded;
        }
    }
}
