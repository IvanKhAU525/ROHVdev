/// <reference path="consumers.tab.notes.view.js" />
define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.notification.model'),
        ModalView = require('views/consumers/popups/consumer.notification.view'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="notifications-view"></div>',
        el: "#notifications-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete"
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#notifications-tab-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {

            if ($.isEmpty(this.model.get("Notifications"))) {
                this.model.set("Notifications", []);
            }          
            var values = this.model.get("Notifications");
            _.each(values, function (model, key, list) {
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
                model["DateUpdated"] = Utils.getDateText(model["DateUpdated"]);
                model["DateStart"] = Utils.getDateText(model["DateStart"]);
            });
            this.model.set("Notifications", values);

            return this.model;
        },
        appendContainer: function () {
            $("#notifications-settings-tab").append(this.container);
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

            var dataModels = this.model.get("Notifications");
            var dataGrid = {
                columns: [
                    { title: "Notification name" },
                    { title: "Note" },
                    { title: "Date start" },
                    { title: "Repeating type" },
                    { title: "Recipients" },
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
        setTable: function () {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#notifications-grid').DataTable({
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
            if ($("#button-template-notification").length != 0) {
                var html_btn = $("#button-template-notification").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];

            data.push(model["Name"]);
            data.push(model["Note"]);
            data.push(model["DateStart"]);
            data.push(model["RepetingTypeName"]);
            data.push(model["RecipientsString"]);
            data.push(model["StatusName"]);
            data.push(model["AddedByName"]);
            data.push(model["UpdatedByName"]);
            if (tml_btn != null) {
                var objToPass = { id: model["Id"] };
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
            var notifications = this.model.get("Notifications");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(notifications, { Id: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            var values = this.model.get("Notifications");
            if (!model.isUpdate) {
                if ($.isEmpty(model.Id)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.Id = count + ".new";
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The notification has been successfully added.");
            }
            else {
                _.extend(_.findWhere(values, { Id: model.Id }), model);
                this.updateRow(model);
                Utils.notify("The notification has been successfully updated.");
            }
            this.model.set("Notifications", values);
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
                var values = _this.model.get("Notifications");
                values = _.without(values, _.findWhere(values, { Id: id }));
                _this.model.set("Notifications", values);
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