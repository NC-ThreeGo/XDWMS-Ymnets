using Quartz;
using System;
using System.Reflection;
using Apps.Common;
using Apps.BLL;
using Apps.DAL;
using Apps.Models;
using Apps.IBLL;
using Apps.DAL.JOB;
using Apps.IBLL.JOB;
using Apps.BLL.JOB;

namespace Apps.Jobs
{

    public class TaskJob : IJob
    {
        public const string ID = "ID";//任务ID关键字
        public const string TASKNAME = "TASKNAME";//任务名称
        public TaskJob()
        {
        }

        public static void UpdateState(ref ValidationErrors errors,string jobName,int state,string result)
        {
            IJOB_TASKJOBSBLL taskJobsBLL = new JOB_TASKJOBSBLL()
            {
                m_Rep = new JOB_TASKJOBSRepository(new DBContainer())
            };

            if (!taskJobsBLL.UpdateState(ref errors, jobName, state, result))
            {
                Log.Write(jobName, "更新任务状态异常:" + errors.Error, "失败");
            }              
        }

        public virtual void Execute(IJobExecutionContext context)
        {
            ValidationErrors validationErrors = new ValidationErrors();
            //取状态值
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string id = dataMap.GetString(ID);//任务ID
            string taskName = dataMap.GetString(TASKNAME);//任务名称
            //
            JobKey jobKey = context.JobDetail.Key;
            string jobName = jobKey.Name;//任务名称=任务名称+任务ID
            try
            {

                //开始执行业务逻辑
                Log.Write(jobName, "开始任务>>>>>>" + jobName,"成功");

               
                //取当前程序集
                Assembly assem = Assembly.GetExecutingAssembly();

                //创建任务对象并执行
                Object o = assem.CreateInstance(taskName, false,
                    BindingFlags.ExactBinding,
                    null, new Object[] { }, null, null);

                MethodInfo m = assem.GetType(taskName).GetMethod("RunJob");//默认调用方法
                Object ret = m.Invoke(o, new Object[] { dataMap, jobName, id, taskName });
                //更新任务状态
                TaskJob.UpdateState(ref validationErrors, jobName, 1, ret.ToString());
                
                //////////////////////////////////////////////////////////////////
                if (validationErrors.Count > 0)
                {
                    Log.Write(jobName, "Error", validationErrors.Error);
                }
                Log.Write(jobName, "<<<<<<<结束任务" + jobName, "成功");
            }
            catch (System.Exception e)
            {
                Log.Write(jobName, "Exception", e.Message);
                JobExecutionException e2 = new JobExecutionException(e);
                e2.UnscheduleAllTriggers = true;
                throw e2;
            }

         
        }
    }


}