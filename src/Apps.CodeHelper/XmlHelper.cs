using System;
 using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

//引用命名空间
using System.Xml;
 
namespace Apps.CodeHelper
{
/// <summary>
/// XmlOp类提供对XML数据库的读写
/// </summary>
    public class XmlHelper
    {
        //public static void SetXmlFileValue(string xmlPath, string AppKey, string AppValue)//写xmlPath是文件路径+文件名，AppKey是 Key Name，AppValue是Value
        public static void SetXmlFileValue(string xmlPath, string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlPath);
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;

            xNode = xDoc.SelectSingleNode("//appSettings");

            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
            {
                xElem1.SetAttribute("value", AppValue);
            }
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(xmlPath);
        }
        //public static void GetXmlFileValue(string xmlPath, string AppKey, ref string AppValue)//读xmlPath是文件路径+文件名，AppKey是 Key Name，AppValue是Value
        public static string GetXmlFileValue(string xmlPath, string AppKey)
        {
            string strValue = "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlPath);
            XmlNode xNode;
            XmlElement xElem1;

            xNode = xDoc.SelectSingleNode("//appSettings");

            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
            {
                strValue = xElem1.GetAttribute("value");
            }
            else
            {
                // MessageBox.Show ("There is not any information!";
            }
            return strValue;
        }
    }
}