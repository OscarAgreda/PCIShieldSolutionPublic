using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using PCIShield.Domain.DomainUtilities;

using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Infrastructure.Services;
public class MerchantComplianceOfficerMessagePublisherService : IMerchantComplianceOfficerMessagePublisherService
{
    private readonly IConfiguration _configuration;
    private IModel _channel;
    private IConnection _connection;
    private string _exchangeName;
    private HashSet<Guid> _publishedMessageIds = new HashSet<Guid>();
    public MerchantComplianceOfficerMessagePublisherService(IConfiguration configuration)
    {
        _configuration = configuration;
        InitPublisher();
    }
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
    public static JsonSerializerOptions GetSystemTextJsonSettings()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
            MaxDepth = 0,
            IgnoreReadOnlyFields = false,
        };
    }
    public void PublishDomainEventToQueue(BaseDomainEvent domainEvent, string queueType)
    {
        string routingKey = queueType;
        try
        {
            var settings = GetSystemTextJsonSettings();
            string message = System.Text.Json.JsonSerializer.Serialize(
                domainEvent,
                settings
            );
            byte[]? body = Encoding.UTF8.GetBytes(message);
            IBasicProperties? properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            _channel.BasicPublish(
                _exchangeName,
                routingKey,
                properties,
                body
            );
        }
        catch (Exception ex)
        {
            int aa = 1;
        }
    }
    private void InitPublisher()
    {
        string? hostName = _configuration["RabbitMq:HostName"];
        string? userName = _configuration["RabbitMq:UserName"];
        string? password = _configuration["RabbitMq:Password"];
        int port = int.Parse(_configuration["RabbitMq:Port"]);
        string? virtualHost = "/";
        if (hostName != null)
        {
            IPAddress[]? addresses = Dns.GetHostAddresses(hostName);
            List<IPAddress>? ipv4Addresses = addresses.ToList()
                .Where(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
            if (ipv4Addresses.Count < 1 || ipv4Addresses[0] == null)
            {
                throw new InvalidOperationException("No IPv4 address found for the specified host.");
            }
            AmqpTcpEndpoint? endpoint = new AmqpTcpEndpoint(ipv4Addresses[0].ToString(), port);
            string? certPath = _configuration["RabbitMq:CertPath"];
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                certPath = "C:\\_o\\shared\\PCIShieldErp\\PCIShieldAppNewApp\\docker\\_\\krakend\\my-rabbitmq.crt";
            }
            else
            {
            }
            try
            {
                ConnectionFactory? factory = new ConnectionFactory()
                {
                    Ssl = new SslOption
                    {
                        Enabled = true,
                        ServerName = hostName,
                        CertPath = certPath,
                        CertPassphrase = _configuration["RabbitMq:CertPassphrase"],
                        AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                 SslPolicyErrors.RemoteCertificateChainErrors
                    },
                    RequestedHeartbeat = TimeSpan.FromSeconds(10),
                    HostName = hostName,
                    UserName = userName,
                    Password = password,
                    Endpoint = endpoint,
                    Port = port,
                    VirtualHost = virtualHost,
                    AutomaticRecoveryEnabled = true,
                    DispatchConsumersAsync = true,
                    TopologyRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };
                string? exchangeName = "ERPExchange";
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _exchangeName = exchangeName;
                _channel.ExchangeDeclare(_exchangeName, "topic", true);
                _channel.QueueBind(
                    "HelloComplianceOfficerIAmTheMerchant",
                    _exchangeName,
                    "hello_ComplianceOfficer_i_am_the_merchant"
                );
                _channel.QueueBind(
                    "MerchantFirstAutoChat",
                    _exchangeName,
                    "merchant_first_auto_chat"
                );
                _channel.QueueBind(
                    "MerchantToServerQueueRegularChat",
                    _exchangeName,
                    "merchant_sent_regular_chat"
                );
                _channel.QueueBind(
                    "FromServerAfterMerchantToServerQueueRegularChat",
                    _exchangeName,
                    "from_server_after_merchant_sent_regular_chat"
                );
                _channel.QueueBind(
                    "MerchantExitedConversation",
                    _exchangeName,
                    "merchant_exited_conversation"
                );
                _channel.QueueBind(
                    "ComplianceOfficerPleaseReloadDashBoard",
                    _exchangeName,
                    "ComplianceOfficer_please_reload_dashboard"
                );
                _channel.QueueBind(
                    "ComplianceOfficerToServerQueueResponseInitChat",
                    _exchangeName,
                    "ComplianceOfficer_responded_init_chat2"
                );
                _channel.QueueBind(
                    "ComplianceOfficerToServerQueueFirstEverChat",
                    _exchangeName,
                    "ComplianceOfficer_first_ever_chat_to_server1"
                );
                _channel.QueueBind(
                    "ComplianceOfficerContinuedRegularChat",
                    _exchangeName,
                    "ComplianceOfficer_continued_regular_chat"
                );
                _channel.QueueBind(
                    "ServerToMerchantFromOrigComplianceOfficerContinueRegularChat",
                    _exchangeName,
                    "from_server_from_ComplianceOfficer_continue_regular_chat"
                );
                _channel.QueueBind(
                    "ComplianceOfficerToServerQueueRegularChat",
                    _exchangeName,
                    "ComplianceOfficer_sent_regular_chat"
                );
                _channel.QueueBind(
                    "FromComplianceOfficerFromServerFirstEver",
                    _exchangeName,
                    "from_ComplianceOfficer_from_server_first_ever1"
                );
            }
            catch (ArgumentException ex)
            {
            }
            catch (Exception ex)
            {
            }
        }
    }

    public void PublishDomainEventToQueue(BaseDomainEvent domainEvent, string queueType, Guid responseMessageId)
    {
        string routingKey = queueType;
        try
        {
            if (_publishedMessageIds.Contains(responseMessageId))
            {
                return;
            }
            _publishedMessageIds.Add(responseMessageId);
            domainEvent.EventId = responseMessageId;

            var settings = GJset.GetSystemTextJsonSettings();
            string message = System.Text.Json.JsonSerializer.Serialize(
                domainEvent,
                settings
            );
            byte[]? body = Encoding.UTF8.GetBytes(message);
            IBasicProperties? properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            _channel.BasicPublish(
                _exchangeName,
                routingKey,
                properties,
                body
            );
        }
        catch (Exception ex)
        {
            int aa = 1;
        }
    }
}