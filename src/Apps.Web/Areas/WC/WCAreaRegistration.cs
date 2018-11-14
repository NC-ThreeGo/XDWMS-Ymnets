using System.Web.Mvc;

namespace Apps.Web.Areas.WC
{
    public class WCAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
               "WCGlobalization", // 路由名称
               "{lang}/WC/{controller}/{action}/{id}", // 带有参数的 URL
               new { lang = "zh", controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
               new { lang = "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$" }    //参数约束
           );
            context.MapRoute(
                "WC_default",
                "WC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}