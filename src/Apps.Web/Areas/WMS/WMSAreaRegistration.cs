using System.Web.Mvc;

namespace Apps.Web.Areas.WMS
{
    public class WMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               "WMSGlobalization", // 路由名称
               "{lang}/WMS/{controller}/{action}/{id}", // 带有参数的 URL
               new { lang = "zh", controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
               new { lang = "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$" }    //参数约束
           );
            context.MapRoute(
                "WMS_default",
                "WMS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}