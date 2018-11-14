using Apps.BLL.Sys;
using Apps.Common;
using Apps.Models.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apps.Web.Core
{
    public static class WebConfigPara
    {
        public static SysConfigModel SiteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));

    }
}