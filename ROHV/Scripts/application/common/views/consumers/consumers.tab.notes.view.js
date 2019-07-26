define(function (require) {

    require('marionette');
    require('underscore');
    var BaseModel = require('models/consumer.note.model'),
        ModalView = require('views/consumers/popups/consumer.note.view'),
        ModalEmailedView = require('views/consumers/popups/consumer.notes.emailed.view'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="notes-view"></div>',
        el: "#notes-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",
                "click [data-action=emailed]": "onEmailed"
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#notes-tab-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {                     
            if ($.isEmpty(this.model.get("Notes"))) {
                this.model.set("Notes", []);
            }
            var values = this.model.get("Notes");
            _.each(values, function (model, key, list) {
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
                model["Date"] = Utils.getDateText(model["Date"]);
                model["DateUpdated"] = Utils.getDateText(model["DateUpdated"]);
            });
            this.model.set("Notes", values);

            return this.model;
        },
        appendContainer: function () {
            $("#notes-tab").append(this.container);
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

            var dataModels = this.model.get("Notes");
            var dataGrid = {
                columns: [
                    { title: "From" },
                    { title: "Type's Note" },
                    { title: "From Dept." },
                    { title: "Date" },
                    { title: "Notes" },
                    { title: "Added by" }, 
                    { title: "Changed by" }, 
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

            if ($("#button-template-note").length != 0) {
                var html_btn = $("#button-template-note").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["ContactName"]);
            data.push(model["TypeName"]);
            data.push(model["TypeFromName"]);
            data.push(model["Date"]);
            data.push(model["Notes"]);
            data.push(model["AddedByName"]);  
            data.push(model["UpdatedByName"]); 
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerNoteId"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        setTable: function () {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#notes-grid').DataTable({
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
            this.modal = new ModalView({ model: { 'Date': Utils.getDateText(new Date())}, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onEdit: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var phones = this.model.get("Notes");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(phones, { ConsumerNoteId: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            var values = this.model.get("Notes");
            if (!model.isUpdate) {
                if ($.isEmpty(model.ConsumerNoteId)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.ConsumerNoteId = count + ".new";
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The note has been successfully added.");

            } else {
                _.extend(_.findWhere(values, { ConsumerNoteId: model.ConsumerNoteId }), model);
                this.updateRow(model);
                Utils.notify("The note has been successfully updated.");
            }
            this.model.set("Notes", values);
        },
        onDelete: function (event) {

            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });

        },
        onEmailed: function (event) {

            var _this = this;
            var id = $(event.currentTarget).attr("data-id");

            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var values = this.model.get("Notes");

            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(values, { ConsumerNoteId: id });

            var showDialog = function () {
                ModalEmailedView.prototype.appendContainer();
                this.modalEmailed = new ModalEmailedView({ model: model, parentView: _this });
                this.modalEmailed.render();
                this.modalEmailed.showModal();
            }
            showDialog();
           
        },
        onConfirm: function (id) {

            var _this = this;
            var success = function () {
                var values = _this.model.get("Notes");
                values = _.without(values, _.findWhere(values, { ConsumerNoteId: id }));
                _this.model.set("Notes", values);
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