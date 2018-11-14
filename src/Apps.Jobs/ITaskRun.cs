using System;
using Quartz;
using Apps.Common;
using Apps.Models.JOB;


namespace Apps.Jobs
{
    public interface ITaskRun
    {
        int runSample<T>(JobModel model, ref ValidationErrors errors) where T : IJob;
        int runComplex<T>(JobModel model, ref ValidationErrors errors) where T : IJob;
        JobModel CreateJobModel(string jobinfo);
        JobModel CreateJobModel(string jobinfo, string jobName, string id);
        JobModel CreateJobModel(string jobinfo, string jobName, string id,string taskTitle);
        bool CloseTaskJob(JobModel model, ref ValidationErrors errors);
        bool CloseTaskJob(string sno, ref ValidationErrors errors);
        bool ResumeTaskJob(string sno, ref ValidationErrors errors);
        bool PauseTaskJob(string sno, ref ValidationErrors errors);
        int ClearUnTrackTask(ref ValidationErrors errors);
    }
}
