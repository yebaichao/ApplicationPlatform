using ApplicationPlatform.BLL;
using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Site.ViewModels.RoleInfoViewModels
{
    public class RoleInfoViewModel
    {
        private IRoleInfoServiceRepository RoleInfoService = new RoleInfoServiceRepository();
        private IPermissionServiceRepository PermissionService = new PermissionServiceRepository();
        private IUserInfoServiceRepository UserService = new UserInfoServiceRepository();
        private DbContext SharingContext = ContextFactory.GetDbContext();
        /// <summary>
        /// 数据库中的所有角色
        /// </summary>
        public List<RoleInfo> RoleAll { get; set; }
        /// <summary>
        /// 所选择的角色
        /// </summary>
        public RoleInfo SelectedRole { get; set; }
        /// <summary>
        /// 数据库中的所有许可权限
        /// </summary>
        public List<Permission> Permissions { get; set; }
        /// <summary>
        /// 数据库中的所有Controller
        /// </summary>
        public List<string> ControllerName { get; set; }
        /// <summary>
        /// 数据库中的所有Action
        /// </summary>
        public List<string> ActionName { get; set; }
        /// <summary>
        /// ActionName和Checked
        /// </summary>
        public Dictionary<string, bool> actionName { get; set; }
        public KeyValuePair<string, bool> kvp { get; set; }
        public int CurrentPageNum = 1;
        public int PageSize = 10;
        /// <summary>
        /// 数据库中的所有权限大标题
        /// </summary>
        string[] _controllerNameAll = { "RoleManagement", "UserManagement", "RequireManagement","PermissionManagement","PriceManagement" };
        /// <summary>
        /// 初始化所选择的角色
        /// </summary>
        /// <param name="selectedRole"></param0>
        public RoleInfoViewModel(RoleInfo selectedRole)
        {

            RoleAll = RoleInfoService.FindAll(x => x.Id != null).OrderBy(x => x.RoleName).ToList();
            Permissions = PermissionService.FindAll(x => x.Id != null).ToList();
            string[] a = { "RoleManagement", "UserManagement", "RequireManagement", "PermissionManagement","PriceManagement" };
            ControllerName = a.ToList();
            DbContext abContext = ContextFactory.GetDbContext();
            if (selectedRole == null)
            {
                var RoleTemp = from o in SharingContext.Set<RoleInfo>().Include(t => t.Permissions)
                               .Where(e => e.RoleName == "Administrator")
                               select o;
                var Role = RoleTemp.FirstOrDefault();
                SelectedRole = Role;
            }
            else
            {
                SelectedRole = selectedRole;
            }

        }

        /// <summary>
        /// 用控制器名查找对应的Action，获取所选择的角色的所有Action许可
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="roleInfo"></param>
        /// <returns></returns>
        public Dictionary<string, bool> GetActionName(string controllerName, RoleInfo roleInfo)
        {
            bool a = false;
            List<Permission> _permission = new List<Permission>();
            //获取当前控制器的所有Permission
            _permission = PermissionService.FindAll(x => x.ControllerName == controllerName).ToList();
            //定义装载action和bool的key和value
            Dictionary<string, bool> _actionName = new Dictionary<string, bool>();

            var context = ContextFactory.GetDbContext();
            //获取所选择的角色对应的所有Permission
            var _roles = context.Set<RoleInfo>().Include("Permissions").Where(e => e.Id == roleInfo.Id).FirstOrDefault();
            foreach (Permission permission in _permission)
            {
                foreach (Permission permissionDic in _roles.Permissions)
                {
                    if (permissionDic.ActionName == permission.ActionName)
                    { a = true; break; }
                    else { a = false; }
                }

                _actionName.Add(permission.ActionName, a);
            }
            actionName = _actionName;
            return _actionName;
        }
        public Dictionary<string, bool> GetAllActionName(RoleInfo roleInfo)
        {
            string[] _controllerName = this._controllerNameAll;
            Dictionary<string, bool> _actionName = new Dictionary<string, bool>();
            foreach (string controllerName in _controllerName)
            {
                bool a = false;
                List<Permission> _permission = new List<Permission>();
                //获取当前控制器的所有Permission
                _permission = PermissionService.FindAll(x => x.ControllerName == controllerName).ToList();
                //定义装载action和bool的key和value


                var context = ContextFactory.GetDbContext();
                //获取所选择的角色对应的所有Permission
                var _roles = context.Set<RoleInfo>().Include("Permissions").Where(e => e.Id == roleInfo.Id).FirstOrDefault();
                foreach (Permission permission in _permission)
                {
                    foreach (Permission permissionDic in _roles.Permissions)
                    {
                        if (permissionDic.ActionName == permission.ActionName)
                        { a = true; break; }
                        else { a = false; }
                    }

                    _actionName.Add(permission.ActionName, a);
                }
            }
            actionName = _actionName;
            return _actionName;
        }
    }
}