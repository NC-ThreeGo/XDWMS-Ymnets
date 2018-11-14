using Quartz;
using System;
using System.Reflection;
using Apps.Common;
using Apps.BLL;
using Apps.DAL;
using Apps.Models;
using Apps.IBLL;
using Apps.Models.JOB;
using Apps.BLL.JOB;
using Apps.IBLL.JOB;
using Apps.DAL.JOB;

namespace Apps.Jobs
{
    public class CutstomProcedureJob : IJob, ITaskJob
    {
        
        public const string ID = "ID";//任务ID关键字
        public const string TASKNAME = "TASKNAME";//任务名称
        public const string PROCNAME = "PROCNAME";//过程名称
        public const string PROCPARARMS = "PROCPARARMS";//过程参数
      
        public static void UpdateState(ref ValidationErrors errors,string jobName,int state,string result)
        {
            IJOB_TASKJOBSBLL taskJobsBLL = new JOB_TASKJOBSBLL()
            {

                m_Rep = new JOB_TASKJOBSRepository(new DBContainer())
            };

            if (!taskJobsBLL.UpdateState(ref errors, jobName, state, result))
            {
                Log.Write(jobName, "更新任务状态异常:" + errors.Error,"失败");
            }              
        }

        public virtual void Execute(IJobExecutionContext context)
        {
            ValidationErrors validationErrors = new ValidationErrors();
            //取状态值
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string id = dataMap.GetString(ID);//任务ID
            string taskName = dataMap.GetString(TASKNAME);//任务名称
            string procName = dataMap.GetString(PROCNAME);//过程名称
            string procParams = dataMap.GetString(PROCPARARMS);//过程参数
            //
            JobKey jobKey = context.JobDetail.Key;
            string jobName = jobKey.Name;//任务名称=任务名称+任务ID
            try
            {

                //开始执行业务逻辑
                Log.Write(jobName, "开始任务[过程]>>>>>>" + jobName, "成功");

                ////过程入口
                using (DBContainer db = new DBContainer())
                {
                    db.P_JOB_ENTERY(jobName);
                }
                
               
                
                //////////////////////////////////////////////////////////////////
                if (validationErrors.Count > 0)
                {
                    Log.Write(jobName, "Error", validationErrors.Error);
                    //更新任务状态
                    TaskJob.UpdateState(ref validationErrors, jobName, 1, "失败");
                }
                Log.Write(jobName, "<<<<<<<结束任务[过程]" + jobName, "成功");
                //更新任务状态
                TaskJob.UpdateState(ref validationErrors, jobName, 1, "成功");
            }
            catch (System.Exception e)
            {
                Log.Write(jobName, "Exception", e.Message);
                JobExecutionException e2 = new JobExecutionException(e);
                e2.UnscheduleAllTriggers = true;
                throw e2;
            }

         
        }

        public string RunJobBefore(JobModel jobModel)
        {
            return null;
        }

        public string CloseJob(JobModel jobModel)
        {
            return null;
        }

        public string RunJob(ref JobDataMap dataMap, string jobName, string id, string taskName)
        {
            return null;
        }
    }
}
