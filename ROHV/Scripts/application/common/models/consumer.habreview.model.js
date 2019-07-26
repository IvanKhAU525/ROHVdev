define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#hab-review-form",
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
        getBoolean: function (val) {
            if (val == null || val == undefined) return null;
            if (val === true || val === false) return val;
            return val == "0" ? false : true;
        },
        parseObj: function (obj) {

            var objToSet = {
                ServiceName: obj.ServiceName,
                ServiceId: obj.ServiceId,

                CHCoordinator: obj.CHCoordinator,
                DHCoordinator: obj.DHCoordinator,

                IsAutoSignature: obj.IsAutoSignature,

                MSC: obj.MSC,                
                
                Parents: obj.Parents,
                Others: obj.Others,                
                Others2: obj.Others2,
                Others3: obj.Others3,

                IsIncludeIndividialToParticipant: this.getBoolean(obj.IsIncludeIndividialToParticipant),
                IsMSCParticipant: this.getBoolean(obj.IsMSCParticipant),

                Notes: obj.Notes,

                AddedByName: obj.AddedByName,
                AddedById: obj.AddedById,

                DateCreated: obj.DateCreated,
                DateReview: obj.DateReview,
                SignatureDate: obj.SignatureDate,

                UpdatedByName: obj.UpdatedByName,
                UpdatedById: obj.UpdatedById,

                DateUpdated: obj.DateUpdated,

                ConsumerId: obj.ConsumerId,

                ConsumerHabReviewIssueStates: obj.ConsumerHabReviewIssueStates,

                ConsumerHabReviewId: obj.ConsumerHabReviewId
            };
            this.set(objToSet);
        },
        getValidationRules: function () {

            var validationModel = {
                ignore: [],
                rules: {                
                    ServiceId:
                    {
                        required: true
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
            var isValid = true;
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
                url: "/api/consumerhabreviewsapi/delete/",
                success: function (response) {
                    Utils.hideOverlay();
                    if (response.status == "ok") {
                        Utils.notify("The hab review has been successfully deleted from the system.");
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
        getSignatureDate: function () {
            var returnValue = this.get('SignatureDate');
            if (!returnValue && this.get('DHCoordinator')) {
                this.set('SignatureDate', Utils.getDateText(new Date()));
            }
            return this.get('SignatureDate');
        },
        postData: function (success, error) {
            var _this = this;
            var model = this.clone();
            model.formId = this.formId;
            Utils.showOverlay();
            model.save(null, {
                type: 'POST',
                wait: false,
                url: "/api/consumerhabreviewsapi/save/",
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