/// <reference path="consumers.tab.notes.view.js" />
define(function (require) {

    require('marionette');
    require('underscore');


    var BaseModel = require('models/consumer.document.model'),
        ModalView = require('views/consumers/popups/consumer.document.view'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="documents-view"></div>',
        el: "#documents-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",
                "click [data-action=download]": "onDownload"
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#documents-tab-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {
            if ($.isEmpty(this.model.get("Documents"))) {
                this.model.set("Documents", []);
            }
            var values = this.model.get("Documents");
            _.each(values, function (model, key, list) {
                model["DateUpdated"] = Utils.getDateText(model["DateUpdated"]);
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
                model["ViewDateDocument"] = Utils.getFormatedDateText(model["DateDocument"],"mm/yyyy");
                
            });
            this.model.set("Documents", values);

            return this.model;
        },
        appendContainer: function () {
            $("#documents-tab").append(this.container);
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

            var dataModels = this.model.get("Documents");
            var dataGrid = {
                columns: [
                    { title: "Document type", className: "documentTypeClass"},
                    { visible: false, name: "documentTypeColor"},
                    { title: "Status" },
                    { title: "# notes" },
                    { title: "From" },
                    { title: "Added by" },
                    { title: "Date created" },                    
                    { title: "Updated by" },
                    { title: "Date updated" },
                    { title: "Date Of Document" },
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
            var docTypeColorIndex = this.data.columns.map(function (e) { return e.name; }).indexOf('documentTypeColor');
            this.dataTable = $('#documents-grid').DataTable({
                responsive: false,
                searching: true,
                paging: false,
                deferRender: true,
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data.dataSet,
                columns: _this.data.columns,
                orderCellsTop: true,
                fixedHeader: true,
                language: {
                    emptyTable: "No records"
                },
                rowCallback: function (row, data) {
                    $("td.documentTypeClass", $(row)).css("background-color", data[docTypeColorIndex]+"5A");
                }
            });

            this.initDocumentGridFilters('#documents-grid');

        },
        getActualCoulumnIndex: function (columnsVisibility, visibleIndex) {
            var inspectedColumns = columnsVisibility.slice(0, visibleIndex + 1);
            var invisibleColumns = inspectedColumns.filter(function (x) { return !x }).length;
            return visibleIndex + invisibleColumns;

        },
        initDocumentGridFilters: function (selector) {
            var _this = this;
            var table = _this.dataTable;
            var columnsVisibility = table.columns().visible().toArray();
            $(selector + ' thead tr').clone(true).appendTo(selector + ' thead');
            var allCells = $(selector + ' thead tr:eq(1) th');
            var lastIndex = allCells.length - 1;
            allCells.each(function (i) {

                var title = $(this).text();
                $(this).removeAttr('class');
                
                var content = lastIndex > i ? '<input type="text" style="width:80%" placeholder="' + title + '" />' : '';
                $(this).html(content);

                $('input', this).on('keyup change', function () {
                    var visibleIndex = $(this).parent().index();
                    var actualColumn = _this.getActualCoulumnIndex(columnsVisibility, visibleIndex);
                    if (table.column(actualColumn).search() !== this.value) {
                        table.column(actualColumn).search(this.value).draw();
                    }
                });

            });
        },
        getDataForRow: function (model) {

            var tml_btn = null;
            if ($("#button-template-documents").length != 0) {
                var html_btn = $("#button-template-documents").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["DocumentTypeName"]);
            data.push(model["DocumentTypeColor"]);
            data.push(model["DocumentStatusName"]);
            data.push(model["NumberNotes"]);
            data.push(model["EmployeeName"]);
            data.push(model["AddedByName"]);
            data.push(model["DateCreated"]);
            data.push(model["UpdatedByName"]);
            data.push(model["DateUpdated"]);
            data.push(model["ViewDateDocument"]);
            if (tml_btn != null) {
                var objToPass = { id: model["EmployeeDocumentId"] };
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
            var documents = this.model.get("Documents");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(documents, { EmployeeDocumentId: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onPopupSave: function (model) {
            model["ViewDateDocument"] = Utils.getFormatedDateText(model["DateDocument"], "mm/yyyy");
            this.modal.closeModal();
            var values = this.model.get("Documents");
            if (!model.isUpdate) {
                if ($.isEmpty(model.EmployeeDocumentId)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.EmployeeDocumentId = count + ".new";
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The print document has been successfully added.");
            }
            else {
                _.extend(_.findWhere(values, { EmployeeDocumentId: model.EmployeeDocumentId }), model);
                this.updateRow(model);
                Utils.notify("The approved service has been successfully updated.");
            }
            this.model.set("Documents", values);
        },
        onDelete: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });
        },
        onDownload: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) {
                id = parseInt(id);
                var documents = this.model.get("Documents");
                var model = _.find(documents, { EmployeeDocumentId: id });
                if (!$.isEmpty(model.DocumentPath)) {
                    var win = window.open("/api/consumerdocumentsapi/getdocumenthandler?id=" + id, "_blank");
                    //  location.href = "/api/consumerdocumentsapi/getdocumenthandler?id=" + id;
                } else {
                    GlobalEvents.trigger("showMessage", { title: "Error", message: "The file hasn't uploaded yet.", type_class: "alert-danger" });
                }
            } else {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "You can't download not saving document. Please save consumer info and try again.", type_class: "alert-danger" });
                return;
            }

        },
        onConfirm: function (id) {
            var _this = this;
            var success = function () {
                var values = _this.model.get("Documents");
                values = _.without(values, _.findWhere(values, { EmployeeDocumentId: id }));
                _this.model.set("Documents", values);
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
            this.dataTable.columns.adjust().draw();
        }
    });

});