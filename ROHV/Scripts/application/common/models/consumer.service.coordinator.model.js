define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#service-coordinator-form",
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);          
        },
        parseObj: function (obj) {

            var objToSet = {                
                ContactId: obj.ContactId,
                FromDate: obj.FromDate,
                ToDate: obj.ToDate       
            };
            this.set(objToSet);
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    ConsumerId:
                    {
                        required: true    
                    },
                    ContactId:
                    {
                        required:true
                    },
                    FromDate:
                    {
                        required: true,
                        date: true
                    },
                    ToDate:
                    {                   
                        date: true,
                        greaterThan: ["#FromDate","From Date"]
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
                url: "/api/coordinatorserviceapi/save/",
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