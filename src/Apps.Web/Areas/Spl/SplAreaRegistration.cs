using System.Web.Mvc;

namespace Apps.Web.Areas.Spl
{
    public class SplAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Spl";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
               "SplGlobalization", // 路由名称
               "{lang}/Spl/{controller}/{action}/{id}", // 带有参数的 URL
               new { lang = "zh", controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
               new { lang = "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$" }    //参数约束
           );
            context.MapRoute(
                "Spl_default",
                "Spl/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}