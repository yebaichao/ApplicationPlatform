using ApplicationPlatform.Models;
using ApplicationPlatform.Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Site.ViewModels.RoleInfoViewModels
{
    public class RoleAddViewModel : BaseModel
    {
        public RoleInfo roleInfo { get; set; }
        public bool isAdminRole { get; set; }
        public RoleAddViewModel()
        {
            this.roleInfo = new RoleInfo();
            this.isAdminRole = false;
            this.PageSize = 10;
            this.CurrentPageNum = 1;
        }
    }
}