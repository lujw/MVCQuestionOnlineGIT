//后台，图片上传成功后执行
function UploadResultFun(file, data, response) {
    //alert('The file ' + file.name + ' was successfully uploaded with a response of ' + response + ':' + data);
    var jsonRe = eval("(" + data + ")");
    if (jsonRe["err"] != "") {
        $("#retip").append(file.name + " " + jsonRe["err"]);
    }
    else {
        var t = "<li>";
        t = t + "<img src=\"" + jsonRe['msg']['url'] + "\" />";
        t = t + "<input type=\"text\" value=\"" + (file.name).substr(0, file.name.lastIndexOf('.')) + "\" />";
        t = t + "<input type=\"text\" value=\"\" />";
        t = t + "<input type=\"radio\" class=\"iscoverbtn\" name=\"IsCover\" />";
        t = t + "<a class=\"albumphotodel\" />";
        t = t + "</li>";

        $(".photobox").append(t);
    }
}

//后台，图片上传保存时遍历对象，生成图片信息的json格式字符串
function GetPhotos() {
    var jsonPhoto = "[";
    $(".photobox li").each(function (index, domEle) {
        if ($(this).html() != "" && $(this).css("display") != "none") {
            jsonPhoto = jsonPhoto + "{\"Src\":\"" + $($(this).children()[0]).attr('src') + "\",";
            jsonPhoto = jsonPhoto + "\"Title\":\"" + $($(this).children()[1]).val().replace(/\"/g, '\'') + "\",";
            jsonPhoto = jsonPhoto + "\"Alt\":\"" + $($(this).children()[2]).val().replace(/\"/g, '\'') + "\",";
            jsonPhoto = jsonPhoto + "\"IsCover\":\"" + ($($(this).children()[3]).attr('checked') ? '1' : '0') + "\"},";
        }
    });

    if (jsonPhoto.length > 1) {
        jsonPhoto = jsonPhoto.substr(0, jsonPhoto.length - 1);
    }
    jsonPhoto = jsonPhoto + "]";

    $("#Content").val(jsonPhoto);
}

//后台，初始化相册图片列表
function InitialPhotos() {
    var jsonPhoto = eval("(" + $("#Content").val() + ")");
    $.each(jsonPhoto, function (i) {
        var t = "<li>";
        t = t + "<img src=\"" + jsonPhoto[i]['Src'] + "\" />";
        t = t + "<input type=\"text\" value=\"" + jsonPhoto[i]['Title'] + "\" />";
        t = t + "<input type=\"text\" value=\"" + jsonPhoto[i]['Alt'] + "\" />";
        t = t + "<input type=\"radio\" class=\"iscoverbtn\" name=\"IsCover\" " + (jsonPhoto[i]['IsCover'] == "1" ? " checked=\"checked\"" : "") + "/>";
        t = t + "<a class=\"albumphotodel\" />";
        t = t + "</li>";
        $(".photobox").append(t);
    });
}

//替换掉编辑器自动加的换行
function ReplaceKESpace(str) {
    var re = /(<p>(\s|\s*&nbsp;\s*|\s*<br\s*\/?\s*>\s*)*<\/p>)+/ig;
    var re2 = /((\s|\s*&nbsp;\s*|\s*<br\s*\/?\s*>\s*)*)+/ig;
    var newstr = str;
    if (newstr.replace(re, "").replace(re2, "") == "") {
        newstr = "";
    }
    else {
        newstr = newstr.replace(re, "<br/>");
    }
    return newstr;
}

$(function () {
    //后台，切换图片展示样式
    $(".photoDisplayChange a").click(function () {
        switch ($(this).html()) {
            case '4X':
                $(".photoUpload ul li").css({ "width": "185px", "height": "180px" });
                $(".photoUpload ul li img").css({ "height": "140px" });
                break;
            case '7X':
                $(".photoUpload ul li").css({ "width": "94px", "height": "110px" });
                $(".photoUpload ul li img").css({ "height": "70px" });
                break;
            case '10X':
                $(".photoUpload ul li").css({ "width": "55px", "height": "80px" });
                $(".photoUpload ul li img").css({ "height": "40px" });
                break;
        }
    });

    //后台，删除相册图片
    $(".albumphotodel").live("click", function () {
        if (confirm(WebLang.DelTip)) {
            $(this).parent().css({ "display": "none" });
        }
    });

});