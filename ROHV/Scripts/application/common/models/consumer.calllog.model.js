define(function (require) {

    require('backbone');
    require('underscore');
    
    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#call-log-form",
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
                CalledOn: obj.CalledOn,
                AddedByName: obj.AddedByName,
                AddedById: obj.AddedById,
                Notes: obj.Notes,
                DateCreated: obj.DateCreated,
                UpdatedByName: obj.UpdatedByName,
                UpdatedById: obj.UpdatedById,
                DateUpdated: obj.DateUpdated
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
                    CalledOn:
                    {
                        required: true,
                        date:true
                    },
                    Notes:
                    {
                        maxlength: 255
                    }
                },
                messages: {
                    ContactId:
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
        postData: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            Utils.showOverlay();
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumercalllogsapi/save/",
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
        deleteData: function (id, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ id: id }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumercalllogsapi/delete/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        Utils.notify("The call log has been successfully deleted from the system.");
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