using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_ReturnInspectionModel
    {
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string PartType { get; set; }
        public string CustomerShortName { get; set; }
        public string SupplierShortName { get; set; }
        public string InvName { get; set; }
        public string SubInvName { get; set; }

        [Display(Name = "物料所属供应商")]
        public string BelongSupplier { get; set; }
    }
}

