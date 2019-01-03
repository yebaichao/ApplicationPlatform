using ApplicationPlatform.Utilities.NodeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;



namespace ApplicationPlatform.Utilities
{
    public class XmlHelp
    {
        public XmlDocument doc;
        public XmlNode root;
        public XmlNode Sites;
        public XmlNodeList SiteNodes;
        public XmlNode Approvers;
        public XmlNodeList ApproverNodes;
        public XmlNode Delivery;
        public XmlNodeList Deliveries;
        public string Path;
        
        public XmlHelp()
        {
            try
            {
                Path = System.Web.HttpContext.Current.Server.MapPath("~/Settings/XmlNode.xml");
                doc = new XmlDocument();
                doc.Load(Path);
                root = doc.SelectSingleNode("HexagonTemplate");
                Sites = doc.SelectSingleNode("HexagonTemplate/Sites");
                SiteNodes = Sites.ChildNodes;
                Approvers = doc.SelectSingleNode("HexagonTemplate/Approvers");
                ApproverNodes = Approvers.ChildNodes;
                Delivery = doc.SelectSingleNode("HexagonTemplate/EmailDelivery");
                Deliveries = Delivery.ChildNodes;
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
            }
        }
        public List<Site> GetSiteVariables()
        {
            List<Site> Variables = new List<Site>();
            XmlElement xe;
            foreach (XmlNode xn1 in SiteNodes)
            {
                Site variable = new Site();
                // 将节点转换为元素，便于得到节点的属性值
                xe = (XmlElement)xn1;
                // 获取特证名
                variable.id = xe.GetAttribute("id");
                variable.site = xe.GetAttribute("site");
                Variables.Add(variable);
            }
            return Variables;
        }
        public string GetSiteName(string id)
        {
            string name = "";
            XmlElement xe;
            foreach (XmlNode xn1 in SiteNodes)
            {
                // 将节点转换为元素，便于得到节点的属性值
                xe = (XmlElement)xn1;
                // 获取特证名
                string ID=xe.GetAttribute("id");
                if (id == ID)
                {
                    name = xe.GetAttribute("site");
                    return name;
                }
            }
            return name;
        }
        public List<Approver> GetApproverVariables()
        {
            List<Approver> Variables = new List<Approver>();
            XmlElement xe;
            foreach (XmlNode xn1 in ApproverNodes)
            {
                Approver variable = new Approver();
                // 将节点转换为元素，便于得到节点的属性值
                xe = (XmlElement)xn1;
                // 获取特证名
                variable.id = xe.GetAttribute("id");
                variable.name = xe.GetAttribute("name");
                Variables.Add(variable);
            }
            return Variables;
        }
        public string GetApproverName(string id)
        {
            string name = "";
            XmlElement xe;
            foreach (XmlNode xn1 in ApproverNodes)
            {
                // 将节点转换为元素，便于得到节点的属性值
                xe = (XmlElement)xn1;
                // 获取特证名
                string ID = xe.GetAttribute("id");
                if (id == ID)
                {
                    name = xe.GetAttribute("name");
                    return name;
                }
            }
            return name;
        }
        public List<EmailDelivery> GetEmailDeliveryVariables()
        {
            List<EmailDelivery> Variables = new List<EmailDelivery>();
            XmlElement xe;
            foreach (XmlNode xn1 in Deliveries)
            {
                EmailDelivery variable = new EmailDelivery();
                // 将节点转换为元素，便于得到节点的属性值
                xe = (XmlElement)xn1;
                // 获取特证名
                variable.UserName = xe.GetAttribute("UserName");
                variable.Email = xe.GetAttribute("Email");
                Variables.Add(variable);
            }
            return Variables;
        }
        public void SetEmailDeliveryVariables(List<EmailDelivery> Variables)
        {
            Delivery.RemoveAll();
            XmlElement xe;
            foreach (EmailDelivery xn1 in Variables)
            {
                XmlNode xmlNode = doc.CreateElement("element", "User","");
                xe = (XmlElement)xmlNode;
                xe.SetAttribute("UserName", xn1.UserName);
                xe.SetAttribute("Email", xn1.Email);
                Delivery.AppendChild(xmlNode);
            }
            doc.Save(Path);
        }
    }
}