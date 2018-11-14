using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

/// <summary>
/// 树形Index视图
/// </summary>
namespace Apps.CodeHelper
{
    public partial class CodeFrom
    {
        public string GetTreeIndex(string tableName)
        {
            string leftStr = GetLeftStr(tableName);
            List<CompleteField> fields = SqlHelper.GetColumnCompleteField(conn, tableName);
            List<CompleteField> parentFields = new List<CompleteField>();
            bool isSort = false;
            bool isCreateTime = false;
            if (cb_EnableParent.Checked)
            {
                parentFields = SqlHelper.GetColumnCompleteField(conn, txt_TableName1.Text);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("@using " + txt_prefix.Text + ".Web.Core;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Web;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Common;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Models.Sys;\r\n");
            sb.Append("@using " + txt_prefix.Text + ".Locale;\r\n");
            sb.Append("@{\r\n");
            sb.Append("    ViewBag.Title = \"" + tableName + "\";\r\n");
            sb.Append("    Layout = \"~/Views/Shared/_Index_Layout.cshtml\";\r\n");
            sb.Append("    List<permModel> perm = null;\r\n");
            sb.Append("}\r\n");
            //导出
            sb.Append("<div id = \"uploadExcel\" class=\"easyui-window\" data-options=\"modal:true,closed:true,minimizable:false,shadow:false\">\r\n");
            sb.Append("<form name = \"form1\" method=\"post\" id=\"form1\">\r\n");
            sb.Append("    <table>\r\n");
            sb.Append("        <tr>\r\n");
            sb.Append("            <th style=\"padding:20px;\"> Excel：</th>\r\n");
            sb.Append("            <td style=\"padding:20px;\">\r\n");
            sb.Append("                <input name=\"ExcelPath\" type=\"text\" maxlength=\"255\" id=\"txtExcelPath\" readonly=\"readonly\" style=\"width:200px\" class=\"txtInput normal left\">\r\n");
            sb.Append("                <a href = \"javascript:$('#FileUpload').trigger('click').void(0);\" class=\"files\">@Resource.Browse</a>\r\n");
            sb.Append("                <input class=\"displaynone\" type=\"file\" id=\"FileUpload\" name=\"FileUpload\" onchange=\"Upload('ExcelFile', 'txtExcelPath', 'FileUpload'); \">\r\n");
            sb.Append("                <span class=\"uploading\">@Resource.Uploading</span>\r\n");
            sb.Append("            </td>\r\n");
            sb.Append("        </tr>\r\n");
            sb.Append("    </table>\r\n");
            sb.Append("    <div class=\"endbtndiv\">\r\n");
            sb.Append("        <a id = \"btnSave\" href=\"javascript:ImportData()\" class=\"easyui-linkbutton btns\">直接保存</a>\r\n");
            //sb.Append("        <a id = \"btnSaveBefor\" href=\"javascript:ImportDataBefor()\" class=\"easyui-linkbutton btnsb\" style=\"width:80px; \">保存前编辑</a>\r\n");
            sb.Append("        <a id = \"btnReturn\" href=\"javascript:$('#uploadExcel').window('close')\" class=\"easyui-linkbutton btnc\">@Resource.Cancel</a>\r\n");
            sb.Append("    </div>\r\n");
            sb.Append("</form>\r\n");
            sb.Append("</div>\r\n");
            //开启父表联合生成
            if (cb_EnableParent.Checked)
            {
                sb.Append("<table style=\"width:100%\">\r\n");
                sb.Append("<tbody>\r\n");
                sb.Append("    <tr>\r\n");
                sb.Append("        <td style=\"width:330px;vertical-align: top\">\r\n");
                sb.Append("             <div class=\"mvctool\">\r\n");
                if (cb_MulView.Checked)//启用子表可以编辑父表
                {
                    //sb.Append("                 <input id=\"txtQueryParent\" type=\"text\" class=\"searchText\" />\r\n");
                    sb.Append("                 @Html.ToolButton(\"btnQueryParent\", \"fa fa-search\", Resource.Query,ref perm, \"Query\", true)\r\n");
                    sb.Append("                 @Html.ToolButton(\"btnCreateParent\", \"fa fa-plus\", Resource.Create,ref perm, \"Create\", true)\r\n");
                    sb.Append("                 @Html.ToolButton(\"btnEditParent\", \"fa fa-pencil\", Resource.Edit,ref perm, \"Edit\", true)\r\n");
                    sb.Append("                 @Html.ToolButton(\"btnDeleteParent\", \"fa fa-trash\", Resource.Delete,ref perm, \"Delete\", true)\r\n");

                }
                sb.Append("             </div>\r\n");
                sb.Append("            <table id=\"ListParent\"></table>\r\n");
                sb.Append("        </td>");
                sb.Append("        <td style=\"width:3px;\"></td>\r\n");
                sb.Append("        <td style=\"vertical-align:top\">\r\n");
                sb.Append("             <div class=\"mvctool\">\r\n");
                sb.Append("                 <input id=\"txtQuery\" type=\"text\" class=\"searchText\" />\r\n");
                sb.Append("                 @Html.ToolButton(\"btnQuery\", \"fa fa-search\", Resource.Query,ref perm, \"Query\", true)\r\n");
                sb.Append("                 @Html.ToolButton(\"btnCreate\", \"fa fa-plus\", Resource.Create,ref perm, \"Create\", true)\r\n");
                sb.Append("                 @Html.ToolButton(\"btnEdit\", \"fa fa-pencil\", Resource.Edit,ref perm, \"Edit\", true)\r\n");
                sb.Append("                 @Html.ToolButton(\"btnDelete\", \"fa fa-trash\", Resource.Delete,ref perm, \"Delete\", true)\r\n");
                sb.Append("                 @Html.ToolButton(\"btnImport\", \"fa fa-level-down\", Resource.Import, ref perm, \"Import\", true)\r\n");
                sb.Append("                 @Html.ToolButton(\"btnExport\", \"fa fa-level-up\", Resource.Export, ref perm, \"Export\", true)\r\n");
                sb.Append("             </div>\r\n");
                sb.Append("            <table id=\"List\"></table>\r\n");
                sb.Append("        </td>\r\n");
                sb.Append("    </tr>\r\n");
                sb.Append("</tbody>\r\n");
                sb.Append("</table>\r\n");
            }
            else
            {
                sb.Append("<div class=\"mvctool\">\r\n");
                sb.Append("    <input id=\"txtQuery\" type=\"text\" class=\"searchText\" />\r\n");
                sb.Append("    @Html.ToolButton(\"btnQuery\", \"fa fa-search\", Resource.Query,ref perm, \"Query\", true)\r\n");
                sb.Append("    @Html.ToolButton(\"btnCreate\", \"fa fa-plus\", Resource.Create,ref perm, \"Create\", true)\r\n");
                sb.Append("    @Html.ToolButton(\"btnEdit\", \"fa fa-pencil\", Resource.Edit,ref perm, \"Edit\", true)\r\n");
                sb.Append("    @Html.ToolButton(\"btnDelete\", \"fa fa-trash\", Resource.Delete,ref perm, \"Delete\", true)\r\n");
                sb.Append("    @Html.ToolButton(\"btnImport\", \"fa fa-level-down\", Resource.Import, ref perm, \"Import\", true)\r\n");
                sb.Append("    @Html.ToolButton(\"btnExport\", \"fa fa-level-up\", Resource.Export, ref perm, \"Export\", true)\r\n");
                sb.Append("</div>\r\n");
                sb.Append("<table id=\"List\"></table>\r\n");
                sb.Append("@Html.Partial(\"~/Views/Shared/_Partial_AutoGrid.cshtml\")\r\n");
            }

            sb.Append("\r\n");
            sb.Append("<div id=\"modalwindow\" class=\"easyui-window\" style=\"width:800px; height:400px;\" data-options=\"modal:true,closed:true,minimizable:false,shadow:false\"></div>\r\n");
            sb.Append("<script type=\"text/javascript\">\r\n");
            sb.Append("    $(function () {\r\n");
            if (cb_EnableParent.Checked)
            {
                #region 父表
                sb.Append("        $('#ListParent').datagrid({\r\n");
                sb.Append("            url: '@Url.Action(\"GetListParent\")',\r\n");
                sb.Append("            width:430,\r\n");
                sb.Append("            methord: 'post',\r\n");
                sb.Append("            height: SetGridHeightSub(45),\r\n");
                sb.Append("            fitColumns: true,\r\n");
                isSort = false;
                isCreateTime = false;
                foreach (CompleteField field in parentFields)
                {
                    if (field.name == "Sort")
                    {
                        isSort = true;
                    }
                    if (field.name == "CreateTime")
                    {
                        isCreateTime = true;
                    }
                }
                if (isSort)
                {
                    sb.Append("            sortName: 'Sort',\r\n");
                    sb.Append("            sortOrder: 'asc',\r\n");
                }
                else if (isCreateTime)
                {
                    sb.Append("            sortName: 'CreateTime',\r\n");
                    sb.Append("            sortOrder: 'desc',\r\n");
                }
                else
                {
                    sb.Append("            sortName: 'Id',\r\n");
                    sb.Append("            sortOrder: 'desc',\r\n");
                }

                sb.Append("            idField: 'Id',\r\n");
                sb.Append("            pageSize: 15,\r\n");
                sb.Append("            pageList: [15, 20, 30, 40, 50],\r\n");
                sb.Append("            pagination: true,\r\n");
                sb.Append("            striped: true, //奇偶行是否区分\r\n");
                sb.Append("            singleSelect: true,//单选模式\r\n");
                sb.Append("            //rownumbers: true,//行号\r\n");
                sb.Append("            onLoadSuccess: function(data) {");
                sb.Append("            },\r\n");
                sb.Append("            columns: [[\r\n");
                foreach (CompleteField field in parentFields)
                {
                    //主键，隐藏
                    if (field.name == "Id")
                    {
                        sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 80,hidden:true },\r\n");
                    }//布尔类型，加formatter
                    else if (field.xType == "104" || field.xType == "bool")
                    {
                        sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 40,sortable:true,align:'center', formatter: function (value) {return EnableFormatter(value)}},\r\n");
                    }
                    else if (field.name.ToLower().Contains("img") || field.name.ToLower().Contains("photo"))
                    {
                        sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 40,sortable:true, align: 'center', formatter: function (value, row, index) {return '<img width=\"80px\" alt=\"example\" src=\"' + value + '\" />';}},\r\n");
                    }
                    else
                    {
                        sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 80,sortable:true },\r\n");
                    }

                }

                sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append("            ]]\r\n");

                sb.Append("         ,onClickRow: function(index, row) {\r\n");
                sb.Append("             $('#List').datagrid(\"load\", { ParentId: row.Id });\r\n");
                sb.Append("}\r\n");

                sb.Append("        }).datagrid('getPager').pagination({ showPageList: false, showRefresh: false });\r\n");
                sb.Append("         $(window).resize(function() {\r\n");
                sb.Append("             resizeLayout();\r\n");
                sb.Append("         });\r\n");

                #endregion
            }

            #region 子表
            sb.Append("        $('#List').treegrid({\r\n");
            if (cb_EnableParent.Checked)
            {
                sb.Append("            url: '@Url.Action(\"GetList\")?parentId=0',\r\n");
                sb.Append("            width:SetGridWidthSub(450),\r\n");
            }
            else
            {
                sb.Append("            url: '@Url.Action(\"GetList\")',\r\n");
                sb.Append("            width:SetGridWidthSub(10),\r\n");
            }
            sb.Append("            methord: 'post',\r\n");
            sb.Append("            height: SetGridHeightSub(45),\r\n");
            sb.Append("            fitColumns: true,\r\n");
            sb.Append("            idField: 'Id',\r\n");
            sb.Append("            treeField: 'Name',\r\n");
            sb.Append("            pagination: false,\r\n");
            sb.Append("            striped: true, //奇偶行是否区分\r\n");
            sb.Append("            singleSelect: true,//单选模式\r\n");
            sb.Append("            onLoadSuccess: function(data) {");
            sb.Append("            },\r\n");
            sb.Append("            columns: [[\r\n");
            foreach (CompleteField field in fields)
            {
                //主键，隐藏
                if (field.name == "Id")
                {
                    sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 80,hidden:true },\r\n");
                }//布尔类型，加formatter
                else if (field.xType == "104" || field.xType == "bool")
                {
                    sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 40,sortable:true,align:'center', formatter: function (value) {return EnableFormatter(value)}},\r\n");
                }
                else if (field.name.ToLower().Contains("img") || field.name.ToLower().Contains("photo"))
                {
                    sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 40,sortable:true, align: 'center', formatter: function (value, row, index) {return '<img width=\"80px\" alt=\"example\" src=\"' + value + '\" />';}},\r\n");
                }
                else
                {
                    sb.Append("                { field: '" + field.name + "', title: '" + (field.remark == "" ? field.name : field.remark) + "', width: 80,sortable:true },\r\n");
                }

            }
            //启用表关联
            if (cb_EnableParent.Checked)
            {
                //表1
                if (!string.IsNullOrWhiteSpace(txt_TableName1.Text))
                {
                    sb.Append("                { field: '" + txt_TableName1.Text.Replace(leftStr + "_", "") + "Name" + "', title: '类别名称" + "', width: 80 },\r\n");
                }

            }
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append("            ]]\r\n");
            sb.Append("        });\r\n");
            sb.Append("    });\r\n");
            #endregion 子表

            sb.Append("    //ifram 返回\r\n");
            sb.Append("    function frameReturnByClose() {\r\n");
            sb.Append("        $(\"#modalwindow\").window('close');\r\n");
            sb.Append("    }\r\n");
            sb.Append("    function frameReturnByReload(flag) {\r\n");

            if (cb_EnableParent.Checked)
            {
                sb.Append("        if (flag)\r\n");
                sb.Append("        {\r\n");
                if (cb_MulView.Checked)
                {
                    sb.Append("            $(\"#ListParent\").datagrid('load');\r\n");
                    sb.Append("            $(\"#List\").treegrid('load');\r\n");
                }
                else
                {
                    sb.Append("            $(\"#List\").datagrid('load');\r\n");
                }

                sb.Append("        }\r\n");
                sb.Append("        else\r\n");
                sb.Append("        {\r\n");
                sb.Append("            $(\"#ListParent\").datagrid('reload');\r\n");
                sb.Append("            $(\"#List\").treegrid('reload');\r\n");
                sb.Append("        }\r\n");
            }
            else
            {
                sb.Append("        if (flag)\r\n");
                sb.Append("            $(\"#List\").treegrid('load');\r\n");
                sb.Append("        else\r\n");
                sb.Append("            $(\"#List\").treegrid('reload');\r\n");
            }

            sb.Append("    }\r\n");
            sb.Append("    function frameReturnByMes(mes) {\r\n");
            sb.Append("        $.messageBox5s(Lang.Tip, mes);\r\n");
            sb.Append("    }\r\n");
            sb.Append("    $(function () {\r\n");

            #region 子表

            sb.Append("        $(\"#btnCreate\").click(function () {\r\n");
            sb.Append("            $.modalWindow(Lang.Create, '@Url.Action(\"Create\")', 700, 400, 'fa fa-plus');\r\n");
            sb.Append("        });\r\n");
            sb.Append("        $(\"#btnEdit\").click(function () {\r\n");
            sb.Append("            var row = $('#List').datagrid('getSelected');\r\n");
            sb.Append("            if (row != null) {\r\n");
            sb.Append("                $.modalWindow(Lang.Edit, '@Url.Action(\"Edit\")?id=' + row.Id + '&Ieguid=' + GetGuid(), 700, 400, 'fa fa-pencil');\r\n");
            sb.Append("            } else { $.messageBox5s(Lang.Tip, Lang.PleaseSelectTheOperatingRecord); }\r\n");
            sb.Append("        });\r\n");
            sb.Append("        $(\"#btnDetails\").click(function () {\r\n");
            sb.Append("            var row = $('#List').datagrid('getSelected');\r\n");
            sb.Append("            if (row != null) {\r\n");
            sb.Append("                $.modalWindow(Lang.Details, '@Url.Action(\"Details\")?id=' + row.Id + '&Ieguid=' + GetGuid(), 700, 400, 'fa fa-list');\r\n");
            sb.Append("            } else { $.messageBox5s(Lang.Tip, Lang.PleaseSelectTheOperatingRecord); }\r\n");
            sb.Append("	        });\r\n");
            sb.Append("        $(\"#btnQuery\").click(function () {\r\n");
            sb.Append("            var queryStr = $(\"#txtQuery\").val();\r\n");
            sb.Append("            if (queryStr == null) {\r\n");
            sb.Append("                queryStr = \"%\";\r\n");
            sb.Append("            }\r\n");
            sb.Append("            $(\"#List\").datagrid(\"load\", { queryStr: queryStr });\r\n");
            sb.Append("\r\n");
            sb.Append("        });\r\n");
            sb.Append("        $(\"#btnDelete\").click(function () {\r\n");
            sb.Append("	            dataDelete(\"@Url.Action(\"Delete\")\", \"List\");\r\n");
            sb.Append("	        });\r\n");
            //导入导出
            sb.Append("        $(\"#btnImport\").click(function() {\r\n");
            sb.Append("             $(\"#txtExcelPath\").val(\"\");\r\n");
            sb.Append("             $(\"#uploadExcel\").window({ title: '@Resource.Import', width: 450, height: 155, iconCls: 'fa fa-level-down' }).window('open');\r\n");
            sb.Append("        });\r\n");
            sb.Append("        $(\"#btnExport\").click(function() {\r\n");
            sb.Append("          $.post(\"@Url.Action(\"CheckExportData\")\", function(data) {\r\n");
            sb.Append("                if (data.type == 1)\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    window.location = \"@Url.Action(\"Export\")\";\r\n");
            sb.Append("                }\r\n");
            sb.Append("                else\r\n");
            sb.Append("                {\r\n");
            sb.Append("                $.messageBox5s(Lang.Tip, data.message);\r\n");
            sb.Append("                }\r\n");
            sb.Append("            }, \"json\");\r\n");
            sb.Append("        });\r\n");



            #endregion

            #region 父表
            if (cb_EnableParent.Checked && cb_MulView.Checked)
            {
                sb.Append("        $(\"#btnCreateParent\").click(function () {\r\n");
                sb.Append("            $.modalWindow(Lang.Create, '@Url.Action(\"CreateParent\")', 700, 400, 'fa fa-plus');\r\n");
                sb.Append("        });\r\n");
                sb.Append("        $(\"#btnEditParent\").click(function () {\r\n");
                sb.Append("            var row = $('#ListParent').datagrid('getSelected');\r\n");
                sb.Append("            if (row != null) {\r\n");
                sb.Append("                $.modalWindow(Lang.Edit, '@Url.Action(\"EditParent\")?id=' + row.Id + '&Ieguid=' + GetGuid(), 700, 400, 'fa fa-pencil');\r\n");
                sb.Append("            } else { $.messageBox5s(Lang.Tip, Lang.PleaseSelectTheOperatingRecord); }\r\n");
                sb.Append("        });\r\n");
                sb.Append("        $(\"#btnDetailsParent\").click(function () {\r\n");
                sb.Append("            var row = $('#List').datagrid('getSelected');\r\n");
                sb.Append("            if (row != null) {\r\n");
                sb.Append("                $.modalWindow(Lang.Details, '@Url.Action(\"DetailsParent\")?id=' + row.Id + '&Ieguid=' + GetGuid(), 700, 400, 'fa fa-list');\r\n");
                sb.Append("            } else { $.messageBox5s(Lang.Tip, Lang.PleaseSelectTheOperatingRecord); }\r\n");
                sb.Append("	        });\r\n");
                sb.Append("        $(\"#btnQueryParent\").click(function () {\r\n");
                sb.Append("            var queryStr = $(\"#txtQueryParent\").val();\r\n");
                sb.Append("            if (queryStr == null) {\r\n");
                sb.Append("                queryStr = \"%\";\r\n");
                sb.Append("            }\r\n");
                sb.Append("            $(\"#ListParent\").datagrid(\"load\", { queryStr: queryStr });\r\n");
                sb.Append("\r\n");
                sb.Append("        });\r\n");
                sb.Append("        $(\"#btnDeleteParent\").click(function () {\r\n");
                sb.Append("	           dataDelete(\"@Url.Action(\"Delete\")\", \"ListParent\");\r\n");
                sb.Append("	       });\r\n");
            }
            #endregion

            sb.Append("    });\r\n");

            //导入导出
            sb.Append("    function ImportData()\r\n");
            sb.Append("    {\r\n");
            sb.Append("        showLoading();\r\n");
            sb.Append("        var url = \"@Url.Action(\"Import\")?filePath=\"+$(\"#txtExcelPath\").val();\r\n");
            sb.Append("        $.post(url, function(data) {\r\n");
            sb.Append("            if (data.type == 1)\r\n");
            sb.Append("            {\r\n");
            sb.Append("             $(\"#List\").datagrid('load');\r\n");
            sb.Append("             $('#uploadExcel').window('close');\r\n");
            sb.Append("             $('#FileUpload').val('');\r\n");
            sb.Append("            }\r\n");
            sb.Append("            hideLoading();\r\n");
            sb.Append("            $.messageBox5s('提示', data.message);\r\n");
            sb.Append("        }, \"json\");\r\n");
            sb.Append("    }\r\n");

            if (cb_EnableParent.Checked && cb_MulView.Checked)
            {
                sb.Append("    function resizeLayout()\r\n");
                sb.Append("     {\r\n");
                sb.Append("         setTimeout(function () {\r\n");
                sb.Append("             $('#ListParent').datagrid('resize', {\r\n");
                sb.Append("             }).datagrid('resize', {\r\n");
                if (cb_MulView.Checked)
                {
                    sb.Append("                 height: SetGridHeightSub(45)\r\n");
                }
                else
                {
                    sb.Append("                 height: SetGridHeightSub(81)\r\n");
                }
                sb.Append("             });\r\n");
                sb.Append("             $('#List').treegrid('resize', {\r\n");
                sb.Append("             }).datagrid('resize', {\r\n");
                sb.Append("                 width: $(window).width() - 450,\r\n");
                sb.Append("                 height: SetGridHeightSub(45)\r\n");
                sb.Append("             });\r\n");
                sb.Append("         },100);\r\n");
                sb.Append("	    }\r\n");
            }

            sb.Append("</script>\r\n");

            return sb.ToString();
        }
    }
}
