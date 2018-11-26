using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

/// <summary>
/// 控制器
/// </summary>
namespace Apps.CodeHelper
{
    public partial class CodeFrom
    {
        public string GetController(string tableName)
        {
            string leftStr = GetLeftStr(tableName);
            List<CompleteField> fields = SqlHelper.GetColumnCompleteField(conn, tableName);
            string table1BLL = "m_" + (txt_TableName1.Text.Trim().IndexOf("_") > 0 ? txt_TableName1.Text.Trim().Split('_')[1] : txt_TableName1.Text.Trim());
            string parentTable1 = txt_TableName1.Text.Trim();

            StringBuilder sb = new StringBuilder();
            sb.Append("using System.Collections.Generic;\r\n");
            sb.Append("using System.Linq;\r\n");
            sb.Append("using Apps.Web.Core;\r\n");
            sb.Append("using Apps.IBLL" + (leftStr == "Sys" ? "" : ("." + leftStr)) + ";\r\n");
            sb.Append("using Apps.Locale;\r\n");
            sb.Append("using System.Web.Mvc;\r\n");
            sb.Append("using " + txt_prefix.Text + ".Common;\r\n");
            sb.Append("using " + txt_prefix.Text + ".IBLL;\r\n");
            sb.Append("using " + txt_prefix.Text + ".Models." + leftStr + ";\r\n");
            sb.Append("using Unity.Attributes;\r\n");
            sb.Append("using Newtonsoft.Json.Linq;\r\n");
            sb.Append("using Newtonsoft.Json;\r\n");
            sb.Append("using System;\r\n");
            sb.Append("using System.Data;\r\n");
            sb.Append("\r\n");
            sb.Append("namespace " + txt_prefix.Text + ".Web" + (leftStr == "Sys" ? "" : (".Areas." + leftStr)) + ".Controllers\r\n");
            sb.Append("{\r\n");
            sb.Append("    public class " + tableName.Replace(leftStr + "_", "") + "Controller : BaseController\r\n");
            sb.Append("    {\r\n");
            sb.Append("        [Dependency]\r\n");
            sb.Append("        public I" + tableName + "BLL m_BLL { get; set; }\r\n");

            //启用表关联
            if (cb_EnableParent.Checked)
            {
                //表1
                if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                {
                    sb.Append("        [Dependency]\r\n");
                    sb.Append("        public I" + txt_TableName1.Text.Trim() + "BLL " + table1BLL + "BLL { get; set; }\r\n");
                }

            }

            sb.Append("        ValidationErrors errors = new ValidationErrors();\r\n");
            sb.Append("        \r\n");
            sb.Append("        [SupportFilter]\r\n");
            sb.Append("        public ActionResult Index()\r\n");
            sb.Append("        {\r\n");
            sb.Append("            return View();\r\n");
            sb.Append("        }\r\n");

            sb.Append("        [HttpPost]\r\n");
            sb.Append("        [SupportFilter(ActionName=\"Index\")]\r\n");

            if (cb_EnableParent.Checked)
            {

                sb.Append("        public JsonResult GetList(GridPager pager, string queryStr,string parentId)\r\n");
                sb.Append("        {\r\n");
                sb.Append("            List<" + tableName + "Model> list = m_BLL.GetListByParentId(ref pager, queryStr,parentId);\r\n");
                sb.Append("            GridRows<" + tableName + "Model> grs = new GridRows<" + tableName + "Model>();\r\n");
                sb.Append("            grs.rows = list;\r\n");
                sb.Append("            grs.total = pager.totalRows;\r\n");
                sb.Append("            return Json(grs);\r\n");
                sb.Append("        }\r\n");
            }
            else
            {
                sb.Append("        public JsonResult GetList(GridPager pager, string queryStr)\r\n");
                sb.Append("        {\r\n");
                sb.Append("            List<" + tableName + "Model> list = m_BLL.GetList(ref pager, queryStr);\r\n");
                sb.Append("            GridRows<" + tableName + "Model> grs = new GridRows<" + tableName + "Model>();\r\n");
                sb.Append("            grs.rows = list;\r\n");
                sb.Append("            grs.total = pager.totalRows;\r\n");
                sb.Append("            return Json(grs);\r\n");
                sb.Append("        }\r\n");
            }

            #region 子表

            sb.Append("        #region 创建\r\n");
            sb.Append("        [SupportFilter]\r\n");
            sb.Append("        public ActionResult Create()\r\n");
            sb.Append("        {\r\n");
            //启用表关联
            if (cb_EnableParent.Checked)
            {
                //表1
                if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                {
                    sb.Append("         ViewBag." + table1BLL.Replace("m_", "") + " = new SelectList(" + table1BLL + "BLL.GetList(ref setNoPagerAscById, \"\"), \"Id\", \"Name\");\r\n");
                }

            }
            sb.Append("            return View();\r\n");
            sb.Append("        }\r\n");
            sb.Append("\r\n");
            sb.Append("        [HttpPost]\r\n");
            sb.Append("        [SupportFilter]\r\n");
            sb.Append("        public JsonResult Create(" + tableName + "Model model)\r\n");
            sb.Append("        {\r\n");
            if (fields[0].xType != "56" && fields[0].xType != "127")//非int型主键
            {
                sb.Append("            model.Id = ResultHelper.NewId;\r\n");
            }
            else
            {
                sb.Append("            model.Id = 0;\r\n");
            }
            sb.Append("            model.CreateTime = ResultHelper.NowTime;\r\n");
            sb.Append("            if (model != null && ModelState.IsValid)\r\n");
            sb.Append("            {\r\n");
            sb.Append("\r\n");
            sb.Append("                if (m_BLL.Create(ref errors, model))\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + model." + fields[0].name + " + \"," + fields[1].name + "\" + model." + fields[1].name + ", \"成功\", \"创建\", \"" + tableName + "\");\r\n");
            sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));\r\n");
            sb.Append("                }\r\n");
            sb.Append("                else\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    string ErrorCol = errors.Error;\r\n");
            sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + model." + fields[0].name + " + \"," + fields[1].name + "\" + model." + fields[1].name + " + \",\" + ErrorCol, \"失败\", \"创建\", \"" + tableName + "\");\r\n");
            sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));\r\n");
            sb.Append("                }\r\n");
            sb.Append("            }\r\n");
            sb.Append("            else\r\n");
            sb.Append("            {\r\n");
            sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));\r\n");
            sb.Append("            }\r\n");
            sb.Append("        }\r\n");
            sb.Append("        #endregion\r\n");
            sb.Append("\r\n");
            sb.Append("        #region 修改\r\n");
            sb.Append("        [SupportFilter]\r\n");
            if (fields[0].xType != "56" && fields[0].xType != "127")//非int型主键
            {
                sb.Append("        public ActionResult Edit(string id)\r\n");
            }
            else
            {
                sb.Append("        public ActionResult Edit(long id)\r\n");
            }
            sb.Append("        {\r\n");

            sb.Append("            " + tableName + "Model entity = m_BLL.GetById(id);\r\n");
            //启用表关联
            if (cb_EnableParent.Checked)
            {
                //表1
                if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                {
                    sb.Append("         ViewBag." + table1BLL.Replace("m_", "") + " = new SelectList(" + table1BLL + "BLL.GetList(ref setNoPagerAscById, \"\"), \"Id\", \"Name\",entity." + txt_TableKey1.Text + ");\r\n");
                }

            }
            sb.Append("            return View(entity);\r\n");
            sb.Append("        }\r\n");
            sb.Append("\r\n");
            sb.Append("        [HttpPost]\r\n");
            sb.Append("        [SupportFilter]\r\n");
            sb.Append("        public JsonResult Edit(" + tableName + "Model model)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            if (model != null && ModelState.IsValid)\r\n");
            sb.Append("            {\r\n");
            sb.Append("\r\n");
            sb.Append("                if (m_BLL.Edit(ref errors, model))\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + model." + fields[0].name + " + \"," + fields[1].name + "\" + model." + fields[1].name + ", \"成功\", \"修改\", \"" + tableName + "\");\r\n");
            sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));\r\n");
            sb.Append("                }\r\n");
            sb.Append("                else\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    string ErrorCol = errors.Error;\r\n");
            sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + model." + fields[0].name + " + \"," + fields[1].name + "\" + model." + fields[1].name + " + \",\" + ErrorCol, \"失败\", \"修改\", \"" + tableName + "\");\r\n");
            sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));\r\n");
            sb.Append("                }\r\n");
            sb.Append("            }\r\n");
            sb.Append("            else\r\n");
            sb.Append("            {\r\n");
            sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));\r\n");
            sb.Append("            }\r\n");
            sb.Append("        }\r\n");
            sb.Append("        #endregion\r\n");
            sb.Append("\r\n");
            sb.Append("        #region 详细\r\n");
            sb.Append("        [SupportFilter]\r\n");
            if (fields[0].xType != "56" && fields[0].xType != "127")//非int型主键
            {
                sb.Append("        public ActionResult Details(string id)\r\n");
            }
            else
            {
                sb.Append("        public ActionResult Details(long id)\r\n");
            }
            sb.Append("        {\r\n");
            sb.Append("            " + tableName + "Model entity = m_BLL.GetById(id);\r\n");
            sb.Append("            return View(entity);\r\n");
            sb.Append("        }\r\n");
            sb.Append("\r\n");
            sb.Append("        #endregion\r\n");
            sb.Append("\r\n");
            sb.Append("        #region 删除\r\n");
            sb.Append("        [HttpPost]\r\n");
            sb.Append("        [SupportFilter]\r\n");

            if (fields[0].xType != "56" && fields[0].xType != "127")//非int型主键
            {
                sb.Append("        public ActionResult Delete(string id)\r\n");
                sb.Append("        {\r\n");
                sb.Append("            if(!string.IsNullOrWhiteSpace(id))\r\n");
                sb.Append("            {\r\n");
                sb.Append("                if (m_BLL.Delete(ref errors, id))\r\n");
                sb.Append("                {\r\n");
                sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + ":\" + id, \"成功\", \"删除\", \"" + tableName + "\");\r\n");
                sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));\r\n");
                sb.Append("                }\r\n");
                sb.Append("                else\r\n");
                sb.Append("                {\r\n");
                sb.Append("                    string ErrorCol = errors.Error;\r\n");
                sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + id + \",\" + ErrorCol, \"失败\", \"删除\", \"" + tableName + "\");\r\n");
                sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));\r\n");
                sb.Append("                }\r\n");
                sb.Append("            }\r\n");
                sb.Append("            else\r\n");
                sb.Append("            {\r\n");
                sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));\r\n");
                sb.Append("            }\r\n");
                sb.Append("        }\r\n");
            }
            else
            {
                sb.Append("        public ActionResult Delete(long id)\r\n");
                sb.Append("        {\r\n");
                sb.Append("            if(id!=0)\r\n");
                sb.Append("            {\r\n");
                sb.Append("                if (m_BLL.Delete(ref errors, id))\r\n");
                sb.Append("                {\r\n");
                sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + ":\" + id, \"成功\", \"删除\", \"" + tableName + "\");\r\n");
                sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));\r\n");
                sb.Append("                }\r\n");
                sb.Append("                else\r\n");
                sb.Append("                {\r\n");
                sb.Append("                    string ErrorCol = errors.Error;\r\n");
                sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + id + \",\" + ErrorCol, \"失败\", \"删除\", \"" + tableName + "\");\r\n");
                sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));\r\n");
                sb.Append("                }\r\n");
                sb.Append("            }\r\n");
                sb.Append("            else\r\n");
                sb.Append("            {\r\n");
                sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));\r\n");
                sb.Append("            }\r\n");
                sb.Append("        }\r\n");
            }


            sb.Append("        #endregion\r\n");
            sb.Append("\r\n");

            //导入导出
            GetImportExportCodeForController(ref sb, tableName, fields);

            #region 父表

            if (cb_EnableParent.Checked)
            {

                List<CompleteField> parentFields = SqlHelper.GetColumnCompleteField(conn, txt_TableName1.Text.Trim());

                sb.Append("        [HttpPost]\r\n");
                sb.Append("        [SupportFilter(ActionName=\"Index\")]\r\n");
                sb.Append("        public JsonResult GetListParent(GridPager pager, string queryStr)\r\n");
                sb.Append("        {\r\n");
                sb.Append("            List<" + parentTable1 + "Model> list = " + table1BLL + "BLL.GetList(ref pager, queryStr);\r\n");
                sb.Append("            GridRows<" + parentTable1 + "Model> grs = new GridRows<" + parentTable1 + "Model>();\r\n");
                sb.Append("            grs.rows = list;\r\n");
                sb.Append("            grs.total = pager.totalRows;\r\n");
                sb.Append("            return Json(grs);\r\n");
                sb.Append("        }\r\n");
                if (cb_MulView.Checked)
                {
                    sb.Append("        #region 创建\r\n");
                    sb.Append("        [SupportFilter(ActionName=\"Create\")]\r\n");
                    sb.Append("        public ActionResult CreateParent()\r\n");
                    sb.Append("        {\r\n");
                    sb.Append("            return View();\r\n");
                    sb.Append("        }\r\n");
                    sb.Append("\r\n");
                    sb.Append("        [HttpPost]\r\n");
                    sb.Append("        [SupportFilter(ActionName=\"Create\")]\r\n");
                    sb.Append("        public JsonResult CreateParent(" + parentTable1 + "Model model)\r\n");
                    sb.Append("        {\r\n");
                    if (parentFields[0].xType != "56" && fields[0].xType != "127")//非int型主键
                    {
                        sb.Append("            model.Id = ResultHelper.NewId;\r\n");
                    }
                    sb.Append("            model.CreateTime = ResultHelper.NowTime;\r\n");
                    sb.Append("            if (model != null && ModelState.IsValid)\r\n");
                    sb.Append("            {\r\n");
                    sb.Append("\r\n");
                    sb.Append("                if (" + table1BLL + "BLL.Create(ref errors, model))\r\n");
                    sb.Append("                {\r\n");
                    sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + parentFields[0].name + "\" + model." + parentFields[0].name + " + \"," + parentFields[1].name + "\" + model." + parentFields[1].name + ", \"成功\", \"创建\", \"" + parentTable1 + "\");\r\n");
                    sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));\r\n");
                    sb.Append("                }\r\n");
                    sb.Append("                else\r\n");
                    sb.Append("                {\r\n");
                    sb.Append("                    string ErrorCol = errors.Error;\r\n");
                    sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + parentFields[0].name + "\" + model." + parentFields[0].name + " + \"," + parentFields[1].name + "\" + model." + parentFields[1].name + " + \",\" + ErrorCol, \"失败\", \"创建\", \"" + parentTable1 + "\");\r\n");
                    sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));\r\n");
                    sb.Append("                }\r\n");
                    sb.Append("            }\r\n");
                    sb.Append("            else\r\n");
                    sb.Append("            {\r\n");
                    sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));\r\n");
                    sb.Append("            }\r\n");
                    sb.Append("        }\r\n");

                    sb.Append("\r\n");
                    sb.Append("        #region 修改\r\n");
                    sb.Append("        [SupportFilter(ActionName=\"Edit\")]\r\n");
                    if (parentFields[0].xType != "56" && fields[0].xType != "127")//非int型主键
                    {
                        sb.Append("        public ActionResult EditParent(string id)\r\n");
                    }
                    else
                    {
                        sb.Append("        public ActionResult EditParent(long id)\r\n");
                    }

                    sb.Append("        {\r\n");
                    sb.Append("            " + parentTable1 + "Model entity = " + table1BLL + "BLL.GetById(id);\r\n");
                    sb.Append("            return View(entity);\r\n");
                    sb.Append("        }\r\n");
                    sb.Append("\r\n");
                    sb.Append("        [HttpPost]\r\n");
                    sb.Append("        [SupportFilter(ActionName=\"Edit\")]\r\n");
                    sb.Append("        public JsonResult EditParent(" + parentTable1 + "Model model)\r\n");
                    sb.Append("        {\r\n");
                    sb.Append("            if (model != null && ModelState.IsValid)\r\n");
                    sb.Append("            {\r\n");
                    sb.Append("\r\n");
                    sb.Append("                if (" + table1BLL + "BLL.Edit(ref errors, model))\r\n");
                    sb.Append("                {\r\n");
                    sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + parentFields[0].name + "\" + model." + parentFields[0].name + " + \"," + parentFields[1].name + "\" + model." + parentFields[1].name + ", \"成功\", \"修改\", \"" + tableName + "\");\r\n");
                    sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));\r\n");
                    sb.Append("                }\r\n");
                    sb.Append("                else\r\n");
                    sb.Append("                {\r\n");
                    sb.Append("                    string ErrorCol = errors.Error;\r\n");
                    sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + parentFields[0].name + "\" + model." + parentFields[0].name + " + \"," + parentFields[1].name + "\" + model." + parentFields[1].name + " + \",\" + ErrorCol, \"失败\", \"修改\", \"" + tableName + "\");\r\n");
                    sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));\r\n");
                    sb.Append("                }\r\n");
                    sb.Append("            }\r\n");
                    sb.Append("            else\r\n");
                    sb.Append("            {\r\n");
                    sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));\r\n");
                    sb.Append("            }\r\n");
                    sb.Append("        }\r\n");
                    sb.Append("        #endregion\r\n");
                    sb.Append("\r\n");
                    sb.Append("        #region 详细\r\n");
                    sb.Append("        [SupportFilter(ActionName=\"Details\")]\r\n");
                    if (parentFields[0].xType != "56" && fields[0].xType != "127")//非int型主键
                    {
                        sb.Append("        public ActionResult DetailsParent(string id)\r\n");
                    }
                    else
                    {
                        sb.Append("        public ActionResult DetailsParent(long id)\r\n");
                    }
                    sb.Append("        {\r\n");
                    sb.Append("            " + parentTable1 + "Model entity = " + table1BLL + "BLL.GetById(id);\r\n");
                    sb.Append("            return View(entity);\r\n");
                    sb.Append("        }\r\n");
                    sb.Append("\r\n");
                    sb.Append("        #endregion\r\n");
                    sb.Append("\r\n");
                    sb.Append("        #region 删除\r\n");
                    sb.Append("        [HttpPost]\r\n");
                    sb.Append("        [SupportFilter(ActionName=\"Delete\")]\r\n");
                    if (parentFields[0].xType != "56" && fields[0].xType != "127")//非int型主键
                    {
                        sb.Append("        public ActionResult DeleteParent(string id)\r\n");
                        sb.Append("        {\r\n");
                        sb.Append("            if(!string.IsNullOrWhiteSpace(id))\r\n");
                        sb.Append("            {\r\n");
                        sb.Append("                if (" + table1BLL + "BLL.Delete(ref errors, id))\r\n");
                        sb.Append("                {\r\n");
                        sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + ":\" + id, \"成功\", \"删除\", \"" + tableName + "\");\r\n");
                        sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));\r\n");
                        sb.Append("                }\r\n");
                        sb.Append("                else\r\n");
                        sb.Append("                {\r\n");
                        sb.Append("                    string ErrorCol = errors.Error;\r\n");
                        sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + id + \",\" + ErrorCol, \"失败\", \"删除\", \"" + tableName + "\");\r\n");
                        sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));\r\n");
                        sb.Append("                }\r\n");
                        sb.Append("            }\r\n");
                        sb.Append("            else\r\n");
                        sb.Append("            {\r\n");
                        sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));\r\n");
                        sb.Append("            }\r\n");
                        sb.Append("        }\r\n");
                    }
                    else
                    {
                        sb.Append("        public ActionResult DeleteParent(long id)\r\n");
                        sb.Append("        {\r\n");
                        sb.Append("            if(id!=0)\r\n");
                        sb.Append("            {\r\n");
                        sb.Append("                if (" + table1BLL + "BLL.Delete(ref errors, id))\r\n");
                        sb.Append("                {\r\n");
                        sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + ":\" + id, \"成功\", \"删除\", \"" + tableName + "\");\r\n");
                        sb.Append("                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));\r\n");
                        sb.Append("                }\r\n");
                        sb.Append("                else\r\n");
                        sb.Append("                {\r\n");
                        sb.Append("                    string ErrorCol = errors.Error;\r\n");
                        sb.Append("                    LogHandler.WriteServiceLog(GetUserId(), \"" + fields[0].name + "\" + id + \",\" + ErrorCol, \"失败\", \"删除\", \"" + tableName + "\");\r\n");
                        sb.Append("                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));\r\n");
                        sb.Append("                }\r\n");
                        sb.Append("            }\r\n");
                        sb.Append("            else\r\n");
                        sb.Append("            {\r\n");
                        sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));\r\n");
                        sb.Append("            }\r\n");
                        sb.Append("        }\r\n");
                    }
                }


            }
            #endregion



            sb.Append("    }\r\n");
            sb.Append("}\r\n");
            return sb.ToString();
        }

        /// <summary>
        /// 导入导出代码
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        public static void GetImportExportCodeForController(ref StringBuilder sb, string tableName, List<CompleteField> fields)
        {
            sb.Append("        #region 导出导入\r\n");

            sb.Append("        [HttpPost]\r\n");
            sb.Append("        [SupportFilter]\r\n");
            sb.Append("        public ActionResult Import(string filePath)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            if (m_BLL.ImportExcelData(Utils.GetMapPath(filePath), ref errors))\r\n");
            sb.Append("            {\r\n");
            sb.AppendFormat("                 LogHandler.WriteImportExcelLog(GetUserId(), \"{0}\", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, \"导入成功\");\r\n", tableName);
            sb.Append("                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));\r\n");
            sb.Append("            }\r\n");
            sb.Append("            else\r\n");
            sb.Append("            {\r\n");
            sb.AppendFormat("                 LogHandler.WriteImportExcelLog(GetUserId(), \"{0}\", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, \"导入失败\");\r\n", tableName);
            sb.Append("                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));\r\n");
            sb.Append("            }\r\n");
            sb.Append("        }\r\n");


            sb.Append("        [HttpPost]\r\n");
            sb.Append("        [SupportFilter(ActionName = \"Export\")]\r\n");
            sb.Append("        public JsonResult CheckExportData(string queryStr)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            List<" + tableName + "Model> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);\r\n");
            sb.Append("            if (list.Count().Equals(0))\r\n");
            sb.Append("            {\r\n");
            sb.Append("                return Json(JsonHandler.CreateMessage(0, \"没有可以导出的数据\"));\r\n");
            sb.Append("            }\r\n");
            sb.Append("            else\r\n");
            sb.Append("            {\r\n");
            sb.Append("                return Json(JsonHandler.CreateMessage(1, \"可以导出\"));\r\n");
            sb.Append("            }\r\n");
            sb.Append("        }\r\n");

            sb.Append("        [SupportFilter]\r\n");
            sb.Append("        public ActionResult Export(string queryStr)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            List<" + tableName + "Model> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);\r\n");
            sb.Append("            JArray jObjects = new JArray();\r\n");
            sb.Append("                foreach (var item in list)\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    var jo = new JObject();\r\n");
            foreach (CompleteField field in fields)
            {
//               sb.AppendFormat("                    jo.Add(\"{0}\", item.{0});\r\n", field.name);
                sb.AppendFormat("                    jo.Add(\"{0}\", item.{1});\r\n", String.IsNullOrEmpty(field.remark) ? field.name : field.remark, field.name);
            }
            sb.Append("                    jObjects.Add(jo);\r\n");
            sb.Append("                }\r\n");
            sb.Append("                var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());\r\n");

            sb.Append("                var exportFileName = string.Concat(\r\n");
            sb.Append("                    RouteData.Values[\"controller\"].ToString() + \"_\",\r\n");
            sb.Append("                    DateTime.Now.ToString(\"yyyyMMddHHmmss\"),\r\n");
            sb.Append("                    \".xlsx\");\r\n");

            sb.Append("                return new ExportExcelResult\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    SheetName = \"Sheet1\",\r\n");
            sb.Append("                    FileName = exportFileName,\r\n");
            sb.Append("                    ExportData = dt\r\n");
            sb.Append("                };\r\n");
            sb.Append("            }\r\n");

            sb.Append("        [SupportFilter(ActionName = \"Export\")]\r\n");
            sb.Append("        public ActionResult ExportTemplate()\r\n");
            sb.Append("        {\r\n");
            sb.Append("            JArray jObjects = new JArray();\r\n");
            sb.Append("            var jo = new JObject();\r\n");
            foreach (CompleteField field in fields)
            {
                sb.AppendFormat("              jo.Add(\"{0}\", \"\");\r\n", String.IsNullOrEmpty(field.remark) ? field.name : field.remark);
            }
            sb.Append("            jObjects.Add(jo);\r\n");
            sb.Append("            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());\r\n");

            sb.Append("            var exportFileName = string.Concat(\r\n");
            sb.Append("                    RouteData.Values[\"controller\"].ToString() + \"_Template\",\r\n");
            sb.Append("                    \".xlsx\");\r\n");

            sb.Append("                return new ExportExcelResult\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    SheetName = \"Sheet1\",\r\n");
            sb.Append("                    FileName = exportFileName,\r\n");
            sb.Append("                    ExportData = dt\r\n");
            sb.Append("                };\r\n");
            sb.Append("            }\r\n");
            sb.Append("        #endregion\r\n");
            #endregion
        }
    }
}
