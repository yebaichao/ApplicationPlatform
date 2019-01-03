using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Utilities.NodeModels
{
    public class ProcessingView
    {
        public string serialNumber { get; set; }
        public string project { get; set; }
        public string item { get; set; }
        public string type { get; set; }
        public string stage { get; set; }
        public string site { get; set; }
        public string progress { get; set; }
        public string quantity { get; set; }
        public string comment { get; set; }
        public string operation { get; set; }
        public string approver1 { get; set; }
        public string approver2 { get; set; }
        public string requirementId { get; set; }
        public string arrangeUser { get; set; }
        public string ETD { get; set; }
        public string ATD { get; set; }
        public string product { get; set; }
        public string subitem { get; set; }
        public string postuser { get; set; }
        public string createtime { get; set; }
        public string savetime { get; set; }
        public string unitprice { get; set; }
        public string totalprice { get; set; }
    }
}