using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using VMManagementDF.DTO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VMManagementDF.Helpers
{
    public class CosmosDBRepository
    {
        private static string AccountURI = "https://vmmanagement-sigr-cosmos-111df5302b.documents.azure.com:443/";
        private static string AccountPK = "pwTpdfnCeP9nOQTqUK61dxhfKszjMN8Bx0mmrhmaTMFQcUEhqL06gZLRlBzsRcz6TUxG5kQwPXOsrHKHnuuzjg==";
        private static CosmosClient client;
        private static Container container;

        public CosmosDBRepository(string DatabaseId, string ContainerId)
        {
            
            client = new CosmosClient(AccountURI, AccountPK);
            container = client.GetContainer(DatabaseId, ContainerId);
        }

        public static async Task CreateItemAsync(VMEntity item, PartitionKey partitionKey)
        {
            await container.CreateItemAsync(item);
        }

        public async Task<List<VMEntity>> GetItemsAsync(ILogger log)
        {
            var results = new List<VMEntity>();

            try
            {
                log.LogInformation("CosmosDBRepository pos 1");

                var queryDefinition = new QueryDefinition("SELECT c.id, c.env, c.type, c.name, c.vm_id, c.apps, c.Power_State, c.VM_Type FROM c WHERE c.type = 'VM'");

                log.LogInformation("CosmosDBRepository pos 2");

                var query = container.GetItemQueryIterator<VMEntity>(queryDefinition);

                log.LogInformation("CosmosDBRepository pos 3");


                while (query.HasMoreResults)
                {
                    log.LogInformation("CosmosDBRepository query.HasMoreResults pos 1");
                    var result = await query.ReadNextAsync();

                    log.LogInformation("CosmosDBRepository query.HasMoreResults pos 2");

                    results.AddRange(result);
                }
            }
            catch (System.Exception ex)
            {
                log.LogError(ex, $"Error in Function GetItemsAsync. Message: {ex.Message}; InnerException: {ex.InnerException}");
            }

            return results;
        }  
        
        public static async Task UpdateItemAsync(VMEntity item, string id)
        {
            await container.ReplaceItemAsync(item, id);
        }
    }
}
