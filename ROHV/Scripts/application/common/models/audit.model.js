define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        initialize: function () {
        },
        formId: "#audit-form",
        parseObj: function (obj) {

            var objToSet = {
                Id: obj.Id,
                ServiceId: obj.ServiceId,
                AuditDate: obj.AuditDate,
                AuditId: obj.AuditId,
                Consumers: obj.Consumers,
                NumberOfAuditRecords: obj.NumberOfAuditRecords
            };

            this.set(objToSet);
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    NumberOfAuditRecords:
                        {
                            required: true,
                            minlength: 1,
                            maxlength: 2
                        },
                    ServiceId:
                        {
                            required: true
                        }
                },
                messages: {
                }
            };
            return validationModel;
        },
        validate: function (attrs, options) {

            if (this.formId == null) return;
            if ($(this.formId).length == 0) return;
            return !$(this.formId).valid();
        },
        
        AddAudit: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.save(null, {
                type: 'POST',               
                url: "/api/consumerauditapi/addnewaudit",
                success: function (model, response) {
                    if (response.status == "ok") {
                        if (success != null) {
                            success(response);
                        }
                    } else {
                        if (error != null) {
                            error();
                        }
                        GlobalEvents.trigger("showMessage", { title: "Error saving...", message: response.message, type_class: "alert-danger" });
                    }

                }, error: function () {
                    if (error != null) {
                        error();
                    }
                    GlobalEvents.trigger("showMessage", { title: "Error saving...", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
                }
            });
        },
        deleteData: function (Id, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ Id: Id }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumerauditapi/delete/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        Utils.notify("The audit record has been successfully deleted from the system.");
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
    });

});