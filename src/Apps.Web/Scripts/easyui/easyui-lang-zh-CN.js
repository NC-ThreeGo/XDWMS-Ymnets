//多语言设置
var Lang = {
    Nofilter: '不过滤',
    Contains: '包含',
    Equal: '相等',
    Notequal: '不等',
    Beginwith: '开头',
    Endwith: '结尾',
    Less: '小于',
    Lessorequal: '小于等于',
    Greater: '大于',
    Greaterorequal: '大于等于',
    Tip: '提示',
    Edit: '修改',
    Create: '创建',
    Delete: '删除',
    Confirm: '确认',
};
//首页
var index_lang_desktop = "我的桌面";
var index_lang_tip = "提示";
var index_lang_info = "我的资料";
var index_YouWantToExitTheSystem = "你确定要退出系统吗 ?";
var index_ChangeThemeWillReloadTheSystem = "改变主题将重新加载系统!";
var index_NoTabsOnTheLeft = "左边没有标签页了";
var index_NoTabsOnTheRight = "右边没有标签页了";
//通用
var CommonLang = {
    PleaseSelectTheOperatingRecord: '请选择要操作的数据！',
    YouWantToDeleteTheSelectedRecords: '你要删除所选择的记录吗？',
};
//EasyUI自带
if ($.fn.pagination) {
    $.fn.pagination.defaults.beforePageText = '第';
    $.fn.pagination.defaults.afterPageText = '共{pages}页';
    $.fn.pagination.defaults.displayMsg = '显示{from}到{to},共{total}记录';
}
if ($.fn.datagrid) {
    $.fn.datagrid.defaults.loadMsg = '正在处理，请稍待。。。';
}
if ($.fn.treegrid && $.fn.datagrid) {
    $.fn.treegrid.defaults.loadMsg = $.fn.datagrid.defaults.loadMsg;
}
if ($.messager) {
    $.messager.defaults.ok = '确定';
    $.messager.defaults.cancel = '取消';
}
$.map(['validatebox', 'textbox', 'filebox', 'searchbox',
		'combo', 'combobox', 'combogrid', 'combotree',
		'datebox', 'datetimebox', 'numberbox',
		'spinner', 'numberspinner', 'timespinner', 'datetimespinner'], function (plugin) {
		    if ($.fn[plugin]) {
		        $.fn[plugin].defaults.missingMessage = '该输入项为必输项';
		    }
		});
if ($.fn.validatebox) {
    $.fn.validatebox.defaults.rules.email.message = '请输入有效的电子邮件地址';
    $.fn.validatebox.defaults.rules.url.message = '请输入有效的URL地址';
    $.fn.validatebox.defaults.rules.length.message = '输入内容长度必须介于{0}和{1}之间';
    $.fn.validatebox.defaults.rules.remote.message = '请修正该字段';
}
if ($.fn.calendar) {
    $.fn.calendar.defaults.weeks = ['日', '一', '二', '三', '四', '五', '六'];
    $.fn.calendar.defaults.months = ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'];
}
if ($.fn.datebox) {
    $.fn.datebox.defaults.currentText = '今天';
    $.fn.datebox.defaults.closeText = '关闭';
    $.fn.datebox.defaults.okText = '确定';
    $.fn.datebox.defaults.formatter = function (date) {
        var y = date.getFullYear();
        var m = date.getMonth() + 1;
        var d = date.getDate();
        return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
    };
    $.fn.datebox.defaults.parser = function (s) {
        if (!s) return new Date();
        var ss = s.split('-');
        var y = parseInt(ss[0], 10);
        var m = parseInt(ss[1], 10);
        var d = parseInt(ss[2], 10);
        if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
            return new Date(y, m - 1, d);
        } else {
            return new Date();
        }
    };
}
if ($.fn.datetimebox && $.fn.datebox) {
    $.extend($.fn.datetimebox.defaults, {
        currentText: $.fn.datebox.defaults.currentText,
        closeText: $.fn.datebox.defaults.closeText,
        okText: $.fn.datebox.defaults.okText
    });
}
if ($.fn.datetimespinner) {
    $.fn.datetimespinner.defaults.selections = [[0, 4], [5, 7], [8, 10], [11, 13], [14, 16], [17, 19]]
}

