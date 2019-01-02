//多语言设置
var Lang = {
    Nofilter: "No Filter",
    Contains: "Contains",
    Equal: "Equal",
    Notequal: "Not Equal",
    Beginwith: "Begin With",
    Endwith: "End With",
    Less: "Less",
    Lessorequal: "Less or Equal",
    Greater: "Greater",
    Greaterorequal: "Greater or Equal"
};
//首页
var index_lang_desktop = "Desktop";
var index_lang_tip = "Tip";
var index_lang_info = "Profile";
var index_YouWantToExitTheSystem = "You want to logout the system?";
var index_ChangeThemeWillReloadTheSystem = "Change theme will reload the system!";
var index_NoTabsOnTheLeft = "No tabs on the left";
var index_NoTabsOnTheRight = "No tabs on the right";
//通用
var CommonLang = {
    PleaseSelectTheOperatingRecord: 'Please select the operating record！',
    YouWantToDeleteTheSelectedRecords: 'You want to delete The selected records!'
};
//EasyUI自带
if ($.fn.pagination) {
	$.fn.pagination.defaults.beforePageText = 'Page';
	$.fn.pagination.defaults.afterPageText = 'of {pages}';
	$.fn.pagination.defaults.displayMsg = 'Displaying {from} to {to} of {total} items';
}
if ($.fn.datagrid){
	$.fn.datagrid.defaults.loadMsg = 'Processing, please wait ...';
}
if ($.fn.treegrid && $.fn.datagrid){
	$.fn.treegrid.defaults.loadMsg = $.fn.datagrid.defaults.loadMsg;
}
if ($.messager){
	$.messager.defaults.ok = 'Ok';
	$.messager.defaults.cancel = 'Cancel';
}
$.map(['validatebox','textbox','filebox','searchbox',
		'combo','combobox','combogrid','combotree',
		'datebox','datetimebox','numberbox',
		'spinner','numberspinner','timespinner','datetimespinner'], function(plugin){
	if ($.fn[plugin]){
		$.fn[plugin].defaults.missingMessage = 'This field is required.';
	}
});
if ($.fn.validatebox){
	$.fn.validatebox.defaults.rules.email.message = 'Please enter a valid email address.';
	$.fn.validatebox.defaults.rules.url.message = 'Please enter a valid URL.';
	$.fn.validatebox.defaults.rules.length.message = 'Please enter a value between {0} and {1}.';
	$.fn.validatebox.defaults.rules.remote.message = 'Please fix this field.';
}
if ($.fn.calendar){
	$.fn.calendar.defaults.weeks = ['S','M','T','W','T','F','S'];
	$.fn.calendar.defaults.months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
}
if ($.fn.datebox){
	$.fn.datebox.defaults.currentText = 'Today';
	$.fn.datebox.defaults.closeText = 'Close';
	$.fn.datebox.defaults.okText = 'Ok';
}
if ($.fn.datetimebox && $.fn.datebox){
	$.extend($.fn.datetimebox.defaults,{
		currentText: $.fn.datebox.defaults.currentText,
		closeText: $.fn.datebox.defaults.closeText,
		okText: $.fn.datebox.defaults.okText
	});
}

