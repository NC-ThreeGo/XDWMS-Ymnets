using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_POModel
    {
        public string PartCode { get; set; }
        public string SupplierShortName { get; set; }
        public decimal? ArrivalQty { get; set; }

        public string MoreAccept { get; set; }
    }
}

