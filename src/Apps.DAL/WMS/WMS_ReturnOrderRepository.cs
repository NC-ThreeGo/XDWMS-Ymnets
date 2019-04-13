using Apps.Models.WMS;
using System;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_ReturnOrderRepository
    {
        public string CreateBatchReturnOrder(string opt, string jsonReturnOrder)
        {
            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_CreateBatchReturnOrder(opt, jsonReturnOrder, returnValue);
            return returnValue.Value.ToString();
        }

        public string CreateReturnOrder(string opt, int? partId, int? supplierId, int? invId, string lot, decimal? qty, string remark)
        {
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_CreateReturnOrder(opt, partId, supplierId, invId, lot, qty, remark, returnValue);

            if (returnValue.Value == DBNull.Value)
                return null;
            else
                return (string)returnValue.Value;
        }

        public string PrintReturnOrder(string opt, string jsonReturnOrder)
        {
            ObjectParameter returnOrderNum = new ObjectParameter("ReturnOrderNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_PrintReturnOrder(opt, jsonReturnOrder, returnOrderNum, returnValue);

            if (returnValue.Value == DBNull.Value)
                return (string)returnOrderNum.Value;
            else
                return null;
        }

        public void ConfirmReturnOrder(string opt, string returnOrderNum)
        {
            //string sql = "update WMS_ReturnOrder set ConfirmStatus = '已确认', ConfirmMan = '" + opt + "', ConfirmDate = getdate(), "
            //    + " ModifyPerson = '" + opt + "', ModifyTime = getdate() "
            //    + " where ReturnOrderNum = '" + returnOrderNum + "'";
            //Context.Database.ExecuteSqlCommand(sql);
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_ConfirmReturnOrder(opt, returnOrderNum, returnValue);
        }
    }
}
