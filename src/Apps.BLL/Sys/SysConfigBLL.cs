using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Caching;
using Apps.Common;
using Apps.DAL.Sys;

namespace Apps.BLL.Sys
{
    public partial class SysConfigBLL
    {
        private readonly SysConfigRepository dal = new SysConfigRepository();

        /// <summary>
        ///  读取配置文件
        /// </summary>
        public Apps.Models.Sys.SysConfigModel loadConfig(string configFilePath)
        {
            Apps.Models.Sys.SysConfigModel model = CacheHelper.Get<Apps.Models.Sys.SysConfigModel>(ContextKeys.CACHE_SITE_CONFIG);
            if (model == null)
            {
                CacheHelper.Insert(ContextKeys.CACHE_SITE_CONFIG, dal.loadConfig(configFilePath), configFilePath);
                model = CacheHelper.Get<Apps.Models.Sys.SysConfigModel>(ContextKeys.CACHE_SITE_CONFIG);
            }
            return model;
        }
        /// <summary>
        /// 读取客户端站点配置信息
        /// </summary>
        public Apps.Models.Sys.SysConfigModel loadConfig(string configFilePath, bool isClient)
        {
            Apps.Models.Sys.SysConfigModel model = CacheHelper.Get<Apps.Models.Sys.SysConfigModel>(ContextKeys.CACHE_SITE_CONFIG_CLIENT);
            if (model == null)
            {
                model = dal.loadConfig(configFilePath);
                model.templateskin = model.webpath + "templates/" + model.templateskin;
                CacheHelper.Insert(ContextKeys.CACHE_SITE_CONFIG_CLIENT, model, configFilePath);
            }
            return model;
        }

        /// <summary>
        ///  保存配置文件
        /// </summary>
        public Apps.Models.Sys.SysConfigModel saveConifg(Apps.Models.Sys.SysConfigModel model, string configFilePath)
        {
            return dal.saveConifg(model, configFilePath);
        }

    }
}
