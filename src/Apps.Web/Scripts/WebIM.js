/*
*
* jquery webim plugins
*
*/
var CBFlag = false;
var CBID = "";
//加载编辑器
var ue = UE.getEditor('content', {
    toolbars: [[
            'fullscreen', 'source',
            'bold', 'italic', 'underline', 'forecolor','insertorderedlist',  
           'fontfamily', 'fontsize',
            'justifyleft', 'justifycenter', 
            'link', 'unlink',
            'simpleupload','snapscreen'
           
            
    ]],
    serverUrl: "/Core/controller.ashx"
});




//前台点常用语后，设置到编辑器
function SetCommmonTalk(mes) {
    $("#CommonTalkList").hide(200);
    ue.setContent(mes)
}



//第一次加载
$(function () {
    $.post("/MIS/WebIM/GetTreeByEasyui", { "id": "0" }, //获取第一层目录
    function (data) {
        $("#MemberListTree").tree({
            data: data,
            onBeforeExpand: function (node, param) {
                $("#MemberListTree").tree('options').url = "/MIS/WebIM/GetTreeByEasyui?id=" + node.id;

            },
            onClick: function (node) {
                if (node.state == 'closed') {
                    $(this).tree('expand', node.target);
                }
            }, onLoadSuccess: function (node) {
                $(".CBGroup").unbind();
                $(".CBMember").unbind();
                BindMemberClick();
                $(".window-mask").hide();
            }
        });
    }, 'json');
    //初始化最近联系人
    //初始化常用语言
    $.post("/WebIM/GetCommonTalk", function (data) { $("#CommonTalkList").html(data); }, "json");

    //定时刷新聊天内容
    refresh_chatcontent();
    //定时刷新在线人数
    refresh_onlineusers();
    //刷新最近联系人
    refresh_recentContact();
    //定时刷新是否有新消息
    refresh_newMessage();
});

//定时刷新当前窗口聊天内容
function refresh_chatcontent() {
    if ($(".webim-info-tit-name .current").html() != null) {
        $.post("/MIS/WebIM/GetChatContent"
        , { memberId: $(".webim-info-tit-name .current span").attr("id") }
        , function (data) {
            if (data != "") {
                //发到前台
                $("#MessagesBox").html($("#MessagesBox").html() + data);
                //让滚动条到下方
                $("#MessagesBox").scrollTop(600000);
                if (data != "")
                    $.post("../../WebIM/SetMessageHasReadByReceiver", { memberId: $(".webim-info-tit-name .current span").attr("id") }, function (data) { }, "json");
            }
        }, "json");
    }
    //定时更新 
    window.setTimeout(refresh_chatcontent, refresh_chatcontentTime);
}
//定时刷新在线人数
function refresh_onlineusers() {
    //定时更新
    return;
    window.setTimeout(refresh_onlineusers, 120000);
}

//定时刷新最近联系人 
function refresh_recentContact() {
    //定时更新
    $.post("../../WebIM/GetRecentContact", function (data) {
        //发到前台
        $("#LatertDiv").html(data);
    }, "json");
    window.setTimeout(refresh_recentContact, refresh_recentContactTime);
}
//定时刷新是否有新消息 15秒
function refresh_newMessage() {
    //调用服务器方法获取最新消息人列表
    $.post("/MIS/WebIM/GetNewMessages", function (data) {
        var dataStr = data.message;
        if (dataStr == "|")//没有任何动静，什么也不需要做了
            return;
        var CurrentMember = "";
        var str = new Array();
        var arr_id = new Array();
        var arr_name = new Array();
        str = dataStr.split("|");
        arr_id = str[0].split(",");
        arr_name = str[1].split(",");
        CurrentMember = $("#CurrentMemberWin").html();
        //读取所有未读信息的人
        for (var i = 0; i < arr_id.length; i++) {
            //第一次加载，把全部读出来，让头闪动
            if (CurrentMember == "") {
                if (arr_id[i] != "") {
                    CurrentMember = CurrentMember
                + "<li><a class=\"webim-info-tit-thename\" href=\"javascript:void(0)\">"
                + "<img src=\"/Content/webim/css/images/webim-01b.gif\" />"
                + "<span id=\"" + arr_id[i] + "\">" + arr_name[i] + "</span></a>"
                + "<a class=\"webim-info-tit-name-close\" href=\"javascript:void(0)\"></a></li>";
                }
            } else { //不是第一次了，更新列表

                //查找现有列表
                var flag = false;
                $("#CurrentMemberWin li").each(function () {
                    //查找是否已经存在
                    if ($(this).find("span").attr("id") == arr_id[i]) {
                        flag = true;
                    }
                });
                if (!flag) { //没有存在这个对话，新添加一个 
                    if (arr_id[i] != "") {//是最后一个
                        CurrentMember = CurrentMember
                            + "<li><a  class=\"webim-info-tit-thename \" href=\"javascript:void(0)\">"
                            + "<img src=\"/Content/webim/css/images/webim-01b.gif\" />"
                            + "<span id=\"" + arr_id[i] + "\">" + arr_name[i] + "</span></a>"
                            + "<a class=\"webim-info-tit-name-close\" href=\"javascript:void(0)\"></a></li>";
                    }
                } else {//有的就表示又来新信息了，闪动头像
                    $("#CurrentMemberWin li img").attr("src", "/Content/webim/css/images/webim-01b.gif");
                }
                //最后让当前的停止闪动
                $("#CurrentMemberWin .current img").attr("src", "/Content/webim/css/images/webim-01.gif");
            }
        }
        $("#CurrentMemberWin").html(CurrentMember);
        BindMemberListWin(); //重新绑定！
        //闪动
        for (var i = 0; i < arr_id.length; i++) {
            if (CurrentMember != "") {
                //查找现有列表
                var flag = false;
                $("#CurrentMemberWin li").each(function () {
                    //查找是否已经存在
                    if ($(this).find("span").attr("id") == arr_id[i]) {
                        flag = true;
                    }
                });
                if (flag) {
                    $("#CurrentMemberWin li img").attr("src", "/Content/webim/css/images/webim-01b.gif");
                }
            }
        }
        //最后让当前的停止闪动
        $("#CurrentMemberWin .current img").attr("src", "/Content/webim/css/images/webim-01.gif");

    }, "json");
    //定时更新
    window.setTimeout(refresh_newMessage, refresh_newMessageTime);
}



function BindSelMember() {
    $("#SelMemberList li a").click(function () {
        $("#WaitMemberList").html($("#WaitMemberList").html() + $(this).html());
        $("#SelMemberList").hide();
    });
}

//初始化功能按钮
$(function () {
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
    //聊天记录
    $("#ChatLog").click(function () {
        $('#ChatLogdialog').dialog({ title: '聊天记录', width: 600, height: 435, resizable: false, collapsible: true, minimizable: false, maximizable: false });
        $('#ChatLogdialog').dialog('open');
        $('#iChatLog').attr("src", "/Mis/WebIM/ChatLog");
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
    //常用语
    $("#CommonTalk").click(function () {
        $("#CommonTalkList").show(200);
    });
    //清屏
    $("#ClearWindow").click(function () {
        $("#MessagesBox").html("");
    });
    //清除等待列表
    $("#ClearWaitList").click(function () {
        $("input[type='checkbox']").prop("checked", function (i, val) {
            return false;
        });
        $("#WaitMemberList").html("");
    });
    //发送新消息
    $("#SendMessage").click(function () {
        if (!ue.hasContents()) {
            $.messager.show({
                title: '注意',
                msg: '不能发送空消息！',
                timeout: 3000,
                showType: 'slide'
            });
            return;
        }
        if ($(".webim-info-tit-name .current").html() == null && $("#WaitMemberList").html() == "") {
            $.messager.show({
                title: '注意',
                msg: '您必须建立一个对话模式，{个人对话}或者{群发功能}！',
                timeout: 3000,
                showType: 'slide'
            });
            return;
        }
        var mydate = new Date();
        //处理发送结果
        var receiver = "";
        var receiverName = "";
        if ($(".webim-info-tit-name .current").html() != null) {
            var str = "<p class='tit'><strong>[我]</strong> <span>" + mydate.getFullYear() + "-" + mydate.getMonth() + "-" + mydate.getDay() + " " + mydate.getHours() + ":" + mydate.getMinutes() + ":" + mydate.getSeconds() + "</span>：</p><p>" + ue.getContent() + "</p>";
            receiver = $(".webim-info-tit-name .current span").attr("id");
            receiverName = "[" + $(".webim-info-tit-name .current span").text() + "]";
        } else {

            $("#WaitMemberList span").each(function () {
                receiver = receiver + $(this).attr("id") + ",";
                receiverName = receiverName + $(this).text() + ",";
            });

            receiver = receiver.substring(0, receiver.length - 1);
            receiverName = receiverName.substring(0, receiverName.length - 1);
            var str = "<p class='tit'><strong>[我] 对 ｛" + receiverName + "｝ 广播</strong> <span>" + mydate.getFullYear() + "-" + mydate.getMonth() + "-" + mydate.getDay() + " " + mydate.getHours() + ":" + mydate.getMinutes() + ":" + mydate.getSeconds() + "</span>：</p><p>" + ue.getContent() + "</p>";
            //去掉[]和最后一个逗号
        }

        //发到前台
        $("#MessagesBox").html($("#MessagesBox").html() + str);
        //让滚动条到下方
        $("#MessagesBox").scrollTop(600000);
        //添加到数据库
        $.post("/MIS/WebIM/SendMessage",
                         {
                             "mes": ue.getContent()
                         , "receiver": receiver
                         , "receiverName": receiverName
                         , 'state': true
                         },
                        function (data) {
                            if (!data)
                                $.messager.show({
                                    title: '提示：',
                                    msg: data,
                                    timeout: 5000,
                                    showType: 'slide'
                                });
                        }, "json");
        //清除消息框
        ue.setContent("");
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

            $.post("/MIS/WebIM/GetChatContent"
            , { memberId: $(".webim-info-tit-name .current span").attr("id") }
            , function (data) {
                //发到前台
                $("#MessagesBox").html(data);
                //让滚动条到下方
                $("#MessagesBox").scrollTop(600000);
                //读过了
                if (data != "") {
                    $.post("/MIS/WebIM/SetMessageHasReadByReceiver", { memberId: $(".webim-info-tit-name .current span").attr("id") }, function (data) { }, "json");
                } else {
                    $.post("/MIS/WebIM/GetChatContentIsReaded"
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
        $(this).find("img").attr("src", "/Content/webim/css/images/webim-01.gif");
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