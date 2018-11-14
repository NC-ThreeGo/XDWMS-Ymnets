using Apps.BLL.Sys;
using Apps.Common;
using Apps.Models.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apps.Web.Controllers
{
    public class IMController : Controller
    {
        private SysConfigModel siteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));
       
        //
        // GET: /IM/
        public ActionResult Index()
        {

            if (Session["Account"] != null)
            {
                //获取是否开启WEBIM
                ViewBag.IsEnable = siteConfig.webimstatus;
                AccountModel account = new AccountModel();
                account = (AccountModel)Session["Account"];
                return View(account);
            }
            else
            {
                return Redirect("/Account");
            }
        }
	}
}