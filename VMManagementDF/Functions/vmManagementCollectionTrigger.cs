using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace VMManagementDF
{
    public static class vmManagementCollectionTrigger
    {
        [FunctionName("vmManagementCollectionTrigger")]

        public static async Task Run([CosmosDBTrigger(
            databaseName: "VMManagement",
            collectionName: "VM_Power_State",
            ConnectionStringSetting = "cosmosDBConnectionString",
            LeaseCollectionName = "leases2",
            CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input,
            [SignalR(HubName = "vmManagement")] IAsyncCollector<SignalRMessage> signalRMessages, ILogger log)
        {
            //if (input != null && input.Count > 0)
            //{
            //    log.LogInformation("COSMOSDB-TRIGGER-FUNCTION 1: Documents modified " + input.Count);
            //    log.LogInformation("COSMOSDB-TRIGGER-FUNCTION 2: First document Id " + input[0].Id);

            //    //return input;
            //}

            foreach (var cosmosItem in input)
            {
                await signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "vmStateUpdated",
                    Arguments = new[] { cosmosItem }
                });
            }
        }
    }
}
