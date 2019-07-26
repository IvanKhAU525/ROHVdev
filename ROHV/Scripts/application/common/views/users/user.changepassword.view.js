define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/user.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="user-password-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#user-password-view",
        events: function () {
            return {
                "click #save-modal": "onSave"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
            this.id = options.id;
        },
        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },
        template: function (serialized_model) {
            var templateHtml = $("#user-password-template").html();

            return templateHtml;
        },
        serializeData: function () {
            return null;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {
            this.model = new BaseModel();
            this.model.setFormChangePasswordValidation();
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
        showModal: function () {
            var _this = this;
            this.$el.modal();

            this.$el.off('hidden.bs.modal');
            this.$el.on('hidden.bs.modal',
                    function () {
                        _this.destroy();
                    });
            this.$el.modal('show');

        },
        closeModal: function () {
            this.$el.modal('hide');
        },
        onSave: function () {
            var _this = this;
            if (this.model.validChangePassword()) {
                var obj = Utils.serializeForm(this.model.formId);
                obj.AspNetUserId = this.id;
                var success = function (responce) {
                    Utils.notify("The password has been successfully changed.");
                    _this.closeModal();
                };
                this.model.sendChangePassword(obj, success);
            }
        }
    });

});