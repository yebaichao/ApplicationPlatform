using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Utilities.NodeModels
{
    public class User
    {
        public string SerialNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WeChat { get; set; }
        public string UserRole { get; set; }
    }
}