using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace VMManagementDF.DTO
{
    public class VMEntity
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("env")]
        public string Environment { get; set; }

        public string type { get; set; }

        public string name { get; set; }

        public string vm_id { get; set; }

        public List<AppEntity> apps { get; set; }

        public string Power_State { get; set; }

        public string VM_Type { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
            //    return JsonSerializer.Serialize(this, Type.GetType());
            //}
        }
    }
}
