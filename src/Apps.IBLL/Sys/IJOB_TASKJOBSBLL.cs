using System;
using System.Collections.Generic;
using Apps.Common;
using Apps.Models;
using Apps.Models.JOB;
namespace Apps.IBLL.JOB
{
   public partial interface IJOB_TASKJOBSBLL
    {
        System.Collections.Generic.List<JOB_TASKJOBSModel> GetListByCustom(ref GridPager pager, string querystr);
        JOB_TASKJOBSModel GetModelById(string sno);
        bool UpdateState(ref ValidationErrors errors, string sno, int state, string result);
        List<P_JOB_GetUnTrackTask_Result> GetUnTrackTask();

        bool DeleteJob(ref ValidationErrors errors, string sno);
    }
}
