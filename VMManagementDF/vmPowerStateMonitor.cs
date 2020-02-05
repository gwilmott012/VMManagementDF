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
        public static async void Run([TimerTrigger("0/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                // Get Connections
                var Repository = new CosmosDBRepository("VMManagement", "VM_Power_State", log);
                var Connection = new AzureConnection("11c4bafa-00bb-43f4-a42d-ed6f2663fbaf",
                                                   "92c95553-455f-4b53-8bcf-e2dfe4dbcb0b",
                                                   "A4LbQRKmqT:2x4Iu/h=yM=pDBuu9VVA1");


                log.LogInformation("Message 1: Got Connections", Repository, Connection);

                // Get Data
                var CosmosItems = await Repository.GetItemsAsync();


                log.LogInformation($"Got {CosmosItems.Count} items from CosmosDB.");

                var AzureMachines = Connection.VirtualMachines;

                log.LogInformation($"Got {AzureMachines.Count} items from Azure VMs.");

                // Check if any data needs updating
                var toBeUpdated = CompareItems(CosmosItems, AzureMachines);
                if (toBeUpdated.Any())
                {
                    foreach (var item in toBeUpdated)
                    {
                        // Update any data requiring updates
                        await Repository.UpdateItemAsync(item);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error in Function. Message: {ex.Message}; InnerException: {ex.InnerException}");
            }
        }

        private static List<VMEntity> CompareItems(List<VMEntity> cosmosItems, List<VirtualMachineHelper> azureMachines)
        {
            var toBeUpdated = new List<VMEntity>();

            foreach (var item in azureMachines)
            {
                var update = cosmosItems.Where(ci => ci.vm_id == item.Key)
                                            .FirstOrDefault(ci => ci.Power_State != item.State);
                if (update != null)
                {
                    update.Power_State = item.State;
                    toBeUpdated.Add(update);
                }
            }

            return toBeUpdated;
        }
    }
}
