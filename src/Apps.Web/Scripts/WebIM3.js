/*  必须确定主窗口存在这2个方法
    function ReturnSelMemberByClose() {
        $("#dailydeal").dialog("close");
    }
    function RetureSelMember(result) {

    }
*/


var CBFlag = false;
var CBID = "";
//第一次加载
$(function () {


    //初始化联系人列表
    var o = { showcheck: false,
        url: "../../WebIM/GetMemberTree",
        onnodeclick: function (item) {
            var tabTitle = item.text;
            var url = "../../" + item.value;
            var icon;
            //打开模块，非模块不打开
            if (!item.hasChildren) {
                // $(this).find("input").eq(0).prop("checked", !($(this).find("input").eq(0).prop("checked")));
            } else {

            }

        }
    };
    o.theme = "bbit-tree-arrows"; //bbit-tree-lines ,bbit-tree-no-lines,bbit-tree-arrows
    $.post("../../WebIM/GetMemberTree", { "id": "0" }, function (data) {
        o.data = data;
        $("#MemberListTree").treeview(o);
        $(".CBGroup").unbind();
        $(".CBMember").unbind();
        BindMemberClick();
        $(".window-mask").hide();
    }, "json");
    //初始化最近联系人
    //初始化常用语言
    $.post("../../WebIM/GetCommonTalk", function (data) { $("#CommonTalkList").html(data); }, "json");


    //定时刷新在线人数
    refresh_onlineusers();
    //刷新最近联系人
    refresh_recentContact();

});


//定时刷新在线人数 2分钟刷新一次
function refresh_onlineusers() {
    //定时更新

    window.setTimeout(refresh_onlineusers, MIS0003);
}

//定时刷新最近联系人 2分钟刷新一次
function refresh_recentContact() {
    //定时更新
    $.post("../../WebIM/GetRecentContact", function (data) {
        //发到前台
        $("#LatertDiv").html(data);
    }, "json");
    window.setTimeout(refresh_recentContact, 120000);
}




function BindSelMember() {
    $("#SelMemberList li a").click(function () {
        $("#WaitMemberList").html($("#WaitMemberList").html() + $(this).html());
        $("#SelMemberList").hide();
    });
}

//初始化功能按钮-------------------------------------------------------
$(function () {


    //返回列表
    $("#btnReturn").click(function () {
        window.parent.ReturnSelMemberByClose();
    });
    //绑定联系人对话
    BindMemberListWin();
    //搜索联系人 
    $("#SelTxt").keyup(function () {
        //什么都没有输入
        if ($.trim($("#SelTxt").val()) == "") {
            $("#SelMemberList").hide();
            return;
        }
        $.post("../../WebIM/GetSelMemberList", { whereStr: $("#SelTxt").val() }, function (data) {
            if (data != "") {
                $("#SelMemberList").html(data).show();
                BindSelMember();
            } else {
                $("#SelMemberList").hide();
            }
        }, "json");


    }).blur(function () {
        // $("#SelMemberList").hide();
    });
    //右边切换
    $("#MemberList").click(function () {
        $(this).addClass("current");
        $("#LatestList").removeClass();
        $("#MemberListDiv").show();
        $("#LatertDiv").hide();
    });
    $("#LatestList").click(function () {
        $(this).addClass("current");
        $("#MemberList").removeClass();
        $("#LatertDiv").show();
        $("#MemberListDiv").hide();
    });


    //清除等待列表
    $("#ClearWaitList").click(function () {
        $("input[type='checkbox']").prop("checked", function (i, val) {
            return false;
        });
        $("#WaitMemberList").html("");
    });
    //确定选中人员
    $("#btnSave").click(function () {
        //处理发送结果
        var receiver = "";
        var receiverName = "";
        if ($(".webim-info-tit-name .current").html() != null) {
            receiver = $(".webim-info-tit-name .current span").attr("id");
            receiverName = $(".webim-info-tit-name .current span").text();
        } else {
            $("#WaitMemberList span").each(function () {
                receiver = receiver + $(this).attr("id") + ",";
                receiverName = receiverName + $(this).text() + ",";
            });
            receiver = receiver.substring(0, receiver.length - 1);
            receiverName = receiverName.substring(0, receiverName.length - 1);

            //去掉[]和最后一个逗号
        }
        if (receiver == "" && receiverName == "") {
            window.parent.ReturnSelMemberByClose();
        } else {
        //返回确定
            window.parent.RetureSelMember(receiver + "^" + receiverName)
        }

    });
});

function BindMemberListWin() {
    $(".webim-info-tit-name .webim-info-tit-thename").unbind();
    //联系人选择
    $(".webim-info-tit-name .webim-info-tit-thename").click(function () {
        if ($(this).parent().attr("class") == "current") {//选中自己，去掉自己样式，准备发送给列表的信息
            $(this).parent().removeClass("current");
            $("#MessagesBox").html("现在您可以使用群发送功能,聊天记录可以查看发送情况！");
        } else { //切换和其他人对话
            $(".webim-info-tit-name li").removeClass("current");
            $(this).parent().addClass("current");
            $("#MessagesBox").html("正在建立对话连接中....请稍后！");
            //读取后台还没有读的信息出来！

            $.post("../../WebIM/GetChatContent"
            , { memberId: $(".webim-info-tit-name .current span").attr("id") }
            , function (data) {
                //发到前台
                $("#MessagesBox").html(data);
                //让滚动条到下方
                $("#MessagesBox").scrollTop(600000);
                //读过了
                if (data != "") {
                    $.post("../../WebIM/SetMessageHasReadByReceiver", { memberId: $(".webim-info-tit-name .current span").attr("id") }, function (data) { }, "json");
                } else {
                    $.post("../../WebIM/GetChatContentIsReaded"
                    , { memberId: $(".webim-info-tit-name .current span").attr("id") }
                    , function (data) {
                        if (data != "") {
                            //发到前台
                            $("#MessagesBox").html(data);
                            //让滚动条到下方
                            $("#MessagesBox").scrollTop(600000);
                        }
                    }, "json");
                }
            }, "json");
        }
        $(this).find("img").attr("src", "../../../../Images/webim/webim-01.gif");
    });
    //联系人的关闭按钮
    $(".webim-info-tit-name .webim-info-tit-name-close").click(function () {
        var thisWin = $(this);
        $.messager.confirm('注意', '您确定要终止此对话吗?', function (r) {
            if (r) {
                thisWin.parent().remove();
                //如果没有当前对话了就提示一些话
                if ($(".webim-info-tit-name .current").html() == null) {
                    $("#MessagesBox").html("现在您可以使用群发送功能,聊天记录可以查看发送情况！")
                }
            }
        });

    });
}

function BindMemberClick() {
    /*
    *   1.组的全选和反选功能
    *   2.如果组没有展开,就先展开之后再进行全选(414代码在tree里面)
    *   3.把全选的组放进发送列表，把取消的表从发送列表移除
    */
    $(".CBGroup").click(function () {//点组的时候 

        var clickShow = $(this);
        if ($("input[ref='" + $(this).attr("id") + "']").size() == 0) { //还没有展开过组
            CBFlag = true; //点后等待全选，或者反选
            CBID = clickShow.attr("id");
            clickShow.parent().parent().parent().find("img").eq(0).trigger("click");

        }
        else {
            $("input[ref='" + $(this).attr("id") + "']").prop("checked", function (i, val) {
                if (clickShow.prop("checked")) {
                    return true;
                } else {
                    return false;
                }
            });
        }
        //处理等待队员列表
        if (clickShow.prop("checked")) {//如果被选中，就添加进发送列表组
            //从组把属下移除，将自己添加进发送列表
            $("input[ref='" + clickShow.attr("id") + "']").each(function (i) {//移除
                $("#WaitMemberList #" + $(this).val() + "").remove();
            });
            //添加组
            $("#WaitMemberList").html($("#WaitMemberList").html() + "<span id=\"_" + clickShow.val() + "\">[+" + clickShow.parent().text() + "]</span>");
        } else { //如果取消，就从发送列表删除组
            $("#WaitMemberList #_" + clickShow.val() + "").remove();
        }

        if ($("#WaitMemberList").html() != "") {
            $(".webim-info-tit-name li").removeClass("current");
            $("#MessagesBox").html("现在您可以使用群发送功能,聊天记录可以查看发送情况！")
        } else {
            $("#MessagesBox").html("现在您可以建立对话模式了！");
        }
    });

    /*
    *   每一次click都执行以下：
    *   1.如果同级元素全部被选中，即组被选中，从列表中移除相关人员，将其组添加进去
    *   2.如果同级有一个或以上没有被选中，即组没有被选中，如果组存在列表中，即被移除，把队员添加进入
    */
    $(".CBMember").click(function () {
        var thisCB = $(this);
        var CBMemberId = $(this).attr("ref");
        //如果选中了就加入
        if (thisCB.prop("checked")) {
            //查看同级是否也是全部选中，如果是全部选中那么移除全部同级包括自己，添加组和让组选中
            $("#WaitMemberList").html($("#WaitMemberList").html() + "<span id=\"" + thisCB.val() + "\">[" + thisCB.parent().text() + "]</span>");
            var count = 0;
            $("input[ref='" + CBMemberId + "']").each(function (i) {
                if ($(this).prop("checked")) {
                    count++;
                }
            });
            if (count == $("input[ref='" + CBMemberId + "']").size()) {//相等就是全选了
                //让组选中
                $("#" + CBMemberId).prop("checked", true);
                //移除这些项，添加他们的组
                $("input[ref='" + CBMemberId + "']").each(function (i) {
                    $("#WaitMemberList #" + $(this).val() + "").remove();
                });
                //添加他们组
                $("#WaitMemberList").html($("#WaitMemberList").html() + "<span id=\"_" + $("#" + CBMemberId).val() + "\">[+" + $("#" + CBMemberId).parent().text() + "]</span>");
            }

        } else {//反选就移出
            //判断是否有存在发送列表中，如果没有，即是组，先移除组，换成人员
            if ($("#WaitMemberList #" + thisCB.val() + "").html() == null) {
                //为空，没有在等待中，组存在,移除组
                $("#WaitMemberList #_" + thisCB.attr("ref") + "").remove();
                //将组转换成组员
                $("input[ref='" + CBMemberId + "']").each(function (i) {
                    if ($(this).prop("checked")) {
                        $("#WaitMemberList").html($("#WaitMemberList").html() + "<span id=\"" + $(this).val() + "\">[" + $(this).parent().text() + "]</span>");
                    }
                });
                //让组不选中
                $("#" + thisCB.attr("ref")).prop("checked", false);
            }
            else {
                $("#WaitMemberList #" + thisCB.val() + "").remove();
            }
        }
        if ($("#WaitMemberList").html() != "") {
            $(".webim-info-tit-name li").removeClass("current");
            $("#MessagesBox").html("现在您可以使用群发送功能,聊天记录可以查看发送情况！");
        } else {
            $("#MessagesBox").html("现在您可以建立对话模式了！");
        }
    });
}
//点最近联系人
function SetWaitListByRecentContact(personId, personName) {
    var arr_id = new Array();
    var arr_name = new Array();
    arr_id = personId.split(",");
    arr_name = personName.split(",");
    var i = 0;
    var str = "";
    for (i; i < arr_id.length; i++) {
        str = str + "<span id=\"" + arr_id[i] + "\">" + arr_name[i] + " </span>";
    }
    $("#CurrentMemberWin li").removeClass("current");
    $("#WaitMemberList").html(str);
}