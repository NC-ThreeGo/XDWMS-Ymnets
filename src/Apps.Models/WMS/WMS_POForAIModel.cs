using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_POForAIModel : WMS_POModel
    {
        [Display(Name = "已到货数量")]
        public decimal? SumAIQty { get; set; }


        [Display(Name = "本次到货数量")]
        public decimal CurrentQty { get; set; }

        [Display(Name = "到货箱数")]
        public decimal BoxNum { get; set; }

        [Display(Name = "到货日期")]
        public DateTime ArrivalDate { get; set; }
    }
}

