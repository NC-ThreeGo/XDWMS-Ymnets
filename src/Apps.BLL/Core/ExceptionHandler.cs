using System;
using System.Web.Configuration;
using Apps.Models;
using System.IO;
using System.Text;
using Apps.Common;
using Apps.Models.Sys;
using Apps.BLL.Sys;

namespace Apps.BLL.Core
{

        /// <summary>
        /// 写入一个异常错误
        /// </summary>
        /// <param name="ex">异常</param>
        public static class ExceptionHander
        {
            /// <summary>
            /// 加入异常日志
            /// </summary>
            /// <param name="ex">异常</param>
            public static void WriteException(Exception ex)
            {
                SysConfigModel siteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));
                //后台异常开启
                if (siteConfig.exceptionstatus == 1)
                {
                    try
                    {
                       using( DBContainer db = new DBContainer())
                       { 
                        
                            SysException model = new SysException()
                            {
                                Id = ResultHelper.NewId,
                                HelpLink = ex.HelpLink,
                                Message = ex.Message,
                                Source = ex.Source,
                                StackTrace = ex.StackTrace,
                                TargetSite = ex.TargetSite.ToString(),
                                Data =ex.Data.ToString(),
                                CreateTime = ResultHelper.NowTime
                                
                            };
                            db.SysException.Add(model);
                            db.SaveChanges();
                       }
                        
                    }
                    catch (Exception ep)
                    {
                        string subFold = DateTime.Now.Year + DateTime.Now.Month.ToString("D2");
                        string fileName = subFold + DateTime.Now.Day.ToString("D2") + ".txt";
                        string path = System.Web.HttpContext.Current.Server.MapPath("~/LogFile/") + subFold;
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string wholePath = path + "\\" + fileName;
                        using (StreamWriter sw = new StreamWriter(wholePath, true, Encoding.UTF8))
                        {
                            sw.WriteLine((ex.Message + "|" + ex.StackTrace + "|" + ep.Message + "|" + DateTime.Now.ToString()).ToString());
                            sw.Dispose();
                            sw.Close();
                        }
                            return;
                        }
                    }

                }

            }
        }
