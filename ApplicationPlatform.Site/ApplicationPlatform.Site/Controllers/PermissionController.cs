using ApplicationPlatform.BLL;
using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Site.Attributes;
using ApplicationPlatform.Site.Utilities;
using ApplicationPlatform.Site.ViewModels.ApplicationInfoViewModels;
using ApplicationPlatform.Site.ViewModels.UserInfoViewModels;
using ApplicationPlatform.Utilities;
using ApplicationPlatform.Utilities.NodeModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebMatrix.WebData;

namespace ApplicationPlatform.Site.Controllers
{
    [Description(No = 1, Name = "PermissionManagement")]
    public class PermissionController : Controller
    {
        private IUserInfoServiceRepository _userInfoServiceRepository = new UserInfoServiceRepository();
        private ICListItemServiceRepository _cListItemServiceRepository = new CListItemServiceRepository();
        private IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
        private DbContext Context = ContextFactory.GetDbContext();

        [RoleAuthorize]
        [Description(No = 1, Name = "GetPermissions")]
        public ActionResult GetPermissions(int? userInfoId)
        {
            try
            {
                var context = ContextFactory.GetDbContext();
                var userInfo = context.Set<UserInfo>().Include("CListItems").Where(e => e.Id == userInfoId).FirstOrDefault();
                ApplicationPermissionViewModel _filePermissionViewModel = new ApplicationPermissionViewModel(userInfo);
                return View(_filePermissionViewModel);
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                return View("~/Views/Shared/ErrorView.cshtml");
            }
        }
        [HttpPost]
        public JsonResult GetUsers(string fileId)
        {
            DbContext context = ContextFactory.GetDbContext();
            int id = Convert.ToInt32(fileId);
            CListItem cListItem = new CListItem();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            if (id == 0)
            {
                cListItem = context.Set<CListItem>().Include("UserInfoes").Where(x => x.Id == 1).FirstOrDefault();
            }
            else
            {
                cListItem = context.Set<CListItem>().Include("UserInfoes").Where(x => x.Id == id).FirstOrDefault();
            }
            if (cListItem == null)
            {
                var response = new { code = 4, msg = "There's an error,please contact administrator!", users = "" };
                return new JsonResult() { Data = jss.Serialize(response) };
            }
            else
            {
                string users = "";
                List<string> s = new List<string>();
                foreach (UserInfo userInfo in cListItem.UserInfoes)
                {
                    if (userInfo.IsDelete == false)
                    {
                        s.Add(userInfo.UserName);
                    }
                }
                users = string.Join("<br/>", s);

                var response = new { code = 5, msg = "", users = users };
                return new JsonResult() { Data = jss.Serialize(response) };
            }
        }
        public ActionResult CreateUserListView(int? currentPageNum, int? pageSize, string CListId)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            DbContext context = ContextFactory.GetDbContext();
            int id = Convert.ToInt32(CListId);
            CListItem cListItem = new CListItem();
            if (id == 0)
            {
                cListItem = context.Set<CListItem>().Include("UserInfoes").Where(x => x.Id == 1).FirstOrDefault();
            }
            else
            {
                cListItem = context.Set<CListItem>().Include("UserInfoes").Where(x => x.Id == id).FirstOrDefault();
            }
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                int rows=Convert.ToInt32(Request.Params["rows"]);
                if (rows != 0)
                {
                    pageSize = rows;
                }
                else { pageSize = UserInfoListViewModel.DefaultPageSize; }
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, userInfoCount;
            Expression<Func<UserInfo, DateTime>> whereDateTime = null;
            whereDateTime = x => x.RegistTime;
            var userInfoes = UserInfoService.FindPaged(pageSize.Value, ref pageNum, out userInfoCount, out pageCount, cListItem.UserInfoes.AsQueryable(), false, whereDateTime).ToList();
            List<User> Users = new List<User>();
            int j = 1;
            foreach (UserInfo itemApp in userInfoes)
            {
                UserInfo userInfo = Context.Set<UserInfo>().Include("RoleInfoes").Where(x => x.Id == itemApp.Id).FirstOrDefault();
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
            var response = new { total = userInfoCount, rows = Users };
            var Data = Jss.Serialize(response);
            return Content(Data);
        }

        [HttpPost]
        [RoleAuthorize]
        [Description(No = 1, Name = "SavePermissions")]
        public JsonResult Save(int _userInfoId)
        {
            try
            {
                DbContext context = ContextFactory.GetDbContext();
                UserInfo _userInfo = context.Set<UserInfo>().Include("CListItems").Where(e => e.Id == _userInfoId).FirstOrDefault();
                //用UserInfo初始化FilePermissionViewModel
                var filePermission = new ApplicationPermissionViewModel(_userInfo);
                //获取tree勾选的Id号
                string NodeId = Request.Params["NodeId"];
                string[] Ids = NodeId.Split(',');
                //获取所有未删除的files，再筛选过滤为勾选的
                List<CListItem> _myFile = context.Set<CListItem>().Include("UserInfoes").Where(e => e.Id != null && e.IsDelete == false).ToList();
                //开始将勾选的文件夹项目以及其中的文件添加到对应的UserInfo
                CListItem myFile;
                if (Ids[0] != "")
                {
                    foreach (string a in Ids)
                    {
                        int b = int.Parse(a);

                        myFile = context.Set<CListItem>().Include("UserInfoes").Where(e => e.Id == b).FirstOrDefault();
                        _myFile.Remove(myFile);
                        if (myFile.UserInfoes.Any(x => x.Id == _userInfo.Id))
                        { continue; }
                        myFile.UserInfoes.Add(_userInfo);
                        context.SaveChanges();
                    }
                }
                //开始将剩余未勾选的文件项目移除列表
                foreach (CListItem a in _myFile)
                {
                    if (a.UserInfoes.Any(x => x.Id == _userInfo.Id))
                    {
                        a.UserInfoes.Remove(_userInfo);
                        context.SaveChanges();
                    }
                }
                var response = new { code = 1, msg = "Save successfully!" };
                return new JsonResult() { Data = new JavaScriptSerializer().Serialize(response) };
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                var response = new { code = 0, msg = "Save failed" };
                return new JsonResult() { Data = new JavaScriptSerializer().Serialize(response) };
            }
        }
        /// <summary>
        /// 获取文件Tree
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public ActionResult GetFileTreeJson(int userInfoId)
        {
            try
            {
                UserInfo userInfo = Context.Set<UserInfo>().Include("CListItems").Where(x => x.Id == userInfoId).FirstOrDefault();
                var cListItems=Context.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId== 0 && x.IsDelete==false);
                List<EasyTreeData> treeList = new List<EasyTreeData>();
                foreach (CListItem item in cListItems)
                {
                    if (item.IsDelete)
                    { continue; }
                    EasyTreeData a = getTree(item, userInfo);
                    treeList.Add(a);
                }

                string json = ToJson(treeList);
                return Content(json);
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
                return View("~/Views/Shared/ErrorView.cshtml");
            }
        }

        /// <summary>
        /// 把对象为json字符串
        /// </summary>
        /// <param name="obj">待序列号对象</param>
        /// <returns></returns>
        protected string ToJson(object obj)
        {
            string jsonData = (new JavaScriptSerializer()).Serialize(obj);
            return jsonData;

        }

        /// <summary>
        /// 递归获取数据
        /// </summary>
        /// <param name="myFile"></param>
        /// <returns></returns>
        public EasyTreeData getTree(CListItem myFile, UserInfo userInfo)
        {
            //转换成Easyui数据
            EasyTreeData model = getEasyui(myFile);
            List<CListItem> list = new List<CListItem>();
            if (userInfo.CListItems.Contains(myFile))
            { model.@checked = "true"; }
            list = Context.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == myFile.Id && x.IsDelete == false).ToList();
            if (list.Count > 0)
            {
                model.state = "closed";
                model.children = new List<EasyTreeData>();
                foreach (var item in list)
                {
                    //递归子节点
                    model.children.Add(getTree(item, userInfo));
                }
            }
            else { model.state = "open"; }
            if (myFile.Id == 1) { model.state = "open"; }
            return model;
        }

        /// <summary>
        /// 转换成Easyui数据
        /// </summary>
        /// <param name="myFile"></param>
        /// <returns></returns>
        public EasyTreeData getEasyui(CListItem myFile)
        {
            EasyTreeData treeData = new EasyTreeData();
            treeData.id = myFile.Id.ToString();
            treeData.text = myFile.Text;
            treeData.iconCls = "icon-folder";
            return treeData;
        }
        [HttpGet]
        public void GetPermissionDetails()
        {
            string ExcelOutTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filePath = Path.Combine(Server.MapPath("~/NetDisk/"), ExcelOutTime + ".xls");
            EmailTools.ExportPermissionExcel(filePath);
            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            string fileNameTemp = HttpUtility.UrlEncode("Permission_Details.xls", System.Text.Encoding.UTF8);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNameTemp);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
    }
}
