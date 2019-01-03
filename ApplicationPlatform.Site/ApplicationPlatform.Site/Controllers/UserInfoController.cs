using ApplicationPlatform.BLL;
using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Site.Attributes;
using ApplicationPlatform.Site.ViewModels.UserInfoViewModels;
using ApplicationPlatform.Utilities;
using ApplicationPlatform.Utilities.NodeModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebMatrix.WebData;

namespace ApplicationPlatform.Site.Controllers
{
    [Description(No = 1, Name = "UserManagement")]
    public class UserInfoController : Controller
    {
        private IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
        //private IDepartmentInfoServiceRepository DepartmentService = new DepartmentInfoServiceRepository();
        private IRoleInfoServiceRepository RoleInfoService = new RoleInfoServiceRepository();
        private DbContext SharingContext = ContextFactory.GetDbContext();

        [RoleAuthorize]
        [Description(No = 1, Name = "GetUserList")]
        public ActionResult UserManagement()
        {
            return View();
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
            string[] includes=new string[]{"RoleInfoes"};
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
                item.WeChat = itemApp.WeChat;
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

        [HttpGet]
        public ActionResult MyProfile()
        {
            string UserName = WebSecurity.CurrentUserName;
            var Elist = from o in SharingContext.Set<UserInfo>().Include("Department")
                        .Include("RoleInfoes")
                        .Where(x => x.UserName == UserName)
                        select o;
            UserInfo userInfo = Elist.FirstOrDefault();

            //List<DepartmentInfo> DepartmentInfoes = DepartmentService.FindAll(x => x.Id != null).ToList();

            //var selectItemListDep = new List<SelectListItem>() { 
            //    new SelectListItem(){Value="0",Text=userInfo.Department.DepartmentName,Selected=true}
            //};
            //var selectListDep = new SelectList(DepartmentInfoes, "Id", "DepartmentName");
            //selectItemListDep.AddRange(selectListDep);
            //ViewBag.Department = selectItemListDep;

            var selectItemListSex = new List<SelectListItem>() { 
                new SelectListItem(){Value="0",Text=userInfo.Sex,Selected=true},
                new SelectListItem(){Value="男",Text="男",Selected=false},
                new SelectListItem(){Value="女",Text="女",Selected=false},
                new SelectListItem(){Value="保密",Text="保密",Selected=false}
            };

            ViewBag.Sex = selectItemListSex;

            UserInfoViewModel userInfoViewModel = new UserInfoViewModel();
            userInfoViewModel.UserInfo = userInfo;
            userInfoViewModel.CurrentPageNum = 1;
            userInfoViewModel.PageSize = 10;
            return View(userInfoViewModel);
        }
        [HttpGet]
        public ActionResult UserDetail(string UserName)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginDN", "Account");
            }
            var Elist = from o in SharingContext.Set<UserInfo>().Include("Department")
                        .Include("RoleInfoes")
                        .Where(x => x.UserName == UserName)
                        select o;
            UserInfo userInfo = Elist.FirstOrDefault();
            UserInfoViewModel userInfoViewModel = new UserInfoViewModel();
            userInfoViewModel.UserInfo = userInfo;
            userInfoViewModel.CurrentPageNum = 1;
            userInfoViewModel.PageSize = 10;
            return View(userInfoViewModel);
        }
        [HttpPost]
        public ActionResult Update(FormCollection formCellection, UserInfoViewModel userInfoViewModel)
        {
            string UserName = WebSecurity.CurrentUserName;
            var Elist = from o in SharingContext.Set<UserInfo>().Include("Department")
                        .Include("RoleInfoes")
                        .Where(x => x.UserName == UserName)
                        select o;
            UserInfo userInfo = Elist.FirstOrDefault();
            if (formCellection["Sex"] != "0")
            {
                userInfo.Sex = formCellection["Sex"];
            }
            userInfo.Email = formCellection["UserInfo.Email"];
            userInfo.Profile = formCellection["headerPath"];
            userInfo.PhoneNumber = formCellection["UserInfo.PhoneNumber"];
            userInfo.WeChat = formCellection["UserInfo.WeChat"];
            userInfoViewModel.UserInfo = userInfo;
            int count = SharingContext.SaveChanges();
            return RedirectToAction("MyProfile", userInfoViewModel);
        }
        [HttpGet]
        [RoleAuthorize]
        [Description(No = 1, Name = "EditUser")]
        public ActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCellection, UserInfoViewModel userInfoViewModel)
        {
            string UserName = formCellection["UserInfo.UserName"];
            var Elist = from o in SharingContext.Set<UserInfo>().Include("Department")
                        .Include("RoleInfoes")
                        .Where(x => x.UserName == UserName)
                        select o;
            UserInfo userInfo = Elist.FirstOrDefault();
            if (formCellection["Sex"] != "0")
            {
                userInfo.Sex = formCellection["Sex"];
            }
            int RoleId = Convert.ToInt32(formCellection["RoleInfo"]);
            RoleInfo roleInfo = SharingContext.Set<RoleInfo>().Include("UserInfoes").Where(x => x.Id == RoleId).FirstOrDefault();
            roleInfo = SharingContext.Set<RoleInfo>().Include("Permissions").Where(x => x.Id == RoleId).FirstOrDefault();
            userInfo.Email = formCellection["UserInfo.Email"];
            userInfo.PhoneNumber = formCellection["UserInfo.PhoneNumber"];
            userInfo.WeChat = formCellection["UserInfo.WeChat"];

            userInfo.RoleInfoes.Clear();
            userInfo.RoleInfoes.Add(roleInfo);
            int count = SharingContext.SaveChanges();
            userInfoViewModel.UserInfo = userInfo;
            //return RedirectToAction("MyProfile", userInfoViewModel);
            return RedirectToAction("GetUserInfoList", new { currentPageNum = userInfoViewModel.CurrentPageNum, pageSize = userInfoViewModel.PageSize });
        }
        [HttpPost]
        public JsonResult UpLoadImage()
        {
            System.Web.Script.Serialization.JavaScriptSerializer Jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                var data = Request.Form["data"];
                JObject jobj = JObject.Parse(data);
                string fileName = jobj["fileName"] != null ? jobj["fileName"].ToString() : "";
                string Header = jobj["header"] != null ? jobj["header"].ToString() : "";
                string strFileSavePath = "";
                string userName = WebSecurity.CurrentUserName;
                string HeaderPath = "";
                var response = new object();
                if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(Header))
                {
                    //说明上传头像
                    strFileSavePath = Request.MapPath("~/Content/Headers");
                    string strFileExtention = Path.GetExtension(fileName);
                    if (!Directory.Exists(strFileSavePath))
                    {
                        Directory.CreateDirectory(strFileSavePath);
                    }
                    strFileSavePath += "/" + userName + strFileExtention;
                    string relativePath = "~/Content/Headers/" + userName + strFileExtention;
                    //将base64字符串转化为图片
                    byte[] buffer = Convert.FromBase64String(Header.Split(',')[1]);
                    MemoryStream memStream = new MemoryStream(buffer);
                    SaveImageByWidthHeight(150, 150, memStream, strFileSavePath);
                    memStream.Dispose();
                    HeaderPath = relativePath;

                }
                else
                {
                    response = new
                    {
                        _code = 10,
                        msg = "请重新上传头像！",
                        path = HeaderPath
                    };
                    return new JsonResult() { Data = Jss.Serialize(response) };
                }
                response = new
                {
                    _code = 5,
                    msg = "",
                    path = HeaderPath
                };
                return new JsonResult() { Data = Jss.Serialize(response) };
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                var response = new
                {
                    _code = 5,
                    msg = "上传头像出错，请重新上传！",
                    path = ""
                };
                return new JsonResult() { Data = Jss.Serialize(response) };
            }
        }
        /// <summary>
        /// 等比例压缩图片
        /// </summary>
        private void SaveImageByWidthHeight(int intImgCompressWidth, int intImgCompressHeight, Stream stream, string strFileSavePath)
        {
            try
            {
                //从输入流中获取上传的image对象
                using (Image img = Image.FromStream(stream))
                {
                    //根据压缩比例求出图片的宽度
                    int intWidth = intImgCompressWidth / intImgCompressHeight * img.Height;
                    int intHeight = img.Width * intImgCompressHeight / intImgCompressWidth;
                    //画布
                    using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(img, new Size(intImgCompressWidth, intImgCompressHeight)))
                    {
                        //在画布上创建画笔对象
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
                        {
                            //将图片使用压缩后的宽高,从0，0位置画在画布上
                            graphics.DrawImage(img, 0, 0, intImgCompressWidth, intImgCompressHeight);
                            //保存图片
                            bitmap.Save(strFileSavePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
            }
        }
        public ActionResult UserSetting()
        {
            string userName = WebSecurity.CurrentUserName;
            UserInfoViewModel userInfoView = new UserInfoViewModel();
            UserInfo userInfo = UserInfoService.Find(x=>x.UserName ==userName);
            userInfoView.UserInfo = userInfo;
            if(userInfo.Sex =="Male")
            { userInfoView.Male="checked";}
            else if (userInfo.Sex == "Female")
            { userInfoView.Female = "checked"; }
            else
            { userInfoView.Secrecy = "checked"; }

            return View(userInfoView);
        }
        public ActionResult EditUser(int UserId)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            }
            List<RoleInfo> RoleInfoes = RoleInfoService.FindAll(x=>x.Id !=null && x.IsDelete==false).ToList();
            UserInfo user = SharingContext.Set<UserInfo>().Include("RoleInfoes").Where(x => x.Id == UserId).FirstOrDefault();
            RoleInfo roleInfo = new RoleInfo();
            var selectItemList1 = new List<SelectListItem>();
            if (user.RoleInfoes.Count > 0)
            { 
                roleInfo = user.RoleInfoes[0];
                selectItemList1 = new List<SelectListItem>() { new SelectListItem() { Value = roleInfo.Id.ToString(), Text = roleInfo.RoleName, Selected = true } };
            }
            else {
                selectItemList1 = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "", Selected = true } };
            }

            var selectList1 = new SelectList(RoleInfoes, "Id", "RoleName");
            selectItemList1.AddRange(selectList1);
            ViewBag.RoleInfoes = selectItemList1;
            return View();
        }
        [HttpPost]
        public ActionResult EditUser(int? id, FormCollection formCollection)
        {

            string userId=formCollection["UserId"];
            id = int.Parse(userId);
            int role = Convert.ToInt32(formCollection["UserRole"]);
            UserInfo userInfo = SharingContext.Set<UserInfo>().Include("RoleInfoes").Where(x => x.Id == id).FirstOrDefault();
            userInfo.PhoneNumber = formCollection["Phone"];
            userInfo.WeChat = formCollection["WeChat"];
            userInfo.Email = formCollection["Email"];
            userInfo.Sex = formCollection["Sex"];
            RoleInfo roleInfo = SharingContext.Set<RoleInfo>().Include("UserInfoes").Where(x => x.Id == role).FirstOrDefault();
            if (roleInfo != null)
            {
                userInfo.RoleInfoes.Clear();
                userInfo.RoleInfoes.Add(roleInfo);
            }
            SharingContext.SaveChanges();
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            return CreateUserListView(currentPageNum, 10); 
        }
        [HttpPost]
        public ActionResult EditMyProfile( FormCollection formCollection)
        {

            string userId = formCollection["UserId"];
            int id = int.Parse(userId);
            int role = Convert.ToInt32(formCollection["UserRole"]);
            UserInfo userInfo = UserInfoService.Find(x => x.Id == id);
            userInfo.PhoneNumber = formCollection["Phone"];
            userInfo.WeChat = formCollection["WeChat"];
            userInfo.Email = formCollection["Email"];
            userInfo.Sex = formCollection["Sex"];
            UserInfoService.Update(userInfo);
            int count=UserInfoService.SaveChanges();
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { code=1 };
            var Data = Jss.Serialize(response);
            return Content(Data);
        }
        public ActionResult DeleteUser(FormCollection formCollection)
        {
            string userIds = formCollection[""];
            string[] Ids = userIds.Split(',');
            foreach (string item in Ids)
            {
                int Id = Convert.ToInt32(item);
                UserInfo user=UserInfoService.Find(x=>x.Id ==Id);
                user.IsDelete = true;
                UserInfoService.Update(user);
            }
            try
            {
                int count = UserInfoService.SaveChanges();
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); }
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            return CreateUserListView(currentPageNum,10);
        }
        public ActionResult UserSearch(int? currentPageNum, int? pageSize)
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
            string userName = Request.Params["userSearchValue"];
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<UserInfo, bool>> where = null;
            Expression<Func<UserInfo, DateTime>> whereDateTime = null;
            where = x => x.Id != null && x.IsDelete == false && x.UserName.Contains(userName);
            whereDateTime = x => x.RegistTime;
            string[] includes = new string[] { "RoleInfoes" };
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
            return Content(Data);
        }
    }
}
