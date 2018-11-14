using Apps.Common;
using Apps.Models;
using Apps.Models.JOB;
namespace Apps.IBLL.JOB
{
   public partial interface IJOB_TASKJOBS_LOGBLL
   {
        bool DeleteBySno(ref ValidationErrors errors, string sno);
   
        System.Collections.Generic.List<JOB_TASKJOBS_LOGModel> GetListBySno(ref GridPager pager, string sno);
        JOB_TASKJOBS_LOGModel GetModelById(int itemId);
       
    }
}
