using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Inv_History_DModel
    {
        [Display(Name = "类别名称")]
        public string Inv_History_HName { get; set; }
     }
}

