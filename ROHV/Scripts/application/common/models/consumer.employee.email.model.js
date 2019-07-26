define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#employee-email-form",
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
        },    
        parseObj: function (obj) {

            var objToSet = {
                Subject: obj.Subject,
                Message: obj.Message
            };
            this.set(objToSet);
        },
        getValidationRules: function () {

            var validationModel = {
                ignore: [],
                rules: {
                    Subject:
                    {
                        required: true,
                        maxlength:1024
                    },
                    Message:
                    {
                        required: true,
                        maxlength:2048

                    }                   
                },
                messages: {}
            };
            return validationModel;
        },
        validate: function (attrs, options) {            
            if (!$(this.formId).valid()) {
                return "Fail";
            }
        },
        showOverlay: function () {
            $("#overlay-dialog").show();
        },
        hideOverlay: function () {
            $("#overlay-dialog").hide();

        },
        send: function (success, error) {
            Utils.showOverlayDialog();
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumeremployeeapi/sendemail/",
                success: function (model, response) {
                    Utils.hideOverlayDialog();
                    if (response.status == "ok") {
                        Utils.notify("The email has been successfully sent.");
                        if (success != null) {
                            success();
                        }
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Error sending...", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });                        
                    }

                }, error: function () {
                    Utils.hideOverlayDialog();
                    if (error != null)
                    {
                        error();
                    }
                    GlobalEvents.trigger("showMessage", { title: "Error sending...", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
                }
            });
        }
    });

});