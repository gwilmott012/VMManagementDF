using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using VMManagementDF.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace VMManagementDF
{
    public static class vmManagementOrchestration
    {
        private static ILogger log;
        private static AzureConnection connection;


        //1. the entry point
        [FunctionName("vmManagementOrchestration_Run")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger logger)
        {
            log= logger;
            log.LogInformation($"LOG-INFO 1 - Run - Starting point ;)");

           

            //Get the id from the querystring
            string name = req.RequestUri.Query;

            // Function input comes from the request content...
            var requestBody = await req.Content.ReadAsAsync<List<string>>();

            VMIDS vmIDs = new VMIDS(requestBody);

            log.LogInformation($"LOG-INFO 3 - About to print out KEYS");
            foreach (var VirtualMachineID in requestBody)
            {
                log.LogInformation($"LOG-INFO 4 - Machine Vmid  from Request Body: {VirtualMachineID}");
                //await starter.StartNewAsync("vmManagementOrchestration", vmid);
            }


            //Get the connection to the Azure subscription
            connection = new AzureConnection("4bc6eea4-c3ed-4346-84a8-18f6868195ac",
                                      "9e497778-66dd-4947-828e-720f6595ff69",
                                      "=RST6396Q_h_rodQR]Lj7O86pckrc.-z");


            string instanceId = await starter.StartNewAsync("StartActivities", requestBody);
            log.LogInformation($"LOG-INFO 1 - Started Orchestration with ID = '{instanceId}'.");

            log.LogInformation($"LOG-INFO 1 - Run - Ending point");

            return starter.CreateCheckStatusResponse(req, "");


            //log.LogInformation($"LOG-INFO 3 - About to print out KEYS");
            //foreach (var VirtualMachineID in requestBody)
            //{
            //    vmid = VirtualMachineID;
            //    log.LogInformation($"LOG-INFO 4 - Machine Vmid: {vmid}");
            //    //await starter.StartNewAsync("vmManagementOrchestration", vmid);
            //}



            //log.LogInformation($"LOG-INFO 1 - About to print out HEADERS");
            //foreach (var item in req.Headers)
            //{
            //    int count = 0;
            //    foreach (var subItem in item.Value)
            //    {
            //        log.LogInformation($"LOG-INFO 2 - Key: {item.Key}#{count++}, Value: {subItem}");
            //    }

            //}

            //var parameters = req.RequestUri.Query.Trim().ToLower().Split(new char[] { '?', '&' });
            //vmid = parameters.FirstOrDefault(p => p.StartsWith("vmid")).Substring(5);


        }






        [FunctionName("StartActivities")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context, VMIDS vmIDs)
        {

            log.LogInformation($"LOG-INFO 2 - StartActivities - entry point");

            var outputs = new List<string>();
            foreach (var VirtualMachineID in vmIDs.vmList)
            {
    
                log.LogInformation($"LOG-INFO 2 - StartActivities - Machine Vmid: {VirtualMachineID}");
                outputs.Add(await context.CallActivityAsync<string>("DoTheWork", VirtualMachineID));
            }


            log.LogInformation($"LOG-INFO 2 - StartActivities - exit point");

            return outputs;
        }

        [FunctionName("DoTheWork")]
        public static string DoTheWork([ActivityTrigger] string vmid, ILogger log)
        {
            log.LogInformation($"LOG-INFO 3 - DoTheWork - Machine Vmid: {vmid}");
            return connection.ToggleMachineState(vmid);
        }

        
        public class VMIDS
        {
            public List<string> vmList = new List<string>();


            public VMIDS(List<string> _vmList)
            {
                vmList = _vmList;
            }
        }


        //private static string Toggle(string key)
        //{
        //    var machine = new AzureConnection("4bc6eea4-c3ed-4346-84a8-18f6868195ac",
        //                               "9e497778-66dd-4947-828e-720f6595ff69",
        //                               "=RST6396Q_h_rodQR]Lj7O86pckrc.-z").ToggleMachineState(key);

        //    return machine;
        //}
    }
}