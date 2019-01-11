using Apps.Models.WMS;
using System;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_Inv_AdjustRepository
    {
        public string CreateInvAdjust(string opt, int? partId, int? invId, decimal? adjustQty, string adjustType, string remark, ref string invAdjustBillNum)
        {
            ObjectParameter invAdjustBillNumP = new ObjectParameter("InvAdjustBillNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_InvAdjust(opt, partId, invId, "", adjustQty, adjustType, remark, invAdjustBillNumP, returnValue);

            if (returnValue.Value == DBNull.Value)
            {
                invAdjustBillNum = (string)invAdjustBillNumP.Value;
                return null;
            }
            else
                return (string)returnValue.Value;
        }
    }
}
