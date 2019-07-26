define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.phone.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="phone-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#phone-view",
        events: function () {
            return {
                "click #save-modal": "onSave"
            }
        },
        initialize: function (options) {            
            this.parentView = options.parentView;
        },
        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },
        template: function (serialized_model) {
            var templateHtml = $("#pop-template-phone").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
                var phone = this.model.get("Phone");
                phone = Utils.formatPhone(phone);
                this.model.set("PhoneFormated", phone);
            }
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {
            $('#PhoneTypeId').selectpicker({
                dropupAuto: false,
            });
            $("#PhoneFormated").inputmask({ "mask": "(999) 999-9999" });
            this.model.setFormValidation();
            this.onRenderBase();
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
            if (_this.model.isValid()) {
                var obj = Utils.serializeForm(_this.model.formId);
                obj.PhoneTypeName = "";
                if ($("#PhoneTypeId").val().length > 0) {
                    obj.PhoneTypeName = $.trim($("option:selected", "#PhoneTypeId").html());
                }
                _this.model.parseObj(obj);
                obj = _this.model.getObject();

                var consumerId = _this.parentView.model.getConsumerId();
                if (consumerId) {
                    _this.model.postData(consumerId, function (model, response) {
                        obj.ConsumerPhoneId = response.consumerPhoneId;
                        _this.parentView.onPopupSave(obj);
                    });
                }
                else {
                    _this.parentView.onPopupSave(obj);
                }
            }                                    
        },
    });

});