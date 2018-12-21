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
            ObjectParameter returnOrderNum = new ObjectParameter("ReturnOrderNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_CreateBatchReturnOrder(opt, jsonReturnOrder, returnOrderNum, returnValue);

            if (returnValue.Value == DBNull.Value)
                return (string)returnOrderNum.Value;
            else
                return null;
        }
    }
}
