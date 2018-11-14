using Apps.Models.JOB;
namespace Apps.Jobs
{
    interface ITaskJob
    {
        string RunJobBefore(JobModel jobModel);
        string CloseJob(JobModel jobModel);
        string RunJob(ref Quartz.JobDataMap dataMap, string jobName, string id, string taskName);
    }
}
