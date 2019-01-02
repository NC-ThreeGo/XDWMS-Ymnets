using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Inv_AdjustModel
    {
        [Display(Name = "物料编码")]
        public string PartCode { get; set; }

        [Display(Name = "库房")]
        public string InvCode { get; set; }
    }
}

