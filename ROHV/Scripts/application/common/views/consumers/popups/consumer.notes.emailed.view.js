define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.note.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="note-emailed-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#note-emailed-view",
        employeesData: [],
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
            var templateHtml = $("#pop-template-note-emailed").html();
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
           
            this.setField("#ConsumerNoteId");
            this.model.setDownloadFormValidation();
            this.onRenderBase();
        },
        setField: function (id) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false
            });
            $(id).on('show.bs.select', function (e) {
                _this.model.resetField($(id));
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
            if (this.model.validateEmailed()) {
                var obj = Utils.serializeForm("#note-emailed-form");
                
                var id = this.model.get("ConsumerNoteId");
                var contactName = this.model.get("ContactName");
                var objToSend = { noteId: id, email: obj.EmailWorker, emailBody: obj.Message,contactName: contactName };
                var base = new BaseModel();
                $(".overlay").show();
                var success = function (response) {
                    $(".overlay").hide();
                    if (response.status == "ok") {
                        Utils.notify("The Note has been successfully sent.");
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: response.message, type_class: "alert-danger" });
                    }
                    _this.closeModal();
                };
                var error = function () {
                    $(".overlay").hide();
                    GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: "Unxpected error...", type_class: "alert-danger" });
                    _this.closeModal();
                };
                base.request("/api/consumernotesapi/sendemail/", objToSend, success, error);
            }
        }
    });/*.end module*/

});/*.end defined*/