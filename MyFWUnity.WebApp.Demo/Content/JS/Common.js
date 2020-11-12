
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

// 全部替换
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
}

//数值转换金额格式
Number.prototype.formatMoney = function (places, symbol, thousand, decimal) {
    places = !isNaN(places = Math.abs(places)) ? places : 2;
    symbol = symbol !== undefined ? symbol : "";
    thousand = thousand || ",";
    decimal = decimal || ".";
    var number = this,
        negative = number < 0 ? "-" : "",
        i = parseInt(number = Math.abs(+number || 0).toFixed(places), 10) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
};

function operateDateFormatter(value, row, index) {
    if (value == "0001-01-01T00:00:00") { return "" }
    return new Date(value).Format("yyyy-MM-dd hh:mm:ss");
}
function operateYesOrNoFormatter(value, row, index) {

    return value ? "<span style='color:red'>是</span>" : "否";
}
function operateTimeStampDateFormatter(value, row, index) {
    return getFormatDate(value);
}

function randomString(len, charSet) {
    charSet = charSet || 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var randomString = '';
    for (var i = 0; i < len; i++) {
        var randomPoz = Math.floor(Math.random() * charSet.length);
        randomString += charSet.substring(randomPoz, randomPoz + 1);
    }
    return randomString;
}
function toThousands(num) {
    if (num > 999) {
        var result = [], counter = 0;
        num = (num || 0).toString().split('');
        for (var i = num.length - 1; i >= 0; i--) {
            counter++;
            result.unshift(num[i]);
            if (!(counter % 3) && i != 0) { result.unshift(','); }
        }
        return result.join('');
    }
    else {
        return num;
    }
}



var customquery = function (params) {
    var temp = {
        condition: ((searchval == undefined) ? "" : searchval),//自定义查询   Condition: params.searchText == undefined ? "" : params.searchText, 自带查询
        pageSize: params.pageSize,
        pageIndex: params.pageNumber,
    };
    return temp;
};
function getFormatDate(timestamp) {
    timestamp = parseInt(timestamp + '000');
    var newDate = new Date(timestamp);
    Date.prototype.format = function (format) {
        var date = {
            'M+': this.getMonth() + 1,
            'd+': this.getDate(),
            'h+': this.getHours(),
            'm+': this.getMinutes(),
            's+': this.getSeconds(),
            'q+': Math.floor((this.getMonth() + 3) / 3),
            'S+': this.getMilliseconds()
        };
        if (/(y+)/i.test(format)) {
            format = format.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
        }
        for (var k in date) {
            if (new RegExp('(' + k + ')').test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1
                    ? date[k] : ('00' + date[k]).substr(('' + date[k]).length));
            }
        }
        return format;
    }
    return newDate.format('yyyy-MM-dd h:m');
}
//########bootstrtable 扩展
var tableextend = new TableExtend();
var searchval = "";
var AddTable = function (option) {
    var config = $.extend({}, AddTable.Config, option);
    if (config) {
        tableextend.addtable(config.tableName, function (toolshtml, that) {
            if (config.isSearchText) {
                toolshtml.push('<input id="searchText" class="search-text" style="float:left;" placeholder="' + config.placeholder + '" />');
            }
            if (config.isAdd) {
                toolshtml.push(sprintf('<button id="add"  class="btn btn-default table-btn' +
                    (that.options.iconSize === undefined ? '' : ' btn-' + that.options.iconSize) +
                    '" type="button" name="add" title="%s">', that.options.formatAdd()),
                    '<span class="fa fa-plus-square" style="top: 3px;margin-right: 6px;"></span>',
                    sprintf('%s', that.options.formatAdd()), '</button>');
            }
            if (typeof (config.toolshtmlCallback) == "function") {
                config.toolshtmlCallback(toolshtml, that);
            }
        }, function (that) {
            if (config.isSearchText) {
                that.$toolbar.find("#searchText").off("keyup").on("keyup", function () {
                    searchval = $(this).val();
                    $('#' + config.tableName).bootstrapTable('refresh');
                })
            }
            if (config.isAdd) {
                that.$toolbar.find('button[name="add"]')
                    .off('click').on('click', function () {
                        if (typeof (config.addCallback) == "function") {
                            config.addCallback();
                        }
                    });
            }
            if (typeof (config.addtoolslaterCallback) == "function") {
                config.addtoolslaterCallback(that);
            }
        });
    }
}
AddTable.Config = {
    tableName: "table",
    placeholder: "搜索",
    isSearchText: true,
    isAdd: true,
    addCallback: function () {
    },
    toolshtmlCallback: function (toolshtml, that) {

    },
    addtoolslaterCallback: function (that) {

    }
};
//##############bootstrtable operate操作栏扩展事件

function operateFormatter(value, row, index) {
    return WOEvents.Formatter(value, row, index).join('');
}

var WOEvents = {};
WOEvents.AddEvent = function (_c, callback) {
    window.operateEvents[_c] = callback;
}
WOEvents.Update = function (row, e) {

}
WOEvents.Delete = function (row, e) {

}
WOEvents.Formatter = function (value, row, index) {
    var str = [];
    str.push('<a href="javascript:;" title="修改" class="btn btn-circle btn-primary update"><i class="fa fa-pencil-square-o"></i></a>');
    str.push('<a href="javascript:;" title="删除"  class="btn btn-circle btn-danger delete"><i class="fa fa-trash"></i></a>');
    str = str.concat(WOEvents.AddFormatter(value, row, index));
    return str;
}
WOEvents.AddFormatter = function (value, row, index) {
    var str = [];
    return str;
}
window.operateEvents = {
    'click .update': function (e, value, row, index) {
        e.stopPropagation();
        e.preventDefault();
        WOEvents.Update(row, this);
    },
    'click .delete': function (e, value, row, index) {
        e.stopPropagation();
        e.preventDefault();
        WOEvents.Delete(row, this);
    }

}

function operateUserFormatter(value, row, index) {
    var str = [];
    if (value != undefined) {
        $.each(value, function (key, item) {
            str.push('<a href="javascript:;" onclick="getUserInfo(this)" data-id="' + key + '" style="text-decoration: underline;color: #1ABB9C;">' + item + '</a>');
        });
    }
    return str.join('');
}

function operateTextFormatter(value, row, index) {
    return '<textarea style="width: 100%;border: none;"  disabled="disabled">' + value + '</textarea>';
}

function getUserInfo(e) {
    MyFWUnity.CustomCommon.SwalLoad("加载数据中，请稍后");
    var id = $(e).data("id");
    AjaxCustom.getAjax("/api/User/GetUserInfoByID/" + id, function (data) {
        MyFWUnity.CustomCommon.SwalClose();
        $("#UserInfoModal").find(".UserFace").attr("src", MyFWUnity.CustomCommon.ParseUndefined(data.UserFace, "/Content/images/h.png"));
        $("#UserInfoModal").find(".UserName").text(MyFWUnity.CustomCommon.ParseUndefined(data.UserName));
        $("#UserInfoModal").find(".TrueName").text(MyFWUnity.CustomCommon.ParseUndefined(data.TrueName));
        $("#UserInfoModal").find(".Sex").text(MyFWUnity.CustomCommon.ParseUndefined(data.Sex));
        $("#UserInfoModal").find(".Company").text(MyFWUnity.CustomCommon.ParseUndefined(data.Company));
        $("#UserInfoModal").find(".Department").text(MyFWUnity.CustomCommon.ParseUndefined(data.Department));
        $("#UserInfoModal").find(".Title").text(MyFWUnity.CustomCommon.ParseUndefined(data.Title));
        $("#UserInfoModal").find(".DisciplineName").text(MyFWUnity.CustomCommon.ParseUndefined(data.DisciplineName));
        $("#UserInfoModal").find(".Phone").text(MyFWUnity.CustomCommon.ParseUndefined(data.Phone));
        $("#UserInfoModal").find(".LandLine").text(MyFWUnity.CustomCommon.ParseUndefined(data.LandLine));
        $("#UserInfoModal").find(".Email").text(MyFWUnity.CustomCommon.ParseUndefined(data.Email));
        $("#UserInfoModal").find(".QQ").text(MyFWUnity.CustomCommon.ParseUndefined(data.QQ));
        $("#UserInfoModal").modal("show");
    });
}


//WOEvents.AddFormatter测试
//WOEvents.AddFormatter = function (value, row, index) {
//    var str = [];
//    str.push('<a href="javascript:;" title="测试" class="btn btn-circle btn-primary test"><i class="fa fa-pencil-square-o"></i></a>');
//    return str;
//}
//WOEvents.AddEvent("click .test", function (e, value, row, index) {
//    alert(JSON.stringify(row));
//})


var MyFWUnity = {};
MyFWUnity.CustomCommon = (function () {
    var GetPostData = function (formId, _data) {
        var form = document.getElementById(formId);
        var data = new Object();
        if (form != null) {
            var tagElements = form.getElementsByTagName('input');
            for (var j = 0; j < tagElements.length; j++) {
                if (tagElements[j].type == "checkbox") {
                    data[tagElements[j].name] = tagElements[j].checked ? tagElements[j].dataset.on : tagElements[j].dataset.off;
                }
                else {
                    data[tagElements[j].name] = tagElements[j].value;
                }
            }
            var tagElement_textareas = form.getElementsByTagName('textarea');
            for (var j = 0; j < tagElement_textareas.length; j++) {
                data[tagElement_textareas[j].name] = tagElement_textareas[j].value;
            }

            var tagElement_select = form.getElementsByTagName('select');
            for (var j = 0; j < tagElement_select.length; j++) {
                data[tagElement_select[j].name] = tagElement_select[j].value;
            }

        }
        if (_data) {
            $.each(_data, function (key, item) {
                data[key] = item;
            });
        }
        return data;
    };
    var GetFormData = function (formId) {
        var form = document.getElementById(formId);
        var data = new FormData();
        if (form != null) {
            var tagElements = form.getElementsByTagName('input');
            for (var j = 0; j < tagElements.length; j++) {
                if (tagElements[j].type == "checkbox") {
                    if (tagElements[j].name != "") {
                        data.append(tagElements[j].name, tagElements[j].checked ? tagElements[j].dataset.on : tagElements[j].dataset.off);
                    }
                }
                else {
                    if (tagElements[j].name != "") {
                        data.append(tagElements[j].name, tagElements[j].value);
                    }
                }
            }
            var tagElement_textareas = form.getElementsByTagName('textarea');
            for (var j = 0; j < tagElement_textareas.length; j++) {
                if (tagElements[j].name != "") {
                    data.append(tagElement_textareas[j].name, tagElement_textareas[j].value);
                }
            }

            var tagElement_select = form.getElementsByTagName('select');
            for (var j = 0; j < tagElement_select.length; j++) {
                if (tagElements[j].name != "") {
                    data.append(tagElement_select[j].name, tagElement_select[j].value);
                }
            }

        }
        return data;
    };
    var AddOrUpdate = function (formId, controller, _data, callback) {
        var flag = $("#" + formId).valid();
        if (!flag) {
            //没有通过验证
            return;
        }
        var data = GetPostData(formId, _data);
        AjaxCustom.postAjax("/api/" + controller + "/AddOrUpdate", data, callback);
    }

    var AddOrUpdateByFormData = function (formId, controller, $fileId, callback) {
        var flag = $("#" + formId).valid();
        if (!flag) {
            //没有通过验证
            return;
        }
        var data = GetFormData(formId);
        if ($fileId) {
            var files = $fileId.uploadfile("getfiledata");
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++) {
                    data.append(files.name, files[i]);
                }
            }
        }
        AjaxCustom.formDataAjax("/api/" + controller + "/AddOrUpdateByFormData", data, callback);
    }
    var AddsAndDeletes = function (controller, addDatas, deleteIds, callback) {
        var data = {
            list: JSON.stringify(addDatas),
            ids: deleteIds.join(',')
        }
        AjaxCustom.postAjax("/api/" + controller + "/AddsAndDeletes", data, callback);
    }
    var AddAndDelete = function (controller, addData, deleteId, callback, isAlert) {
        if (isAlert == undefined) {
            isAlert = true;
        }
        var data = {
            source: JSON.stringify(addData),
            id: deleteId,
            isAlert: isAlert
        }
        AjaxCustom.postAjax("/api/" + controller + "/AddAndDelete", data, callback);
    }


    var Delete = function (controller, ID, callback) {
        AjaxCustom.deleteConfirmAjax("/api/" + controller + "/Remove/" + ID, callback);
    }

    var InitProjectAll = function ($select, change) {
        AjaxCustom.getAjax("/api/Project/All", function (data) {
            $select.empty();
            var html = '';
            $.each(data, function (key, item) {
                html += '<option value="' + item.ID + '">' + item.Name + '</option>'
            });
            $select.html(html);
            if (typeof change == "function") {
                $select.off("change").on("change", change);
            }
        });
    }

    var SwalLoad = function (title) {
        swal({
            title: title,
            text: '',
            imageUrl: '/Content/Images/loading_Spinner.gif',
            imageWidth: 100,
            imageHeight: 100,
            animation: false,
            showConfirmButton: false,
            width: 250,
            allowOutsideClick: false,
            allowEscapeKey: false,
            background: '#F3F3F3',
        });


    }
    var SelectMultipleParseValue = function (value) {
        if (value) {
            if (value.indexOf(',') > -1) {
                return value.split(',');
            }
        }
        return value;
    }
    var SwalClose = function () {
        swal.close();
    }

    var ParseUndefined = function (data, setData) {
        var s = "";
        if (setData) {
            s = setData;
        }
        return data == undefined ? s : data;
    }

    var GetCurrentUser = function () {
        var user = null;
        AjaxCustom.getAjax("/api/User/GetCurrentUser", function (data) {
            user = data;
        }, null, false);
        return user;
    }
    var GetAttachmentsData = function (row) {
        var fileData = [];
        if (row.Attachments) {
            $.each(row.Attachments, function (key, item) {
                var obj = new Object();
                obj.Base64Stream = item.FilePath;
                obj.ID = item.ID;
                obj.SizeDisplay = item.SizeDisplay;
                obj.Size = "";
                obj.Name = item.Name;
                fileData.push(obj);
            })
        }
        return fileData;
    }
    return {
        AddAndDelete: AddAndDelete,
        AddsAndDeletes: AddsAndDeletes,
        AddOrUpdate: AddOrUpdate,
        Delete: Delete,
        InitProjectAll: InitProjectAll,
        SwalLoad: SwalLoad,
        SwalClose: SwalClose,
        GetPostData: GetPostData,
        GetFormData: GetFormData,
        AddOrUpdateByFormData: AddOrUpdateByFormData,
        ParseUndefined: ParseUndefined,
        SelectMultipleParseValue: SelectMultipleParseValue,
        GetCurrentUser: GetCurrentUser,
        GetAttachmentsData:GetAttachmentsData
    };
})();



document.onreadystatechange = subSomething;//当页面加载状态改变的时候执行这个方法. 
function subSomething() {
    if (document.readyState == "complete") //当页面加载状态 
    {

    }
} 
