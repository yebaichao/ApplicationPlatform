using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Utilities.NodeModels
{
    public class Price
    {
        public string requirementId { get; set; }
        public string serialNumber { get; set; }
        public string product { get; set; }
        public string project { get; set; }
        public string type { get; set; }
        public string item { get; set; }
        public string subitem { get; set; }
        public string unitprice { get; set; }
    }
}