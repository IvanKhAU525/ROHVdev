define(function (require) {

    require('backbone');
    require('underscore');
    
    var baseModel = require('models/base.model');

    return baseModel.extend({
        formId: "#notification-form",    
        initialize: function () {
        },
        setFormValidation: function () {
            var validationRules = this.getValidationRules();
            $(this.formId).validate(validationRules);
        },
        setRecipientFormValidation:function()
        {
            var validationRules = this.getValidationRecipientRules();
            $("#recipient-form").validate(validationRules);            
        },
        resetField: function (elm) {
            var parent = elm.parent().parent();
            $(parent).removeClass("has-error");
            $(".help-block", parent).hide();
        },
        normalizeRedipients: function (objToSet)
        {
            for (var i = 0; i < objToSet.Recipients.length; i++)
            {
                objToSet.Recipients[i].Id = (i + 1);
            }
        },
        parseObj: function (obj) {

            var objToSet = {              
              ConsumerId: obj.ConsumerId,
              RepetingTypeId: obj.RepetingTypeId,
              RepetingTypeName: obj.RepetingTypeName,
              StatusId: obj.StatusId,
              StatusName: obj.StatusName,
              Name: obj.Name,
              Note: obj.Note,
              DateStart: obj.DateStart,
              AddedByName: obj.AddedByName,
              AddedById: obj.AddedById,
              DateCreated: obj.DateCreated,
              UpdatedByName: obj.UpdatedByName,
              UpdatedById: obj.UpdatedById,
              DateUpdated: obj.DateUpdated,
              Recipients: obj.Recipients,
              RecipientsString: obj.RecipientsString
            };
            this.set(objToSet);
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    RepetingTypeId:
                    {
                        required: true
                    },
                    StatusId:
                    {
                        required: true                    
                    },
                    Name:
                    {
                        required: true,
                        maxlength:100
                    },
                    Note:
                    {
                        required:true,
                        maxlength:1024
                    },
                    DateStart:
                    {
                        required:true,
                        date:true
                    }
                },
                messages: {
                    RepetingTypeId:
                    {
                        required: "This field is required."
                    },
                    StatusId:
                    {
                        required: "This field is required."
                    }
                }
            };
            return validationModel;
        },
        getValidationRecipientRules: function () {
            var validationModel = {
                ignore: [],
                rules: {                    
                    Email:
                    {
                        required: true,
                        email:true
                    },
                    Position:
                    {
                        maxlength: 100
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
        validateRecipient: function () {
            if (!$("#recipient-form").valid()) {
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
                url: "/api/consumernotificationsapi/delete/",
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
                url: "/api/consumernotificationsapi/save/",
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