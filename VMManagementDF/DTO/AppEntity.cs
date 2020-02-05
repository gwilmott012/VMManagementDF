using System;
using System.Collections.Generic;
using System.Text;


namespace VMManagementDF.DTO
{
    public class AppEntity
    {
        public string appNameShort { get; set; }

        public string appNameLong { get; set; }

        //[JsonIgnore()]
        //public List<VMEntity> Servers { get; set; }
    }
}
