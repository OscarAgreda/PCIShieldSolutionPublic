using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace PCIShield.Client.Services.Common;
public class GiveMeMyHttpHandler
{
    private readonly string _apiBaseUrl;
    public GiveMeMyHttpHandler(string apiBaseUrl)
    {
        _apiBaseUrl = apiBaseUrl;
    }
    public SocketsHttpHandler CreateMessageHandler()
    {
        if (_apiBaseUrl.Contains("localhost"))
        {
            return GetUnsafeMyHandler();
        }
        return GetMyHandler();
    }
    private SocketsHttpHandler GetMyHandler()
    {
        SocketsHttpHandler? handler = new()
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                EnabledSslProtocols = SslProtocols.Tls13,
                RemoteCertificateValidationCallback = (sender, certificate, chain, errors) =>
                {

                    var cert1 = certificate as X509Certificate2;
                    if (cert1 != null && cert1.Subject.Contains("CN=api"))
                    {
                        return true;
                    }
                    if (errors == SslPolicyErrors.RemoteCertificateNameMismatch)
                    {
                        var cert = certificate as X509Certificate2;
                        if (cert != null && cert.Subject.Contains("CN=api"))
                        {
                            return true;
                        }
                    }
                    else if (errors == SslPolicyErrors.RemoteCertificateChainErrors)
                    {
                        var cert = certificate as X509Certificate2;
                        if (cert != null && cert.Subject.Contains("CN=api"))
                        {
                            return true;
                        }
                    }

                    return errors == SslPolicyErrors.None;
                }
            },
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            EnableMultipleHttp2Connections = true,
            Expect100ContinueTimeout = TimeSpan.FromSeconds(1),
            ActivityHeadersPropagator = null,
            ConnectTimeout = TimeSpan.FromSeconds(30),
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            MaxConnectionsPerServer = 100,
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            MaxResponseDrainSize = 1024 * 1024,
            ResponseDrainTimeout = TimeSpan.FromSeconds(2),
            RequestHeaderEncodingSelector = (name, request) => Encoding.UTF8,
            ResponseHeaderEncodingSelector = (name, request) => Encoding.UTF8
        };
        return handler;
    }
    private SocketsHttpHandler GetUnsafeMyHandler()
    {
        SocketsHttpHandler? handler = new()
        {
            SslOptions =
                new SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    },
                    EnabledSslProtocols = SslProtocols.None,
                },
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            EnableMultipleHttp2Connections = true,
            Expect100ContinueTimeout = TimeSpan.FromSeconds(1),
            ActivityHeadersPropagator = null,
            ConnectTimeout = TimeSpan.FromSeconds(30),
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            MaxConnectionsPerServer = 100,
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            MaxResponseDrainSize = 1024 * 1024,
            ResponseDrainTimeout = TimeSpan.FromSeconds(2),
            RequestHeaderEncodingSelector = (name, request) => Encoding.UTF8,
            ResponseHeaderEncodingSelector = (name, request) => Encoding.UTF8
        };
        return handler;
    }
}