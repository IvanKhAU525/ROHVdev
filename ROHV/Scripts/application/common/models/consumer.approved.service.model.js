define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#aproved-service-form",
        defaults: {
            "UnitQuantities": ""
        },
        initialize: function () {

        }
        ,
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
                ConsumerId: obj.ConsumerId,
                AgencyDate: obj.AgencyDate,
                AnnualUnits: obj.AnnualUnits,
                EffectiveDate: obj.EffectiveDate,
                ServiceId: obj.ServiceId,
                ServiceName: obj.ServiceName,
                UnitQuantities: obj.UnitQuantities,
                UnitQuantitiesName: obj.UnitQuantitiesName,
                DWorkers: obj.DWorkers,
                Inactive: obj.Inactive,
                Notes: obj.Notes,
                DateInactive: obj.DateInactive,
                TotalHours: obj.TotalHours,
                FileId: obj.FileId,
                FileData: obj.FileData,
                FileName: obj.FileName,
                UsedHoursStartDate: obj.UsedHoursStartDate,
                UsedHoursEndDate: obj.UsedHoursEndDate,
                UsedHours: obj.UsedHours

            };
            this.set(objToSet);
        },
        validateEmailed: function () {
            return true;
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    ServiceId:
                    {
                        required: true
                    },
                    AnnualUnits:
                    {
                        digits: true
                    },
                    EffectiveDate:
                    {
                        date: true
                    },
                    AgencyDate:
                    {
                        date: true
                    },
                    DateInactive:
                    {
                        date: true
                    },
                    UsedHoursStartDate: {
                        date: true,
                        required: function (el) {
                            return !!$("#UsedHoursEndDate").val();
                        },
                    },
                    UsedHoursEndDate: {
                        date: true,
                        required: function (el) {
                            return !!$("#UsedHoursStartDate").val();
                        },
                        greaterThan: ["#UsedHoursStartDate", "Start Date"]
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

        },
        deleteData: function (serviceId, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ serviceId: serviceId }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumerservicesapi/delete/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        Utils.notify("The user has been successfully deleted from the system.");
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
                url: "/api/consumerservicesapi/save/",
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

        getTotalHours: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            Utils.showOverlay();
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumerservicesapi/gettotalhours/",
                success: function (model, response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        if (success != null) {
                            success(response);
                        }
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Error getting data...", message: response.message, type_class: "alert-danger" });
                    }

                }, error: function () {
                    Utils.hideOverlay();
                    GlobalEvents.trigger("showMessage", { title: "Error getting data...", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
                }
            });
        }
    });

});