using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Spl
{
    public partial class Spl_WareDetailsModel
    {
        public string WareCategoryName { get; set; }
        [Display(Name = "主键ID")]
        public override string Id { get; set; }

        [Display(Name = "名称")]
        public override string Name { get; set; }

        [Display(Name = "编码")]
        public override string Code { get; set; }

        [Display(Name = "条码")]
        public override string BarCode { get; set; }

        [Display(Name = "存货分类")]
        public override string WareCategoryId { get; set; }

        [Display(Name = "计量单位")]
        public override string Unit { get; set; }

        [Display(Name = "存货标签")]
        public override string Lable { get; set; }

        [Display(Name = "参考进价")]
        public override decimal? BuyPrice { get; set; }

        [Display(Name = "参考售价")]
        public override decimal SalePrice { get; set; }

        [Display(Name = "零售价")]
        public override decimal? RetailPrice { get; set; }

        [Display(Name = "备注")]
        public override string Remark { get; set; }

        [Display(Name = "其他信息-厂家")]
        public override string Vender { get; set; }

        [Display(Name = "品牌")]
        public override string Brand { get; set; }

        [Display(Name = "颜色")]
        public override string Color { get; set; }

        [Display(Name = "材质")]
        public override string Material { get; set; }

        [Display(Name = "尺码")]
        public override string Size { get; set; }

        [Display(Name = "重量")]
        public override string Weight { get; set; }

        [Display(Name = "产地")]
        public override string ComeFrom { get; set; }

        [Display(Name = "最高库存")]
        public override decimal? UpperLimit { get; set; }

        [Display(Name = "最低库存")]
        public override decimal? LowerLimit { get; set; }

        [Display(Name = "参考成本")]
        public override decimal? PrimeCost { get; set; }

        [Display(Name = "一级价格")]
        public override decimal? Price1 { get; set; }

        [Display(Name = "二级价格")]
        public override decimal? Price2 { get; set; }

        [Display(Name = "三级价格")]
        public override decimal? Price3 { get; set; }

        [Display(Name = "四级价格")]
        public override decimal? Price4 { get; set; }

        [Display(Name = "五级价格")]
        public override decimal? Price5 { get; set; }

        [Display(Name = "照片1")]
        public override string Photo1 { get; set; }

        [Display(Name = "照片2")]
        public override string Photo2 { get; set; }

        [Display(Name = "照片3")]
        public override string Photo3 { get; set; }

        [Display(Name = "照片4")]
        public override string Photo4 { get; set; }

        [Display(Name = "照片5")]
        public override string Photo5 { get; set; }

        [Display(Name = "状态")]
        public override bool Enable { get; set; }

        [Display(Name = "创建时间")]
        public override DateTime CreateTime { get; set; }

        [Display(Name = "所属仓库")]
        public string WareHouseName { get; set; }

        [Display(Name = "剩余库存")]
        public int Quantity { get; set; }
    }
}

