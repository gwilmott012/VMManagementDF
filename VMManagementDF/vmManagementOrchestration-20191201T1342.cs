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
using System.Text;

namespace VMManagementDF
{
    public static class vmManagementOrchestration
    {
        private static string vmid;
        private static AzureConnection connection;

        //Funtion 1 - Entrypoint

        [FunctionName("vmManagementOrchestration_Run")]
        public static async Task<HttpResponseMessage> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
           [OrchestrationClient]DurableOrchestrationClient starter,
           ILogger log)
        {
            var data = await req.Content.ReadAsAsync<VirtualMachineIds>();

            //log.LogInformation($"Data = '{data.ToString()}'.");

            string instanceId = await starter.StartNewAsync("vmManagementOrchestration", data);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);


        }


        [FunctionName("vmManagementOrchestration")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {

            log.LogInformation($"************** RunOrchestrator method executing ********************");

            VirtualMachineIds virtualMachineIds = context.GetInput<VirtualMachineIds>();

            // Fanning out
            log.LogInformation($"************** Fanning out ********************");


            var parallelActivities = new List<Task<string>>();
            foreach (var vmid in virtualMachineIds.VirtualMachineIdList)
            {

                log.LogInformation($"************** vmid = " + vmid);

                // Start a new activity function and capture the task reference
                Task<string> task = context.CallActivityAsync<string>("vmManagementOrchestration_ToggleVM", vmid);

                // Store the task reference for later
                parallelActivities.Add(task);
            }


            // Wait until all the activity functions have done their work
            log.LogInformation($"************** 'Waiting' for parallel results ********************");
            await Task.WhenAll(parallelActivities);
            log.LogInformation($"************** All activity functions complete ********************");

            // Now that all parallel activity functions have completed,
            // fan in AKA aggregate the results, in this case into a single
            // string using a StringBuilder
            log.LogInformation($"************** fanning in ********************");
            var sb = new StringBuilder();
            foreach (var completedParallelActivity in parallelActivities)
            {
                sb.AppendLine(completedParallelActivity.Result);
            }

            return sb.ToString();

        }

        [FunctionName("vmManagementOrchestration_ToggleVM")]
        public static string ToggleVM([ActivityTrigger] string key)
        {
            //return Toggle(key);
            var machine = new AzureConnection("4bc6eea4-c3ed-4346-84a8-18f6868195ac",
                                       "9e497778-66dd-4947-828e-720f6595ff69",
                                       "=RST6396Q_h_rodQR]Lj7O86pckrc.-z").ToggleMachineState(key);

            return machine;
        }

       
    }

    public class VirtualMachineIds
    {
        public List<string> VirtualMachineIdList;

        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    foreach (var item in VirtualMachineIdList)
        //    {
        //        sb.AppendLine(item);
        //    }

        //    return sb.ToString();
        //}
    }
}