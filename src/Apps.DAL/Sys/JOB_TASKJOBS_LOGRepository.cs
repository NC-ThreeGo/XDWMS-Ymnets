
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Apps.DAL;
using Apps.Models;
namespace Apps.DAL.JOB
{
    public partial class JOB_TASKJOBS_LOGRepository 
    {


        public int DeleteBySno(string sno)
        {
            var logList = Context.JOB_TASKJOBS_LOG.Where(a => a.sno == sno);
                foreach (var log in logList)
                {
                    Context.JOB_TASKJOBS_LOG.Remove(log);

                }
                return Context.SaveChanges();
        }
       
        //取对象名称
        public string GetNameById(int itemId)
        {
                var entity = Context.JOB_TASKJOBS_LOG.SingleOrDefault(a => a.itemID == itemId);
                return entity == null ? "" : entity.taskName;
        }
       
    }
}
