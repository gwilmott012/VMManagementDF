using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMManagementDF.Helpers
{
    public class AzureConnection
    {
        private IAzure azure;

        public List<VirtualMachineHelper> VirtualMachines => InitialiseList();
 
        public AzureConnection(string tenantID, string clientID, string clientSecret)
        {
            var authContextURL = "https://login.windows.net/" + tenantID;
            var authenticationContext = new AuthenticationContext(authContextURL);
            var credential = new ClientCredential(clientID, clientSecret);
            var result = authenticationContext.AcquireTokenAsync(resource: "https://management.azure.com/", clientCredential: credential);


            //https://docs.microsoft.com/en-us/dotnet/azure/dotnet-sdk-azure-authenticate?view=azure-dotnet#mgmt-auth
            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(clientID,
                clientSecret,
                tenantID,
                AzureEnvironment.AzureGlobalCloud);


            azure = Azure
                .Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();
        }

        private List<VirtualMachineHelper> InitialiseList()
        {
            var returnList = new List<VirtualMachineHelper>();

            foreach (var machine in azure.VirtualMachines.List())
            {
                returnList.Add(new VirtualMachineHelper(machine));
            }

            return returnList;
        }

        public string ToggleMachineState(string machineKey)
        {
            try
            {
                var targetMachineList = azure.VirtualMachines.List();

                var targetMachine = targetMachineList.FirstOrDefault(m => m.Inner.VmId == machineKey);

                if (targetMachine != null && !string.IsNullOrEmpty(targetMachine.PowerState.Value))
                {
                    var items = targetMachine.PowerState.Value.Split('/');

                    if (items.Length > 0)
                    {
                        switch (items[1].ToLower())
                        {
                            case "running":
                                Console.WriteLine("Toggling Off");
                                targetMachine.Deallocate();
                                break;
                            case "stopped":
                                Console.WriteLine("Toggling On");
                                targetMachine.Start();
                                break;
                            case "deallocated":
                                Console.WriteLine("Toggling On");
                                targetMachine.Start();
                                break;
                        }
                    }
                }

                return new VirtualMachineHelper(targetMachine).ToString();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Could not find target machine with key: {machineKey}. Message: {ex.Message}");
                throw;
            }
        }

    }
}
