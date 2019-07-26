define(function (require) {

    require('marionette');
    require('underscore');

    var ModalView = require('views/users/user.form.view'),
        DeleteDialog = require('views/confirm.view'),
        ChangePasswordModalView = require('views/users/user.changepassword.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="users-view"></div>',
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",
                "click [data-action=set-password]": "OnSetPassword"
            }
        },
        template: function (serialized_model) {
            var template = Marionette.TemplateCache.get("users");;
            return template;
        },
        serializeData: function () {
            return this.model;
        },
        appendContainer: function () {
            $("#users-container").append(this.container);
        },
        fetchData: function (collection, success, error) {

            collection.fetch({
                success: function (collectionResponse, response, options) {
                    if ($.isEmptyObject(response)) {
                        error();
                        return;
                    }
                    if (!$.isEmptyObject(response["status"]) && response["status"] == "error") {
                        error();
                        return;
                    }
                    success();
                },
                error: function () {
                    error();
                }
            });
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

            var dataModels = this.model.models;
            var dataGrid = {
                columns: [
                    { title: "Name" },
                    { title: "Email" },
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
            this.dataTable = $('#users-grid').DataTable({
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
            if ($("#button-template-users").length != 0) {
                var html_btn = $("#button-template-users").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];

            data.push(model.get("LastName") + ", " + model.get("FirstName"));
            data.push(model.get("Email"));
            if (tml_btn != null) {
                var objToPass = { id: model.get("UserId") };
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
            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: null, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onEdit: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            if ($.isNumeric(id)) id = parseInt(id);
            var model = this.model.get(id);

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            if (!model.get("IsUpdate")) {
                this.model.addModel(model);
                this.addRowToGrid(model);
                Utils.notify("The user has been successfully added.");
            } else {
                this.model.updateModel(model)
                this.updateRow(model);
                Utils.notify("The user has been successfully saved.");
            }
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
                _this.model.deleteById(id);
                _this.removeRowGrid(id);
            };
            this.model.deleteRequestById(id, success);

        },
        OnSetPassword: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = this.model.get(id);

            ChangePasswordModalView.prototype.appendContainer();
            this.modalChangePassword = new ChangePasswordModalView({ id: model.get("AspNetUserId"), parentView: this });
            this.modalChangePassword.render();
            this.modalChangePassword.showModal();
        },
        reloadTable: function () {
            var _this = this;
            this.dataTable.clear().draw();
            var data = this.getDataGrid();
            _.each(data.dataSet, function (item) {
                _this.dataTable.row.add(item).draw(false);
            });
            this.dataTable.columns.adjust().draw();
        }

    });

});