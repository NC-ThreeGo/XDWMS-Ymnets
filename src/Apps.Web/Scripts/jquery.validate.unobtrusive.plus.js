//双向验证，请加入以下代码
  
 

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
 