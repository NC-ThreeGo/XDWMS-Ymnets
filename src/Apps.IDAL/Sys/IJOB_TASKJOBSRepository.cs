using System;
using System.Collections.Generic;
using Apps.Models;
using System.Linq;
namespace Apps.IDAL.JOB
{
    public partial interface IJOB_TASKJOBSRepository
    {

        void DeleteJob(string sno);
        string GetNameById(string sno);
        void UpdateState(string sno, int state,string result);
        List<P_JOB_GetUnTrackTask_Result> GetUnTrackTask();
    }
}
