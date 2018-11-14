//只有当启用父表时才必要复制
using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Spl
{
    public partial class Spl_WareCheckTotalModel
    {
        [Display(Name = "类别名称")]
        public string WarehouseName { get; set; }
        [Display(Name = "审核人")]
        //[NotNullExpression]
        //public override string Checker { get; set; }//修改2018年3月13
        public string CheckerName { get; set; }
        [Display(Name = "创建人")]
        public string CreaterName { get; set; }
        [Display(Name = "商品")]
        public string WareDetailsName { get; set; }
        [Display(Name = "商品编号")]
        public string WareDetailsCode { get; set; }
        public string WareDetailsUnit { get; set; }
        [Display(Name = "所属类别")]
        public string WareDetailsCategory { get; set; }
        [Display(Name = "厂家")]
        public string WareDetailsVender { get; set; }
        [Display(Name = "品牌")]

        public string WareDetailsBrand { get; set; }
        public string WareDetailsSize { get; set; }
        public string oper { get; set; }


    }
}

