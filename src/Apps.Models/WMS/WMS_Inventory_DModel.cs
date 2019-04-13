using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Inventory_DModel
    {
        [Display(Name = "类别名称")]
        public string Inventory_HName { get; set; }

        [Display(Name = "物料编码")]
        public string PartCode { get; set; }

        [Display(Name = "物料名称")]
        public string PartName { get; set; }

        [Display(Name = "保管员")]
        public string StoreMan { get; set; }

        [Display(Name = "库房")]
        public string InvCode { get; set; }
        public string InvName { get; set; }

        [Display(Name = "子库房")]
        public string SubInvName { get; set; }

        [Display(Name = "盘点类型")]
        public string InventoryType { get; set; }

        [Display(Name = "盘点状态")]
        public string InventoryStatus { get; set; }

    }
}

