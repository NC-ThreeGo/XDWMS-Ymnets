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
/// Create视图
/// </summary>
namespace Apps.CodeHelper
{
    public partial class CodeFrom
    {
      
        public string GetCreate(string tableName)
        {
            string leftStr = GetLeftStr(tableName);
            List<CompleteField> fields = SqlHelper.GetColumnCompleteField(conn, tableName);

            StringBuilder sb = new StringBuilder();
            sb.Append("@model " + txt_prefix.Text + ".Models." + (leftStr == "Sys" ? "" : (leftStr + ".")) + "" + tableName + "Model\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Web.Core;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Common;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Models." + leftStr + ";\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Web;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Locale;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Models.Sys;\r\n");
            sb.Append("@{\r\n");
            sb.Append("    ViewBag.Title = \"创建\";\r\n");
            sb.Append("    Layout = \"~/Views/Shared/_Index_LayoutEdit.cshtml\";\r\n");
            sb.Append("    List<permModel> perm = null;\r\n");
            sb.Append("}\r\n");
            sb.Append("\r\n");
            sb.Append("<script type=\"text/javascript\">\r\n");
            sb.Append("$(function () {\r\n");
            sb.Append("    $(\"#btnSave\").click(function () {\r\n");
            sb.Append("        if ($(\"form\").valid()) {\r\n");
            sb.Append("            $.ajax({\r\n");
            sb.Append("                url: \"@Url.Action(\"Create\")\",\r\n");
            sb.Append("                type: \"Post\",\r\n");
            sb.Append("                data: $(\"form\").serialize(),\r\n");
            sb.Append("                dataType: \"json\",\r\n");
            sb.Append("                success: function (data) {\r\n");
            sb.Append("                    if (data.type == 1) {\r\n");
            sb.Append("                        window.parent.frameReturnByMes(data.message);\r\n");
            sb.Append("                        window.parent.frameReturnByReload(true);\r\n");
            sb.Append("                        window.parent.frameReturnByClose()\r\n");
            sb.Append("                    }\r\n");
            sb.Append("                    else {\r\n");
            sb.Append("                        window.parent.frameReturnByMes(data.message);\r\n");
            sb.Append("                    }\r\n");
            sb.Append("                }\r\n");
            sb.Append("            });\r\n");
            sb.Append("        }\r\n");
            sb.Append("        return false;\r\n");
            sb.Append("    });\r\n");
            sb.Append("    $(\"#btnReturn\").click(function () {\r\n");
            sb.Append("         window.parent.frameReturnByClose();\r\n");
            sb.Append("    });\r\n");
            sb.Append("});\r\n");
            sb.Append("function frameReturnByClose() {\r\n");
            sb.Append("$(\"#modalwindow\").window('close');\r\n");
            sb.Append("}\r\n");
            sb.Append("function frameReturnByMes(mes) {\r\n");
            sb.Append("$.messageBox5s(Lang.Tip, mes);\r\n");
            sb.Append("}\r\n");
            sb.Append("</script>\r\n");
            sb.Append("<div id=\"modalwindow\" class=\"easyui-window\" style=\"width:800px; height:400px;\" data-options=\"modal:true,closed:true,minimizable:false,shadow:false\"></div>\r\n");
            sb.Append("<div class=\"mvctool bgb\">\r\n");
            sb.Append("@Html.ToolButton(\"btnSave\", \"fa fa-save\", Resource.Save,ref perm, \"Save\", true)\r\n");
            sb.Append("@Html.ToolButton(\"btnReturn\", \"fa fa-reply\", Resource.Reply,false)\r\n");
            sb.Append("</div>\r\n");
            sb.Append("@using (Html.BeginForm())\r\n");
            sb.Append("{\r\n");
            foreach (CompleteField field in fields)
            {

                if (field.name == "Id")
                {
                    if (field.xType != "56" && field.xType != "127")//非int型主键
                    {
                        sb.Append("             @Html.HiddenFor(model => model." + field.name + ")\r\n");
                    }
                    else
                    {
                        sb.Append("             @Html.HiddenFor(model => model." + field.name + ", new { @Value = 0})\r\n");
                    }
                }
                if (field.name == "CreateTime")
                {
                    sb.Append("             <input id=\"CreateTime\" type=\"hidden\" name=\"CreateTime\" value=\"2000-1-1\" />\r\n");
                }
            }
            sb.Append(" <table class=\"formtable\">\r\n");
            sb.Append("    <tbody>\r\n");

            //启用表关联
            if (cb_EnableParent.Checked)
            {
                //表1
                if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                {
                    sb.Append("        <tr>\r\n");
                    sb.Append("            <th>\r\n");
                    sb.Append("                @Html.LabelFor(model => model." + txt_TableKey1.Text.Trim() + ")：\r\n");
                    sb.Append("            </th>\r\n");
                    sb.Append("            <td>\r\n");
                    sb.Append("                 @Html.DropDownListFor(model => model." + txt_TableKey1.Text.Trim() + ", ViewBag." + (txt_TableName1.Text.Trim().IndexOf("_") > 0 ? txt_TableName1.Text.Trim().Split('_')[1] : txt_TableName1.Text.Trim()) + " as SelectList)\r\n");
                    sb.Append("            </td>\r\n");
                    sb.Append("            <td>@Html.ValidationMessageFor(model => model." + txt_TableKey1.Text.Trim() + ")</td>\r\n");
                    sb.Append("        </tr>\r\n");
                }

            }

            foreach (CompleteField field in fields)
            {
                //启用表关联
                if (cb_EnableParent.Checked)
                {
                    //表1
                    if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                    {
                        if (field.name == txt_TableKey1.Text.Trim())
                            continue;
                    }

                }
                if (field.name != "Id" && field.name != "CreateTime")
                {
                    if (field.xType == "104" || field.xType == "bool")
                    {
                        sb.Append("        <tr>\r\n");
                        sb.Append("            <th>\r\n");
                        sb.Append("                @Html.LabelFor(model => model." + field.name + ")：\r\n");
                        sb.Append("            </th>\r\n");
                        sb.Append("            <td>\r\n");
                        sb.Append("                  @Html.RadioFor(\"" + field.name + "\", true,\"\",\"\")\r\n");
                        sb.Append("            </td>\r\n");
                        sb.Append("            <td>@Html.ValidationMessageFor(model => model." + field.name + ")</td>\r\n");
                        sb.Append("        </tr>\r\n");

                    }
                    else if (field.name.ToLower().Contains("img") || field.name.ToLower().Contains("photo"))
                    {
                        sb.Append("        <tr>\r\n");
                        sb.Append("            <th>\r\n");
                        sb.Append("                @Html.LabelFor(model => model." + field.name + ")：\r\n");
                        sb.Append("            </th>\r\n");
                        sb.Append("            <td>\r\n");
                        sb.Append("             @Html.HiddenFor(model => model." + field.name + ")\r\n");
                        sb.Append("             <img class=\"expic\" src=\"/Content/Images/NotPic.jpg\" /><br />\r\n");
                        sb.Append("             <a onclick=\"$('#FileUpload_" + field.name + "').trigger('click')\" class=\"files\">@Resource.Browse</a><a onclick=\"resetImg(this)\" class=\"files\">清除</a>\r\n");
                        sb.Append("             <input type=\"file\" class=\"displaynone\" id=\"FileUpload_" + field.name + "\" name=\"FileUpload_" + field.name + "\" onchange=\"Upload('SingleFile', '" + field.name + "', 'FileUpload_" + field.name + "','1','1');\" />\r\n");
                        sb.Append("             <span class=\"uploading\">@Resource.Uploading</span>\r\n");
                        sb.Append("            </td>\r\n");
                        sb.Append("            <td>@Html.ValidationMessageFor(model => model." + field.name + ")</td>\r\n");
                        sb.Append("        </tr>\r\n");
                    }
                    else if (field.xType == "61" || field.xType == "datetime")
                    {
                        sb.Append("        <tr>\r\n");
                        sb.Append("            <th>\r\n");
                        sb.Append("                @Html.LabelFor(model => model." + field.name + ")：\r\n");
                        sb.Append("            </th>\r\n");
                        sb.Append("            <td >\r\n");
                        sb.Append("               @Html.TextBoxFor(model => model." + field.name + ", new { @onClick = \"WdatePicker()\",@style = \"width: 105px\" })\r\n");
                        sb.Append("            </td>\r\n");
                        sb.Append("            <td>@Html.ValidationMessageFor(model => model." + field.name + ")</td>\r\n");
                        sb.Append("        </tr>\r\n");
                    }
                    else
                    {
                        sb.Append("        <tr>\r\n");
                        sb.Append("            <th>\r\n");
                        sb.Append("                @Html.LabelFor(model => model." + field.name + ")：\r\n");
                        sb.Append("            </th>\r\n");
                        sb.Append("            <td>\r\n");
                        sb.Append("                @Html.EditorFor(model => model." + field.name + ")\r\n");
                        sb.Append("            </td>\r\n");
                        sb.Append("            <td>@Html.ValidationMessageFor(model => model." + field.name + ")</td>\r\n");
                        sb.Append("        </tr>\r\n");
                    }
                }
            }
            sb.Append("    </tbody>\r\n");
            sb.Append("</table>\r\n");
            sb.Append("}\r\n");
            return sb.ToString();
        }
    
    }
}
