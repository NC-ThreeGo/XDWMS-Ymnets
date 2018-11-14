using System;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System.Reflection;
using Apps.Common;
using Apps.BLL;
using Apps.DAL;
using Apps.Models;
using Apps.IBLL;
using Apps.Models.JOB;
using Apps.DAL.JOB;
using Apps.IBLL.JOB;
using Apps.BLL.JOB;

namespace Apps.Jobs
{
    public class TaskRun : ITaskRun
    {

        public const string JOB_PRIFEX = "JOB_";
        public const string TRIGGER_PRIFEX = "TRG_";
        public JobModel CreateJobModel(string jobinfo)
        {
            try
            {
                JobModel jobmodel = new JobModel();
                char[] p = { '^', '&' };
                string[] items = jobinfo.Split(p);
                jobmodel.seconds = items[1];
                jobmodel.minutes = items[3];
                jobmodel.hours = items[5];
                jobmodel.dayOfMonth = items[7];
                jobmodel.month = items[9];
                jobmodel.dayOfWeek = items[11];
                jobmodel.year = items[13];
                jobmodel.repeatCount = string.IsNullOrEmpty(items[15]) ? 0 : int.Parse(items[15]);
                jobmodel.intervalCount = string.IsNullOrEmpty(items[17]) ? 0 : int.Parse(items[17]);
                jobmodel.intervalType = items[19];

                DateTime startDate;
                if (string.IsNullOrEmpty(items[21]))
                {
                    startDate = DateTime.Now;
                }
                else
                {
                    if (!DateTime.TryParse(items[21], out startDate))
                    {
                        startDate = DateTime.Now;
                    }
                }

                DateTime endDate;
                if (string.IsNullOrEmpty(items[23]))
                {
                    endDate = DateTime.Now;
                }
                else
                {
                    if (!DateTime.TryParse(items[23], out endDate))
                    {
                        endDate = DateTime.Now;
                    }
                }

                jobmodel.startDate = startDate;
                jobmodel.endDate = endDate;
                jobmodel.taskType = int.Parse(items[25]);
                jobmodel.taskName =items[27];
                jobmodel.id = items[29];
                jobmodel.cronExpression = items[31];
                jobmodel.repeatForever = items[33] == null || items[33] == "false" ? false : true;
                jobmodel.taskTitle = items[35];
                jobmodel.creator = items[37];
                jobmodel.procName = items[39];
                jobmodel.procParams = items[41];


                return jobmodel;
            }
            catch
            {
                return null;
            }
        }
        public JobModel CreateJobModel(string jobinfo, string jobName, string id)
        {
            JobModel jobModel = CreateJobModel(jobinfo);
            if (jobModel != null)
            {
                jobModel.taskName = jobName;
                jobModel.id = id;
                return jobModel;
            }
            return null; 
        }
        public JobModel CreateJobModel(string jobinfo, string jobName, string id, string taskTitle)
        {
            JobModel jobModel = CreateJobModel(jobinfo, jobName, id);
            if (jobModel != null)
            {
                jobModel.taskTitle = taskTitle;
                return jobModel;
            }
            return null;

        }
        /// <summary>
        /// 简单任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public int runSample<T>(JobModel model, ref ValidationErrors errors) where T : IJob
        {
            try
            {
                //是否为简单任务            
                if (model.taskType != 0)
                {
                    errors.Add("不是简单任务");
                    return 0;
                }

                //间隔类型
                if (string.IsNullOrEmpty(model.intervalType))
                {
                    errors.Add("未设置间隔类型");
                    return 0;
                }
                //时间间隔
                if (model.intervalCount < 0)
                {
                    errors.Add("未设置间隔次数(0为无限次)");
                    return 0;
                }
                //如果开始时间少于当前时间,取当前时间

                if (DateTime.Now > model.startDate)
                {
                    model.startDate = DateTime.Now;
                }

                //执行前置任务
                string error = RunBefore(model);
                if (!string.IsNullOrEmpty(error))
                {
                    errors.Add(error);
                    return 0;
                }
                /////////////////////////////////////////////////////////////
                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                /////////////////////////////////////////////////////////////////////////

                //在开始时间的10秒后执行
                DateTimeOffset runTime = new DateTimeOffset(model.startDate).AddSeconds(10);

                string Description = model.taskName + "任务时间:" + runTime + ",创建时间:" + DateTime.Now;
                //生成作业名称,作业名称=任务名称 +任务ID
                string jobName = model.taskName + "_" + model.id + "_" + NewGuid();

                //向Job_TaskJobs增加任务

                string sno = JOB_PRIFEX + jobName;
                string intervalTitle = model.intervalType == "second" ? "秒" : model.intervalType == "minute" ? "分" : model.intervalType == "hour" ? "小时" : "天";

                string teskCmd;
                if (model.repeatForever)
                {
                    teskCmd = "start:" + model.startDate + " 重复:无限次 间隔:" + model.intervalCount + " " + intervalTitle + " ";
                }
                else
                {
                    teskCmd = "start:" + model.startDate + " 重复:" + model.repeatCount + " 间隔:" + model.intervalCount + " " + intervalTitle + " ";
                }
                JOB_TASKJOBSModel taskJobsModel = new JOB_TASKJOBSModel();
                taskJobsModel.sno = sno;
                taskJobsModel.taskName = model.taskName;
                taskJobsModel.Id = model.id;
                taskJobsModel.taskTitle = model.taskTitle;
                taskJobsModel.taskCmd = teskCmd;
                taskJobsModel.crtDt = DateTime.Now;
                taskJobsModel.state = 0;
                taskJobsModel.creator = model.creator;
                taskJobsModel.procName = model.procName;
                taskJobsModel.procParams = model.procParams;

                IJOB_TASKJOBSBLL taskJobsBLL = new JOB_TASKJOBSBLL()
                {
                    m_Rep = new JOB_TASKJOBSRepository(new DBContainer())
                };
                if (!taskJobsBLL.Create(ref errors, taskJobsModel))
                {
                    return 0;
                }
                /////////////////////////////////////////////////

                IJobDetail job = JobBuilder.Create<T>()
                    .WithIdentity(JOB_PRIFEX + jobName, "group1")
                    .WithDescription(Description)
                    .Build();
                //

                ITrigger trigger = null;
                //执行重复次数
                if (model.repeatForever)
                {
                    //无限次
                    switch (model.intervalType.ToLower())
                    {
                        case "day"://日
                            TimeSpan ts = new TimeSpan(model.intervalCount, 0, 0, 0);

                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithInterval(ts).RepeatForever())
                                .WithDescription(Description)
                                .Build();

                            break;
                        case "hour"://小时
                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithIntervalInHours(model.intervalCount).RepeatForever())
                                .WithDescription(Description)
                                .Build();
                            break;
                        case "minute"://分
                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithIntervalInMinutes(model.intervalCount).RepeatForever())
                                .WithDescription(Description)
                                .Build();
                            break;

                        case "second"://秒
                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithIntervalInSeconds(model.intervalCount).RepeatForever())
                                .WithDescription(Description)
                                .Build();
                            break;
                        default:
                            trigger = TriggerBuilder.Create()
                                                  .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                                  .StartAt(model.startDate)
                                                  .WithDescription(Description)
                                                  .Build();
                            break;
                    }
                }
                else
                {
                    //有限次
                    switch (model.intervalType.ToLower())
                    {
                        case "day"://日
                            TimeSpan ts = new TimeSpan(model.intervalCount, 0, 0, 0);

                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithInterval(ts).WithRepeatCount(model.repeatCount))
                                .WithDescription(Description)
                                .Build();

                            break;
                        case "hour"://小时
                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithIntervalInHours(model.intervalCount).WithRepeatCount(model.repeatCount))
                                .WithDescription(Description)
                                .Build();
                            break;
                        case "minute"://分
                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithIntervalInMinutes(model.intervalCount).WithRepeatCount(model.repeatCount))
                                .WithDescription(Description)
                                .Build();
                            break;

                        case "second"://秒
                            trigger = TriggerBuilder.Create()
                                .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                .StartAt(model.startDate)
                                .WithSimpleSchedule(a => a.WithIntervalInSeconds(model.intervalCount).WithRepeatCount(model.repeatCount))
                                .WithDescription(Description)
                                .Build();
                            break;
                        default:
                            trigger = TriggerBuilder.Create()
                                                  .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                                                  .StartAt(model.startDate)
                                                  .WithDescription(Description)
                                                  .Build();
                            break;
                    }

                }

                //如果已存在相同任务删除
                if (sched.CheckExists(job.Key))
                {
                    sched.DeleteJob(job.Key);
                }
                //传入任务关键字参数(任务ID标识、任务名称)
                job.JobDataMap.Put(TaskJob.ID, model.id);//传入任务ID标识
                job.JobDataMap.Put(TaskJob.TASKNAME, model.taskName);//传入任务名称，带命名空间

                //传入任务其他参数列表
                if (model.param != null)
                {


                    for (int i = 0; i < model.param.GetLength(0); i++)
                    {
                        job.JobDataMap.Put(model.param[i, 0], model.param[i, 1]);
                    }
                }
                //////////////////////////////////////////////////////////
                DateTimeOffset dtoff = sched.ScheduleJob(job, trigger);


                return 1;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return 0;
            }

        }

        public bool CloseTaskJob(string sno, ref ValidationErrors errors)
        {
            try
            {

                if (string.IsNullOrEmpty(sno))
                {
                    errors.Add("任务序号不能为空");
                    return false;
                }
                /////////////////////////////////////////////////////////////
                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                /////////////////////////////////////////////////////////////////////////

                //如果已存在相同任务删除
                foreach (var group in sched.GetJobGroupNames())
                {
                    var jobKeys = sched.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                    foreach (var jobkey in jobKeys)
                    {
                        if (jobkey.Name.Contains(sno))
                        {
                            return sched.DeleteJob(jobkey);

                        }
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return false;
            }
        }
        public bool CloseTaskJob(JobModel model, ref ValidationErrors errors)
        {
            try
            {
                //执行前置任务
                string error = CloseTaskJob(model);

                if (!string.IsNullOrEmpty(error))
                {
                    errors.Add(error);
                    return false;
                }
                /////////////////////////////////////////////////////////////
                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                /////////////////////////////////////////////////////////////////////////

                //生成作业名称,作业名称=任务名称 +任务ID
                string jobName = model.taskName + "_" + model.id;

                //如果已存在相同任务删除
                int count = 0;
                foreach (var group in sched.GetJobGroupNames())
                {
                    var jobKeys = sched.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                    foreach (var jobkey in jobKeys)
                    {
                        if (jobkey.Name.Contains(JOB_PRIFEX + jobName))
                        {
                            sched.DeleteJob(jobkey);
                            count++;
                        }
                    }

                }
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 复杂任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public int runComplex<T>(JobModel model, ref ValidationErrors errors) where T : IJob
        {
            try
            {
                //是否为简单任务            
                if (model.taskType != 1)
                {
                    errors.Add("不是复杂任务");
                    return 0;
                }

                if (string.IsNullOrEmpty(model.cronExpression))
                {
                    errors.Add("任务表达式不能为空，请正确执行时间");
                    return 0;
                }

                //对表达式进行检查

                //执行前置任务
                string error = RunBefore(model);
                if (!string.IsNullOrEmpty(error))
                {
                    errors.Add(error);
                    return 0;
                }

                /////////////////////////////////////////////////////////////////
                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                /////////////////////////////////////////////////////////////////////////

                //在开始时间的10秒后执行
                DateTimeOffset runTime = new DateTimeOffset(model.startDate).AddSeconds(10);

                string Description = model.taskName + "任务时间:" + runTime + ",创建时间:" + DateTime.Now;
                //生成作业名称,作业名称=任务名称 +任务ID+序号
                string jobName = model.taskName + "_" + model.id + "_" + NewGuid();
                //向Job_TaskJobs增加任务
                string sno = JOB_PRIFEX + jobName;

                string taskCmd;
                taskCmd = "秒:" + model.seconds + " 分:" + model.minutes + " 小时:" + model.hours +
                        " 日/月:" + model.dayOfMonth + " 月:" + model.month + " 日/周:" + model.dayOfWeek + " 指令表达式:" + model.cronExpression;

                JOB_TASKJOBSModel taskJobsModel = new JOB_TASKJOBSModel();
                taskJobsModel.sno = sno;
                taskJobsModel.taskName = model.taskName;
                taskJobsModel.Id = model.id;
                taskJobsModel.taskTitle = model.taskTitle;
                taskJobsModel.taskCmd = taskCmd;
                taskJobsModel.crtDt = DateTime.Now;
                taskJobsModel.state = 0;
                taskJobsModel.creator = model.creator;
                IJOB_TASKJOBSBLL taskJobsBLL = new JOB_TASKJOBSBLL()
                {
                    m_Rep = new JOB_TASKJOBSRepository(new DBContainer())
                };
                if (!taskJobsBLL.Create(ref errors, taskJobsModel))
                {
                    return 0;
                }
                /////////////////////////////////////////////////
                IJobDetail job = JobBuilder.Create<T>()
                    .WithIdentity(JOB_PRIFEX + jobName, "group1")
                    .WithDescription(Description)
                    .Build();
                //



                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                    .WithIdentity(TRIGGER_PRIFEX + jobName, "group1")
                    .WithCronSchedule(model.cronExpression)
                    .WithDescription(Description)
                    .Build();

                //如果已存在相同任务删除
                if (sched.CheckExists(job.Key))
                {
                    sched.DeleteJob(job.Key);
                }
                //传入任务关键字参数(任务ID标识、任务名称)
                job.JobDataMap.Put(TaskJob.ID, model.id);//传入任务ID标识
                job.JobDataMap.Put(TaskJob.TASKNAME, model.taskName);//传入任务名称，带命名空间

                //传入任务其他参数列表
                if (model.param != null)
                {


                    for (int i = 0; i < model.param.GetLength(0); i++)
                    {
                        job.JobDataMap.Put(model.param[i, 0], model.param[i, 1]);
                    }
                }

                //////////////////////////////////////////////////////////
                DateTimeOffset dtoff = sched.ScheduleJob(job, trigger);
                return 1;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return 0;
            }

        }
        /// <summary>
        /// 生成任务序号
        /// </summary>
        /// <returns></returns>
        private string NewGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        //执行作业前任务
        private string RunBefore(JobModel model)
        {
            try
            {
                //执行作业前任务
                Log.Write(model.taskName, "[前置任务]开始>>>>>>" + model.taskName + "_" + model.id, "成功");

                //取当前程序集
                Assembly assem = Assembly.GetExecutingAssembly();

                //创建任务对象并执行
                Object o = assem.CreateInstance(model.taskName, false,
                    BindingFlags.ExactBinding,
                    null, new Object[] { }, null, null);

                MethodInfo m = assem.GetType(model.taskName).GetMethod("RunJobBefore");//默认调用方法
                Object ret = m.Invoke(o, new Object[] { model }); //参数


                //更新任务状态
                string sno = JOB_PRIFEX + model.taskName + "_" + model.id;

                ValidationErrors errors = new ValidationErrors();
                TaskJob.UpdateState(ref errors, sno, 0, "初始化数据");

                Log.Write(model.taskName, "<<<<<<<[前置任务]结束" + model.taskName + "_" + model.id,"成功");

                return ret.GetString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        private string CloseTaskJob(JobModel model)
        {
            try
            {
                //执行作业前任务
                Log.Write(model.taskName, "[关闭任务]开始>>>>>>" + model.taskName + "_" + model.id, "成功");
                //生成作业名称,作业名称=任务名称 +任务ID+序号
                string jobName = model.taskName + "_" + model.id;
                //更新任务状态
                string sno = JOB_PRIFEX + jobName;

                ValidationErrors errors = new ValidationErrors();
                TaskJob.UpdateState(ref errors, sno, 2, "关闭任务");
                /////////////////////////////////////////////////
                //取当前程序集
                Assembly assem = Assembly.GetExecutingAssembly();

                //创建任务对象并执行
                Object o = assem.CreateInstance(model.taskName, false,
                    BindingFlags.ExactBinding,
                    null, new Object[] { }, null, null);

                MethodInfo m = assem.GetType(model.taskName).GetMethod("CloseJob");//默认调用方法
                Object ret = m.Invoke(o, new Object[] { model }); //参数

                Log.Write(model.taskName, "<<<<<<<[关闭任务]结束" + model.taskName + "_" + model.id, "成功");

                return ret.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool ResumeTaskJob(string sno, ref ValidationErrors errors)
        {
            try
            {
                if (string.IsNullOrEmpty(sno))
                {
                    errors.Add("任务序号不能为空");
                    return false;
                }
                /////////////////////////////////////////////////////////////
                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                /////////////////////////////////////////////////////////////////////////

                //重启任务
                int count = 0;
                foreach (var group in sched.GetJobGroupNames())
                {
                    var jobKeys = sched.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                    foreach (var jobkey in jobKeys)
                    {
                        if (jobkey.Name.Contains(sno))
                        {
                            sched.ResumeJob(jobkey);
                            TaskJob.UpdateState(ref errors, sno, 4, "重启");
                            count++;
                        }
                    }
                }
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return false;
            }
        }
        public bool PauseTaskJob(string sno, ref ValidationErrors errors)
        {

            if (string.IsNullOrEmpty(sno))
            {
                errors.Add("任务序号不能为空");
                return false;
            }
            /////////////////////////////////////////////////////////////
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            /////////////////////////////////////////////////////////////////////////

            //停止任务
            int count = 0;
            foreach (var group in sched.GetJobGroupNames())
            {
                var jobKeys = sched.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                foreach (var jobkey in jobKeys)
                {
                    if (jobkey.Name.Contains(sno))
                    {

                        sched.PauseJob(jobkey);
                        TaskJob.UpdateState(ref errors, sno, 3, "挂起");
                        count++;
                    }
                }

            }
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int ClearUnTrackTask(ref ValidationErrors errors)
        {
            try
            {
                IJOB_TASKJOBSBLL taskJobsBLL = new JOB_TASKJOBSBLL()
                {
                    m_Rep = new JOB_TASKJOBSRepository(new DBContainer())
                };
                /////////////////////////////////////////////////////////////
                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                /////////////////////////////////////////////////////////////////////////

                List<P_JOB_GetUnTrackTask_Result> taskNameList = taskJobsBLL.GetUnTrackTask();

                int count = 0;
                foreach (var taskName in taskNameList)
                {
                    string jobName = taskName.JOB_NAME;
                    foreach (var group in sched.GetJobGroupNames())
                    {
                        var jobKeys = sched.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                        foreach (var jobkey in jobKeys)
                        {
                            if (jobkey.Name.Contains(jobName))
                            {
                                sched.DeleteJob(jobkey);
                                count++;
                            }
                        }

                    }

                }
                return count;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return 0;
            }
        }
    }
}
