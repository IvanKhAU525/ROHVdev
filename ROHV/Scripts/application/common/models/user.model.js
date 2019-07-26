define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#user-form",
        initialize: function () {
        },
        setFormValidation: function (isEdit) {
            var validationRules = this.getValidationRules(isEdit);
            $(this.formId).validate(validationRules);
        },
        parseObj: function (obj) {

            var objToSet = {
                FirstName: obj.FirstName,
                LastName: obj.LastName,
                Email: obj.Email,                
                Role: obj.Role,                
                Password: obj.Password,
                IsUpdate: obj.IsUpdate,                
                CanManageServices: obj.CanManageServices,
                EmailPassword: obj.EmailPassword
            };
            if (obj.UserId == -1) {
                objToSet.UserId = obj.UserId;
            }
            this.set(objToSet);
        },
        getValidationRules: function (isEdit) {            
            var validationModel = {
                ignore: [],
                rules: {
                    FirstName:
                    {
                        minlength: 3,
                        maxlength: 255,
                        required: true
                    },
                    LastName:
                   {
                       minlength: 3,
                       maxlength: 255,
                       required: true
                   },
                    Email:
                    {
                        minlength: 3,
                        maxlength: 255,
                        required: true,
                        email: true
                    },
                    EmailPassword:
                    {
                        maxlength: 30
                    }
                },
                messages: {                  
                }
            };
            if (!isEdit) {
                validationModel["rules"]["Password"] = {
                    minlength: 8,
                    maxlength: 128,
                    required: true                  
                };
                validationModel["rules"]["ConfirmPassword"] = {
                    equalTo: "#Password"
                };
            }
            return validationModel;
        },
        setFormChangePasswordValidation: function () {
            this.formId = null;
            var validationRules = {
                ignore: [],
                rules: {
                    Password:
                    {
                        minlength: 8,
                        maxlength: 128,
                        required: true,
                        strongPassword: true
                    },
                    ConfirmPassword:
                   {
                       equalTo: "#Password"
                   }
                }
            };
            $("#user-change-password-form").validate(validationRules);
        },
        validChangePassword:function()
        {
            return $("#user-change-password-form").valid();
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
                url: "/api/usersapi/get/" + id,
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
                url: "/api/usersapi/saveuser/",
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
        updateProfileData: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/usersapi/updateprofile/",
                success: function (model, response) {
                    if (response.status == "ok") {
                        if (success != null) {
                            success(response);
                        }
                    } else {
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
        deleteData: function (success, error) {
            $(".overlay", "#users-container").show();            
            this.save(null, {
                type: 'DELETE',
                wait: false,
                contentType: 'application/json; charset=utf-8',
                url: "/api/usersapi/deleteuser/",
                success: function (model, response) {
                    $(".overlay", "#users-container").hide();
                    if (response.status == "ok") {
                        Utils.notify("The user has been successfully deleted from the system.");
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
        sendChangePassword:function(obj, success)
        {
            this.set(obj);
            this.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/usersapi/changepassword/",
                success: function (model, response) {
                    if (response.status == "ok") {
                        if (success != null) {
                            success(response);
                        }
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Error changing...", message: response.message, type_class: "alert-danger" });
                    }

                }, error: function () {
                    if (error != null) {
                        error();
                    }
                    GlobalEvents.trigger("showMessage", { title: "Error changing...", message: "Something is going wrong. Please try again.", type_class: "alert-danger" });
                }
            });

        }
    });

});