using Apps.Models.WMS;
using System;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_ReturnInspectionRepository
    {
        public string CreateBatchReturnInspection(string opt, string jsonReturnInspection)
        {
            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            ObjectParameter returnInspectionNum = new ObjectParameter("ReturnInspectionNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_CreateBatchReturnInspection(opt, jsonReturnInspection, returnInspectionNum, returnValue);

            if (returnValue.Value == DBNull.Value)
                return (string)returnInspectionNum.Value;
            else
                return null;
        }

        public string ProcessReturnInspectBill(string opt, string jsonReturnInspection)
        {
            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_ProcessReturnInspectBill(opt, jsonReturnInspection, returnValue);

            return returnValue.Value.ToString();
        }
    }
}
