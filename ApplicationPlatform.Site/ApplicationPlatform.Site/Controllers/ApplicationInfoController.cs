using ApplicationPlatform.BLL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Utilities;
using ApplicationPlatform.Site.ViewModels.ApplicationInfoViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SiteAddress = ApplicationPlatform.Utilities.NodeModels.Site;
using ApplicationPlatform.Utilities.NodeModels;
using WebMatrix.WebData;
using ApplicationPlatform.Site.Attributes;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using ApplicationPlatform.DAL;
using System.IO;
using System.Text;
using ApplicationPlatform.Site.Utilities;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace ApplicationPlatform.Site.Controllers
{
    [Description(No = 1, Name = "RequireManagement")]
    public class ApplicationInfoController : Controller
    {
        private IApplicationInfoServiceRepository ApplicationInfoService = new ApplicationInfoServiceRepository();
        private IUserInfoServiceRepository UserInfoService = new UserInfoServiceRepository();
        private ICListItemServiceRepository CListItemService = new CListItemServiceRepository();
        private IPriceInfoServiceRepository PriceInfoService = new PriceInfoServiceRepository();
        private XmlHelp xmlHelp = new XmlHelp();
        private JavaScriptSerializer Jss = new JavaScriptSerializer();
        private DbContext ApplicationContext = ContextFactory.GetDbContext();
        //
        // GET: /ApplicationInfo/Create
        public ActionResult Create()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            }
            var roleInfoe1s = from o in ApplicationContext.Set<RoleInfo>()
                                 .Include("Permissions")
                                 .Include("UserInfoes")
                                 .Where(x => x.Permissions.Any(c => c.Action == "TechnicalApprove"))
                             select o;
            List<UserInfo> Approver1s =new List<UserInfo>();
            foreach (RoleInfo role in roleInfoe1s)
            {
                foreach (UserInfo user in role.UserInfoes)
                {
                    if (!Approver1s.Contains(user))
                    { Approver1s.Add(user); }
                }
            }
            var selectItemList1 = new List<SelectListItem>() { new SelectListItem(){Value="0",Text="/",Selected=true}};
            var selectList1 = new SelectList(Approver1s, "Id", "UserName");
            selectItemList1.AddRange(selectList1);
            ViewBag.Approver1 = selectItemList1;
            var roleInfoe2s = from o in ApplicationContext.Set<RoleInfo>()
                     .Include("Permissions")
                     .Include("UserInfoes")
                     .Where(x => x.Permissions.Any(c => c.Action == "CommercialApprove"))
                             select o;
            List<UserInfo> Approver2s = new List<UserInfo>();
            foreach (RoleInfo role in roleInfoe2s)
            {
                foreach (UserInfo user in role.UserInfoes)
                {
                    if (!Approver2s.Contains(user))
                    { Approver2s.Add(user); }
                }
            }
            var selectItemList2 = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "/", Selected = true } };
            var selectList2 = new SelectList(Approver2s, "Id", "UserName");
            selectItemList2.AddRange(selectList2);
            ViewBag.Approver2 = selectItemList2;
            SessionValue sessionValue=Session["SessionValue"] as SessionValue;
            if(sessionValue.HasEditButton)
            { ViewBag.Edit = "true"; }
            else { ViewBag.Edit = "false"; }
            return View();
        }
        public ActionResult CreateSite()
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            List<SiteAddress> SiteAddresses = new List<SiteAddress>();
            SiteAddresses = xmlHelp.GetSiteVariables();
            var Data = Jss.Serialize(SiteAddresses);
            return Content(Data);
        }
        //
        // POST: /ApplicationInfo/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [RoleAuthorize]
        [Description(No = 1, Name = "Create")]
        public JsonResult Create(string checkBoxVal)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    var response0 = new
                    {
                        code = 0,
                        msg = "Please login before creating!"
                    };
                    return new JsonResult() { Data = Jss.Serialize(response0) };
                }
                // TODO: Add insert logic here
                var data = Request.Form["data"];
                JObject jobj = JObject.Parse(data);
                string Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                string Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                string Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                string Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                string Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(checkBoxVal))
                {
                    string[] requirements = checkBoxVal.Split(',');
                    List<ApplicationInfo> applicationInfoes = new List<ApplicationInfo>();
                    foreach (string item in requirements)
                    {
                        string[] items = item.Split('/');
                        int count=items.Length;
                        if(count ==1)
                        {
                            ApplicationInfo applicationInfo = new ApplicationInfo();
                            InitializeApplicationInfo(applicationInfo, jobj, items[0], "", "","", "");
                            applicationInfoes.Add(applicationInfo);
                        }
                        else if (count == 2)
                        {
                            ApplicationInfo applicationInfo = new ApplicationInfo();
                            InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], "", "", "");
                            applicationInfoes.Add(applicationInfo);
                        }
                        else if (count == 3)
                        {
                            ApplicationInfo applicationInfo = new ApplicationInfo();
                            InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], items[2], "", "");
                            applicationInfoes.Add(applicationInfo);
                        }
                        else if (count == 4)
                        {
                            ApplicationInfo applicationInfo = new ApplicationInfo();
                            InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], items[2], items[3], "");
                            applicationInfoes.Add(applicationInfo);
                        }
                        else if (count == 5)
                        {
                            ApplicationInfo applicationInfo = new ApplicationInfo();
                            InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], items[2], items[3], items[4]);
                            applicationInfoes.Add(applicationInfo);
                        }
                    }
                    CreateApplicationInfoAll(applicationInfoes);
                }
                else
                {
                    //正常添加ApplicationInfo
                    ApplicationInfo applicationInfo = new ApplicationInfo();
                    applicationInfo.UserName = WebSecurity.CurrentUserName;
                    applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                    applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                    applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                    applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                    applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                    applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                    applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
                    applicationInfo.CreateTime = System.DateTime.Now;
                    applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
                    applicationInfo.IsComApproved = false;
                    int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                    UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                    applicationInfo.TechApprovedUser = approver.UserName;
                    int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                    approver = UserInfoService.Find(x => x.Id == approver2);
                    applicationInfo.ComApprovedUser = approver.UserName;
                    applicationInfo.TechDsp = "";
                    applicationInfo.ComDsp = "";
                    applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                    applicationInfo.IsDelete = false;
                    SessionValue sessionValue = Session["SessionValue"] as SessionValue;
                    if (sessionValue.HasTechAppprove && WebSecurity.CurrentUserName == applicationInfo.TechApprovedUser)
                    {
                        applicationInfo.Status = "Commercial Approval";
                        applicationInfo.IsTechApproved = true;
                        //设置单价
                        PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
                        if (priceInfo != null)
                        { applicationInfo.UnitPrice = priceInfo.Price; }
                        var temp = ApplicationInfoService.Add(applicationInfo);
                        int count = ApplicationInfoService.SaveChanges();
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.TechApprovedUser);
                        userInfoes.Add(userInfo);
                        EmailTools.SendCommercial("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                    }
                    else
                    {
                        applicationInfo.Status = "Technical Approval";
                        applicationInfo.IsTechApproved = false;
                        //设置单价
                        PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
                        if (priceInfo != null)
                        { applicationInfo.UnitPrice = priceInfo.Price; }
                        var temp = ApplicationInfoService.Add(applicationInfo);
                        int count = ApplicationInfoService.SaveChanges();
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.TechApprovedUser);
                        userInfoes.Add(userInfo);
                        EmailTools.SendTechnical("Technical Approval", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                    }
                    CreateCListItems(applicationInfo);
                }

                var response1 = new
                {
                    code = 1,
                    msg = "Create successfully!"
                };
                return new JsonResult() { Data = Jss.Serialize(response1) };
            }
            catch
            {
                var response = new
                {
                    code = 0,
                    msg = "Create failed!"
                };
                return new JsonResult() { Data = Jss.Serialize(response) };
            }
        }
        public ActionResult CreateDetailShow(string Product, string Project, string Type, string Item, string Subitem)
        {
            if (Product == "All" || Project == "All" || Type == "All" || Item == "All" || Subitem == "All")
            {
                List<string> CreateString = new List<string>();
                List<ApplicationInfo> applicationInfoes = new List<ApplicationInfo>();
                if (Product == "All")
                {
                    var products = CListItemService.FindAll(x => x.ParentId == 0 && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                    if (products.Count() > 0)
                    {
                        foreach (CListItem itemProduct in products)
                        {
                            var projects = CListItemService.FindAll(x => x.ParentId == itemProduct.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                            if (projects.Count() > 0)
                            {
                                foreach (CListItem itemProject in projects)
                                {
                                    var types = CListItemService.FindAll(x => x.ParentId == itemProject.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                                    if (types.Count() > 0)
                                    {
                                        foreach (CListItem itemType in types)
                                        {
                                            var items = CListItemService.FindAll(x => x.ParentId == itemType.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                                            if (items.Count() > 0)
                                            {
                                                foreach (CListItem itemItem in items)
                                                {
                                                    var subitems = CListItemService.FindAll(x => x.ParentId == itemItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                                                    if (subitems.Count() > 0)
                                                    {
                                                        foreach (CListItem itemSubitem in subitems)
                                                        {
                                                            CreateString.Add(itemProduct.Text + "/" + itemProject.Text + "/" + itemType.Text + "/" + itemItem.Text + "/" + itemSubitem.Text);
                                                        }
                                                    }
                                                    else { CreateString.Add(itemProduct.Text + "/" + itemProject.Text + "/" + itemType.Text + "/" + itemItem.Text); }                                           
                                                }
                                            }
                                            else { CreateString.Add(itemProduct.Text + "/" + itemProject.Text+"/"+itemType.Text); }
                                        }
                                    }
                                    else { CreateString.Add(itemProduct.Text + "/" + itemProject.Text); }
                                }
                            }
                            else { CreateString.Add(Product); }

                        }
                    }
                    else
                    {
                        return Content("");
                    }
                    return Content(Jss.Serialize(CreateString));
                }
                if (Project == "All")
                {
                    CListItem product = CListItemService.Find(x => x.Text == Product && x.ParentId == 0);
                    var projects = CListItemService.FindAll(x => x.ParentId == product.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                    if (projects.Count() > 0)
                    {
                        foreach (CListItem itemProject in projects)
                        {
                            var types = CListItemService.FindAll(x => x.ParentId == itemProject.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                            if (types.Count() > 0)
                            {
                                foreach (CListItem itemType in types)
                                {
                                    var items = CListItemService.FindAll(x => x.ParentId == itemType.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                                    if (items.Count() > 0)
                                    {
                                        foreach (CListItem itemItem in items)
                                        {
                                            var subitems = CListItemService.FindAll(x => x.ParentId == itemItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                                            if (subitems.Count() > 0)
                                            {
                                                foreach (CListItem itemSubitem in subitems)
                                                {
                                                    CreateString.Add(Product + "/" + itemProject.Text + "/" + itemType.Text + "/" + itemItem.Text + "/" + itemSubitem.Text);
                                                }
                                            }
                                            else { CreateString.Add(Product + "/" + itemProject.Text + "/" + itemType.Text + "/" + itemItem.Text); }
                                        }
                                    }
                                    else { CreateString.Add(Product + "/" + itemProject.Text + "/" + itemType.Text); }
                                }
                            }
                            else { CreateString.Add(Product + "/" + itemProject.Text); }
                        }
                    }
                    else { CreateString.Add(Product); }
                    return Content(Jss.Serialize(CreateString));
                }
                if (Type == "All")
                {
                    CListItem product = CListItemService.Find(x => x.Text == Product && x.ParentId == 0);
                    CListItem project = CListItemService.Find(x => x.Text == Project && x.ParentId == product.Id);
                    var types = CListItemService.FindAll(x => x.ParentId == project.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                    if (types.Count() > 0)
                    {
                        foreach (CListItem itemType in types)
                        {
                            var items = CListItemService.FindAll(x => x.ParentId == itemType.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                            if (items.Count() > 0)
                            {
                                foreach (CListItem itemItem in items)
                                {
                                    var subitems = CListItemService.FindAll(x => x.ParentId == itemItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                                    if (subitems.Count() > 0)
                                    {
                                        foreach (CListItem itemSubitem in subitems)
                                        {
                                            CreateString.Add(Product + "/" + Project + "/" + itemType.Text + "/" + itemItem.Text + "/" + itemSubitem.Text);
                                        }
                                    }
                                    else { CreateString.Add(Product + "/" + Project + "/" + itemType.Text + "/" + itemItem.Text); }
                                }
                            }
                            else { CreateString.Add(Product + "/" + Project + "/" + itemType.Text); }
                        }
                    }
                    else { CreateString.Add(Product + "/" + Project); }
                    return Content(Jss.Serialize(CreateString));
                }
                if (Item == "All")
                {
                    CListItem product = CListItemService.Find(x => x.Text == Product && x.ParentId == 0);
                    CListItem project = CListItemService.Find(x => x.Text == Project && x.ParentId == product.Id);
                    CListItem type = CListItemService.Find(x => x.Text == Type && x.ParentId == project.Id);
                    var items = CListItemService.FindAll(x => x.ParentId == type.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                    if (items.Count() > 0)
                    {
                        foreach (CListItem itemItem in items)
                        {
                            var subitems = CListItemService.FindAll(x => x.ParentId == itemItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                            if (subitems.Count() > 0)
                            {
                                foreach (CListItem itemSubitem in subitems)
                                {
                                    CreateString.Add(Product + "/" + Project + "/" + Type + "/" + itemItem.Text + "/" + itemSubitem.Text);
                                }
                            }
                            else { CreateString.Add(Product + "/" + Project + "/" + Type + "/" + itemItem.Text); }
                        }
                    }
                    else { CreateString.Add(Product + "/" + Project + "/" + Type); }
                    return Content(Jss.Serialize(CreateString));
                }
                if (Subitem == "All")
                {
                    CListItem product = CListItemService.Find(x => x.Text == Product && x.ParentId == 0);
                    CListItem project = CListItemService.Find(x => x.Text == Project && x.ParentId == product.Id);
                    CListItem type = CListItemService.Find(x => x.Text == Type && x.ParentId == project.Id);
                    CListItem item = CListItemService.Find(x => x.Text == Item && x.ParentId == type.Id);
                    var subitems = CListItemService.FindAll(x => x.ParentId == item.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == WebSecurity.CurrentUserName));
                    if (subitems.Count() > 0)
                    {
                        foreach (CListItem itemSubitem in subitems)
                        {
                            CreateString.Add(Product + "/" + Project + "/" + Type + "/" + Item + "/" + itemSubitem.Text);
                        }
                    }
                    else { CreateString.Add(Product + "/" + Project + "/" + Type + "/" + Item); }
                    return Content(Jss.Serialize(CreateString));
                }
                var response = Jss.Serialize(applicationInfoes);
                return Content(response);
            }
            else { return Content(""); }
        }
        public void InitializeApplicationInfo(ApplicationInfo applicationInfo, JObject jobj, string product,string project, string type,string item,string subitem)
        {
            applicationInfo.UserName = WebSecurity.CurrentUserName;
            applicationInfo.Product = product;
            applicationInfo.Project = project;
            applicationInfo.Type = type;
            applicationInfo.Item = item;
            applicationInfo.Subitem = subitem;
            applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
            applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
            applicationInfo.CreateTime = System.DateTime.Now;
            applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
            applicationInfo.IsComApproved = false;
            int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
            UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
            applicationInfo.TechApprovedUser = approver.UserName;
            int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
            approver = UserInfoService.Find(x => x.Id == approver2);
            applicationInfo.ComApprovedUser = approver.UserName;
            SessionValue sessionValue = Session["SessionValue"] as SessionValue;
            if (sessionValue.HasTechAppprove && WebSecurity.CurrentUserName == applicationInfo.TechApprovedUser)
            {
                applicationInfo.Status = "Commercial Approval";
                applicationInfo.IsTechApproved = true;
            }
            else
            {
                applicationInfo.Status = "Technical Approval";
                applicationInfo.IsTechApproved = false;
            }
            applicationInfo.TechDsp = "";
            applicationInfo.ComDsp = "";
            applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
            applicationInfo.IsDelete = false;
            //设置单价
            PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
            if (priceInfo != null)
            { applicationInfo.UnitPrice = priceInfo.Price; }
        }
        public void InitializeApplicationInfo(ApplicationInfo applicationInfo, string product, string project, string type, string item, string subitem)
        {
            applicationInfo.Product = product;
            applicationInfo.Project = project;
            applicationInfo.Type = type;
            applicationInfo.Item = item;
            applicationInfo.Subitem = subitem;
        }
        public void CreateApplicationInfoAll(List<ApplicationInfo> applicationInfoes)
        {
            foreach (ApplicationInfo applicationInfo in applicationInfoes)
            {
                var temp = ApplicationInfoService.Add(applicationInfo);
                int count = ApplicationInfoService.SaveChanges();
                SessionValue sessionValue = Session["SessionValue"] as SessionValue;
                if (sessionValue.HasTechAppprove && WebSecurity.CurrentUserName == applicationInfo.TechApprovedUser)
                {
                    //发送邮件
                    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    List<UserInfo> userInfoes = new List<UserInfo>();
                    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.TechApprovedUser);
                    userInfoes.Add(userInfo);
                    EmailTools.SendCommercial("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                    //发送邮件结束
                }
                else
                {
                    //发送邮件
                    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    List<UserInfo> userInfoes = new List<UserInfo>();
                    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.TechApprovedUser);
                    userInfoes.Add(userInfo);
                    EmailTools.SendTechnical("Technical Approval", userInfoes, applicationInfo, XmlPath);
                    //发送邮件结束
                }
            }
        }
        public void CreateSaveApplicationInfoAll(List<ApplicationInfo> applicationInfoes)
        {
            foreach (ApplicationInfo applicationInfo in applicationInfoes)
            {
                var temp = ApplicationInfoService.Add(applicationInfo);
                int count = ApplicationInfoService.SaveChanges();
            }
        }
        [HttpPost]
        public ActionResult Save(int? Id)
        {
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return CreateSavedView(currentPageNum,10);
                }
                //获取保存的Id
                var data = Request.Form["data"];
                JObject jobj = JObject.Parse(data);
                int ApplicationId = Convert.ToInt32(jobj["requirementId"] != null ? jobj["requirementId"].ToString() : string.Empty);
                string saveOrsubmit = jobj["saveOrsubmit"] != null ? jobj["saveOrsubmit"].ToString() : string.Empty;
                ApplicationInfo saved = ApplicationInfoService.Find(x=>x.Id ==ApplicationId);
                ApplicationInfo applicationInfo = new ApplicationInfo();
                int count = 0;
                //
                if (saved == null)
                {
                    applicationInfo.UserName = WebSecurity.CurrentUserName;
                    applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                    applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                    applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                    applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                    applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                    applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                    applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
                    applicationInfo.CreateTime = System.DateTime.Now;
                    applicationInfo.SavedTime = applicationInfo.CreateTime;
                    //applicationInfo.Status = "Technical Approval";
                    applicationInfo.Status = "";
                    applicationInfo.IsSaved = true;
                    applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
                    applicationInfo.IsComApproved = false;
                    int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                    UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                    applicationInfo.TechApprovedUser =approver !=null? approver.UserName:"";
                    applicationInfo.IsTechApproved = false;
                    int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                    approver = UserInfoService.Find(x => x.Id == approver2);
                    applicationInfo.ComApprovedUser = approver != null ? approver.UserName : "";
                    applicationInfo.TechDsp = "";
                    applicationInfo.ComDsp = "";
                    applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                    applicationInfo.IsDelete = false;
                    //设置单价
                    PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
                    if (priceInfo != null)
                    { applicationInfo.UnitPrice = priceInfo.Price; }
                    var temp = ApplicationInfoService.Add(applicationInfo);
                    count = ApplicationInfoService.SaveChanges();
                }
                else
                {
                    if (saveOrsubmit != "submit")
                    {
                        applicationInfo = saved;
                        applicationInfo.UserName = WebSecurity.CurrentUserName;
                        applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                        applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                        applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                        applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                        applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                        applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                        applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
                        //applicationInfo.CreateTime = System.DateTime.Now;
                        applicationInfo.SavedTime = System.DateTime.Now;
                        //applicationInfo.Status = "Technical Approval";
                        applicationInfo.Status = "";
                        applicationInfo.IsSaved = true;
                        applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
                        applicationInfo.IsComApproved = false;
                        int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                        UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                        applicationInfo.TechApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.IsTechApproved = false;
                        int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                        approver = UserInfoService.Find(x => x.Id == approver2);
                        applicationInfo.ComApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.TechDsp = "";
                        applicationInfo.ComDsp = "";
                        applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                        applicationInfo.IsDelete = false;
                        var temp = ApplicationInfoService.Update(applicationInfo);
                        count = ApplicationInfoService.SaveChanges();
                    }
                    else
                    {
                        applicationInfo = saved;
                        applicationInfo.UserName = WebSecurity.CurrentUserName;
                        applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                        applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                        applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                        applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                        applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                        applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                        applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
                        applicationInfo.SavedTime = System.DateTime.Now;
                        applicationInfo.Status = "Technical Approval";
                        applicationInfo.IsSaved = false;
                        applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
                        applicationInfo.IsComApproved = false;
                        int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                        UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                        applicationInfo.TechApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.IsTechApproved = false;
                        int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                        approver = UserInfoService.Find(x => x.Id == approver2);
                        applicationInfo.ComApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.TechDsp = "";
                        applicationInfo.ComDsp = "";
                        applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                        applicationInfo.IsDelete = false;
                        var temp = ApplicationInfoService.Update(applicationInfo);
                        count = ApplicationInfoService.SaveChanges();
                    }
                }
                CreateCListItems(applicationInfo);
                return CreateSavedView(currentPageNum, 10);
            }
            catch
            {
                return CreateSavedView(currentPageNum, 10);
            }
        }
        [HttpPost]
        public JsonResult SaveAjax(string checkBoxVal)
        {
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            try
            {
                if (!Request.IsAuthenticated)
                {
                    var response0 = new
                    {
                        code = 0,
                        msg = "Please login before saving!"
                    };
                    return new JsonResult() { Data = Jss.Serialize(response0) };
                }
                //获取保存的Id
                var data = Request.Form["data"];
                JObject jobj = JObject.Parse(data);
                int ApplicationId = Convert.ToInt32(jobj["requirementId"] != null ? jobj["requirementId"].ToString() : string.Empty);
                string saveOrsubmit = jobj["saveOrsubmit"] != null ? jobj["saveOrsubmit"].ToString() : string.Empty;
                ApplicationInfo saved = ApplicationInfoService.Find(x => x.Id == ApplicationId);
                ApplicationInfo applicationInfo = new ApplicationInfo();
                int count = 0;
                //
                if (saved == null)
                {
                    string Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                    string Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                    string Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                    string Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                    string Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                    if (Product == "All" || Project == "All" || Type == "All" || Item == "All" || Subitem == "All")
                    {
                        if (!string.IsNullOrEmpty(checkBoxVal))
                        {
                            string[] requirements = checkBoxVal.Split(',');
                            List<ApplicationInfo> applicationInfoes = new List<ApplicationInfo>();
                            foreach (string item in requirements)
                            {
                                string[] items = item.Split('/');
                                count = items.Length;
                                if (count == 1)
                                {
                                    applicationInfo = new ApplicationInfo();
                                    InitializeApplicationInfo(applicationInfo, jobj, items[0], "", "", "", "");
                                    applicationInfo.IsSaved = true;
                                    applicationInfoes.Add(applicationInfo);
                                }
                                else if (count == 2)
                                {
                                    applicationInfo = new ApplicationInfo();
                                    InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], "", "", "");
                                    applicationInfo.IsSaved = true;
                                    applicationInfoes.Add(applicationInfo);
                                }
                                else if (count == 3)
                                {
                                    applicationInfo = new ApplicationInfo();
                                    InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], items[2], "", "");
                                    applicationInfo.IsSaved = true;
                                    applicationInfoes.Add(applicationInfo);
                                }
                                else if (count == 4)
                                {
                                    applicationInfo = new ApplicationInfo();
                                    InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], items[2], items[3], "");
                                    applicationInfo.IsSaved = true;
                                    applicationInfoes.Add(applicationInfo);
                                }
                                else if (count == 5)
                                {
                                    applicationInfo = new ApplicationInfo();
                                    InitializeApplicationInfo(applicationInfo, jobj, items[0], items[1], items[2], items[3], items[4]);
                                    applicationInfo.IsSaved = true;
                                    applicationInfoes.Add(applicationInfo);
                                }
                            }
                            CreateApplicationInfoAll(applicationInfoes);
                        }
                    }
                    else
                    {
                        applicationInfo.UserName = WebSecurity.CurrentUserName;
                        applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                        applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                        applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                        applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                        applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                        applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                        applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
                        applicationInfo.CreateTime = System.DateTime.Now;
                        applicationInfo.SavedTime = applicationInfo.CreateTime;
                        //applicationInfo.Status = "Technical Approval";
                        applicationInfo.Status = "";
                        applicationInfo.IsSaved = true;
                        applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
                        applicationInfo.IsComApproved = false;
                        int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                        UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                        applicationInfo.TechApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.IsTechApproved = false;
                        int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                        approver = UserInfoService.Find(x => x.Id == approver2);
                        applicationInfo.ComApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.TechDsp = "";
                        applicationInfo.ComDsp = "";
                        applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                        applicationInfo.IsDelete = false;
                        var temp = ApplicationInfoService.Add(applicationInfo);
                        count = ApplicationInfoService.SaveChanges();
                    }
                }
                else
                {
                    if (saveOrsubmit != "submit")
                    {
                        applicationInfo = saved;
                        applicationInfo.UserName = WebSecurity.CurrentUserName;
                        applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                        applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                        applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                        applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                        applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                        applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                        applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
                        //applicationInfo.CreateTime = System.DateTime.Now;
                        applicationInfo.SavedTime = System.DateTime.Now;
                        //applicationInfo.Status = "Technical Approval";
                        applicationInfo.Status = "";
                        applicationInfo.IsSaved = true;
                        applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
                        applicationInfo.IsComApproved = false;
                        int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                        UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                        applicationInfo.TechApprovedUser = approver.UserName;
                        applicationInfo.IsTechApproved = false;
                        int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                        approver = UserInfoService.Find(x => x.Id == approver2);
                        applicationInfo.ComApprovedUser = approver.UserName;
                        applicationInfo.TechDsp = "";
                        applicationInfo.ComDsp = "";
                        applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                        applicationInfo.IsDelete = false;
                        var temp = ApplicationInfoService.Update(applicationInfo);
                        count = ApplicationInfoService.SaveChanges();
                    }
                    else
                    {
                        applicationInfo = saved;
                        applicationInfo.UserName = WebSecurity.CurrentUserName;
                        applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                        applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                        applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                        applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                        applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                        applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
                        applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : "1");
                        applicationInfo.SavedTime = System.DateTime.Now;
                        applicationInfo.IsSaved = false;
                        applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;
                        applicationInfo.IsComApproved = false;
                        int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                        UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                        applicationInfo.TechApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.IsTechApproved = false;
                        int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                        approver = UserInfoService.Find(x => x.Id == approver2);
                        applicationInfo.ComApprovedUser = approver != null ? approver.UserName : "";
                        applicationInfo.TechDsp = "";
                        applicationInfo.ComDsp = "";
                        applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                        applicationInfo.IsDelete = false;
                        SessionValue sessionValue = Session["SessionValue"] as SessionValue;
                        if (sessionValue.HasTechAppprove && WebSecurity.CurrentUserName == applicationInfo.TechApprovedUser)
                        {
                            applicationInfo.Status = "Commercial Approval";
                            applicationInfo.IsTechApproved = true;
                            //设置单价
                            PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
                            if (priceInfo != null)
                            { applicationInfo.UnitPrice = priceInfo.Price; }
                            //发送邮件
                            string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                            List<UserInfo> userInfoes = new List<UserInfo>();
                            UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.ComApprovedUser);
                            userInfoes.Add(userInfo);
                            EmailTools.SendCommercial("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                            //发送邮件结束
                        }
                        else
                        {
                            applicationInfo.Status = "Technical Approval";
                            applicationInfo.IsTechApproved = false;
                            //设置单价
                            PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
                            if (priceInfo != null)
                            { applicationInfo.UnitPrice = priceInfo.Price; }
                            //发送邮件
                            string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                            List<UserInfo> userInfoes = new List<UserInfo>();
                            UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.TechApprovedUser);
                            userInfoes.Add(userInfo);
                            EmailTools.SendTechnical("Technical Approval", userInfoes, applicationInfo, XmlPath);
                            //发送邮件结束
                        }
                        var temp = ApplicationInfoService.Update(applicationInfo);
                        count = ApplicationInfoService.SaveChanges();
                    }
                }
                CreateCListItems(applicationInfo);
                var response1 = new
                {
                    code = 1,
                    msg = "Save successfully!"
                };
                return new JsonResult() { Data = Jss.Serialize(response1) };
            }
            catch
            {
                var response0 = new
                {
                    code = 0,
                    msg = "Save failed!"
                };
                return new JsonResult() { Data = Jss.Serialize(response0) };
            }
        }
        public void CreateCListItems(ApplicationInfo applicationInfo)
        {
            CListItem Product = new CListItem();
            CListItem Project = new CListItem();
            CListItem Type = new CListItem();
            CListItem Item = new CListItem();
            CListItem SubItem = new CListItem();
            CListItem cListItem = new CListItem();
            int count = 0;
            string UserName = WebSecurity.CurrentUserName;
            UserInfo userInfo = UserInfoService.Find(x => x.UserName == UserName);

            Product = CListItemService.Find(x => x.ParentId == 0 && x.Text == applicationInfo.Product);
            if (Product == null)
            {
                if (!string.IsNullOrEmpty(applicationInfo.Product) && applicationInfo.Product != "/")
                {
                    cListItem.Text = applicationInfo.Product;
                    cListItem.ParentId = 0;
                    cListItem.UserInfoes.Add(userInfo);
                    Product = CListItemService.Add(cListItem);
                    count = CListItemService.SaveChanges();
                    Project = CListItemService.Find(x => x.ParentId == Product.Id && x.Text == applicationInfo.Project);
                    if (Project==null)
                    {
                        if (!string.IsNullOrEmpty(applicationInfo.Project) && applicationInfo.Project != "/")
                        {
                            cListItem = new CListItem();
                            cListItem.Text = applicationInfo.Project;
                            cListItem.ParentId = Product.Id;
                            cListItem.UserInfoes.Add(userInfo);
                            Project = CListItemService.Add(cListItem);
                            count = CListItemService.SaveChanges();
                            Type = CListItemService.Find(x => x.ParentId == Project.Id && x.Text == applicationInfo.Type);
                            if (Type == null )
                            {
                                if (!string.IsNullOrEmpty(applicationInfo.Type) && applicationInfo.Type != "/")
                                {
                                    cListItem = new CListItem();
                                    cListItem.Text = applicationInfo.Type;
                                    cListItem.ParentId = Project.Id;
                                    cListItem.UserInfoes.Add(userInfo);
                                    Type = CListItemService.Add(cListItem);
                                    count = CListItemService.SaveChanges();
                                    Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                                    if (Item == null )
                                    {
                                        if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                                        {
                                            cListItem = new CListItem();
                                            cListItem.Text = applicationInfo.Item;
                                            cListItem.ParentId = Type.Id;
                                            cListItem.UserInfoes.Add(userInfo);
                                            Item = CListItemService.Add(cListItem);
                                            count = CListItemService.SaveChanges();
                                            SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                            if (SubItem == null )
                                            {
                                                if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                                {
                                                    cListItem = new CListItem();
                                                    cListItem.Text = applicationInfo.Subitem;
                                                    cListItem.ParentId = Item.Id;
                                                    cListItem.UserInfoes.Add(userInfo);
                                                    SubItem = CListItemService.Add(cListItem);
                                                    count = CListItemService.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                        if (SubItem == null )
                                        {
                                            if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                            {
                                                cListItem = new CListItem();
                                                cListItem.Text = applicationInfo.Item;
                                                cListItem.ParentId = Item.Id;
                                                cListItem.UserInfoes.Add(userInfo);
                                                SubItem = CListItemService.Add(cListItem);
                                                count = CListItemService.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                                if (Item == null )
                                {
                                    if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                                    {
                                        cListItem = new CListItem();
                                        cListItem.Text = applicationInfo.Item;
                                        cListItem.ParentId = Type.Id;
                                        cListItem.UserInfoes.Add(userInfo);
                                        Item = CListItemService.Add(cListItem);
                                        count = CListItemService.SaveChanges();
                                        SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                        if (SubItem == null)
                                        {
                                            if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                            {
                                                cListItem = new CListItem();
                                                cListItem.Text = applicationInfo.Subitem;
                                                cListItem.ParentId = Item.Id;
                                                cListItem.UserInfoes.Add(userInfo);
                                                SubItem = CListItemService.Add(cListItem);
                                                count = CListItemService.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                    if (SubItem == null)
                                    {
                                        if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                        {
                                            cListItem = new CListItem();
                                            cListItem.Text = applicationInfo.Subitem;
                                            cListItem.ParentId = Item.Id;
                                            cListItem.UserInfoes.Add(userInfo);
                                            SubItem = CListItemService.Add(cListItem);
                                            count = CListItemService.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Type = CListItemService.Find(x => x.ParentId == Project.Id && x.Text == applicationInfo.Type);
                        if (Type == null)
                        {
                            if (!string.IsNullOrEmpty(applicationInfo.Type) && applicationInfo.Type != "/")
                            {
                                cListItem = new CListItem();
                                cListItem.Text = applicationInfo.Type;
                                cListItem.ParentId = Project.Id;
                                cListItem.UserInfoes.Add(userInfo);
                                Type = CListItemService.Add(cListItem);
                                count = CListItemService.SaveChanges();
                                Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                                if (Item == null)
                                {
                                    if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                                    {
                                        cListItem = new CListItem();
                                        cListItem.Text = applicationInfo.Item;
                                        cListItem.ParentId = Type.Id;
                                        cListItem.UserInfoes.Add(userInfo);
                                        Item = CListItemService.Add(cListItem);
                                        count = CListItemService.SaveChanges();
                                        SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                        if (SubItem == null)
                                        {
                                            if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                            {
                                                cListItem = new CListItem();
                                                cListItem.Text = applicationInfo.Subitem;
                                                cListItem.ParentId = Item.Id;
                                                cListItem.UserInfoes.Add(userInfo);
                                                SubItem = CListItemService.Add(cListItem);
                                                count = CListItemService.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                    if (SubItem == null )
                                    {
                                        if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                        {
                                            cListItem = new CListItem();
                                            cListItem.Text = applicationInfo.Subitem;
                                            cListItem.ParentId = Item.Id;
                                            cListItem.UserInfoes.Add(userInfo);
                                            SubItem = CListItemService.Add(cListItem);
                                            count = CListItemService.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                            if (Item == null )
                            {
                                if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                                {
                                    cListItem = new CListItem();
                                    cListItem.Text = applicationInfo.Item;
                                    cListItem.ParentId = Type.Id;
                                    cListItem.UserInfoes.Add(userInfo);
                                    Item = CListItemService.Add(cListItem);
                                    count = CListItemService.SaveChanges();
                                    SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                    if (SubItem == null )
                                    {
                                        if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                        {
                                            cListItem = new CListItem();
                                            cListItem.Text = applicationInfo.Subitem;
                                            cListItem.ParentId = Item.Id;
                                            cListItem.UserInfoes.Add(userInfo);
                                            SubItem = CListItemService.Add(cListItem);
                                            count = CListItemService.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                if (SubItem == null )
                                {
                                    if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                    {
                                        cListItem = new CListItem();
                                        cListItem.Text = applicationInfo.Subitem;
                                        cListItem.ParentId = Item.Id;
                                        cListItem.UserInfoes.Add(userInfo);
                                        SubItem = CListItemService.Add(cListItem);
                                        count = CListItemService.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Project = CListItemService.Find(x => x.ParentId == Product.Id && x.Text == applicationInfo.Project);
                if (Project == null)
                {
                    if (!string.IsNullOrEmpty(applicationInfo.Project) && applicationInfo.Project != "/")
                    {
                        cListItem = new CListItem();
                        cListItem.Text = applicationInfo.Project;
                        cListItem.ParentId = Product.Id;
                        cListItem.UserInfoes.Add(userInfo);
                        Project = CListItemService.Add(cListItem);
                        count = CListItemService.SaveChanges();
                        Type = CListItemService.Find(x => x.ParentId == Project.Id && x.Text == applicationInfo.Type);
                        if (Type == null)
                        {
                            if (!string.IsNullOrEmpty(applicationInfo.Type) && applicationInfo.Type != "/")
                            {
                                cListItem = new CListItem();
                                cListItem.Text = applicationInfo.Type;
                                cListItem.ParentId = Project.Id;
                                cListItem.UserInfoes.Add(userInfo);
                                Type = CListItemService.Add(cListItem);
                                count = CListItemService.SaveChanges();
                                Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                                if (Item == null )
                                {
                                    if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                                    {
                                        cListItem = new CListItem();
                                        cListItem.Text = applicationInfo.Item;
                                        cListItem.ParentId = Type.Id;
                                        cListItem.UserInfoes.Add(userInfo);
                                        Item = CListItemService.Add(cListItem);
                                        count = CListItemService.SaveChanges();
                                        SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                        if (SubItem == null )
                                        {
                                            if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                            {
                                                cListItem = new CListItem();
                                                cListItem.Text = applicationInfo.Subitem;
                                                cListItem.ParentId = Item.Id;
                                                cListItem.UserInfoes.Add(userInfo);
                                                SubItem = CListItemService.Add(cListItem);
                                                count = CListItemService.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                    if (SubItem == null )
                                    {
                                        if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                        {
                                            cListItem = new CListItem();
                                            cListItem.Text = applicationInfo.Subitem;
                                            cListItem.ParentId = Item.Id;
                                            cListItem.UserInfoes.Add(userInfo);
                                            SubItem = CListItemService.Add(cListItem);
                                            count = CListItemService.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                            if (Item == null)
                            {
                                if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                                {
                                    cListItem = new CListItem();
                                    cListItem.Text = applicationInfo.Item;
                                    cListItem.ParentId = Type.Id;
                                    cListItem.UserInfoes.Add(userInfo);
                                    Item = CListItemService.Add(cListItem);
                                    count = CListItemService.SaveChanges();
                                    SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                    if (SubItem == null )
                                    {
                                        if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                        {
                                            cListItem = new CListItem();
                                            cListItem.Text = applicationInfo.Subitem;
                                            cListItem.ParentId = Item.Id;
                                            cListItem.UserInfoes.Add(userInfo);
                                            SubItem = CListItemService.Add(cListItem);
                                            count = CListItemService.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                if (SubItem == null )
                                {
                                    if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                    {
                                        cListItem = new CListItem();
                                        cListItem.Text = applicationInfo.Subitem;
                                        cListItem.ParentId = Item.Id;
                                        cListItem.UserInfoes.Add(userInfo);
                                        SubItem = CListItemService.Add(cListItem);
                                        count = CListItemService.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Type = CListItemService.Find(x => x.ParentId == Project.Id && x.Text == applicationInfo.Type);
                    if (Type == null )
                    {
                        if (!string.IsNullOrEmpty(applicationInfo.Type) && applicationInfo.Type != "/")
                        {
                            cListItem = new CListItem();
                            cListItem.Text = applicationInfo.Type;
                            cListItem.ParentId = Project.Id;
                            cListItem.UserInfoes.Add(userInfo);
                            Type = CListItemService.Add(cListItem);
                            count = CListItemService.SaveChanges();
                            Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                            if (Item == null )
                            {
                                if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                                {
                                    cListItem = new CListItem();
                                    cListItem.Text = applicationInfo.Item;
                                    cListItem.ParentId = Type.Id;
                                    cListItem.UserInfoes.Add(userInfo);
                                    Item = CListItemService.Add(cListItem);
                                    count = CListItemService.SaveChanges();
                                    SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                    if (SubItem == null )
                                    {
                                        if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                        {
                                            cListItem = new CListItem();
                                            cListItem.Text = applicationInfo.Subitem;
                                            cListItem.ParentId = Item.Id;
                                            cListItem.UserInfoes.Add(userInfo);
                                            SubItem = CListItemService.Add(cListItem);
                                            count = CListItemService.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                if (SubItem == null )
                                {
                                    if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                    {
                                        cListItem = new CListItem();
                                        cListItem.Text = applicationInfo.Subitem;
                                        cListItem.ParentId = Item.Id;
                                        cListItem.UserInfoes.Add(userInfo);
                                        SubItem = CListItemService.Add(cListItem);
                                        count = CListItemService.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Item = CListItemService.Find(x => x.ParentId == Type.Id && x.Text == applicationInfo.Item);
                        if (Item == null )
                        {
                            if (!string.IsNullOrEmpty(applicationInfo.Item) && applicationInfo.Item != "/")
                            {
                                cListItem = new CListItem();
                                cListItem.Text = applicationInfo.Item;
                                cListItem.ParentId = Type.Id;
                                cListItem.UserInfoes.Add(userInfo);
                                Item = CListItemService.Add(cListItem);
                                count = CListItemService.SaveChanges();
                                SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                                if (SubItem == null && !string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                {
                                    if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                    {
                                        cListItem = new CListItem();
                                        cListItem.Text = applicationInfo.Subitem;
                                        cListItem.ParentId = Item.Id;
                                        cListItem.UserInfoes.Add(userInfo);
                                        SubItem = CListItemService.Add(cListItem);
                                        count = CListItemService.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            SubItem = CListItemService.Find(x => x.ParentId == Item.Id && x.Text == applicationInfo.Subitem);
                            if (SubItem == null )
                            {
                                if (!string.IsNullOrEmpty(applicationInfo.Subitem) && applicationInfo.Subitem != "/")
                                {
                                    cListItem = new CListItem();
                                    cListItem.Text = applicationInfo.Subitem;
                                    cListItem.ParentId = Item.Id;
                                    cListItem.UserInfoes.Add(userInfo);
                                    SubItem = CListItemService.Add(cListItem);
                                    count = CListItemService.SaveChanges();
                                }
                            }
                        }
                    }
                }

            }
        }
        public ActionResult EditSelectDialog()
        {
            return View();
        }

        //
        // POST: /ApplicationInfo/Delete/5

        [HttpPost]
        [RoleAuthorize]
        [Description(No = 1, Name = "Delete")]
        public ActionResult Delete(string RequirementIds, FormCollection collection)
        {
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add delete logic here
                string[] Ids = RequirementIds.Split(',');
                foreach(string item in Ids)
                {
                    int id = int.Parse(item);
                    ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == id);
                    applicationInfo.IsDelete = true;
                    ApplicationInfoService.Update(applicationInfo);
                    int count = ApplicationInfoService.SaveChanges(); 
                }
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return CreateProcessingView(currentPageNum, 10);
            }
        }
        public ActionResult DeleteSave(string RequirementIds, FormCollection collection)
        {
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add delete logic here
                string[] Ids = RequirementIds.Split(',');
                foreach (string item in Ids)
                {
                    int id = int.Parse(item);
                    ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == id);
                    applicationInfo.IsDelete = true;
                    ApplicationInfoService.Update(applicationInfo);
                    int count = ApplicationInfoService.SaveChanges();
                }
                return CreateSavedView(currentPageNum, 10);
            }
            catch
            {
                return CreateSavedView(currentPageNum, 10);
            }
        }
        public ActionResult SubmitSave(string RequirementIds, FormCollection collection)
        {
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add delete logic here
                string[] Ids = RequirementIds.Split(',');
                foreach (string item in Ids)
                {
                    int id = int.Parse(item);
                    ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == id);
                    applicationInfo.IsSaved = false;
                    SessionValue sessionValue = Session["SessionValue"] as SessionValue;
                    if (sessionValue.HasTechAppprove && WebSecurity.CurrentUserName == applicationInfo.TechApprovedUser)
                    {
                        applicationInfo.Status = "Commercial Approval";
                        applicationInfo.IsTechApproved = true;
                        //设置单价
                        PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
                        if (priceInfo != null)
                        { applicationInfo.UnitPrice = priceInfo.Price; }
                        ApplicationInfoService.Update(applicationInfo);
                        int count = ApplicationInfoService.SaveChanges();
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.ComApprovedUser);
                        userInfoes.Add(userInfo);
                        EmailTools.SendCommercial("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                    }
                    else
                    {
                        applicationInfo.Status = "Technical Approval";
                        applicationInfo.IsTechApproved = false;
                        //设置单价
                        PriceInfo priceInfo = PriceInfoService.Find(x => x.Product == applicationInfo.Product && x.Project == applicationInfo.Project && x.Type == applicationInfo.Type && x.Item == applicationInfo.Item && x.Subitem == applicationInfo.Subitem);
                        if (priceInfo != null)
                        { applicationInfo.UnitPrice = priceInfo.Price; }
                        ApplicationInfoService.Update(applicationInfo);
                        int count = ApplicationInfoService.SaveChanges();
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.TechApprovedUser);
                        userInfoes.Add(userInfo);
                        EmailTools.SendTechnical("Technical Approval", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                    }
                }
                return CreateSavedView(currentPageNum, 10);
            }
            catch
            {
                return CreateSavedView(currentPageNum, 10);
            }
        }
        [HttpGet]
        public ActionResult ProcessingView()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            }
            ViewBag.CurrentTask = "";
            return View();
        }
        //[HttpGet]
        //public ActionResult ProcessingView(string CurrentTask)
        //{
        //    if (!Request.IsAuthenticated)
        //    {
        //        return RedirectToAction("LoginView", "Account");
        //    }
        //    ViewBag.CurrentTask = CurrentTask;
        //    return View();
        //}
        public ActionResult CreateProcessingView(int? currentPageNum, int? pageSize)
        {
            SessionValue sessionValue = System.Web.HttpContext.Current.Session["SessionValue"] as SessionValue;
            string CurrentTask=Request.Params["CurrentTask"];
            //当前登录用户名
            string UserName=WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            string progress = Request.Params["progress"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            if (string.IsNullOrEmpty(progress))
            {
                where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.IsSaved == false);
            }
            else 
            {
                if (progress =="Technical Approval")
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.Status == "Technical Approval" && x.IsSaved == false);
                }
                else if (progress == "Commercial Approval")
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.Status == "Commercial Approval" && x.IsSaved == false);
                }
                else if (progress == "Hexagon Technical Planning")
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && (x.Status == "Hexagon Technical Planning" || x.Status == "Waiting for CAD" || x.Status == "DFM") && x.IsSaved == false);
                }
                else
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.IsTechApproved == false && x.IsComApproved == false && (x.Status == "Technical Rejection" || x.Status == "Commercial Rejection") && x.IsSaved == false);
                }
            }
            whereDateTime = x => x.CreateTime;
            //var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();
            var applicationInfoes = ApplicationInfoService.FindAll(where).OrderByDescending(whereDateTime);
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                //Tech和Com都审批完的状态
                if (itemApp.IsComApproved && itemApp.IsTechApproved )
                {
                    if (sessionValue.HasArrange)
                    {
                        if (itemApp.Status == "Waiting for CAD")
                        {
                            if (itemApp.Statuses.Contains("DFM"))
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Production</option><option>Inventory</option></select> <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                            else
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>DFM</option><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                        }
                        else if (itemApp.Status == "DFM")
                        {
                            if (itemApp.Statuses.Contains("Waiting for CAD"))
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                            else
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Waiting for CAD</option><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                        }
                        else
                        {
                            item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Waiting for CAD</option><option>DFM</option><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                        }
                    }
                    else
                    {
                        if (itemApp.ComApprovedUser == UserName || itemApp.TechApprovedUser == UserName || itemApp.UserName == UserName)
                        { item.operation = ""; }
                        else { continue; }
                    }
                }
                //提交后需要审批的状态
                else if (!itemApp.IsComApproved || !itemApp.IsTechApproved)
                {
                    if (itemApp.IsTechApproved)
                    {
                        //如果技术审核后，有商务审核权限那么就赋予商务审核按钮和拒绝按钮
                        if (sessionValue.HasComAppprove && itemApp.Status != "Technical Rejection" && itemApp.Status != "Commercial Rejection" && itemApp.ComApprovedUser == UserName)
                        {

                            item.operation = "<input value=\"Approve\" type=\"button\" class=\"processApproveBtn\" style=\"background:#5CB85C\"  onclick=\"approveComFun(" + itemApp.Id.ToString() + ");\"/><input value=\"Reject\" type=\"button\" class=\"processRejectBtn\"  style=\"background:#D9534F\"  onclick=\"rejectFun(" + itemApp.Id.ToString() + ");\"/>";
                        }
                        else
                        {
                            if (itemApp.ComApprovedUser == UserName || itemApp.TechApprovedUser == UserName || itemApp.UserName == UserName)
                            { item.operation = ""; }
                            else { continue; }
                        }

                    }
                    else
                    {
                        //提交后开始技术审批，如果有技术审核权限那么就赋予技术审核按钮和拒绝按钮
                        if (sessionValue.HasTechAppprove && itemApp.Status != "Technical Rejection" && itemApp.Status != "Commercial Rejection" && itemApp.TechApprovedUser == UserName)
                        {
                            item.operation = "<input value=\"Approve\" type=\"button\" class=\"processApproveBtn\" style=\"background:#5CB85C\"  onclick=\"approveTechFun(" + itemApp.Id.ToString() + ");\"/><input value=\"Reject\" type=\"button\" class=\"processRejectBtn\"  style=\"background:#D9534F\"  onclick=\"rejectFun(" + itemApp.Id.ToString() + ");\"/>";

                        }
                        else
                        {
                            if (itemApp.ComApprovedUser == UserName || itemApp.TechApprovedUser == UserName || itemApp.UserName == UserName)
                            {
                                if (itemApp.UserName == UserName && (itemApp.Status == "Commercial Rejection" || itemApp.Status == "Technical Rejection"))
                                {
                                    item.operation = "<input value=\"Edit\" type=\"button\" class=\"processApproveBtn\" style=\"background:#9B30FF\"  onclick=\"editTableInfoFun1();\"/>";
                                }
                                else { item.operation = ""; }                             
                            }
                            else { continue; }
                        }
                    }
                }
                item.approver1 = itemApp.TechApprovedUser;
                item.approver2 = itemApp.ComApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                ProcessingViews.Add(item);
                j++;
            }
            ProcessingViews = FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, ProcessingViews.AsQueryable()).ToList();
            //设置显示结束
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = applicationCount, rows = ProcessingViews };
            var Data = Jss.Serialize(response);
            return Content(Data);
        }
        public IQueryable<ProcessingView> FindPaged(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, IQueryable<ProcessingView> TEntities)
        {

            var _list = TEntities;
            totalRecord = 0;
            pageCount = 0;
            if (_list != null)
            {

                totalRecord = _list.Count<ProcessingView>();
                if (totalRecord == 0)
                { return _list; }
                if (totalRecord % pageSize == 0)
                {
                    pageCount = totalRecord / pageSize;
                }
                else
                {
                    pageCount = totalRecord / pageSize + 1;
                }

                if (pageNum <= 1)
                {
                    pageNum = 1;
                }
                if (pageNum >= pageCount)
                {
                    pageNum = pageCount;
                }

                _list = _list.Skip<ProcessingView>((pageNum - 1) * pageSize).Take<ProcessingView>(pageSize);

            }
            return _list;
        }
        public ActionResult CompletedView()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            }
            return View();
        }
        public ActionResult CreateCompletedView(int? currentPageNum, int? pageSize)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            //where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == true) && (x.UserName == UserName || x.ComApprovedUser == UserName || x.TechApprovedUser == UserName);
            where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == true && x.UserName == UserName)
            ||
            (x.Id != null && x.IsDelete == false && x.IsCompleted == true && x.ComApprovedUser == UserName)
            ||
            (x.Id != null && x.IsDelete == false && x.IsCompleted == true && x.TechApprovedUser == UserName);
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.arrangeUser = itemApp.ArrangeUser;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                item.operation = "";
                item.approver1 = itemApp.TechApprovedUser;
                item.approver2 = itemApp.ComApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                item.ETD = itemApp.EndTime.ToString().Split(' ')[0];
                ProcessingViews.Add(item);
                j++;
            }
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = applicationCount, rows = ProcessingViews };
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult CreateAllView(int? currentPageNum, int? pageSize)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false;
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.arrangeUser = itemApp.ArrangeUser;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                item.operation = "";
                item.approver1 = itemApp.ComApprovedUser;
                item.approver2 = itemApp.TechApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                item.ETD = itemApp.EndTime.ToString().Split(' ')[0];
                item.ATD = itemApp.ATD.ToString().Split(' ')[0];
                item.unitprice = itemApp.UnitPrice.ToString();
                item.totalprice = (itemApp.UnitPrice * itemApp.Num).ToString();
                ProcessingViews.Add(item);
                j++;
            }
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = applicationCount, rows = ProcessingViews };
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult SaveRequirements()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            } 
            return View();
        }
        public ActionResult EditSaveReqView()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            }
            var roleInfoe1s = from o in ApplicationContext.Set<RoleInfo>()
                                 .Include("Permissions")
                                 .Include("UserInfoes")
                                 .Where(x => x.Permissions.Any(c => c.Action == "TechnicalApprove"))
                              select o;
            List<UserInfo> Approver1s = new List<UserInfo>();
            foreach (RoleInfo role in roleInfoe1s)
            {
                foreach (UserInfo user in role.UserInfoes)
                {
                    if (!Approver1s.Contains(user))
                    { Approver1s.Add(user); }
                }
            }
            var selectItemList1 = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "/", Selected = true } };
            var selectList1 = new SelectList(Approver1s, "Id", "UserName");
            selectItemList1.AddRange(selectList1);
            ViewBag.Approver1 = selectItemList1;
            var roleInfoe2s = from o in ApplicationContext.Set<RoleInfo>()
                     .Include("Permissions")
                     .Include("UserInfoes")
                     .Where(x => x.Permissions.Any(c => c.Action == "CommercialApprove"))
                              select o;
            List<UserInfo> Approver2s = new List<UserInfo>();
            foreach (RoleInfo role in roleInfoe2s)
            {
                foreach (UserInfo user in role.UserInfoes)
                {
                    if (!Approver2s.Contains(user))
                    { Approver2s.Add(user); }
                }
            }
            var selectItemList2 = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "/", Selected = true } };
            var selectList2 = new SelectList(Approver2s, "Id", "UserName");
            selectItemList2.AddRange(selectList2);
            ViewBag.Approver2 = selectItemList2;
            SessionValue sessionValue = Session["SessionValue"] as SessionValue;
            if (sessionValue.HasEditButton)
            { ViewBag.Edit = "true"; }
            else { ViewBag.Edit = "false"; }
            return View();
        }
        public ActionResult CreateSavedView(int? currentPageNum, int? pageSize)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            where = x => x.Id != null && x.IsDelete == false && x.IsSaved ==true && x.UserName==UserName;
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.arrangeUser = itemApp.ArrangeUser;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                item.operation = "";
                item.approver1 = itemApp.TechApprovedUser;
                item.approver2 = itemApp.ComApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                item.ETD = itemApp.EndTime.ToString().Split(' ')[0];
                item.savetime = itemApp.SavedTime.ToString().Split(' ')[0];
                ProcessingViews.Add(item);
                j++;
            }
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = applicationCount, rows = ProcessingViews };
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult CreateProcessingViewSearch(int? currentPageNum, int? pageSize, string project)
        {
            SessionValue sessionValue = System.Web.HttpContext.Current.Session["SessionValue"] as SessionValue;
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            string progress = Request.Params["progress"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            whereDateTime = x => x.CreateTime;
            if (string.IsNullOrEmpty(progress))
            {
                where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.IsSaved == false && (x.Project.Contains(project) || x.Product.Contains(project) || x.Site.Contains(project) || x.Stage.Contains(project) || x.Subitem.Contains(project) || x.Type.Contains(project) || x.UserName.Contains(project) || x.TechApprovedUser.Contains(project) || x.ComApprovedUser.Contains(project)));
            }
            else
            {
                if (progress == "Technical Approval")
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.Status == "Technical Approval" && (x.Project.Contains(project) || x.Product.Contains(project) || x.Site.Contains(project) || x.Stage.Contains(project) || x.Subitem.Contains(project) || x.Type.Contains(project) || x.UserName.Contains(project) || x.TechApprovedUser.Contains(project) || x.ComApprovedUser.Contains(project)));
                }
                else if (progress == "Commercial Approval")
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.Status == "Commercial Approval" && (x.Project.Contains(project) || x.Product.Contains(project) || x.Site.Contains(project) || x.Stage.Contains(project) || x.Subitem.Contains(project) || x.Type.Contains(project) || x.UserName.Contains(project) || x.TechApprovedUser.Contains(project) || x.ComApprovedUser.Contains(project)));
                }
                else if (progress == "Hexagon Technical Planning")
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && (x.Status == "Hexagon Technical Planning" || x.Status == "Waiting for CAD" || x.Status == "DFM") && (x.Project.Contains(project) || x.Product.Contains(project) || x.Site.Contains(project) || x.Stage.Contains(project) || x.Subitem.Contains(project) || x.Type.Contains(project) || x.UserName.Contains(project) || x.TechApprovedUser.Contains(project) || x.ComApprovedUser.Contains(project)));
                }
                else
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.IsTechApproved == false && x.IsComApproved == false && (x.Status == "Technical Rejection" || x.Status == "Commercial Rejection") && (x.Project.Contains(project) || x.Product.Contains(project) || x.Site.Contains(project) || x.Stage.Contains(project) || x.Subitem.Contains(project) || x.Type.Contains(project) || x.UserName.Contains(project) || x.TechApprovedUser.Contains(project) || x.ComApprovedUser.Contains(project)));
                }
            }
            //var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();
            var applicationInfoes = ApplicationInfoService.FindAll(where).OrderByDescending(whereDateTime); List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";

                //Tech和Com都审批完的状态
                if (itemApp.IsComApproved && itemApp.IsTechApproved)
                {
                    if (sessionValue.HasArrange)
                    {
                        if (itemApp.Status == "Waiting for CAD")
                        {
                            if (itemApp.Statuses.Contains("DFM"))
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Production</option><option>Inventory</option></select><input value=\"Arrange\" type=\"button\" onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                            else
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>DFM</option><option>Production</option><option>Inventory</option></select><input value=\"Arrange\" type=\"button\" onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                        }
                        else if (itemApp.Status == "DFM")
                        {
                            if (itemApp.Statuses.Contains("Waiting for CAD"))
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Production</option><option>Inventory</option></select><input value=\"Arrange\" type=\"button\" onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                            else
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Waiting for CAD</option><option>Production</option><option>Inventory</option></select><input value=\"Arrange\" type=\"button\" onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                        }
                        else
                        {
                            item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Waiting for CAD</option><option>DFM</option><option>Production</option><option>Inventory</option></select><input value=\"Arrange\" type=\"button\" onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                        }
                    }
                    else
                    {
                        if (itemApp.ComApprovedUser == UserName || itemApp.TechApprovedUser == UserName || itemApp.UserName == UserName)
                        { item.operation = ""; }
                        else { continue; }
                    }
                }
                //提交后需要审批的状态
                else if (!itemApp.IsComApproved || !itemApp.IsTechApproved)
                {
                    if (itemApp.IsTechApproved)
                    {
                        //如果技术审核后，有商务审核权限那么就赋予商务审核按钮和拒绝按钮
                        if (sessionValue.HasComAppprove && itemApp.Status != "Technical Rejection" && itemApp.Status != "Commercial Rejection" && itemApp.ComApprovedUser == UserName)
                        {

                            item.operation = "<input value=\"Approve\" type=\"button\" onclick=\"approveComFun(" + itemApp.Id.ToString() + ");\"/><input value=\"Reject\" type=\"button\" onclick=\"rejectFun(" + itemApp.Id.ToString() + ");\"/>";

                        }
                        else
                        {
                            if (itemApp.ComApprovedUser == UserName || itemApp.TechApprovedUser == UserName || itemApp.UserName == UserName)
                            { item.operation = ""; }
                            else { continue; }
                        }

                    }
                    else
                    {
                        //提交后开始技术审批，如果有技术审核权限那么就赋予技术审核按钮和拒绝按钮
                        if (sessionValue.HasTechAppprove && itemApp.Status != "Technical Rejection" && itemApp.Status != "Commercial Rejection" && itemApp.TechApprovedUser == UserName)
                        {
                            item.operation = "<input value=\"Approve\" type=\"button\" onclick=\"approveTechFun(" + itemApp.Id.ToString() + ");\"/><input value=\"Reject\" type=\"button\" onclick=\"rejectFun(" + itemApp.Id.ToString() + ");\"/>";
                        }
                        else
                        {
                            if (itemApp.ComApprovedUser == UserName || itemApp.TechApprovedUser == UserName || itemApp.UserName == UserName)
                            {
                                if (itemApp.UserName == UserName && (itemApp.Status == "Commercial Rejection" || itemApp.Status == "Technical Rejection"))
                                {
                                    item.operation = "<input value=\"Edit\" type=\"button\" class=\"processApproveBtn\" style=\"background:#9B30FF\"  onclick=\"editTableInfoFun1();\"/>";
                                }
                                else { item.operation = ""; }
                            }
                            else { continue; }
                        }
                    }
                }
                item.approver1 = itemApp.TechApprovedUser;
                item.approver2 = itemApp.ComApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                ProcessingViews.Add(item);
                j++;
            }
            ProcessingViews = FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, ProcessingViews.AsQueryable()).ToList();
            var response = new { total = applicationCount, rows = ProcessingViews };
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult CreateCompletedIntervalViewSearch(int? currentPageNum, int? pageSize, string searchInputValC,string searchCriteria)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string interval = Request.Params["timeInValC"];
            string startDateA = Request.Params["startDateC"];
            string endDateA = Request.Params["endDateC"];
            if (string.IsNullOrEmpty(searchInputValC))
            { searchInputValC = ""; }
            DateTime now = DateTime.Now;
            DateTime  span1;
            DateTime  span2;
            switch (interval)
            { 
                case "1 Month":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "3 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-90);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "6 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-180);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "1 Year":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-365);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                default :
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
            }


            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value;
            int pageCount=0;
            int applicationCount=0;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            if (span1 == null && span2 == null)
            {
                //where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == true) && (x.UserName == UserName || x.ComApprovedUser == UserName || x.TechApprovedUser == UserName) && x.Project.Contains(searchInputValC);
                where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == true && (x.UserName == UserName || x.ComApprovedUser == UserName || x.TechApprovedUser == UserName) && (x.Project.Contains(searchInputValC) || x.Site.Contains(searchInputValC) || x.Stage.Contains(searchInputValC) || x.Subitem.Contains(searchInputValC) || x.Type.Contains(searchInputValC) || x.TechApprovedUser.Contains(searchInputValC) || x.ComApprovedUser.Contains(searchInputValC)));
            }
            else
            {
                //where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == true) && (x.UserName == UserName || x.ComApprovedUser == UserName || x.TechApprovedUser == UserName) && x.Project.Contains(searchInputValC) && x.CreateTime >= span1 && x.CreateTime <= span2;
                if (searchCriteria != "ETD")
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == true && (x.UserName == UserName || x.ComApprovedUser == UserName || x.TechApprovedUser == UserName) && (x.Project.Contains(searchInputValC) || x.Site.Contains(searchInputValC) || x.Stage.Contains(searchInputValC) || x.Subitem.Contains(searchInputValC) || x.Type.Contains(searchInputValC) || x.TechApprovedUser.Contains(searchInputValC) || x.ComApprovedUser.Contains(searchInputValC)) && x.CreateTime >= span1 && x.CreateTime <= span2);
                }
                else
                {
                    where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == true && (x.UserName == UserName || x.ComApprovedUser == UserName || x.TechApprovedUser == UserName) && (x.Project.Contains(searchInputValC) || x.Site.Contains(searchInputValC) || x.Stage.Contains(searchInputValC) || x.Subitem.Contains(searchInputValC) || x.Type.Contains(searchInputValC) || x.TechApprovedUser.Contains(searchInputValC) || x.ComApprovedUser.Contains(searchInputValC)) && x.EndTime >= span1 && x.EndTime <= span2);
                }
            }
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime);

            //var applicationInfoes = ApplicationInfoService.FindAll(x=>x.Id !=null);
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            if (applicationInfoes.Count() != 0)
            {
                int j = 1;
                foreach (ApplicationInfo itemApp in applicationInfoes)
                {
                    ProcessingView item = new ProcessingView();
                    item.product = itemApp.Product;
                    item.subitem = itemApp.Subitem;
                    item.arrangeUser = itemApp.ArrangeUser;
                    item.serialNumber = j.ToString();
                    item.project = itemApp.Project;
                    item.item = itemApp.Item;
                    item.type = itemApp.Type;
                    item.stage = itemApp.Stage;
                    item.site = itemApp.Site;
                    item.progress = itemApp.Status;
                    item.quantity = itemApp.Num.ToString();
                    item.quantity = itemApp.Num.ToString();
                    item.postuser = itemApp.UserName;
                    item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                    item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                    item.approver1 = itemApp.TechApprovedUser;
                    item.approver2 = itemApp.ComApprovedUser;
                    item.requirementId = itemApp.Id.ToString();
                    item.ETD = itemApp.EndTime.ToString().Split(' ')[0];
                    ProcessingViews.Add(item);
                    j++;
                }
            }
            var response = new { total = applicationCount, rows = ProcessingViews };
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult CreateAllIntervalViewSearch(int? currentPageNum, int? pageSize, string searchInputValA, string searchCriteria)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            //
            string interval = Request.Params["timeInValC"];
            string startDateA = Request.Params["startDateA"];
            string endDateA = Request.Params["endDateA"];
            if (string.IsNullOrEmpty(searchInputValA))
            { searchInputValA = ""; }
            DateTime now = DateTime.Now;
            DateTime span1=new DateTime();
            DateTime span2;
            switch (interval)
            {
                case "1 Month":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "3 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-90);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "6 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-180);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "1 Year":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-365);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
            }
            //
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
            {
                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Project.Contains(searchInputValA) || x.Product.Contains(searchInputValA) || x.Site.Contains(searchInputValA) || x.Stage.Contains(searchInputValA) || x.Subitem.Contains(searchInputValA) || x.Type.Contains(searchInputValA) || x.UserName.Contains(searchInputValA) || x.TechApprovedUser.Contains(searchInputValA) || x.ComApprovedUser.Contains(searchInputValA));
            }
            else
            {
                if (searchCriteria == "Create Date")
                {
                    where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Project.Contains(searchInputValA) || x.Product.Contains(searchInputValA) || x.Site.Contains(searchInputValA) || x.Stage.Contains(searchInputValA) || x.Subitem.Contains(searchInputValA) || x.Type.Contains(searchInputValA) || x.UserName.Contains(searchInputValA) || x.TechApprovedUser.Contains(searchInputValA) || x.ComApprovedUser.Contains(searchInputValA)) && x.CreateTime >= span1 && x.CreateTime <= span2;
                }
                else if (searchCriteria == "ATD")
                {
                    where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Project.Contains(searchInputValA) || x.Product.Contains(searchInputValA) || x.Site.Contains(searchInputValA) || x.Stage.Contains(searchInputValA) || x.Subitem.Contains(searchInputValA) || x.Type.Contains(searchInputValA) || x.UserName.Contains(searchInputValA) || x.TechApprovedUser.Contains(searchInputValA) || x.ComApprovedUser.Contains(searchInputValA)) && x.ATD >= span1 && x.ATD <= span2;
                }
                else
                {
                    where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Project.Contains(searchInputValA) || x.Product.Contains(searchInputValA) || x.Site.Contains(searchInputValA) || x.Stage.Contains(searchInputValA) || x.Subitem.Contains(searchInputValA) || x.Type.Contains(searchInputValA) || x.UserName.Contains(searchInputValA) || x.TechApprovedUser.Contains(searchInputValA) || x.ComApprovedUser.Contains(searchInputValA)) && x.EndTime >= span1 && x.EndTime <= span2;
                }
            }
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.arrangeUser = itemApp.ArrangeUser;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                item.approver1 = itemApp.ComApprovedUser;
                item.approver2 = itemApp.TechApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                item.ETD = itemApp.EndTime.ToString().Split(' ')[0];
                ProcessingViews.Add(item);
                j++;
            }
            var response = new { total = applicationCount, rows = ProcessingViews };
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        [HttpGet]
        public void DownloadAllIntervalViewSearch(string TimeInValC, string StartDateA, string EndDateA, string searchInputValA, string searchCriteria)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            string interval = TimeInValC;
            string startDateA = StartDateA;
            string endDateA = EndDateA;
            if (string.IsNullOrEmpty(searchInputValA))
            { searchInputValA = ""; }
            DateTime now = DateTime.Now;
            DateTime span1;
            DateTime span2;
            switch (interval)
            {
                case "1 Month":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "3 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-90);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "6 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-180);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "1 Year":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-365);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
            }
            //
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            // where = x => x.Id != null && x.IsDelete == false && x.Project.Contains(project);
            if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA) && string.IsNullOrEmpty(interval))
            {
                where = x => x.Id != null && x.IsDelete == false && x.IsSaved==false && (x.Project.Contains(searchInputValA) || x.Product.Contains(searchInputValA) || x.Site.Contains(searchInputValA) || x.Stage.Contains(searchInputValA) || x.Subitem.Contains(searchInputValA) || x.Type.Contains(searchInputValA) || x.UserName.Contains(searchInputValA) || x.TechApprovedUser.Contains(searchInputValA) || x.ComApprovedUser.Contains(searchInputValA));
            }
            else
            {
                if (searchCriteria != "ETD")
                {
                    where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Project.Contains(searchInputValA) || x.Product.Contains(searchInputValA) || x.Site.Contains(searchInputValA) || x.Stage.Contains(searchInputValA) || x.Subitem.Contains(searchInputValA) || x.Type.Contains(searchInputValA) || x.UserName.Contains(searchInputValA) || x.TechApprovedUser.Contains(searchInputValA) || x.ComApprovedUser.Contains(searchInputValA)) && x.CreateTime >= span1 && x.CreateTime <= span2;
                }
                else
                {
                    where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Project.Contains(searchInputValA) || x.Product.Contains(searchInputValA) || x.Site.Contains(searchInputValA) || x.Stage.Contains(searchInputValA) || x.Subitem.Contains(searchInputValA) || x.Type.Contains(searchInputValA) || x.UserName.Contains(searchInputValA) || x.TechApprovedUser.Contains(searchInputValA) || x.ComApprovedUser.Contains(searchInputValA)) && x.EndTime >= span1 && x.EndTime <= span2;
                }
            }
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindAll(where).ToList();
            string ExcelOutTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filePath = Path.Combine(Server.MapPath("~/NetDisk/"), ExcelOutTime+".xls");
            ExportExcel(applicationInfoes, filePath);
            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            string fileNameTemp = HttpUtility.UrlEncode("Delivery_List.xls", System.Text.Encoding.UTF8);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNameTemp);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
        public ActionResult CreateSavedViewSearch(int? currentPageNum, int? pageSize, string searchValue)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            where = x => (x.Id != null && x.IsDelete == false && x.IsSaved == true && x.UserName == UserName && (x.Project.Contains(searchValue) || x.Site.Contains(searchValue) || x.Stage.Contains(searchValue) || x.Subitem.Contains(searchValue) || x.Type.Contains(searchValue) || x.TechApprovedUser.Contains(searchValue) || x.ComApprovedUser.Contains(searchValue)));
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.arrangeUser = itemApp.ArrangeUser;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                item.operation = "";
                item.approver1 = itemApp.TechApprovedUser;
                item.approver2 = itemApp.ComApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                item.ETD = itemApp.EndTime.ToString().Split(' ')[0];
                item.savetime = itemApp.SavedTime.ToString().Split(' ')[0];
                ProcessingViews.Add(item);
                j++;
            }
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = applicationCount, rows = ProcessingViews };
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public void ExportExcel(List<ApplicationInfo> DataUsers, string filePath)
        {
            try
            {
                if (DataUsers.Count > 0)
                {
                    //设置导出文件路径

                    //设置新建文件路径及名称
                    string savePath = filePath;

                    //创建文件
                    FileStream file = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write);

                    //以指定的字符编码向指定的流写入字符
                    StreamWriter sw = new StreamWriter(file, Encoding.GetEncoding("GB2312"));

                    StringBuilder strbu = new StringBuilder();

                    strbu.Append("Delivery List" + "\t");
                    strbu.Append(Environment.NewLine);
                    strbu.Append(Environment.NewLine);
                    //写入标题
                    strbu.Append("NO." + "\t" + "Product" + "\t" + "Project" + "\t" + "Type" + "\t" + "Item" + "\t" + "Subitem" + "\t" + "Progress" + "\t" + "Stage" + "\t" + "Site" + "\t" + "Quantity" + "\t" + "TechnicalApproval" + "\t" + "CommercialApproval" + "\t" + "ArrangeDRI" + "\t" + "ETD" + "\t" + "ATD" + "\t" + "UnitPrice" + "\t" + "TotalPrice" + "\t" + "Applicant" + "\t" + "Date" + "\t" + "Comment" + "\t");
                    //加入换行字符串
                    strbu.Append(Environment.NewLine);

                    //写入内容
                    int xuhao = 0;
                    foreach (ApplicationInfo item in DataUsers)
                    {
                        xuhao++;
                        string temp_data = xuhao.ToString() + "\t" + item.Product + "\t" + item.Project + "\t" + item.Type + "\t" + item.Item + "\t" + item.Subitem + "\t" + item.Status + "\t" + item.Stage + "\t" + item.Site + "\t" + item.Num + "\t" + item.TechApprovedUser + "\t" + item.ComApprovedUser + "\t" + item.ArrangeUser + "\t" + item.ATD.ToString().Split(' ')[0] + "\t" + item.EndTime.ToString().Split(' ')[0] + "\t" + item.UnitPrice + "\t" + item.UnitPrice * item.Num+"\t"+ item.UserName + "\t" + item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + item.Description + "\t";
                        strbu.Append(temp_data);
                        strbu.Append(Environment.NewLine);
                    }

                    sw.Write(strbu.ToString());
                    sw.Flush();

                    file.Flush();

                    sw.Close();
                    sw.Dispose();

                    file.Close();
                    file.Dispose();
                }
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); }
        }
        public ActionResult EditView()
        {
            //
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            }
            var roleInfoe1s = from o in ApplicationContext.Set<RoleInfo>()
                                 .Include("Permissions")
                                 .Include("UserInfoes")
                                 .Where(x => x.Permissions.Any(c => c.Action == "TechnicalApprove"))
                              select o;
            List<UserInfo> Approver1s = new List<UserInfo>();
            foreach (RoleInfo role in roleInfoe1s)
            {
                foreach (UserInfo user in role.UserInfoes)
                {
                    if (!Approver1s.Contains(user))
                    { Approver1s.Add(user); }
                }
            }
            var selectItemList1 = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "/", Selected = true } };
            var selectList1 = new SelectList(Approver1s, "Id", "UserName");
            selectItemList1.AddRange(selectList1);
            ViewBag.Approver1 = selectItemList1;
            var roleInfoe2s = from o in ApplicationContext.Set<RoleInfo>()
                     .Include("Permissions")
                     .Include("UserInfoes")
                     .Where(x => x.Permissions.Any(c => c.Action == "CommercialApprove"))
                              select o;
            List<UserInfo> Approver2s = new List<UserInfo>();
            foreach (RoleInfo role in roleInfoe2s)
            {
                foreach (UserInfo user in role.UserInfoes)
                {
                    if (!Approver2s.Contains(user))
                    { Approver2s.Add(user); }
                }
            }
            var selectItemList2 = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "/", Selected = true } };
            var selectList2 = new SelectList(Approver2s, "Id", "UserName");
            selectItemList2.AddRange(selectList2);
            ViewBag.Approver2 = selectItemList2;
            return View();
        }
        [HttpPost]
        public ActionResult EditView( FormCollection collection)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                var data = Request.Form["editFormData"];
                JObject jobj = JObject.Parse(data);
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                // TODO: Add insert logic here
                int Id = int.Parse(jobj["requirementId"] != null ? jobj["requirementId"].ToString() : string.Empty);
                ApplicationInfo applicationInfo = ApplicationInfoService.Find(x=>x.Id ==Id);
                if (applicationInfo.IsComApproved || applicationInfo.IsTechApproved || applicationInfo.IsCompleted)
                {
                    Response.ContentType = "text/html";
                    Response.Write("<script>alert('This requirement can not be edited');</script>");
                    return CreateProcessingView(currentPageNum, 10);
                }
                else
                {
                    applicationInfo.Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
                    applicationInfo.Site = jobj["site"] != null ? jobj["site"].ToString() : string.Empty;
                    applicationInfo.Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
                    applicationInfo.Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
                    applicationInfo.Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
                    applicationInfo.Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;;
                    applicationInfo.Num = int.Parse(jobj["quantity"] != null ? jobj["quantity"].ToString() : string.Empty);
                    applicationInfo.Stage = jobj["stage"] != null ? jobj["stage"].ToString() : string.Empty;;
                    applicationInfo.IsComApproved = false;
                    int approver1 = Convert.ToInt32(jobj["approver1"] != null ? jobj["approver1"].ToString() : string.Empty);
                    UserInfo approver = UserInfoService.Find(x => x.Id == approver1);
                    applicationInfo.TechApprovedUser = approver.UserName;
                    int approver2 = Convert.ToInt32(jobj["approver2"] != null ? jobj["approver2"].ToString() : string.Empty);
                    approver = UserInfoService.Find(x => x.Id == approver2);
                    applicationInfo.ComApprovedUser = approver.UserName;
                    applicationInfo.Description = jobj["comment"] != null ? jobj["comment"].ToString() : string.Empty;
                    SessionValue sessionValue = Session["SessionValue"] as SessionValue;
                    if (sessionValue.HasTechAppprove && WebSecurity.CurrentUserName == applicationInfo.TechApprovedUser)
                    {
                        applicationInfo.Status = "Commercial Approval";
                        applicationInfo.IsTechApproved = true;
                    }
                    else
                    {
                        applicationInfo.Status = "Technical Approval";
                        applicationInfo.IsTechApproved = false;
                    }
                    ApplicationInfoService.Update(applicationInfo);
                    int count = ApplicationInfoService.SaveChanges();

                }
                return CreateProcessingView(currentPageNum,10);
            }
            catch
            {
                return CreateProcessingView(1, 10);
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "AllRequirements")]
        public ActionResult AllRequirements()
        {
            ViewBag.Progress = Request.Params["Progress"];
            ViewBag.timeInValARS = Request.Params["timeInValARS"];
            ViewBag.startDateARS = Request.Params["startDateARS"];
            ViewBag.endDateARS = Request.Params["endDateARS"];
            ViewBag.Category = Request.Params["Category"];           
            return View();
        }
                
        [RoleAuthorize]
        [Description(No = 1, Name = "AllRequirementsSummary")]
        public ActionResult AllRequirementsSummary()
        {
            return View();
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "CommercialApprove")]
        public ActionResult CommercialApprove(FormCollection collection)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add insert logic here
                int Id = int.Parse(collection["requirementId"]);
                ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                //if (applicationInfo.Status == "Technical Approval")
                //{
                //    applicationInfo.IsTechApproved = true;
                //    applicationInfo.Status = "Commercial Approval";
                //    //发送邮件
                //    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                //    List<UserInfo> userInfoes = new List<UserInfo>();
                //    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                //    userInfoes.Add(userInfo);
                //    EmailTools.SendTechnicalApproval("Technical Approval", userInfoes, applicationInfo, XmlPath);
                //    //发送邮件结束
                //}
                if (applicationInfo.Status == "Commercial Approval")
                {
                    applicationInfo.IsComApproved = true;
                    applicationInfo.Status = "Hexagon Technical Planning";
                    //发送邮件
                    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    List<UserInfo> userInfoes = new List<UserInfo>();
                    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                    userInfoes.Add(userInfo);
                    EmailTools.SendCommercialApproval("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                    //发送邮件结束
                    applicationInfo.TechApprovedUser = WebSecurity.CurrentUserName;
                    ApplicationInfoService.Update(applicationInfo);
                    int count = ApplicationInfoService.SaveChanges();
                }
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return View();
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "TechnicalApprove")]
        public ActionResult TechnicalApprove(FormCollection collection)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add insert logic here
                int Id = int.Parse(collection["requirementId"]);
                ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                if (applicationInfo.Status == "Technical Approval")
                {
                    applicationInfo.IsTechApproved = true;
                    applicationInfo.Status = "Commercial Approval";
                    //发送邮件
                    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    List<UserInfo> userInfoes = new List<UserInfo>();
                    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                    userInfoes.Add(userInfo);
                    EmailTools.SendTechnicalApproval("Technical Approval", userInfoes, applicationInfo, XmlPath);
                    //
                    userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.ComApprovedUser);
                    userInfoes.Clear();
                    userInfoes.Add(userInfo);
                    EmailTools.SendCommercial("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                    //发送邮件结束
                    applicationInfo.TechApprovedUser = WebSecurity.CurrentUserName;
                    ApplicationInfoService.Update(applicationInfo);
                    int count = ApplicationInfoService.SaveChanges();
                }
                //else if (applicationInfo.Status == "Commercial Approval")
                //{
                //    applicationInfo.IsComApproved = true;
                //    applicationInfo.Status = "Hexagon Technical Planning";
                //    //发送邮件
                //    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                //    List<UserInfo> userInfoes = new List<UserInfo>();
                //    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                //    userInfoes.Add(userInfo);
                //    EmailTools.SendCommercialApproval("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                //    //
                //    userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.ArrangeUser);
                //    userInfoes.Clear();
                //    userInfoes.Add(userInfo);
                //    EmailTools.SendArranger("Arrange", userInfoes, applicationInfo, XmlPath);
                //    //发送邮件结束
                //}
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return View();
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "CommercialApproves")]
        public ActionResult CommercialApproves(FormCollection collection)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                SessionValue sessionValue = System.Web.HttpContext.Current.Session["SessionValue"] as SessionValue;
                // TODO: Add insert logic here
                string RequirementIds = collection["RequirementIds"];
                string[] Ids = RequirementIds.Split(',');
                foreach (string item in Ids)
                {
                    int Id = int.Parse(item);
                    ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                    //if (applicationInfo.Status == "Technical Approval" && sessionValue.HasTechAppprove)
                    //{
                    //    applicationInfo.IsTechApproved = true;
                    //    applicationInfo.Status = "Commercial Approval";
                    //    //发送邮件
                    //    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    //    List<UserInfo> userInfoes = new List<UserInfo>();
                    //    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                    //    userInfoes.Add(userInfo);
                    //    EmailTools.SendTechnicalApproval("Technical Approval", userInfoes, applicationInfo, XmlPath);
                    //    //发送邮件结束
                    //    applicationInfo.ComApprovedUser = WebSecurity.CurrentUserName;
                    //    ApplicationInfoService.Update(applicationInfo);
                    //    int count = ApplicationInfoService.SaveChanges();
                    //}
                    if (applicationInfo.Status == "Commercial Approval" && sessionValue.HasComAppprove)
                    {
                        applicationInfo.IsComApproved = true;
                        applicationInfo.Status = "Hexagon Technical Planning";
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                        userInfoes.Add(userInfo);
                        EmailTools.SendCommercialApproval("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                        applicationInfo.ComApprovedUser = WebSecurity.CurrentUserName;
                        ApplicationInfoService.Update(applicationInfo);
                        int count = ApplicationInfoService.SaveChanges();
                    }
                }
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return View();
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "TechnicalApproves")]
        public ActionResult TechnicalApproves(FormCollection collection)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                SessionValue sessionValue = System.Web.HttpContext.Current.Session["SessionValue"] as SessionValue;
                // TODO: Add insert logic here
                string RequirementIds = collection["RequirementIds"];
                string[] Ids = RequirementIds.Split(',');
                foreach (string item in Ids)
                {
                    int Id = int.Parse(item);
                    ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                    if (applicationInfo.Status == "Technical Approval" && sessionValue.HasTechAppprove)
                    {
                        applicationInfo.IsTechApproved = true;
                        applicationInfo.Status = "Commercial Approval";
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                        userInfoes.Add(userInfo);
                        EmailTools.SendTechnicalApproval("Technical Approval", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                        applicationInfo.TechApprovedUser = WebSecurity.CurrentUserName;
                        ApplicationInfoService.Update(applicationInfo);
                        int count = ApplicationInfoService.SaveChanges();
                    }
                    //else if (applicationInfo.Status == "Commercial Approval" && sessionValue.HasComAppprove)
                    //{
                    //    applicationInfo.IsComApproved = true;
                    //    applicationInfo.Status = "Hexagon Technical Planning";
                    //    //发送邮件
                    //    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    //    List<UserInfo> userInfoes = new List<UserInfo>();
                    //    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                    //    userInfoes.Add(userInfo);
                    //    EmailTools.SendCommercialApproval("Commercial Approval", userInfoes, applicationInfo, XmlPath);
                    //    //发送邮件结束
                    //    applicationInfo.TechApprovedUser = WebSecurity.CurrentUserName;
                    //    ApplicationInfoService.Update(applicationInfo);
                    //    int count = ApplicationInfoService.SaveChanges();
                    //}
                }
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return View();
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "Reject")]
        public ActionResult Reject(FormCollection collection)
        {
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add insert logic here
                int Id = int.Parse(collection["RequirementId"]);
                ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                if (applicationInfo.Status == "Technical Approval")
                { 
                    applicationInfo.Status = "Technical Rejection";
                    //发送邮件
                    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    List<UserInfo> userInfoes = new List<UserInfo>();
                    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                    userInfoes.Add(userInfo);
                    EmailTools.SendTechnicalRejection("Technical Rejection", userInfoes, applicationInfo, XmlPath);
                    //发送邮件结束
                }
                else if (applicationInfo.Status == "Commercial Approval")
                { 
                    applicationInfo.Status = "Commercial Rejection";
                    //发送邮件
                    string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                    List<UserInfo> userInfoes = new List<UserInfo>();
                    UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                    userInfoes.Add(userInfo);
                    EmailTools.SendCommercialRejection("Commercial Rejection", userInfoes, applicationInfo, XmlPath);
                    //发送邮件结束
                }
                applicationInfo.IsComApproved = false;
                applicationInfo.IsTechApproved = false;
                ApplicationInfoService.Update(applicationInfo);
                int count = ApplicationInfoService.SaveChanges();
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return CreateProcessingView(currentPageNum, 10);
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "Rejects")]
        public ActionResult Rejects(FormCollection collection)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add insert logic here
                string RequirementIds = collection["RequirementIds"];
                string[] Ids = RequirementIds.Split(',');
                foreach (string item in Ids)
                {
                    int Id = int.Parse(item);
                    ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                    if (applicationInfo.Status == "Technical Approval")
                    { 
                        applicationInfo.Status = "Technical Rejection";
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                        userInfoes.Add(userInfo);
                        EmailTools.SendTechnicalRejection("Technical Rejection", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                    }
                    else if (applicationInfo.Status == "Commercial Approval")
                    { 
                        applicationInfo.Status = "Commercial Rejection";
                        //发送邮件
                        string XmlPath = Server.MapPath("~/EmailTemplate.xml");
                        List<UserInfo> userInfoes = new List<UserInfo>();
                        UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                        userInfoes.Add(userInfo);
                        EmailTools.SendCommercialRejection("Commercial Rejection", userInfoes, applicationInfo, XmlPath);
                        //发送邮件结束
                    }
                    applicationInfo.IsComApproved = false;
                    applicationInfo.IsTechApproved = false;
                    ApplicationInfoService.Update(applicationInfo);
                }
                int count = ApplicationInfoService.SaveChanges();
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return View();
            }
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "Arrange")]
        public ActionResult Arrange(int requirementId,string process)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LoginView", "Account");
                }
                // TODO: Add insert logic here
                int Id = requirementId;
                ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                if (process == "Waiting for CAD" || process == "DFM")
                {
                    applicationInfo.Status = process;
                    if (string.IsNullOrEmpty(applicationInfo.Statuses))
                    { applicationInfo.Statuses = process; }
                    else { applicationInfo.Statuses = applicationInfo.Statuses+","+process; }
                }
                else if (process == "Inventory")
                {                    
                    applicationInfo.Status = "Completion";
                    applicationInfo.IsCompleted = true;
                    applicationInfo.EndTime = DateTime.Now;
                }
                else
                {
                    applicationInfo.Status = "Completion";
                    applicationInfo.IsCompleted = true;
                    applicationInfo.EndTime = DateTime.Parse(process);
                }
                applicationInfo.ArrangeUser = WebSecurity.CurrentUserName;
                ApplicationInfoService.Update(applicationInfo);
                int count = ApplicationInfoService.SaveChanges();
                //发送邮件
                string XmlPath=Server.MapPath("~/EmailTemplate.xml");
                List<UserInfo> userInfoes = new List<UserInfo>();
                UserInfo userInfo = UserInfoService.Find(x => x.UserName == applicationInfo.UserName);
                userInfoes.Add(userInfo);
                EmailTools.SendArrange("Arrange",userInfoes,applicationInfo, XmlPath);
                //发送邮件结束
                string tempPageNum = Request.Params["page"];
                int currentPageNum = Convert.ToInt32(tempPageNum);
                return CreateProcessingView(currentPageNum, 10);
            }
            catch
            {
                return View();
            }
        }
        public ActionResult DetailView()
        {
            return View();
        }

        public ActionResult DateBoxView(string requirementId)
        {
            ViewBag.requirementId = requirementId;
            return View();
        }
        public ActionResult GetCListItemsById(int Id)
        {
            List<CItem> treeList = new List<CItem>();
            string UserName=WebSecurity.CurrentUserName;
            List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == Id && x.IsDelete==false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
            //List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == Id && x.IsDelete==false).ToList();
            treeList.Add(new CItem("/", "/"));
            treeList.Add(new CItem("All", "All"));
            foreach (CListItem info in cityList)
            {
                treeList.Add(new CItem(info.Id.ToString(), info.Text));
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var data = jss.Serialize(treeList);
            return Content(data);
        }
        //public ActionResult GetCListItemsByNameProduct(string Text)
        //{
        //    List<CItem> treeList = new List<CItem>();
        //    string UserName = WebSecurity.CurrentUserName;
        //    List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == 0 && x.Text == Text && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
        //    treeList.Add(new CItem("/", "/"));
        //    foreach (CListItem info in cityList)
        //    {
        //        treeList.Add(new CItem(info.Id.ToString(), info.Text));
        //    }
        //    JavaScriptSerializer jss = new JavaScriptSerializer();
        //    var data = jss.Serialize(treeList);
        //    return Content(data);
        //}
        //public ActionResult GetCListItemsByNameProject(string Text)
        //{
        //    List<CItem> treeList = new List<CItem>();
        //    string UserName = WebSecurity.CurrentUserName;
        //    List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == 0 && x.Text == Text && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
        //    List<CListItem> cityLists = new List<CListItem>();
        //    foreach (CListItem item in cityList)
        //    {
        //       CListItem itemtemp= ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == item.Id && x.Text == Text && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).FirstOrDefault();
        //       cityLists.Add(itemtemp);
        //    }
        //    //List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == Id && x.IsDelete==false).ToList();
        //    treeList.Add(new CItem("/", "/"));
        //    foreach (CListItem info in cityLists)
        //    {
        //        treeList.Add(new CItem(info.Id.ToString(), info.Text));
        //    }
        //    JavaScriptSerializer jss = new JavaScriptSerializer();
        //    var data = jss.Serialize(treeList);
        //    return Content(data);
        //}
        public ActionResult GetAllCList()
        {
            List<CItem> treeList = new List<CItem>();
            string UserName = WebSecurity.CurrentUserName;
            List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == 0 && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
            treeList.Add(new CItem("/", "/"));
            treeList.Add(new CItem("All", "All"));
            foreach (CListItem info in cityList)
            {
                treeList.Add(new CItem(info.Id.ToString(), info.Text));
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var data = jss.Serialize(treeList);
            return Content(data);
        }
        public ActionResult AddCListItem(int? Id,string Text)
        {
            string UserName = WebSecurity.CurrentUserName;
            UserInfo userInfo = UserInfoService.Find(x => x.UserName == UserName);
            int CListItemId = Convert.ToInt32(Id);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //判断当前目录下有没有相同名称的项目
            if (CListItemId == 0)
            {
                List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == 0 && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                if (cityList.Any(x => x.Text == Text))
                {
                    var response0 = new { code = 0 };
                    return Content(jss.Serialize(response0));
                }
            }
            else
            {
                List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == CListItemId && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                if (cityList.Any(x => x.Text == Text))
                {
                    var response0 = new { code = 0 };
                    return Content(jss.Serialize(response0));
                }
            }
            //
            CListItem cListItem = new CListItem();
            cListItem.Text = Text;
            cListItem.ParentId = CListItemId;
            cListItem.UserInfoes.Add(userInfo);
            CListItemService.Add(cListItem);
            //JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                int count = CListItemService.SaveChanges();
            }
            catch
            {
                var response0 = new { code = 0 };
                return Content(jss.Serialize(response0));
            }
            //刷新前端当前selectbox的列表
            List<CItem> treeList = new List<CItem>();
            if (CListItemId == 0)
            {
                List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == 0 && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                treeList.Add(new CItem("/", "/"));
                foreach (CListItem info in cityList)
                {
                    treeList.Add(new CItem(info.Id.ToString(), info.Text));
                }
            }
            else
            {
                List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == CListItemId && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                treeList.Add(new CItem("/", "/"));
                treeList.Add(new CItem("All", "All"));
                foreach (CListItem info in cityList)
                {
                    treeList.Add(new CItem(info.Id.ToString(), info.Text));
                }
            }
            //刷新结束
            var response1 = new { code = 1, data = treeList};
            return Content(jss.Serialize(response1));
        }
        public ActionResult AddCListItems(string product,string project,string type,string item ,string subitem)
        {
            string UserName = WebSecurity.CurrentUserName;
            ApplicationInfo applicationInfo = new ApplicationInfo();
            applicationInfo.Product = product;
            applicationInfo.Project = project;
            applicationInfo.Type = type;
            applicationInfo.Item = item;
            applicationInfo.Subitem = subitem;
            CreateCListItems(applicationInfo);
            //刷新前端当前selectbox的列表
            List<CItem> treeListProduct = new List<CItem>();
            List<CItem> treeListProject = new List<CItem>();
            List<CItem> treeListType = new List<CItem>();
            List<CItem> treeListItem = new List<CItem>();
            List<CItem> treeListSubitem = new List<CItem>();
            //重载Product
            List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == 0 && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
            treeListProduct.Add(new CItem("/", "/"));
            foreach (CListItem info in cityList)
            {
                treeListProduct.Add(new CItem(info.Id.ToString(), info.Text));
            }
            CListItem cListItem = new CListItem();
            //重载Project
            cListItem = CListItemService.Find(x => x.ParentId == 0 && x.IsDelete == false && x.Text == product);
            treeListProject.Add(new CItem("/", "/"));
            if (cListItem != null)
            {
                cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == cListItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                foreach (CListItem info in cityList)
                {
                    treeListProject.Add(new CItem(info.Id.ToString(), info.Text));
                }

                //重载Type
                cListItem = CListItemService.Find(x => x.ParentId == cListItem.Id && x.IsDelete == false && x.Text == project);
                treeListType.Add(new CItem("/", "/"));
                if (cListItem != null)
                {
                    cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == cListItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                    foreach (CListItem info in cityList)
                    {
                        treeListType.Add(new CItem(info.Id.ToString(), info.Text));
                    }

                    //重载Item
                    cListItem = CListItemService.Find(x => x.ParentId == cListItem.Id && x.IsDelete == false && x.Text == type);
                    treeListItem.Add(new CItem("/", "/"));
                    if (cListItem != null)
                    {
                        cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == cListItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                        foreach (CListItem info in cityList)
                        {
                            treeListItem.Add(new CItem(info.Id.ToString(), info.Text));
                        }

                        //重载Subitem
                        cListItem = CListItemService.Find(x => x.ParentId == cListItem.Id && x.IsDelete == false && x.Text == item);
                        treeListSubitem.Add(new CItem("/", "/"));
                        if (cListItem != null)
                        {
                            cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == cListItem.Id && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
                            foreach (CListItem info in cityList)
                            {
                                treeListSubitem.Add(new CItem(info.Id.ToString(), info.Text));
                            }
                        }
                    }
                }
            }
            var response1 = new { code = 1, dataProduct = treeListProduct, dataProject = treeListProject, dataType = treeListType, dataItem = treeListItem, dataSubitem = treeListSubitem };
            return Content(Jss.Serialize(response1));

        }
        /// <summary>
        /// 删除Create页面中的指定Combox项
        /// </summary>
        /// <param name="Id">对应的CListItem的Id</param>
        /// <returns></returns>
        public ActionResult DeleteCListItem(int? Id)
        {
            string UserName = WebSecurity.CurrentUserName;
            int CListItemId = Convert.ToInt32(Id);

            CListItem cListItem = CListItemService.Find(x => x.Id == CListItemId);
            cListItem.IsDelete = true;
            CListItemService.Update(cListItem);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                int count = CListItemService.SaveChanges();
            }
            catch
            {
                var response0 = new { code = 0 };
                return Content(jss.Serialize(response0));
            }
            //刷新前端当前selectbox的列表
            List<CItem> treeList = new List<CItem>();
            List<CListItem> cityList = ApplicationContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == cListItem.ParentId && x.IsDelete == false && x.UserInfoes.Any(c => c.UserName == UserName)).ToList();
            treeList.Add(new CItem("/", "/"));
            treeList.Add(new CItem("All", "All"));
            foreach (CListItem info in cityList)
            {
                treeList.Add(new CItem(info.Id.ToString(), info.Text));
            }     
            //刷新结束
            var response1 = new { code = 1, data = treeList };
            return Content(jss.Serialize(response1));
        }
        /// <summary>
        /// 编辑Create页面中的指定Combox项
        /// </summary>
        /// <param name="Id">对应的CListItem的Id</param>
        /// <param name="Text">对应的CListItem文本</param>
        /// <returns></returns>
        public ActionResult EditCListItem(int? Id,string Text)
        {

            int CListItemId = Convert.ToInt32(Id);

            CListItem cListItem = CListItemService.Find(x => x.Id == CListItemId);
            cListItem.Text = Text;
            CListItemService.Update(cListItem);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                int count = CListItemService.SaveChanges();
            }
            catch
            {
                var response0 = new { code = 0 };
                return Content(jss.Serialize(response0));
            }
            var response1 = new { code = 1 };
            return Content(jss.Serialize(response1));
        }
        /// <summary>
        /// 返回UnitPriceView
        /// </summary>
        /// <returns></returns>
        public ActionResult UnitPriceView()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            } 
            return View();
        }
        public ActionResult SetUnitPrice(string unitePriceVal, string requirementIds)
        {
            string[] Ids = requirementIds.Split(',');
            foreach (string item in Ids)
            {
                int Id = Convert.ToInt32(item);
                ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                applicationInfo.UnitPrice = Convert.ToInt32(unitePriceVal);
                ApplicationInfoService.Update(applicationInfo);
                int count = ApplicationInfoService.SaveChanges();
            }
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            return CreateAllView(currentPageNum, 10);
        }
        public ActionResult DeliveryDateView()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LoginView", "Account");
            } 
            return View();
        }
        public ActionResult SetDeliveryDate(string deliveryDate, string requirementIds)
        {
            string[] Ids = requirementIds.Split(',');
            foreach (string item in Ids)
            {
                int Id =Convert.ToInt32(item);
                ApplicationInfo applicationInfo = ApplicationInfoService.Find(x => x.Id == Id);
                applicationInfo.ATD = DateTime.Parse(deliveryDate);
                ApplicationInfoService.Update(applicationInfo);
                int count=ApplicationInfoService.SaveChanges();
            }
            string tempPageNum = Request.Params["page"];
            int currentPageNum = Convert.ToInt32(tempPageNum);
            return CreateAllView(currentPageNum, 10);

        }
        public ActionResult SubmitEmailUsers(HttpPostedFileBase[] fileToUpload)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            if (fileToUpload[0] != null)
            {
                Stream inputStream = fileToUpload[0].InputStream;
                string fileSaveFolder = Server.MapPath("~/App_Data/EmailUsers");
                //如果目标不存在，则创建
                if (!Directory.Exists(fileSaveFolder))
                {
                    Directory.CreateDirectory(fileSaveFolder);
                }
                byte[] buffer = new byte[inputStream.Length];
                inputStream.Read(buffer, 0, buffer.Length);
                string strFileMd5 = Md5Helper.Encrypt(buffer);
                //名称格式一律以md5命名。
                string strNewName = strFileMd5 + Path.GetExtension(fileToUpload[0].FileName);
                string fileSavePath = Path.Combine(fileSaveFolder, strNewName);
                if (!System.IO.File.Exists(fileSavePath))
                {
                    fileToUpload[0].SaveAs(fileSavePath);
                }
                ImportEmailUserExcel(fileSavePath);
            }
            else
            {
                var response0 = new { code = 0 };
                var Data0 = Jss.Serialize(response0);
                return Content(Data0);
            }
            var response1 = new { code = 1 };
            var Data1 = Jss.Serialize(response1);
            return Content(Data1);
        }
        public void DownloadEmailUsers()
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();

            string fileToOpen = Server.MapPath("~/App_Data/EmailUsers.xlsx");

            string filePath = ExportEmailUserExcel(fileToOpen);
            if (!string.IsNullOrEmpty(filePath))
            {
                //以字符流的形式下载文件
                FileStream fs = new FileStream(filePath, FileMode.Open);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                Response.ContentType = "application/octet-stream";
                //通知浏览器下载文件而不是打开
                string fileNameTemp = HttpUtility.UrlEncode("EmailUsers.xlsx", System.Text.Encoding.UTF8);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNameTemp);
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }
        private void ImportEmailUserExcel(string FilePath)
        {
            Excel.Application ExcelApp;
            Excel.Workbooks xlsWorkBooks;
            Excel.Workbook xlsWorkBook;
            Excel.Worksheet xlsWorkSheet;
            //
            //设置程序运行语言
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //创建Application
            ExcelApp = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application")) as Excel.Application;
            //设置是否显示警告窗体
            ExcelApp.DisplayAlerts = false;
            //设置是否显示Excel
            ExcelApp.Visible = false;
            //禁止刷新屏幕
            ExcelApp.ScreenUpdating = false;
            xlsWorkBooks = ExcelApp.Workbooks;
            xlsWorkBook = xlsWorkBooks.Open(FilePath);
            xlsWorkSheet = xlsWorkBook.Worksheets[1];
            try
            {
                int num = xlsWorkSheet.UsedRange.Rows.Count;
                bool start = false;
                List<EmailDelivery> EmailUsers = new List<EmailDelivery>();
                for (int i = 1; i <= num; i++)
                {
                    if (xlsWorkSheet.Cells[i, 1].Value.ToString() == "NO.")
                    { start = true; i++; }
                    if (!start)
                    { continue; }
                    EmailDelivery emailUser = new EmailDelivery();
                    emailUser.UserName = xlsWorkSheet.Cells[i, 2].Value.ToString();
                    emailUser.Email = xlsWorkSheet.Cells[i, 3].Value.ToString();
                    EmailUsers.Add(emailUser);
                }
                xmlHelp.SetEmailDeliveryVariables(EmailUsers);
                xlsWorkBook.Close();
                xlsWorkBook = null;
                xlsWorkBooks.Close();
                xlsWorkBooks = null;
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); }
            finally
            {
                if (xlsWorkBook != null)
                {
                    //xlsWorkBook.Close();
                    xlsWorkBook = null;
                }
                if (xlsWorkBooks != null)
                {
                    //xlsWorkBooks.Close();
                    xlsWorkBooks = null;
                }
                if (ExcelApp != null)
                {
                    CloseExcelDLSB(ExcelApp);
                }
            }
        }
        private string ExportEmailUserExcel(string FilePath)
        {
            Excel.Application ExcelApp;
            Excel.Workbooks xlsWorkBooks;
            Excel.Workbook xlsWorkBook;
            Excel.Worksheet xlsWorkSheet;
            //
            //设置程序运行语言
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //创建Application
            ExcelApp = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application")) as Excel.Application;
            //设置是否显示警告窗体
            ExcelApp.DisplayAlerts = false;
            //设置是否显示Excel
            ExcelApp.Visible = false;
            //禁止刷新屏幕
            ExcelApp.ScreenUpdating = false;
            xlsWorkBooks = ExcelApp.Workbooks;
            xlsWorkBook = xlsWorkBooks.Add(FilePath);
            xlsWorkSheet = xlsWorkBook.Worksheets[1];
            try
            {
                var emailUsers = xmlHelp.GetEmailDeliveryVariables();
                int i = 0;
                foreach (EmailDelivery item in emailUsers)
                {
                    //No.
                    xlsWorkSheet.Cells[2 + i, 1].Value = (i+1).ToString();
                    //UserName
                    xlsWorkSheet.Cells[2 + i, 2].Value = item.UserName;
                    //Email
                    xlsWorkSheet.Cells[2 + i, 3].Value = item.Email;
                    i++;
                }

                string fileToSave = Server.MapPath("~/App_Data/EmailUsers");
                fileToSave = Path.Combine(fileToSave, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx");
                xlsWorkBook.SaveAs(fileToSave);
                xlsWorkBook.Close();
                xlsWorkBook = null;
                xlsWorkBooks.Close();
                xlsWorkBooks = null;
                return fileToSave;
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); return ""; }
            finally
            {
                if (xlsWorkBook != null)
                {
                    //xlsWorkBook.Close();
                    xlsWorkBook = null;
                }
                if (xlsWorkBooks != null)
                {
                    //xlsWorkBooks.Close();
                    xlsWorkBooks = null;
                }
                if (ExcelApp != null)
                {
                    CloseExcelDLSB(ExcelApp);
                }
            }
        }
        private void CloseExcelDLSB(Excel.Application xlApp)
        {
            try
            {
                xlApp.Quit();
                IntPtr hwnd = new IntPtr(xlApp.Hwnd);
                int iD = 0;
                GetWindowThreadProcessId(hwnd, out iD);
                Process.GetProcessById(iD).Kill();
                xlApp = null;
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); }
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public ActionResult CreateSector(string searchValARS)
        {
            searchValARS = searchValARS != null ? searchValARS : "{}";
            JObject jobj = JObject.Parse(searchValARS);

            string interval = jobj["timeInVal"] != null ? jobj["timeInVal"].ToString() : string.Empty;
            string startDateA = jobj["startVal"] != null ? jobj["startVal"].ToString() : string.Empty;
            string endDateA = jobj["endVal"] != null ? jobj["endVal"].ToString() : string.Empty;
            string Product = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
            string Project = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
            string Type = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
            string Item = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
            string Subitem = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;

            DateTime now = DateTime.Now;
            DateTime span1;
            DateTime span2;
            switch (interval)
            {
                case "1 Month":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "3 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-90);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "6 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-180);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "1 Year":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-365);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
            }
            Expression<Func<ApplicationInfo, bool>> where = null;
            if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA) && string.IsNullOrEmpty(interval) && interval != "/")
            {
                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem);

                if ((Product != "All" && Product != "/") && (Project != "All" && Project != "/") && (Type != "All" && Type != "/") && (Item != "All" && Item != "/") && (Subitem != "All" && Subitem != "/"))
                {
                    where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem);
                }
                if (Product == "All" || Project == "All" || Type == "All" || Item == "All" || Subitem == "All")
                {
                    if (Product == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false;
                    }
                    if (Project == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null;
                    }
                    if (Type == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null;
                    }
                    if (Item == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null;
                    }
                    if (Subitem == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null;
                    }
                }
                if (Product == "/" || Project == "/" || Type == "/" || Item == "/" || Subitem == "/")
                {
                    if (Product == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product==Product;
                    }
                    if (Project == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product==Product && x.Project=="" && x.Type=="" && x.Item=="" && x.Subitem=="";
                    }
                    if (Type == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product==Product && x.Project==Project && x.Type=="" && x.Item=="" && x.Subitem=="";
                    }
                    if (Item == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product==Product && x.Project==Project && x.Type==Type && x.Item=="" && x.Subitem=="";
                    }
                    if (Subitem == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product==Product && x.Project==Project && x.Type==Type && x.Item==Item && x.Subitem=="";
                    }
                }
            }
            else
            {
                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem);
                if ((Product != "All" && Product != "/") && (Project != "All" && Project != "/") && (Type != "All" && Type != "/") && (Item != "All" && Item != "/") && (Subitem != "All" && Subitem != "/"))
                {
                    where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem);
                }
                if (Product == "All" || Project == "All" || Type == "All" || Item == "All" || Subitem == "All")
                {
                    if (Product == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2;
                    }
                    if (Project == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project != "" && x.Project != null;
                    }
                    if (Type == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null;
                    }
                    if (Item == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null;
                    }
                    if (Subitem == "All")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null;
                    }
                }
                if (Product == "/" || Project == "/" || Type == "/" || Item == "/" || Subitem == "/")
                {
                    if (Product == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product;
                    }
                    if (Project == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "";
                    }
                    if (Type == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "";
                    }
                    if (Item == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "";
                    }
                    if (Subitem == "/")
                    {
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "";
                    }
                }
            }
            var Apps= ApplicationInfoService.FindAll(where);
            int count=Apps.Count();
            var AppTechnicalRejection = Apps.Where(x => x.Status == "Technical Rejection");
            var AppCommercialRejection = Apps.Where(x => x.Status == "Commercial Rejection");
            var AppTechnicalApproval = Apps.Where(x => x.Status == "Technical Approval");
            var AppCommercialApproval = Apps.Where(x => x.Status == "Commercial Approval");
            var AppArrange = Apps.Where(x => x.Status == "Waiting for CAD" || x.Status == "DFM" || x.Status == "Hexagon Technical Planning");
            var AppCompletion = Apps.Where(x => x.Status == "Completion");
            List<Object> objects = new List<object>()
            { new { value = AppTechnicalRejection.Count(), name = "Technical Rejection" } 
            ,new { value = AppCommercialRejection.Count(), name = "Commercial Rejection" } 
            ,new { value = AppTechnicalApproval.Count(), name = "Technical Approval" } 
            ,new { value = AppCommercialApproval.Count(), name = "Commercial Approval" } 
            ,new { value = AppArrange.Count(), name = "Hexagon Technical Planning" } 
            ,new { value = AppCompletion.Count(), name = "Completion" } };

            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var Data = Jss.Serialize(objects);
            var content = Content(Data);
            return Content(Data);
        }
        public Expression<Func<ApplicationInfo, bool>> GetWhere(string Product, string Project, string Type, string Item, string Subitem, string Status)
        {

            Expression<Func<ApplicationInfo, bool>> where = null;

            if ((Product != "All" && Product != "/") && (Project != "All" && Project != "/") && (Type != "All" && Type != "/") && (Item != "All" && Item != "/") && (Subitem != "All" && Subitem != "/"))
            {
                switch (Status)
                {
                    case "Technical Rejection":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Technical Rejection";
                        break;
                    case "Commercial Rejection":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Commercial Rejection";
                        break;
                    case "Technical Approval":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Technical Approval";
                        break;
                    case "Commercial Approval":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Commercial Approval";
                        break;
                    case "Hexagon Technical Planning":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                        break;
                    case "Completion":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Completion";
                        break;
                    default :
                        break;
                }
            }
            if (Product == "All" || Project == "All" || Type == "All" || Item == "All" || Subitem == "All")
            {
                if (Product == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                }
                if (Project == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null;
                }
                if (Type == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null;
                }
                if (Item == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null;
                }
                if (Subitem == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                   //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null;
                }
            }
            if (Product == "/" || Project == "/" || Type == "/" || Item == "/" || Subitem == "/")
            {
                if (Product == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product;
                }
                if (Project == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "";
                }
                if (Type == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "";
                }
                if (Item == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "";
                }
                if (Subitem == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Technical Rejection";
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Commercial Rejection";
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Technical Approval";
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Commercial Approval";
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD");
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Completion";
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "";
                }
            }
            return where;
        }
        public Expression<Func<ApplicationInfo, bool>> GetWhere(string Product, string Project, string Type, string Item, string Subitem, string Status,DateTime span1,DateTime span2)
        {

            Expression<Func<ApplicationInfo, bool>> where = null;

            if ((Product != "All" && Product != "/") && (Project != "All" && Project != "/") && (Type != "All" && Type != "/") && (Item != "All" && Item != "/") && (Subitem != "All" && Subitem != "/"))
            {
                switch (Status)
                {
                    case "Technical Rejection":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                        break;
                    case "Commercial Rejection":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                        break;
                    case "Technical Approval":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                        break;
                    case "Commercial Approval":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                        break;
                    case "Hexagon Technical Planning":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                        break;
                    case "Completion":
                        where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Product) && x.Project.Contains(Project) && x.Type.Contains(Type) && x.Item.Contains(Item) && x.Subitem.Contains(Subitem) && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                        break;
                    default:
                        break;
                }
            }
            if (Product == "All" || Project == "All" || Type == "All" || Item == "All" || Subitem == "All")
            {
                if (Product == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                }
                if (Project == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project != "" && x.Project != null;
                }
                if (Type == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type != "" && x.Type != null;
                }
                if (Item == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item != "" && x.Item != null;
                }
                if (Subitem == "All")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem != "" && x.Subitem != null;
                }
            }
            if (Product == "/" || Project == "/" || Type == "/" || Item == "/" || Subitem == "/")
            {
                if (Product == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product;
                }
                if (Project == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "";
                }
                if (Type == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "" && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == "" && x.Item == "" && x.Subitem == "";
                }
                if (Item == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "" && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == "" && x.Subitem == "";
                }
                if (Subitem == "/")
                {
                    switch (Status)
                    {
                        case "Technical Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Technical Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Rejection":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Commercial Rejection" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Technical Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Technical Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Commercial Approval":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Commercial Approval" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Hexagon Technical Planning":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && (x.Status == "Hexagon Technical Planning" || x.Status == "DFM" || x.Status == "Waiting for CAD") && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        case "Completion":
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "" && x.Status == "Completion" && x.CreateTime >= span1 && x.CreateTime <= span2;
                            break;
                        default:
                            break;
                    }
                    //where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == "";
                }
            }
            return where;
        }
        public ActionResult CreateBar(string searchValARS)
        {
            searchValARS = searchValARS != null ? searchValARS : "{}";
            JObject jobj = JObject.Parse(searchValARS);

            string interval = jobj["timeInVal"] != null ? jobj["timeInVal"].ToString() : string.Empty;
            string startDateA = jobj["startVal"] != null ? jobj["startVal"].ToString() : string.Empty;
            string endDateA = jobj["endVal"] != null ? jobj["endVal"].ToString() : string.Empty;
            string Productstr = jobj["product"] != null ? jobj["product"].ToString() : string.Empty;
            string Projectstr = jobj["project"] != null ? jobj["project"].ToString() : string.Empty;
            string Typestr = jobj["type"] != null ? jobj["type"].ToString() : string.Empty;
            string Itemstr = jobj["item"] != null ? jobj["item"].ToString() : string.Empty;
            string Subitemstr = jobj["subitem"] != null ? jobj["subitem"].ToString() : string.Empty;
            DateTime now = DateTime.Now;
            DateTime span1;
            DateTime span2;
            switch (interval)
            {
                case "1 Month":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "3 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-90);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "6 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-180);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "1 Year":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-365);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
            }
            Expression<Func<ApplicationInfo, bool>> wheretemp = null;
            Func<ApplicationInfo, bool> where = null;
            if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA) && string.IsNullOrEmpty(interval) && interval != "/")
            {
                wheretemp = x => x.Id != null && x.IsDelete == false && x.IsSaved == false;
            }
            else
            {
                wheretemp = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2;
            }
            List<ApplicationInfo> Apps = ApplicationInfoService.FindAll(wheretemp).ToList();
            List<string> objectBarNames = new List<string>();
            List<string> objectBarValues = new List<string>();
            if (string.IsNullOrEmpty(Productstr))
            {
                for (int i = 0; i < Apps.Count; i++)
                {
                    try
                    {
                        List<ApplicationInfo> temps = Apps.Where(x => x.Product == Apps[i].Product && x.Project == Apps[i].Project && x.Type == Apps[i].Type && x.Item == Apps[i].Item && x.Subitem == Apps[i].Subitem).ToList();
                        if (temps.Count > 0)
                        {
                            string requirement = Apps[i].Product + (string.IsNullOrEmpty(Apps[i].Project) ? "" : "/" + Apps[i].Project) + (string.IsNullOrEmpty(Apps[i].Type) ? "" : "/" + Apps[i].Type) + (string.IsNullOrEmpty(Apps[i].Item) ? "" : "/" + Apps[i].Item) + (string.IsNullOrEmpty(Apps[i].Subitem) ? "" : "/" + Apps[i].Subitem);
                            if (!objectBarNames.Any(x => x == requirement))
                            {
                                objectBarNames.Add(requirement);
                                objectBarValues.Add(temps.Count.ToString());
                            }
                            else { continue; }

                        }
                    }
                    catch (Exception ex)
                    { ExceptionLogHelp.WriteLog(ex); }
                }
            }
            else
            {
                try
                {
                    if ((Productstr != "All" && Productstr != "/") && (Projectstr != "All" && Projectstr != "/") && (Typestr != "All" && Typestr != "/") && (Itemstr != "All" && Itemstr != "/") && (Subitemstr != "All" && Subitemstr != "/"))
                    {
                        where = x => x.IsDelete == false && x.IsSaved == false && x.Product.Contains(Productstr) && x.Project.Contains(Projectstr) && x.Type.Contains(Typestr) && x.Item.Contains(Itemstr) && x.Subitem.Contains(Subitemstr);
                    }
                    if (Productstr == "All" || Projectstr == "All" || Typestr == "All" || Itemstr == "All" || Subitemstr == "All")
                    {
                        if (Productstr == "All")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false;
                        }
                        if (Projectstr == "All")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project != "" && x.Project != null;
                        }
                        if (Typestr == "All")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project == Projectstr && x.Type != "" && x.Type != null;
                        }
                        if (Itemstr == "All")
                        {
                            where = x =>x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project == Projectstr && x.Type == Typestr && x.Item != "" && x.Item != null;
                        }
                        if (Subitemstr == "All")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project == Projectstr && x.Type == Typestr && x.Item == Itemstr && x.Subitem != "" && x.Subitem != null;
                        }
                    }
                    if (Productstr == "/" || Projectstr == "/" || Typestr == "/" || Itemstr == "/" || Subitemstr == "/")
                    {
                        if (Productstr == "/")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr;
                        }
                        if (Projectstr == "/")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project == "" && x.Type == "" && x.Item == "" && x.Subitem == "";
                        }
                        if (Typestr == "/")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project == Projectstr && x.Type == "" && x.Item == "" && x.Subitem == "";
                        }
                        if (Itemstr == "/")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project == Projectstr && x.Type == Typestr && x.Item == "" && x.Subitem == "";
                        }
                        if (Subitemstr == "/")
                        {
                            where = x => x.IsDelete == false && x.IsSaved == false && x.Product == Productstr && x.Project == Projectstr && x.Type == Typestr && x.Item == Itemstr && x.Subitem == "";
                        }
                    }
                    List<ApplicationInfo> temps = Apps.Where(where).ToList();
                    if (temps.Count > 0)
                    {
                        foreach (ApplicationInfo appItem in temps)
                        {
                            var tempsl = Apps.Where(x => x.Product == appItem.Product && x.Project == appItem.Project && x.Type == appItem.Type && x.Item == appItem.Item && x.Subitem == appItem.Subitem).ToList();
                            if (tempsl.Count > 0)
                            {
                                string requirements = appItem.Product + (string.IsNullOrEmpty(appItem.Project) ? "" : "/" + appItem.Project) + (string.IsNullOrEmpty(appItem.Type) ? "" : "/" + appItem.Type) + (string.IsNullOrEmpty(appItem.Item) ? "" : "/" + appItem.Item) + (string.IsNullOrEmpty(appItem.Subitem) ? "" : "/" + appItem.Subitem);
                                if (!objectBarNames.Any(x => x == requirements))
                                {
                                    objectBarNames.Add(requirements);
                                    objectBarValues.Add(tempsl.Count.ToString());
                                }
                                else { continue; }
                            }
                        }
                        //}
                    }
                }
                catch (Exception ex)
                { ExceptionLogHelp.WriteLog(ex); }
            }

            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var Data = Jss.Serialize(new { names = objectBarNames, values = objectBarValues });
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult CreateProgressViewSearch(int? currentPageNum, int? pageSize, string timeInValARS, string startDateARS, string endDateARS, string Progress, string Category)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string interval = timeInValARS;
            string startDateA = startDateARS;
            string endDateA = endDateARS;
            DateTime now = DateTime.Now;
            DateTime span1;
            DateTime span2;
            switch (interval)
            {
                case "1 Month":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "3 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-90);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "6 Months":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-180);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                case "1 Year":
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-365);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
                default:
                    //无interval
                    if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA))
                    {
                        span1 = now.AddDays(-30);
                        span2 = now;
                    }
                    else
                    {
                        span1 = DateTime.Parse(startDateA);
                        span2 = DateTime.Parse(endDateA);
                    }
                    break;
            }
            //
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, applicationCount;
            Expression<Func<ApplicationInfo, bool>> where = null; 
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            if (string.IsNullOrEmpty(startDateA) && string.IsNullOrEmpty(endDateA) && string.IsNullOrEmpty(interval) && interval !="/")
            {
                switch (Progress)
                {
                    case "Technical Rejection":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Technical Rejection";
                        }
                        else 
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Technical Rejection");
                        }
                        break;
                    case "Commercial Rejection":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Commercial Rejection";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Commercial Rejection");
                        }
                        break;
                    case "Technical Approval":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Technical Approval";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Technical Approval");
                        }
                        break;
                    case "Commercial Approval":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Commercial Approval";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Commercial Approval");
                        }
                        break;
                    case "Hexagon Technical Planning":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Hexagon Technical Planning";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Hexagon Technical Planning");
                        }
                        break;
                    case "Completion":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Status == "Completion";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Completion");
                        }
                        break;
                    default:
                        if (Progress.Split('/').Count() >= 1)
                        {
                            if (Progress.Split('/').Count() == 1)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = "";
                                string Type = "";
                                string Item = "";
                                string Subitem = ""; 
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project ==Project && x.Type==Type && x.Item ==Item && x.Subitem ==Subitem;
                            }
                            else if (Progress.Split('/').Count() == 2)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = "";
                                string Item = "";
                                string Subitem = "";
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem;
                            }
                            else if (Progress.Split('/').Count() == 3)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = Progress.Split('/')[2];
                                string Item = "";
                                string Subitem = "";
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem;
                            }
                            else if (Progress.Split('/').Count() == 4)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = Progress.Split('/')[2];
                                string Item = Progress.Split('/')[3];
                                string Subitem = "";
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem;
                            }
                            else if (Progress.Split('/').Count() == 5)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = Progress.Split('/')[2];
                                string Item = Progress.Split('/')[3];
                                string Subitem = Progress.Split('/')[4]; 
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem;
                            }
                            else { where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false; }
                        }
                        else { where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false; }
                        break;
                }

            }
            else
            {
                switch (Progress)
                {
                    case "Technical Rejection":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Status == "Technical Rejection";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Technical Rejection", span1, span2);
                        }
                        break;
                    case "Commercial Rejection":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Status == "Commercial Rejection";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Commercial Rejection", span1, span2);
                        }
                        break;
                    case "Technical Approval":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Status == "Technical Approval";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Technical Approval", span1, span2);
                        }
                        break;
                    case "Commercial Approval":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Status == "Commercial Approval";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Commercial Approval", span1, span2);
                        }
                        break;
                    case "Hexagon Technical Planning":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Status == "Hexagon Technical Planning";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Hexagon Technical Planning", span1, span2);
                        }
                        break;
                    case "Completion":
                        if (string.IsNullOrEmpty(Category))
                        {
                            where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.CreateTime >= span1 && x.CreateTime <= span2 && x.Status == "Completion";
                        }
                        else
                        {
                            string[] categories = Category.Split(',');
                            where=GetWhere(categories[0], categories[1], categories[2], categories[3], categories[4], "Completion", span1, span2);
                        }
                        break;
                    default:
                        if (Progress.Split('/').Count() > 1)
                        {
                            if (Progress.Split('/').Count() == 1)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = "";
                                string Type = "";
                                string Item = "";
                                string Subitem = "";
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem && x.CreateTime >= span1 && x.CreateTime <= span2;
                            }
                            else if (Progress.Split('/').Count() == 2)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = "";
                                string Item = "";
                                string Subitem = "";
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem && x.CreateTime >= span1 && x.CreateTime <= span2;
                            }
                            else if (Progress.Split('/').Count() == 3)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = Progress.Split('/')[2];
                                string Item = "";
                                string Subitem = "";
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem && x.CreateTime >= span1 && x.CreateTime <= span2;
                            }
                            else if (Progress.Split('/').Count() == 4)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = Progress.Split('/')[2];
                                string Item = Progress.Split('/')[3];
                                string Subitem = "";
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem && x.CreateTime >= span1 && x.CreateTime <= span2;
                            }
                            else if (Progress.Split('/').Count() == 5)
                            {
                                string Product = Progress.Split('/')[0];
                                string Project = Progress.Split('/')[1];
                                string Type = Progress.Split('/')[2];
                                string Item = Progress.Split('/')[3];
                                string Subitem = Progress.Split('/')[4];
                                where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false && x.Product == Product && x.Project == Project && x.Type == Type && x.Item == Item && x.Subitem == Subitem && x.CreateTime >= span1 && x.CreateTime <= span2;
                            }
                            else { where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false; }
                        }
                        else { where = x => x.Id != null && x.IsDelete == false && x.IsSaved == false; }
                        break;
                }

            }
            whereDateTime = x => x.CreateTime;
            var applicationInfoes = ApplicationInfoService.FindPaged(pageSize.Value, ref pageNum, out applicationCount, out pageCount, where, false, whereDateTime).ToList();

            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 1;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.arrangeUser = itemApp.ArrangeUser;
                item.serialNumber = j.ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                item.approver1 = itemApp.ComApprovedUser;
                item.approver2 = itemApp.TechApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                item.ETD = itemApp.EndTime.ToString().Split(' ')[0];
                ProcessingViews.Add(item);
                j++;
            }
            var response = new { total = applicationCount, rows = ProcessingViews };
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult GetCurrentTaskNum()
        {
            SessionValue sessionValue = System.Web.HttpContext.Current.Session["SessionValue"] as SessionValue;
            string CurrentTask = Request.Params["CurrentTask"];
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            Expression<Func<ApplicationInfo, bool>> where = null;
            where = x => (x.Id != null && x.IsDelete == false && x.IsCompleted == false && x.IsSaved == false);
            var applicationInfoes = ApplicationInfoService.FindAll(where).ToList();
            List<ProcessingView> ProcessingViews = new List<ProcessingView>();
            int j = 0;
            foreach (ApplicationInfo itemApp in applicationInfoes)
            {
                ProcessingView item = new ProcessingView();
                item.product = itemApp.Product;
                item.subitem = itemApp.Subitem;
                item.serialNumber = (j+1).ToString();
                item.project = itemApp.Project;
                item.item = itemApp.Item;
                item.type = itemApp.Type;
                item.stage = itemApp.Stage;
                item.site = itemApp.Site;
                item.progress = itemApp.Status;
                item.quantity = itemApp.Num.ToString();
                item.postuser = itemApp.UserName;
                item.createtime = itemApp.CreateTime.ToString().Split(' ')[0];
                item.comment = "<span class=\"easyui-tooltip\" title=\"" + itemApp.Description + "\">" + itemApp.Description + "</span>";
                //Tech和Com都审批完的状态
                if (itemApp.IsComApproved && itemApp.IsTechApproved)
                {
                    if (sessionValue.HasArrange)
                    {
                        if (itemApp.Status == "Waiting for CAD")
                        {
                            if (itemApp.Statuses.Contains("DFM"))
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Production</option><option>Inventory</option></select> <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                            else
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>DFM</option><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                        }
                        else if (itemApp.Status == "DFM")
                        {
                            if (itemApp.Statuses.Contains("Waiting for CAD"))
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                            else
                            {
                                item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Waiting for CAD</option><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                            }
                        }
                        else
                        {
                            item.operation = "<select id='arrangeSelect" + itemApp.Id.ToString() + "'><option>Waiting for CAD</option><option>DFM</option><option>Production</option><option>Inventory</option></select>  <input value=\"Arrange\" type=\"button\" class=\"processArrangeBtn\" style=\"background:#428BCA\"  onclick=\"arrangeFun(" + itemApp.Id.ToString() + ");\"/>";
                        }
                    }
                    else 
                    {
                        if (itemApp.UserName == UserName && (itemApp.Status == "Technical Rejection" || itemApp.Status == "Commercial Rejection"))
                        { }
                        else { continue; }
                    }
                }
                //提交后需要审批的状态
                else if (!itemApp.IsComApproved || !itemApp.IsTechApproved)
                {
                    if (itemApp.IsTechApproved)
                    {
                        //如果技术审核后，有商务审核权限那么就赋予商务审核按钮和拒绝按钮
                        if (sessionValue.HasComAppprove && itemApp.Status != "Technical Rejection" && itemApp.Status != "Commercial Rejection" && itemApp.ComApprovedUser == UserName)
                        {

                            item.operation = "<input value=\"Approve\" type=\"button\" class=\"processApproveBtn\" style=\"background:#5CB85C\"  onclick=\"approveComFun(" + itemApp.Id.ToString() + ");\"/><input value=\"Reject\" type=\"button\" class=\"processRejectBtn\"  style=\"background:#D9534F\"  onclick=\"rejectFun(" + itemApp.Id.ToString() + ");\"/>";
                        }
                        else 
                        { 
                            //item.operation = ""; 
                            continue;
                        }

                    }
                    else
                    {
                        //提交后开始技术审批，如果有技术审核权限那么就赋予技术审核按钮和拒绝按钮
                        if (sessionValue.HasTechAppprove && itemApp.Status != "Technical Rejection" && itemApp.Status != "Commercial Rejection" && itemApp.TechApprovedUser == UserName)
                        {
                            item.operation = "<input value=\"Approve\" type=\"button\" class=\"processApproveBtn\" style=\"background:#5CB85C\"  onclick=\"approveTechFun(" + itemApp.Id.ToString() + ");\"/><input value=\"Reject\" type=\"button\" class=\"processRejectBtn\"  style=\"background:#D9534F\"  onclick=\"rejectFun(" + itemApp.Id.ToString() + ");\"/>";

                        }
                        else 
                        { 
                           // item.operation = "";
                            if (itemApp.UserName == UserName && (itemApp.Status == "Technical Rejection" || itemApp.Status == "Commercial Rejection"))
                            { }
                            else { continue; }
                        }
                    }
                }
                item.approver1 = itemApp.TechApprovedUser;
                item.approver2 = itemApp.ComApprovedUser;
                item.requirementId = itemApp.Id.ToString();
                ProcessingViews.Add(item);
                j++;
            }
            var CurrentNum = new { currentNum = j };
            return Content(Jss.Serialize(CurrentNum));
        }
    }
}
