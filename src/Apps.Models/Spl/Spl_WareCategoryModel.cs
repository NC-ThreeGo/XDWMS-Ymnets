using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Spl
{
    public partial class Spl_WareCategoryModel
    {
        public string WareCategoryName { get; set; }

        public string state { get; set; }

        public string Type { get; set; }
        [NotNullExpression]
        public override string Code { get; set; }
    }
}

