using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Feed_ListModel
    {
        [Display(Name = "总成物料编码")]
        public string AssemblyPartCode { get; set; }

        [Display(Name = "总成物料名称码")]
        public string AssemblyPartName { get; set; }

        [Display(Name = "物料编码")]
        public string SubAssemblyPartCode { get; set; }

        [Display(Name = "物料名称码")]
        public string SubAssemblyPartName { get; set; }

        [Display(Name = "库房编码")]
        public string InvCode { get; set; }

        [Display(Name = "库房")]
        public string InvName { get; set; }
    }
}

