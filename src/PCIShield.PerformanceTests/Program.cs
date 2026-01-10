using System;
using NBomber.CSharp;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using PCIShield.PerformanceTests.Config;
using PCIShield.PerformanceTests.Scenarios;

// Get test profile from environment or use default
var testProfile = Environment.GetEnvironmentVariable("NBOMBER_PROFILE") ?? "smoke";
Console.WriteLine($"Running NBomber with profile: {testProfile}");

// Select scenarios based on profile
var scenarios = testProfile.ToLower() switch
{
    "smoke" => new[] { MerchantScenarios.CreateSmokeTestScenario() },
    "load" => new[] { 
        MerchantScenarios.CreateCrudScenario(),
        MerchantScenarios.CreateQueryScenario(),
        MerchantScenarios.CreateRelationshipScenario()
    },
    "stress" => new[] { 
        MerchantScenarios.CreateStressScenario(),
        MerchantScenarios.CreateConcurrencyScenario()
    },
    "soak" => new[] { MerchantScenarios.CreateSoakTestScenario() },
    "full" => new[] { 
        MerchantScenarios.CreateCrudScenario(),
        MerchantScenarios.CreateQueryScenario(),
        MerchantScenarios.CreateRelationshipScenario(),
        MerchantScenarios.CreateCascadeScenario(),
        MerchantScenarios.CreatePerformanceScenario()
    },
    _ => new[] { MerchantScenarios.CreateSmokeTestScenario() }
};

// Configure and run NBomber
NBomberRunner
    .RegisterScenarios(scenarios)
    .WithTestSuite("PCIShield.Performance")
    .WithTestName("Merchant_Performance_{0:yyyyMMdd_HHmmss}".Replace("yyyyMMdd_HHmmss", DateTime.Now.ToString("yyyyMMdd_HHmmss")))
    .WithReportFolder("reports")
    .WithReportFormats(
        ReportFormat.Html,
        ReportFormat.Csv,
        ReportFormat.Txt,
        ReportFormat.Md)
    .Run();

Console.WriteLine("Performance test completed. Check the reports folder for results.");
