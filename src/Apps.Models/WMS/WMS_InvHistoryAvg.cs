using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.WMS
{
    public partial class WMS_InvHistoryAvg
    {
        public Nullable<long> Id { get; set; }
        public int PartId { get; set; }
        public Nullable<int> InvId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string InvCode { get; set; }
        public string InvName { get; set; }
        public Nullable<decimal> AvgQty { get; set; }
        public Nullable<decimal> InvQty { get; set; }
        public Nullable<decimal> BalanceQty { get; set; }
        public string StoreMan { get; set; }
    }
}
