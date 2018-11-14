//多語言設置
var Lang = {
    Nofilter: '不過濾',
    Contains: '包含',
    Equal: '相等',
    Notequal: '不等',
    Beginwith: '開頭',
    Endwith: '結尾',
    Less: '小於',
    Lessorequal: '小於等於',
    Greater: '大於',
    Greaterorequal: '大於等於'
};
//首頁
var index_lang_desktop = "我的桌面";
var index_lang_tip = "提示";
var index_lang_info = "我的資料";
var index_YouWantToExitTheSystem = "你確定要退出系統嗎 ?";
var index_ChangeThemeWillReloadTheSystem = "改變主題將重新載入系統!";
var index_NoTabsOnTheLeft = "左邊沒有標籤頁了";
var index_NoTabsOnTheRight = "右邊沒有標籤頁了";
//通用
var CommonLang = {
    PleaseSelectTheOperatingRecord :'請選擇要操作的資料！'
};
//EasyUI自帶
if ($.fn.pagination) {
	$.fn.pagination.defaults.beforePageText = '第';
	$.fn.pagination.defaults.afterPageText = '共{pages}頁';
	$.fn.pagination.defaults.displayMsg = '顯示{from}到{to},共{total}記錄';
}
if ($.fn.datagrid){
	$.fn.datagrid.defaults.loadMsg = '正在處理，請稍待。。。';
}
if ($.fn.treegrid && $.fn.datagrid){
	$.fn.treegrid.defaults.loadMsg = $.fn.datagrid.defaults.loadMsg;
}
if ($.messager){
	$.messager.defaults.ok = '確定';
	$.messager.defaults.cancel = '取消';
}
$.map(['validatebox','textbox','filebox','searchbox',
		'combo','combobox','combogrid','combotree',
		'datebox','datetimebox','numberbox',
		'spinner','numberspinner','timespinner','datetimespinner'], function(plugin){
	if ($.fn[plugin]){
		$.fn[plugin].defaults.missingMessage = '該輸入項為必輸項';
	}
});
if ($.fn.validatebox){
	$.fn.validatebox.defaults.rules.email.message = '請輸入有效的電子郵寄地址';
	$.fn.validatebox.defaults.rules.url.message = '請輸入有效的URL位址';
	$.fn.validatebox.defaults.rules.length.message = '輸入內容長度必須介於{0}和{1}之間';
	$.fn.validatebox.defaults.rules.remote.message = '請修正該欄位';
}
if ($.fn.calendar){
	$.fn.calendar.defaults.weeks = ['日','一','二','三','四','五','六'];
	$.fn.calendar.defaults.months = ['一月','二月','三月','四月','五月','六月','七月','八月','九月','十月','十一月','十二月'];
}
if ($.fn.datebox){
	$.fn.datebox.defaults.currentText = '今天';
	$.fn.datebox.defaults.closeText = '關閉';
	$.fn.datebox.defaults.okText = '確定';
	$.fn.datebox.defaults.formatter = function(date){
		var y = date.getFullYear();
		var m = date.getMonth()+1;
		var d = date.getDate();
		return y+'-'+(m<10?('0'+m):m)+'-'+(d<10?('0'+d):d);
	};
	$.fn.datebox.defaults.parser = function(s){
		if (!s) return new Date();
		var ss = s.split('-');
		var y = parseInt(ss[0],10);
		var m = parseInt(ss[1],10);
		var d = parseInt(ss[2],10);
		if (!isNaN(y) && !isNaN(m) && !isNaN(d)){
			return new Date(y,m-1,d);
		} else {
			return new Date();
		}
	};
}
if ($.fn.datetimebox && $.fn.datebox){
	$.extend($.fn.datetimebox.defaults,{
		currentText: $.fn.datebox.defaults.currentText,
		closeText: $.fn.datebox.defaults.closeText,
		okText: $.fn.datebox.defaults.okText
	});
}
if ($.fn.datetimespinner){
	$.fn.datetimespinner.defaults.selections = [[0,4],[5,7],[8,10],[11,13],[14,16],[17,19]]
}

