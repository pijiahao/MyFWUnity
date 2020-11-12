(function ($) {
    var Uploadfile = function (el, option) {
        this.$this = $(el);
        this.default = option;
        this.init();
        this.batchUploadIDs = [];
        this.disable = false;
        this.initbody();
        if (this.default.button != null) {
            this.onclickbtn();
        }
    };
    Uploadfile.DEFAULT = {
        title: "上传",
        button: null,
        collenght: 2,
        text: "上传",
        formid: "",
        filename: null,//post 后台api接收区分文件分类参数
        postuploadidskey: null,//post 后台api接收区uploadid分类参数
        filenumber: 0,
        isremoveloaddata: false,
        allowedFileExtensions: null,//['jpg', 'gif', 'png', 'pdf']接收的文件后缀
        filedata: null,
        isclickadd: true,//wugaiming
        isvalidate: false,
        deletefile: function (data, obj) {
            return {};
        },
        submitfile: function (formdata) {
            return {};
        },
        validate: function () {//wugaiming
            return {};
        }
    }
    Uploadfile.prototype.init = function () {
        this.loadfilecount = 0;
        this.clickfilecount = 0;
        this.removeloaddataids = [];
        // this.attachmentcount = 0;
        this.allfilecount = 0;
        this.currentfiles = [];
        this.loaddata = [];

    }

    Uploadfile.prototype.load = function (data) {
        if (data == null) {
            return;
        }
        if (data.length > 0) {
            this.init();
            this.batchUploadIDs = [];
            this.$this.find(".filelist").html('');
            this.loadfilecount += data.length;
            this.allfilecount += data.length;
            this.loaddata = data;
            this.loadfile(data, false);
        }
    }
    Uploadfile.prototype.appendLoad = function (data) {
        if (data == null) {
            return;
        }
        if (data.length > 0) {
            this.loadAppendFileDatas = [];
            this.allfilecount += data.length;
            this.loadAppendFile(data, false);
        }
    }

    Uploadfile.prototype.delete = function (loadfiledelete) {
        this.default.deletefile = loadfiledelete;
    }
    Uploadfile.prototype.initbody = function () {
        var randomnumber = Math.random();
        var self = this;
        var html = '<div class="form-group   inputfile">' +
            '<label class="col-sm-' + this.default.collenght + ' control-label form-label label-attachment">{0}</label>' +
            ' <div class="file-caption-main" >' +
            '     <div class="input-group-btn uploadbtn" style="float: left;">' +
            '       <div tabindex="500"  class="btn btn-default btn-file input-file fileclick">' +
            '      <i class="glyphicon glyphicon-folder-open"></i>&nbsp;' +
            '       <span class="hidden-xs"  >{1}</span>' +
            '                 </div>' +
            '       </div>' +
            '   </div>' +
            '    <div id="AttachmentCountDiv" class="left showfileslable">' +
            // '       (当前新增<span id="AttachmentCount"></span>个附件)' +
            '     </div>' +
            '    <div id="SwitchAttachmentDiv"  class="switch-attachment showfileslable right" style="display:none"></label>' +
            '      <input type="checkbox" id="showfiles_' + randomnumber + '"   class="checked_Effect" checked="checked" />' +
            '       <label id="SwitchAttachment"   for="showfiles_' + randomnumber + '" >' +
            '    </div></div>' +

            '<div id="AttachmentDiv" class="form-group images" >' +
            '  <label class="col-sm-' + this.default.collenght + ' control-label form-label" ></label >' +
            '   <div class="col-sm-' + (12 - this.default.collenght) + '" > ' +
            '   <div class="filelist clearfix" style="border:1px solid #ccc;" > ' +

            '      </div> ' +
            '      </div> ' +
            '        </div>';
        this.$this.html(html.format(this.default.title, this.default.text));
        this.createfileinput();
        //$(".showfileslable").fadeOut();
        this.$this.find(".fileclick").off("click").on("click", function () {
            upload.showdialog(null, null, self.$this);
            // self.$this.find("input[type='file']").click();
        })
        this.$this.find("input[type='checkbox']").on("click", function () {
            if ($(this).is(":checked")) {
                self.$this.find("#AttachmentDiv").slideDown();
            }
            else {
                self.$this.find("#AttachmentDiv").slideUp();
            }
        })

    }
    Uploadfile.prototype.createfileinput = function () {
        var self = this;
        var fileinput = '<input  name="uploadfileinput" type="file" multiple="multiple" class="file" style="display:none" />';
        //  this.$this.append(fileinput);
        this.$this.find("input[type='file']").on("change", function () {
            if (self.default.isvalidate) {
                if (self.default.validate()) {
                    self.clickfile(this);
                }
                else {
                    self.refresh();
                }
            }
            if (!self.default.isvalidate) {
                self.clickfile(this);
            }

        })
    }
    Uploadfile.prototype.clearfileinput = function () {
        this.$this.find("input[type='file']").remove();
    }
    Uploadfile.prototype.onclickbtn = function () {
        var self = this;
        $(this.default.button).off("click").on("click", function () {
            var formdata = self.getformdata();
            self.default.submitfile(formdata);
        })
    }
    Uploadfile.prototype.checkedfilenumber = function () {
        var result = true;
        if (this.default.filenumber > 0) {
            if (this.allfilecount >= this.default.filenumber) {
                result = false;
            }
        }
        return result;
    }
    Uploadfile.prototype.clickfile = function (file) {
        var self = this;
        if (file.files.length > 0) {
            if (!this.default.isclickadd) {
                this.removeallfiles();
                this.currentfiles = [];
                this.allfilecount = this.loadfilecount;
            }
            this.isclickLoadfile(file);
            $.each(file.files, function (k, v) {
                var selffile = this;
                var obj = new Object();
                var files = [];
                var i = Math.floor(Math.log(v.size) / Math.log(1024));
                var sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
                var reader = new FileReader();
                reader.readAsDataURL(v);
                reader.onload = function (theFile) {
                    var filename = v.name;
                    var fileext = filename.split('.');
                    if (self.isextension(fileext[fileext.length - 1]) && !self.ishasclickfile(filename)) {
                        // self.attachmentcount++;
                        obj.Name = filename;
                        var imgbases = theFile.target.result.split(",");
                        obj.Base64Stream = imgbases[1];
                        obj.ID = 0;
                        obj.SizeDisplay = (v.size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + sizes[i];
                        obj.Size = v.size;
                        files.push(obj);
                        if (self.checkedfilenumber()) {
                            self.allfilecount++;
                            self.loadfile(files, true);
                            self.currentfiles.push(selffile);
                        }
                        else {
                            sweetAlert("警告", "当前文件仅支持上传" + self.default.filenumber + "个文件,多余文件已过滤", "warning");
                        }
                    }
                }
            });
        }
    }
    Uploadfile.prototype.ishasclickfile = function (filename) {
        var result = false;
        if (this.currentfiles.length > 0) {
            for (var i = 0; i < this.currentfiles.length; i++) {
                if (this.currentfiles[i].name === filename) {
                    result = true;
                }
            }
        }
        return result
    }

    Uploadfile.prototype.isclickLoadfile = function (file) {
        var self = this;
        var NotFileExtensionsFile = true;
        var HasFileExtensionsFile = false;
        if (this.default.allowedFileExtensions != null) {
            $.each(file.files, function (k, v) {
                var filename = v.name;
                var fileext = filename.split('.');
                if (self.isextension(fileext[fileext.length - 1])) {
                    HasFileExtensionsFile = true;
                }
                else {
                    NotFileExtensionsFile = false;
                    //self.removecurrentfiles(self.clickfilecount);
                }
                //  self.clickfilecount++;
            });
            if (!NotFileExtensionsFile) {
                sweetAlert("警告", "当前文件仅支持" + this.default.allowedFileExtensions.join(",") + "格式上传,其他格式已过滤", "warning");
            }
        }
        return HasFileExtensionsFile;
    }


    Uploadfile.prototype.isextension = function (ext) {
        var result = false;
        if (this.default.allowedFileExtensions != null) {
            for (var i = 0; i < this.default.allowedFileExtensions.length; i++) {
                if (this.default.allowedFileExtensions[i].trim() == ext.toLowerCase().trim()) {
                    result = true;
                }
            }
        }
        else {
            result = true;
        }
        return result;
    }
    Uploadfile.prototype.getimghtml = function (filename, fileurl) {
        var strhtml = '';
        var filearry = filename.split(".");
        var ext = filearry[filearry.length - 1];
        ext = ext.toLowerCase();
        if (ext == "doc" || ext == "docx") {
            strhtml += '<img src="/content/images/word.png" class="file-preview-image kv-preview-data" onclick="ImgMagnifying(this)" title= "' + filename + '" alt= "' + filename + '" style= "width:auto;height:40px;max-width:80px;" >';

        }
        else if (ext == "xls" || ext == "xlsx") {
            strhtml += '<img src="/content/images/excel.png" class="file-preview-image kv-preview-data" onclick="ImgMagnifying(this)" title= "' + filename + '" alt= "' + filename + '" style= "width:auto;height:40px;max-width:80px;" >';

        }
        else if (ext == "ppt" || ext == "pptx") {
            strhtml += '<img src="/content/images/ppt.png" class="file-preview-image kv-preview-data" onclick="ImgMagnifying(this)" title= "' + filename + '" alt= "' + filename + '" style= "width:auto;height:40px;max-width:80px;" >';
        }
        else if (ext == "pdf") {
            strhtml += '<img src="/content/images/pdf.png" class="file-preview-image kv-preview-data" onclick="ImgMagnifying(this)" title= "' + filename + '" alt= "' + filename + '" style= "width:auto;height:40px;max-width:80px;" >';
        }
        else if (ext == "mp4" || ext == "rmvb" || ext == "wmv" || ext == "avi" || ext == "3gp" || ext == "rm") {
            strhtml += '<img src="/content/images/upload-video.png" class="file-preview-image kv-preview-data" onclick="ImgMagnifying(this)" title= "' + filename + '" alt= "' + filename + '" style= "width:auto;height:40px;max-width:80px;" >';
        }
        else if (ext == "jpg" || ext == "png" || ext == "gif" || ext == "pdf") {
            strhtml += '<img src="/content/images/upload-image.png" class="file-preview-image kv-preview-data" onclick="ImgMagnifying(this)" title= "' + filename + '" alt= "' + filename + '" style= "width:auto;height:40px;max-width:80px;" >';
        }
        else {
            strhtml += ' <img src="/content/images/upload-file.png" class="file-preview-image kv-preview-data"  title= "' + filename + '" alt= "' + filename + '" style= "width:auto;height:40px;max-width:80px;" >';
        }
        return strhtml
    }

    Uploadfile.prototype.refresh = function () {
        this.clearfileinput();//清除文件
        this.createfileinput();//重新生成文件
        this.init();
        this.$this.find(".filelist").html('');
        this.currentfiles = [];
        this.batchUploadIDs = [];
        this.$this.find(".showfileslable").fadeOut();
        this.$this.find("#AttachmentDiv").slideUp();

    }

    Uploadfile.prototype.addfilesinformdata = function (formdata) {
        if (this.currentfiles.length > 0) {
            for (var i = 0; i < this.currentfiles.length; i++) {
                if (this.default.filename == null) {
                    formdata.append(this.currentfiles[i].name, this.currentfiles[i]);
                }
                else {
                    formdata.append(this.default.filename, this.currentfiles[i]);
                }
            }
        }

        return formdata;
    }
    Uploadfile.prototype.addBatchUploadIDsInFormData = function (formdata) {
        if (this.batchUploadIDs.length > 0) {
            if (this.default.postuploadidskey == null) {
                formdata.append("batchUploadIDs", this.batchUploadIDs.join(","));
            }
            else {
                formdata.append(postuploadidskey, this.batchUploadIDs.join(","));
            }
        }
        return formdata;
    }


    Uploadfile.prototype.getfiledata = function () {
        return this.currentfiles;
    }

    Uploadfile.prototype.getformdata = function () {
        var form = document.getElementById(this.default.formid);
        var formdata = new FormData();
        if (form != null) {
            var tagElements = form.getElementsByTagName('input');
            for (var j = 0; j < tagElements.length; j++) {
                formdata.append(tagElements[j].name, tagElements[j].value);
            }
            var tagElement_textareas = form.getElementsByTagName('textarea');
            for (var j = 0; j < tagElement_textareas.length; j++) {
                formdata.append(tagElement_textareas[j].name, tagElement_textareas[j].value);
            }

            var tagElement_select = form.getElementsByTagName('select');
            for (var j = 0; j < tagElement_select.length; j++) {
                formdata.append(tagElement_select[j].name, tagElement_select[j].value);
            }
            if (this.currentfiles.length > 0) {
                for (var i = 0; i < this.currentfiles.length; i++) {
                    if (this.default.filename == null) {
                        formdata.append(this.currentfiles[i].name, this.currentfiles[i]);
                    }
                    else {
                        formdata.append(this.default.filename, this.currentfiles[i]);
                    }
                }
            }
            if (this.batchUploadIDs.length > 0) {
                if (this.default.postuploadidskey == null) {
                    formdata.append("batchUploadIDs", this.batchUploadIDs.join(","));
                }
                else {
                    formdata.append(postuploadidskey, this.batchUploadIDs.join(","));
                }
            }
        }
        return formdata;

    }
    Uploadfile.prototype.getremoveloaddataids = function () {
        return this.removeloaddataids;
    }
    Uploadfile.prototype.loadAppendFile = function (data) {
        var self = this;
        if (data != null && data.length != 0) {
            this.$this.find(".showfileslable").fadeIn();
            for (var i = 0; i < data.length; i++) {
                this.loadAppendFileDatas.push(data[i]);
                var strhtml;
                var docver = data[i].Name;
                strhtml = '<div class="file-preview-frame krajee-default  kv-preview-thumb"  onmouseover="frameover(this)" onmouseout="frameout(this)" id="preview-1497597237748-' + i + '" data-fileindex="' + i + '" data-template="image"><div class="kv-file-content">';
                strhtml += this.getimghtml(docver, data[i].UploadTempRelativePath);
                strhtml += '</div><div class="file-thumbnail-footer"><div class="file-footer-caption" title="' + docver + '">' + docver + '<br> <samp>(' + data[i].SizeDisplay + ')</samp></div>';
                strhtml += ' <div class="file-actions">  <div class="file-footer-buttons">';
                var btnclass = '';
                btnclass = 'kv-appendfile-remove '
                strhtml += ' <a  href="javascript:;" data-index="' + (this.loadAppendFileDatas.length - 1) + '"   data-isclickfile="false" class="' + btnclass + ' " style="color:#fff;text-decoration:none;" title="删除文件" > <i class="glyphicon glyphicon-trash"></i></a > ';
                //strhtml += ' <a  href="javascript:;" style="color:#fff;text-decoration:none;" title="预览文件" > <i class="fa fa-eye"></i></a > ';
                // strhtml += ' <a  href="/api/DocManager/DownloadFiles?docVersionIDs=' + data[i].ID + '" style="color:#fff;text-decoration:none;" title="下载文件" > <i class="fa fa-download"></i></a > ';
                strhtml += '</div> <div class="clearfix"></div> </div></div></div>';
                this.$this.find(".filelist").append(strhtml);
                this.$this.find("#AttachmentDiv").slideDown();
                this.$this.find("input[type='checkbox']").prop("checked", true);
                this.$this.find("a.kv-appendfile-remove").off("click").on("click", function () {
                    if (!self.disable) {
                        var index = $(this).data("index");
                        var obj = this;
                        var uploadId = self.loadAppendFileDatas[index].BatchUploadID;
                        var fileName = self.loadAppendFileDatas[index].Name;
                        swal({
                            title: "警告",
                            text: "您确定要删除该文件？",
                            type: "warning",
                            showCancelButton: true,
                            confirmButtonText: "确定",
                            confirmButtonColor: "#ec6c62"
                        }, function () {
                            BIMPlatformAjax.getAjax("/api/UploadFile/RemoveTempUploadedFileOfBatch/?batchUploadID=" + uploadId + "&fileName=" + fileName, function (data) {

                                self.removefile(obj);
                            })
                        });
                    }
                })
            }
            //  $("#AttachmentCount").html(this.attachmentcount);
            // this.loadfilecount++;
        }
    }
    Uploadfile.prototype.loadfile = function (data, isclickfile) {
        var self = this;
        if (data != null && data.length != 0) {
            this.$this.find(".showfileslable").fadeIn();
            for (var i = 0; i < data.length; i++) {
                var strhtml;
                var docver = data[i].Name;
                strhtml = '<div class="file-preview-frame krajee-default  kv-preview-thumb"  onmouseover="frameover(this)" onmouseout="frameout(this)" id="preview-1497597237748-' + i + '" data-fileindex="' + i + '" data-template="image"><div class="kv-file-content">';
                strhtml += this.getimghtml(docver, data[i].Base64Stream);
                strhtml += '</div><div class="file-thumbnail-footer"><div class="file-footer-caption" title="' + docver + '">' + docver + '<br> <samp>(' + data[i].SizeDisplay + ')</samp></div>';
                strhtml += ' <div class="file-actions">  <div class="file-footer-buttons">';
                var btnclass = '';
                if (!isclickfile) {
                    btnclass = 'kv-file-remove '
                    strhtml += ' <a  href="javascript:;" data-index="' + (this.currentfiles.length + i) + '"  data-isclickfile="' + isclickfile + '" class="' + btnclass + ' " style="color:#fff;text-decoration:none;" title="删除文件" > <i class="glyphicon glyphicon-trash"></i></a > ';
                    //strhtml += ' <a  href="javascript:;" style="color:#fff;text-decoration:none;" title="预览文件" > <i class="fa fa-eye"></i></a > ';
                    strhtml += ' <a  href="javascript:;" data-id="' + data[i].ID + '" class="kv-download" style="color:#fff;text-decoration:none;" title="下载文件" > <i class="fa fa-download"></i></a > ';

                }
                else {
                    //this.createfileinput();
                    btnclass = 'kv-click-file-remove  removefile_click_' + i + '';
                    strhtml += ' <a  href="javascript:;" data-index="' + (this.currentfiles.length + i) + '"  data-isclickfile="' + isclickfile + '" class="' + btnclass + ' " style="color:#fff;text-decoration:none;" title="删除文件" > <i class="glyphicon glyphicon-trash"></i>删除</a > ';

                }

                strhtml += '</div> <div class="clearfix"></div> </div></div></div>';
                this.$this.find(".filelist").append(strhtml);
                this.$this.find("#AttachmentDiv").slideDown();
                this.$this.find("input[type='checkbox']").prop("checked", true);
                if (!isclickfile) {
                    this.$this.find("a.kv-file-remove").off("click").on("click", function () {
                        if (!self.disable) {
                            var index = $(this).data("index");
                            if (self.default.isremoveloaddata) {
                                self.removeloaddataids.push(self.loaddata[index].ID);
                                self.removefile(this);
                            }
                            else {
                                self.default.deletefile(self.loaddata[index], this);
                            }
                        }
                    })
                    this.$this.find("a.kv-download").off("click").on("click", function () {
                        var id = $(this).data("id");
                        BIMPlatformAjax.getAjax('/api/DocManager/DownloadFiles?docVersionIDs=' + id, function (data) {
                            $.fileDownload('/DocManager/DownloadFile?path=' + data.downloadInfo.RelativePath);
                        });
                    })
                }
                else {
                    if (!self.disable) {
                        this.$this.find("a.kv-click-file-remove").off("click").on("click", function () {
                            self.removefile(this);
                            self.clearfileinput();//清除文件
                            self.createfileinput();//重新生成文件
                        })
                    }
                }
            }
            //  $("#AttachmentCount").html(this.attachmentcount);
            // this.loadfilecount++;
        }
    }
    Uploadfile.prototype.removeallfiles = function () {
        if (this.currentfiles.length > 0) {
            // this.attachmentcount = 0;
            //this.loadfilecount = 0;
            for (var i = 0; i < this.currentfiles.length; i++) {
                this.$this.find(".removefile_click_" + i).parents(".kv-preview-thumb").remove();
            }
        }
    }
    Uploadfile.prototype.removecurrentfiles = function (index) {
        var removefiles = [];
        for (var i = 0; i < this.currentfiles.length; i++) {
            if (i != index) {
                removefiles.push(this.currentfiles[i])
            }
        }
        this.currentfiles = removefiles;
    }
    Uploadfile.prototype.removefile = function (event) {
        $(event).parents(".kv-preview-thumb").remove();
        var filelen = this.$this.find("div.filelist div.kv-preview-thumb").length;
        if (filelen == 0) {
            this.$this.find(".showfileslable").fadeOut();
            this.$this.find("#AttachmentDiv").slideUp();
        }
    }
    Uploadfile.prototype.disabled = function (disable) {
        this.disable = disable;
        if (this.disable) {
            this.$this.find("input[type='file']").attr("disabled", "disabled");
        }
        else {
            this.$this.find("input[type='file']").removeAttr("disabled");

        }
        $("a.kv-file-remove").hide();
        $(".uploadbtn").hide();
    }
    Uploadfile.prototype.pushBatchUploadIDs = function (batchUploadID) {
        this.batchUploadIDs.push(batchUploadID);
    }


    var uploadfileallowedMethods = ["getformdata", "refresh", "getfiledata", "load", "appendLoad", "delete",
        "removefile", "addfilesinformdata", "disabled", "getremoveloaddataids", "pushBatchUploadIDs", "addBatchUploadIDsInFormData"];

    $.fn.uploadfile = function (option) {
        var value, args = Array.prototype.slice.call(arguments, 1);
        var data;
        this.each(function () {
            data = $(this).data("uploadfile");
            var uploadfiledefault;
            if (!data) {
                uploadfiledefault = $.extend({}, Uploadfile.DEFAULT, option);
                $(this).data("uploadfile", (data = new Uploadfile(this, uploadfiledefault)));
            }
            else {
                if (typeof option == "string") {
                    if ($.inArray(option, uploadfileallowedMethods) < 0) {
                        throw new Error("Unknown method: " + option);
                    }
                    if (!data) {
                        return;
                    }
                    value = data[option].apply(data, args);
                }
            }
        })
        return typeof value === 'undefined' ? this : value;
    };

})(jQuery)


function frameover(obj) {
    $frame = $(obj);
    $frame.find(".file-actions").show();
}

function frameout(obj) {
    $frame = $(obj);
    $frame.find(".file-actions").hide();
}