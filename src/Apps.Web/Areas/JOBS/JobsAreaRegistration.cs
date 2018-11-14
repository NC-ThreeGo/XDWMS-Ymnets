using System.Web.Mvc;

namespace Apps.Web.Areas.JOBS
{
    public class JobsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "JOBS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "JobsGlobalization", // 路由名称
                "{lang}/JOBS/{controller}/{action}/{id}", // 带有参数的 URL
                new { lang = "zh", controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
                new { lang = "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$" }    //参数约束
            );
            context.MapRoute(
                "Jobs_default",
                "JOBS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
