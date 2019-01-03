using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ApplicationPlatform.Site.Filters;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.BLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Site.Utilities;
using ApplicationPlatform.DAL;
using System.Linq.Expressions;
using ApplicationPlatform.Site.ViewModels.UserInfoViewModels;
using System.Web.Script.Serialization;
using ApplicationPlatform.Utilities.NodeModels;
using ApplicationPlatform.Site.Attributes;
using ApplicationPlatform.Utilities;
using System.Configuration;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Data.Entity;

namespace ApplicationPlatform.Site.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult LoginView(string returnUrl)
        {
            IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
            UserInfo userInfo = UserInfoService.Find(x => x.UserName == "admin");
            if (userInfo == null)
            { InitializeHelper.Index(); }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginView(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                DbContext SharingContext = ContextFactory.GetDbContext();
                UserInfo userInfo = SharingContext.Set<UserInfo>().Include("RoleInfoes").Where(x => x.UserName == model.UserName).FirstOrDefault();
                SessionValue sessionValue = new SessionValue();
                foreach(RoleInfo item in userInfo.RoleInfoes)
                {
                    if (item.RoleName == "Administrator")
                    { sessionValue.HasEditButton = true; }
                    RoleInfo role =SharingContext.Set<RoleInfo>().Include("Permissions").Where(x => x.Id == item.Id).FirstOrDefault();
                    foreach(Permission permission in role.Permissions)
                    {
                        if (permission.Action == "TechnicalApprove" || permission.Action == "TechnicalApproves")
                        {
                            sessionValue.HasTechAppprove = true;
                        }
                        if (permission.Action == "CommercialApprove" || permission.Action == "CommercialApproves")
                        {
                            sessionValue.HasComAppprove = true;
                        }
                        if(permission.Action == "Arrange")
                        { sessionValue.HasArrange = true; }
                    }
                }
                System.Web.HttpContext.Current.Session["SessionValue"] = sessionValue;
                return RedirectToLocal(returnUrl);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "UserName or Password is wrong");
            if (ModelState.ContainsKey("UserName"))
            {
                if (ModelState["UserName"].Errors.Count > 0)
                {
                    ModelState["UserName"].Errors[0] = new ModelError("UserName is empty");
                }
            }
            if (ModelState.ContainsKey("Password"))
            {
                if (ModelState["UserName"].Errors.Count > 0)
                {
                    ModelState["Password"].Errors[0] = new ModelError("Password is empty");
                }
            }
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session["SessionValue"] = null;
            return RedirectToAction("LoginView", "Account");
        }
        public void LogOffAuto()
        {
            WebSecurity.Logout();
            Session["SessionValue"] = null;
        }

        //
        public ActionResult RegisterView()
        {
            IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
            UserInfo userInfo = UserInfoService.Find(x => x.UserName == "admin");
            if (userInfo == null)
            { InitializeHelper.Index(); }
            return View();
        }

        [HttpPost]
        public ActionResult RegisterView(FormCollection formCollection)
        {
            try
            {
                IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
                IRoleInfoServiceRepository RoleInfoService = new RoleInfoServiceRepository();
                string strCode = string.Empty;
                byte[] bufferPssword = VerifyCode.Create(6, out strCode);
                WebSecurity.CreateUserAndAccount(formCollection["UserName"], strCode);
                //WebSecurity.Login(model.UserName, model.Password);
                //创建用户
                UserInfo user = new UserInfo();
                user.UserName = formCollection["UserName"];
                user.Sex = "Secrecy";
                user.IsDelete = false;
                user.PhoneNumber = "";
                user.Profile = "";
                user.WeChat = "";
                user.Email = formCollection["Email"];
                user.RegistTime = System.DateTime.Now;
                UserInfoService.Add(user);
                int count = UserInfoService.SaveChanges();
                _registerEmail(user, strCode);
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                return CreateUserListView(currentPageNum, 10);
            }
            catch
            {
                return CreateUserListView(1, 10);
            }
        }
        public ActionResult CreateUserListView(int? currentPageNum, int? pageSize)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = UserInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<UserInfo, bool>> where = null;
            Expression<Func<UserInfo, DateTime>> whereDateTime = null;
            where = x => x.Id != null && x.IsDelete == false && x.UserName !="admin";
            whereDateTime = x => x.RegistTime;
            string[] includes = new string[] { "RoleInfoes" };
            IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
            var userInfoes = UserInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime, includes).ToList();
            List<User> Users = new List<User>();
            int j = 1;
            foreach (UserInfo itemApp in userInfoes)
            {
                User item = new User();
                item.SerialNumber = j.ToString();
                item.UserName = itemApp.UserName;
                item.Email = itemApp.Email;
                item.Phone = itemApp.PhoneNumber;
                item.Sex = itemApp.Sex;
                item.UserId = itemApp.Id.ToString();
                if (itemApp.RoleInfoes.Count > 0)
                {
                    item.UserRole = itemApp.RoleInfoes[0].RoleName;
                }
                else { item.UserRole = ""; }
                Users.Add(item);
                j++;
            }
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = applicationCount, rows = Users };
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        //注册账户
        public void _registerEmail(UserInfo userInfo, string pwd)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = "Register";
            string path = Server.MapPath("~/EmailTemplate.xml");
            string emailBody = XMLHelp.GetRegister(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", userInfo.UserName);
            emailBody = emailBody.Replace("Password", pwd);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            emailAddress.Add(userInfo.Email);
            email.mailToArray = new string[emailAddress.Count];//接收者邮件集合
            for (int j = 0; j < emailAddress.Count; j++)
            {
                email.mailToArray[j] = emailAddress[j];
            }
            //email.mailCcArray = new string[] { "120698234@qq.com" };//抄送者邮件集合
            if (emailAddress.Count > 0)
            {
                Thread thread = new Thread(new ThreadStart(email.Send));
                thread.Start();
            }
        }
        //重置密码
        public void _resetPwd(UserInfo userInfo, string pwd)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = "Password Reset";
            string path = Server.MapPath("~/EmailTemplate.xml");
            string emailBody = XMLHelp.GetResetPassword(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("Password", pwd);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            emailAddress.Add(userInfo.Email);
            email.mailToArray = new string[emailAddress.Count];//接收者邮件集合
            for (int j = 0; j < emailAddress.Count; j++)
            {
                email.mailToArray[j] = emailAddress[j];
            }
            //email.mailCcArray = new string[] { "120698234@qq.com" };//抄送者邮件集合
            if (emailAddress.Count > 0)
            {
                Thread thread = new Thread(new ThreadStart(email.Send));
                thread.Start();
            }
        }
        private JavaScriptSerializer Jss = new JavaScriptSerializer();
        public JsonResult UserSettingChangePW()
        {
            var data = Request.Form["data"];
            JObject jobj = JObject.Parse(data);
            int Id = Convert.ToInt32(jobj["UserId"] != null ? jobj["UserId"].ToString() : string.Empty);
            IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
            UserInfo userInfo = UserInfoService.Find(x=>x.Id==Id);
            string OldPassword = jobj["currentPW"] != null ? jobj["currentPW"].ToString() : string.Empty;
            string NewPassword = jobj["newPW"] != null ? jobj["newPW"].ToString() : string.Empty;
            bool changePasswordSucceeded = WebSecurity.ChangePassword(userInfo.UserName, OldPassword, NewPassword);
            if (!changePasswordSucceeded)
            {
                var response1 = new
                {
                    code = 0,
                    msg = "Create failed!"
                };
                return new JsonResult() { Data = Jss.Serialize(response1) };
            }
            var response2 = new
            {
                code = 1,
                msg = "Create successfully!"
            };
            return new JsonResult() { Data = Jss.Serialize(response2) };
        }

        #region 帮助程序
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
