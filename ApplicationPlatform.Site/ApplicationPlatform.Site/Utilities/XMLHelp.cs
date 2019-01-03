using ApplicationPlatform.Utilities;
using ApplicationPlatform.Utilities.NodeModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace ApplicationPlatform.Site.Utilities
{
    public class XMLHelp
    {
        public XmlDocument doc;
        public XmlNode root;
        public XmlNode Delivery;
        public XmlNodeList Deliveries;
        public string Path;
        public static string GetTechnicalApproval(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "technicalApproval")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetTechnicalRejection(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "technicalRejection")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetTechnical(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "technical")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetCommercialApproval(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "commercialApproval")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetCommercialRejection(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "commercialRejection")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetCommercial(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "commercial")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetArrange(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "arrange")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetArranger(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "arranger")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetResetPassword(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "resetpassword")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetRegister(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "register")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetSignature(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "signature")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public static string GetAttachment(string path)
        {
            string uploadEmail = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "attachment")
                    {

                        uploadEmail = reader.ReadElementString();
                        break;
                    }

                }

            }
            return uploadEmail;
        }
        public XMLHelp()
        {
            try
            {
                string path = System.Environment.CurrentDirectory;
                Path = Directory.GetParent(path) + "\\Settings\\XmlNode.xml";
                doc = new XmlDocument();
                doc.Load(Path);
                root = doc.SelectSingleNode("HexagonTemplate");
                Delivery = doc.SelectSingleNode("HexagonTemplate/EmailDelivery");
                Deliveries = Delivery.ChildNodes;
            }
            catch (Exception ex)
            {
                ExceptionLogHelp.WriteLog(ex);
            }
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