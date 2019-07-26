define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.approved.service.model'),
        BaseView = require('views/file.upload.base.view');

    return BaseView.extend({

        container: '<div id="edit-approved-service-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#edit-approved-service-view",
        events: function () {
            return {
                "click #save-modal": "onSave",
                "change #AnnualUnits": "calculateTotalHours",
                "change #TotalHours": "calculateAnnualUnits",
                "change #UsedHoursStartDate": "onChangeHoursDate",
                "change #UsedHoursEndDate": "onChangeHoursDate",
                "change #ServiceId": "onChangeHoursDate",
                "click [data-action=download-file-popup]": "onDownload"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
            this.fullModel = this.parentView.model;
        },
        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },
        template: function (serialized_model) {
            var templateHtml = $("#pop-template-approved-service").html();
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
            this.trackEvents = false;
            this.model.setFormValidation();
     
            $('.rohv-date').datepicker({ autoclose: true });
            $('.rohv-date').inputmask();            

            $('#UnitQuantities').selectpicker({
                dropupAuto: false,
            });

            this.setServiceField();
            this.setTable();
            this.setFileEvent();
            this.setFileName();
            this.onRenderBase();
            
            this.setupDropzone('#dropzone');
            this.trackEvents = true;
        },
        onChangeHoursDate: function () {  
            this.model.set('UsedHoursStartDate', $('#UsedHoursStartDate').val());
            this.model.set('UsedHoursEndDate', $('#UsedHoursEndDate').val());
            this.model.set('ServiceId', $('#ServiceId').val());
            if (!!this.trackEvents && this.model.isValid() && $('#UsedHoursEndDate').val() && $('#UsedHoursEndDate').val()) {
                this.model.getTotalHours(function (data) {
                    if (data && data.status == "ok") {
                        var model = data.model;
                        if (model) {
                            $('#UsedHours').val(model.UsedHours);
                        }
                }
                });
            }
        },
        setServiceField: function () {
            var _this = this;
            $('#ServiceId').selectpicker({
                dropupAuto: false
            });
            $('#ServiceId').on('changed.bs.select', function (e) {
                _this.reloadTable();
            });
            $('#ServiceId').on('show.bs.select', function (e) {
                _this.model.resetField($('#ServiceId'));
            });
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
        calculateAnnualUnits: function () {
            this.model.set('TotalHours', $('#TotalHours').val());
            var totalHours = this.model.get('TotalHours');
            if (totalHours) {
                var result = Math.round(totalHours * 4);
                this.model.set('AnnualUnits', result);
                $("#AnnualUnits").val(result);
            }
        },
        calculateTotalHours: function () {
            this.model.set('AnnualUnits', $('#AnnualUnits').val());

            var unitsInAnnual = this.model.get('AnnualUnits');
            if (unitsInAnnual) {
                var result = unitsInAnnual / 4;
                this.model.set('TotalHours', result);
                $("#TotalHours").val(result);
            }
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

        },
        isEdit: function () {
            var isEdit = this.model.get("ConsumerServiceId") != null;
            return isEdit;
        },
        onSave: function () {
            var _this = this;
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);

                obj.ServiceName = "";
                if ($("#ServiceId").val().length > 0) {
                    obj.ServiceName = $.trim($("option:selected", "#ServiceId").html());
                }
                obj.UnitQuantitiesName = "";
                if ($("#UnitQuantities").val().length > 0) {
                    obj.UnitQuantitiesName = $.trim($("option:selected", "#UnitQuantities").html());
                }
                obj.DWorkers = this.getDirectWorkers(obj.ServiceId);
                obj.ConsumerId = this.parentView.model.get("ConsumerId");
                
                var returnObj = this.model.getObject();
                var isUpdate = this.isEdit();
                var needToSaveFile = this.fileData != null;

                obj.FileName = this.fileData ? this.fileData.name : this.model.get("FileName");

                var saveFileCallBack = function () {
                    _this.model.parseObj(obj);
                    if ($.isEmpty(obj.ConsumerId)) {
                        this.parentView.onPopupSave(returnObj);
                    } else {
                        var success = function (responce) {
                            returnObj["ConsumerServiceId"] = responce.id;
                            returnObj["AddedByView"] = responce.AddedByView;
                            returnObj["EditedByView"] = responce.EditedByView;
                            returnObj["isUpdate"] = isUpdate;
                            returnObj["FileId"] = responce.fileId;
                            returnObj["FileName"] = responce.fileName;
                            _this.parentView.onPopupSave(returnObj);
                        };
                        _this.model.postData(success);
                    }
                }
                if (needToSaveFile) {
                    _this.readFileData(function (result) {                   
                        obj.FileData = result;
                        _this.fileData = null;
                        saveFileCallBack();
                    })
                } else {                   
                    saveFileCallBack();
                }
            }
        },
        onDownload: function () {
            let id = this.model.get("FileId");
            if ($.isNumeric(id)) {
                id = parseInt(id);
                window.open("/api/filedataapi/getfilehandler?id=" + id, "_blank");
            } else {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "The file hasn't uploaded yet.", type_class: "alert-danger" });
                return true;
            }

            return false;
        },
        setFileName: function () {
            if (this.model) {
                var fileName = this.model.get("FileName");
                if (fileName) {
                    $("#file-name").html(fileName);
                }
            }
        }
    });

});