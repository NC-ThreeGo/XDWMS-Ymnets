using System;
using System.Collections.Generic;
using System.Linq;
using Apps.Models;
namespace Apps.DAL.JOB
{
    public partial class JOB_TASKJOBSRepository
    {

        public void DeleteJob(string sno)
        {
            Context.P_JOB_DeleteTaskJobs(sno);
        }
        //添加
        public void UpdateState(string sno, int state, string result)
        {
            Context.P_JOB_UpdateState(sno, state, result);
        }
       
        //取对象名称
        public string GetNameById(string sno)
        {
            var entity = Context.JOB_TASKJOBS.SingleOrDefault(a => a.sno == sno);
                return entity == null ? "" : entity.taskName;
        }
      

        public List<P_JOB_GetUnTrackTask_Result> GetUnTrackTask()
        {
            var taskNameList = Context.P_JOB_GetUnTrackTask();
                return taskNameList.ToList();
        }
    }
}
