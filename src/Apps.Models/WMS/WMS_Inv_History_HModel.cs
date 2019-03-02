using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Inv_History_HModel
    {
        [Display(Name = "物料编码")]
        public string PartCode { get; set; }

        [Display(Name = "物料名称")]
        public string PartName { get; set; }

        [Display(Name = "库房")]
        public string InvCode { get; set; }
        public string InvName { get; set; }

        [Display(Name = "子库房")]
        public string SubInvName { get; set; }
    }
}

