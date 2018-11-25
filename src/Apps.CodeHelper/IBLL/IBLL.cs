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
            sb.Append("using " + txt_prefix.Text + ".Models." + leftStr.Replace(".", "") + ";\r\n");
            sb.Append("\r\n");
            sb.Append("namespace " + txt_prefix.Text + ".IBLL" + (leftStr == ".Sys" ? "" : "." + leftStr) + "\r\n");
            sb.Append("{\r\n");
            sb.AppendFormat("    public partial interface I{0}BLL : IBaseBLL<{0}>\r\n", tableName);
            sb.Append("    {\r\n");
            sb.Append("         /// <summary>\r\n");
            sb.Append("         /// 导入Excel文件，当发生导入错误时，回写错误信息，并且全部回滚。\r\n");
            sb.Append("         /// </summary>\r\n");
            sb.Append("         /// <param name=\"filePath\"></param>\r\n");
            sb.Append("         /// <param name=\"errors\"></param>\r\n");
            sb.Append("         /// <returns></returns>\r\n");
            sb.Append("         bool ImportExcelData(string filePath, ref ValidationErrors errors);\r\n");
            sb.Append("    }r\n");

            return sb.ToString();
        }
    }
}
