using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Contracts;
using PCIShield.PerformanceTests.Infrastructure;
using PCIShield.PerformanceTests.DataFakers;
using PCIShield.Client.Services.Merchant;
using PCIShield.BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.Merchant;
using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel.Interfaces;
using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.BlazorMauiShared.Models.Asset;
using PCIShield.BlazorMauiShared.Models.CompensatingControl;
using PCIShield.BlazorMauiShared.Models.ComplianceOfficer;
using PCIShield.BlazorMauiShared.Models.CryptographicInventory;
using PCIShield.BlazorMauiShared.Models.Evidence;
using PCIShield.BlazorMauiShared.Models.NetworkSegmentation;
using PCIShield.BlazorMauiShared.Models.PaymentChannel;
using PCIShield.BlazorMauiShared.Models.ServiceProvider;

namespace PCIShield.PerformanceTests.Steps;

public static class MerchantSteps
{
    private static readonly TestDataContext _testContext = new();

    #region Merchant Main Entity Steps

    public static async Task<IResponse> CreateMerchant(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        var faker = new MerchantFaker();
        var dto = faker.Generate();

        try
        {
            var request = new CreateMerchantRequest { Merchant = dto };
            var response = await client.CreatePostMerchantAsync(request);

            if (response?.Merchant != null)
            {
                var id = response.Merchant.MerchantId;
                context.Data["MerchantId"] = id;
                _testContext.Set("MerchantId", id);
                _testContext.AddToList("MerchantIds", id);
                context.Logger.Information("Created Merchant: {Id}", id);
                return Response.Ok(statusCode: "201");
            }

            return Response.Fail(statusCode: "422", message: "Failed to create Merchant");
        }
        catch (Exception ex)
        {
            context.Logger.Error("Error creating Merchant: {Message}", ex.Message);
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateMerchant(IScenarioContext context)
    {
        Guid id;
        if (context.Data.TryGetValue("MerchantId", out var idObj) && idObj is Guid)
        {
            id = (Guid)idObj;
        }
        else
        {
            id = _testContext.GetRandomFromList("MerchantIds", context.Random);
            if (id == Guid.Empty)
                return Response.Fail(statusCode: "400", message: "MerchantId not found in context or cache");
        }

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var getResponse = await client.GetOneFullMerchantByIdAsync(id);
            if (getResponse?.Merchant == null)
                return Response.Fail(statusCode: "404", message: "Entity not found for update");

            var dto = getResponse.Merchant;
            dto.MerchantCode = $"Updated_{DateTime.UtcNow:yyyyMMdd_HHmmss}";

            var request = new UpdateMerchantRequest { Merchant = dto };
            var updateResponse = await client.UpdatePutMerchantAsync(request);

            if (updateResponse?.Merchant != null)
            {
                context.Logger.Information("Updated Merchant: {Id}", id);
                return Response.Ok();
            }

            return Response.Fail(statusCode: "422", message: "Failed to update Merchant");
        }
        catch (Exception ex)
        {
            context.Logger.Error("Error updating Merchant: {Message}", ex.Message);
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteMerchant(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var idObj) || idObj is not Guid)
            return Response.Fail(statusCode: "400", message: "MerchantId not found in context");
        var id = (Guid)idObj;

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteMerchantAsync(id);
            context.Logger.Information("Deleted Merchant: {Id}", id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            context.Logger.Error("Error deleting Merchant: {Message}", ex.Message);
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetMerchantById(IScenarioContext context)
    {
        Guid id;
        if (context.Data.TryGetValue("MerchantId", out var idObj) && idObj is Guid)
        {
            id = (Guid)idObj;
        }
        else
        {
            id = _testContext.GetRandomFromList("MerchantIds", context.Random);
            if (id == Guid.Empty)
                return Response.Fail(statusCode: "400", message: "MerchantId not found");
        }

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var response = await client.GetOneFullMerchantByIdAsync(id, withPostGraph: true);
            if (response?.Merchant != null)
            {
                context.Logger.Information("Retrieved Merchant: {Id}", id);
                return Response.Ok();
            }
            return Response.Fail(statusCode: "404", message: "Entity not found");
        }
        catch (Exception ex)
        {
            context.Logger.Error("Error getting Merchant: {Message}", ex.Message);
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetLastCreatedMerchant(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var response = await client.GetLastCreatedMerchantAsync();
            if (response?.Merchant != null)
            {
                context.Data["MerchantId"] = response.Merchant.MerchantId;
                context.Logger.Information("Retrieved last created Merchant");
                return Response.Ok();
            }
            return Response.Fail(statusCode: "404", message: "No last created entity found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedMerchants(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedMerchantsListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedMerchants");

            var items = resp.Merchants;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged Merchants: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchMerchants(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchMerchantByMerchantAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchMerchants");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredMerchant(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredMerchantListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredMerchantListAsync");

            var items = resp.Merchants;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered Merchants: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> RunMerchantPerformanceTests(IScenarioContext context)
    {
        Guid id;
        if (context.Data.TryGetValue("MerchantId", out var idObj) && idObj is Guid)
        {
            id = (Guid)idObj;
        }
        else
        {
            id = _testContext.GetRandomFromList("MerchantIds", context.Random);
            if (id == Guid.Empty)
                return Response.Fail(statusCode: "400", message: "MerchantId not found");
        }

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var response = await client.RunMerchantPerformanceTests(id);
            if (response != null)
            {
                context.Logger.Information("Performance test completed for Merchant: {Id}", id);
                return Response.Ok();
            }
            return Response.Fail(statusCode: "422", message: "Performance test returned empty response");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    #endregion

    #region Child Entity Steps

    public static async Task<IResponse> CreateAssessment(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new AssessmentFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateAssessmentAsync(dto);
            if (response?.Assessment != null)
            {
                var newEntity = response.Assessment;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["AssessmentId"] = newEntity.AssessmentId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("AssessmentIds", newEntity.AssessmentId);

                context.Logger.Information("Created Assessment: {Id}", newEntity.AssessmentId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create Assessment");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateAssessment(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("AssessmentId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "AssessmentId not found");
            var get = await client.GetOneFullAssessmentByIdAsync(id);
            if (get?.Assessment is null)
                return Response.Fail(statusCode: "404", message: "Assessment not found");
            var dto = get.Assessment;
            var updated = await client.UpdateAssessmentAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update Assessment failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteAssessment(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("AssessmentId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "AssessmentId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteAssessmentAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetAssessmentById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("AssessmentId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "AssessmentId not found");
            var response = await client.GetOneFullAssessmentByIdAsync(id);
            return response?.Assessment != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "Assessment not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedAssessments(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedAssessmentsListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedAssessments");

            var items = resp.Assessments;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged Assessments: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchAssessments(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchAssessmentsAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchAssessments");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredAssessments(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredAssessmentsListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredAssessmentListAsync");

            var items = resp.Assessments;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered Assessments: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreateAsset(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new AssetFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateAssetAsync(dto);
            if (response?.Asset != null)
            {
                var newEntity = response.Asset;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["AssetId"] = newEntity.AssetId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("AssetIds", newEntity.AssetId);

                context.Logger.Information("Created Asset: {Id}", newEntity.AssetId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create Asset");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateAsset(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("AssetId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "AssetId not found");
            var get = await client.GetOneFullAssetByIdAsync(id);
            if (get?.Asset is null)
                return Response.Fail(statusCode: "404", message: "Asset not found");
            var dto = get.Asset;
            var updated = await client.UpdateAssetAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update Asset failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteAsset(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("AssetId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "AssetId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteAssetAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetAssetById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("AssetId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "AssetId not found");
            var response = await client.GetOneFullAssetByIdAsync(id);
            return response?.Asset != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "Asset not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedAssets(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedAssetsListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedAssets");

            var items = resp.Assets;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged Assets: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchAssets(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchAssetsAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchAssets");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredAssets(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredAssetsListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredAssetListAsync");

            var items = resp.Assets;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered Assets: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreateCompensatingControl(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new CompensatingControlFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateCompensatingControlAsync(dto);
            if (response?.CompensatingControl != null)
            {
                var newEntity = response.CompensatingControl;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["CompensatingControlId"] = newEntity.CompensatingControlId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("CompensatingControlIds", newEntity.CompensatingControlId);

                context.Logger.Information("Created CompensatingControl: {Id}", newEntity.CompensatingControlId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create CompensatingControl");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateCompensatingControl(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("CompensatingControlId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "CompensatingControlId not found");
            var get = await client.GetOneFullCompensatingControlByIdAsync(id);
            if (get?.CompensatingControl is null)
                return Response.Fail(statusCode: "404", message: "CompensatingControl not found");
            var dto = get.CompensatingControl;
            var updated = await client.UpdateCompensatingControlAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update CompensatingControl failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteCompensatingControl(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("CompensatingControlId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "CompensatingControlId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteCompensatingControlAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetCompensatingControlById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("CompensatingControlId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "CompensatingControlId not found");
            var response = await client.GetOneFullCompensatingControlByIdAsync(id);
            return response?.CompensatingControl != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "CompensatingControl not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedCompensatingControls(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedCompensatingControlsListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedCompensatingControls");

            var items = resp.CompensatingControls;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged CompensatingControls: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchCompensatingControls(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchCompensatingControlsAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchCompensatingControls");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredCompensatingControls(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredCompensatingControlsListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredCompensatingControlListAsync");

            var items = resp.CompensatingControls;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered CompensatingControls: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreateComplianceOfficer(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new ComplianceOfficerFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateComplianceOfficerAsync(dto);
            if (response?.ComplianceOfficer != null)
            {
                var newEntity = response.ComplianceOfficer;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["ComplianceOfficerId"] = newEntity.ComplianceOfficerId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("ComplianceOfficerIds", newEntity.ComplianceOfficerId);

                context.Logger.Information("Created ComplianceOfficer: {Id}", newEntity.ComplianceOfficerId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create ComplianceOfficer");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateComplianceOfficer(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("ComplianceOfficerId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "ComplianceOfficerId not found");
            var get = await client.GetOneFullComplianceOfficerByIdAsync(id);
            if (get?.ComplianceOfficer is null)
                return Response.Fail(statusCode: "404", message: "ComplianceOfficer not found");
            var dto = get.ComplianceOfficer;
            var updated = await client.UpdateComplianceOfficerAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update ComplianceOfficer failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteComplianceOfficer(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("ComplianceOfficerId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "ComplianceOfficerId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteComplianceOfficerAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetComplianceOfficerById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("ComplianceOfficerId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "ComplianceOfficerId not found");
            var response = await client.GetOneFullComplianceOfficerByIdAsync(id);
            return response?.ComplianceOfficer != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "ComplianceOfficer not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedComplianceOfficers(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedComplianceOfficersListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedComplianceOfficers");

            var items = resp.ComplianceOfficers;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged ComplianceOfficers: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchComplianceOfficers(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchComplianceOfficersAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchComplianceOfficers");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredComplianceOfficers(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredComplianceOfficersListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredComplianceOfficerListAsync");

            var items = resp.ComplianceOfficers;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered ComplianceOfficers: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreateCryptographicInventory(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new CryptographicInventoryFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateCryptographicInventoryAsync(dto);
            if (response?.CryptographicInventory != null)
            {
                var newEntity = response.CryptographicInventory;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["CryptographicInventoryId"] = newEntity.CryptographicInventoryId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("CryptographicInventoryIds", newEntity.CryptographicInventoryId);

                context.Logger.Information("Created CryptographicInventory: {Id}", newEntity.CryptographicInventoryId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create CryptographicInventory");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateCryptographicInventory(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("CryptographicInventoryId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "CryptographicInventoryId not found");
            var get = await client.GetOneFullCryptographicInventoryByIdAsync(id);
            if (get?.CryptographicInventory is null)
                return Response.Fail(statusCode: "404", message: "CryptographicInventory not found");
            var dto = get.CryptographicInventory;
            var updated = await client.UpdateCryptographicInventoryAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update CryptographicInventory failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteCryptographicInventory(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("CryptographicInventoryId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "CryptographicInventoryId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteCryptographicInventoryAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetCryptographicInventoryById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("CryptographicInventoryId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "CryptographicInventoryId not found");
            var response = await client.GetOneFullCryptographicInventoryByIdAsync(id);
            return response?.CryptographicInventory != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "CryptographicInventory not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedCryptographicInventories(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedCryptographicInventoriesListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedCryptographicInventories");

            var items = resp.CryptographicInventories;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged CryptographicInventories: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchCryptographicInventories(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchCryptographicInventoriesAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchCryptographicInventories");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredCryptographicInventories(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredCryptographicInventoriesListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredCryptographicInventoryListAsync");

            var items = resp.CryptographicInventories;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered CryptographicInventories: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreateEvidence(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new EvidenceFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateEvidenceAsync(dto);
            if (response?.Evidence != null)
            {
                var newEntity = response.Evidence;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["EvidenceId"] = newEntity.EvidenceId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("EvidenceIds", newEntity.EvidenceId);

                context.Logger.Information("Created Evidence: {Id}", newEntity.EvidenceId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create Evidence");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateEvidence(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("EvidenceId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "EvidenceId not found");
            var get = await client.GetOneFullEvidenceByIdAsync(id);
            if (get?.Evidence is null)
                return Response.Fail(statusCode: "404", message: "Evidence not found");
            var dto = get.Evidence;
            var updated = await client.UpdateEvidenceAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update Evidence failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteEvidence(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("EvidenceId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "EvidenceId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteEvidenceAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetEvidenceById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("EvidenceId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "EvidenceId not found");
            var response = await client.GetOneFullEvidenceByIdAsync(id);
            return response?.Evidence != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "Evidence not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedEvidences(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedEvidencesListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedEvidences");

            var items = resp.Evidences;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged Evidences: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchEvidences(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchEvidencesAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchEvidences");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredEvidences(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredEvidencesListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredEvidenceListAsync");

            var items = resp.Evidences;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered Evidences: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreateNetworkSegmentation(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new NetworkSegmentationFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateNetworkSegmentationAsync(dto);
            if (response?.NetworkSegmentation != null)
            {
                var newEntity = response.NetworkSegmentation;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["NetworkSegmentationId"] = newEntity.NetworkSegmentationId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("NetworkSegmentationIds", newEntity.NetworkSegmentationId);

                context.Logger.Information("Created NetworkSegmentation: {Id}", newEntity.NetworkSegmentationId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create NetworkSegmentation");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateNetworkSegmentation(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("NetworkSegmentationId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "NetworkSegmentationId not found");
            var get = await client.GetOneFullNetworkSegmentationByIdAsync(id);
            if (get?.NetworkSegmentation is null)
                return Response.Fail(statusCode: "404", message: "NetworkSegmentation not found");
            var dto = get.NetworkSegmentation;
            var updated = await client.UpdateNetworkSegmentationAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update NetworkSegmentation failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteNetworkSegmentation(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("NetworkSegmentationId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "NetworkSegmentationId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteNetworkSegmentationAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetNetworkSegmentationById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("NetworkSegmentationId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "NetworkSegmentationId not found");
            var response = await client.GetOneFullNetworkSegmentationByIdAsync(id);
            return response?.NetworkSegmentation != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "NetworkSegmentation not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedNetworkSegmentations(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedNetworkSegmentationsListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedNetworkSegmentations");

            var items = resp.NetworkSegmentations;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged NetworkSegmentations: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchNetworkSegmentations(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchNetworkSegmentationsAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchNetworkSegmentations");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredNetworkSegmentations(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredNetworkSegmentationsListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredNetworkSegmentationListAsync");

            var items = resp.NetworkSegmentations;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered NetworkSegmentations: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreatePaymentChannel(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new PaymentChannelFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreatePaymentChannelAsync(dto);
            if (response?.PaymentChannel != null)
            {
                var newEntity = response.PaymentChannel;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["PaymentChannelId"] = newEntity.PaymentChannelId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("PaymentChannelIds", newEntity.PaymentChannelId);

                context.Logger.Information("Created PaymentChannel: {Id}", newEntity.PaymentChannelId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create PaymentChannel");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdatePaymentChannel(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("PaymentChannelId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "PaymentChannelId not found");
            var get = await client.GetOneFullPaymentChannelByIdAsync(id);
            if (get?.PaymentChannel is null)
                return Response.Fail(statusCode: "404", message: "PaymentChannel not found");
            var dto = get.PaymentChannel;
            var updated = await client.UpdatePaymentChannelAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update PaymentChannel failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeletePaymentChannel(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("PaymentChannelId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "PaymentChannelId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeletePaymentChannelAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPaymentChannelById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("PaymentChannelId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "PaymentChannelId not found");
            var response = await client.GetOneFullPaymentChannelByIdAsync(id);
            return response?.PaymentChannel != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "PaymentChannel not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedPaymentChannels(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedPaymentChannelsListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedPaymentChannels");

            var items = resp.PaymentChannels;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged PaymentChannels: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchPaymentChannels(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchPaymentChannelsAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchPaymentChannels");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredPaymentChannels(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredPaymentChannelsListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredPaymentChannelListAsync");

            var items = resp.PaymentChannels;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered PaymentChannels: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> CreateServiceProvider(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("MerchantId", out var parentIdObj) || parentIdObj is not Guid parentId)
            return Response.Fail(statusCode: "400", message: "Parent MerchantId not found");
        var client = ClientFactory.CreateMerchantClient();
        var faker = new ServiceProviderFaker();
        var dto = faker.Generate();
        dto.MerchantId = parentId;
        try
        {
            var response = await client.CreateServiceProviderAsync(dto);
            if (response?.ServiceProvider != null)
            {
                var newEntity = response.ServiceProvider;

                // Always store the new entity's own PK in the current scenario's context.
                context.Data["ServiceProviderId"] = newEntity.ServiceProviderId;

                // For simple, non-composite entities, add their ID to the shared context
                // so other test scenarios can randomly pick from this list.
                _testContext.AddToList("ServiceProviderIds", newEntity.ServiceProviderId);

                context.Logger.Information("Created ServiceProvider: {Id}", newEntity.ServiceProviderId);
                return Response.Ok(statusCode: "201");
            }
            return Response.Fail(statusCode: "422", message: "Failed to create ServiceProvider");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> UpdateServiceProvider(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("ServiceProviderId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "ServiceProviderId not found");
            var get = await client.GetOneFullServiceProviderByIdAsync(id);
            if (get?.ServiceProvider is null)
                return Response.Fail(statusCode: "404", message: "ServiceProvider not found");
            var dto = get.ServiceProvider;
            var updated = await client.UpdateServiceProviderAsync(dto);
            return updated != null
                ? Response.Ok()
                : Response.Fail(statusCode: "500", message: "Update ServiceProvider failed");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> DeleteServiceProvider(IScenarioContext context)
    {
        if (!context.Data.TryGetValue("ServiceProviderId", out var idObj) || idObj is not Guid id)
            return Response.Fail(statusCode: "400", message: "ServiceProviderId not found");

        var client = ClientFactory.CreateMerchantClient();
        try
        {
            await client.DeleteServiceProviderAsync(id);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetServiceProviderById(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            if (!context.Data.TryGetValue("ServiceProviderId", out var idObj) || idObj is not Guid id)
                return Response.Fail(statusCode: "400", message: "ServiceProviderId not found");
            var response = await client.GetOneFullServiceProviderByIdAsync(id);
            return response?.ServiceProvider != null
                ? Response.Ok()
                : Response.Fail(statusCode: "404", message: "ServiceProvider not found");
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetPagedServiceProviders(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedServiceProvidersListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedServiceProviders");

            var items = resp.ServiceProviders;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged ServiceProviders: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchServiceProviders(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchServiceProvidersAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchServiceProviders");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredServiceProviders(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredServiceProvidersListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredServiceProviderListAsync");

            var items = resp.ServiceProviders;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered ServiceProviders: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    #endregion

    #region Descendant Entity Steps

    //public static async Task<IResponse> CreateControl(IScenarioContext context)
    //{
    //    var client = ClientFactory.CreateMerchantClient();
    //    var faker = new ControlFaker();
    //    var dto = faker.Generate();
    //    try
    //    {
    //        var response = await client.CreateControlAsync(dto);
    //        if (response?.Control != null)
    //        {
    //            var newEntity = response.Control;

    //            // Always store the new entity's own PK in the current scenario's context.
    //            context.Data["ControlId"] = newEntity.ControlId;

    //            // For simple, non-composite entities, add their ID to the shared context
    //            // so other test scenarios can randomly pick from this list.
    //            _testContext.AddToList("ControlIds", newEntity.ControlId);

    //            context.Logger.Information("Created Control: {Id}", newEntity.ControlId);
    //            return Response.Ok(statusCode: "201");
    //        }
    //        return Response.Fail(statusCode: "422", message: "Failed to create Control");
    //    }
    //    catch (Exception ex)
    //    {
    //        return Response.Fail(statusCode: "500", message: ex.Message);
    //    }
    //}

    //public static async Task<IResponse> UpdateControl(IScenarioContext context)
    //{
    //    var client = ClientFactory.CreateMerchantClient();
    //    try
    //    {
    //        if (!context.Data.TryGetValue("ControlId", out var idObj) || idObj is not Guid id)
    //            return Response.Fail(statusCode: "400", message: "ControlId not found");
    //        var get = await client.GetOneFullControlByIdAsync(id);
    //        if (get?.Control is null)
    //            return Response.Fail(statusCode: "404", message: "Control not found");
    //        var dto = get.Control;
    //        var updated = await client.UpdateControlAsync(dto);
    //        return updated != null
    //            ? Response.Ok()
    //            : Response.Fail(statusCode: "500", message: "Update Control failed");
    //    }
    //    catch (Exception ex)
    //    {
    //        return Response.Fail(statusCode: "500", message: ex.Message);
    //    }
    //}

    //public static async Task<IResponse> DeleteControl(IScenarioContext context)
    //{
    //    if (!context.Data.TryGetValue("ControlId", out var idObj) || idObj is not Guid id)
    //        return Response.Fail(statusCode: "400", message: "ControlId not found");

    //    var client = ClientFactory.CreateMerchantClient();
    //    try
    //    {
    //        await client.DeleteControlAsync(id);
    //        return Response.Ok();
    //    }
    //    catch (Exception ex)
    //    {
    //        return Response.Fail(statusCode: "500", message: ex.Message);
    //    }
    //}

    //public static async Task<IResponse> GetControlById(IScenarioContext context)
    //{
    //    var client = ClientFactory.CreateMerchantClient();
    //    try
    //    {
    //        if (!context.Data.TryGetValue("ControlId", out var idObj) || idObj is not Guid id)
    //            return Response.Fail(statusCode: "400", message: "ControlId not found");
    //        var response = await client.GetOneFullControlByIdAsync(id);
    //        return response?.Control != null
    //            ? Response.Ok()
    //            : Response.Fail(statusCode: "404", message: "Control not found");
    //    }
    //    catch (Exception ex)
    //    {
    //        return Response.Fail(statusCode: "500", message: ex.Message);
    //    }
    //}

    public static async Task<IResponse> GetPagedControls(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var pageNumber = context.Random.Next(1, 5);
            var pageSize  = context.Random.Next(10, 50);
            var resp = await client.GetPagedControlsListAsync(pageNumber, pageSize);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetPagedControls");

            var items = resp.Controls;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Paged Controls: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> SearchControls(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var terms = new[] { "test", "demo", "sample", "data", "entity" };
            var term  = terms[context.Random.Next(terms.Length)];
            var resp = await client.SearchControlsAsync(term);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from SearchControls");

            var count = resp.Count(); // empty is OK
            context.Logger.Information("Search '{Term}' returned {Count} results", term, count);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    public static async Task<IResponse> GetFilteredControls(IScenarioContext context)
    {
        var client = ClientFactory.CreateMerchantClient();
        try
        {
            var filters = new Dictionary<string, string>
            {
                { "search", "test" },
                { "status", "active" }
            };

            var sorting = new List<Sort>
            {
                new Sort { Field = "MerchantId", Direction = SortDirection.Descending }
            };

            var pageNumber = 1;
            var pageSize   = 20;
            var resp = await client.GetFilteredControlsListAsync(pageNumber, pageSize, filters, sorting);
            if (resp == null)
                return Response.Fail(statusCode: "502", message: "Null response from GetFilteredControlListAsync");

            var items = resp.Controls;
            var count = items == null ? 0 : items.Count();
            context.Logger.Information("Filtered Controls: {Count} items (page {Page}, size {Size})", count, pageNumber, pageSize);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            return Response.Fail(statusCode: "500", message: ex.Message);
        }
    }

    #endregion

    #region Utility Steps

    public static async Task<IResponse> Pause(IScenarioContext context, int milliseconds = 500)
    {
        try
        {
            await Task.Delay(milliseconds, context.ScenarioCancellationToken);
            return Response.Ok();
        }
        catch (OperationCanceledException)
        {
            // Scenario got canceled during the pause; treat as graceful completion
            return Response.Ok(statusCode: "499", message: "Pause canceled");
        }
    }

    public static Task<IResponse> ClearContext(IScenarioContext context)
    {
        context.Data.Clear();
        _testContext.Clear();
        return Task.FromResult<IResponse>(Response.Ok());
    }

    #endregion

}
