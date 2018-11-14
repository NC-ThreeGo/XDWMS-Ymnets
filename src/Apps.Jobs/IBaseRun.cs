using System;
namespace Apps.Jobs
{
    interface IBaseRun
    {
        int run<T>(string taskName,string parm, string value, string task) where T : Quartz.IJob;
        int run<T>(string taskName, string[,] param, string task) where T : Quartz.IJob;
    }
}
