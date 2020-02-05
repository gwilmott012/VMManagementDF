using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using VMManagementDF.Helpers;
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
                var Repository = new CosmosDBRepository("VMManagement", "VM_Power_State");
                log.LogInformation("Message 1: Got Container");
                var Items = await Repository.GetItemsAsync(log);
                log.LogInformation("Message 2: Got Data from DB");
                LogItemsToString(log, Items);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error in Function. Message: {ex.Message}; InnerException: {ex.InnerException}");
            }

            try
            {
                var Connection = new AzureConnection("11c4bafa-00bb-43f4-a42d-ed6f2663fbaf",
                           "92c95553-455f-4b53-8bcf-e2dfe4dbcb0b",
                           "A4LbQRKmqT:2x4Iu/h=yM=pDBuu9VVA1");

                log.LogInformation("Message 1: Got Azure Connection");
                var Items = Connection.VirtualMachines;
                log.LogInformation("Message 2: Got Machine Data from Azure Connection");
                LogItemsToString(log, Items);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error in Function. Message: {ex.Message}; InnerException: {ex.InnerException}");
            }
        }

        private static void LogItemsToString<T>(ILogger log, List<T> Items)
        {
            foreach (var item in Items)
            {
                log.LogInformation(item.ToString());
            }
        }
    }
}
