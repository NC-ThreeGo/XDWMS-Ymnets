using System.Web;
using System.Web.Optimization;

namespace Apps.Web
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                        "~/Scripts/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                       "~/Scripts/home.js"));
            bundles.Add(new ScriptBundle("~/bundles/account").Include(
                       "~/Scripts/Account.js"));
            //easyui
            bundles.Add(new StyleBundle("~/Content/themes/coolblacklight/css").Include("~/Content/themes/skin-coolblacklight.css"));
            bundles.Add(new StyleBundle("~/Content/themes/coolblack/css").Include("~/Content/themes/skin-coolblack.css"));
            bundles.Add(new StyleBundle("~/Content/themes/redlight/css").Include("~/Content/themes/skin-redlight.css"));
            bundles.Add(new StyleBundle("~/Content/themes/red/css").Include("~/Content/themes/skin-red.css"));
            bundles.Add(new StyleBundle("~/Content/themes/yellowlight/css").Include("~/Content/themes/skin-yellowlight.css"));
            bundles.Add(new StyleBundle("~/Content/themes/yellow/css").Include("~/Content/themes/skin-yellow.css"));
            bundles.Add(new StyleBundle("~/Content/themes/purplelight/css").Include("~/Content/themes/skin-purplelight.css"));
            bundles.Add(new StyleBundle("~/Content/themes/purple/css").Include("~/Content/themes/skin-purple.css"));
            bundles.Add(new StyleBundle("~/Content/themes/greenlight/css").Include("~/Content/themes/skin-greenlight.css"));
            bundles.Add(new StyleBundle("~/Content/themes/green/css").Include("~/Content/themes/skin-green.css"));
            bundles.Add(new StyleBundle("~/Content/themes/bluelight/css").Include("~/Content/themes/skin-bluelight.css"));
            bundles.Add(new StyleBundle("~/Content/themes/blue/css").Include("~/Content/themes/skin-blue.css"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryfrom").Include(
                        "~/Scripts/jquery.form.js"));

            bundles.Add(new ScriptBundle("~/bundles/easyui/easyuiplus").Include(
                        "~/Scripts/easyui/jquery.easyui.plus.js"));
            bundles.Add(new ScriptBundle("~/bundles/easyui/datagridfilter").Include(
                       "~/Scripts/easyui/datagrid-filter.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.unobtrusive.plus.js"));

            //加载easyui.combobox.py.js
            //bundles.Add(new ScriptBundle("~/bundles/easyuicomboboxpy").Include(
            //            "~/Scripts/easyui.combobox.py.js"));


            // 使用 Modernizr 的开发版本进行开发和了解信息。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));





        }
    }
}