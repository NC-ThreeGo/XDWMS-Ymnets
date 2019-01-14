using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Inventory_DModel
    {
        [Display(Name = "类别名称")]
        public string Inventory_HName { get; set; }
     }
}

