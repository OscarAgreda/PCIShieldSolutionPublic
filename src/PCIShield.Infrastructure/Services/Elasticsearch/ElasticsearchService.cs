using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using PCIShieldLib.SharedKernel.Interfaces;
using System.Linq;
namespace PCIShield.Infrastructure.Services.Elasticsearch
{
    public interface IElasticsearchService<T> where T : class, IAggregateRoot
    {
        Task<bool> CreateIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<IndexResponse> IndexDocumentAsync(T document, string indexName, CancellationToken cancellationToken = default);
        Task<BulkResponse> IndexManyAsync(IEnumerable<T> documents, string indexName, CancellationToken cancellationToken = default);
        Task<T?> GetDocumentAsync(string indexName, string id, CancellationToken cancellationToken = default);
        Task<SearchResponse<T>> SearchAsync(string indexName, SearchRequestDescriptor<T> descriptor, CancellationToken cancellationToken = default);
        Task<UpdateResponse<T>> UpdateDocumentAsync(string indexName, string id, T document, CancellationToken cancellationToken = default);
        Task<DeleteResponse> DeleteDocumentAsync(string indexName, string id, CancellationToken cancellationToken = default);
        Task<CountResponse> CountAsync(string indexName, Func<CountRequestDescriptor<T>, CountRequestDescriptor<T>> descriptor, CancellationToken cancellationToken = default);
        Task<bool> RefreshIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<bool> CreateIndexStandardAsync(string indexName, CancellationToken cancellationToken = default);
        Task<bool> CreateIndexRefinedAsync(string indexName, CancellationToken cancellationToken = default);
        Task<bool> CreateIndexERPAsync<TEntity>(string indexName, IQueryable<TEntity> complexQuery, CancellationToken cancellationToken = default)
            where TEntity : class;
        Task<bool> CreateSimpleIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<bool> CreateIndexWithCustomAnalysisAsync(string indexName, CancellationToken cancellationToken = default);
        Task<bool> IndexObjectGraphAsync(
            string indexName,
            IQueryable<T> specificationQuery,
            CancellationToken cancellationToken = default
        );
        Task IndexDocumentAsync(string indexName, T document, CancellationToken cancellationToken = default);
        Task BulkIndexAsync(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default);
        ElasticsearchClient GetClient();
    }
    public class ElasticsearchService<T> : IElasticsearchService<T> where T : class, IAggregateRoot
    {
        private readonly ElasticsearchClient _client;
        private readonly IAppLoggerService<ElasticsearchService<T>> _logger;
        private readonly IPollyService _pollyService;
        public ElasticsearchService(
            IConfiguration configuration,
            IAppLoggerService<ElasticsearchService<T>> logger,
            IPollyService pollyService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pollyService = pollyService ?? throw new ArgumentNullException(nameof(pollyService));
            var settings = CreateElasticsearchSettings(configuration);
            _client = new ElasticsearchClient(settings);
        }
        #region Internal Setup
        public ElasticsearchClient GetClient() => _client;
        private ElasticsearchClientSettings CreateElasticsearchSettings(IConfiguration configuration)
        {
            var cloudId = configuration["Elasticsearch:CloudId"];
            var apiKey = configuration["Elasticsearch:ApiKey"];

            ElasticsearchClientSettings settings;
            if (!string.IsNullOrEmpty(cloudId) && !string.IsNullOrEmpty(apiKey))
            {
                _logger.LogInformation("[ES] Configuring Elasticsearch using Cloud ID");
                settings = new ElasticsearchClientSettings(cloudId, new ApiKey(apiKey));
            }
            else
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                _logger.LogInformation($"[ES] Configuring Elasticsearch for {environment} environment");
                var url = configuration["Elasticsearch:Url"];
                if (string.IsNullOrEmpty(url))
                {
                    url = environment == "Docker" || environment == "Production"
                        ? "http://elasticsearch:9200"
                        : "http://localhost:9200";
                    _logger.LogWarning($"[ES] URL not found in configuration, using default for {environment}: {url}");
                }

                var username = configuration["Elasticsearch:Username"] ?? "elastic";
                var password = configuration["Elasticsearch:Password"] ?? "changeme";
                var certFingerprint = configuration["Elasticsearch:CertificateFingerprint"];
                var defaultIndex = configuration["Elasticsearch:DefaultIndex"] ?? "pciShield_erp";

                try
                {
                    settings = new ElasticsearchClientSettings(new Uri(url))
                        .Authentication(new BasicAuthentication(username, password))
                        .DefaultIndex(defaultIndex);
                    if (!string.IsNullOrEmpty(certFingerprint))
                    {
                        settings = settings.CertificateFingerprint(certFingerprint);
                    }
                    var enableDebugMode = configuration.GetValue<bool?>("Elasticsearch:EnableDebugMode")
                        ?? (environment == "Development" || environment == "Docker");

                    if (enableDebugMode)
                    {
                        settings = settings.EnableDebugMode()
                            .DisableDirectStreaming()
                            .OnRequestCompleted(details =>
                            {
                                _logger.LogInformation(
                                    $"[ES] Request completed => {details.HttpMethod} {details.Uri} | Status: {details.HttpStatusCode}");
                            });
                    }
                    var timeoutMinutes = configuration.GetValue<int?>("Elasticsearch:RequestTimeoutMinutes") ?? 2;
                    settings = settings.RequestTimeout(TimeSpan.FromMinutes(timeoutMinutes))
                        .ThrowExceptions();
                    if (environment == "Development")
                    {
                        settings = settings.ServerCertificateValidationCallback(
                            (sender, certificate, chain, sslPolicyErrors) => true);
                        _logger.LogWarning("[ES] SSL certificate validation disabled for development environment");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[ES] Failed to configure Elasticsearch client settings");
                    throw new InvalidOperationException("Failed to configure Elasticsearch client settings", ex);
                }
            }
            ConfigureTypeMappings(settings);

            return settings;
        }
        private void ConfigureTypeMappings(ElasticsearchClientSettings settings)
        {
            settings.DefaultMappingFor<T>(m => m
                .IndexName(typeof(T).Name.ToLowerInvariant())
                .IdProperty(GetIdPropertyName()));
        }
        private string GetIdPropertyName()
        {
            var type = typeof(T);
            var idProperty = type.GetProperty("Id") ?? type.GetProperty($"{type.Name}Id")
                ?? throw new InvalidOperationException($"No ID property found for {type.Name}");
            return idProperty.Name;
        }
        #endregion
        #region CreateIndex (Existing approach)
        public async Task<bool> CreateIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.Indices.CreateAsync(indexName, descriptor => descriptor
                    .Settings(s => s
                        .NumberOfShards(3)
                        .NumberOfReplicas(1)
                        .Analysis(a => a
                            .TokenFilters(tf => tf
                                .Stop("stop_filter", st => st
                                    .Stopwords(new[] { "_english_" })
                                )
                                .AsciiFolding("asciifolding_filter", af => af
                                    .PreserveOriginal(false)
                                )
                                .Lowercase("lowercase_filter")
                                .WordDelimiterGraph("word_delimiter_filter", wd => wd
                                    .SplitOnCaseChange()
                                    .PreserveOriginal()
                                )
                            )
                            .CharFilters(cf => cf
                                .HtmlStrip("html_strip_filter")
                            )
                            .Analyzers(an => an
                                .Custom("default_analyzer", ca => ca
                                    .Tokenizer("standard")
                                    .Filter(new [] {"lowercase_filter", "asciifolding_filter", "stop_filter", "word_delimiter_filter"})
                                    .CharFilter(new[] { "html_strip_filter" })
                                )
                            )
                        )
                    ), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed to create index {indexName}: {response.DebugInformation}");
                    return false;
                }
                return true;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        #endregion
        #region DeleteIndex
        public async Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.Indices.DeleteAsync(indexName, null, cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed to delete index {indexName}: {response.DebugInformation}");
                    return false;
                }
                return true;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        #endregion
        #region Three "CreateIndex" Methods (with typed descriptor usage)
        public async Task<bool> CreateIndexStandardAsync(string indexName, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.Indices.CreateAsync(indexName, d => d
                    .Settings(s => s
                        .NumberOfShards(1)
                        .NumberOfReplicas(1)
                    ), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"[Standard] Failed to create index {indexName}: {response.DebugInformation}");
                    return false;
                }
                return true;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<bool> CreateIndexRefinedAsync(string indexName, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.Indices.CreateAsync(indexName, descriptor =>
                {
                    descriptor.Settings(s =>
                    {
                        s.NumberOfShards(2)
                            .NumberOfReplicas(1)
                            .Analysis(
                                a => a
                                    .TokenFilters(
                                        tf => tf
                                            .Synonym(
                                                "my_synonym_filter",
                                                syn => syn
                                                    .Synonyms(new[] { "ny => new york", "sf => san francisco" }))
                                            .Stop(
                                                "stop_en",
                                                st => st
                                                    .Stopwords(new[] { "_english_" })))
                                    .Analyzers(
                                        an => an
                                            .Custom(
                                                "synonym_en",
                                                ca => ca
                                                    .Tokenizer("standard")
                                                    .Filter(new[] { "lowercase", "my_synonym_filter", "stop_en" }))))
                            ;
                    });
                   var WhatDoIDoWithTheDescriptorNow = descriptor;
                }, cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"[Refined] Failed to create index {indexName}: {response.DebugInformation}");
                    return false;
                }
                return true;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<bool> CreateIndexERPAsync<TEntity>(
            string indexName,
            IQueryable<TEntity> complexQuery,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var createResponse = await _client.Indices.CreateAsync(indexName, desc =>
                {
                    desc.Settings(s =>
                    {
                        s.NumberOfShards(3)
                         .NumberOfReplicas(1)
                         .Analysis(a => a
                             .TokenFilters(tf => tf
                                 .Lowercase("lowercase_filter")
                             )
                             .Analyzers(an => an
                                 .Custom("erp_analyzer", ca => ca
                                     .Tokenizer("standard")
                                     .Filter(new [] {"lowercase_filter"})
                                 )
                             )
                         );
                    });
                }, cancellationToken);
                if (!createResponse.IsValidResponse)
                {
                    _logger.LogError($"[ERP] Failed to create index {indexName}: {createResponse.DebugInformation}");
                    return false;
                }
                var dataToIndex = await Task.Run(() => complexQuery.ToList(), cancellationToken);
                if (dataToIndex.Count == 0)
                {
                    _logger.LogInformation($"[ERP] No data found from EF query. Index '{indexName}' created but zero docs indexed.");
                    return true;
                }
                var bulkResponse = await _client.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(dataToIndex)
                    .Refresh(Refresh.WaitFor),
                    cancellationToken);
                if (!bulkResponse.IsValidResponse)
                {
                    _logger.LogError($"[ERP] Failed to bulk index data into {indexName}: {bulkResponse.DebugInformation}");
                    return false;
                }
                _logger.LogInformation($"[ERP] Successfully created & indexed {dataToIndex.Count} documents into '{indexName}'.");
                return true;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<bool> CreateSimpleIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            var response = await _client.Indices.CreateAsync(indexName, null, cancellationToken);
            if (!response.IsValidResponse)
            {
                Console.Error.WriteLine($"Failed to create index {indexName}: {response.DebugInformation}");
                return false;
            }
            return true;
        }
        public async Task<bool> CreateIndexWithCustomAnalysisAsync(string indexName, CancellationToken cancellationToken = default)
        {
            var response = await _client.Indices.CreateAsync(indexName, d =>
            {
                d.Settings(s =>
                {
                    s.NumberOfShards(2)
                        .NumberOfReplicas(1)
                        .Analysis(a => a
                            .TokenFilters(tf => tf
                                .Synonym("my_synonym_filter", syn => syn
                                    .Synonyms(new[] { "ny => new york", "sf => san francisco" })
                                )
                                .Stop("stop_en", st => st.Stopwords(new[] { "_english_" }))
                            )
                            .Analyzers(an => an
                                .Custom("english_synonym", ca => ca
                                    .Tokenizer("standard")
                                    .Filter( new [] {"lowercase", "my_synonym_filter", "stop_en"})
                                )
                            )
                        );
                });
                var WhatDoIDoWithDHereNow = d;
            }, cancellationToken);
            if (!response.IsValidResponse)
            {
                Console.Error.WriteLine($"Failed to create index {indexName}: {response.DebugInformation}");
                return false;
            }
            return true;
        }
        public async Task<bool> IndexObjectGraphAsync(
            string indexName,
            IQueryable<T> specificationQuery,
            CancellationToken cancellationToken = default)
        {
            var itemsToIndex = specificationQuery.ToList();
            if (itemsToIndex.Count == 0)
            {
                return true;
            }
            try
            {
                var response = await _client.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(itemsToIndex)
                    .Refresh(Refresh.WaitFor), cancellationToken);
                if (!response.IsValidResponse)
                {
                    Console.Error.WriteLine($"Bulk index to {indexName} failed: {response.DebugInformation}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception during bulk index: {ex.Message}");
                return false;
            }
        }
        public async Task IndexDocumentAsync(string indexName, T document, CancellationToken cancellationToken = default)
        {
            var response = await _client.IndexAsync(document, req => req
                    .Index(indexName)
                    .Refresh(Refresh.WaitFor),
                cancellationToken);
            if (!response.IsValidResponse)
                Console.Error.WriteLine($"IndexDocument failed: {response.DebugInformation}");
        }
        public async Task BulkIndexAsync(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default)
        {
            var response = await _client.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(documents)
                    .Refresh(Refresh.WaitFor),
                cancellationToken);
            if (!response.IsValidResponse)
                Console.Error.WriteLine($"BulkIndex failed: {response.DebugInformation}");
        }
        #endregion
        #region Document-Level CRUD & Search
        public async Task<IndexResponse> IndexDocumentAsync(T document, string indexName, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.IndexAsync(document, i => i
                    .Index(indexName)
                    .Refresh(Refresh.WaitFor), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed to index document: {response.DebugInformation}");
                }
                return response;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<BulkResponse> IndexManyAsync(IEnumerable<T> documents, string indexName, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(documents)
                    .Refresh(Refresh.WaitFor), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed bulk index operation: {response.DebugInformation}");
                }
                return response;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<T?> GetDocumentAsync(string indexName, string id, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.GetAsync<T>(id, g => g.Index(indexName), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed to get document {id}: {response.DebugInformation}");
                    return null;
                }
                return response.Source;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<SearchResponse<T>> SearchAsync(
            string indexName,
            SearchRequestDescriptor<T> descriptor,
            CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.SearchAsync(descriptor.Index(indexName), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Search failed: {response.DebugInformation}");
                }
                return response;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<UpdateResponse<T>> UpdateDocumentAsync(string indexName, string id, T document, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.UpdateAsync<T, T>(indexName, id, u => u
                    .Doc(document)
                    .Refresh(Refresh.WaitFor), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed to update document {id}: {response.DebugInformation}");
                }
                return response;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<DeleteResponse> DeleteDocumentAsync(string indexName, string id, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.DeleteAsync<T>(id, d => d
                    .Index(indexName)
                    .Refresh(Refresh.WaitFor), cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed to delete document {id}: {response.DebugInformation}");
                }
                return response;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<CountResponse> CountAsync(
            string indexName,
            Func<CountRequestDescriptor<T>, CountRequestDescriptor<T>> descriptor,
            CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.CountAsync<T>(c =>
                {
                    var d = new CountRequestDescriptor<T>();
                    d = descriptor.Invoke(d);
                    var WhatIDowithDnowHere = d;
                }, cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Count operation failed: {response.DebugInformation}");
                }
                return response;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        public async Task<bool> RefreshIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            return await _pollyService.ExecuteFuncWithRetryPolicyAsync(async () =>
            {
                var response = await _client.Indices.RefreshAsync(indexName, cancellationToken: cancellationToken);
                if (!response.IsValidResponse)
                {
                    _logger.LogError($"Failed to refresh index {indexName}: {response.DebugInformation}");
                    return false;
                }
                return true;
            }, 3, TimeSpan.FromSeconds(5), cancellationToken);
        }
        #endregion
    }
}