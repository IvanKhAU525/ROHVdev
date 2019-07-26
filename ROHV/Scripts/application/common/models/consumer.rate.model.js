define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#rate-form",
        initialize: function () {},
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
        },
        parseObj: function (obj) {

            var objToSet = {
                ConsumerId: obj.ConsumerId,
                Rate: obj.Rate,
                MaxHoursPerWeek: obj.MaxHoursPerWeek,
                MaxHoursPerYear: obj.MaxHoursPerYear,
                RateNote: obj.RateNote
            };            
            this.set(objToSet);            
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    Rate:
                    {
                        number: true
                    },
                    MaxHoursPerWeek:
                    {
                        digits: true
                    },
                    MaxHoursPerYear:
                    {
                        digits: true
                    },
                    RateNote:
                    {
                        maxlength: 2024
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

        postData: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            Utils.showOverlay();
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumerapi/saverate/",
                success: function (model, response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        if (success != null) {
                            success(response);
                        }
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Error saving...", message: response.message, type_class: "alert-danger" });
                    }

                }, error: function () {
                    Utils.hideOverlay();
                    GlobalEvents.trigger("showMessage", { title: "Error saving...", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
                }
            });
        }
    });

});