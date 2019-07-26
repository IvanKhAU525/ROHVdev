define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#document-form",
        defaults: {
            Notes: []
        },
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
        },
        setNoteFormValidation: function () {
            var validationRules = this.getValidationRulesForNote();
            $("#add-document-note").validate(validationRules);
        },
        resetField: function (elm) {
            var parent = elm.parent().parent();
            $(parent).removeClass("has-error");
            $(".help-block", parent).hide();
        },
        parseObj: function (obj) {

            var objToSet = {
                ConsumerId: obj.ConsumerId,
                DocumentTypeName: obj.DocumentTypeName,
                DocumentTypeColor: obj.DocumentTypeColor,
                DocumentTypeId: obj.DocumentTypeId,
                DocumentStatusName: obj.DocumentStatusName,
                DocumentStatusId: obj.DocumentStatusId,
                NumberNotes: obj.NumberNotes,
                Notes: obj.Notes,
                EmployeeName: obj.EmployeeName,
                EmployeeCompanyName: obj.EmployeeCompanyName,
                EmployeeId: obj.EmployeeId,
                AddedByName: obj.AddedByName,
                AddedById: obj.AddedById,
                DateCreated: obj.DateCreated,
                UpdatedByName: obj.UpdatedByName,
                UpdatedById: obj.UpdatedById,
                DateUpdated: obj.DateUpdated,
                DateDocument: obj.DateDocument
            };
            if (obj.FileData != null) {
                objToSet.FileData = obj.FileData;
            }
            this.set(objToSet);
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    DocumentTypeId:
                    {
                        required: true
                    },
                    DocumentStatusId:
                    {
                        required: true
                    },
                    EmployeeId:
                    {
                        required: true
                    },
                    DateDocument:
                    {
                        date: true
                    }
                },
                messages: {
                    DocumentTypeId:
                    {
                        required: "This field is required."
                    },
                    DocumentStatusId:
                    {
                        required: "This field is required."
                    },
                    EmployeeId:
                    {
                        required: "This field is required."
                    }
                }
            };
            return validationModel;
        },
        getValidationRulesForNote: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    DocumentNote:
                    {
                        required: true,
                        maxlength: 1024
                    }
                },
                messages: {
                }
            };
            return validationModel;
        },
        validate: function (attrs, options) {
            if (!$(this.formId).valid()) {
                return "Fail";
            }
        },
        validateNote: function () {
            if (!$("#add-document-note").valid()) {
                return false;
            }
            return true;
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
                url: "/api/consumerdocumentsapi/delete/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        Utils.notify("The document has been successfully deleted from the system.");
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
                url: "/api/consumerdocumentsapi/save/",
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