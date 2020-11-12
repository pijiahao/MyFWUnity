
var DocUploadFile = function () {
    this.uploadurl = "/api/File/TempUploadBigFile";
    this.formdata = new FormData();
    this.batchUploadID = "";
    this.uploadfilecontrol = "";
    this.init();
    this.fc();
}
DocUploadFile.global = {
    allfilelength: 0,
    progress: 0,
    index: 0,
}
DocUploadFile.extendupload = null;
DocUploadFile.prototype.fc = function () {
    var _that = this;
    $(function () {
        // _that.showdialog(0, true);
    })
}
DocUploadFile.prototype.showdialog = function (folderID, canupload, uploadfilecontrol) {
    DocUploadFile.extendupload = this;
    this.uploadfilecontrol = uploadfilecontrol;
    var guid = new GUID();
    this.batchUploadID = guid.newGUID();
    this.formdata = new FormData();
    this.formdata.append("BatchUploadID", this.batchUploadID);
    $('#input-700').fileinput('refresh');
    $('#input-700').fileinput('unlock');
    $("#DocUploadDialog").modal("show");
}
DocUploadFile.prototype.hidedialog = function () {
    $("#DocUploadDialog").modal("hide");
}
DocUploadFile.prototype.init = function () {
    var _that = this;
    $("#input-700").fileinput({
        language: 'zh',
        uploadUrl: _that.uploadurl,
        uploadAsync: true,
        maxFileCount: 50,
        showZoom: false,
        showDrag: false,
        enctype: 'multipart/form-data',
        validateInitialCount: true
    }).on("fileuploaded", function (event, data) {
        if (DocUploadFile.global.index == 0) {
            for (var x = 0; x < data.files.length; x++) {
                DocUploadFile.global.allfilelength = DocUploadFile.global.allfilelength + cutFile(data.files[x], 1024 * 1024 * 4).length;
            }
        }
        DocUploadFile.global.index++;
        if (islast) {
            DocUploadFile.global.progress = parseFloat((DocUploadFile.global.progress + 100 / DocUploadFile.global.allfilelength).toFixed(2));
            if (DocUploadFile.global.index == DocUploadFile.global.allfilelength) {
                _that.uploadsuccesscallback();
                _that.clearinputFile();
                sweetAlert("提示", "上传成功!", "success");
            } else {
                $(".bc-upload-progress").show();
                $(".progress-bar").attr("aria-valuenow", DocUploadFile.global.progress).css("width", DocUploadFile.global.progress + "%").html(DocUploadFile.global.progress + "%");
            }
        }
    }).on('fileremoved', function (event, data, msg) {
        $('#input-700').fileinput('unlock');
    }).on("fileclear", function () {
        _that.clearinputFile();
    }).on('filecleared', function () {
        _that.clearinputFile();
    });
    $('#input-700').fileinput('refresh', {
        uploadUrl: _that.uploadurl
    });
}
DocUploadFile.prototype.getBatchUploadedTempFiles = function (callback) {
    AjaxCustom.getAjax('/api/File/GetBatchUploadedTempFiles?batchUploadID=' + this.batchUploadID, function (data) {
        if (data.items.length > 0) {
            if (typeof callback == "function") {
                callback(data.items);
            }
        }
    });
}
DocUploadFile.prototype.clearinputFile = function () {
    $('#input-700').fileinput('refresh');
    $('#input-700').fileinput('unlock');
    DocUploadFile.global.index = 0;
    DocUploadFile.global.progress = 0;
    DocUploadFile.global.allfilelength = 0;
    $(".bc-upload-progress").hide();
}
DocUploadFile.prototype.uploadsuccesscallback = function () {

}
