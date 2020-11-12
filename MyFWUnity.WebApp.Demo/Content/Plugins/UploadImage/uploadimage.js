(function ($) {
    var UploadImage = function (el, option) {
        this.$this = $(el);
        this.default = option;
        this.uploadImageSrc = null;
        this.init();
    };
    UploadImage.DEFAULT = {
        text: "",
        col: 3,
        disabled:false,
        imagesrc: "/Content/images/uploadfile.png",
        success: function (result) {


        }
    };

    UploadImage.prototype.init = function () {
        this.initBody();
    };

    UploadImage.prototype.initBody = function () {
        var self = this;
        this.$this.html('');
        var strHtml = ' <label class="col-sm-' + this.default.col + ' control-label form-label">' + this.default.text + '</label>';
        strHtml += '<div class="col-sm-' + (12 - this.default.col) + '" style="height:100px;">';
        strHtml += '  <img src="' + this.default.imagesrc + '" height="80" class="upload-file" id="HeadImage" style="cursor:pointer" />';
        strHtml += ' </div>';
        this.$this.append(strHtml);
        this.initFile();
        this.$this.find("img").off("click").on("click", function () {
            if (!self.default.disabled) {
                self.$this.find("input[type='file']").click();
            }
        });

    };
    UploadImage.prototype.initFile = function () {
        var self = this;
        this.$this.find("input[type='file']").remove();
        this.$this.append('<input type="file" style="display:none;"  />');
        this.$this.find("input[type='file']").off("change").on("change", function () {
            var file = this.files;
            $.each(file, function (k, v) {
                var i = Math.floor(Math.log(v.size) / Math.log(1024));
                var filename = v.name;
                var fileext = filename.split('.');
                var ext = fileext[fileext.length - 1].toLowerCase().trim();
                var size = (v.size / Math.pow(1024, i)).toFixed(2) * 1;
                if (i > 10 && size > 10) {
                    sweetAlert("警告", "超出限制大小！！！", "warning");
                }
                else if (ext != "jpg" && ext != "png" && ext != "jpeg" && ext != "gif" && ext != "bmp") {
                    sweetAlert("警告", "图片格式不正确！！！", "warning");
                }
                else {
                    self.upload(file[0]);
                }
            });
        });
    };

    UploadImage.prototype.refresh = function () {
        this.initBody();
        this.initFile();
        this.uploadImageSrc = null;
    };
    UploadImage.prototype.disabled = function (value) {
        this.default.disabled = value;
    };
    UploadImage.prototype.setImage = function (src) {
        if (src != undefined) {
            this.uploadImageSrc = src;
            this.$this.find("img").attr("src", src);
        }
        else {
            this.uploadImageSrc = null;
            this.$this.find("img").attr("src", this.default.imagesrc);
        }
       
    };
    UploadImage.prototype.getImage = function () {
        return this.uploadImageSrc;
    };

    UploadImage.prototype.upload = function (file) {
        var self = this;
        var formData = new FormData();
        formData.append("file", file);
        AjaxCustom.formDataAjax("/api/File/UploadWebImage", formData, function (data) {
            self.$this.find("img").attr("src", data.url);
            self.uploadImageSrc = data.url;
            if (typeof self.default.success === "function") {
                self.default.success(data);
            }
        });
    };
    var uploadfileallowedMethods = ["setImage", "refresh", "getImage","disabled"];

    $.fn.uploadimage = function (option) {
        var value, args = Array.prototype.slice.call(arguments, 1);
        var data;
        this.each(function () {
            data = $(this).data("uploadimage");
            var uploadfiledefault;
            if (!data) {
                uploadfiledefault = $.extend({}, UploadImage.DEFAULT, option);
                $(this).data("uploadimage", (data = new UploadImage(this, uploadfiledefault)));
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
        });
        return typeof value === 'undefined' ? this : value;
    };
})(jQuery);