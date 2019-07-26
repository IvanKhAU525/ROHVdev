define(function (require) {

    require('marionette');
    require('underscore');

    var BaseView = require('views/base.view'),
        BaseModel = require('models/audit.model'),
        AuditsCollection = require('collections/consumer.audit.collection'),
        DeleteDialog = require('views/confirm.view');

    return BaseView.extend({ 
        dataTable: {},
        data: {},
        tableTypes: { Audits: "Audits", AuditsDetails: "AuditsDetails"},
        container: '<div id="consumer-audits-view"></div>',
        events: function () {
            return {
                "click [data-action=add-new-audit]": "onAdd",
                "click [data-action=delete]": "onDelete",
                "click [data-action=get-details]": "onGetAuditDetails",
                "click [data-action=get-customer-details]": "onGetCustomerDetails"
                
                
                
            }
        },
        template: function (serialized_model) {
            var template = Marionette.TemplateCache.get("consumeraudit");;
            return template;
        },
        serializeData: function () {           
            return this.model;
        },
        appendContainer: function () {
            $("#consumer-audits-container").append(this.container);
        },
        onBeforeRender: function () {            
        },
        onDomRefresh: function () {
            this.setTable(this.tableTypes.Audits);
            this.setTable(this.tableTypes.AuditsDetails);
            $('#ServiceId').selectpicker({
                dropupAuto: false
            });
        },
        onRender: function () {
            this.onRenderBase();            
        },
        getAuditDataGrid: function () {

            var dataModels = this.model.models;
            var dataGrid = {
                columns: [
                    { title: "Audit Date" },
                    { title: "Service" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels, function (model, key, list) {
                var data = _this.getAuditDataForRow(model);
                dataGrid.dataSet.push(data);
            });
            return dataGrid;
        },
        getAuditDetailsDataGrid: function (additionalData) {
            var dataGrid = {
                columns: [
                    { title: "Consumer" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            if (additionalData) {
                var selectedAuditId = additionalData.Id;
                var dataModel = this.model.models.find(function (el) { return el.get("Id") == selectedAuditId; });

                var _this = this;
                if (dataModel) {
                    _.each(dataModel.get("Consumers"), function (model, key, list) {
                        var data = _this.getAuditDetailsDataForRow(model);
                        dataGrid.dataSet.push(data);
                    });
                }
            }
            return dataGrid;
        },
        getAuditDetailsDataForRow: function (model) {


            var data = [];
            
            data.push(model["ConsumerLastName"] +", " +  model["ConsumerFirstName"]);

            var tml_btn = null;
            if ($("#button-template-audit-details-grids").length != 0) {
                var html_btn = $("#button-template-audit-details-grids").html();
                tml_btn = _.template(html_btn);
            }
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerId"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },      
        setTable: function (tableType,additionalData) {
            var _this = this, data, tableData; 
            switch (tableType) {
                case this.tableTypes.Audits:
                    data = this.getAuditDataGrid();
                    tableData = $('#consumer-audits-grid').DataTable({
                        responsive: false,
                        searching: false,
                        paging: false,
                        deferRender: true,
                        dom: '<"top"lp>rt<"bottom"i><"clear">',
                        data: data.dataSet,
                        columns: data.columns,
                        language: {
                            emptyTable: "No records"
                        }
                    });
                    break;
                case this.tableTypes.AuditsDetails:
                    data = this.getAuditDetailsDataGrid(additionalData);
                    tableData = $('#consumer-audits-details-grid').DataTable({
                        responsive: false,
                        searching: false,
                        paging: false,
                        autoWidth: true,
                        deferRender: true,
                        dom: '<"top"lp>rt<"bottom"i><"clear">',
                        data: data.dataSet,
                        columns: data.columns,
                        language: {
                            emptyTable: "No records"
                        }
                    });
                    break;
            }
            this.data[tableType] = data;
            this.dataTable[tableType] = tableData;

        },
        getAuditDataForRow: function (model) {

           
            var data = [];            
            data.push(Utils.getDateText(model.get("AuditDate")));         
            data.push(model.get("ServiceName"));         

            var tml_btn = null;
            if ($("#button-template-audit-grids").length != 0) {
                var html_btn = $("#button-template-audit-grids").html();
                tml_btn = _.template(html_btn);
            }
            if (tml_btn != null) {
                var objToPass = { id: model.get("Id") };
                data.push(tml_btn(objToPass));
            }
            return data;
        },      
        removeRowGrid: function (id,tableType) {
            var _this = this;
            _.each(this.dataTable[tableType].rows().ids(), function (item, key) {
                var dataArray = _this.dataTable[tableType].row(key).data();
                var action = _.last(dataArray);
                var idRow = $(action).attr("data-row-id");
                if (idRow == id) {
                    _this.dataTable[tableType].row(key).remove().draw(false);
                    return false;
                }
            });
        },      
        setModel: function (model) {
            this.model = model;
            this.render();
        },
        onGetAuditDetails: function (event) {
            var id = $(event.currentTarget).attr("data-id");           
            
            if ($.isNumeric(id)) id = parseInt(id);
            this.reloadTable(this.tableTypes.AuditsDetails, { Id: id });
            $(event.currentTarget).closest("tr").addClass('selected');
        },
        onGetCustomerDetails: function (event) {
            var id = $(event.currentTarget).attr("data-id");           
            var win = window.open("/consumers/" + id, "_blank");
        },
        onAdd: function () {
            var _this = this;
            var model = new BaseModel();
            var obj = Utils.serializeForm(model.formId);
            model.parseObj(obj);            
            model.setFormValidation();
            model.AddAudit(function () {                
                _this.reloadModel();
            })
            return false;
        },
        onDelete: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });
        },
        onConfirm: function (id, type) {
            var _this = this;
            var success = function () {
                var values = _this.model.models;
                values = _.without(values, _.findWhere(values, { Id: id }));
                _this.model.models = values;
                _this.removeRowGrid(id, _this.tableTypes.Audits);
                _this.reloadTable(_this.tableTypes.AuditsDetails);
            }
            var baseModel = new BaseModel();
            baseModel.deleteData(id, success);
        },
        reloadModel: function (callback) {
            var _this = this;
            var audits = new AuditsCollection();
            _this.fetchData(audits, function () {
                _this.model = audits;
                _this.reloadTable(_this.tableTypes.AuditsDetails);
                _this.reloadTable(_this.tableTypes.Audits);
                callback && callback();
            });
        },
        reloadTable: function (tableType, additionalData) {
            var _this = this;
            this.dataTable[tableType].clear().draw();
            var data;
            switch (tableType) {
                case this.tableTypes.Audits:
                    data = this.getAuditDataGrid();                    
                    break;
                case this.tableTypes.AuditsDetails:
                    data = this.getAuditDetailsDataGrid(additionalData);                    
                    break;
            }
            
            _.each(data.dataSet, function (item) {
                _this.dataTable[tableType].row.add(item).draw(false);
            });
            this.dataTable[tableType].columns.adjust().draw();
            this.dataTable[this.tableTypes.Audits].$('tr.selected').removeClass('selected');
        },
        fetchData: function (collection, success, error) {

            collection.fetch({
                success: function (collectionResponse, response, options) {                   
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

    });

});