using System;
using System.Collections.Generic;
using System.Text;
using Apps.Common;

namespace Apps.DAL.Sys
{
    /// <summary>
    /// 数据访问类:站点配置
    /// </summary>
    public partial class SysConfigRepository
    {
        private static object lockHelper = new object();

        /// <summary>
        ///  读取站点配置文件
        /// </summary>
        public Apps.Models.Sys.SysConfigModel loadConfig(string configFilePath)
        {
            return (Apps.Models.Sys.SysConfigModel)SerializationHelper.Load(typeof(Apps.Models.Sys.SysConfigModel), configFilePath);
        }

        /// <summary>
        /// 写入站点配置文件
        /// </summary>
        public Apps.Models.Sys.SysConfigModel saveConifg(Apps.Models.Sys.SysConfigModel model, string configFilePath)
        {
            lock (lockHelper)
            {
                SerializationHelper.Save(model, configFilePath);
            }
            return model;
        }

    }
}
