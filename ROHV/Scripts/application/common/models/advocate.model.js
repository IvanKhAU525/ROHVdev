define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({

        formId: "#advocate-form",
        initialize: function () {
        },
        setFormValidation: function (isEdit) {
            var validationRules = this.getValidationRules(isEdit);
            $(this.formId).validate(validationRules);
        },
        getBoolean: function (val) {
            if (val == null || val == undefined) return null;
            if (val === true || val === false) return val;
            return val == "0" ? false : true;
        },
        parseObj: function (obj) {
            var _this = this;

            var objToSet = {
                Company: obj.Company,
                Specialization: obj.Specialization,
                FirstName: obj.FirstName,
                LastName: obj.LastName,
                Address1: obj.Address1,
                Address2: obj.Address2,
                City: obj.City,
                State: obj.State,
                Zip: obj.Zip,
                Email: obj.Email,
                Phone: obj.Phone,
                Notes: obj.Notes,
                IsUpdate: obj.IsUpdate
            };
            this.set(objToSet);
        },
        getValidationRules: function (isEdit) {
            var validationModel = {
                ignore: [],
                rules: {
                    Company:
                    {
                        required: true,
                    },
                    FirstName:
                    {
                        required: true,
                        minlength: 3,
                        maxlength: 255
                    },
                    LastName:
                   {
                       required: true,
                       minlength: 3,
                       maxlength: 255
                   },
                    Email:
                    {
                        minlength: 3,
                        maxlength: 255,
                        email: true,
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
            if (!$(this.formId).valid()) {
                return "Fail";
            }
        },
        getModel: function (id, success, error) {
            var _this = this;
            Utils.showOverlay();
            this.fetch({
                type: 'GET',
                wait: false,
                contentType: 'application/json; charset=utf-8',
                url: "/api/advocatesapi/get/" + id,
                success: function (model, response) {
                    setTimeout(function () {
                        Utils.hideOverlay();
                        if (response != undefined && response != null) {
                            if (success != null) {
                                success(model, response);
                            }
                        } else {
                            GlobalEvents.trigger("showMessage", { title: "Error", message: "Unxepected error. Please realod the page and try again.", type_class: "alert-danger" });
                            error();
                        }
                    }, 300);
                }, error: function () {
                    Utils.hideOverlay();
                    GlobalEvents.trigger("showMessage", { title: "Error", message: "Unxepected error. Please realod the page and try again.", type_class: "alert-danger" });
                    error(model, response);
                }
            });
        },
        postData: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/advocatesapi/saveadvocate/",
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
        deleteData: function (id, success, error) {
            $(".overlay", "#users-container").show();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ id: id }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/advocatesapi/deleteadvocate/",
                success: function (response) {
                    $(".overlay", "#users-container").hide();
                    if (response.status == "ok") {
                        Utils.notify("The employee has been successfully deleted from the system.");
                        if (success != null) {
                            success();
                        }
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Error", message: response.message, type_class: "alert-danger" });
                    }
                }, error: function () {
                    $(".overlay", "#users-container").hide();
                    if (error != null) {
                        error();
                    }
                    GlobalEvents.trigger("showMessage", { title: "Error", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
                }
            });
        }
    });

});