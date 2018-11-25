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
        public string GetPartialBLL(string tableName)
        {
            string leftStr = GetLeftStr(tableName);
            List<string> fields = SqlHelper.GetColumnField(conn, tableName);
            List<CompleteField> comFields = SqlHelper.GetColumnCompleteField(conn, tableName);

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
            sb.Append("\r\n");


            #region 增加ImportExcelData方法
            sb.Append("\t\tpublic bool ImportExcelData(string filePath, ref ValidationErrors errors)\r\n");
            sb.Append("\t\t{\r\n");
            sb.Append("\t\t\tbool rtn = true;\r\n");
            sb.Append("\r\n");
            sb.Append("\t\t\tvar targetFile = new FileInfo(filePath);\r\n");
            sb.Append("\r\n");
            sb.Append("\t\t\tif (!targetFile.Exists)\r\n");
            sb.Append("\t\t\t{\r\n");
            sb.Append("\t\t\t\terrors.Add(\"导入的数据文件不存在\");\r\n");
            sb.Append("\t\t\t\treturn false;\r\n");
            sb.Append("\t\t\t}\r\n");
            sb.Append("\r\n");
            sb.Append("\t\t\tvar excelFile = new ExcelQueryFactory(filePath);\r\n");
            sb.Append("\r\n");
            sb.Append("\t\t\tusing (XLWorkbook wb = new XLWorkbook(filePath))\r\n");
            sb.Append("\t\t\t{\r\n");
            sb.Append("\t\t\t\t//第一个Sheet\r\n");
            sb.Append("\t\t\t\tusing (IXLWorksheet wws = wb.Worksheets.First())\r\n");
            sb.Append("\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t//对应列头\r\n");
            foreach (CompleteField field in comFields)
            {
                if (field.name != "Id")
                    sb.AppendFormat("excelFile.AddMapping<{0}Model>(x => x.{1}, \"{2}\");\r\n", tableName, field.name, field.remark ?? field.name);
            }
            sb.Append("\r\n");
            sb.Append("\t\t\t\t\t//SheetName，第一个Sheet\r\n");
            sb.AppendFormat("\t\t\t\t\tvar excelContent = excelFile.Worksheet<{0}Model>(0);\r\n", tableName);
            sb.Append("\r\n");
            sb.Append("\t\t\t\t\t//开启事务\r\n");
            sb.Append("\t\t\t\t\tusing (DBContainer db = new DBContainer())\r\n");
            sb.Append("\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\tvar tran = db.Database.BeginTransaction();  //开启事务\r\n");
            sb.Append("\t\t\t\t\t\tint rowIndex = 0;\r\n");
            sb.Append("\r\n");
            sb.Append("\t\t\t\t\t\t//检查数据正确性\r\n");
            sb.Append("\t\t\t\t\t\tforeach (var row in excelContent)\r\n");
            sb.Append("\t\t\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\t\t\trowIndex += 1;\r\n");
            sb.Append("\t\t\t\t\t\t\t\tstring errorMessage = String.Empty;\r\n");
            sb.AppendFormat("\t\t\t\t\t\t\t\tvar model = new {0}Model();\r\n", tableName);
            foreach (CompleteField field in comFields)
            {
                sb.AppendFormat("\t\t\t\t\t\t\t\tmodel.{0} = row.{0};\r\n", field.name);
            }
            sb.Append("\r\n");
            sb.Append("\t\t\t\t\t\t\t\tif (!String.IsNullOrEmpty(errorMessage))\r\n");
            sb.Append("\t\t\t\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\trtn = false;\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\terrors.Add(string.Format(\"第 {0} 列发现错误：{1}{2}\", rowIndex, errorMessage, \"<br/>\"));\r\n");
            sb.AppendFormat("\t\t\t\t\t\t\t\t\twws.Cell(rowIndex + 1, {0}).Value = errorMessage;\r\n", comFields.Count().ToString());
            sb.Append("\t\t\t\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\t\t\t\telse\r\n");
            sb.Append("\t\t\t\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t//写入数据库\r\n");
            sb.AppendFormat("\t\t\t\t\t\t\t\t\t{0} entity = new {0}();\r\n", tableName);
            foreach (CompleteField field in comFields)
            {
                sb.AppendFormat("\t\t\t\t\t\t\t\t\tentity.{0} = model.{0};\r\n", field.name);
            }
            sb.Append("\r\n");
            sb.AppendFormat("\t\t\t\t\t\t\t\t\tdb.{0}.Add(entity);\r\n", tableName);
            sb.AppendFormat("\t\t\t\t\t\t\t\t\ttry\r\n", tableName);
            sb.Append("\t\t\t\t\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t\tdb.SaveChanges();\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\tcatch (Exception ex)\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t\trtn = false;\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t\t//将当前报错的entity状态改为分离，类似EF的回滚（忽略之前的Add操作）\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t\tdb.Entry(entity).State = System.Data.Entity.EntityState.Detached;\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t\terrorMessage = ex.InnerException.InnerException.Message;\r\n");
            sb.Append("\t\t\t\t\t\t\t\t\t\terrors.Add(string.Format(\"第 {0} 列发现错误：{1}{2}\", rowIndex, errorMessage, \"<br/>\"));\r\n");
            sb.AppendFormat("\t\t\t\t\t\t\t\t\t\twws.Cell(rowIndex + 1, {0}).Value = errorMessage;\r\n", comFields.Count().ToString());
            sb.Append("\t\t\t\t\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\t\t\t}\r\n");
            sb.Append("\r\n");
            sb.Append("\t\t\t\t\t\t\tif (rtn)\r\n");
            sb.Append("\t\t\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\t\t\ttran.Commit();  //必须调用Commit()，不然数据不会保存\r\n");
            sb.Append("\t\t\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\t\t\telse\r\n");
            sb.Append("\t\t\t\t\t\t\t{\r\n");
            sb.Append("\t\t\t\t\t\t\t\ttran.Rollback();    //出错就回滚       \r\n");
            sb.Append("\t\t\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\t}\r\n");
            sb.Append("\t\t\t\t\twb.Save();\r\n");
            sb.Append("\t\t\t\t}\r\n");
            sb.Append("\r\n");
            sb.Append("\t\t\t\treturn rtn;\r\n");

            sb.Append("\t\t\t}\r\n");

            #endregion

            sb.Append("    }\r\n");
            sb.Append(" }\r\n");
            return sb.ToString();
        }
    }
}
