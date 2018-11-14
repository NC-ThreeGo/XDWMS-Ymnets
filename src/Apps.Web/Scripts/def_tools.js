
//导航表格
function RowNav(gridName) {
        //////////////////////////////////////
        RowNav.prototype.FirstRow = function () {
            $(gridName).datagrid("selectRow", 0);
        }
        RowNav.prototype.PriorRow = function () {
            var selected = $(gridName).datagrid('getSelected');
            if (selected) {
                var index = $(gridName).datagrid('getRowIndex', selected);
                if (index == 0) {
                    $(gridName).datagrid("selectRow", 0);
                } else {
                    $(gridName).datagrid('selectRow', index - 1);
                }
            } else {
                $(gridName).datagrid("selectRow", 0);
            }
        }
        RowNav.prototype.NextRow = function () {
            var selected = $(gridName).datagrid('getSelected');
            var len = $(gridName).datagrid('getRows').length;
            if (selected) {
                var index = $(gridName).datagrid('getRowIndex', selected);
                if (index ==len-1) {
                    $(gridName).datagrid("selectRow", len - 1);
                } else {
                    $(gridName).datagrid('selectRow', index + 1);
                }
            } else {
                $(gridName).datagrid("selectRow", 0);
            }
        }
        RowNav.prototype.LastRow = function () {
            var len = $(gridName).datagrid('getRows').length;
            $(gridName).datagrid("selectRow", len-1);

        }
}

//获取错误级别
function GetErrorLevl(value)
{
    switch (value)
    {
        case 1: return "普通";
        case 2: return "异常";
        case 3: return "严重";
        case 4: return "严重+";
        case 5: return "严重++";
        default: return "不处理";
    }
}
//true返回对，flase返回X，null返回无
function GetPassResut(value)
{
    if (value == null) {
        return "<span class='nobegin'>○</span>";
    }
    if (value) {
        return "<span class='spanpass'>√</span>";
    } else {
        return "<span class='spanfail'>×</span>";
    }
}