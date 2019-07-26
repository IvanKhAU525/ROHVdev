define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#hab-plan-form",
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
            $("#hab-plan-valued-form").validate({ ignore: [] });
            $("#hab-plan-safeguard-form").validate({ ignore: [] });
        },
        resetField: function (elm) {
            var parent = elm.parent().parent();
            $(parent).removeClass("has-error");
            $(".help-block", parent).hide();
        },
        parseObj: function (obj) {


            var objToSet = {
                Name: obj.Name,
                HabServiceName: obj.HabServiceName,
                HabServiceId: obj.HabServiceId,
                StatusName: obj.StatusName,
                StatusId: obj.StatusId,
                DurationId: obj.DurationId,
                FrequencyId: obj.FrequencyId,
                IsAproved: obj.IsAproved,
                DatePlan: obj.DatePlan,
                SignatureDate: obj.SignatureDate,
                EnrolmentDate: obj.EnrolmentDate,
                EffectivePlan: obj.EffectivePlan,
                Coordinator: obj.Coordinator,
                IsAutoSignature: obj.IsAutoSignature,
                AddedByName: obj.AddedByName,
                AddedById: obj.AddedById,
                DateCreated: obj.DateCreated,
                UpdatedByName: obj.UpdatedByName,
                UpdatedById: obj.UpdatedById,
                DateUpdated: obj.DateUpdated,
                ValuedOutcomes: obj.ValuedOutcomes,
                Safeguards: obj.Safeguards,
                QMRP: obj.QMRP,
                ConsumerId: obj.ConsumerId
            };
            this.set(objToSet);
        },
        getValidationRules: function () {

            var validationModel = {
                ignore: [],
                rules: {
                    Name:
                    {
                        required: true,
                        maxlength: 255
                    },
                    HabServiceId:
                    {
                        required: true
                    },
                    CoordinatorId:
                    {
                        required: true
                    },
                    FrequencyId:
                    {
                        required: true
                    },
                    DurationId:
                    {
                        required: true
                    },
                    QMRP:
                    {
                        required: true,
                        maxlength: 255
                    },
                    StatusId:
                    {
                        required: true
                    },
                    EnrolmentDate:
                    {
                        required: true,
                        date: true
                    },
                    DatePlan:
                    {
                        required: true,
                        date: true
                    },
                    SignatureDate:
                    {
                        date: true
                    },
                    EffectivePlan:
                    {
                        required: true,
                        date: true
                    }
                },
                messages: {
                    HabServiceId:
                    {
                        required: "This field is required."
                    },
                    FrequencyId:
                    {
                        required: "This field is required."
                    },
                    DurationId:
                    {
                        required: "This field is required."
                    },
                    StatusId:
                    {
                        required: "This field is required."
                    }
                }
            };
            return validationModel;
        },
        setValidationRulesForValued: function (element) {
            if (element.hasClass("rohv-not-required")) {
                return;
            }
            element.rules("add", {
                required: true,
                minlength: 2,
                messages: {
                    required: "This field is required.",
                    minlength: jQuery.validator.format("Please, at least {0} characters are necessary")
                }
            });
        },
        setValidationRulesForSafeguard: function (element) {
            if (element.hasClass("rohv-not-required")) {
                return;
            }
            element.rules("add", {
                required: true,
                minlength: 2,
                messages: {
                    required: "This field is required.",
                    minlength: jQuery.validator.format("Please, at least {0} characters are necessary")
                }
            });
        },
        validValued: function () {
            return $("#hab-plan-valued-form").valid();
        },
        validSafeguards: function () {
            return $("#hab-plan-safeguard-form").valid();
        },
        validate: function (attrs, options) {
            if ($.isEmpty(this.formId)) return;
            var isValid = this.validValued() && this.validSafeguards();
            if (!$(this.formId).valid() || !isValid) {
                return "Fail";
            }
        },
        showOverlay: function () {
            $("#overlay-dialog").show();
        },
        hideOverlay: function () {
            $("#overlay-dialog").hide();

        },
        deleteData: function (id, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ id: id }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumerhabplansapi/delete/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        Utils.notify("The hab plan has been successfully deleted from the system.");
                        if (success != null) {
                            success();
                        }
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Error", message: response.message, type_class: "alert-danger" });
                    }
                }, error: function () {
                    Utils.hideOverlay();
                    GlobalEvents.trigger("showMessage", { title: "Error", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
                }
            });
        },

        postData: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            Utils.showOverlay();
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumerhabplansapi/save/",
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
        },
        checkPassword: function (password, contactId, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'POST',
                wait: false,
                data: JSON.stringify({ password: password, contactId: contactId }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumerapi/checkPassword/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        success(true);
                    } else {
                        success(false);
                    }
                }, error: function () {
                    Utils.hideOverlay();
                    error();
                }
            });
        },
        request: function (url, objToSend, success, error) {
            this.formId = null;
            this.save(null, {
                type: 'POST',
                wait: false,
                data: JSON.stringify(objToSend),
                contentType: 'application/json; charset=utf-8',
                url: url,
                success: function (model, response) {
                    if (success != null) {
                        success(response);
                    }
                }, error: function (model, response) {
                    if (error != null) {
                        error();
                    }
                }
            });
        }
    });

});