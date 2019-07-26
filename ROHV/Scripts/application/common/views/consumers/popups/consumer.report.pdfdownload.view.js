define(function (require) {
    require('marionette');
    require('underscore');

    var BaseView = require('views/base.view'),
        BaseModel = require('models/consumer.report.download.model')

    return BaseView.extend({
        container: '<div id="hab-plan-pdf-download"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#hab-plan-pdf-download",

        events: function () {
            return {
                "click [data-action=generate-report]": "onGenerateReport",
            }
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        initialize: function (options) {
            this.parentView = options.parentView;         
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
            }
            return this.model;
        },
        onRender: function () {
            this.setReportTemplatesField();
            this.onRenderBase();
        },
        setReportTemplatesField: function (){
            const id = '#ReportTemplate';

            $(id).selectpicker({
                dropupAuto: false
            });
        },
        template: function (serialized_model) {
            var templateHtml = $("#pop-template-report-pdf-download").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },

        showModal: function() {
            var _this = this;
            this.$el.modal();

            this.$el.off('hidden.bs.modal');
            this.$el.on('hidden.bs.modal',
                function() {
                    _this.destroy();
                });

            this.$el.modal('show');
        },
        closeModal: function() {
            this.$el.modal('hide');
        },

        onGenerateReport: function() {
            const date = Utils.getDateText($('#ReportTemplate').val());

            this.parentView.generatePdf(this.model.get('reportId'), date);

            this.closeModal();
        },
    });
});