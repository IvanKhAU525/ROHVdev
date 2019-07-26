define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/user.model'),
        ChangePasswordModalView = require('views/users/user.changepassword.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        events: function () {
            return {
                "click [data-action=change-password]": "OnSetPassword",
                "click [data-action=submit-form]": "onSave",

            }
        },
        template: function (serialized_model) {
            var templateHtml = Marionette.TemplateCache.get("profile");
            var templateHtml2 = Marionette.TemplateCache.get("profile/templates");
            var template = _.template(templateHtml);

            return template({ model: serialized_model, templates: templateHtml2 });
        },
        serializeData: function () {
            return this.model;
        },
        fetchData: function (success, error) {

            this.model = new BaseModel();

            var successFn = function (model, response, options) {
                if ($.isEmptyObject(response)) {
                    error();
                    return;
                }
                if (!$.isEmptyObject(response["status"]) && response["status"] == "error") {
                    error();
                    return;
                }
                success();
            };

            var errorFn = function () {
                error();
            };
            this.model.getModel(CurrentUserInfo.Id, successFn, errorFn);

        },
        onBeforeRender: function () {
        },
        onDomRefresh: function () {


            this.model.setFormValidation(true);

            this.setField('#Role');
            this.setField('#State');

            $("#Phone").inputmask({ "mask": "(999) 999-9999" });
        },
        onRender: function () {
            this.onRenderBase();
        },
        resetField: function (elm) {
            //Fix for validation
            var parent = elm.parent().parent();
            $(parent).removeClass("has-error");
            $(".help-block", parent).hide();
        },
        setField: function (id) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false
            });
            $(id).on('show.bs.select', function (e) {
                _this.resetField($(id));
            });
        },
        OnSetPassword: function (event) {
            var model = this.model;

            ChangePasswordModalView.prototype.appendContainer();
            this.modalChangePassword = new ChangePasswordModalView({ id: model.get("AspNetUserId"), parentView: this });
            this.modalChangePassword.render();
            this.modalChangePassword.showModal();
        },
        onSave: function () {
            var _this = this;
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);
                this.model.parseObj(obj);
                var success = function (responce) {
                    Utils.notify("The profile data has been successfully updatyed.");
                };
                this.model.updateProfileData(success);
            }
        }

    });

});