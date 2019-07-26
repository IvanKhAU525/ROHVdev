define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#print-document-form",
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
            $("#print-document-valued-form").validate({ ignore: [] });
        },
        setDownloadFormValidation: function () {
            var validationRules = this.getValidationDownloadRules();
            $("#print-document-download-form").validate(validationRules);
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
                EffectiveDate: obj.EffectiveDate,
                AddedByName: obj.AddedByName,
                AddedById: obj.AddedById,
                Notes: obj.Notes,
                DateCreated: obj.DateCreated,
                UpdatedByName: obj.UpdatedByName,
                UpdatedById: obj.UpdatedById,
                DateUpdated: obj.DateUpdated,
                StatusName: obj.StatusName,
                ServiceTypeId: obj.ServiceTypeId,
                ServiceTypeName: obj.ServiceTypeName,
                ValuedOutcomes: obj.ValuedOutcomes,
                ServiceTypeTitle: obj.ServiceTypeTitle
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
                    EffectiveDate:
                   {
                       required:
                           function () {
                               if ($("#ServiceTypeId").val() != 3) {
                                   return true;
                               }
                               return false;
                           }
                   },
                    ServiceTypeId:
                    {
                        required: true
                    }
                },
                messages: {
                    ContactId:
                    {
                        required: "This field is required."
                    },
                    ServiceTypeId:
                    {
                        required: "This field is required."
                    }
                }
            };
            return validationModel;
        },
        getValidationDownloadRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    PrintDocumnentTypeId:
                    {
                        required: true
                    }
                },
                messages: {
                    PrintDocumnentTypeId:
                    {
                        required: "This field is required."
                    }
                }
            };
            return validationModel;
        },

        setValidationRulesForValued: function (element) {
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
            return $("#print-document-valued-form").valid();
        },

        validate: function (attrs, options) {
            if ($.isEmpty(this.formId)) return;
            var isValid = this.validValued();
            if (!$(this.formId).valid() || !isValid) {
                return "Fail";
            }
        },
        validateDownload: function () {
            if (!$("#print-document-download-form").valid()) {
                return false;
            }
            return true;
        },
        validateEmailed: function () {
            return true;
        },
        showOverlay: function () {
            $("#overlay-dialog").show();
        },
        hideOverlay: function () {
            $("#overlay-dialog").hide();

        },
        postData: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            Utils.showOverlay();
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumerdocumnetprintapi/save/",
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
                    if (error != null) {
                        error();
                    }
                    GlobalEvents.trigger("showMessage", { title: "Error saving...", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
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
        },
        deleteData: function (id, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ id: id }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumerdocumnetprintapi/delete/",
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
        }
    });

});