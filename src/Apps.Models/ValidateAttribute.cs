using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;
using Apps.Locale;

namespace Apps.Models
{
    /** 使用说明
        * 继承 ValidationAttribute 抽象类，重写 IsValid() 方法，以实现服务端验证
        * 实现 IClientValidatable 接口的 GetClientValidationRules() 方法，以实现客户端验证
        * 
        * 1.   [IntRangeExpression(18, 30)]           数字在18与30之间,可以不填写，但填写就进入验证
        * 2.   [MaxWordsExpression(50)]               字符数在不能大50，可以不填写，但填写就进入验证
        * 3.   [NotNullExpression]                    验证是否为空且 调用
        * 4.   [DateExpression]                       是否是日期格式：2012-10-1
        * 5.   [IsCharExpression]                     只能是数字，字母，下划线，中划线组成，可以不填写
        * 6.   [ChinaCharExpression]                  只能输入汉字，可以不填写
        * 7.   [NotEqualExpression("abcd")]           不能等于指定的值，可以不填写：如不能等于abcd
        * 8.   [ContainExpression("abc")]             验证是否包含指定字符串，可以不填写：如必须包含abc
        * 9.   [NotContainExpression("abc")]          验证不能指定字符串,可以不填写,如不能含有abc
        *10.   [IsIntegerEpression]                   验证是否是数字格式 可以不填写,可以为任意整数 1,-5
        *11.   [IsPositiveIntegerExpression]          验证是否是数字格式，可以不填写,必须是任意正整数 25
        *12.   [IsNegativeIntegerExpression]          验证是否是数字格式，可以不填写,必须是任意负整数 -25
        *13.   [IsDecimalExpression]                  验证是否是数字格式 可以不填写,可以为任意浮点数 12.12,12,-12.12
        *14.   [IsPositiveDecimalExpression]          验证是否是数字格式 可以不填写,可以为任意正浮点数 1.4
        *15.   [IsNegativeDecimalExpression]          验证是否是数字格式 可以不填写,可以为任意负浮点数 -1.2
        *16.   [EmailExpression]                      验证是否是Email 
        *17.   [PhoneExpression]                      验证是否是中国电话号码 如：0769-222222-222 正确格式为："XXX-XXXXXXX"、"XXXX- XXXXXXXX"、"XXX-XXXXXXX"、"XXX-XXXXXXXX"、"XXXXXXX"和"XXXXXXXX"。
        *18.   [SiteExpression]                       验证是否是一个完整的网址 如：www.163.com
        *19.   [IsNumberExpression]                   验证是否是数字格式 可以不填写,可以为任意数字
        * 
        * 
        * 组合使用演示
        * [DisplayName("姓名")]                       名称
        * [NotNullExpression]                         不能为空
        * [MaxWordsExpression(50)]                    最多50个字符，25个汉字
        * [IsCharExpression]                          只能由数字，字母，中划线，下划线组成（一般用于验证ID）
        * [NotEqualExpression("admin")]               不能包含有admin字符串
        * public string Id { get; set; }
        * 
        * 数字判断演示
        *  [IsNumberExpression]  可以不填写，填写判断是否是数字
        *  [DisplayName("代号")]
        *  public int? age { get; set; }
        * 非空字断使用 
        * //[Required(ErrorMessageResourceType = typeof(ErrorRs), ErrorMessageResourceName = "IsNumberExpression")]
        *  或//[Required(ErrorMessage="必须填写这个字段")] 来覆盖本地化 如public int age; int?为可空字端
        **/
    /// <summary>
    ///  [IntRangeExpression(18, 30)] 数字在18与30之间,可以不填写，但填写就进入验证
    /// </summary>
    public class IntRangeExpressionAttribute : ValidationAttribute, IClientValidatable
    {
        long minMum { get; set; }
        long maxMum { get; set; }
        public IntRangeExpressionAttribute(long minimum, long maximum)
        {
            minMum = minimum;
            maxMum = maximum;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            long intValue = Convert.ToInt64(value);
            return intValue >= minMum && intValue <= maxMum;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                 Resource.IntRangeExpression, name, minMum, maxMum);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "isinteger",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            // 向客户端验证代码传递参数
            validationRule.ValidationParameters.Add("param1", minMum);
            validationRule.ValidationParameters.Add("param2", maxMum);
            yield return validationRule;
        }
    }
    /// <summary>
    ///   [MaxWordsExpression(50)]字符数在不能大50，可以不填写，但填写就进入验证
    /// </summary>
    public class MaxWordsExpressionAttribute : ValidationAttribute, IClientValidatable
    {

        int maxStr { get; set; }
        public MaxWordsExpressionAttribute(int maximum)
        {
            maxStr = maximum;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            string valueAsString = value.ToString();

            return (Encoding.Default.GetByteCount(valueAsString) <= maxStr);

        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                 Resource.MaxWordsExpression, name, maxStr / 2, maxStr);

        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "maxwords",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            validationRule.ValidationParameters.Add("param", maxStr);
            yield return validationRule;
        }
    }

    /// <summary>
    ///   [MinWordsExpression(50)]字符数在不能少于50个字符，可以不填写，但填写就进入验证
    /// </summary>
    public class MinWordsExpressionAttribute : ValidationAttribute, IClientValidatable
    {

        int minStr { get; set; }
        public MinWordsExpressionAttribute(int minimum)
        {
            minStr = minimum;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            string valueAsString = value.ToString();

            return (Encoding.Default.GetByteCount(valueAsString) >= minStr);

        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                 Resource.MinWordsExpression, name, minStr);

        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "minwords",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            validationRule.ValidationParameters.Add("param", minStr);
            yield return validationRule;
        }
    }
    /// <summary>
    /// [NotNullExpression] 验证是否为空且不能有空格 调用
    /// </summary>
    public class NotNullExpressionAttribute : ValidationAttribute, IClientValidatable
    {
        static string DispalyName = "";
        public NotNullExpressionAttribute()
            : base(Resource.NotNullExpression)
        {
        }
        public override string FormatErrorMessage(string name)
        {
            return String.Format(name, DispalyName);
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            string valueAsString = value.ToString();
            bool result = !string.IsNullOrEmpty(valueAsString);
            return result;

        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            DispalyName = metadata.DisplayName;
            return new[]{
                new ModelClientValidationRequiredRule(FormatErrorMessage(Resource.NotNullExpression))
            };
        }
    }
    /// <summary>
    ///   [DateExpression] 是否是日期格式：2012-10-1
    /// </summary>
    public class DateExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)$))";
        public DateExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            string valueAsString = value.ToString();
            string dtvalue;
            if (value.ToString().IndexOf(" ") > 0)
            {
                dtvalue = valueAsString.Replace("/", "-").Substring(0, valueAsString.IndexOf(" "));
            }
            else
            {
                dtvalue = valueAsString.Replace("/", "-");
            }
            Match mch = reg.Match(dtvalue);
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是日期格式：2012-10-10", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    ///  [IsCharExpression] 只能是数字，字母，下划线，中划线组成，可以不填写
    /// </summary>
    public class IsCharExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^[0-9A-Za-z_-]{0,}$";
        public IsCharExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
            Resource.IsCharExpression, name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    ///  [ChinaCharExpression] 只能输入汉字，可以不填写
    /// </summary>
    public class ChinaCharExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^[\u4e00-\u9fa5]{0,}$";
        public ChinaCharExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}只能填写汉字", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    ///   [NotEqualExpression("abcd")]，不能等于指定的值，可以不填写：如不能等于abcd
    /// </summary>
    public class NotEqualExpressionAttribute : ValidationAttribute, IClientValidatable
    {
        string InputString { get; set; }
        public NotEqualExpressionAttribute(string inputString)
        {
            InputString = inputString;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            string valueAsString = value.ToString();

            return (valueAsString != InputString);

        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                 "{0}不能等同于{1}", name, InputString);

        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "notequal",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            validationRule.ValidationParameters.Add("param", InputString);
            yield return validationRule;
        }
    }
    /// <summary>
    /// [ContainExpression("abc")]  验证是否包含指定字符串，可以不填写：如必须包含abc
    /// </summary>
    public class ContainExpressionAttribute : ValidationAttribute, IClientValidatable
    {
        string InputString { get; set; }
        public ContainExpressionAttribute(string inputString)
        {
            InputString = inputString;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            string valueAsString = value.ToString();
            return (valueAsString.Contains(InputString));
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                "{0}必须不能包含字符串：{1}", name, InputString);

        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "contain",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            validationRule.ValidationParameters.Add("param", InputString);
            yield return validationRule;
        }
    }
    /// <summary>
    ///  [NotContainExpression("abc")]，验证不能指定字符串,可以不填写,如不能含有abc
    /// </summary>
    public class NotContainExpressionAttribute : ValidationAttribute, IClientValidatable
    {
        string InputString { get; set; }
        public NotContainExpressionAttribute(string inputString)
        {
            InputString = inputString;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            string valueAsString = value.ToString();
            return (!valueAsString.Contains(InputString));
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                 "{0}必须不能包含字符串：{1}", name, InputString);

        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "notcontain",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            validationRule.ValidationParameters.Add("param", InputString);
            yield return validationRule;
        }
    }
    /// <summary>
    /// [IsNumberExpression] 验证是否是数字格式，可以不填写,必须是任意数字
    /// </summary>
    public class IsNumberExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^[-]?\d+(\.\d+)?$";
        public IsNumberExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            string input = value.ToString();
            Match mch = reg.Match(input);
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是一个数字", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    /// [IsIntegerExpression] 验证是否是数字格式，可以不填写,必须是任意整数 -1，1
    /// </summary>
    public class IsIntegerExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^-?\d+$";
        public IsIntegerExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是一个整数", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }


    /// <summary>
    /// [IsPositiveIntegerExpression] 验证是否是数字格式，可以不填写,必须是任意正整数 25
    /// </summary>
    public class IsPositiveNumberExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^\d+$";
        public IsPositiveNumberExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是一个正整数", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    /// [IsNegativeIntegerExpression] 验证是否是数字格式，可以不填写,必须是任意负整数 -25
    /// </summary>
    public class IsNegativeNumberExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^-?\d+$";
        public IsNegativeNumberExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是一个负整数", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    /// [IsDecimalExpression] 验证是否是数字格式 可以不填写,可以为任意浮点数 12.12,12,-12.12
    /// </summary>
    public class IsDecimalExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^[-]?\d+(\.\d+)?$";
        public IsDecimalExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是一个浮点数", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    /// [IsPositiveDecimalExpression] 验证是否是数字格式 可以不填写,可以为任意正浮点数 1.4
    /// </summary>
    public class IsPositiveDecimalExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^\d+(\.\d+)?$";
        public IsPositiveDecimalExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是一个浮正点数", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    /// [IsNegativeDecimalExpression] 验证是否是数字格式 可以不填写,可以为任意负浮点数 -1.2
    /// </summary>
    public class IsNegativeDecimalExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^-\d+(\.\d+)?$";
        public IsNegativeDecimalExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是一个浮负点数", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    ///  [EmailExpression] 验证是否是Email 
    /// </summary>
    public class EmailExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$";
        public EmailExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是邮箱地址", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    /// [PhoneExpression] 验证是否是中国电话号码 如：0769-222222-222 正确格式为："XXX-XXXXXXX"、"XXXX- XXXXXXXX"、"XXX-XXXXXXX"、"XXX-XXXXXXXX"、"XXXXXXX"和"XXXXXXXX"。
    /// </summary>
    public class PhoneExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"^((0\d{2,5}-)|\(0\d{2,5}\))?\d{7,8}(-\d{3,4})?$";
        public PhoneExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}不是正确的电话号码,如：0769-2222222", name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }
    /// <summary>
    ///  验证是否是网址 调用[SiteExpression]
    /// </summary>
    public class SiteExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        static string regStr = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        public SiteExpressionAttribute()
            : base(regStr)
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            Regex reg = new Regex(regStr);
            Match mch = reg.Match(value.ToString());
            if (mch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              "{0}必须是网址,如：http://www.google.com", name);
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var name = metadata.GetDisplayName();
            var rule = new ModelClientValidationRegexRule(FormatErrorMessage(name), Pattern);
            yield return rule;
        }
    }


}

/* 双向验证，请加入以下代码
  
 

// [IntRangeExpression(18, 30)] 数字在18与30之间,可以不填写，但填写就进入验证
jQuery.validator.addMethod('isinteger', function (value, element, params) {
    if (value >= parseInt(params['param1']) && value <= parseInt(params['param2']))
        return true;
    return false;
});
jQuery.validator.unobtrusive.adapters.add('isinteger', ['param1', 'param2'],
            function (options) {
                options.rules['isinteger'] = { param1: options.params.param1, param2: options.params.param2 };
                options.messages['isinteger'] = options.message;
            }
        );
// [MaxWordsExpression(50)]字符数在不能大50，可以不填写，但填写就进入验证
jQuery.validator.addMethod('checkMaxWords', function (value, element, param) {
    if (strLen(value) > param)
        return false;
    return true;
});
jQuery.validator.unobtrusive.adapters.add('maxwords', ['param'],
            function (options) {
                options.rules['checkMaxWords'] = { param: options.params.param };
                options.messages['checkMaxWords'] = options.message;
            }
         );
// [MinWordsExpression(50)]字符数在不能shaoyu 50，可以不填写，但填写就进入验证
jQuery.validator.addMethod('checkMinWords', function (value, element, param) {
    if (strLen(value) < param)
        return false;
    return true;
});
jQuery.validator.unobtrusive.adapters.add('minwords', ['param'],
            function (options) {
                options.rules['checkMinWords'] = { param: options.params.param };
                options.messages['checkMinWords'] = options.message;
            }
         );
// [NotEqualExpression("abcd")]，不能等于指定的值，可以不填写：如不能等于abcd
jQuery.validator.addMethod('checkNotEqual', function (value, element, param) {
    if (value == param)
        return false;
    return true;
});
jQuery.validator.unobtrusive.adapters.add('notequal', ['param'],
            function (options) {
                options.rules['checkNotEqual'] = { param: options.params.param };
                options.messages['checkNotEqual'] = options.message;
            }
         );
// [ContainExpression("abc")]  验证是否包含指定字符串，可以不填写：如必须包含abc
jQuery.validator.addMethod('checkContain', function (value, element, param) {
    if (value.indexOf(param) == -1)
        return false;
    return true;
});
jQuery.validator.unobtrusive.adapters.add('contain', ['param'],
            function (options) {
                options.rules['checkContain'] = { param: options.params.param };
                options.messages['checkContain'] = options.message;
            }
         );

// [NotContainExpression("abc")]，验证不能指定字符串,可以不填写,如不能含有abc
var v_checknotcontainReturn = "";
jQuery.validator.addMethod('checkNotContain', function (value, element, param) {
    if (value.indexOf(param) != -1)
        return false;
    return true;
});
jQuery.validator.unobtrusive.adapters.add('notcontain', ['param'],
            function (options) {
                options.rules['checkNotContain'] = { param: options.params.param };
                options.messages['checkNotContain'] = options.message;
            }
         );
 */