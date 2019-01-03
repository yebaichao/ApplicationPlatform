using ApplicationPlatform.BLL;
using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Site.Attributes;
using ApplicationPlatform.Site.ViewModels.RoleInfoViewModels;
using ApplicationPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ApplicationPlatform.Site.Controllers
{
    [Description(No = 1, Name = "RoleManagement")]
    public class RoleInfoController : Controller
    {

        private IRoleInfoServiceRepository _roleInfoServiceRepository = new RoleInfoServiceRepository();
        private IPermissionServiceRepository _permissionServiceRepository = new PermissionServiceRepository();
        private IUserInfoServiceRepository _userInfoServiceRepository = new UserInfoServiceRepository();
        private DbContext SharingContext = ContextFactory.GetDbContext();

        [HttpGet]
        [RoleAuthorize]
        [Description(No = 1, Name = "AddRole")]
        public ActionResult AddRoleView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddRoleView(FormCollection formCollection)
        {
            try
            {
                RoleInfo roleAdd = new RoleInfo();
                roleAdd.CreateTime = DateTime.Now;
                roleAdd.RoleName = formCollection["RoleName"];
                roleAdd.RoleDescription = formCollection["Dsp"];
                _roleInfoServiceRepository.Add(roleAdd);
                _roleInfoServiceRepository.SaveChanges();
                JavaScriptSerializer Jss = new JavaScriptSerializer();
                var data = new { code = 1 };
                return Content(Jss.Serialize(data));
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
        [HttpPost]
        public void Save(int roleId, FormCollection formCollection)
        {
            try
            {
                var RoleTemp = from o in SharingContext.Set<RoleInfo>().Include(t => t.Permissions)
                            .Where(e => e.Id == roleId)
                               select o;
                var Role = RoleTemp.FirstOrDefault();
                if (Role.RoleName == "Administrator")
                { 
                    //return RedirectToAction("GetRoleInfoes", new { roleId = Role.Id });
                    Response.ContentType = "text/html";
                    Response.Write("<script>alert('Save successfully!')</script>");
                    Response.End();
                }
                var roleAllInfo = new RoleInfoViewModel(Role);
                var temp = roleAllInfo.GetAllActionName(Role);
                foreach (KeyValuePair<string, bool> _kvp in temp)
                {
                    bool MyCheckBox = formCollection[_kvp.Key].Contains("true");
                    if (MyCheckBox)
                    {
                        var permissions = SharingContext.Set<Permission>().Include("roleInfoes").Where(x => x.ActionName == _kvp.Key).FirstOrDefault();
                        Role.Permissions.Add(permissions);
                        SharingContext.SaveChanges();
                    }
                    else
                    {
                        var permissions = SharingContext.Set<Permission>().Include("roleInfoes").Where(x => x.ActionName == _kvp.Key).FirstOrDefault();
                        if (Role.Permissions.Contains(permissions))
                        {
                            Role.Permissions.Remove(permissions);
                            SharingContext.SaveChanges();
                        }
                    }

                }
                //return RedirectToAction("GetRoleInfoes", new { roleId = Role.Id });
                Response.ContentType = "text/html";
                Response.Write("<script>alert('Save successfully!')</script>");
                Response.End();		
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                //return View("~/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult Save(int roleId, string permissions)
        {
            try
            {
                var RoleTemp = from o in SharingContext.Set<RoleInfo>().Include(t => t.Permissions)
                            .Where(e => e.Id == roleId)
                               select o;
                var Role = RoleTemp.FirstOrDefault();
                if (Role.RoleName == "Administrator")
                {
                    return RedirectToAction("RoleManagement", new { roleId = Role.Id });
                    //Response.ContentType = "text/html";
                    //Response.Write("<script>alert('Save successfully!')</script>");
                    //Response.End();
                }
                var roleAllInfo = new RoleInfoViewModel(Role);
                var temp = roleAllInfo.GetAllActionName(Role);
                string[] permission = permissions.Split(',');
                foreach (string _kvp in permission)
                {
                    string[] trueOrfalse = _kvp.Split(':');
                    bool MyCheckBox = _kvp.Contains("true");
                    if (MyCheckBox)
                    {
                        string actionName = trueOrfalse[0];
                        var permissionsTemp = SharingContext.Set<Permission>().Include("roleInfoes").Where(x => x.ActionName == actionName).FirstOrDefault();
                        Role.Permissions.Add(permissionsTemp);
                        SharingContext.SaveChanges();
                    }
                    else
                    {
                        string actionName = trueOrfalse[0];
                        var permissionsTemp = SharingContext.Set<Permission>().Include("roleInfoes").Where(x => x.ActionName == actionName).FirstOrDefault();
                        if (Role.Permissions.Contains(permissionsTemp))
                        {
                            Role.Permissions.Remove(permissionsTemp);
                            SharingContext.SaveChanges();
                        }
                    }

                }
                return RedirectToAction("RoleManagement", new { roleId = Role.Id });
                //Response.ContentType = "text/html";
                //Response.Write("<script>alert('Save successfully!')</script>");
                //Response.End();
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "DeleteRole")]
        public ActionResult Delete(int _RoleId)
        {
            try
            {
                RoleInfo roleInfo = _roleInfoServiceRepository.Find(x => x.Id == _RoleId);
                _roleInfoServiceRepository.Delete(roleInfo);
                return RedirectToAction("RoleManagement");
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                return View("~/Views/Shared/Error.cshtml");
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "RoleManagement")]
        public ActionResult RoleManagement(int? roleId)
        {
            try
            {
                var RoleTemp = from o in SharingContext.Set<RoleInfo>().Include(t => t.Permissions)
                            .Include(t => t.UserInfoes)
                            .Where(e => e.Id == roleId)
                               select o;
                var Role = RoleTemp.FirstOrDefault();
                RoleInfoViewModel roleInfoViewModel = new RoleInfoViewModel(Role);
                return View(roleInfoViewModel);
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                return View("~/Views/Shared/Error.cshtml");
            }
        }

    }
}
