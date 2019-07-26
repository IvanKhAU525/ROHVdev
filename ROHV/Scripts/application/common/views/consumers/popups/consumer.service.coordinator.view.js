define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.service.coordinator.model'),
        BaseView = require('views/base.contact.search.view');

    return BaseView.extend({

        container: '<div id="service-coordinator-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#service-coordinator-view",
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
            var templateHtml = $("#pop-template-service-coordinator").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            if (!tmp || !tmp.attributes) {
                this.model = new BaseModel();
                if (tmp != null) {
                    tmp["FromDate"] = Utils.getDateText(tmp["FromDate"]);
                    tmp["ToDate"] = Utils.getDateText(tmp["ToDate"]);
                    this.model.set(tmp);
                }
            }
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {

            this.model.setFormValidation();

            this.setMedicalService('#ContactId');
            $('#FromDate').datepicker({ autoclose: true });
            $('#FromDate').inputmask();
            $('#ToDate').datepicker({ autoclose: true });
            $('#ToDate').inputmask();
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
        isProcessing: false
        ,
        onSave: function () {
            var _this = this;
            if (!_this.isProcessing && _this.model.isValid()) {
                _this.isProcessing = true;
                var obj = Utils.serializeForm(_this.model.formId);
               
                _this.model.parseObj(obj);
                obj = _this.model.getObject();

                var consumerId = _this.parentView.model.getConsumerId();
                if (consumerId) {
                    _this.model.postData(consumerId, function (model, response) {
                        if (response.status == "ok") {
                            obj.Id = response.model.Id;
                            obj.New = response.New;
                            obj.ViewContactName = response.model.ViewContactName;
                            _this.parentView.onPopupServiceCoordinatorSave(obj);
                        } else {
                            Utils.notify(response.errorMessage, "error");
                        }
                        _this.isProcessing = false;
                    }, function (error) { _this.isProcessing = false; });
                }
                else {
                    _this.isProcessing = false;
                    Utils.notify("Can't add Service Coordinator data range without selected consumer", "error");
                }
            }                                    
        },
    });

});