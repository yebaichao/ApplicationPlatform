using ApplicationPlatform.BLL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using ApplicationPlatform.Site.Attributes;
using ApplicationPlatform.Site.Controllers;
using ApplicationPlatform.Utilities;

namespace ApplicationPlatform.Site.Utilities
{
    public class InitializeHelper
    {
        public static void Index()
        {
            try
            {
                IRoleInfoServiceRepository RoleInfoService = new RoleInfoServiceRepository();
                IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
                IPermissionServiceRepository PermissionService = new PermissionServiceRepository();
                #region init permission
                CreatePermission(new RoleInfoController());
                CreatePermission(new ApplicationInfoController());
                CreatePermission(new UserInfoController());
                CreatePermission(new PermissionController());
                CreatePermission(new PriceInfoController());
                #endregion

                var allDefinedPermissions = PermissionService.FindAll(x => x.Id != null);
                #region 管理员角色初始化
                if (!RoleInfoService.Exist(x => x.RoleName == "Administrator"))
                {
                    UserInfo adminUser = new UserInfo()
                    {
                        UserName = "admin",
                        Sex = "Secrecy",
                        Email = "baichao.ye@hexagon.com",
                        RegistTime = System.DateTime.Now
                    };
                    //
                    WebSecurity.CreateUserAndAccount(adminUser.UserName, "123456");
                    var adminPermissions = new List<Permission>();
                    adminPermissions = allDefinedPermissions.ToList();

                    RoleInfo adminRole = new RoleInfo
                    {
                        RoleName = "Administrator",
                        RoleDescription = "Administrator",
                        Permissions = adminPermissions,
                        CreateTime = DateTime.Now,
                    };
                    adminRole.UserInfoes.Add(adminUser);

                    //增加管理员角色
                    RoleInfoService.Add(adminRole);
                    int roleCount = RoleInfoService.SaveChanges();

                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
            }
        }
        private static void CreatePermission(Controller customController)
        {
            try
            {
                IPermissionServiceRepository roleApi = new PermissionServiceRepository();

                var controllerName = "";
                var controller = ""; var controllerNo = 0;
                var actionName = ""; var action = ""; var actionNo = 0;
                var controllerDesc = new KeyValuePair<string, int>();
                var controllerType = customController.GetType();
                controller = controllerType.Name.Replace("Controller", "");
                controllerDesc = Getdesc(controllerType);
                if (!string.IsNullOrEmpty(controllerDesc.Key))
                {
                    controllerName = controllerDesc.Key;
                    controllerNo = controllerDesc.Value;
                    foreach (var m in controllerType.GetMethods())
                    {
                        var mDesc = GetPropertyDesc(m);
                        if (string.IsNullOrEmpty(mDesc.Key)) continue;
                        action = m.Name;
                        actionName = mDesc.Key;
                        actionNo = mDesc.Value;
                        roleApi.Add(new Permission { ActionNo = actionNo, ControllerNo = controllerNo, ActionName = actionName, ControllerName = controllerName, Controller = controller, Action = action, RoleInfoes = new List<RoleInfo>() });
                        int SaveCount = roleApi.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
            }
        }
        private static KeyValuePair<string, int> Getdesc(Type type)
        {
            var descriptionAttribute = (DescriptionAttribute)(type.GetCustomAttributes(false).FirstOrDefault(x => x is DescriptionAttribute));
            if (descriptionAttribute == null) return new KeyValuePair<string, int>();
            return new KeyValuePair<string, int>(descriptionAttribute.Name, descriptionAttribute.No);
        }
        private static KeyValuePair<string, int> GetPropertyDesc(System.Reflection.MethodInfo type)
        {
            var descriptionAttribute = (DescriptionAttribute)(type.GetCustomAttributes(false).FirstOrDefault(x => x is DescriptionAttribute));
            if (descriptionAttribute == null) return new KeyValuePair<string, int>();
            return new KeyValuePair<string, int>(descriptionAttribute.Name, descriptionAttribute.No);
        }
    }
}