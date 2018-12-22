using Apps.Models.WMS;
using System;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_ReturnOrderRepository
    {
        //public string CreateBatchReturnOrder(string opt, string jsonReturnOrder)
        //{
        //    ObjectParameter returnOrderNum = new ObjectParameter("ReturnOrderNum", typeof(string));
        //    ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
        //    Context.P_WMS_CreateBatchReturnOrder(opt, jsonReturnOrder, returnOrderNum, returnValue);

        //    if (returnValue.Value == DBNull.Value)
        //        return (string)returnOrderNum.Value;
        //    else
        //        return null;
        //}

        public string CreateReturnOrder(string opt, int? partId, int? supplierId, int? invId, decimal? qty, string remark)
        {
            //ObjectParameter paramUserId = new ObjectParameter("UserId", opt);
            //ObjectParameter paramPartId = new ObjectParameter("PartId", partId);
            //ObjectParameter paramSupplierId = new ObjectParameter("SupplierId", supplierId);
            //ObjectParameter paramInvId = new ObjectParameter("InvId", invId);
            //ObjectParameter paramQty = new ObjectParameter("Qty", qty);
            //ObjectParameter paramRemark = new ObjectParameter("Remark", remark);
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_CreateReturnOrder(opt, partId, supplierId, invId, qty, remark, returnValue);

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
    }
}
