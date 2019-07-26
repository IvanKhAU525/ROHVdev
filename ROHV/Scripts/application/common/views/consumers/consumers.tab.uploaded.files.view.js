define(function (require) {

    require('marionette');
    require('underscore');

    var ModalView = require('views/consumers/popups/consumer.uploaded.file.view'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/base.view'),
        BaseModel = require('models/uploaded.file.model');
    

    return BaseView.extend({

        container: '<div id="uploaded-file-view"></div>',
        el: "#uploaded-file-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=delete]": "onDelete",
                "click [data-action=update]": "onEdit",
                "click [data-action=download]": "onDownload",
            }
        },
        initialize: function (options) {

        },
        template: function (serialized_model) {
            var templateHtml = $("#uploaded-file-tab-template").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        adjustModel: function (model) {
            model["CreatedOn"] = Utils.getDateText(model["CreatedOn"]);
            model["UpdatedOn"] = Utils.getDateText(model["UpdatedOn"]);      
        },
        serializeData: function () {
            if ($.isEmpty(this.model.get("UploadFiles"))) {
                this.model.set("UploadFiles", []);
            }
            var _this = this;
            var values = this.model.get("UploadFiles");
            _.each(values, function (model) {
                _this.adjustModel(model);
            });
            this.model.set("UploadFiles", values);

            return this.model;
        },
        appendContainer: function () {
            $("#uploaded-file-tab").append(this.container);
        },

        onBeforeRender: function () {
        },

        onDomRefresh: function () {        
        },

        onRender: function () {
            this.setTable();
            this.onRenderBase();
        },
        setTable: function () {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#uploaded-file-grid').DataTable({
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
        getDataGrid: function () {

            var dataModels = this.model.get("UploadFiles");
            var dataGrid = {
                columns: [
                    { title: "File Display Name"},
                    { title: "Created On" },
                    { title: "Added by" },
                    { title: "Updated On" },                    
                    { title: "Updated by" },
                    { title: "Note"},
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
        getDataForRow: function (model) {           
            var tml_btn = null;          

            if ($("#button-template-uploaded-file-grid").length != 0) {
                var html_btn = $("#button-template-uploaded-file-grid").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["FileDisplayName"]);
            data.push(model["CreatedOn"]);
            data.push(model["AddedByView"]);
            data.push(model["UpdatedOn"]);
            data.push(model["UpdatedByView"]); 
            data.push(model["Note"]); 
            if (tml_btn != null) {
                var objToPass = { id: model["Id"], downloadId: model["DownloadId"]};
                data.push(tml_btn(objToPass));
            }
            return data;
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
            var uploadFiles = this.model.get("UploadFiles");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(uploadFiles, { Id: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },        
        onPopupSave: function (model) {
            this.adjustModel(model);
            this.modal.closeModal();
            var values = this.model.get("UploadFiles");
            var oldModel = _.findWhere(values, { Id: model.Id });            

            if (!oldModel) {               
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The print document has been successfully added.");

            } else {    
                _.extend(oldModel, model);
                this.updateRow(model);
                Utils.notify("The print document has been successfully updated.");
            }
            this.model.set("UploadFiles", values);
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
                var values = _this.model.get("UploadFiles");
                values = _.without(values, _.findWhere(values, { Id: id }));
                _this.model.set("UploadFiles", values);
                _this.removeRowGrid(id);
            }
            var baseModel = new BaseModel();
            baseModel.deleteData(id, success);
        },
        updateTable: function () {
            this.dataTable.columns.adjust().draw();
        },
        onDownload: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) {
                id = parseInt(id);
                var win = window.open("/api/filedataapi/getfilehandler?id=" + id, "_blank");
            } else {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "The file hasn't uploaded yet.", type_class: "alert-danger" });
                return;
            }
        }
    });

});