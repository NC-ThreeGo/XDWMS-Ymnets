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
/// 业务层
/// </summary>
namespace Apps.CodeHelper
{
    public partial class CodeFrom
    {
        public string GetTreePartialBLL(string tableName)
        {
            string leftStr = GetLeftStr(tableName);
            List<string> fields = SqlHelper.GetColumnField(conn, tableName);

            StringBuilder sb = new StringBuilder();
            sb.Append("using Apps.Common;\r\n");
            sb.Append("using Apps.Models;\r\n");
            sb.Append("using System.Linq;\r\n");
            sb.Append("using System.Collections.Generic;\r\n");
            sb.Append("using System.Linq;\r\n");
            sb.Append("using System;\r\n");
            sb.Append("using " + txt_prefix.Text + ".Models." + leftStr.Replace(".", "") + ";\r\n");
            sb.Append("\r\n");
            sb.Append("namespace " + txt_prefix.Text + ".BLL" + (leftStr == ".Sys" ? "" : "." + leftStr) + "\r\n");
            sb.Append("{\r\n");
            sb.Append("    public  partial class " + tableName + "BLL\r\n");
            sb.Append("    {\r\n");
            sb.Append("\r\n");
            if (cb_EnableParent.Checked)
            {
                List<CompleteField> comFields = SqlHelper.GetColumnCompleteField(conn, tableName);
                CompleteField parentField = null;
                foreach (CompleteField field in comFields)
                {
                    if (field.name == txt_TableKey1.Text.Trim())
                    {
                        parentField = field;
                    }
                }

                if (parentField == null)
                {
                    MessageBox.Show("关键父表的外键" + txt_TableKey1.Text + "不正确，请重新确认！不然生成数据需要手动更改");
                    return "";
                }

                sb.Append("    public override List<" + tableName + "Model> GetListByParentId(ref GridPager pager, string queryStr, object parentId)\r\n");
                sb.Append("    {\r\n");
                sb.Append("        IQueryable<" + tableName + "> queryData = null;\r\n");
                if (parentField.xType != "56" && parentField.xType != "127")//非int型主键
                {
                    sb.Append("        string pid = parentId.ToString();\r\n");
                }
                else
                {
                    sb.Append("        int pid = Convert.ToInt32(parentId);\r\n");
                }

                sb.Append("        if (pid != " + (parentField.xType != "56" && parentField.xType != "127" ? "\"0\"" : "0") + ")\r\n");
                sb.Append("        {\r\n");
                sb.Append("        queryData = m_Rep.GetList(a => a." + txt_TableKey1.Text + " == pid);\r\n");
                sb.Append("        }\r\n");
                sb.Append("        else\r\n");
                sb.Append("        {\r\n");
                sb.Append("        queryData = m_Rep.GetList();\r\n");
                sb.Append("        }\r\n");

                sb.Append("        if (!string.IsNullOrWhiteSpace(queryStr))\r\n");
                sb.Append("        {\r\n");
                sb.Append("            queryData = m_Rep.GetList(\r\n");
                sb.Append("                        a => (\r\n");
                bool orEnable = false;
                foreach (CompleteField field in comFields)
                {
                    if (field.xType == "167" || field.xType == "35" || field.xType == "231" || field.xType == "239")
                    {
                        sb.Append("                               " + (orEnable ? "||" : "") + " a." + field.name + ".Contains(queryStr)\r\n");
                        orEnable = true;
                    }
                }
                sb.Append("                             )\r\n");
                sb.Append("                        );\r\n");
                sb.Append("        }\r\n");
                sb.Append("        pager.totalRows = queryData.Count();\r\n");
                sb.Append("        queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);\r\n");
                sb.Append("        return CreateModelList(ref queryData);\r\n");
                sb.Append("    }\r\n");
            }


            sb.Append("        public List<" + tableName + "Model> GetList(string parentId)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            IQueryable <" + tableName + "> queryData = null;\r\n");
            sb.Append("            queryData = m_Rep.GetList(a => a.ParentId == parentId).OrderBy(a => a.CreateTime);\r\n");
            sb.Append("            return CreateModelList(ref queryData);\r\n");
            sb.Append("        }\r\n");


            sb.Append("        public override List<" + tableName + "Model> CreateModelList(ref IQueryable<" + tableName + "> queryData)\r\n");
            sb.Append("        {\r\n");
            sb.Append("\r\n");
            sb.Append("            List<" + tableName + "Model> modelList = (from r in queryData\r\n");
            sb.Append("                                              select new " + tableName + "Model\r\n");
            sb.Append("                                              {\r\n");
            foreach (string field in fields)
            {
                sb.Append("                                                  " + field + " = r." + field + ",\r\n");
            }
            //启用表关联
            if (cb_EnableParent.Checked)
            {
                //表1
                if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                {
                    sb.Append("                                                  " + txt_TableName1.Text.Replace(leftStr + "_", "") + "Name".Trim() + " = r." + txt_TableName1.Text.Trim() + ".Name,\r\n");
                }
            }
            sb.Append("                                              }).ToList();\r\n");
            sb.Append("            return modelList;\r\n");
            sb.Append("        }\r\n");
            sb.Append("    }\r\n");
            sb.Append(" }\r\n");
            return sb.ToString();
        }
    }
}
