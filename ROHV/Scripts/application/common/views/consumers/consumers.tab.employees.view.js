define(function (require) {

    require('marionette');
    require('underscore');


    var BaseModel = require('models/consumer.employee.model'),
        ModalView = require('views/consumers/popups/consumer.employee.view'),
        DeleteDialog = require('views/confirm.view'),
        SendModalView = require('views/consumers/popups/consumer.employee.email.view'),
        ModelEmployeeInfo = require('models/employee.model'),
        ModalEmployeeInfoView = require('views/consumers/popups/consumer.employee.info.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="employees-view"></div>',
        el: "#employees-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",
                "click [data-action=send-email]": "onSendEmail",
                "click [data-action=details]": "onWorkerInfo",
                "click [data-action=download-document]": "onDownloadDocument",
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#employees-tab-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {
            if ($.isEmpty(this.model.get("Employees"))) {
                this.model.set("Employees", []);
            }
            var values = this.model.get("Employees");
            _.each(values, function (model, key, list) {
                model["StartDate"] = Utils.getDateText(model["StartDate"]);
                model["EndDate"] = Utils.getDateText(model["EndDate"]);
            });
            this.model.set("Employees", values);
        },
        appendContainer: function () {
            $("#employees-tab").append(this.container);
        },

        onBeforeRender: function () {
        },
        onDomRefresh: function () {
            this.setTable();
        },
        onRender: function () {
            this.onRenderBase();
        },
        getDataGrid: function () {

            var dataModels = this.model.get("Employees");
            var dataGrid = {
                columns: [
                    { title: "Employee" },
                    { title: "Hab service worked on" },
                    { title: "Start date" },
                    { title: "End date" },
                    { title: "Rate" },
                    { title: "Max Hours Per Week" },
                    { title: "Max Hours Per Year" },
                    { title: "Rate Note" },
                    { title: "Uploaded file"},
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
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
            this.dataTable = $('#employees-grid').DataTable({
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

            var tml_btn = null;
            if ($("#button-template-employee").length != 0) {
                var html_btn = $("#button-template-employee").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];

            data.push(model["ContactName"]);
            data.push(model["ServiceName"]);
            data.push(model["StartDate"]);
            data.push(model["EndDate"]);
            data.push(model["Rate"]);
            data.push(model["MaxHoursPerWeek"]);
            data.push(model["MaxHoursPerYear"]);
            data.push(Utils.shrinkText(model["RateNote"], 15));
            data.push(model["FileName"]);
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerEmployeeId"], employee_id: model["ContactId"], fileId: model["FileId"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        addRowToGrid: function (model) {
            var rowData = this.getDataForRow(model);
            this.dataTable.row.add(rowData).draw(false);
            this.dataTable.columns.adjust().draw();
        },
        removeRowGrid: function (id) {
            var _this = this;
            _.each(this.dataTable.rows().ids(), function (item, key) {
                var dataArray = _this.dataTable.row(key).data();
                var action = _.last(dataArray);
                var idRow = $(action).attr("data-row-id");
                if (idRow == id) {
                    _this.dataTable.row(key).remove().draw(false);
                    return false;
                }
            });
        },
        updateRow: function (model) {
            var rowData = this.getDataForRow(model);
            this.dataTable.row(this.currentIdxIndex).data(rowData).draw(false);
        },
        setModel: function (model) {
            this.model = model;
            this.render();
        },
        onAdd: function () {
            if ($.isEmpty(this.model.get("ConsumerId"))) {
                GlobalEvents.trigger("showMessage", { title: "Can't perform the action...", message: "Save consumer info first", type_class: "alert-danger" });
                return;
            }
            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: null, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onEdit: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var phones = this.model.get("Employees");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(phones, { ConsumerEmployeeId: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            var values = this.model.get("Employees");
            if (!model.isUpdate) {
                if ($.isEmpty(model.ConsumerEmployeeId)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.ConsumerEmployeeId = count + ".new";
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The employee has been successfully added.");
            }
            else {
                _.extend(_.findWhere(values, { ConsumerEmployeeId: model.ConsumerEmployeeId }), model);
                this.updateRow(model);
                Utils.notify("The employee has been successfully updated.");
            }
            this.model.set("Employees", values);
            this.sendChangeEvent();
        },
        onDelete: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });
        },
        onConfirm: function (id) {

            var _this = this;
            var success = function () {
                var values = _this.model.get("Employees");
                values = _.without(values, _.findWhere(values, { ConsumerEmployeeId: id }));
                _this.model.set("Employees", values);
                _this.removeRowGrid(id);
                _this.sendChangeEvent();
            }
            var baseModel = new BaseModel();
            baseModel.deleteData(id, success);
        },
        sendChangeEvent: function () {
            this.triggerMethod('change:employee-list');
        },

        onSendEmail: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);
            var values = this.model.get("Employees");
            var employeeModel = _.findWhere(values, { ConsumerEmployeeId: id });
            if ($.isEmpty(employeeModel.Email)) {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "The worker doesn't have the email. Please set the email for this employee and try again.", type_class: "alert-danger" });
                return;
            }
            SendModalView.prototype.appendContainer();
            this.modalSend = new SendModalView({ model: employeeModel, parentView: this });
            this.modalSend.render();
            this.modalSend.showModal();
        },
        onWorkerInfo: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            var _this = this;
            var model = new ModelEmployeeInfo();
            model.getModel(id, function (_model, response) {
                var modelSet = new ModelEmployeeInfo();
                modelSet.set(response);
                ModalEmployeeInfoView.prototype.appendContainer();
                _this.modal = new ModalEmployeeInfoView({ model: modelSet, parentView: _this });
                _this.modal.render();
                _this.modal.showModal();
            });
        },
        onDownloadDocument: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) {
                id = parseInt(id);
                var win = window.open("/api/filedataapi/getfilehandler?id=" + id, "_blank");
                //  location.href = "/api/consumerdocumentsapi/getdocumenthandler?id=" + id;
            } else {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "The file hasn't uploaded yet.", type_class: "alert-danger" });
                return;
            }
        },
        updateTable: function () {
            this.dataTable.columns.adjust().draw();
        }

    });

});