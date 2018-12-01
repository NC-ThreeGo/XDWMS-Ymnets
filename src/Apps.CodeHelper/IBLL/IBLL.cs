using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.CodeHelper
{
    public partial class CodeFrom
    {
        public string GetIBLL(string tableName)
        {
            string leftStr = GetLeftStr(tableName);
            StringBuilder sb = new StringBuilder();
            sb.Append("using Apps.Common;\r\n");
            sb.Append("using " + txt_prefix.Text + ".Models." + leftStr.Replace(".", "") + ";\r\n");
            sb.Append("\r\n");
            sb.Append("namespace " + txt_prefix.Text + ".IBLL" + (leftStr == ".Sys" ? "" : "." + leftStr) + "\r\n");
            sb.Append("{\r\n");
            sb.AppendFormat("    public partial interface I{0}BLL\r\n", tableName);
            sb.Append("    {\r\n");
            sb.Append("         /// <summary>\r\n");
            sb.Append("         /// 导入Excel文件，当发生导入错误时，回写错误信息，并且全部回滚。\r\n");
            sb.Append("         /// </summary>\r\n");
            sb.Append("         /// <param name=\"filePath\"></param>\r\n");
            sb.Append("         /// <param name=\"errors\"></param>\r\n");
            sb.Append("         /// <returns></returns>\r\n");
            sb.Append("         bool ImportExcelData(string filePath, ref ValidationErrors errors);\r\n");
            sb.Append("    \r\n");
            sb.Append("         /// <summary>\r\n");
            sb.Append("         /// 对导入进行附加的校验，例如物料编码是否存在等。\r\n");
            sb.Append("         /// </summary>\r\n");
            sb.Append("         /// <param name=\"model\"></param>\r\n");
            sb.AppendFormat("         void AdditionalCheckExcelData({0}Model model);\r\n", tableName);
            sb.Append("    }\r\n");
            sb.Append("    \r\n");
            sb.Append("         /// <summary>\r\n");
            sb.Append("         /// 根据where字符串获取列表数据。\r\n");
            sb.Append("         /// </summary>\r\n");
            sb.Append("         /// <param name=\"pager\"></param>\r\n");
            sb.Append("         /// <param name=\"whereStr\"></param>\r\n");
            sb.AppendFormat("         List<{0}Model> GetListByWhere(ref GridPager pager, string where)", tableName);
            sb.Append("    }\r\n");
            sb.Append("}");

            return sb.ToString();
        }
    }
}
