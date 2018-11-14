using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Apps.Models.Sys;

namespace Apps.Web.Core
{
    public static class ExtendMvcHtml
    {
        public static MvcHtmlString SwitchDropdown(this HtmlHelper helper, string name, bool check, string ontext, string offtext)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<select id=\"{0}\" name=\"{0}\"><option {1} value=\"true\">{2}</option><option {3} value=\"false\">{4}</option></select>",
                name, check ? "selected=\"selected\"" : "", ontext == "" ? "启用" : ontext, !check ? "selected=\"selected\"" : "", offtext == "" ? "禁用" : offtext);

            return new MvcHtmlString(sb.ToString());

        }

        public static MvcHtmlString SwitchButtonByEdit(this HtmlHelper helper, string name, bool check)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<input class=\"easyui-switchbutton\" style=\"width:43px;\" value=\"{0}\" ontext=\"\" id=\"{1}\" name=\"{2}\" offtext=\"\" {3}>", (!check ? "false" : "true"), name, name, (!check ? "" : "checked"));

            return new MvcHtmlString(sb.ToString());

        }
        public static MvcHtmlString SwitchButtonByEdit(this HtmlHelper helper, string name, bool check, string ontext, string offtext, string width)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<input class=\"easyui-switchbutton\" value=\"{0}\"  id=\"{1}\" name=\"{2}\" {3} offtext=\"{4}\" ontext=\"{5}\"  style=\"width:{6}px;\">", (!check ? "false" : "true"), name, name, (!check ? "" : "checked"), ontext, offtext, width);

            return new MvcHtmlString(sb.ToString());

        }
        /// <summary>
        /// 单选是和否
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="name">控件name</param>
        /// <param name="check">默认选中值</param>
        /// <param name="ontext">值一文本</param>
        /// <param name="offtext">值二文本</param>
        /// <returns>html</returns>
        public static MvcHtmlString RadioFor(this HtmlHelper helper, string name, bool check, string ontext, string offtext)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<input class=\"magic-radio\" type=\"radio\" value=\"true\" name=\"{0}\" {1} id=\"{0}1\"><label for=\"{0}1\">{2}</label><input class=\"magic-radio\" type=\"radio\"  value=\"false\" name=\"{0}\" {3} id=\"{0}2\"><label for=\"{0}2\">{4}</label>",
                name, check ? "checked=\"checked\"" : "", ontext == "" ? "启用" : ontext, !check ? "checked=\"checked\"" : "", offtext == "" ? "禁用" : offtext);

            return new MvcHtmlString(sb.ToString());

        }
        /// <summary>
        /// 多个单选的处理结果:Html.RadiosFor("IsLast", 默认值, new string[]{ "值一","值二" }, new string[] { "文本一","文本二" })
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="name">控件name</param>
        /// <param name="check">默认选中值</param>
        /// <param name="ontext">值一文本</param>
        /// <param name="offtext">值二文本</param>
        /// <returns>html</returns>
        public static MvcHtmlString RadiosFor(this HtmlHelper helper, string name, object check, string[] values, string[] texts)
        {

            StringBuilder sb = new StringBuilder();
            string value = check.ToString().ToLower();
            for (int i = 0; i < values.Length; i++)
            {
                sb.AppendFormat("<input class=\"magic-radio\" type=\"radio\" name=\"{0}\" value=\"{1}\" {2} id=\"{3}\"><label for=\"{3}\">{4}</label>", name, values[i], values[i] == value ? "checked=\"checked\"" : "", name + i, texts[i]);
            }
            return new MvcHtmlString(sb.ToString());

        }

        /// <summary>
        /// 多个复选的处理结果:Html.ChecksFor("IsLast", 默认值, new string[]{ "值一","值二" }, new string[] { "文本一","文本二" })
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="name">控件name</param>
        /// <param name="check">默认选中值</param>
        /// <param name="ontext">值一文本</param>
        /// <param name="offtext">值二文本</param>
        /// <returns>html</returns>
        public static MvcHtmlString ChecksFor(this HtmlHelper helper, string name, object check, string[] values, string[] texts)
        {

            StringBuilder sb = new StringBuilder();
            string value = check.ToString().ToLower();
            for (int i = 0; i < values.Length; i++)
            {
                sb.AppendFormat("<input class=\"magic-checkbox\" type=\"checkbox\" name=\"{0}\" value=\"{1}\" {2} id=\"{3}\"><label for=\"{3}\">{4}</label>", name, values[i], values[i] == value ? "checked=\"checked\"" : "", name + i, texts[i]);
            }
            return new MvcHtmlString(sb.ToString());

        }

        /// <summary>
        /// 下拉选择是和否
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="name">控件name</param>
        /// <param name="check">默认选中值</param>
        /// <param name="ontext">值一文本</param>
        /// <param name="offtext">值二文本</param>
        /// <returns>html</returns>
        public static MvcHtmlString SwitchDropdownNum(this HtmlHelper helper, string name, bool check, string ontext, string offtext)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<select id=\"{0}\" name=\"{0}\"><option {1} value=\"1\">{2}</option><option {3} value=\"0\">{4}</option></select>",
                name, check ? "selected=\"selected\"" : "", ontext == "" ? "启用" : ontext, !check ? "selected=\"selected\"" : "", offtext == "" ? "禁用" : offtext);

            return new MvcHtmlString(sb.ToString());

        }

        /// <summary>
        /// 权限按钮
        /// </summary>
        /// <param name="helper">htmlhelper</param>
        /// <param name="id">控件Id</param>
        /// <param name="icon">控件icon图标class</param>
        /// <param name="text">控件的名称</param>
        /// <param name="perm">权限列表</param>
        /// <param name="keycode">操作码</param>
        /// <param name="hr">分割线</param>
        /// <returns>html</returns>
        public static MvcHtmlString ToolButton(this HtmlHelper helper, string id, string icon, string text, ref List<permModel> perm, string keycode, bool hr)
        {
            if (perm == null)
            {
                string filePath = HttpContext.Current.Request.FilePath;
                perm = (List<permModel>)HttpContext.Current.Session[filePath];
            }
            if (perm != null && perm.Where(a => a.KeyCode == keycode).Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<a id=\"{0}\" class=\"{1}\">", id, GetKeyBtn(keycode));
                sb.AppendFormat("<span class=\"{0}\"></span>&nbsp;{1}</a>", icon, text);

                return new MvcHtmlString(sb.ToString());
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        /// <summary>
        /// 获取操作码颜色
        /// </summary>
        /// <param name="keycode">操作码颜色</param>
        /// <returns></returns>
        private static string GetKeyBtn(string keycode)
        {
            keycode = keycode.ToLower();
            string btn = "";
            switch (keycode)
            {
                case "create": btn = "btn btn-success"; break;
                case "delete": btn = "btn btn-danger"; break;
                case "edit": btn = "btn btn-warning"; break;
                case "save": btn = "btn btn-success"; break;
                default: btn = "btn btn-default"; break;
            };
            return btn;
        }


        /// <summary>
        /// 普通按钮
        /// </summary>
        /// <param name="helper">htmlhelper</param>
        /// <param name="id">控件Id</param>
        /// <param name="icon">控件icon图标class</param>
        /// <param name="text">控件的名称</param>
        /// <param name="hr">分割线</param>
        /// <returns>html</returns>
        public static MvcHtmlString ToolButton(this HtmlHelper helper, string id, string icon, string text, bool hr)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<a id=\"{0}\" class=\"btn btn-default\">", id);
            sb.AppendFormat("<span class=\"{0}\"></span>&nbsp;{1}</a>", icon, text);
            return new MvcHtmlString(sb.ToString());

        }
    }
}