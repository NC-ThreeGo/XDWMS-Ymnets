//全局配置文件。来源于JavascriptServer->Home/ConfigJS
var _YMGlobal;
(function (_YMGlobal) {
    var Config = (function () {
        function Config() {

        }
        Config.currentCulture = "";
        Config.apiUrl = "";
        Config.token = "";
        return Config;
    })();
    _YMGlobal.Config = Config;
})(_YMGlobal || (_YMGlobal = {}));


//要获取的参数名称
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return r[2];
    }
    return null
}
//获取URL参数
//var identity = getUrlParam('identity', '');
function getUrlParam(key, defaultValue) {
    var url = location.href.split("?");
    if (url[1] == null) return defaultValue;
    if (!window.interface) {
        url[1] = unescape(url[1]);
    }
    var field = url[1].split("&");
    for (i = 0; i < field.length; i++) {
        var val = field[i].split("=");
        if (val[0] == key) {
            if (window.interface) {
                return decodeURI(val[1]);
            }
            else {
                return unescape(unescape(val[1]));
            }
        }
    }
    return defaultValue;
}
//===========================字符串辅助================================

//生成唯一的GUID
function GetGuid() {
    var s4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return s4() + s4() + s4() + "-" + s4();
}

//JQGrid行保存后成功返回结果分解对象
function JsonMessage(data) {
    /*
    适用于jQuery('#List').jqGrid('saveRow', id,successfunc(data){});
    */
    var msg = data.responseText.replace(/\"/g, "");
    var msgs = msg.split(',');
    this.type = msgs[0].split(':')[1];
    this.message = msgs[1].split(':')[1];
    this.value = msgs[2].split(':')[1];
    this.obj = msgs[3].split(':')[1];
};

//格式化日期2012-9-10 10:25:14=>2012-9-10
function DateFormat(name) {
    var date = $("#" + name).val();
    var dateArray = date.split(" ");
    $("#" + name).val(dateArray[0].replace("/","-").replace("/", "-"));
}
//去掉最后一个逗号
function RemoveLastChar(str) {
    return str.substring(0, receiver.length - 1);
}
//根据字符将字符串分解成数组
function AnalyzeArr(str) {
    var arr_id = new Array();
    arr_id = str.split(",");
    return arr_id
}
/**
*取得字符的字节长度（汉字占2，字母占1）
*
*/
function strLen(s) {
    var len = 0;
    for (var i = 0; i < s.length; i++) {

        if (!ischinese(s.charAt(i))) {
            len += 1;
        } else {
            len += 2;
        }
    }
    return len;

}
/**
*判断是否中文函数
*/
function ischinese(s) {
    var ret = false;

    for (var i = 0; i < s.length; i++) {
        if (s.charCodeAt(i) >= 256) {
            ret = true;
            break;
        }
    }

    return ret;
}

//===========================系统管理JS函数================================
//Tab控制函数
function customTabs(tabId, tabNum) {
    //设置点击后的切换样式
    $(tabId + " .tab_nav li").removeClass("selected");
    $(tabId + " .tab_nav li").eq(tabNum).addClass("selected");
    //根据参数决定显示内容
    $(tabId + " .tab_con").hide();
    $(tabId + " .tab_con").eq(tabNum).show();
}

//接收人,接收人名称，信息 组需要加_ 
//返回信息ID
function SendMessage(receiver, receiverTitle, mes) {
    var returnResult = showModalDialog("/MIS/WebIM/CeleritySend", [receiver, receiverTitle, mes], "dialogLeft:150px;dialogTop:200px;dialogwidth:726px; dialogheight:206px;");
    return returnResult;
}
//返回选择人员列表
//返回人员ID列表   人员ID:^人员名称" ,格式：a,b,_c^[a],[b],[+c]
//没有选择返回一个""
function SelMemberList() {
    var value = 1;
    var returnResult = showModalDialog("/MIS/WebIM/SelMember", [value], "dialogLeft:150px;dialogTop:200px;dialogwidth:420px; dialogheight:440px;");
    return returnResult;
}
//================上传文件JS函数开始，需和jquery.form.js一起使用===============
//文件上传
function Upload(action, repath, uppath, iswater, isthumbnail,form) {

    var sendUrl = "/Core/upload_ajax.ashx?action=" + action + "&ReFilePath=" + repath + "&UpFilePath=" + uppath;
    //判断是否打水印
    if (arguments.length == 4) {
        sendUrl = "/Core/upload_ajax.ashx?action=" + action + "&ReFilePath=" + repath + "&UpFilePath=" + uppath + "&IsWater=" + iswater;
    }
    //判断是否生成宿略图
    if (arguments.length == 5 || arguments.length == 6) {
        //不是图片
        if (iswater != "" && isthumbnail != "")
        {
            sendUrl = "/Core/upload_ajax.ashx?action=" + action + "&ReFilePath=" + repath + "&UpFilePath=" + uppath + "&IsWater=" + iswater + "&IsThumbnail=" + isthumbnail;
        }
    }
    if (form == undefined || form == "")
    {
        form = "form";
    }
    //开始提交
    $(form).ajaxSubmit({
        beforeSubmit: function (formData, jqForm, options) {
            //隐藏上传按钮
            $("#" + repath).nextAll(".files").eq(0).hide();
            //显示LOADING图片
            $("#" + repath).nextAll(".uploading").eq(0).show();
        },
        success: function (data, textStatus) {
            if (data.msg == 1) {
                
                $("#" + repath).val(data.msgbox.split(",")[0]);
                
                $("#" + repath).next("img").attr("src", data.msgbox.split(",")[0]);
            } else {
                alert(data.msgbox);
            }
            $("#" + repath).nextAll(".files").eq(0).show();
            $("#" + repath).nextAll(".uploading").eq(0).hide();
        },
        error: function (data, status, e) {
            alert("上传失败，错误信息：" + e);
            $("#" + repath).nextAll(".files").eq(0).show();
            $("#" + repath).nextAll(".uploading").eq(0).hide();
        },
        url: sendUrl,
        type: "post",
        dataType: "json",
        timeout: 600000
    });
};
//附件上传
function AttachUpload(repath, uppath) {
    var submitUrl = "/Core/upload_ajax.ashx?action=AttachFile&UpFilePath=" + uppath;
    //开始提交
    $("form").ajaxSubmit({
        beforeSubmit: function (formData, jqForm, options) {
            //隐藏上传按钮
            $("#" + uppath).parent().hide();
            //显示LOADING图片
            $("#" + uppath).parent().nextAll(".uploading").eq(0).show();
        },
        success: function (data, textStatus) {
            if (data.msg == 1) {
                var listBox = $("#" + repath + " ul");
                var newLi = '<li>'
                + '<input name="hidFileName" type="hidden" value="0|' + data.mstitle + "|" + data.msgbox + '" />'
                + '<b class="close" title="删除" onclick="DelAttachLi(this);"></b>'
                + '<span class="right">下载积分：<input name="txtPoint" type="text" class="input2" value="0" onkeydown="return checkNumber(event);" /></span>'
                + '<span class="title">附件：' + data.mstitle + '</span>'
                + '<span>人气：0</span>'
                + '<a href="javascript:;" class="upfile"><input type="file" name="FileUpdate" onchange="AttachUpdate(\'hidFileName\',this);" /></a>'
                + '<span class="uploading">正在更新...</span>'
                + '</li>';
                listBox.append(newLi);
                //alert(data.mstitle);
            } else {
                alert(data.msgbox);
            }
            $("#" + uppath).parent().show();
            $("#" + uppath).parent().nextAll(".uploading").eq(0).hide();
        },
        error: function (data, status, e) {
            alert("上传失败，错误信息：" + e);
            $("#" + uppath).parent().show();
            $("#" + uppath).parent().nextAll(".uploading").eq(0).hide();
        },
        url: submitUrl,
        type: "post",
        dataType: "json",
        timeout: 600000
    });
};
//更新附件上传
function AttachUpdate(repath, uppath) {
    var btnOldName = $(uppath).attr("name");
    var btnNewName = "NewFileUpdate";
    $(uppath).attr("name", btnNewName);
    var submitUrl = "/Core/upload_ajax.ashx?action=AttachFile&UpFilePath=" + btnNewName;
    //开始提交
    $("form").ajaxSubmit({
        beforeSubmit: function (formData, jqForm, options) {
            //隐藏上传按钮
            $(uppath).parent().hide();
            //显示LOADING图片
            $(uppath).parent().nextAll(".uploading").eq(0).show();
        },
        success: function (data, textStatus) {
            if (data.msg == 1) {
                var ArrFileName = $(uppath).parent().prevAll("input[name='" + repath + "']").val().split("|");
                $(uppath).parent().prevAll("input[name='" + repath + "']").val(ArrFileName[0] + "|" + data.mstitle + "|" + data.msgbox);
                $(uppath).parent().prevAll(".title").html("附件：" + data.mstitle);
            } else {
                alert(data.msgbox);
            }
            $(uppath).parent().show();
            $(uppath).parent().nextAll(".uploading").eq(0).hide();
            $(uppath).attr("name", btnOldName);
        },
        error: function (data, status, e) {
            alert("上传失败，错误信息：" + e);
            $(uppath).parent().show();
            $(uppath).parent().nextAll(".uploading").eq(0).hide();
            $(uppath).attr("name", btnOldName);
        },
        url: submitUrl,
        type: "post",
        dataType: "json",
        timeout: 600000
    });
};
//===========================上传文件JS函数结束================================


//===========================计算辅助================================
//保留2位小数 3.14159 =3.14
function changeTwoDecimal(x) {
    if (x == "Infinity") {
        return;
    }
    var f_x = parseFloat(x);
    if (isNaN(f_x)) {
        return
    } else {
        var f_x = Math.round(x * 100) / 100;
        var s_x = f_x.toString();
        var pos_decimal = s_x.indexOf('.');
        if (pos_decimal < 0) {
            pos_decimal = s_x.length;
            s_x += '.';
        }
        while (s_x.length <= pos_decimal + 2) {
            s_x += '0';
        }
        return s_x;
    }
}
function isDate_yyyyMMdd(str) {
    var reg = /^([0-9]{1,4})(-|\/)([0-9]{1,2})\2([0-9]{1,2})$/;
    var r = str.match(reg);
    if (r == null) return false;
    var d = new Date(r[1], r[3] - 1, r[4]);
    var newstr = d.getFullYear() + r[2] + (d.getMonth() + 1) + r[2] + d.getDate();
    var yyyy = parseInt(r[1], 10);
    var mm = parseInt(r[3], 10);
    var dd = parseInt(r[4], 10);
    var compstr = yyyy + r[2] + mm + r[2] + dd;
    return newstr == compstr;
}
//===========================上传文件JS函数结束================================
//是否存在指定函数 
function isExitsFunction(funcName) {
    try {
        if (typeof (eval(funcName)) == "function") {
            return true;
        }
    } catch (e) { }
    return false;
}
//是否存在指定变量 
function isExitsVariable(variableName) {
    try {
        if (typeof (variableName) == "undefined") {
            //alert("value is undefined"); 
            return false;
        } else {
            //alert("value is true"); 
            return true;
        }
    } catch (e) { }
    return false;
}

//loading
function showLoading() {
    $("#over").show();
    $("#layout").show();

}
function hideLoading() {
    $("#over").hide();
    $("#layout").hide();
}



// 获取日期信息
function getDateInfo(_date) {
    if (_date) {
        var date = new Date(_date);
    } else {
        var date = new Date();
    }

    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hour = date.getHours();
    var minute = date.getMinutes();
    var second = date.getSeconds();
    var dayNum = new Date(year, month, 0).getDate();
    var dayNum2 = new Date(year, 12, 0).getDate();
    var dayNum3 = new Date(year + 1, 2, 0).getDate();

    var week = date.getDay();
    switch (week) {
        case 1: week = "星期一"; break;
        case 2: week = "星期二"; break;
        case 3: week = "星期三"; break;
        case 4: week = "星期四"; break;
        case 5: week = "星期五"; break;
        case 6: week = "星期六"; break;
        default: week = "星期天";
    }

    var quarter = '第一季度';
    switch (month) {
        case 1: case 2: case 3: quarter = "第一季度"; dateQuarterStart = year + '-01-01'; dateQuarterEnd = year + '-03-' + new Date(year, 3, 0).getDate(); break;
        case 4: case 5: case 6: quarter = "第二季度"; dateQuarterStart = year + '-04-01'; dateQuarterEnd = year + '-06-' + new Date(year, 6, 0).getDate(); break;
        case 7: case 8: case 9: quarter = "第三季度"; dateQuarterStart = year + '-07-01'; dateQuarterEnd = year + '-09-' + new Date(year, 9, 0).getDate(); break;
        case 10: case 11: case 12: quarter = "第四季度"; dateQuarterStart = year + '-10-01'; dateQuarterEnd = year + '-12-' + new Date(year, 12, 0).getDate(); break;
        default: quarter = "第一季度";
    }

    var getYearWeek = function (a, b, c) {
        var d1 = new Date(a, b - 1, c), d2 = new Date(a, 0, 1),
        d = Math.round((d1 - d2) / 86400000);
        return Math.ceil((d + ((d2.getDay() + 1) - 1)) / 7);
    }

    month = (month < 10 ? '0' + month : month);
    day = (day < 10 ? '0' + day : day);
    hour = (hour < 10 ? '0' + hour : hour);
    minute = (minute < 10 ? '0' + minute : minute);
    second = (second < 10 ? '0' + second : second);
    dayNum = (dayNum < 10 ? '0' + dayNum : dayNum);

    return {
        year: year, // 当前年份
        month: month, // 当前月份
        day: day, // 当前日
        hour: hour, // 当前小时
        minute: minute, // 当前分钟
        second: second, // 当前秒数
        week: week, // 当前星期数
        quarter: quarter, // 当前季度
        weekNum: getYearWeek(year, month, day), // 当前周数
        dayNum: dayNum, // 当月天数

        // 当前日期
        dateNow: year + '-' + month + '-' + day,

        // 月度区间
        dateMonthStart: year + '-' + month + '-01',
        dateMonthEnd: year + '-' + month + '-' + dayNum,

        // 年度区间
        dateYearStart: year + '-01-01',
        dateYearEnd: year + '-12-' + dayNum2,

        // 年度区间
        dateYearStart2: year + '-03-01',
        dateYearEnd2: (year + 1) + '-02-' + dayNum3,

        // 季度区间
        dateQuarterStart: dateQuarterStart,
        dateQuarterEnd: dateQuarterEnd
    }
}