using System;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Contracts;
using PCIShield.PerformanceTests.Steps;
using PCIShield.PerformanceTests.Config;

namespace PCIShield.PerformanceTests.Scenarios;

public static class MerchantScenarios
{
    public static ScenarioProps CreateSmokeTestScenario()
    {
        return Scenario.Create("Merchant_SmokeTest", async context =>
        {
            var clear = await Step.Run("Clear_Context", context, async () =>
            {
                IResponse result = await MerchantSteps.ClearContext(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CLEAR_ERR", message: result.Message ?? "Clear context failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (clear.IsError) return clear;

            var create = await Step.Run("Create", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CREATE_ERR", message: result.Message ?? "Create failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "201", message: result.Message);
            });
            if (create.IsError) return create;

            var get = await Step.Run("GetById", context, async () =>
            {
                IResponse result = await MerchantSteps.GetMerchantById(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "GET_ERR", message: result.Message ?? "GetById failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (get.IsError) return get;

            var delete = await Step.Run("Delete", context, async () =>
            {
                IResponse result = await MerchantSteps.DeleteMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "DELETE_ERR", message: result.Message ?? "Delete failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            return delete;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetSmokeTestSimulations());
    }

    /// <summary>
    /// Full CRUD workflow for Merchant with light pacing.
    /// </summary>
    public static ScenarioProps CreateCrudScenario()
    {
        return Scenario.Create("Merchant_CRUD", async context =>
        {
            var clear = await Step.Run("Clear_Context", context, async () =>
            {
                IResponse result = await MerchantSteps.ClearContext(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CLEAR_ERR", message: result.Message ?? "Clear context failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (clear.IsError) return clear;
            var create = await Step.Run("Create", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CREATE_ERR", message: result.Message ?? "Create failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "201", message: result.Message);
            });
            if (create.IsError) return create;
            await Step.Run("Pause_1", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 200);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });
            var get1 = await Step.Run("GetById_1", context, async () =>
            {
                IResponse result = await MerchantSteps.GetMerchantById(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "GET_ERR", message: result.Message ?? "GetById failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (get1.IsError) return get1;
            await Step.Run("Pause_2", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 200);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });
            var update = await Step.Run("Update", context, async () =>
            {
                IResponse result = await MerchantSteps.UpdateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "UPDATE_ERR", message: result.Message ?? "Update failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (update.IsError) return update;
            await Step.Run("Pause_3", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 200);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });
            var get2 = await Step.Run("GetById_2", context, async () =>
            {
                IResponse result = await MerchantSteps.GetMerchantById(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "GET_ERR", message: result.Message ?? "GetById failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (get2.IsError) return get2;
            await Step.Run("Pause_4", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });
            var delete = await Step.Run("Delete", context, async () =>
            {
                IResponse result = await MerchantSteps.DeleteMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "DELETE_ERR", message: result.Message ?? "Delete failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            return delete;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetLoadTestSimulations());
    }

    /// <summary>
    /// Read-heavy scenario: paged, search, filtered queries for Merchant.
    /// </summary>
    public static ScenarioProps CreateQueryScenario()
    {
        return Scenario.Create("Merchant_Queries", async context =>
        {
            var paged = await Step.Run("GetPaged", context, async () =>
            {
                IResponse result = await MerchantSteps.GetPagedMerchants(context);
                return result.IsError
                    ? Response.Fail(statusCode: "PAGED_ERR", message: result.Message ?? "Paged query failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (paged.IsError) return paged;
            await Step.Run("Pause_1", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });
            var search = await Step.Run("Search", context, async () =>
            {
                IResponse result = await MerchantSteps.SearchMerchants(context);
                return result.IsError
                    ? Response.Fail(statusCode: "SEARCH_ERR", message: result.Message ?? "Search failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (search.IsError) return search;
            await Step.Run("Pause_2", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });
            var filtered = await Step.Run("GetFiltered", context, async () =>
            {
                IResponse result = await MerchantSteps.GetFilteredMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: "FILTER_ERR", message: result.Message ?? "Filtered query failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            return filtered;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetLoadTestSimulations());
    }

    /// <summary>
    /// Creates a Merchant and exercises a subset of related entities (children, joins).
    /// </summary>
    public static ScenarioProps CreateRelationshipScenario()
    {
        return Scenario.Create("Merchant_Relationships", async context =>
        {
            var clear = await Step.Run("Clear_Context", context, async () =>
            {
                IResponse result = await MerchantSteps.ClearContext(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CLEAR_ERR", message: result.Message ?? "Clear context failed")
                    : Response.Ok(statusCode: "CLEAR_OK", message: result.Message);
            });
            if (clear.IsError) return clear;


            var createRoot = await Step.Run("CreateRoot", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_ERR", message: result.Message ?? "Create root failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createRoot.IsError) return createRoot;
            await Step.Run("Pause_After_Root", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 150);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });

            var child1 = await Step.Run("Create_Assessment", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateAssessment(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_ERR", message: result.Message ?? "Child create failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (child1.IsError) return child1;
            await Step.Run("Pause_Child_1", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });

            var child2 = await Step.Run("Create_Asset", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateAsset(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_ERR", message: result.Message ?? "Child create failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (child2.IsError) return child2;
            await Step.Run("Pause_Child_2", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });

            var child3 = await Step.Run("Create_CompensatingControl", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateCompensatingControl(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_ERR", message: result.Message ?? "Child create failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (child3.IsError) return child3;
            await Step.Run("Pause_Child_3", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });

            var get = await Step.Run("GetById_Final", context, async () =>
            {
                IResponse result = await MerchantSteps.GetMerchantById(context);
                return result.IsError
                    ? Response.Fail(statusCode: "GET_ERR", message: result.Message ?? "GetById failed")
                    : Response.Ok(statusCode: "200", message: result.Message);
            });
            if (get.IsError) return get;
            var del = await Step.Run("DeleteRoot", context, async () =>
            {
                IResponse result = await MerchantSteps.DeleteMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: "DELETE_ERR", message: result.Message ?? "Delete failed")
                    : Response.Ok(statusCode: "200", message: result.Message);
            });
            return del;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetLoadTestSimulations());
    }

    /// <summary>
    /// Simulates the full master-detail page orchestration for a single Merchant aggregate, including all discovered relationships.
    /// It creates prerequisites (Parents, Lookups, Cascades), the main aggregate, and then builds out and verifies the entire relational graph.
    /// </summary>
    public static ScenarioProps CreateMasterDetailOrchestrationScenario()
    {
        return Scenario.Create("Merchant_MasterDetail_Orchestration", async context =>
        {
            // 1. Cleanup and Prerequisite Setup
            var clear = await Step.Run("1_Clear_Context", context, async () =>
            {
                IResponse result = await MerchantSteps.ClearContext(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CLEAR_ERR", message: result.Message ?? "Clear context failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (clear.IsError) return clear;

            // 2. Create ALL Prerequisites
            //var prereq_descendant_Control = await Step.Run("2.4_Prereq_Descendant_Control", context, async () =>
            //{
            //    IResponse result = await MerchantSteps.CreateControl(context);
            //    return result.IsError ? Response.Fail(message: result.Message) : Response.Ok();
            //});
            //if (prereq_descendant_Control.IsError) return prereq_descendant_Control;

            // 3. Create the Main Aggregate
            var createMain = await Step.Run("3_Create_Main_Merchant", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_MAIN_ERR", message: result.Message ?? "Create main failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createMain.IsError) return createMain;
            await Step.Run("Pause_After_Main_Create", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });

            // 4. Create all Child Entities, propagating the main aggregate's ID
            var createChild_Assessment = await Step.Run("4_Create_Child_Assessment", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateAssessment(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_Assessment.IsError) return createChild_Assessment;
            var createChild_Asset = await Step.Run("4_Create_Child_Asset", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateAsset(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_Asset.IsError) return createChild_Asset;
            var createChild_CompensatingControl = await Step.Run("4_Create_Child_CompensatingControl", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateCompensatingControl(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_CompensatingControl.IsError) return createChild_CompensatingControl;
            var createChild_ComplianceOfficer = await Step.Run("4_Create_Child_ComplianceOfficer", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateComplianceOfficer(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_ComplianceOfficer.IsError) return createChild_ComplianceOfficer;
            var createChild_CryptographicInventory = await Step.Run("4_Create_Child_CryptographicInventory", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateCryptographicInventory(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_CryptographicInventory.IsError) return createChild_CryptographicInventory;
            var createChild_Evidence = await Step.Run("4_Create_Child_Evidence", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateEvidence(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_Evidence.IsError) return createChild_Evidence;
            var createChild_NetworkSegmentation = await Step.Run("4_Create_Child_NetworkSegmentation", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateNetworkSegmentation(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_NetworkSegmentation.IsError) return createChild_NetworkSegmentation;
            var createChild_PaymentChannel = await Step.Run("4_Create_Child_PaymentChannel", context, async () =>
            {
                IResponse result = await MerchantSteps.CreatePaymentChannel(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_PaymentChannel.IsError) return createChild_PaymentChannel;
            var createChild_ServiceProvider = await Step.Run("4_Create_Child_ServiceProvider", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateServiceProvider(context);
                return result.IsError
                    ? Response.Fail(statusCode: "CREATE_CHILD_ERR", message: result.Message ?? "Create child failed")
                    : Response.Ok(statusCode: "201", message: result.Message);
            });
            if (createChild_ServiceProvider.IsError) return createChild_ServiceProvider;

            // 5. Create all Join Entities, linking the main aggregate to the prerequisites

            // 6. Read and Verify the entire graph
            var getGraph = await Step.Run("6.1_Get_Full_Graph", context, async () =>
            {
                IResponse result = await MerchantSteps.GetMerchantById(context);
                return result.IsError
                    ? Response.Fail(statusCode: "GET_GRAPH_ERR", message: result.Message ?? "Get graph failed")
                    : Response.Ok(statusCode: "200", message: result.Message);
            });
            if (getGraph.IsError) return getGraph;
            await Step.Run("Pause_After_Graph_Get", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 100);
                return result.IsError
                    ? Response.Fail(statusCode: "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: "PAUSE_OK", message: result.Message);
            });

            // 7. Update the main aggregate and one of each related entity type
            var updateMain = await Step.Run("7_Update_Main_Merchant", context, async () =>
            {
                IResponse result = await MerchantSteps.UpdateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: "UPDATE_MAIN_ERR", message: result.Message ?? "Update main failed")
                    : Response.Ok(statusCode: "200", message: result.Message);
            });
            if (updateMain.IsError) return updateMain;
            var updateChild_Assessment = await Step.Run("8_Update_Child_Assessment", context, async () =>
            {
                IResponse result = await MerchantSteps.UpdateAssessment(context);
                return result.IsError
                    ? Response.Fail(statusCode: "UPDATE_CHILD_ERR", message: result.Message ?? "Update child failed")
                    : Response.Ok(statusCode: "200", message: result.Message);
            });
            if (updateChild_Assessment.IsError) return updateChild_Assessment;

            // 8. Delete one of each related entity type to test deletion logic
            var deleteChild_Assessment = await Step.Run("10_Delete_Child_Assessment", context, async () =>
            {
                IResponse result = await MerchantSteps.DeleteAssessment(context);
                return result.IsError
                    ? Response.Fail(statusCode: "DELETE_CHILD_ERR", message: result.Message ?? "Delete child failed")
                    : Response.Ok(statusCode: "200", message: result.Message);
            });
            if (deleteChild_Assessment.IsError) return deleteChild_Assessment;

            // 9. Final Cleanup: Delete the main aggregate
            var deleteMain = await Step.Run("12_Delete_Main_Merchant", context, async () =>
            {
                IResponse result = await MerchantSteps.DeleteMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: "DELETE_MAIN_ERR", message: result.Message ?? "Delete main failed")
                    : Response.Ok(statusCode: "200", message: result.Message);
            });
            return deleteMain;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetLoadTestSimulations());
    }

    /// <summary>
    /// Focuses on cascade lookups and dependent paging.
    /// </summary>
    public static ScenarioProps CreateCascadeScenario()
    {
        return Scenario.Create("Merchant_Cascades", async context =>
        {
            // No cascades for this aggregate, treat as a no-op
            return Response.Ok(statusCode: "NO_CASCADES", message: "No cascades configured");
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetLoadTestSimulations());
    }

    /// <summary>
    /// Stress: repeated read-heavy operations with intermittent writes.
    /// </summary>
    public static ScenarioProps CreateStressScenario()
    {
        return Scenario.Create("Merchant_Stress", async context =>
        {
            var create = await Step.Run("Prime_Create", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CREATE_ERR", message: result.Message ?? "Prime create failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "201", message: result.Message);
            });
            if (create.IsError) return create;

            for (var i = 0; i < 3; i++)
            {
                var paged = await Step.Run($"GetPaged_{i}", context, async () =>
                {
                    IResponse result = await MerchantSteps.GetPagedMerchants(context);
                    return result.IsError
                        ? Response.Fail(statusCode: result.StatusCode ?? "PAGED_ERR", message: result.Message ?? "Paged query failed")
                        : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
                });
                if (paged.IsError) return paged;
                await Step.Run($"Pause_A_{i}", context, async () =>
                {
                    IResponse result = await MerchantSteps.Pause(context, 50);
                    return result.IsError
                        ? Response.Fail(statusCode: result.StatusCode ?? "PAUSE_ERR", message: result.Message ?? "Pause failed")
                        : Response.Ok(statusCode: result.StatusCode ?? "PAUSE_OK", message: result.Message);
                });
                var byId = await Step.Run($"GetById_{i}", context, async () =>
                {
                    IResponse result = await MerchantSteps.GetMerchantById(context);
                    return result.IsError
                        ? Response.Fail(statusCode: result.StatusCode ?? "GET_ERR", message: result.Message ?? "Get failed")
                        : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
                });
                if (byId.IsError) return byId;
                await Step.Run($"Pause_B_{i}", context, async () =>
                {
                    IResponse result = await MerchantSteps.Pause(context, 50);
                    return result.IsError
                        ? Response.Fail(statusCode: result.StatusCode ?? "PAUSE_ERR", message: result.Message ?? "Pause failed")
                        : Response.Ok(statusCode: result.StatusCode ?? "PAUSE_OK", message: result.Message);
                });
            }

            var upd = await Step.Run("Occasional_Update", context, async () =>
            {
                IResponse result = await MerchantSteps.UpdateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "UPDATE_ERR", message: result.Message ?? "Update failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            return upd;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetStressTestSimulations());
    }

    /// <summary>
    /// Concurrency/iterations micro-benchmark of core operations.
    /// </summary>
    public static ScenarioProps CreateConcurrencyScenario()
    {
        return Scenario.Create("Merchant_Concurrency", async context =>
        {
            var create = await Step.Run("Create", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CREATE_ERR", message: result.Message ?? "Create failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "201", message: result.Message);
            });
            if (create.IsError) return create;
            await Step.Run("Pause_1", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 50);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "PAUSE_OK", message: result.Message);
            });
            var get = await Step.Run("GetById", context, async () =>
            {
                IResponse result = await MerchantSteps.GetMerchantById(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "GET_ERR", message: result.Message ?? "Get failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (get.IsError) return get;
            var update = await Step.Run("Update", context, async () =>
            {
                IResponse result = await MerchantSteps.UpdateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "UPDATE_ERR", message: result.Message ?? "Update failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            return update;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetIterationsTestSimulations());
    }

    /// <summary>
    /// Soak: long-running light load of mixed reads to surface leaks or slow degradation.
    /// </summary>
    public static ScenarioProps CreateSoakTestScenario()
    {
        return Scenario.Create("Merchant_Soak", async context =>
        {
            var paged = await Step.Run("GetPaged", context, async () =>
            {
                IResponse result = await MerchantSteps.GetPagedMerchants(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "PAGED_ERR", message: result.Message ?? "Paged query failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (paged.IsError) return paged;
            await Step.Run("Pause_1", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 150);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "PAUSE_OK", message: result.Message);
            });
            var filtered = await Step.Run("GetFiltered", context, async () =>
            {
                IResponse result = await MerchantSteps.GetFilteredMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "FILTER_ERR", message: result.Message ?? "Filtered query failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (filtered.IsError) return filtered;
            await Step.Run("Pause_2", context, async () =>
            {
                IResponse result = await MerchantSteps.Pause(context, 150);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "PAUSE_ERR", message: result.Message ?? "Pause failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "PAUSE_OK", message: result.Message);
            });
            var search = await Step.Run("Search", context, async () =>
            {
                IResponse result = await MerchantSteps.SearchMerchants(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "SEARCH_ERR", message: result.Message ?? "Search failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            return search;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetSoakTestSimulations());
    }

    /// <summary>
    /// End-to-end performance path including a domain-specific perf hook if available.
    /// </summary>
    public static ScenarioProps CreatePerformanceScenario()
    {
        return Scenario.Create("Merchant_Performance", async context =>
        {
            var create = await Step.Run("Create", context, async () =>
            {
                IResponse result = await MerchantSteps.CreateMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "CREATE_ERR", message: result.Message ?? "Create failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "201", message: result.Message);
            });
            if (create.IsError) return create;
            var perf = await Step.Run("Domain_Perf", context, async () =>
            {
                IResponse result = await MerchantSteps.RunMerchantPerformanceTests(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "PERF_ERR", message: result.Message ?? "Perf hook failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            if (perf.IsError) return perf;
            var del = await Step.Run("Delete", context, async () =>
            {
                IResponse result = await MerchantSteps.DeleteMerchant(context);
                return result.IsError
                    ? Response.Fail(statusCode: result.StatusCode ?? "DELETE_ERR", message: result.Message ?? "Delete failed")
                    : Response.Ok(statusCode: result.StatusCode ?? "200", message: result.Message);
            });
            return del;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(LoadSimulationProfiles.GetLoadTestSimulations());
    }

}
