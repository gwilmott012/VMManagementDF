using Microsoft.Azure.Management.Compute.Fluent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VMManagementDF.Helpers
{
    public class VirtualMachineHelper
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string Key { get; set; }

        public VirtualMachineHelper(IVirtualMachine machine)
        {
            Name = machine.Name.ToTitleCase();
            State = "Updating";
            if (machine.PowerState != null)
            {
                State = machine.PowerState.Value.Substring(11).ToTitleCase();
            }
            Key = machine.Inner.VmId;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
