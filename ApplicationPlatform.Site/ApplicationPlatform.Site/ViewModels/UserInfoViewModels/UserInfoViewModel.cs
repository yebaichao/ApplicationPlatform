using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Site.ViewModels.UserInfoViewModels
{
    public class UserInfoViewModel
    {
        public UserInfo UserInfo { get; set; }
        public int CurrentPageNum { get; set; }
        public int PageSize { get; set; }
        public string Male { get; set; }
        public string Female { get; set; }
        public string Secrecy { get; set; }
    }
}