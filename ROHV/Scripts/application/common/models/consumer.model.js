define(function (require) {

    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({

        formId: "#consumer-info-form",
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
        getDate: function (date) {
            var dpg = $.fn.datepicker.DPGlobal;
            return dpg.parseDate(date, dpg.parseFormat('mm/dd/yyyy')).toISOString().replace("Z", "");
        },
        parseObj: function (obj) {

            var objToSet = {
                FirstName: obj.FirstName,
                MiddleName: obj.MiddleName,
                LastName: obj.LastName,
                AdvocateId: obj.AdvocateId,
                AdvocateName: obj.AdvocateName,
                AdvocatePaperId: obj.AdvocatePaperId,
                AdvocatePaperName: obj.AdvocatePaperName,
                Address1: obj.Address1,
                Address2: obj.Address2,
                City: obj.City,
                State: obj.State,
                Zip: obj.Zip,
                ParentName: obj.ParentName,
                Status: obj.Status,
                Gender: obj.Gender,
                DateOfBirth: this.getDate(obj.DateOfBirth),
                MedicaidNo: obj.MedicaidNo,
                SocialSecurityNo: obj.SocialSecurityNo,
                PrimaryDiagnosis: obj.PrimaryDiagnosis,
                SecondaryDiagnosis: obj.SecondaryDiagnosis,
                TABSNo: obj.TABSNo,
                HasServiceCoordinator: obj.HasServiceCoordinator,
                ServiceCoordinator: obj.ServiceCoordinator,
                DH: obj.DH,
                CH: obj.CH,
                AgencyName: obj.AgencyName,
                DayProgramId: obj.DayProgramId,
                DayProgramName: obj.DayProgramName,
                Cc2: obj.Cc2
            };
            this.set(objToSet);
        },
        getValidationRules: function () {
            var validationModel = {
                ignore: [],
                rules: {
                    FirstName:
                    {
                        required: true,
                        maxlength: 50
                    },
                    MiddleName:
                    {
                        maxlength: 50
                    },
                    LastName:
                    {
                        required: true,
                        maxlength: 50
                    },
                    Address1:
                    {
                        required: true,
                        maxlength: 200
                    },
                    Address2:
                    {
                        maxlength: 50
                    },
                    City:
                    {
                        required: true,
                        maxlength: 50
                    },
                    State:
                    {
                        required: true,
                        maxlength: 10
                    },
                    Zip:
                    {
                        required: true,
                        zipcode: true
                    },
                    ParentName:
                    {
                        maxlength: 100
                    },
                    Cc2:
                    {
                        maxlength: 10
                    },
                    DateOfBirth:
                    {
                        required: true,
                        date: true
                    },
                    MedicaidNo:
                    {
                        required: true,
                        maxlength: 50
                    },
                    SocialSecurityNo:
                    {
                        required: true,
                        maxlength: 50
                    },
                    AgencyName:
                    {
                        maxlength: 50
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
        getModel: function (id, success, error) {
            var _this = this;
            Utils.showOverlay();
            this.fetch({
                type: 'GET',
                wait: false,
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                url: "/api/consumerapi/get/" + id,
                success: function (model, response) {
                    setTimeout(function () {
                        Utils.hideOverlay();
                        if (response != undefined && response != null) {
                            if (success != null) {
                                success();
                            }
                        } else {
                            GlobalEvents.trigger("showMessage", { title: "Error", message: "Unxepected error. Please realod the page and try again.", type_class: "alert-danger" });
                        }
                    }, 300);
                }, error: function () {
                    Utils.hideOverlay();
                    GlobalEvents.trigger("showMessage", { title: "Error", message: "Unxepected error. Please realod the page and try again.", type_class: "alert-danger" });
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
                url: "/api/consumerapi/save/",
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
        deleteData: function (id, success, error) {
            Utils.showOverlay();
            $.ajax({
                type: 'DELETE',
                wait: false,
                data: JSON.stringify({ id: id }),
                contentType: 'application/json; charset=utf-8',
                url: "/api/consumerapi/delete/",
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
        deletePhoneData: function (consumerPhoneId, success, error) {
            $.ajax({
                url: "/api/consumerphoneapi/delete/",
                type: "POST",
                data: { phoneId: consumerPhoneId },
                success: success,
                dataType: "json"
            });
        },
        deleteMedicaidNumber: function(id, success, error) {
            $.ajax({
                url: "/api/consumermedicaidnumberapi/delete/",
                type: "POST",
                data: { id: id },
                success: success,
                dataType: "json"
            });
        },
        deleteCoordinatorService: function (Id, success, error) {
            $.ajax({
                url: "/api/coordinatorserviceapi/delete/",
                type: "POST",
                data: { Id: Id },
                success: success,
                dataType: "json"
            });
        },
        deleteAddress: function (Id, success, error) {
            $.ajax({
                url: "/api/consumeraddressapi/delete/",
                type: "POST",
                data: { Id: Id },
                success: success,
                dataType: "json"
            });
        },
        getConsumerId: function () {
            return this.attributes.ConsumerId;
        }
    });

});