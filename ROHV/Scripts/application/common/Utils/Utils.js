var Utils = {

    isStartSending: false,
    ajax: function (data, successFunction, errorFunction, btnElm) {
        if (!Utils.isStartSending) {
            Utils.isStartSending = true;
            var headers = {};
            this.showLoader(btnElm);
            $.ajax({
                type: 'POST',
                url: data.action,
                data: data.postData,
                dataType: "json",
                traditional: true,
                contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                success: function (response) {
                    Utils.isStartSending = false;
                    if (successFunction) successFunction(response);
                    Utils.hideLoader(btnElm);

                },
                error: function (xhr, y, z) {
                    Utils.isStartSending = false;
                    if (errorFunction) errorFunction();
                    Utils.hideLoader(btnElm);
                }
            });
        }
    },
    notify: function (message, level, delay) {
        $.notify({
            // options
            icon: 'glyphicon glyphicon-warning-sign',
            message: message
        }, {
                // settings
                type: level || 'success',
                delay: 2000,
                timer: delay == null ? 500 : delay,
                z_index: 100000,
                placement: {
                    from: "top",
                    align: "center"
                },
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                }
            });
    },
    serializeForm: function (form) {
        var o = {};
        var a = $(form).serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        $("input,textarea,select", form).each(function () {

            var name = $(this).attr("name");
            if (!name) return;
            var type = $(this).attr("type");
            var isFound = false;
            for (var prop in o) {
                if (name == prop) {
                    isFound = true;
                }
            }
            if (!isFound) {
                o[name] = $(this).val();
            }
            if (type == "checkbox") {
                o[name] = $(this).is(":checked");
            }
        });
        return o;
    },
    initValudationPlugin: function () {
        $.validator.setDefaults({
            highlight: function (element) {
                $(element).closest('.form-group').addClass('has-error');
            },
            unhighlight: function (element) {
                $(element).closest('.form-group').removeClass('has-error');
            },
            errorElement: 'span',
            errorClass: 'help-block',
            errorPlacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            }
        });

        $.validator.addMethod(
            "stackNumber",
            function (value, element) {
                if (value.length == 0) return true;
                var pattern = new RegExp("^-?\\d*\\.?\\d*$");
                return pattern.test(value);
            },
            "Please enter a valid number."
        );

        $.validator.addMethod(
            "stackNumberPositive",
            function (value, element) {
                if (value.length == 0) return true;
                var number = numeral().unformat(value);
                return number > 0;
            },
            "Please enter a number greater than 0."
        );
        $.validator.addMethod(
            "stackNumberPositiveAndZero",
            function (value, element) {
                if (value.length == 0) return true;
                var number = numeral().unformat(value);
                return number >= 0;
            },
            "Please enter a number greater or equal  0."
        );

        $.validator.addMethod("zipcode", function (value, element) {
            return this.optional(element) || /^\d{5}(?:-\d{4})?$/.test(value);
        }, "Please provide a valid zipcode.");

        $.validator.addMethod("greaterThan",
            function (value, element, params) {
                if (!params || params.length < 1 || !value || !$(params[0]).val()) { return true; }
                
                if (!/Invalid|NaN/.test(new Date(value))) {
                    return new Date(value) > new Date($(params[0]).val());
                }

                return isNaN(value) || isNaN($(params[0]).val())
                    || (Number(value) > Number($(params[0]).val()));
            }, 'Must be greater than {1}.');
        $.validator.addMethod("strongPassword", function (value, element) {
            return this.optional(element) || /^[A-Za-z0-9\d=!\-@._*]*$/.test(value) // consists of only these
                && /[a-z]/.test(value) // has a lowercase letter
                && /[A-Z]/.test(value) // has a uppercase letter
                && /\d/.test(value) // has a digit;
        }, "Your password should contain allowed characters, low/uppercase and digital letter");

    },
    isNumber: function (value) {
        var pattern = new RegExp("^-?\\d*\\.?\\d*$");
        return pattern.test(value);
    },
    addGlobalError: function (message) {
        var alert = '<div class="alert alert-danger fade in" role="alert">' + message + '</div>';
        $("#alert-container").html(alert);
        $(".alert").alert();
    },
    removeGlobalError: function () {
        $(".alert").alert("close");
    },
    setEventEnterSubmit: function (formElm, callBack) {
        $(formElm).off('keypress');
        $(formElm).on('keypress', function (e) {
            if (e.keyCode == 13) {
                if (callBack != null) {
                    callBack();
                }
            }
        });
    },
    delay: function (ms) {
        var cur_d = new Date();
        var cur_ticks = cur_d.getTime();
        var ms_passed = 0;
        while (ms_passed < ms) {
            var d = new Date();  // Possible memory leak?
            var ticks = d.getTime();
            ms_passed = ticks - cur_ticks;
            // d = null;  // Prevent memory leak?
        }
    },

    showOverlay: function () {
        $(".overlay").show();
    },
    hideOverlay: function () {
        $(".overlay").hide();
    },
    showOverlayDialog: function () {
        $("#overlay-dialog").show();
    },
    hideOverlayDialog: function () {
        $("#overlay-dialog").hide();

    },
    getDateText: function (serializateDate) {
        if ($.isEmpty(serializateDate)) return serializateDate;
        if (serializateDate instanceof Date) {
            return dateFormat(serializateDate, "mm/dd/yyyy");
        }
        if (serializateDate == "/Date(-62135596800000)/" || serializateDate == "/Date(-62135578800000)/") {
            return "";
        }

        var dateJson = serializateDate;
        var cleanedValue = dateJson.replace(/\/Date\((-?\d+)\)\//g, "$1");
        var dateValue = parseInt(cleanedValue).toString() == cleanedValue ? parseInt(cleanedValue) : cleanedValue;
        var date = new Date(dateValue);
        var dateText = dateFormat(date, "mm/dd/yyyy");
        return dateText;
    },
    getFormatedDateText: function (serializateDate, format) {
        var date = Utils.getDateFromJson(serializateDate);
        return date ? dateFormat(date, format) : '';
    },
    getDateFromJson: function (serializateDate) {
        if ($.isEmpty(serializateDate)) return serializateDate;
        if (serializateDate instanceof Date) {
            return serializateDate;
        }
        if (serializateDate == "/Date(-62135596800000)/" || serializateDate == "/Date(-62135578800000)/") {
            return;
        }

        var dateJson = serializateDate;
        var cleanedValue = dateJson.replace(/\/Date\((-?\d+)\)\//g, "$1");
        var dateValue = parseInt(cleanedValue).toString() == cleanedValue ? parseInt(cleanedValue) : cleanedValue;
        var date = new Date(dateValue);
        
        return date;
    },
    shrinkText: function (fullText, maxLimit) {

        maxLimit = maxLimit || 80;
        var result = fullText;
        if (result && result.length > maxLimit) {
            result = result.substring(0, maxLimit - 3) + "...";
        }
        return result;
    },
    getCurrentDate: function () {
        var date = new Date();
        var dateText = dateFormat(date, "mm/dd/yyyy");
        return dateText;
    },
    formatPhone: function (phone) {
        if ($.isEmpty(phone)) return "";
        if (phone.length != 10) return phone;
        phone = phone.replace(/(\d\d\d)(\d\d\d)(\d\d\d\d)/, "($1) $2-$3");
        return phone;
    },
    unformatPhone: function (phone) {
        if (phone == null || phone.length == 0) return null;
        phone = phone.replace(/-/g, "").replace("(", "").replace(")", "").replace(/ /g, "").replace(/_/g, "");
        return phone;
    },

    scrollTo: function (elm) {
        $('html, body').animate({
            scrollTop: elm.offset().top
        }, 300);
    },

    scrollToInside: function (container, elm) {
        container.animate({
            scrollTop: elm.offset().top
        }, 300);
    }
}