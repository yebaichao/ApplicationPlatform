using ApplicationPlatform.BLL;
using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Utilities;
using ApplicationPlatform.Utilities.NodeModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ApplicationPlatform.Site.Utilities
{
    public class EmailTools
    {
        /// <summary>
        /// 发送消息给申请人已经技术审核完毕
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendTechnicalApproval(string MailSubject,List<UserInfo> UserInfoes,ApplicationInfo Requirement,string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetTechnicalApproval(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.TechApprovedUser);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        /// <summary>
        /// 发消息给申请人已经商务审核完毕
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendCommercialApproval(string MailSubject, List<UserInfo> UserInfoes, ApplicationInfo Requirement, string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetTechnicalApproval(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.ComApprovedUser);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        /// <summary>
        /// 发送消息给申请人已经技术拒绝
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendTechnicalRejection(string MailSubject, List<UserInfo> UserInfoes, ApplicationInfo Requirement, string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetTechnicalRejection(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.TechApprovedUser);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        /// <summary>
        /// 发消息给技术审批人有新的申请需要审批
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendTechnical(string MailSubject, List<UserInfo> UserInfoes, ApplicationInfo Requirement, string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetTechnical(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.UserName);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        /// <summary>
        /// 发消息给申请人已经商务拒绝
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendCommercialRejection(string MailSubject, List<UserInfo> UserInfoes, ApplicationInfo Requirement, string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetCommercialRejection(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.ComApprovedUser);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        /// <summary>
        /// 发消息给商务审批人有新的申请需要审批
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendCommercial(string MailSubject, List<UserInfo> UserInfoes, ApplicationInfo Requirement, string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetCommercial(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.UserName);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        /// <summary>
        /// 发消息给申请人已经安排对应阶段
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendArrange(string MailSubject, List<UserInfo> UserInfoes, ApplicationInfo Requirement, string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetArrange(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.ArrangeUser);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        /// <summary>
        /// 发消息给安排人有新的申请需要安排
        /// </summary>
        /// <param name="MailSubject"></param>
        /// <param name="UserInfoes"></param>
        /// <param name="Requirement"></param>
        /// <param name="Path"></param>
        public static void SendArranger(string MailSubject, List<UserInfo> UserInfoes, ApplicationInfo Requirement, string Path)
        {
            Email email = new Email();
            email.mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            email.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            email.mailSubject = MailSubject;
            string path = Path;
            string emailBody = XMLHelp.GetArranger(path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(path);
            emailSignature.Replace("\r\n", "<br/>");
            emailBody = emailBody.Replace("UserName", Requirement.UserName);
            string requirement = Requirement.Product + (string.IsNullOrEmpty(Requirement.Project) ? "" : "/" + Requirement.Project) + (string.IsNullOrEmpty(Requirement.Type) ? "" : "/" + Requirement.Type) + (string.IsNullOrEmpty(Requirement.Item) ? "" : "/" + Requirement.Item) + (string.IsNullOrEmpty(Requirement.Subitem) ? "" : "/" + Requirement.Subitem);
            emailBody = emailBody.Replace("RequirementReplace", requirement);
            emailBody = emailBody.Replace("StageReplace", Requirement.Stage);
            emailBody = emailBody.Replace("QuantityReplace", Requirement.Num.ToString());
            emailBody = emailBody.Replace("SiteReplace", Requirement.Site);
            emailBody = emailBody.Replace("comment", Requirement.Description);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = ConfigurationManager.AppSettings["mailHost"];//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (UserInfo item in UserInfoes)
            {
                if (item.IsDelete == true) { continue; }
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
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
        public static void SendEmailUsers(string MailSubject, List<EmailDelivery> UserInfoes, string Path)
        {
            Email email = new Email();
            email.mailFrom = "baichao.ye@hexagon.com";
            email.mailPwd = "";
            email.mailSubject = MailSubject;
            string emailBody = XMLHelp.GetAttachment(Path);
            emailBody = emailBody.Replace("\r\n", "<br/>");
            string emailSignature = XMLHelp.GetSignature(Path);
            emailSignature.Replace("\r\n", "<br/>");
            //emailBody = emailBody.Replace("UserName", Requirement.UserName);
            email.mailBody = emailBody + emailSignature;
            email.isbodyHtml = true;    //是否是HTML
            email.host = "smtp1.hexagonmetrology.com";//如果是QQ邮箱则：smtp:qq.com,依次类推

            List<string> emailAddress = new List<string>();
            foreach (EmailDelivery item in UserInfoes)
            {
                if (!emailAddress.Contains(item.Email))
                {
                    emailAddress.Add(item.Email);
                }
            }
            email.mailToArray = new string[emailAddress.Count];//接收者邮件集合
            for (int j = 0; j < emailAddress.Count; j++)
            {
                email.mailToArray[j] = emailAddress[j];
            }
            //email.mailCcArray = new string[] { "120698234@qq.com" };//抄送者邮件集合
            email.attachmentsPath = new string[1];
            //邮件附件集合
            email.attachmentsPath[0] = ExportEmailUsersExcel();
            if (emailAddress.Count > 0)
            {
                Thread thread = new Thread(new ThreadStart(email.Send));
                thread.Start();
            }
        }
        public static string ExportEmailUsersExcel()
        {
            Expression<Func<ApplicationInfo, bool>> where = null;
            Expression<Func<ApplicationInfo, DateTime>> whereDateTime = null;
            where = x => x.Id != null && x.IsDelete == false;            
            whereDateTime = x => x.CreateTime;
            IApplicationInfoServiceRepository ApplicationInfoService = new ApplicationInfoServiceRepository();
            var applicationInfoes = ApplicationInfoService.FindAll(where).ToList();
            string ExcelOutTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string path = System.Environment.CurrentDirectory;
            string filePath = Directory.GetParent(path) + "\\NetDisk\\" + ExcelOutTime + ".xls";
            ExportExcel(applicationInfoes, filePath);
            return filePath;
        }
        public static void ExportExcel(List<ApplicationInfo> DataUsers, string filePath)
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

                    strbu.Append("All Requirements" + "\t");
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
                        string temp_data = xuhao.ToString() + "\t" + item.Product + "\t" + item.Project + "\t" + item.Type + "\t" + item.Item + "\t" + item.Subitem + "\t" + item.Status + "\t" + item.Stage + "\t" + item.Site + "\t" + item.Num + "\t" + item.TechApprovedUser + "\t" + item.ComApprovedUser + "\t" + item.ArrangeUser + "\t" + item.ATD.ToString().Split(' ')[0] + "\t" + item.EndTime.ToString().Split(' ')[0] + "\t" + item.UnitPrice + "\t" + item.UnitPrice * item.Num + "\t" + item.UserName + "\t" + item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + item.Description + "\t";
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
        //public static string ExportPermissionExcel()
        //{
        //    string ExcelOutTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        //    string path = System.Environment.CurrentDirectory;
        //    string filePath = Directory.GetParent(path) + "\\NetDisk\\" + ExcelOutTime + ".xls";
        //    ExportPermissionExcel(filePath);
        //    return filePath;
        //}
        public static void ExportPermissionExcel(string filePath)
        {
            AppContext appContext = ContextFactory.GetDbContext() as AppContext;
            List<CListItem> Products = new List<CListItem>();
            List<CListItem> Projects = new List<CListItem>();
            List<CListItem> Types = new List<CListItem>();
            List<CListItem> Items = new List<CListItem>();
            List<CListItem> Subitems = new List<CListItem>();

            List<CListItem> CListItemProducts = appContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == 0 && x.IsDelete==false).ToList();
            foreach (CListItem product in CListItemProducts)
            {
                List<CListItem> CListItemProjects = appContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == product.Id && x.IsDelete == false).ToList();
                if (CListItemProjects.Count > 0)
                {
                    foreach (CListItem project in CListItemProjects)
                    {
                        List<CListItem> CListItemTypes = appContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == project.Id && x.IsDelete == false).ToList();
                        if (CListItemTypes.Count > 0)
                        {
                            foreach (CListItem type in CListItemTypes)
                            {
                                List<CListItem> CListItemItems = appContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == type.Id && x.IsDelete == false).ToList();
                                if (CListItemItems.Count > 0)
                                {
                                    foreach (CListItem item in CListItemTypes)
                                    {
                                        List<CListItem> CListItemSubitems = appContext.Set<CListItem>().Include("UserInfoes").Where(x => x.ParentId == item.Id && x.IsDelete == false).ToList();
                                        if (CListItemSubitems.Count > 0)
                                        {
                                            foreach (CListItem subitem in CListItemSubitems)
                                            {
                                                Products.Add(product);
                                                Projects.Add(project);
                                                Types.Add(type);
                                                Items.Add(item);
                                                Subitems.Add(subitem);
                                            }
                                        }
                                        else
                                        {
                                            Products.Add(product);
                                            Projects.Add(project);
                                            Types.Add(type);
                                            Items.Add(item); 
                                            Subitems.Add(new CListItem());
                                        }
                                        //Items.Add(item);
                                    }
                                }
                                else
                                {
                                    Products.Add(product);
                                    Projects.Add(project);
                                    Types.Add(type);
                                    Items.Add(new CListItem());
                                    Subitems.Add(new CListItem());
                                }
                                //Types.Add(type);
                            }
                        }
                        else
                        {
                            Products.Add(product);
                            Projects.Add(project);
                            Types.Add(new CListItem());
                            Items.Add(new CListItem());
                            Subitems.Add(new CListItem());
                        }
                        //Projects.Add(project);
                    }
                }
                else {
                    Products.Add(product);
                    Projects.Add(new CListItem());
                    Types.Add(new CListItem());
                    Items.Add(new CListItem());
                    Subitems.Add(new CListItem());
                }
                //Products.Add(product);
            }
            try
            {
                if (Products.Count > 0)
                {
                    //设置导出文件路径

                    //设置新建文件路径及名称
                    string savePath = filePath;

                    //创建文件
                    FileStream file = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write);

                    //以指定的字符编码向指定的流写入字符
                    StreamWriter sw = new StreamWriter(file, Encoding.GetEncoding("GB2312"));

                    StringBuilder strbu = new StringBuilder();

                    strbu.Append("Permission Details" + "\t");
                    strbu.Append(Environment.NewLine);
                    strbu.Append(Environment.NewLine);
                    //写入标题
                    strbu.Append("NO." + "\t" + "Product" + "\t" + "Project" + "\t" + "Type" + "\t" + "Item" + "\t" + "Subitem" + "\t" + "Users" + "\t");
                    //加入换行字符串
                    strbu.Append(Environment.NewLine);

                    //写入内容
                    int xuhao = 0;
                    for (int i = 0; i < Products.Count;i++ )
                    {
                        xuhao++;
                        string users = "";
                        if (Products[i].UserInfoes.Count > 0)
                        {
                            foreach (UserInfo userInfo in Products[i].UserInfoes)
                            {
                                if (users == "")
                                { users = userInfo.UserName;}
                                else
                                {
                                    users = users + ";" + userInfo.UserName;
                                }
                            }
                        }
                        if (Projects[i].UserInfoes.Count > 0)
                        {
                            users = "";
                            foreach (UserInfo userInfo in Projects[i].UserInfoes)
                            {
                                if (users == "")
                                { users = userInfo.UserName; }
                                else
                                {
                                    users = users + ";" + userInfo.UserName;
                                }
                            }
                        }
                        if (Types[i].UserInfoes.Count > 0)
                        {
                            users = "";
                            foreach (UserInfo userInfo in Types[i].UserInfoes)
                            {
                                if (users == "")
                                { users = userInfo.UserName; }
                                else
                                {
                                    users = users + ";" + userInfo.UserName;
                                }
                            }
                        }
                        if (Items[i].UserInfoes.Count > 0)
                        {
                            users = "";
                            foreach (UserInfo userInfo in Items[i].UserInfoes)
                            {
                                if (users == "")
                                { users = userInfo.UserName; }
                                else
                                {
                                    users = users + ";" + userInfo.UserName;
                                }
                            }
                        }
                        if (Subitems[i].UserInfoes.Count > 0)
                        {
                            users = "";
                            foreach (UserInfo userInfo in Subitems[i].UserInfoes)
                            {
                                if (users == "")
                                { users = userInfo.UserName; }
                                else
                                {
                                    users = users + ";" + userInfo.UserName;
                                }
                            }
                        }
                        string temp_data = xuhao.ToString() + "\t" + Products[i].Text + "\t" + Projects[i].Text + "\t" + Types[i].Text + "\t" + Items[i].Text + "\t" + Subitems[i].Text + "\t" + users + "\t";
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
    }
}