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

                },
                error: function (xhr, y, z) {
                    Utils.isStartSending = false;
                    if (errorFunction) errorFunction();
                    Utils.hideLoader(btnElm);
                }
            });
        }
    },
    showLoader: function (btnElm) {
        $(".overlay").show();
    },
    hideLoader: function (btnElm) {
        $(".overlay").hide();
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
    }
}