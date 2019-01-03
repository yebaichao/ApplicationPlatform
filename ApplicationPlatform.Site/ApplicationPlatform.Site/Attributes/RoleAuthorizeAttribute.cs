using ApplicationPlatform.BLL;
using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Site.Utilities;
using ApplicationPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace ApplicationPlatform.Site.Attributes
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        public const string NoPermissionView = "NoPermission";
        private IUserInfoServiceRepository UserService = new UserInfoServiceRepository();
        private DbContext SharingContext = ContextFactory.GetDbContext();
        //
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //string UserIp = GetIpHelper.GetWebClientIp();
            //List<string> IpString = System.Web.HttpContext.Current.Session["IpString"] as List<string>;
            //if (IpString != null)
            //{
            //    if (IpString.Contains(UserIp))
            //    { filterContext.Result = new ViewResult { ViewName = NoPermissionView }; return; }
            //}
            //else { System.Web.HttpContext.Current.Session["IpString"] = new List<string>(); }

            //string RequestNum = CacheHelper.GetCache("RequestNum") as string;
            //if (string.IsNullOrEmpty(RequestNum))
            //{
            //    CacheHelper.SetCache("RequestNum", "1", 180);
            //}
            //else
            //{
            //    int num = Convert.ToInt32(RequestNum) + 1; ;
            //    if (num > 180)
            //    {
            //        IpString = System.Web.HttpContext.Current.Session["IpString"] as List<string>;
            //        IpString.Add(UserIp);
            //        System.Web.HttpContext.Current.Session["IpString"] = IpString;
            //        filterContext.Result = new ViewResult { ViewName = NoPermissionView }; return;
            //    }
            //    else { CacheHelper.SetCache("RequestNum", num.ToString(), 180); }
            //}

            bool hasPermission = false;
            string userName = WebSecurity.CurrentUserName;
            UserInfo userInfo = SharingContext.Set<UserInfo>().Where(x => x.UserName == userName).FirstOrDefault();
            if (userInfo != null)
            {
                var actionDescriptor = filterContext.ActionDescriptor;
                var controllerDescriptor = actionDescriptor.ControllerDescriptor;
                var controller = controllerDescriptor.ControllerName;
                var action = actionDescriptor.ActionName;

                try
                {
                    var _users = SharingContext.Set<UserInfo>().Include("RoleInfoes").Where(e => e.Id == userInfo.Id).FirstOrDefault();
                    foreach (var role in _users.RoleInfoes)
                    {
                        var _roles = SharingContext.Set<RoleInfo>().Include("Permissions").Where(e => e.Id == role.Id).FirstOrDefault();
                        hasPermission = _roles.Permissions.Any(x => x.Controller.ToLower() == controller.ToLower() && x.Action.ToLower() == action.ToLower());
                        if (hasPermission)
                        { break; }
                    }
                }
                catch (Exception ex) { ExceptionLogHelp.WriteLog(ex); }
            }

            if (hasPermission)
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new ViewResult { ViewName = NoPermissionView };
            }
        }
    }
}