/// <reference path="consumers.tab.notes.view.js" />
define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.printdocument.model'),
        ModalView = require('views/consumers/popups/consumer.printdocument.view'),
        ModalDownloadView = require('views/consumers/popups/consumer.printdocument.download.view'),
        ModalEmailedView = require('views/consumers/popups/consumer.printdocument.emailed.view'),
        DeleteDialog = require('views/confirm.view'),
        ModelEmployeeInfo = require('models/employee.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="print-documents-view"></div>',
        el: "#print-documents-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=download-empty]": "onDownloadEmpty",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",
                "click [data-action=generate-pdf]": "onGeneratePdf",
                "click [data-action=emailed]": "onEmailed"
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#print-documents-tab-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {

            if ($.isEmpty(this.model.get("PrintDocuments"))) {
                this.model.set("PrintDocuments", []);
            }
            var values = this.model.get("PrintDocuments");
            _.each(values, function (model, key, list) {
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
                model["EffectiveDate"] = Utils.getDateText(model["EffectiveDate"]);
                model["DateUpdated"] = Utils.getDateText(model["DateUpdated"]);
            });
            this.model.set("PrintDocuments", values);
            return this.model;
        },
        appendContainer: function () {
            $("#print-documents-tab").append(this.container);
        },

        onBeforeRender: function () {
        },
        onDomRefresh: function () {

        },
        onRender: function () {
            this.setTable();
            this.onRenderBase();
        },
        getDataGrid: function () {

            var dataModels = this.model.get("PrintDocuments");
            var dataGrid = {
                columns: [
                    { title: "Made for" },
                    { title: "Service type" },
                    { title: "Notes" },
                    { title: "Date added" },
                    { title: "Status" },
                    { title: "Added by" },
                    { title: "Updated by" },
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
        getDataForRow: function (model) {

            var tml_btn = null;

            if ($("#button-template-print-document").length != 0) {
                var html_btn = $("#button-template-print-document").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["ContactName"]);
            data.push(model["ServiceTypeName"]);
            data.push(model["Notes"]);
            data.push(model["DateCreated"]);
            data.push(model["StatusName"]);
            data.push(model["AddedByName"]);
            data.push(model["UpdatedByName"]);
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerPrintDocumentId"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        setTable: function () {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#print-documents-grid').DataTable({
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
            this.dataTable.columns.adjust().draw();
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
            var printDocuments = this.model.get("PrintDocuments");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(printDocuments, { ConsumerPrintDocumentId: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            var values = this.model.get("PrintDocuments");
            if (!model.isUpdate) {
                if ($.isEmpty(model.ConsumerPrintDocumentId)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.ConsumerPrintDocumentId = count + ".new";
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The print document has been successfully added.");

            } else {
                _.extend(_.findWhere(values, { ConsumerPrintDocumentId: model.ConsumerPrintDocumentId }), model);
                this.updateRow(model);
                Utils.notify("The print document has been successfully updated.");
            }
            this.model.set("PrintDocuments", values);
        },
        onDelete: function (event) {

            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });

        },
        onGeneratePdf: function (event) {

            var id = $(event.currentTarget).attr("data-id");
            var isEmpty = $(event.currentTarget).attr("data-empty") == "true" ? true : false;
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var printDocuments = this.model.get("PrintDocuments");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(printDocuments, { ConsumerPrintDocumentId: id });

            ModalDownloadView.prototype.appendContainer();
            this.modalDownload = new ModalDownloadView({ model: model, parentView: this, isEmpty: isEmpty });
            this.modalDownload.render();
            this.modalDownload.showModal();

        },
        onDownloadEmpty: function (event) {
            ModalDownloadView.prototype.appendContainer();
            this.modalDownload = new ModalDownloadView({ model: null, parentView: this, isEmpty: true });
            this.modalDownload.render();
            this.modalDownload.showModal();
        },
        onEmailed: function (event) {

            var _this = this;
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var printDocuments = this.model.get("PrintDocuments");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(printDocuments, { ConsumerPrintDocumentId: id });

            var showDialog = function () {
                ModalEmailedView.prototype.appendContainer();
                this.modalEmailed = new ModalEmailedView({ model: model, parentView: _this });
                this.modalEmailed.render();
                this.modalEmailed.showModal();
            }

            var AdvocatePaperId = this.model.get("AdvocatePaperId");
            if (!$.isEmpty(AdvocatePaperId)) {
                var modelEmployee = new ModelEmployeeInfo();
                modelEmployee.getModel(AdvocatePaperId, function (_model, response) {
                    model["AdvocatePaperInfo"] = response;
                    showDialog();
                });
            } else {
                showDialog();
            }
        },
        onConfirm: function (id) {

            var _this = this;
            var success = function () {
                var values = _this.model.get("PrintDocuments");
                values = _.without(values, _.findWhere(values, { ConsumerPrintDocumentId: id }));
                _this.model.set("PrintDocuments", values);
                _this.removeRowGrid(id);
            }
            var baseModel = new BaseModel();
            baseModel.deleteData(id, success);
        },

        reloadTable: function () {
            var _this = this;
            this.dataTable.clear().draw();
            var data = this.getDataGrid();
            _.each(data.dataSet, function (item) {
                _this.dataTable.row.add(item).draw(false);
            });
            this.dataTable.columns.adjust().draw();
        },
        updateTable: function () {
            if (!$.isEmptyObject(this.dataTable)) {
                this.dataTable.columns.adjust().draw();
            }
        }
    });

});