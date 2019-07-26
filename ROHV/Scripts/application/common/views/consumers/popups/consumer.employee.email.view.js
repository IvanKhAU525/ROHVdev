define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.employee.email.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="employee-email-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#employee-email-view",        
        events: function () {
            return {
                "click #save-modal": "onSend"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        template: function (serialized_model) {
            var templateHtml = $("#pop-template-employee-email").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
            }
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {
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
        onSend: function () {

            var _this = this;
            this.model.showOverlay();
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);              
                this.model.parseObj(obj);
                var callback = function()
                {
                    _this.model.hideOverlay();
                    _this.closeModal();
                }
                this.model.send(callback, callback);                
            }else
            {
                _this.model.hideOverlay();
            }
        }
    });

});