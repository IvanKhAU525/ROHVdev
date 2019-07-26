define(function (require) {

    require('marionette');
    require('underscore');
    var gridTypes =
        {
            Phone: "phone",
            Address: "addresses",
            ServiceCoordinator: "serviceCoordinator",
            MedicaidNumbers: "medicaidNumbers"
        }

    var BaseModel = require('models/consumer.model'),
        PhoneModal = require('views/consumers/popups/consumer.phone.view'),
        ServiceCoordinatorModal = require('views/consumers/popups/consumer.service.coordinator.view'),
        ConsumerMedicaidNumbersModal = require('views/consumers/popups/consumer.medicaidnumbers.view'),
        AddressModal = require('views/consumers/popups/consumer.address.view'),
        DeleteDialog = require('views/confirm.view'),
        ModalStatusInfoView = require('views/consumers/popups/consumer.note.list.view'),
        ModelEmployeeInfo = require('models/employee.model'),
        ModalEmployeeInfoView = require('views/consumers/popups/consumer.employee.info.view'),
        BaseView = require('views/base.contact.search.view');

    return BaseView.extend({

        container: '<div id="consumer-info-view"></div>',
        el: "#consumer-info-view",
        events: function () {
            return {
                "click #add-phone": "onAddPhone",
                "click [data-action=edit]": "onEditPhone",
                "click [data-action=delete]": "onDeletePhone",
                "click #save-consumer": "onGlobalSave",
                "click #delete-consumer": "onGlobalDelete",
                "click [href=status-info]": "onStatusInfo",
                "click [href=advocate-info]": "onAdvocateInfo",
                "click [href=advocate-paper-info]": "onAdvocatePaperInfo",
                "click [href=coordinator-info]": "onCoordinatorInfo",
                "click #add-medicaid-number": "onAddMedicaidNumber",
                "click [data-action=edit-medicaid-number]": "onEditMedicaidNumber",
                "click [data-action=delete-medicaid-number]": "onDeleteMedicaidNumber",
                "click #add-service-coordinator": "onAddServiceCoordinator",
                "click [data-action=edit-service-coordinator]": "onEditServiceCoordinator",
                "click [data-action=delete-service-coordinator]": "onDeleteServiceCoordinator",
                "click #add-address": "onAddAddress",
                "click [data-action=edit-address]": "onEditAddress",
                "click [data-action=delete-address]": "onDeleteAddress",
            }

        },
        initialize: function () { },
        appendContainer: function () {
            $("#consumerInfoContainer").append(this.container);
        },
        template: function (serialized_model) {
            var templateHtml = $("#consumer-info-template").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            if (this.model == null) {
                this.model = new BaseModel();
            } else {
                var dateJson = this.model.get("DateOfBirth");
                var dateText = Utils.getDateText(dateJson);
                this.model.set("DateOfBirthText", dateText);
            }
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {
            $('#Gender').selectpicker({
                dropupAuto: false,
            });
            this.data = {};
            this.dataTable = {};
            this.getDataForRow = {};
            this.deleteFromGrid = {};
            this.setGridDataForRow();
            this.setDeleteGridData();
            this.model.setFormValidation();
            this.model.set("StatusChanges", null);
            this.setDateBirth();
            this.setSNN();
            this.setAdvocate("#AdvocateId");
            this.setAdvocate("#AdvocatePaperId");
            this.setMedicalInfo();
            this.setStatus();
            this.setMedicalService('#ServiceCoordinatorId');
            this.setMedicalService('#DHCoordinatorId');
            this.setMedicalService('#CHCoordinatorId');
            this.setPhoneTable();
            this.setServiceCoordinatorTable();
            this.setMedicaidNumbersTable();
            this.setAddressesTable();
            this.setField("#PrimaryDiagnosis");
            this.setField("#DayProgramId");
        },
        setGridDataForRow: function () {
            this.getDataForRow[gridTypes.ServiceCoordinator] = this.getServiceCoordinatorDataForRow;
            this.getDataForRow[gridTypes.Phone] = this.getPhoneDataForRow;
            this.getDataForRow[gridTypes.Address] = this.getAddressDataForRow;
            this.getDataForRow[gridTypes.MedicaidNumbers] = this.getMedicaidNumbersDataForRow;
        },
        setDeleteGridData: function () {
            this.deleteFromGrid[gridTypes.ServiceCoordinator] = this.deleteServiceCoordinator;
            this.deleteFromGrid[gridTypes.Phone] = this.deletePhone;
            this.deleteFromGrid[gridTypes.Address] = this.deleteAddress;
            this.deleteFromGrid[gridTypes.MedicaidNumbers] = this.deleteMedicaidNumber;
        },
        setField: function (id) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false
            });
            $(id).on('show.bs.select', function (e) {
                _this.model.resetField($(id));
            });
        },
        setMedicaidNumbersTable: function () {
            var _this = this;
            this.data[gridTypes.MedicaidNumbers] = this.getMedicaidNumbersDataGrid();
            this.dataTable[gridTypes.MedicaidNumbers] = $('#consumer-medicaid-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                scroller: true,
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data[gridTypes.MedicaidNumbers].dataSet,
                columns: _this.data[gridTypes.MedicaidNumbers].columns,
                language: {
                    "emptyTable": "No records"
                }
            });
        },
        setServiceCoordinatorTable: function () {
            var _this = this;
            this.data[gridTypes.ServiceCoordinator] = this.getServiceCoordinatorDataGrid();
            this.dataTable[gridTypes.ServiceCoordinator] = $('#service-coordinator-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                scroller: true,
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data[gridTypes.ServiceCoordinator].dataSet,
                columns: _this.data[gridTypes.ServiceCoordinator].columns,
                language: {
                    "emptyTable": "No records"
                }
            });
        },
        setAddressesTable: function () {
            var _this = this;
            this.data[gridTypes.Address] = this.getAddressesDataGrid();
            this.dataTable[gridTypes.Address] = $('#addresses-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                scroller: true,
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data[gridTypes.Address].dataSet,
                columns: _this.data[gridTypes.Address].columns,
                language: {
                    "emptyTable": "No records"
                }
            });
        },
        getAddressesDataGrid: function () {
            var dataModels = this.model.get("Addresses");
            var dataGrid = {
                columns: [
                    { title: "Address1" },
                    { title: "Address2" },
                    { title: "City" },
                    { title: "State" },
                    { title: "Zip" },
                    { title: "FromDate" },
                    { title: "ToDate" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels, function (model, key, list) {
                var data = _this.getAddressDataForRow(model);
                dataGrid.dataSet.push(data);
            });

            return dataGrid;
        },
        getAddressDataForRow: function (model) {

            var tml_btn = null;
            if ($("#button-template-addresses").length != 0) {
                var html_btn = $("#button-template-addresses").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["Address1"]);
            data.push(model["Address2"]);
            data.push(model["City"]);
            data.push(model["State"]);
            data.push(model["Zip"]);
            data.push(Utils.getDateText(model["FromDate"]));
            data.push(Utils.getDateText(model["ToDate"]));

            if (tml_btn != null) {
                var objToPass = { id: model["Id"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        getMedicaidNumbersDataGrid : function () {
            var dataModels = this.model.get("MedicaidNumbers");
            var dataGrid = {
                columns: [
                    { title: "MedicaidNo" },
                    { title: "FromDate "},
                    { title: "ToDate" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels, function (model, key, list) {
                var data = _this.getMedicaidNumbersDataForRow(model);
                dataGrid.dataSet.push(data);
            });

            return dataGrid;
        },
        getMedicaidNumbersDataForRow: function (model) {
            var tml_btn = null;
            if ($("#button-template-medicaid-numbers").length != 0) {
                var html_btn = $("#button-template-medicaid-numbers").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["MedicaidNo"]);
            data.push(Utils.getDateText(model["FromDate"]));
            data.push(Utils.getDateText(model["ToDate"]));

            if (tml_btn != null) {
                var objToPass = { id: model["Id"] };
                var res = tml_btn(objToPass)
                data.push(res);
            }
            return data;
        },
        getServiceCoordinatorDataGrid: function () {
            var dataModels = this.model.get("ServiceCoordinators");
            var dataGrid = {
                columns: [
                    { title: "Contact" },
                    { title: "FromDate" },
                    { title: "ToDate" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels, function (model, key, list) {
                var data = _this.getServiceCoordinatorDataForRow(model);
                dataGrid.dataSet.push(data);
            });
            
            return dataGrid;
        },
        getServiceCoordinatorDataForRow: function (model) {
            var tml_btn = null;
            if ($("#button-template-service-coordinators").length != 0) {
                var html_btn = $("#button-template-service-coordinators").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["ViewContactName"]);
            data.push(Utils.getDateText(model["FromDate"]));
            data.push(Utils.getDateText(model["ToDate"]));

            if (tml_btn != null) {
                var objToPass = { id: model["Id"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        getDataGrid: function () {

            var dataModels = this.model.get("Phones");
            var dataGrid = {
                columns: [
                    { title: "Phone type" },
                    { title: "Phone" },
                    { title: "Extension" },
                    { title: "Note" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels, function (model, key, list) {
                var data = _this.getDataForRow[gridTypes.Phone](model);
                dataGrid.dataSet.push(data);
            });

            return dataGrid;
        },
        setPhoneTable: function () {
            var _this = this;
            this.data[gridTypes.Phone] = this.getDataGrid();
            this.dataTable[gridTypes.Phone] = $('#phones-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                scroller: true,
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data[gridTypes.Phone].dataSet,
                columns: _this.data[gridTypes.Phone].columns,
                language: {
                    "emptyTable": "No records"
                }
            });

        },
        getPhoneDataForRow: function (model) {

            var tml_btn = null;
            if ($("#button-template-phones").length != 0) {
                var html_btn = $("#button-template-phones").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["PhoneTypeName"]);
            data.push(Utils.formatPhone(model["Phone"]));
            data.push(model["Extension"]);
            data.push(model["Note"]);
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerPhoneId"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        addRowToGrid: function (model, gridName) {
            var rowData = this.getDataForRow[gridName](model);
            this.dataTable[gridName].row.add(rowData).draw(false);
            this.dataTable[gridName].columns.adjust().draw();
        },
        removeRowGrid: function (id, gridName) {
            var _this = this;
            _.each(this.dataTable[gridName].rows().ids(), function (item, key) {
                var dataArray = _this.dataTable[gridName].row(key).data();
                var action = _.last(dataArray);
                var idRow = $(action).attr("data-row-id");
                if (idRow == id) {
                    _this.dataTable[gridName].row(key).remove().draw(false);
                    return false;
                }
            });
        },
        updateRow: function (model, gridName) {
            var rowData = this.getDataForRow[gridName](model);
            this.dataTable[gridName].row(this.currentIdxIndex).data(rowData).draw(false);
        },
        setModel: function (model) {
            this.model = model;
            this.render();
        },
        getAge: function (dateString) {
            var today = new Date();
            var birthDate = new Date(dateString);
            var age = today.getFullYear() - birthDate.getFullYear();
            var m = today.getMonth() - birthDate.getMonth();
            if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
                age--;
            }
            return age;
        },

        setSNN: function () {
            $("#SocialSecurityNo").inputmask({ "mask": "999-99-9999" }); //specifying options
        },
        setAge: function () {
            var date = $('#DateOfBirth').val();
            if (date.length > 0) {
                var ages = this.getAge(date);
                if (ages < 0 || isNaN(ages)) {
                    ages = 0;
                }
                $('#Age').val(ages + " years");
            }
        },
        setDateBirth: function () {
            var _this = this;
            $('#DateOfBirth').datepicker({ autoclose: true });
            $('#DateOfBirth').on("clearDate", function (e) {
                $('#Age').val("");
            });
            $('#DateOfBirth').on("changeDate", function (e) {
                _this.setAge();
            });
            $('#DateOfBirth').inputmask();
            _this.setAge();
        },
        setAdvocate: function (id) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/advocateslist/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };

                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Select an advocate...'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            result.push(
                                {
                                    'value': curr.AdvocateId,
                                    'text': curr.LastName + ', ' + curr.FirstName,
                                    'data': {
                                        'icon': 'glyphicon-user',
                                        'subtext': curr.CompanyName + " (" + curr.City + ", " + curr.State + ")"
                                    },
                                    'disabled': false
                                }
                            );
                        }
                    }
                    return result;
                },
                preserveSelected: true,
                requestDelay: 100
            });
        },
        setStatus: function () {
            var id = '#Status';
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false,
            });
            $(id).on('show.bs.select', function (e) {
                _this.model.resetField($(id));
            });
            $(id).on('changed.bs.select', function (e) {

                var to = $.trim($("option:selected", $(this)).html());
                var from = _this.model.get("StatusName");
                if ($.isEmpty(from)) {
                    from = 'Undefined';
                }
                _this.model.set("StatusChanges", { From: from, To: to });
            });

        },
        setMedicalInfo: function () {
            $('#PrimaryDiagnosis').selectpicker({
                dropupAuto: false,
            });
            $('#SecondaryDiagnosis').selectpicker({
                dropupAuto: false,
            });

            $('#DayProgramId').selectpicker({
                dropupAuto: false,
            });
        },
        triggerUpdate: function (id) {
            var list = $('#' + id).data()['AjaxBootstrapSelect'].list;
            list.destroy();
            list.cache = [];
            var event = $('#' + id).data()['AjaxBootstrapSelect'].options.bindEvent;
            $('#' + id).data()['selectpicker'].$searchbox.trigger(event);
        },
        onRender: function () {
            this.onRenderBase();
            this.showInactiveConsumerNotification();
            this.showPopUpNotes();
        },
        showInactiveConsumerNotification: function () {
            var inactiveConsumerStatus = this.model.get("StatusName") && this.model.get("StatusName").toLowerCase() === "inactive";
            var inactiveServicesCount = this.model.get("ApprovedServices")
                ? this.model.get("ApprovedServices").filter(function (item) { return item.Inactive }).length
                : 0;

            if (inactiveConsumerStatus || inactiveServicesCount > 0) {
                var message = "Inactive consumer";

                if (inactiveServicesCount > 0) {
                    message += " with " + inactiveServicesCount + " inactive service(s)";
                }
                Utils.notify(message, "warning",0);
            }
        },
        showPopUpNotes: function () {
            var notes = this.model.get("Notes");
            if (notes && notes.length > 0) {
                var popupNotes = notes.filter(function (el) {
                    return el.TypeId == GlobalData.NoteTypes.PopupNote;
                });

                if (popupNotes) {
                    popupNotes.forEach(function (el) {
                        Utils.notify(el.Notes, "info",0);
                    });
                }

            }
        },
        onAddMedicaidNumber: function() {
            ConsumerMedicaidNumbersModal.prototype.appendContainer();
            this.modal = new ConsumerMedicaidNumbersModal({ model: null, parentView: this});
            this.modal.render();
            this.modal.showModal();
        },
        onEditMedicaidNumber: function(event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable[gridTypes.MedicaidNumbers].row(row).index();
            var medicaidNumbers = this.model.get("MedicaidNumbers");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(medicaidNumbers, { Id: id });
            ConsumerMedicaidNumbersModal.prototype.appendContainer();
            this.modal = new ConsumerMedicaidNumbersModal({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onDeleteMedicaidNumber: function(event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this, type: gridTypes.MedicaidNumbers });
        },
        onAddServiceCoordinator: function () {
            ServiceCoordinatorModal.prototype.appendContainer();
            this.modal = new ServiceCoordinatorModal({ model: null, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onEditServiceCoordinator: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable[gridTypes.ServiceCoordinator].row(row).index();
            var serviceCoordinators = this.model.get("ServiceCoordinators");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(serviceCoordinators, { Id: id });
            ServiceCoordinatorModal.prototype.appendContainer();
            this.modal = new ServiceCoordinatorModal({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onAddAddress: function () {
            AddressModal.prototype.appendContainer();
            this.modal = new AddressModal({ model: null, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onDeleteAddress: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this, type: gridTypes.Address });

        },
        onEditAddress: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable[gridTypes.Address].row(row).index();
            var addresses = this.model.get("Addresses");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(addresses, { Id: id });
            AddressModal.prototype.appendContainer();
            this.modal = new AddressModal({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onDeleteServiceCoordinator: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this, type: gridTypes.ServiceCoordinator });
        },
        onAddPhone: function () {
            PhoneModal.prototype.appendContainer();
            this.modal = new PhoneModal({ model: null, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onEditPhone: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable[gridTypes.Phone].row(row).index();
            var phones = this.model.get("Phones");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(phones, { ConsumerPhoneId: id });
            PhoneModal.prototype.appendContainer();
            this.modal = new PhoneModal({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onDeletePhone: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this, type: gridTypes.Phone });
        },
        onPopupAddressSave: function (model) {
            this.modal.closeModal();
            var addresses = this.model.get("Addresses");

            if (model.New) {
                var count = 1;
                if (!$.isEmptyObject(addresses)) { count = addresses.length + 1; }
                else {
                    addresses = [];
                }

                addresses.push(model);
                this.addRowToGrid(model, gridTypes.Address);
                Utils.notify("The Address has been successfully added.");
            }
            else {
                addresses = addresses || [];
                _.extend(_.findWhere(addresses, { Id: model.Id }), model);
                this.updateRow(model, gridTypes.Address);
                Utils.notify("The Address has been  successfully updated.");

            }
            this.model.set("Addresses", addresses);
        },
        onPopupMedicaidNumberSave: function (model) {
            this.modal.closeModal();
            var medicaidNumbers = this.model.get("MedicaidNumbers");

            if (model.New) {
                var count = 1;
                if (!$.isEmptyObject(medicaidNumbers)) { count = medicaidNumbers.length + 1; }
                else {
                    medicaidNumbers = [];
                }

                medicaidNumbers.push(model);
                this.addRowToGrid(model, gridTypes.MedicaidNumbers);
                Utils.notify("The medicaid number has been successfully added.");
            } else {
                medicaidNumbers = medicaidNumbers || [];
                _.extend(_.findWhere(medicaidNumbers, { Id: model.Id }), model);
                this.updateRow(model, gridTypes.MedicaidNumbers);
                Utils.notify("The medicaid number has been successfully updated.");
            }
            this.model.set("MedicaidNumbers", medicaidNumbers);
        },
        onPopupServiceCoordinatorSave: function (model) {
            this.modal.closeModal();
            var serviceCoordinators = this.model.get("ServiceCoordinators");

            if (model.New) {
                var count = 1;
                if (!$.isEmptyObject(serviceCoordinators)) { count = serviceCoordinators.length + 1; }
                else {
                    serviceCoordinators = [];
                }

                serviceCoordinators.push(model);
                this.addRowToGrid(model, gridTypes.ServiceCoordinator);
                Utils.notify("The Service coordinator range has been successfully added.");
            }
            else {
                serviceCoordinators = serviceCoordinators || [];
                _.extend(_.findWhere(serviceCoordinators, { Id: model.Id }), model);
                this.updateRow(model, gridTypes.ServiceCoordinator);
                Utils.notify("The Service coordinator range has been  successfully updated.");

            }
            this.model.set("ServiceCoordinators", serviceCoordinators);
        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            var phones = this.model.get("Phones");

            if ($.isEmpty(model.ConsumerPhoneId)) {
                var count = 1;
                if (!$.isEmptyObject(phones)) count = phones.length + 1;
                else phones = [];
                model.ConsumerPhoneId = count + ".new";
                phones.push(model);
                this.addRowToGrid(model, gridTypes.Phone);
                Utils.notify("The phone has been successfully added.");
            }
            else {
                phones = phones != undefined ? phones : [];
                var isNewPhone = phones.find(function (item) {
                    return item.ConsumerPhoneId == model.ConsumerPhoneId
                });
                if (isNewPhone) {
                    _.extend(_.findWhere(phones, { ConsumerPhoneId: model.ConsumerPhoneId }), model);
                    this.updateRow(model, gridTypes.Phone);
                    Utils.notify("The phone has been successfully updated.");
                }
                else {
                    phones.push(model);
                    this.addRowToGrid(model, gridTypes.Phone);
                    Utils.notify("The phone has been successfully added.");
                }
            }
            this.model.set("Phones", phones);
        },
        onPopupSaveNote: function () {
            this.triggerMethod('update:notes');
        },
        deletePhone: function (_this, id, baseModel) {
            var phones = _this.model.get("Phones");

            phones = _.without(phones, _.findWhere(phones, { ConsumerPhoneId: id }));
            _this.model.set("Phones", phones);
            baseModel.deletePhoneData(id);
        },
        deleteMedicaidNumber: function (_this, id, baseModel) {
            baseModel.deleteMedicaidNumber(id, function (result) {
                var medicaidNumbers = _this.model.get("MedicaidNumbers");

                medicaidNumbers = _.without(medicaidNumbers, _.findWhere(medicaidNumbers, { Id: id }));
                _this.model.set("MedicaidNumbers", medicaidNumbers);
            });
        },
        deleteServiceCoordinator: function (_this, id, baseModel) {

            baseModel.deleteCoordinatorService(id, function (result) {
                var serviceCoordinators = _this.model.get("ServiceCoordinators");

                serviceCoordinators = _.without(serviceCoordinators, _.findWhere(serviceCoordinators, { Id: id }));
                _this.model.set("ServiceCoordinators", serviceCoordinators);
            });
        },
        deleteAddress: function (_this, id, baseModel) {

            baseModel.deleteAddress(id, function (result) {
                var addresses = _this.model.get("Addresses");

                addresses = _.without(addresses, _.findWhere(addresses, { Id: id }));
                _this.model.set("Addresses", addresses);
            });
        },
        onConfirm: function (id, type) {
            var _this = this;
            var baseModel = new BaseModel();
            var deleteFromgrid = this.deleteFromGrid[type];
            if (deleteFromgrid) {
                deleteFromgrid(_this, id, baseModel);
                this.removeRowGrid(id, type);
            }
            else {
                var success = function () {
                    _this.triggerMethod('delete:consumer');
                }
                baseModel.deleteData(id, success);
            }
        },
        getObj: function (elm) {
            var company = $("option:selected", elm).attr("data-subtext");
            var name = $.trim($("option:selected", elm).html());
            var id = elm.val();
            return { Id: id, Name: name, Company: company };
        },
        onSave: function (isValidPrevious) {

            var _this = this;
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);
                obj.AdvocateName = "";
                if ($("#AdvocateId", "#consumer-info-form").val().length > 0) {
                    obj.AdvocateName = $.trim($("option:selected", $("#AdvocateId", "#consumer-info-form")).html());
                }
                obj.AdvocatePaperName = "";
                if ($("#AdvocatePaperId", "#consumer-info-form").val().length > 0) {
                    obj.AdvocatePaperName = $.trim($("option:selected", $("#AdvocatePaperId", "#consumer-info-form")).html());
                }
                obj.ServiceCoordinator = this.getObj($("#ServiceCoordinatorId"));
                obj.DH = this.getObj($("#DHCoordinatorId"));
                obj.CH = this.getObj($("#CHCoordinatorId"));

                obj.StatusName = "";
                if ($("#Status", "#consumer-info-form").val().length > 0) {
                    obj.StatusName = $.trim($("option:selected", $("#Status", "#consumer-info-form")).html());
                }
                this.model.parseObj(obj);
                obj = this.model.getObject();
                var success = function (response) {
                    Utils.notify("The consumer info has been successfully updated.");
                    _this.model.set("ConsumerId", response.id);
                    _this.render();
                };
                this.model.postData(success);
                return true;
            }
            return false;

        },
        onGlobalSave: function () {
            this.triggerMethod('save:consumer');
        },
        onGlobalDelete: function (event) {
            var id = this.model.get("ConsumerId");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = {};
            model["message"] = "Are you sure you want to delete full consumer information?";
            model["title"] = "Delete consumer?";
            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog({ model: model });
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });
        },
        onStatusInfo: function () {
            ModalStatusInfoView.prototype.appendContainer();
            this.modal = new ModalStatusInfoView({ parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onAdvocateInfo: function () {
            var id = $("#AdvocateId").val();
            if ($.isEmpty(id)) {
                GlobalEvents.trigger("showMessage", { title: "Can't perform the action...", message: "Please select advocate first", type_class: "alert-danger" });
                return;
            }
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
        onAdvocatePaperInfo: function () {
            var id = $("#AdvocatePaperId").val();
            if ($.isEmpty(id)) {
                GlobalEvents.trigger("showMessage", { title: "Can't perform the action...", message: "Please select advocate first", type_class: "alert-danger" });
                return;
            }
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
        onCoordinatorInfo: function (event) {
            var idElm = $(event.currentTarget).attr("data-id");
            var id = $("#" + idElm).val();
            if ($.isEmpty(id)) {
                GlobalEvents.trigger("showMessage", { title: "Can't perform the action...", message: "Please select coordinator first", type_class: "alert-danger" });
                return;
            }
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
        }

    });

});