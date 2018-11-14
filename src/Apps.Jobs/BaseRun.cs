using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Apps.Common;
using Quartz.Impl;
using Apps.DAL;
using Apps.Models;
using System.Data.Entity.Core.Objects;
namespace Apps.Jobs
{
    public class BaseRun : IBaseRun 
    {
        public static string GetJobExecuteTime(string id)
        {
            using (DBContainer db = new DBContainer())
            {
                //定义trigger名称
                string triggerName="Trigger_"+id;
                ObjectResult<P_QRTZ_GetJobExecuteTime_Result> result = db.P_QRTZ_GetJobExecuteTime(triggerName);
                if (result == null)
                {
                    return "";
                }
                var tigResult = result.SingleOrDefault();
                if (tigResult == null)
                {
                    return "";
                }
                long starttime = tigResult.START_TIME;
                TimeSpan t = new TimeSpan(0);
                DateTimeOffset df = new DateTimeOffset(starttime, t);
                return df.DateTime.ToLocalTime().ToString();
            }
        }
        /// <summary>
        /// 执行作业
        /// </summary>
        /// <typeparam name="T">实现IRunJob接口的类型</typeparam>
        /// <param name="taskName">任务名称</param>
        /// <param name="parm">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="task">任务内容(是否执行[0,1],执行时间)</param>
        /// <returns></returns>
        public int run<T>(string taskName, string parm, string value, string task) where T : IJob
        {
            
            //分解任务内容
            string[] aJobplan = task.Split(',');
            if (aJobplan.Length != 2)
            {
                return 0;
            }
            bool plan = (aJobplan[0] == "true" ? true : false);
            if (!plan)
            {
                return 0;
            }

            DateTime plantime = DateTime.Parse(aJobplan[1]);
            if (DateTime.Now > plantime)
            {
                return 0;
            }
            
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            /////////////////////////////////////////////////////////////////////////

            //作业
            string Description =taskName+ "任务时间:" + plantime + ",创建时间:" + DateTime.Now;
            DateTimeOffset runTime = new DateTimeOffset(plantime);


            string schName = taskName + "_" + value;


            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity("Job_" + schName, "group1")
                .WithDescription(Description)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("Trigger_" + schName, "group1")
                .StartAt(runTime)
                .WithDescription(Description)
                .Build();

            //如果已存在相同任务删除
            if (sched.CheckExists(job.Key))
            {
                sched.DeleteJob(job.Key);
            }

            job.JobDataMap.Put(parm, value);
            
            //////////////////////////////////////////////////////////
            sched.ScheduleJob(job, trigger);

            return 1;
        }

        /// <summary>
        /// 执行作业
        /// </summary>
        /// <typeparam name="T">实现IRunJob接口的类型</typeparam>
        /// <param name="taskName">任务名称</param>
        /// <param name="param">参数数组(名称,值)</param>
        /// <param name="task">任务内容(是否执行[0,1],执行时间)</param>
        /// <returns></returns>
        public int run<T>(string taskName, string[,] param, string task) where T : IJob
        {
            //分解任务内容
            string[] aJobplan = task.Split(',');
            if (aJobplan.Length != 2)
            {
                return 0;
            }
            bool plan = (aJobplan[0] == "true" ? true : false);
            if (!plan)
            {
                return 0;
            }

            DateTime plantime = DateTime.Parse(aJobplan[1]);
            if (DateTime.Now > plantime)
            {
                return 0;
            }

            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            /////////////////////////////////////////////////////////////////////////

            //作业1
            
            DateTimeOffset runTime = new DateTimeOffset(plantime);
            string Description = taskName + "任务时间:" + runTime + ",创建时间:" + DateTime.Now;
            //生成作业名称
            string schName="";
            if ( param.Length > 0)
            {
                schName = taskName + "_" + param[0,1];
            }
            else
            {
                schName = taskName + "_" + ResultHelper.NewId;
            }

           


            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity("Job_" + schName, "group1")
                .WithDescription(Description)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("Trigger_" + schName, "group1")
                .StartAt(runTime)
                .WithDescription(Description)
                .Build();
            //如果已存在相同任务删除
            if (sched.CheckExists(job.Key))
            {
                sched.DeleteJob(job.Key);
            }
            //传入参数
            for(int i=0;i< param.GetLength(0);i++)
            {
                job.JobDataMap.Put(param[i,0], param[i,1]);
            }

            //////////////////////////////////////////////////////////
            sched.ScheduleJob(job, trigger);

            return 1;
        }


    }
}
