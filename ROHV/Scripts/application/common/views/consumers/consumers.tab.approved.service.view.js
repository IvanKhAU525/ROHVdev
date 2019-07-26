define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.approved.service.model'),
        ModalView = require('views/consumers/popups/consumer.approved.service.view'),
        ReadonlyModalView = require('views/consumers/popups/consumer.readonly.approved.service.view'),
        ModalEmailedView = require('views/consumers/popups/consumer.approved.service.emailed.view'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="approved-service-view"></div>',
        el: "#approved-service-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=view]": "onView",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",
                "click [data-action=send-email]": "onEmailed",
                "click [data-action=download-document]": "onDownloadDocument"
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#approved-services-tab-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {

            if ($.isEmpty(this.model.get("ApprovedServices") ))
            {                
                this.model.set("ApprovedServices",[]);
            }
            var values = this.model.get("ApprovedServices");
            _.each(values, function (model, key, list) {
                model["EffectiveDate"] = Utils.getDateText(model["EffectiveDate"]);
                model["DateInactive"] = Utils.getDateText(model["DateInactive"]);
                model["AgencyDate"] = Utils.getDateText(model["AgencyDate"]);

                model["UsedHoursStartDate"] = Utils.getDateText(model["UsedHoursStartDate"]);
                model["UsedHoursEndDate"] = Utils.getDateText(model["UsedHoursEndDate"]);
                
            });
            this.model.set("ApprovedServices", values);

            return this.model;
        },
        appendContainer: function () {
            $("#approved-service-tab").append(this.container);
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

            var dataModels = this.model.get("ApprovedServices");
            var dataGrid = {
                columns: [
                    { title: "Service" },
                    { title: "Effective date" },
                    { title: "Added By" },
                    { title: "Edited By" },
                    { title: "Total Hours" },                  
                    { title: "Date Inactive" },
                    { title: "Inactive" }, 
                    { title: "Notes" }, 
                    { title: "Direct care workers" },
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
            this.dataTable = $('#aproved-services-grid').DataTable({
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
            if ($("#button-template-approved-service").length != 0) {
                var html_btn = $("#button-template-approved-service").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];

            data.push(model["ServiceName"]);
            data.push(model["EffectiveDate"]);
            data.push(model["AddedByView"]);
            data.push(model["EditedByView"]);
            data.push(model["TotalHours"]);         
            data.push(model["DateInactive"]);
            data.push(model["Inactive"]);
            data.push(Utils.shrinkText(model["Notes"],15));
            data.push(model["DWorkers"]);
            data.push(model["FileName"]);
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerServiceId"], fileId: model["FileId"] };
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
            if($.isEmpty(this.model.get("ConsumerId")))
            {
                GlobalEvents.trigger("showMessage", { title: "Can't perform the action...", message: "Save consumer info first", type_class: "alert-danger" });
                return;
            }
            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: { ConsumerId: this.model.get("ConsumerId")}, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onView: function (event) {
            var _this = this;

            let id = $(event.currentTarget).attr("data-id");
            let row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            let approvedServices = this.model.get("ApprovedServices");
            if ($.isNumeric(id)) id = parseInt(id);
            let model = _.find(approvedServices, { ConsumerServiceId: id });

            var showDialog = function () {
                ReadonlyModalView.prototype.appendContainer();
                this.viewService = new ReadonlyModalView({ model: model, parentView: _this });
                this.viewService.render();
                this.viewService.showModal();
            }
            showDialog();
        },
        onEdit: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var phones = this.model.get("ApprovedServices");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(phones, { ConsumerServiceId: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            var values = this.model.get("ApprovedServices");
            if (!model.isUpdate) 
            {
                if ($.isEmpty(model.ConsumerServiceId)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.ConsumerServiceId = count + ".new";                   
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The print document has been successfully added.");
            }
            else {
                _.extend(_.findWhere(values, { ConsumerServiceId: model.ConsumerServiceId }), model);
                this.updateRow(model);
                Utils.notify("The approved service has been successfully updated.");
            }
            this.model.set("ApprovedServices", values);
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
                var values = _this.model.get("ApprovedServices");
                values = _.without(values, _.findWhere(values, { ConsumerServiceId: id }));
                _this.model.set("ApprovedServices", values);
                _this.removeRowGrid(id);
            }
            var baseModel = new BaseModel();
            baseModel.deleteData(id, success);
        },
        onEmailed: function (event) {

            var _this = this;
            var id = $(event.currentTarget).attr("data-id");

            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var values = this.model.get("ApprovedServices");

            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(values, { ConsumerServiceId: id });

            var showDialog = function () {
                ModalEmailedView.prototype.appendContainer();
                this.modalEmailed = new ModalEmailedView({ model: model, parentView: _this });
                this.modalEmailed.render();
                this.modalEmailed.showModal();
            }
            showDialog();

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
        refillDirectWorkers: function () {
            var employees = this.model.get("Employees");
            var approvedServices = this.model.get("ApprovedServices");
            _.each(approvedServices, function (item) {
                var list = _.filter(employees, function (employeesItem) {
                    return parseInt(employeesItem.ServiceId) == parseInt(item.ServiceId);
                });
                var dWorkers = "";
                if (!$.isEmptyObject(list)) {
                    var listWokers = [];
                    _.each(list, function (worker) {
                        listWokers.push(worker.ContactName);
                    }
                    );
                    dWorkers = listWokers.join("; ");
                }
                item.DWorkers = dWorkers;
            });
            this.model.set("ApprovedServices", approvedServices);
            this.reloadTable();
        },
        onDownloadDocument: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) {
                id = parseInt(id);
                var win = window.open("/api/filedataapi/getfilehandler?id=" + id, "_blank");
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