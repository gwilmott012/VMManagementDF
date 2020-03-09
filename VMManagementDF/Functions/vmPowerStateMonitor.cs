using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using VMManagementDF.Helpers;
using VMManagementDF.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VMManagementDF
{
    public static class vmPowerStateMonitor
    {
        [FunctionName("vmPowerStateMonitor")]
        public static async void Run([TimerTrigger("0/20 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                // Get Connections
                var Repository = new CosmosDBRepository("VMManagement", "VM_Power_State", log);
                var Connection = new AzureConnection("11c4bafa-00bb-43f4-a42d-ed6f2663fbaf",
                                                   "92c95553-455f-4b53-8bcf-e2dfe4dbcb0b",
                                                   "A4LbQRKmqT:2x4Iu/h=yM=pDBuu9VVA1", 
                                                   log);


                log.LogInformation("RUN-MESSAGE 1 (AW 0903): Got Connections");

                // Get Data
                var CosmosItems = await Repository.GetItemsAsync();


                log.LogInformation($"RUN-MESSAGE 2: Got {CosmosItems.Count} items from CosmosDB.");

                var AzureMachines = Connection.VirtualMachines;

                log.LogInformation($"RUN-MESSAGE 3: Got {AzureMachines.Count} items from Azure VMs.");

                // Check if any data needs updating
                var toBeUpdated = CompareItems(CosmosItems, AzureMachines, log);

                if (toBeUpdated.Any())
                {
                    log.LogInformation($"RUN-MESSAGE 5: Got {toBeUpdated.Count} items to be updated.");

                    foreach (var cosmosItem in toBeUpdated)
                    {
                        // Update any data requiring updates
                        await Repository.UpdateItemAsync(cosmosItem);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"ERROR-RUN in Function. Message: {ex.Message}; InnerException: {ex.InnerException}");
            }
        }

        private static List<VMEntity> CompareItems(List<VMEntity> cosmosItems, List<VirtualMachineHelper> azureMachines, ILogger log)
        {
            var toBeUpdated = new List<VMEntity>();



            try
            {

                foreach (var azureVM in azureMachines)
                {

                    //log.LogInformation($"COMPARE_ITEMS 1: vm name {azureVM.Name},  {azureVM.State}, {azureVM.Key}");

                    var cosmosVM = cosmosItems.FirstOrDefault(ci => ci.vm_id.ToLower().Equals(azureVM.Key.ToLower()));



                    if (cosmosVM != null)
                    {
                        //log.LogInformation($"COMPARE_ITEMS 2: vm from CosmosDB name {cosmosVM.name}, {cosmosVM.Power_State}");

                        if (cosmosVM.Power_State != azureVM.State)
                        {
                            //log.LogInformation($"COMPARE_ITEMS 3: Update  {cosmosVM.name}, {cosmosVM.Power_State}");

                            //Set the state of the CosmosVM item to the same as the AzureVM
                            toBeUpdated.Add(new VMEntity
                            {
                                apps = cosmosVM.apps,
                                Environment = cosmosVM.Environment,
                                id = cosmosVM.id,
                                name = cosmosVM.name,
                                Power_State = azureVM.State,
                                type = cosmosVM.type,
                                vm_id = cosmosVM.vm_id,
                                VM_Type = cosmosVM.VM_Type
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError(ex, $"ERROR-COMPAREITEMS in CompareItems Function. Message: {ex.Message}; InnerException: {ex.InnerException}");
            }

            return toBeUpdated;
        }
    }
}
