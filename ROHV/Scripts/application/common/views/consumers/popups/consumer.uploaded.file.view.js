define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/uploaded.file.model'),
        BaseView = require('views/file.upload.base.view');

    return BaseView.extend({

        container: '<div id="uploaded-file-view-popup"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#uploaded-file-view-popup",
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
            var templateHtml = $("#pop-template-uploaded-file").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            if (!tmp || !tmp.attributes) {
                this.model = new BaseModel();
                if (tmp != null) {
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
            this.setFileEvent();
            this.setFileName();
            this.onRenderBase();
            this.setupDropzone('#dropzone');
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
        isProcessing: false,
        setFileName: function () {
            if (this.model) {
                var fileName = this.model.get("FileDisplayName");
                if (fileName) {
                    $("#file-name").html(fileName);
                }
            }
        },       
        onSave: function () {
            var _this = this;
            if (!_this.isProcessing && _this.model.isValid()) {
                _this.isProcessing = true;
                var obj = Utils.serializeForm(_this.model.formId);

                var consumerId = _this.parentView.model.getConsumerId();
                if (consumerId) {

                    this.readFileDataPromise().then(function (savedData) {
                        if (savedData) {
                            obj["FileData"] = savedData;
                        }                      
                        obj["ParentEntityId"] = consumerId;
                        _this.model.parseObj(obj);
                        _this.model.postData(function (response) {
                            if (response.status == "ok") {
                                _this.parentView.onPopupSave(response.model);
                            } else {
                                Utils.notify(response.errorMessage, "error");
                            }
                            _this.isProcessing = false;
                        }, function (error) { _this.isProcessing = false; });
                    });



                }
            }
        },
    });

});