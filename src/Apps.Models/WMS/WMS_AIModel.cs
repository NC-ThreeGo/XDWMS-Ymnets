using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_AIModel
    {
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string PO { get; set; }
        public string SupplierShortName { get; set; }
        public DateTime? PlanDate { get; set; }
        public string InvName { get; set; }
        public string SubInvName { get; set; }
    }
}

