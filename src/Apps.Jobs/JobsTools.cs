using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Apps.Common;
using Quartz.Impl;
using Apps.Jobs;
using Apps.Models.JOB;
namespace Apps.Jobs
{
    public class JobsTools
    {
        /// <summary>
        /// 简单任务
        /// </summary>
        /// <param name="Errors">错误集合</param>
        /// <param name="task">任务内容，包括时间及任务类型等</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="taskId">任务ID号，与被处理的业务ID关联</param>
        /// <returns></returns>
        public static int CreateTaskJob(ref ValidationErrors Errors, string task, string taskName, string taskId)
        {

            ITaskRun taskRun = new TaskRun();
            JobModel jobmodel = taskRun.CreateJobModel(task, taskName, taskId);
            if (jobmodel != null)
            {
                if (jobmodel.taskType == 0)
                {
                    //执行简单任务
                    return taskRun.runSample<TaskJob>(jobmodel, ref Errors);
                }
                else
                {
                    //执行复杂任务
                    return taskRun.runComplex<TaskJob>(jobmodel, ref Errors);
                }
            }

            return 0;
        }
        public static int CreateCustomProcedureJob(ref ValidationErrors Errors, string task)
        {
            
            string taskName,taskId;
            taskName = "Apps.Jobs.CutstomProcedureJob";
            taskId = Guid.NewGuid().ToString().Replace("-", "");
            
            ITaskRun taskRun = new TaskRun();
            JobModel jobmodel = taskRun.CreateJobModel(task, taskName, taskId);
            if (jobmodel != null)
            {
                if (jobmodel.taskType == 0)
                {
                    //执行简单任务
                    return taskRun.runSample<CutstomProcedureJob>(jobmodel, ref Errors);
                }
                else
                {
                    //执行复杂任务
                    return taskRun.runComplex<CutstomProcedureJob>(jobmodel, ref Errors);
                }
            }

            return 0;
        }
        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="Errors">错误返回</param>
        /// <param name="task">任务</param>
        /// <param name="taskName">命名空间类</param>
        /// <param name="taskId">可利用标识</param>
        /// <param name="taskTitle">任务标题</param>
        /// <returns></returns>
        public static int CreateTaskJob(ref ValidationErrors Errors, string task, string taskName, string taskId,string taskTitle)
        {

            ITaskRun taskRun = new TaskRun();
            JobModel jobmodel = taskRun.CreateJobModel(task, taskName, taskId,taskTitle);
            if (jobmodel != null)
            {
                if (jobmodel.taskType == 0)
                {
                    //执行简单任务
                    return taskRun.runSample<TaskJob>(jobmodel, ref Errors);
                }
                else
                {
                    //执行复杂任务
                    return taskRun.runComplex<TaskJob>(jobmodel, ref Errors);
                }
            }

            return 0;
        }
        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="Errors"></param>
        /// <param name="taskName"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static int CloseTaskJob(ref ValidationErrors Errors, string taskName, string taskId)
        {

            ITaskRun taskRun = new TaskRun();
            JobModel jobmodel = new JobModel()
            {
                id = taskId,
                taskName = taskName,

            };
            if (jobmodel != null)
            {

                if (taskRun.CloseTaskJob(jobmodel, ref Errors))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            }

            return 0;
        }
        /// <summary>
        /// 中断任务
        /// </summary>
        /// <param name="Errors"></param>
        /// <param name="sno"></param>
        /// <returns></returns>
        public static int PauseTaskJob(ref ValidationErrors Errors, string sno)
        {
            ITaskRun taskRun = new TaskRun();

            if (taskRun.PauseTaskJob(sno, ref Errors))
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="Errors"></param>
        /// <param name="sno"></param>
        /// <returns></returns>
        public static int ResumeTaskJob(ref ValidationErrors Errors, string sno)
        {
            ITaskRun taskRun = new TaskRun();

            if (taskRun.ResumeTaskJob(sno, ref Errors))
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public static int ClearUnTrackTask(ref ValidationErrors errors)
        {
            try
            {
                ITaskRun taskRun = new TaskRun();
                return taskRun.ClearUnTrackTask(ref errors);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="Errors"></param>
        /// <param name="sno"></param>
        /// <returns></returns>
        public static int CloseTaskJob(ref ValidationErrors Errors, string sno)
        {

            ITaskRun taskRun = new TaskRun();

            if (taskRun.CloseTaskJob(sno, ref Errors))
            {
                return 1;
            }
            else
            {
                return 0;
            }


        }
    }
}
