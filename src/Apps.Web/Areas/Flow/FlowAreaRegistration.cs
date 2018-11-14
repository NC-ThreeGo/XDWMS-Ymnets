using System.Web.Mvc;

namespace Apps.Web.Areas.Flow
{
    public class FlowAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Flow";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
               "FlowGlobalization", // 路由名称
               "{lang}/Flow/{controller}/{action}/{id}", // 带有参数的 URL
               new { lang = "zh", controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
               new { lang = "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$" }    //参数约束
           );
            context.MapRoute(
                "Flow_default",
                "Flow/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
