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
/// Model层
/// </summary>
namespace Apps.CodeHelper
{
    public partial class CodeFrom
    {
        public string GetTreePartialModel(string tableName)
        {
            string leftStr = GetLeftStr(tableName);
            List<CompleteField> fields = SqlHelper.GetColumnCompleteField(conn, tableName);

            StringBuilder sb = new StringBuilder();
            sb.Append("using System;\r\n");
            sb.Append("using System.ComponentModel.DataAnnotations;\r\n");
            sb.Append("using " + txt_prefix.Text + ".Models;\r\n");
            sb.Append("namespace " + txt_prefix.Text + ".Models." + leftStr.Replace(".", "") + "\r\n");
            sb.Append("{\r\n");
            sb.Append("    public partial class " + tableName + "Model\r\n");
            sb.Append("    {\r\n");
            //foreach (CompleteField field in fields)
            //{
            //    //sb.Append("" + SetValid(field) + "");
            //    sb.Append("        [Display(Name = \"" + field.remark + "\")]\r\n");
            //    sb.Append("        public override " + SqlHelper.GetType(field.xType) + " " + field.name + " { get; set; }\r\n");
            //    sb.Append("\r\n");
            //}
            //启用表关联
            if (cb_EnableParent.Checked)
            {
                //表1
                if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                {
                    sb.Append("        [Display(Name = \"类别名称\")]\r\n");
                    sb.Append("        public string " + txt_TableName1.Text.Replace(leftStr + "_", "") + "Name".Trim() + " { get; set; }\r\n");
                }

            }
            sb.Append("        public string state { get; set; }\r\n");
            sb.Append("     }\r\n");
            sb.Append("}\r\n");


            return sb.ToString();
        }
    }
}
