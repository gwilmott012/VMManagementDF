using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VMManagementDF.Helpers;

namespace VMManagementDF.Functions
{
    public static class vmManagementCosmosCollection
    {
        [FunctionName("vmManagementCosmosCollection")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get CosmosDB Connection
            var Repository = new CosmosDBRepository("VMManagement", "VM_Power_State", log);
            log.LogInformation("RUN-MESSAGE 1 (GW 1103): Got CosmosDB Connection");

            // Get CosmosDB Data
            var CosmosItems = await Repository.GetItemsAsync();
            log.LogInformation("RUN-MESSAGE 2: Got CosmosDB Items", CosmosItems);

            var jsonCosmosItems = JsonConvert.SerializeObject(CosmosItems);

            return jsonCosmosItems;
        }
    }
}
