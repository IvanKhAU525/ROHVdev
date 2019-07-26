﻿define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/user.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="user-form-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#user-form-view",
        events: function () {
            return {
                "click #save-modal": "onSave"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
            this.fullModel = this.parentView.model;
        },
        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },
        template: function (serialized_model) {
            var templateHtml = $("#user-form-template").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {

            if (this.model == null || this.model == undefined) {
                this.model = new BaseModel();
            }
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {
            var isEdit = (this.model.get("UserId") != null);
            this.model.setFormValidation(isEdit);

            this.setField('#Role');
            this.setField('#State');

            $('#Role').selectpicker({
                dropupAuto: false
            });

            $("#Phone").inputmask({ "mask": "(999) 999-9999" });
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
            if (this.model.isValid()) {
                var isEdit = this.model.get("UserId") != null;
                if (isEdit) {
                    isEdit = this.model.get("UserId") != -1;
                }
                var obj = Utils.serializeForm(this.model.formId);
                obj.IsUpdate = isEdit;
                if (!isEdit) {
                    obj.UserId = -1;
                }
                this.model.parseObj(obj);

                var success = function (responce) {
                    _this.model.set("UserId", responce.id);
                    _this.model.set("AspNetUserId", responce.aspnetid);
                    _this.parentView.onPopupSave(_this.model);
                };
                var error = function () {                    
                    _this.closeModal();
                };
                this.model.postData(success, error);
            }
        }
    });

});