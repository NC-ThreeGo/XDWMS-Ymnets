using Apps.Models.WMS;
using System;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_Feed_ListRepository
    {
        public string PrintFeedList(string opt, string feedBillNum)
        {
            ObjectParameter releaseBillNum = new ObjectParameter("ReleaseBillNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_PrintFeedList(opt, feedBillNum, releaseBillNum, returnValue);

            if (returnValue.Value == DBNull.Value)
                return (string)releaseBillNum.Value;
            else
                return null;
        }

        public void ConfirmFeedList(string opt, string releaseBillNum)
        {
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_ConfirmFeedList(opt, releaseBillNum, returnValue);
        }
    }
}
