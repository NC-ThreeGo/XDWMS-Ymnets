using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Spl
{
    public partial class Spl_WarehouseAllocationModel
    {
        [Display(Name = "单号")]
        public override string Id { get; set; }

        [Display(Name = "调拨时间")]
        public override DateTime InTime { get; set; }

        [Display(Name = "经手人")]
        public override string Handler { get; set; }
         [Display(Name = "经手人")]
        public string HandlerName { get; set; }
        [Display(Name = "描述")]
        public override string Remark { get; set; }

        [Display(Name = "总价")]
        public override decimal PriceTotal { get; set; }
        //1 正常 2 审核中 3 审核完成 0作废
        [Display(Name = "状态")]
        public override int State { get; set; }

        [Display(Name = "审核人")]
        [NotNullExpression]
        public override string Checker { get; set; }
        public string CheckerName { get; set; }

        [Display(Name = "审核时间")]
        public override DateTime? CheckTime { get; set; }

        [Display(Name = "创建时间")]
        public override DateTime CreateTime { get; set; }

        [Display(Name = "制单人")]
        public override string CreatePerson { get; set; }

        [Display(Name = "制单人")]
        public string CreatePersonName { get; set; }

        [Display(Name = "修改时间")]
        public override DateTime? ModifyTime { get; set; }

        [Display(Name = "修改人")]
        public override string ModifyPerson { get; set; }

        [Display(Name = "单据确认")]
        public override bool Confirmation { get; set; }


        [Display(Name = "从仓库")]
        public override string FromWarehouseId { get; set; }

        [Display(Name = "到仓库")]
        public override string ToWarehouseId { get; set; }

        [Display(Name = "从仓库")]
        public string FromWarehouseName { get; set; }
        [Display(Name = "到仓库")]
        public string ToWarehouseName { get; set; }
    }


    public class Spl_WareAllocationReportModel
    {
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public string Unit { get; set; }
        public string Vender { get; set; }
        public string WareCategoryId { get; set; }
        public decimal? SalePrice { get; set; }
        public int? nowQuantity { get; set; }
        public decimal? LowerLimit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuantityTotal { get; set; }
        public string Material { get; set; }

        public decimal? TotalPrice { get; set; }


    }
}
