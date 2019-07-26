define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.approved.service.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="readonly-approved-service-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#readonly-approved-service-view",
        initialize: function (options) {
            this.parentView = options.parentView;
            this.fullModel = this.parentView.model;
        },
        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },
        template: function (serialized_model) {
            var templateHtml = $("#readonly-pop-template-approved-service").html();
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
            this.setTable();
          
            this.onRenderBase();
        },
        getDataGrid: function () {
            var dataModels = [];
            var serviceId = $('#ServiceId').val();
            if (!$.isEmpty(serviceId)) {
                var employees = this.fullModel.get("Employees");
                dataModels = _.filter(employees, function (item) {
                    return parseInt(item.ServiceId) == parseInt(serviceId)
                });
            }
            var dataGrid = {
                columns: [
                    { title: "Employee" },
                    { title: "Start date" },
                    { title: "End date" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels, function (model, key, list) {
                var data = _this.getDataForRow(model);
                dataGrid.dataSet.push(data);
            });
            return dataGrid;
        },
        setTable: function () {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#direct-workers-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data.dataSet,
                columns: _this.data.columns,
                language: {
                    emptyTable: "No records"
                }
            });
        },
        getDataForRow: function (model) {
            var data = [];
            data.push(model["ContactName"]);
            data.push(model["StartDate"]);
            data.push(model["EndDate"]);
            return data;
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
        getDirectWorkers: function (serviceId) {
            var employees = this.fullModel.get("Employees");
            var list = _.filter(employees, function (item) {
                return parseInt(item.ServiceId) == parseInt(serviceId)
            });

            if (!$.isEmptyObject(list)) {
                var listWokers = [];
                _.each(list, function (item) {
                    listWokers.push(item.ContactName);
                }
                );
                return listWokers.join("; ");
            }
            return "";
        }
    });
});