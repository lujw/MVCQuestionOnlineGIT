var jsonNav = category;
//如果网站定义在子目录，需修改此处，eg:/blog/
var urlPath = WebLang.Lang == currentLang ? "/" : "/" + WebLang.Lang + "/";
//无限极菜单
function ViewNav(parentid, layer) {
    var layer = layer + 1;
    var navItemStr;
    var currentParent;
    var currentItem;

    if (parentid == 0) {
        currentParent = $(".menu");
        currentParent.append("<ul class=\"topnav\"><li id=\"Nav_0\" v=\"0\"><a href=\"" + urlPath + "\">" + WebLang.Home + "</a></li></ul>");
    }
    else {
        currentParent = $("#Nav_" + parentid);
        if (layer == 2)
        { currentParent.append("<ul class=\"subnav\" id=\"PNav_" + parentid + "\"></ul>"); }
        else
        { currentParent.append("<ul class=\"subnav2\" id=\"PNav_" + parentid + "\"></ul>"); }
    }
    currentParent = currentParent.find("ul");

    //循环取json中的数据
    $.each(jsonNav, function (i) {

        if ((jsonNav[i].IsNav == 1) && (jsonNav[i].ParentId == parentid)) {
            //添加
            var urlstr = urlPath+"cate/" + jsonNav[i].CateId;
            if (jsonNav[i].ReName != "")
            { urlstr = urlPath + jsonNav[i].ReName+"/"; }
            navItemStr = "<li id=\"Nav_" + jsonNav[i].CateId + "\" v=\"" + jsonNav[i].CateId + "\"><a href=\"" + urlstr + "\">" + GetCateName(jsonNav[i].CateId, jsonNav[i].CateName) + "</a></li>";
            currentParent.append(navItemStr)
            currentItem = $("#Nav_" + jsonNav[i].CateId);

            //有子栏目时添加下拉样式标签以及hasSub样式（用于显示子栏目的操作）
            if (jsonNav[i].SubCount > 0 && jsonNav[i].Type != "5") {
                if (layer == 1)
                { currentItem.append("<span></span>"); }
                else {
                    currentItem.find("a").css({ "background-position": "145px 15px" });
                }
                currentItem.addClass("hasSub");
                ViewNav(jsonNav[i].CateId, layer);
            }

            if (layer > 2)
            { currentItem.find("a").css({ "float": "left", "width": "150px" }); }
        }
    });
}

//根据cateid查询catename
function GetCateName(cateid, catename) {
    if (WebLang.Lang == "") { return catename; }
    var t = "";
    $.each(categorylang, function (i) {
        if (categorylang[i].CateId == cateid) { t = categorylang[i].CateName; return; }

    });
    if (t == "") { t = catename; }
    return t;
}

//文章列表修饰图异步加载
//function LoadSummaryImg() {
//    if (summaryimgs) {
//        $.each(summaryimgs, function (i, n) {
//            $("#simg_" + n["id"]).html("<img src=\"" + n["img"] + "\"/>");
//        });
//    }
//}

//评论提交成功后执行
function CommentSubmitSuccess(re) {
    if (re == 0 || re == 2) {
        if (re == 2) {
            var replyid = $("#ReplyId").val();
            var node = $("#" + replyid);
            node.find(".replycontent").html(UBBToHtml($("#Reply").val()));
            var curl = window.location.href;
            curl = curl.substr(0, curl.indexOf('#'));
            window.location.href = "#"+replyid;
            $("#ReplyId").val("0");
         }
        else {
            var userinfo = $('#UserName').val();
            if ($('#Url').val().replace(/\s/g, "") != "") {
                userinfo = "<a href=\"" + $('#Url').val() + "\" target=\"_blank\">" + $('#UserName').val() + "</a>"
            }
            $(".commentlist ul").append("<li><p>" + userinfo + " " + GetCurrentTime() + "</p>" + UBBToHtml($("#Reply").val()) + "</li>");
        }
        $("#Reply").val('');
        $('#AjaxResult').show().html(WebLang.AjaxSuccess).hide(3000);
    }
    else {
        $('#AjaxResult').show().html(WebLang.AjaxFaild).hide(3000);
    }
}

//加载日历
function initArticleCalendar() {
    var datepickerYear = $(".ui-datepicker-year").html();

    //英文月份：alert($.datepicker.regional[''].monthNames);
    var datepickerMonth;
    if (WebLang.Lang == "" || WebLang.Lang == "zh-tw" || WebLang.Lang == "zh-cn") {
        datepickerMonth = $.inArray($(".ui-datepicker-month").html(), $.datepicker.regional['zh-CN'].monthNames) + 1;
    }
    if (WebLang.Lang == "en-us") {
        datepickerMonth = $.inArray($(".ui-datepicker-month").html(), $.datepicker.regional[''].monthNames) + 1;
    }

    $(".ui-datepicker-calendar a").each(function (index, domEle) {

        var datepickerDay = $(this).html();
        var datepickerDate = datepickerYear + "-" + datepickerMonth + "-" + datepickerDay;

        if ($.inArray(datepickerDate, strArticleDates) > -1) {
            $(this).parent().attr("onclick", "null");
            $(this).attr("href", urlPath + "Archives/" + datepickerDate + "/").css({ "background": "#680707" });
        }
        else {
            $(this).parent().attr("onclick", "function(){return false;}").html("<div class=\"ui-state-word\">" + datepickerDay + "</div>");
        }
    });
}

$(function () {
    //导航显示子栏目
    $(".hasSub").hover(function () {
        $(this).find("ul").first().show();
        $(this).addClass("subhover");
    }, function () {
        $(this).children("ul").hide();
        $(this).removeClass("subhover");
    });

    //登录信息
    //    $.get(urlPath + "Account/CheckLogin/", { rn: Math.random() }, function (re) {
    //        $("#logindisplay").html(re);
    //    })

    //推荐
    $(".digg_favor").click(function () {
        $.ajax({
            url: urlPath + "home/AjaxVote/",
            type: "post",
            data: { articleid: ArticleId, vote: 1 },
            dataType: 'json',
            success: function (re) {
                $(".digg_tip").html(re["message"]);
                if (re["error"] == 0)
                { $(".digg_favor").html(re["value"]); }
            }
        });
    });

    //反对
    $(".digg_against").click(function () {
        $.ajax({
            url: urlPath + "home/AjaxVote/",
            type: "post",
            data: { articleid: ArticleId, vote: 0 },
            dataType: 'json',
            success: function (re) {
                $(".digg_tip").html(re["message"]);
                if (re["error"] == 0)
                { $(".digg_against").html(re["value"]); }
            }
        });
    });

    //评论翻页
    $(".commentpager a").live("click", function () {
        GetComment($(this).attr("p"));
    });

    //引用
    $(".reply-quote").live("click", function () {
        var curl = window.location.href;
        curl = curl.substr(0, curl.indexOf('#'));
        window.location.href = "#commentform";
        var node = $(this).parent().parent();
        var replyid = node.attr("id");
        var uname = node.attr("u");
        var replycontent = node.find(".replycontent").html();
        var txt = $("#Reply").val();
        //txt = txt + "<fieldset class=\"comment_quote\"><legend><a title=\"" + WebLang.ViewContent + "\" href=\"" + curl + "#" + replyid + "\">" + WebLang.Quote + "</a></legend>" + uname + "：<br>" + replycontent + "</fieldset>";
        txt = "@" + uname + "\r\n" + txt + "[quote]" + uname + "：\r\n" + HtmlToUBB(replycontent) + "[/quote]\r\n";
        $("#Reply").focus().val(txt);
    });

    //回复
    $(".reply-re").live("click", function () {
        window.location.href = "#commentform";
        var node = $(this).parent().parent();
        var uname = node.attr("u");
        var txt = $("#Reply").val();
        txt = "@" + uname + "\r\n" + txt;
        $("#Reply").focus().val(txt);
    });

    //评论/留言删除
    $(".reply-del").live("click", function () {
        if (confirm(WebLang.DelTip)) {
            var node = $(this).parent().parent();
            var replyid = node.attr("id");
            $.ajax({
                url: "/home/UserArticleDel/",
                type: "post",
                data: { id: replyid },
                success: function (re) {
                    if (re == "0") {
                        alert(WebLang.DelSuccess);
                        $('#' + replyid).hide();
                    }
                    else {
                        alert(WebLang.OperationFailed);
                    }
                }
            });
        }
    });

    //留言修改
    $(".note-update").live("click", function () {
        var node = $(this).parent().parent();
        var noteid = node.attr("id");
        var txt = node.find(".replycontent").html();
        $('#Reply').focus().val(HtmlToUBB(txt));
        $("#NoteId").val(noteid);
    });

    //评论修改
    $(".reply-update").live("click", function () {
        var node = $(this).parent().parent();
        var replyid = node.attr("id");
        var txt = node.find(".replycontent").html();
        $('#Reply').focus().val(HtmlToUBB(txt));
        $("#ReplyId").val(replyid);
    });

    //搜索
    $(".search_button").click(function () {
        window.location.href = urlPath + "search/" + $("#searchkey").val().replace(/\./g, "");
    });

    //语言/风格切换
    $(".dropmenu").live({
        mouseenter:
        function () {
            $(this).find(".dropmenu-list").show();
            $(this).find(".dropmenu-title").addClass("dropmenutitlehover");
        },
        mouseleave:
        function () {
            $(this).find(".dropmenu-list").hide();
            $(this).find(".dropmenu-title").removeClass("dropmenutitlehover");
        }
    });
    $(".dropmenu-list a").live({
        mouseenter:
        function () {
            $(this).addClass("langlihover");
        },
        mouseleave:
        function () {
            $(this).removeClass("langlihover");
        }
    });

    //不同模板下图片路径
    var currentThemePath = "/Content/image/";

    if (currentTheme != "") {
        currentThemePath = "/Themes/" + currentTheme + "/Content/image/";
    }

    //装饰图延迟加载
    $(".post_image img").lazyload({
        placeholder: currentThemePath + "decorate/default.jpg",
        effect: "fadeIn"
    });
});


//异步读取评论
function GetComment(pageno) {
    $('#comment').html('loading...');
    $.get(urlPath + "home/CommentList/", { id: ArticleId, pageNo: pageno, rn: Math.random() }, function (re) {
        $("#comment").html(re);
    })
}

//取得当前时间
function GetCurrentTime() {
    var date = new Date();
    var now = "";
    now = date.getFullYear() + "-";
    now = now + (date.getMonth() + 1) + "-";
    now = now + date.getDate() + " ";
    now = now + date.getHours() + ":";
    now = now + date.getMinutes() + ":";
    now = now + date.getSeconds();
    return now;
}

//HtmlToUBB
function HtmlToUBB(str) {
    str = str.replace(/\r/g, "");
    str = str.replace(/on(load|click|dbclick|mouseover|mousedown|mouseup)="[^"]+"/ig, "");
    str = str.replace(/<script[^>]*?>([\w\W]*?)<\/script>/ig, "");
    str = str.replace(/<a[^>]+href="([^"]+)"[^>]*>(.*?)<\/a>/ig, "[url=$1]$2[/url]");
    str = str.replace(/<font[^>]+color=([^ >]+)[^>]*>(.*?)<\/font>/ig, "[color=$1]$2[/color]");
    str = str.replace(/<img[^>]+src="([^"]+)"[^>]*>/ig, "[img]$1[/img]");
    str = str.replace(/<fieldset.*?<\/legend>(.*?)<\/fieldset>/ig, "[quote]$1[/quote]");
    str = str.replace(/<([\/]?)b>/ig, "[$1b]");
    str = str.replace(/<([\/]?)strong>/ig, "[$1b]");
    str = str.replace(/<([\/]?)u>/ig, "[$1u]");
    str = str.replace(/<([\/]?)i>/ig, "[$1i]");
    str = str.replace(/ /g, " ");
    str = str.replace(/&/g, "&");
    str = str.replace(/"/g, "\"");
    str = str.replace(/</g, "<");
    str = str.replace(/>/g, ">");
    str = str.replace(/<br>/ig, "\r\n");
    str = str.replace(/<br\/>/ig, "\r\n");
    str = str.replace(/<[^>]*?>/g, "");
    str = str.replace(/\[url=([^\]]+)\]\n(\[img\]\1\[\/img\])\n\[\/url\]/g, "$2");
    str = str.replace(/\n+/g, "\n");
    return str;
}

//UBBToHtml
function UBBToHtml(_strers) {
    var tstrers = _strers;
    if (tstrers) {
        var tthis = this;
        var tReplaceAry = [
        [/\r/igm, '', true],
        [/\n/igm, '<br/>', true],
        [/\[br]/igm, '<br/>', true],
        [/\[br\/]/igm, '<br/>', true],
        [/\[p\]([^\[]*?)\[\/p\]/igm, '<p>$1</p>', true],
        [/\[b\]([^\[]*?)\[\/b\]/igm, '<b>$1</b>', true],
        [/\[i\]([^\[]*?)\[\/i\]/igm, '<i>$1</i>', true],
        [/\[u\]([^\[]*?)\[\/u\]/igm, '<u>$1</u>', true],
        [/\[ol\]([^\[]*?)\[\/ol\]/igm, '<ol>$1</ol>', true],
        [/\[ul\]([^\[]*?)\[\/ul\]/igm, '<ul>$1</ul>', true],
        [/\[li\]([^\[]*?)\[\/li\]/igm, '<li>$1</li>', true],
        [/\[code\]([^\[]*?)\[\/code\]/igm, '<div class="ubb_code">$1</div>', true],
        [/\[quote\]([^\[]*?)\[\/quote\]/igm, '<fieldset class="comment_quote"><legend> ' + WebLang.Quote + ' </legend>$1</fieldset>', true],
        [/\[color=([^\]]*)\]([^\[]*?)\[\/color\]/igm, '<font style="color: $1">$2</font>', true],
        [/\[hilitecolor=([^\]]*)\]([^\[]*?)\[\/hilitecolor\]/igm, '<font style="background-color: $1">$2</font>', true],
        [/\[align=([^\]]*)\]([^\[]*?)\[\/align\]/igm, '<p align="$1">$2</p>', true],
        [/\[url=([^\]]*)\]([^\[]*?)\[\/url\]/igm, '<a href="$1">$2</a>', true],
        [/\[img\]([^\[]*?)\[\/img\]/igm, '<img src="$1" />', true]
      ];
        tstrers = this.tHTMLEncode(tstrers);
        tstrers = this.tReplace(tstrers, tReplaceAry);
    };
    return tstrers;
};

function tHTMLEncode(_strers) {
    var tstrers = _strers;
    if (tstrers) {
        tstrers = tstrers.replace(/&/igm, '&amp;');
        tstrers = tstrers.replace(/</igm, '&lt;');
        tstrers = tstrers.replace(/>/igm, '&gt;');
        tstrers = tstrers.replace(/\"/igm, '&quot;');
        tstrers = tstrers.replace(/ /igm, '&nbsp;');
        tstrers = tstrers.replace(/&amp;#91;/igm, '&#91;');
        tstrers = tstrers.replace(/&amp;#93;/igm, '&#93;');
    };
    return tstrers;
};

function tReplace(_strers, _reary, _ign) {
    var tstrers = _strers;
    var treary = _reary;
    var tign = _ign;
    var tstate1 = true;
    for (var ti = 0; ti < treary.length; ti++) {
        if (!treary[ti][2]) tstrers = tstrers.replace(treary[ti][0], (tign ? '' : treary[ti][1]));
    };
    while (tstate1) {
        tstate1 = false;
        for (var ti = 0; ti < treary.length; ti++) {
            if (treary[ti][2] && tstrers.search(treary[ti][0]) != -1) {
                tstate1 = true;
                tstrers = tstrers.replace(treary[ti][0], (tign ? '' : treary[ti][1]));
            };
        };
    };
    return tstrers;
};

var BROWSER = {};

function isUndefined(variable) {
    return typeof variable == 'undefined' ? true : false;
}

function strlen(str) {
    return (BROWSER.ie && str.indexOf('\n') != -1) ? str.replace(/\r?\n/g, '_').length : str.length;
}

function doane(event) {
    e = event ? event : window.event;
    if (!e) return;
    if (BROWSER.ie) {
        e.returnValue = false;
        e.cancelBubble = true;
    } else if (e) {
        e.stopPropagation();
        e.preventDefault();
    }
}

function seditor_insertunit(obj, text, textend, moveend) {
    var tar = $("#" + obj);
    tar.focus();
    textend = isUndefined(textend) ? '' : textend;
    moveend = isUndefined(textend) ? 0 : moveend;
    startlen = strlen(text);
    endlen = strlen(textend);
    if (!isUndefined(tar.selectionStart)) {
        var opn = tar.selectionStart + 0;
        if (textend != '') {
            text = text + tar.value.substring(tar.selectionStart, tar.selectionEnd) + textend;
        }
        tar.value = tar.value.substr(0, tar.selectionStart) + text + tar.value.substr(tar.selectionEnd);
        if (!moveend) {
            tar.selectionStart = opn + strlen(text) - endlen;
            tar.selectionEnd = opn + strlen(text) - endlen;
        }
    } else if (document.selection && document.selection.createRange) {
        var sel = document.selection.createRange();
        if (textend != '') {
            text = text + sel.text + textend;
        }
        sel.text = text.replace(/\r?\n/g, '\r\n');
        if (!moveend) {
            sel.moveStart('character', -endlen);
            sel.moveEnd('character', -endlen);
        }
        sel.select();
    } else {
        tar.value += text;
    }
    if (BROWSER.ie) {
        doane();
    }
}