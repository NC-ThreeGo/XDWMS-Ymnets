using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Common
{
    public class ConfigPara
    {


        public static string EFDBConnection {
            get {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["DBContainer"].ConnectionString;
                string result = AESEncryptHelper.Decrypt(connection);
                return result;
                //return "name=DBContainer";
            }
        }

        public static string QuartzDBConnection
        {
            get
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["QuartzConnectionString"].ConnectionString;
                string result = AESEncryptHelper.Decrypt(connection);
                return result;
            }
        }

        public static List<string> NoFilterUrl
        {
            get
            {
                string[] arr  = ConfigurationManager.AppSettings["NoFilterUrl"].Split(',');
                return arr.ToList();
            }
        }

        public static string MS_HttpContext
        {
            get
            {
                return "MS_HttpContext";
            }
        }


        public static string Token
        {
            get
            {
                return "token";
            }
        }
    }
    public class QuartzPara 
    { 
        public string quartz_threadPool_type {
            get{ return ConfigurationManager.AppSettings["quartz.threadPool.type"]; } 
        }
        public string quartz_threadPool_threadCount {
            get{ return ConfigurationManager.AppSettings["quartz.threadPool.threadCount"]; } 
        }
        public string quartz_threadPool_threadPriority {
            get{ return ConfigurationManager.AppSettings["quartz.threadPool.threadPriority"]; } 
        }
        
         public string quartz_jobStore_misfireThreshold {
            get{ return ConfigurationManager.AppSettings["quartz.jobStore.misfireThreshold"]; } 
        }

         public string quartz_jobStore_type {
            get{ return ConfigurationManager.AppSettings["quartz.jobStore.type"]; } 
        }
         public string quartz_jobStore_useProperties {
            get{ return ConfigurationManager.AppSettings["quartz.jobStore.useProperties"]; } 
        }
        public string quartz_jobStore_dataSource {
            get{ return ConfigurationManager.AppSettings["quartz.jobStore.dataSource"]; } 
        }

        public string quartz_jobStore_tablePrefix {
            get{ return ConfigurationManager.AppSettings["quartz.jobStore.tablePrefix"]; } 
        }

        public string quartz_jobStore_driverDelegateType {
            get{ return ConfigurationManager.AppSettings["quartz.jobStore.driverDelegateType"]; } 
        }
        public string quartz_jobStore_lockHandler_type
        {
            get { return ConfigurationManager.AppSettings["quartz.jobStore.lockHandler.type"]; }
        }
        public string quartz_dataSource_default_provider
        {
            get{ return ConfigurationManager.AppSettings["quartz.dataSource.default.provider"]; } 
        }
        public NameValueCollection GetQuartzPara()
        {
            NameValueCollection properties = new NameValueCollection();
            QuartzPara para = new QuartzPara();
            properties["quartz.threadPool.type"] = quartz_threadPool_type;
            properties["quartz.threadPool.threadCount"] = quartz_threadPool_threadCount;
            properties["quartz.threadPool.threadPriority"] = quartz_threadPool_threadPriority;
            properties["quartz.jobStore.misfireThreshold"] = quartz_jobStore_misfireThreshold;
            properties["quartz.jobStore.type"] = quartz_jobStore_type;
            properties["quartz.jobStore.useProperties"] = quartz_jobStore_useProperties;
            properties["quartz.jobStore.dataSource"] = quartz_jobStore_dataSource;
            properties["quartz.jobStore.tablePrefix"] = quartz_jobStore_tablePrefix;
            // if running SQLite we need this
            properties["quartz.jobStore.lockHandler.type"] = quartz_jobStore_lockHandler_type;
            properties["quartz.jobStore.driverDelegateType"] = quartz_jobStore_driverDelegateType;

            properties["quartz.dataSource.default.connectionString"] = ConfigPara.QuartzDBConnection;
            properties["quartz.dataSource.default.provider"] = quartz_dataSource_default_provider;
            return properties;
        }
    }

    public class WebChatPara
    {
        public static string ApiUrl
        {
            get { return ConfigurationManager.AppSettings["WebChatApiUrl"]; }
        }

        public static string SiteUrl
        {
            get { return ConfigurationManager.AppSettings["SiteUrl"]; }
        }
    }

}
