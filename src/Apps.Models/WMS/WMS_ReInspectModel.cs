using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_ReInspectModel
    {
        public string InspectBillNum { get; set; }
        public string PO { get; set; }
    }
}

