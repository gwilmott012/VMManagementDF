using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VMManagementDF.Helpers;

namespace StartServers2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VirtualMachineController : ControllerBase
    {

        // GET /
        [HttpGet]
        public IEnumerable<VirtualMachineHelper> Get()
        {
            var conn = new AzureConnection("11c4bafa-00bb-43f4-a42d-ed6f2663fbaf",
               "92c95553-455f-4b53-8bcf-e2dfe4dbcb0b",
               "A4LbQRKmqT:2x4Iu/h=yM=pDBuu9VVA1");

            var GetMachines = conn.VirtualMachines;

            //var GetMachines = new AzureConnection("4bc6eea4-c3ed-4346-84a8-18f6868195ac",
            //                            "9e497778-66dd-4947-828e-720f6595ff69",
            //                            "=RST6396Q_h_rodQR]Lj7O86pckrc.-z").VirtualMachines;

            return GetMachines;
        }
    }
}
