define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
    //return Backbone.Model.extend({

        formId: "#employee-form",
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
                CategoryId: obj.CategoryId,
                CompanyName: obj.CompanyName,
                Salutation: obj.Salutation,
                FirstName: obj.FirstName,
                MiddleName: obj.MiddleName,
                LastName: obj.LastName,
                Address1: obj.Address1,
                Address2: obj.Address2,
                City: obj.City,
                State: obj.State,
                Zip: obj.Zip,
                IsServiceCoordinator: _this.getBoolean(obj.IsServiceCoordinator),
                JobTitle: obj.JobTitle,
                EmailAddress: obj.EmailAddress,
                Notes: obj.Notes,
                DepartmentId: obj.DepartmentId,
                ContactTypeId: obj.ContactTypeId,
                Phone: obj.Phone,
                MobilePhone: obj.MobilePhone,
                IsUpdate: obj.IsUpdate,
                FileNumber: obj.FileNumber,
                PhoneExtension: obj.PhoneExtension,
                Fax: obj.Fax,
                CCO: obj.CCO
            };
            if (obj.ContactId == -1) {
                objToSet.ContactId = obj.ContactId;
            }
            this.set(objToSet);
        },
        getValidationRules: function (isEdit) {
            var _this = this;
            var validationModel = {
                ignore: [],
                rules: {
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
                    EmailAddress:
                    {
                        minlength: 3,
                        maxlength: 255,
                        email: true
                    },
                    FileNumber:
                    {
                        maxlength: 20
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
                url: "/api/employeesapi/get/" + id,
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
                url: "/api/employeesapi/saveemployee/",
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
                url: "/api/employeesapi/deleteemployee/",
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