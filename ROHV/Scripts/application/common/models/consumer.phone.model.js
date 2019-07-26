define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#phone-form",
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);          
        },
        parseObj: function (obj) {

            var objToSet = {                
                Extension: obj.Extension,
                Note: obj.Note,
                Phone: Utils.unformatPhone(obj.PhoneFormated),
                PhoneTypeId: obj.PhoneTypeId,
                PhoneTypeName: obj.PhoneTypeName       
            };
            this.set(objToSet);
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    PhoneTypeId:
                    {
                        required: true    
                    },
                    PhoneFormated:
                    {
                        required:true
                    },
                    Note:
                    {
                        maxlength: 100
                    },
                    Extension:
                    {
                        maxlength: 50
                    }
                },
                messages: {}
            };
            return validationModel;
        },
        postData: function (consumerId, success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            model.attributes.ConsumerId = consumerId;
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumerphoneapi/save/",
                success: function (model, response) {
                    if (success) {
                        success(model, response);
                    }
                }, error: function (err) {
                    if (error) {
                        error(err);
                    }
                }
            });
        },
        removePostData: function (consumerPhoneId, success, error) {
            $.ajax({
                url: "/api/consumerphoneapi/delete/",
                type: "POST",
                data: { phoneId: consumerPhoneId },
                success: success,
                dataType: "json"
            });
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

        }
    });

});