(function ($) {
    var CheckList = function (el, option) {
        this.$this = $(el);
        this.default = option;
        this.init();
    };
    CheckList.DEFAULT = {
        data: [],
        checkData: "",
        disabled: false,
        radio: false,
        click: function (value) {
        }
    };

    CheckList.prototype.init = function () {
        this.initBody();
    };
    CheckList.prototype.hasCheckDataInArray = function (value) {
        if (this.default.checkData == null) {
            return false;
        }
        var array = this.default.checkData.split(";");
        return $.inArray(value, array) > -1;
    }
    CheckList.prototype.initBody = function () {
        var self = this;
        this.$this.addClass("check-list");
        this.$this.html('');
        var strHtml = ' ';
        for (var i = 0; i < this.default.data.length; i++) {
            var value = this.default.data[i].value;
            var checked = "";
            if (this.hasCheckDataInArray(value)) {
                checked = "checked";
            }
            strHtml += '   <a href="javascript:;" class="' + checked + '"  data-value="' + this.default.data[i].value + '">' + this.default.data[i].text + '</a>';

        }
        this.$this.append(strHtml);
        this.$this.find("a").off("click").on("click", function () {

            if (!self.default.disabled) {
                if (self.default.radio) {
                    self.$this.find("a").removeClass("checked");
                }
                if ($(this).hasClass("checked")) {
                    $(this).removeClass("checked");
                }
                else {
                    $(this).addClass("checked");
                }
                if (typeof self.default.click == "function") {
                    self.default.click($(this).data("value"));
                }
            }

        });
    };
    CheckList.prototype.loadData = function (data) {
        this.default.data = data;
        this.initBody();
    };
    CheckList.prototype.getValue = function () {
        var value = [];
        $.each(this.$this.find("a"), function () {
            if ($(this).hasClass("checked")) {
                value.push($(this).data("value"));
            }
        });
        return value.join(";");
    };
    CheckList.prototype.refresh = function () {
        this.default.checkData = "";
        this.initBody();
    };
    CheckList.prototype.checkData = function (data) {
        this.default.checkData = data;
        this.initBody();
    };
    CheckList.prototype.disabled = function (value) {
        this.default.disabled = value;
    };
    var checklistallowedMethods = ["getValue", "refresh", "checkData", "disabled", "loadData"];
    $.fn.checklist = function (option) {
        var value, args = Array.prototype.slice.call(arguments, 1);
        var data;
        this.each(function () {
            data = $(this).data("checklist");
            var checklistdefault;
            if (!data) {
                checklistdefault = $.extend({}, CheckList.DEFAULT, option);
                $(this).data("checklist", (data = new CheckList(this, checklistdefault)));
            }
            else {
                if (typeof option == "string") {
                    if ($.inArray(option, checklistallowedMethods) < 0) {
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