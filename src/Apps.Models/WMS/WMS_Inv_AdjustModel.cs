using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Inv_AdjustModel
    {
        [Display(Name = "物料编码")]
        public string PartCode { get; set; }

        [Display(Name = "物料名称")]
        public string PartName { get; set; }

        [Display(Name = "库房代码")]
        public string InvCode { get; set; }

        [Display(Name = "库房名称")]
        public string InvName { get; set; }
    }
}

