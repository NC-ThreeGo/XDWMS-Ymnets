using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Apps.DAL;
using Apps.BLL;
using Apps.Common;
using System.IO;
using System.Text;
using Apps.BLL.Core;
using Apps.Models;
using Apps.DAL.Sys;

namespace Apps.Jobs
{
    public class Log
    {
        public static void Write(string jobName, string message,string Result)
        {
            try
            {

                SysLog entity = new SysLog();
                entity.Id = ResultHelper.NewId;
                entity.Operator = "Scheduler";
                entity.Message = "jobName:"+jobName+"message"+message;
                entity.Result = Result;
                entity.Type = "调度";
                entity.Module = "任务调度";
                entity.CreateTime = ResultHelper.NowTime;
                using (SysLogRepository logRepository = new SysLogRepository(new DBContainer()))
                {
                    logRepository.Create(entity);
                }


            }
            catch (Exception ep)
            {
                ExceptionHander.WriteException(ep);
            }

        }

   

        public void Dispose()
        {

        }
    }
}