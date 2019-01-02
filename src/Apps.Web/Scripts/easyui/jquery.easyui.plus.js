//zip:Update 2018/12/17
//返回一个只包括日期格式的日期（不包含时分秒）
formatterDateNoHour = function (date)
{
    if (date)
    {
        return date.substring(0, 10);
        //var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
        //var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : "0"
        //    + (date.getMonth() + 1);
        //return date.getFullYear() + '-' + month + '-' + day;
    }
    else
    {
        return date;
    }
};


//删除
function dataDelete(url, datagrid) {
    var row = $('#' + datagrid).datagrid('getSelected');
    if (row != null) {
        $.messager.confirm(index_lang_tip, CommonLang.YouWantToDeleteTheSelectedRecords, function (r) {
            if (r) {
                $.post(url, row, function (data) {
                    if (data.type == 1)
                        $('#' + datagrid).datagrid('load');
                    $.messageBox5s(Lang.Tip, data.message);
                }, "json");

            }
        });
    } else { $.messageBox5s(Lang.Tip, CommonLang.PleaseSelectTheOperatingRecord); }
}
//批量操作
function dataBatchOperater(url, datagrid) {
    var rows = $('#' + datagrid).datagrid('getChecked');
    var ids = "";
    if (rows.length > 0) {
        $.each(rows, function (index, row) {
            ids = ids + row.Id + ",";
        });
        ids = ids.substring(0, ids.length - 1);
        $.post(url + "?id=" + ids, function (data) {
            if (data.type == 1) {
                $('#' + datagrid).datagrid('load');
                $('#' + datagrid).datagrid('clearChecked');
            }
            $.messageBox5s(Lang.Tip, data.message);

        }, "json");
    } else { $.messageBox5s(Lang.Tip, CommonLang.PleaseSelectTheOperatingRecord); }
}

//封装所有弹出窗口
$.extend({
    modalWindow: function (title, src, width, height, icon, winname) {
        if (winname == null || winname == "") {
            winname = "modalwindow";
        }
        $("#" + winname).html("<iframe width='100%' height='100%' scrolling='auto' frameborder='0'' src='" + src + "'></iframe>");
        $("#" + winname).window({ title: title, width: width, height: height, iconCls: icon }).window('open');
    }
});

//list为datagrid控件的id，module为模块名称，可以成为数据库的标示

//获取自定义的datagrid设置结果
$.extend({
    getDatagridHeader: function (list, module) {
        showLoading();
        $.ajax({
            url: "SysDataGridConfig/GetList",
            type: "Post",
            data: {  moduleName: module },
            dataType: "json",
            success: function (data) {

                if (data != "")
                {
                    //赋值隐藏或者修改
                    var columns = [];
                    for (var j = 0; j < data.length; j++) {
                        var bcolumn = $('#' + list).datagrid('getColumnOption', data[j].Field);
                        bcolumn.title = data[j].Title;
                        bcolumn.hidden = data[j].Hidden;
                        columns.push(bcolumn);
                    }

                    $('#' + list).datagrid({ columns: [columns] });
                }

                hideLoading();
            }
        });
    }
});
$.extend({
   editDatagridHeader : function (list,module) {

       var str = '<div class="mvctool bgb"><a id="btnSaveDataGrid" class="btn btn-default"><span class="fa fa-save"></span>&nbsp;保存</a>&nbsp;<a id="btnRetrunDataGrid" class="btn btn-default"><span class="fa fa-reply"></span>&nbsp;返回</a></div><div style="padding:5px;">';
       //获取列表中的所有列
       var opts = $('#' + list).datagrid("options").columns[0];
       for (var i = 0; i < opts.length; i++) {
           //console.log(opts[i]);
           str = str + "<div class='editDatagrid'><input type='hidden' id='id_" + (opts[i].field) + "' value='" + (opts[i].field) + "' /><input style='width:100px;' id='title_" + (opts[i].field) + "' type='text' value='" + (opts[i].title) + "' />" +
               "<input class='magic-checkbox' type='checkbox' " + (opts[i].hidden != true ? "checked='checked'" : "") + " id='show_" + (opts[i].field) + "'/><label for='show_" + (opts[i].field) + "'>显示</label><label> 顺序 </label>" +
               "<input style='width:23px;' type='text' id='sort_" + (opts[i].field) + "' value='" + (i * 10) + "' /></div>"
       }
       str = str + "</div>";
       $("#modalwindow").html(str);
       $("#modalwindow").window({ title: "编辑表头", width: 720, height: 500, iconCls: "fa fa-pencil" }).window('open');

       //保存数据
       $("#btnSaveDataGrid").bind("click", function () {
           var array = [];
           var columns = [];
           $(".editDatagrid").each(function (i) {
               var id = $(this).find("input[id^='id_']").val();
               var title = $(this).find("input[id^='title_']").val();
               var hidden = $(this).find("input[id^='show_']").is(':checked');
               var sort = $(this).find("input[id^='sort_']").val();
               var temp = { Field: id, Title: title, Hidden: !hidden, Sort: sort };
               array.push(temp)
           });
           //获得需要保存的数据
           array.sort(up)
           showLoading();
           $.ajax({
               url: "SysDataGridConfig/Create",
               type: "Post",
               data: { inserted: JSON.stringify(array), moduleName: module },
               dataType: "json",
               success: function (data) {
                   if(data.type==1)
                   {
                       hideLoading();
                       $("#modalwindow").window('close');
                       $.messageBox5s('提示', "保存成功！");
                   }else
                   {
                       showLoading();
                       $.messageBox5s('提示', "请检查数据的可行性！");
                   }
               }
           });

           //赋值隐藏或者修改
           for (var j = 0; j < array.length; j++) {
               var bcolumn = $('#' + list).datagrid('getColumnOption', array[j].Field);
               bcolumn.title = array[j].Title;
               bcolumn.hidden = array[j].Hidden;

               columns.push(bcolumn);
           }
           
           $('#' + list).datagrid({ columns: [columns] });
       });
       //返回
       $("#btnRetrunDataGrid").bind("click", function () {
           $("#modalwindow").window('close');
       });
    }
});
//按升序排列
function up(x, y) {
    return x.Sort - y.Sort
}
/** 
* 在iframe中调用，在父窗口中出提示框(herf方式不用调父窗口)
*/
$.extend({
    messageBox5s: function (title, msg) {
        $.messager.show({
            title: '<span class="fa fa-info">&nbsp;&nbsp;' + title + '</span>', msg: msg, timeout: 5000, showType: 'slide', style: {
                left: '',
                right: 30,
                top: '',
                bottom: 30,
                width: 250,


            }
        });
    }
});
$.extend({
    messageBox10s: function (title, msg) {
        $.messager.show({
            title: '<span class="fa fa-info">&nbsp;&nbsp;' + title + '</span>', msg: msg, height: 'auto', width: 350, timeout: 10000, showType: 'slide', style: {
                left: '',
                right: 5,
                top: '',
                bottom: -document.body.scrollTop - document.documentElement.scrollTop + 5
            }
        });
    }
});
$.extend({
    show_alert: function (strTitle, strMsg) {
        $.messager.alert(strTitle, strMsg);
    }
});





/** 
* panel关闭时回收内存，主要用于layout使用iframe嵌入网页时的内存泄漏问题
*/
$.fn.panel.defaults.onBeforeDestroy = function () {

    var frame = $('iframe', this);
    try {
        // alert('销毁，清理内存');
        if (frame.length > 0) {
            for (var i = 0; i < frame.length; i++) {
                frame[i].contentWindow.document.write('');
                frame[i].contentWindow.close();
            }
            frame.remove();
            if ($.browser.msie) {
                CollectGarbage();
            }
        }
    } catch (e) {
    }
};


var oriFunc = $.fn.datagrid.defaults.view.onAfterRender;
$.fn.datagrid.defaults.view.onAfterRender = function (tgt) {
    if ($(tgt).datagrid("getRows").length > 0) {

        $(tgt).datagrid("getPanel").find("div.datagrid-body").find("div.datagrid-cell").each(function () {
            var $Obj = $(this)
            $Obj.attr("title", $Obj.text());
        })
    }
};


var oriFunctree = $.fn.treegrid.defaults.view.onAfterRender;
$.fn.treegrid.defaults.view.onAfterRender = function (tgt) {
    if ($(tgt).treegrid("getRoots").length > 0) {

        $(tgt).treegrid("getPanel").find("div.datagrid-body").find("div.datagrid-cell").each(function () {
            var $Obj = $(this)
            $Obj.attr("title", $Obj.text());
        })
    }
};

/**
* 防止panel/window/dialog组件超出浏览器边界
* @param left
* @param top
*/

var easyuiPanelOnMove = function (left, top) {
    var l = left;
    var t = top;
    if (l < 1) {
        l = 1;
    }
    if (t < 1) {
        t = 1;
    }
    var width = parseInt($(this).parent().css('width')) + 14;
    var height = parseInt($(this).parent().css('height')) + 14;
    var right = l + width;
    var buttom = t + height;
    var browserWidth = $(window).width();
    var browserHeight = $(window).height();
    if (right > browserWidth) {
        l = browserWidth - width;
    }
    if (buttom > browserHeight) {
        t = browserHeight - height;
    }
    $(this).parent().css({/* 修正面板位置 */
        left: l,
        top: t
    });
};
//$.fn.dialog.defaults.onMove = easyuiPanelOnMove;
//$.fn.window.defaults.onMove = easyuiPanelOnMove;
//$.fn.panel.defaults.onMove = easyuiPanelOnMove;
//让window居中
var easyuiPanelOnOpen = function (left, top) {

    var iframeWidth = $(this).parent().parent().width();

    var iframeHeight = $(this).parent().parent().height();

    var windowWidth = $(this).parent().width();
    var windowHeight = $(this).parent().height();

    var setWidth = (iframeWidth - windowWidth) / 2;
    var setHeight = (iframeHeight - windowHeight) / 2;
    $(this).parent().css({/* 修正面板位置 */
        left: setWidth,
        top: setHeight
    });

    if (iframeHeight < windowHeight) {
        $(this).parent().css({/* 修正面板位置 */
            left: setWidth,
            top: 0
        });
    }
    $(".window-shadow").hide();
    //修复被撑大的问题
    if ($(".window-mask") != null) {
        if ($(".window-mask").size() > 1) {
            $(".window-mask")[0].remove();
        }
    }
    $(".window-mask").attr("style", "display: block; z-index: 9002; width: " + iframeWidth - 200 + "px; height: " + iframeHeight - 200 + "px;");

    //$(".window-mask").hide().width(1).height(3000).show();
};
$.fn.window.defaults.onOpen = easyuiPanelOnOpen;
var easyuiPanelOnClose = function (left, top) {


    $(".window-mask").hide();

    //$(".window-mask").hide().width(1).height(3000).show();
};

$.fn.window.defaults.onClose = easyuiPanelOnClose;
var easyuiPanelOnResize = function (left, top) {


    var iframeWidth = $(this).parent().parent().width();

    var iframeHeight = $(this).parent().parent().height();

    var windowWidth = $(this).parent().width();
    var windowHeight = $(this).parent().height();


    var setWidth = (iframeWidth - windowWidth) / 2;
    var setHeight = (iframeHeight - windowHeight) / 2;
    $(this).parent().css({/* 修正面板位置 */
        left: setWidth-6,
        top: setHeight-6
    });

    if (iframeHeight < windowHeight) {
        $(this).parent().css({/* 修正面板位置 */
            left: setWidth,
            top: 0
        });
    }
    $(".window-shadow").hide();
    //$(".window-mask").hide().width(1).height(3000).show();
};
$.fn.window.defaults.onResize = easyuiPanelOnResize;


/**
* 
* @requires jQuery,EasyUI
* 
* 扩展tree，使其支持平滑数据格式
*/
$.fn.tree.defaults.loadFilter = function (data, parent) {
    var opt = $(this).data().tree.options;
    var idFiled, textFiled, parentField;
    //alert(opt.parentField);
    if (opt.parentField) {
        idFiled = opt.idFiled || 'id';
        textFiled = opt.textFiled || 'text';
        parentField = opt.parentField;
        var i, l, treeData = [], tmpMap = [];
        for (i = 0, l = data.length; i < l; i++) {
            tmpMap[data[i][idFiled]] = data[i];
        }
        for (i = 0, l = data.length; i < l; i++) {
            if (tmpMap[data[i][parentField]] && data[i][idFiled] != data[i][parentField]) {
                if (!tmpMap[data[i][parentField]]['children'])
                    tmpMap[data[i][parentField]]['children'] = [];
                data[i]['text'] = data[i][textFiled];
                tmpMap[data[i][parentField]]['children'].push(data[i]);
            } else {
                data[i]['text'] = data[i][textFiled];
                treeData.push(data[i]);
            }
        }
        return treeData;
    }
    return data;
};

/**

* @requires jQuery,EasyUI
* 
* 扩展combotree，使其支持平滑数据格式
*/
$.fn.combotree.defaults.loadFilter = $.fn.tree.defaults.loadFilter;

//如果datagrid过长显示...截断(格式化时候，然后调用resize事件)
//$.DataGridWrapTitleFormatter("值",$("#List"),"字段");
//onResizeColumn:function(field,width){ var refreshFieldList = ["字段名称","字段名称","字段名称"]; if(refreshFieldList.indexOf(field)>=0){$("#List").datagrid("reload");})}
$.extend({
    DataGridWrapTitleFormatter: function (value, obj, fidld) {
        if (value == undefined || value == null || value == "") {
            return "";
        }
        var options = obj.datagrid('getColumnOption', field);
        var cellWidth = 120;
        if (options != undefined) {
            cellWidth = options.width - 10;
        }
        return "<div style='width:" + cellWidth + "px;padding:0px 6px;line-height:25px;height:25px;margin-top:1px;cursor:pointer;white-space:nowrap:overflow:hidden;text-overflow:ellipsis;' title='" + value + "'>" + value + "</div>";
    }
});
//替换字符串
/* 
 * 功    能：替换字符串中某些字符
 * 参    数：sInput-原始字符串  sChar-要被替换的子串 sReplaceChar-被替换的新串
 * 返 回 值：被替换后的字符串
 */
$.extend({
    ReplaceStrAll: function (sInput, sChar, sReplaceChar) {
        if (sInput == "" || sInput == undefined) {
            return "";
        }
        var oReg = new RegExp(sChar, "g");
        return sInput.replace(oReg, sReplaceChar);

    }
});

/*
 * 功    能：替换字符串中某些字符（只能是第一个被替换掉）
 * 参    数：sInput-原始字符串  sChar-要被替换的子串 sReplaceChar-被替换的新串
 * 返 回 值：被替换后的字符串
 */
$.extend({
    ReplaceOne: function (sInput, sChar, sReplaceChar) {
        if (sInput == "" || sInput == undefined) {
            return "";
        }
        return sInput.replace(sChar, sReplaceChar);
    }
});


function myformatter(date) {
    var dateArray = date.split(" ");
    return dateArray[0].replace("/", "-").replace("/", "-");
}

function myparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}

function SetGridWidthSub(w) {
    return $(window).width() - w;
}
function SetGridHeightSub(h) {
    return $(window).height() - h
}


function SubStrYMD(value) {
    if (value == null || value == "") {
        return "";
    } else {
        return value.substr(0, value.indexOf(' '))
    }
}
function CustomFormatter(value, success, error) {
    if (value) {
        return "<span class='label label-success'>" + success + "</span>";
    } else {
        return "<span class='label label-error'>" + error + "</span>";
    }
}
function EnableFormatterMes(value,mes) {
    if (value) {
        return "<span class='label label-success'>" + mes + "</span>";
    } else {
        return "<span class='label label-error'>" + mes + "</span>";
    }
}

//圆点状态设置，只有蓝和绿色
function EnableFormatter(value) {
    if (value) {
        return "<span class='label label-success'>启用</span>";
    } else {
        return "<span class='label label-error'>禁用</span>";
    }
}

function CheckFormatter(value) {
    if (value) {
        return "<span class='label label-success'>已审核</span>";
    } else {
        return "<span class='label label-info'>未审核</span>";
    }
}

function ImgFormatter(value) {
    if (value) {
        return "<img height='55' src='" + value + "' />"
    } else {
        return ""
    }
}
function MoneyFormatter(value) {
    if (value) {
        return "￥" + value
    } else {
        return ""
    }
}


//ComboTree数据过滤
function queryComboTree(q, comboid, roots) {
    var datalist = [];//过滤后的数据源
    var childrenlist = [];//子节点数据

    $(comboid).combotree('setText', q);
    var entertext = $(comboid).combotree('getText');
    if (entertext == null || entertext == "") {
        //清空值之后重新加载数据
        $(comboid).combotree("loadData", roots).combotree("clear");
        return;
    }
    //循环数组
    for (var i = 0; i < roots.length; i++) {
        var org = {
            'id': roots[i].id,
            'text': roots[i].text,
            'children': []
        };
        if (q.toLowerCase() == roots[i].text.toLowerCase()) {
            $(comboid).combotree('setValue', roots[i].id);
        }
        var childrens = [];//查询到的子节点
        //递归找子节点
        childrensTree(comboid, roots[i], childrens, q);
        if (childrens.length > 0) {
            org.children = childrens;
            datalist.push(org);
        }
        else if (org.text.toLowerCase().indexOf(q.toLowerCase()) >= 0 && org.text != "") {
            //没有子节点，但是根节点符合要求
            datalist.push(org);
        }
    }

    if (datalist.length > 0) {
        $(comboid).combotree("loadData", datalist);
        $(comboid).combotree('setText', q);
        datalist = [];//初始化
        return;
    }

}
//组件ID，根节点，需要填充的数组，查询值
function childrensTree(comboid, roots, datalist, q) {
    var roots = roots.children;
    if (roots != undefined) {
        for (var j = 0; j < roots.length; j++) {
            var org = {
                'id': roots[j].id,
                'text': roots[j].text,
                'children': []
            };
            if (q.toLowerCase() == roots[j].text.toLowerCase()) {
                $(comboid).combotree('setValue', roots[j].id);
            }
            var childrens = [];//查询到的子节点
            //递归找子节点
            childrensTree(comboid, roots[j], childrens, q);
            if (childrens.length > 0) {
                org.children = childrens;
                datalist.push(org);
            }
            else if (org.text.toLowerCase().indexOf(q.toLowerCase()) >= 0 && org.text != "") {
                //没有子节点，但是根节点符合要求
                datalist.push(org);
            }
        }
    }
}

//合并列
//onLoadSuccess: function (data) {
//    $(this).datagrid("autoMergeCells", ['Area', 'PosCode']);
//},
$.extend($.fn.datagrid.methods, {
    autoMergeCells: function (jq, fields) {
        return jq.each(function () {
            var target = $(this);
            if (!fields) {
                fields = target.datagrid("getColumnFields");
            }
            var rows = target.datagrid("getRows");
            var i = 0,
			j = 0,
			temp = {};
            for (i; i < rows.length; i++) {
                var row = rows[i];
                j = 0;
                for (j; j < fields.length; j++) {
                    var field = fields[j];
                    var tf = temp[field];
                    if (!tf) {
                        tf = temp[field] = {};
                        tf[row[field]] = [i];
                    } else {
                        var tfv = tf[row[field]];
                        if (tfv) {
                            tfv.push(i);
                        } else {
                            tfv = tf[row[field]] = [i];
                        }
                    }
                }
            }
            $.each(temp, function (field, colunm) {
                $.each(colunm, function () {
                    var group = this;

                    if (group.length > 1) {
                        var before,
						after,
						megerIndex = group[0];
                        for (var i = 0; i < group.length; i++) {
                            before = group[i];
                            after = group[i + 1];
                            if (after && (after - before) == 1) {
                                continue;
                            }
                            var rowspan = before - megerIndex + 1;
                            if (rowspan > 1) {
                                target.datagrid('mergeCells', {
                                    index: megerIndex,
                                    field: field,
                                    rowspan: rowspan
                                });
                            }
                            if (after && (after - before) != 1) {
                                megerIndex = after;
                            }
                        }
                    }
                });
            });
        });
    }
});


$.extend($.fn.datagrid.defaults.editors, {
    textreadonly: {
        init: function (t, a) {
            var i = $('<input type="text" class="datagrid-editable-input bgcolor-gray" readonly="readonly"/>').appendTo(t);
           
            return i
        },
        destroy: function (e) {
            $(e).remove()
        },
        getValue: function (e) {
            return $(e).val()
        },
        setValue: function (e, t) {
            $(e).val(t);
            setTimeout(function () {
                e.focus()
            },
            100)
        },
        resize: function (e, t) {
            $(e[0]).width(t - 15);
        }
    },
    textevent: {
        init: function (t, a) {
            //setKeyUpValue自行逻辑
            var i = $('<input type="text" class="datagrid-editable-input" onkeyup="setKeyUpValue(this)"/>').appendTo(t);
            return i
        },
        destroy: function (e) {
            $(e).remove()
        },
        getValue: function (e) {
            return $(e).val()
        },
        setValue: function (e, t) {
            $(e).val(t);
            setTimeout(function ()
            {
                //zip Update:不需要自动获得焦点。
                //e.focus()
            },
            100)
        },
        resize: function (e, t) {
            $(e[0]).width(t - 15);
        }
    },
    zip_textevent: {
        init: function (t, a)
        {
            var field = $(t).context.attributes[0].value;
            //setKeyUpValue自行逻辑
            var i = $('<input type="text" field="' + field + '" class="datagrid-editable-input" onkeyup="setKeyUpValue(this)"/>').appendTo(t);
            return i
        },
        destroy: function (e)
        {
            $(e).remove()
        },
        getValue: function (e)
        {
            return $(e).val()
        },
        setValue: function (e, t)
        {
            $(e).val(t);
            setTimeout(function ()
            {
                //zip Update:不需要自动获得焦点。
                //e.focus()
            },
                100)
        },
        resize: function (e, t)
        {
            $(e[0]).width(t - 15);
        }
    },
    seltext: {
        init: function (t, a) {
            //设置一个输入框和一个扩大镜图标
            var i = $('<input type="text" class="datagrid-editable-input bgcolor-gray"  readonly="readonly" />&nbsp;<a href="javascript:SelDetails()" class="fa fa-search color-black"></a>').appendTo(t);
            if (a != undefined && a != null && a._medg) {
                i.keydown(function (t) {
                    e(t, a._medg)
                })
            }
            return i
        },
        destroy: function (e) {
            //销毁
             $(e).remove()
        },
        getValue: function (e) {
            //datagrid 结束编辑模式，通过该方法返回编辑最终值
            return $(e).val()
        },
        setValue: function (e, t) {
            //datagrid 进入编辑器模式，通过该方法为编辑赋值
            $(e).val(t);
            setTimeout(function () {
                e.focus()
            },
            100)
        },
        resize: function (e, t) {
            //列宽改变后调整编辑器宽度
            $(e[0]).width(t - 30);
        }
    },
    textarea: {
        init: function (t, a) {
            var i = $('<textarea class="datagrid-editable-input"></textarea>').appendTo(t);
            if (a != undefined && a != null && a._medg) {
                i.keydown(function (t) {
                    e(t, a._medg)
                })
            }
            return i
        },
        destroy: function (e) {
            $(e).remove()
        },
        getValue: function (e) {
            return $(e).val()
        },
        setValue: function (e, t) {
            $(e).val(t);
            setTimeout(function () {
                e.focus()
            },
            100)
        },
        resize: function (e, t) {
            e.outerWidth(t);
            e.outerHeight(e.parents("td[field]").height())
        }
    },
    checkbox: {//调用名称
        init: function (container, options) {
            //container 用于装载编辑器 options,提供编辑器初始参数
            var input = $('<input type="checkbox" class="datagrid-editable-input">').appendTo(container);
            //这里我把一个 checkbox类型的输入控件添加到容器container中
            // 需要渲染成easyu提供的控件，需要时用传入options,我这里如果需要一个combobox，就可以 这样调用 input.combobox(options);
            return input;
        },
        getValue: function (target) {
            //datagrid 结束编辑模式，通过该方法返回编辑最终值
            //这里如果用户勾选中checkbox返回1否则返回0
            return $(target).prop("checked") ? 1 : 0;
        },
        setValue: function (target, value) {
            //datagrid 进入编辑器模式，通过该方法为编辑赋值
            //我传入value 为0或者1，若用户传入1则勾选编辑器
            if (value)
                $(target).prop("checked", "checked")
        },
        resize: function (target, width) {
            //列宽改变后调整编辑器宽度
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    }

});

function ccc(o) {
    console.log(o);
}

// strPrintName 打印任务名  
// printDatagrid 要打印的datagrid  
function CreateFormPage(strPrintName, printDatagrid) {
    var tableString = '<div class="mvctool bgb"><a onclick="$(\'.dg-pb\').jqprint();" class="btn btn-default"><span class="fa fa-print"></span>&nbsp;打印</a></div><table cellspacing="0" class="dg-pb">';
    var frozenColumns = printDatagrid.datagrid("options").frozenColumns;  // 得到frozenColumns对象  
    var columns = printDatagrid.datagrid("options").columns;    // 得到columns对象  
    var nameList = '';

    // 载入title  
    if (typeof columns != 'undefined' && columns != '') {
        $(columns).each(function (index) {
            tableString += '\n<tr>';
            if (typeof frozenColumns != 'undefined' && typeof frozenColumns[index] != 'undefined') {
                for (var i = 0; i < frozenColumns[index].length; ++i) {
                    if (!frozenColumns[index][i].hidden) {
                        tableString += '\n<th width="' + frozenColumns[index][i].width + '"';
                        if (typeof frozenColumns[index][i].rowspan != 'undefined' && frozenColumns[index][i].rowspan > 1) {
                            tableString += ' rowspan="' + frozenColumns[index][i].rowspan + '"';
                        }
                        if (typeof frozenColumns[index][i].colspan != 'undefined' && frozenColumns[index][i].colspan > 1) {
                            tableString += ' colspan="' + frozenColumns[index][i].colspan + '"';
                        }
                        if (typeof frozenColumns[index][i].field != 'undefined' && frozenColumns[index][i].field != '') {
                            nameList += ',{"f":"' + frozenColumns[index][i].field + '", "a":"' + frozenColumns[index][i].align + '"}';
                        }
                        tableString += '>' + frozenColumns[0][i].title + '</th>';
                    }
                }
            }
            for (var i = 0; i < columns[index].length; ++i) {
                if (!columns[index][i].hidden) {
                    tableString += '\n<th width="' + columns[index][i].width + '"';
                    if (typeof columns[index][i].rowspan != 'undefined' && columns[index][i].rowspan > 1) {
                        tableString += ' rowspan="' + columns[index][i].rowspan + '"';
                    }
                    if (typeof columns[index][i].colspan != 'undefined' && columns[index][i].colspan > 1) {
                        tableString += ' colspan="' + columns[index][i].colspan + '"';
                    }
                    if (typeof columns[index][i].field != 'undefined' && columns[index][i].field != '') {
                        nameList += ',{"f":"' + columns[index][i].field + '", "a":"' + columns[index][i].align + '"}';
                    }
                    tableString += '>' + columns[index][i].title + '</th>';
                }
            }
            tableString += '\n</tr>';
        });
    }
    // 载入内容  
    var rows = printDatagrid.datagrid("getRows"); // 这段代码是获取当前页的所有行  
    var nl = eval('([' + nameList.substring(1) + '])');
    for (var i = 0; i < rows.length; ++i) {
        tableString += '\n<tr>';
        $(nl).each(function (j) {
            var e = nl[j].f.lastIndexOf('_0');

            tableString += '\n<td';
            if (nl[j].a != 'undefined' && nl[j].a != '') {
                tableString += ' style="text-align:' + nl[j].a + ';"';
            }
            tableString += '>';
            if (e + 2 == nl[j].f.length) {

                if (rows[i][nl[j].f.substring(0, e)] != null) {
                    tableString += rows[i][nl[j].f.substring(0, e)];
                } else {
                    tableString += "";
                }
            }
            else { 
                if (rows[i][nl[j].f] != null) {
                    tableString += rows[i][nl[j].f];
                } else {
                    tableString += "";
                }

            }
            tableString += '</td>';
        });
        tableString += '\n</tr>';
    }
    tableString += '\n</table>';

    return tableString;
}