using Apps.Models.WMS;
using System;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_ReInspectRepository
    { 
        string CreateReInspect(string userId, int aIID, string nCheckOutResult, decimal? nQualifyQty, decimal? nNoQualifyQty, string nCheckOutRemark, DateTime? nCheckOutDate, string remark);
    }
}
