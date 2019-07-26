define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#employee-form",
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
                ConsumerId: obj.ConsumerId,
                ContactId: obj.ContactId,
                ContactName: obj.ContactName,
                StartDate: obj.StartDate,
                EndDate: obj.EndDate,
                ServiceId: obj.ServiceId,
                ServiceName: obj.ServiceName,
                Email: obj.Email,
                CompanyName: obj.CompanyName,
                Rate: obj.Rate,
                MaxHoursPerWeek: obj.MaxHoursPerWeek,
                MaxHoursPerYear: obj.MaxHoursPerYear,
                RateNote: obj.RateNote,                
                FileId: obj.FileId,
                FileData: obj.FileData,
                FileName: obj.FileName
            };
            this.set(objToSet);
        },
        getValidationRules: function () {

            var validationModel = {
                ignore: [],
                rules: {
                    ContactId:
                    {
                        required: true
                    },
                    ServiceId:
                    {
                        required: true
                    },
                    StartDate:
                    {
                        date: true
                    },
                    EndDate:
                    {
                        date: true
                    },
                    Rate:
                    {
                        number: true
                    },
                    MaxHoursPerWeek:
                    {
                        number: true
                    },
                    MaxHoursPerYear:
                    {
                        number: true
                    }

                },
                messages: {
                    ContactId:
                    {
                        required: "This field is required."
                    },
                    ServiceId:
                    {
                        required: "This field is required."
                    }
                }
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
        deleteData: function (serviceId, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ serviceId: serviceId }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumeremployeeapi/delete/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        Utils.notify("The employee has been successfully deleted from the system.");
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
                url: "/api/consumeremployeeapi/save/",
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