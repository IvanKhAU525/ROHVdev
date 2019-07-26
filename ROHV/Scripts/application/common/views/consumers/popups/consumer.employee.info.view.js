define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/employee.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="employee-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#employee-view",
        employeesData: [],
        events: function () {
            return {
                "click #print-label-employee": "onPrintLabel"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        template: function (serializedModel) {
            var templateHtml = $("#pop-template-employee-info").html();
            var template = _.template(templateHtml);
            return template({ model: serializedModel });
        },
        serializeData: function () {
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {
            this.onRenderBase();            
        },
        setServiceField: function () {
            var _this = this;
            $('#ServiceId').selectpicker({
                dropupAuto: false
            });
            $('#ServiceId').on('show.bs.select', function (e) {
                _this.model.resetField($('#ServiceId'));
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
        onPrintLabel: function () {
            var _this = this;

            var base = new BaseModel();
            $(".overlay").show();
            var success = function (response) {
                $(".overlay").hide();
                if (response.status == "ok") {
                    var win = window.open(response.url, "_blank");
                } else {
                    GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: response.message, type_class: "alert-danger" });
                }
            };
            var error = function () {
                $(".overlay").hide();
                GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: "Unxpected error...", type_class: "alert-danger" });
            };
            var id = this.model.get("ContactId");
            var objToSend = { contactId: id };
            base.request("/api/employeesapi/getpdfemployeelabel/", objToSend, success, error);

        }
    });

});