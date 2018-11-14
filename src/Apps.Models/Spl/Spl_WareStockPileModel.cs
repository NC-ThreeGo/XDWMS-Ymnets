//只有当启用父表时才必要复制
using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Spl
{
    public partial class Spl_WareStockPileModel
    {
        [Display(Name = "类别名称")]
        public string WarehouseName { get; set; }

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

    public class Spl_WreStockPileListModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string WareDetailsId { get; set; }
        public string WarehouseId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime ExecTime{ get; set; }
}

    public class Spl_WareStockReportModel
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
        public int? Quantity { get; set; }
        public decimal? QuantityTotal { get; set; }
        public string Material { get; set; }

        public decimal? TotalPrice { get; set; }


    }
}

