define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#approved-service-emailed-form",       
        initialize: function () {
         
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
        },
        resetField: function (elm) {
            var parent = elm.parent().parent();
            $(parent).removeClass("has-error");
            $(".help-block", parent).hide();
        },
        parseObj: function (obj) {

            var objToSet = {
                EmailWorker: obj.EmailWorker,
                Message: obj.Message
            };
            this.set(objToSet);
        },
        validateEmailed: function () {
            return $(this.formId).valid();
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    EmailWorker:
                    {
                        required: true
                    }
                },
                messages: {
                    ServiceId:
                    {
                        required: "This field is required."
                    }
                }
            };
            return validationModel;
        },
        validate: function (attrs, options) {
            if ($.isEmpty(this.formId)) return;
            if (!$(this.formId).valid()) {
                return "Fail";
            }
        },
        showOverlay: function () {
            $("#overlay-dialog").show();
        },
        hideOverlay: function () {
            $("#overlay-dialog").hide();

        }
    });

});