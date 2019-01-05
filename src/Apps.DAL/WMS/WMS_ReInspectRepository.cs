using Apps.Models.WMS;
using System;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_ReInspectRepository
    {
        public string CreateReInspect(string userId, int aIID, string nCheckOutResult, decimal? nQualifyQty, decimal? nNoQualifyQty, string nCheckOutRemark, DateTime? nCheckOutDate, string remark)
        {
            //ObjectParameter invAdjustBillNumP = new ObjectParameter("InvAdjustBillNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_CreateReInspect(userId, aIID, nCheckOutResult, nQualifyQty, nNoQualifyQty, nCheckOutRemark, nCheckOutDate, remark, returnValue);

            if (returnValue.Value == DBNull.Value)
            {
                //invAdjustBillNum = (string)invAdjustBillNumP.Value;
                return null;
            }
            else
                return (string)returnValue.Value;
        }
    }
}
