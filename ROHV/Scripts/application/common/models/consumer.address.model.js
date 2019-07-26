define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#address-form",
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);          
        },
        parseObj: function (obj) {

            var objToSet = {                
                Address1: obj.Address1,
                Address2: obj.Address2,
                City: obj.City,
                State: obj.State,
                Zip: obj.Zip,
                FromDate: obj.FromDate,
                ToDate: obj.ToDate       
            };
            this.set(objToSet);
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                   
                    Address1:
                    {
                        required:true
                    },
                    City:
                    {
                        required: true
                    },
                    State:
                    {
                        required: true
                    },
                    Zip:
                    {
                        required: true
                    },
                    FromDate:
                    {
                        required: true,
                        date: true
                    },
                    ToDate:
                    {                      
                        date: true,
                        greaterThan: ["#FromDate", "From Date"]
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
                url: "/api/consumeraddressapi/save/",
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