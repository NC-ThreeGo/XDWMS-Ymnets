using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_InvModel
    {
        [Display(Name = "可用数量")]
        public decimal AvailableQty { get; set; }

        [Display(Name = "物料编码")]
        public string PartCode { get; set; }

        [Display(Name = "物料名称")]
        public string PartName { get; set; }

        [Display(Name = "库房")]
        public string InvCode { get; set; }
        public string InvName { get; set; }

        [Display(Name = "子库房")]
        public string SubInvName { get; set; }

        [Display(Name = "批次号")]
        public string LotDisp { get; set; }
        

        [Display(Name = "安全库存")]
        public decimal SafeStock { get; set; }

        [Display(Name = "保管员")]
        public string StoreMan { get; set; }

        [Display(Name = "预扣减数量")]
        public decimal PreDeductionQty { get; set; }
    }
}

