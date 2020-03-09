using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace VMManagementDF
{
    public static class vmManagementCollectionTrigger
    {
        [FunctionName("vmManagementCollectionTrigger")]
        public static string Run([CosmosDBTrigger(
            databaseName: "VMManagement",
            collectionName: "VM_Power_State",
            ConnectionStringSetting = "cosmosDBConnectionString",
            LeaseCollectionName = "leases2",
            CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("COSMOSDB-TRIGGER-FUNCTION 1: Documents modified " + input.Count);
                log.LogInformation("COSMOSDB-TRIGGER-FUNCTION 2: First document Id " + input[0].Id);

                //return input;
            }

            return "Hi from CosmosDB Run Function"
        }
    }
}
