using ApplicationPlatform.DAL;
using ApplicationPlatform.Site.Utilities;
using ApplicationPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplicationPlatform.Site.Attributes
{
    public class AntiReptilianAttribute : ActionFilterAttribute
    {
        public const string NoPermissionView = "NoPermission";
        private DbContext SharingContext = ContextFactory.GetDbContext();
        //
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string UserIp = GetIpHelper.GetWebClientIp();
            List<string> IpString = System.Web.HttpContext.Current.Session["IpString"] as List<string>;
            if (IpString != null)
            {
                if (IpString.Contains(UserIp))
                { filterContext.Result = new ViewResult { ViewName = NoPermissionView }; return; }
            }
            else { System.Web.HttpContext.Current.Session["IpString"] = new List<string>(); }

            string RequestNum = CacheHelper.GetCache("RequestNum") as string;
            if (string.IsNullOrEmpty(RequestNum))
            {
                CacheHelper.SetCache("RequestNum", "1", 180);
            }
            else
            {
                int num = Convert.ToInt32(RequestNum) + 1; ;
                if (num > 180)
                {
                    IpString = System.Web.HttpContext.Current.Session["IpString"] as List<string>;
                    IpString.Add(UserIp);
                    System.Web.HttpContext.Current.Session["IpString"] = IpString;
                    filterContext.Result = new ViewResult { ViewName = NoPermissionView }; return;
                }
                else { CacheHelper.SetCache("RequestNum", num.ToString(), 180); }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}