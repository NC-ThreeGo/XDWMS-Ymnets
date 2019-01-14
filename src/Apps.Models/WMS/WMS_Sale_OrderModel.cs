using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Sale_OrderModel
    {
        [Display(Name = "物料编码")]
        public string PartCode { get; set; }

        [Display(Name = "物料名称码")]
        public string PartName { get; set; }

        [Display(Name = "库房编码")]
        public string InvCode { get; set; }

        [Display(Name = "库房")]
        public string InvName { get; set; }

        [Display(Name = "客户简称")]
        public virtual string CustomerShortName { get; set; }
        [Display(Name = "客户名称")]
        public virtual string CustomerName { get; set; }
    }
}

