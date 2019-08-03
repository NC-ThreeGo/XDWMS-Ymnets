using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_AIModel
    {
        [Display(Name = "到货日期")]
        public override DateTime? ArrivalDate { get; set; }

        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string PO { get; set; }
        public string SupplierShortName { get; set; }
        public DateTime? PlanDate { get; set; }
        public string InvName { get; set; }
        public string SubInvName { get; set; }
        public decimal QTY { get; set; }
        public string SupplierName { get; set; }
        public string POStatus { get; set; }
        public decimal? ArrivalQtySum { get; set; }

    }
}

