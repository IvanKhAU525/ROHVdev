define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.printdocument.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="print-document-emailed-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#print-document-emailed-view",
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
            var templateHtml = $("#pop-template-printdocument-emailed").html();
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
            var advocateInfo = this.model.get("AdvocatePaperInfo");
            if (advocateInfo != null)
            {
                $("#EmailAdditional").val(advocateInfo["EmailAddress"]);
            }
            this.setField("#PrintDocumnentTypeId");
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
                var obj = Utils.serializeForm("#print-document-emailed-form");
                var types = []
                $("[name=DocumentTypes]").each(function () {

                    if ($(this).is(":checked")) {
                        types.push($(this).val());
                    }

                });
                var id = this.model.get("ConsumerPrintDocumentId");
                var contactName = this.model.get("ContactName");
                var objToSend = { documentId: id, documentTypes: types, email: obj.EmailWorker, emailOther: obj.EmailAdditional, emailBody: obj.Message,contactName: contactName };
                var base = new BaseModel();
                $(".overlay").show();
                var success = function (response) {
                    $(".overlay").hide();
                    if (response.status == "ok") {
                        Utils.notify("The print documents has been successfully sent.");
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
                base.request("/api/consumerdocumnetprintapi/sendemail/", objToSend, success, error);
            }
        }
    });/*.end module*/

});/*.end defined*/