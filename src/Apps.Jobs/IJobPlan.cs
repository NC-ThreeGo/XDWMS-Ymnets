using System;
using Quartz.Impl;
using Quartz;
namespace Apps.Jobs
{
    public interface IJobRun
    {
        int run(string id, string task);
    }
}
