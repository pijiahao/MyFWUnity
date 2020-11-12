var CustomSetting = {
    IsMobile: false
}
var AjaxCustom = (function () {
    var HandleJsonAlert = function (data) {
        if (data.HasMessage && data.MessageModel.Type == 1) {
            if (CustomSetting.IsMobile) {
                mui.toast(data.MessageModel.Message);
            }
            else {
                sweetAlert("成功", data.MessageModel.Message, "success");
            }
        }
        else if (data.HasMessage && data.MessageModel.Type == 2) {
            if (CustomSetting.IsMobile) {
                mui.toast(data.MessageModel.Message);
            }
            else {
                sweetAlert("警告", data.MessageModel.Message, "warning");
            }
        }
        else if (data.HasMessage && data.MessageModel.Type == 3) {
            if (CustomSetting.IsMobile) {
                mui.toast(data.MessageModel.Message);
            }
            else {
                sweetAlert("错误", data.MessageModel.Message, "error");
            }
        }
        else if (data.responseJSON != undefined && data.responseJSON.HasMessage) {

            if (data.responseJSON.MessageModel.Type == 2) {
                if (CustomSetting.IsMobile) {
                    mui.toast(data.responseJSON.MessageModel.Message);
                }
                else {
                    sweetAlert("错误", data.responseJSON.MessageModel.Message, "error");
                }
            } else if (data.responseJSON.MessageModel.Type == 3) {
                if (CustomSetting.IsMobile) {
                    mui.toast(data.responseJSON.MessageModel.Message);
                }
                else {
                    sweetAlert("警告", data.responseJSON.MessageModel.Message, "warning");
                }
            } else {
                if (CustomSetting.IsMobile) {
                    mui.toast(data.responseJSON.MessageModel.Message);
                }
                else {
                    sweetAlert("未知错误", data.responseJSON.MessageModel.Message, "Information");
                }
            }
        } else if (data.responseJSON != undefined) {
            if (CustomSetting.IsMobile) {
                mui.toast(data.responseJSON.ExceptionMessage);
            }
            else {
                sweetAlert("错误", data.responseJSON.ExceptionMessage, "error");
            }
        } else {
            if (data.statusText != undefined) {
                if (CustomSetting.IsMobile) {
                    mui.toast(data.statusText);
                }
                else {
                    sweetAlert("错误", data.statusText, "error");
                }
            }
        }
    }

    var getAjax = function (url, callback, errorCallback, isAsync) {
        $.ajax({
            type: "Get",
            url: url,
            async: ((isAsync == undefined ? true : isAsync)),
            dataType: "json",
            success: function (data) {
                HandleJsonAlert(data);
                if (typeof callback === "function") {
                    callback(data.ResultData);
                }
            },
            error: function (data) {
                if (typeof errorCallback === "function") {
                    errorCallback(data.ResultData);
                }
                HandleJsonAlert(data);
            }
        });
    }
    var postAjax = function (url, dataInfo, callback, errorCallback, contentType, isAsync) {
        if (contentType == undefined) {
            contentType = "application/x-www-form-urlencoded";
        }
        $.ajax({
            type: "Post",
            url: url,
            data: dataInfo,
            dataType: "json",
            async:(isAsync == undefined ? true : isAsync),
            contentType: contentType,
            success: function (data) {
                HandleJsonAlert(data);
                if (typeof callback === "function") {
                    callback(data.ResultData);
                }
            },
            error: function (data) {
                if (typeof errorCallback === "function") {
                    errorCallback(data.ResultData);
                }
                HandleJsonAlert(data);
            }
        });
    }
    var deleteConfirmAjax = function (url, callback, errorCallback, isAsync, info) {
        info = info || "确认删除吗？";
        swal({
            title: "警告",
            text: info,
            type: "warning",
            showCancelButton: true,
            closeOnConfirm: false,
            confirmButtonText: "确定",
            confirmButtonColor: "#ec6c62"
        }, function () {
            info = "再次确认删除吗？";
            swal({
                title: "警告",
                text: info,
                type: "warning",
                showCancelButton: true,
                closeOnConfirm: false,
                confirmButtonText: "确定",
                confirmButtonColor: "#ec6c62"
            }, function () {
                getAjax(url, callback, errorCallback, isAsync);
            });
        });
    }

    var submitAjax = function (url, formId, callback, errorCallback) {
        $('#' + formId + '').ajaxSubmit({
            type: "Post",
            url: url,
            dataType: "json",
            success: function (data) {
                HandleJsonAlert(data);
                if (typeof callback === "function") {
                    callback(data.ResultData);
                }
            },
            error: function (data) {
                if (typeof errorCallback === "function") {
                    errorCallback(data.ResultData);
                }
                HandleJsonAlert(data);
            }
        });
    };

    var formDataAjax = function (url, formData, callback, errorCallback) {
        $.ajax({
            type: 'post',
            url: url,
            data: formData,
            cache: false,
            processData: false, // 不处理发送的数据，因为data值是Formdata对象，不需要对数据做处理
            contentType: false, // 不设置Content-type请求头
            success: function (data) {
                HandleJsonAlert(data);
                if (typeof callback === "function") {
                    callback(data.ResultData);
                }
            },
            error: function (data) {
                if (typeof errorCallback === "function") {
                    errorCallback(data.ResultData);
                }
                HandleJsonAlert(data);
            }
        })
    }

    return {
        getAjax: getAjax,
        postAjax: postAjax,
        deleteConfirmAjax: deleteConfirmAjax,
        submitAjax: submitAjax,
        formDataAjax: formDataAjax,
    }
})();


$(function () {
    $(document).ajaxError(function (e, status, responseText, statusText) {
        if (statusText == "Unauthorized") {// 未认证
            window.location = "/Account/Logout";
        }
    });
});