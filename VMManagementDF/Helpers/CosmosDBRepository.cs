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
        private static ILogger log;
        private static CosmosClient client;
        private static Container container;

        public CosmosDBRepository(string DatabaseId, string ContainerId, ILogger _log)
        {
            log = _log;
            client = new CosmosClient(AccountURI, AccountPK);
            container = client.GetContainer(DatabaseId, ContainerId);
        }

        public static async Task CreateItemAsync(VMEntity item)
        {
            await container.CreateItemAsync(item);
        }

        public async Task<List<VMEntity>> GetItemsAsync()
        {
            var results = new List<VMEntity>();

            try
            {
                var queryDefinition = new QueryDefinition("SELECT c.id, c.env, c.type, c.name, c.vm_id, c.apps, c.Power_State, c.VM_Type FROM c WHERE c.type = 'VM'");

                var query = container.GetItemQueryIterator<VMEntity>(queryDefinition);
                
                while (query.HasMoreResults)
                {
                    var result = await query.ReadNextAsync();

                    results.AddRange(result);
                }
            }
            catch (System.Exception ex)
            {
                log.LogError(ex, $"Error in Function GetItemsAsync. Message: {ex.Message}; InnerException: {ex.InnerException}");
            }

            return results;
        }  
        
        public async Task UpdateItemAsync(VMEntity item)
        {
            try
            {
                log.LogInformation($"Updating {item.name} with new PowerState {item.Power_State}");
                await container.ReplaceItemAsync(item, item.id);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex, $"Error in Function GetItemsAsync. Message: {ex.Message}; InnerException: {ex.InnerException}");
            } 
        }
    }
}
