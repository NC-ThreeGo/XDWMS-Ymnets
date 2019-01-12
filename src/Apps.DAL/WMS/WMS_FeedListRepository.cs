using Apps.Models.WMS;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_Feed_ListRepository
    {
        public string PrintFeedList(string opt, string feedBillNum, ref string releaseBillNum)
        {
            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            ObjectParameter paramrReleaseBillNum = new ObjectParameter("ReleaseBillNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_PrintFeedList(opt, feedBillNum, paramrReleaseBillNum, returnValue);

            if (returnValue.Value == DBNull.Value)
            {
                releaseBillNum = (string)paramrReleaseBillNum.Value;
                return null;
            }
            else
                return (string)returnValue.Value;
        }

        public void ConfirmFeedList(string opt, string releaseBillNum)
        {
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));

            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
            Context.P_WMS_ConfirmFeedList(opt, releaseBillNum, returnValue);
        }
    }
}
