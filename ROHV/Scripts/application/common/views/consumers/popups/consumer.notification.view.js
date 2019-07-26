define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.notification.model'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="notification-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#notification-view",
        contactCurrent: null,
        employeesData: [],
        events: function () {
            return {
                "click #save-modal": "onSave",
                "click #add-recipient": "onAdd",
                "click [data-action=delete]": "onDelete"
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
            var templateHtml = $("#pop-template-notification").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
            }
            var values = this.model.get("Recipients");
            _.each(values, function (model, key, list) {
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
            });
            this.model.set("Recipients", values);
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {
            this.model.setRecipientFormValidation();
            this.model.setFormValidation();
            $('#DateStart').datepicker({ autoclose: true });
            $('#DateStart').inputmask();

            this.setEmployee();
            this.setStatusField();
            this.setRepetingField();
            this.setTable();
            this.onRenderBase();
        },
        addtoEmployeeSearchData: function (items) {
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var data = _.findWhere(this.employeesData, { ContactId: parseInt(item.ContactId) });
                if (data != undefined) {
                    continue
                }
                this.employeesData.push(item);
            }
        },
        setEmployee: function () {
            var _this = this;
            $('#Contact').selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/searchingemployee/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Select employee name'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
                        _this.addtoEmployeeSearchData(data.data);
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            result.push(
                                {
                                    'value': curr.ContactId,
                                    'text': curr.LastName + ', ' + curr.FirstName,
                                    'data': {
                                        'icon': 'glyphicon-user',
                                        'subtext': curr.CompanyName
                                    },
                                    'disabled': false
                                }
                            );
                        }
                    }
                    return result;
                },
                processData: function () {
                },
                preserveSelected: true,
                clearOnEmpty: true,
                log: false,
                requestDelay: 100
            });
            $('#Contact').on('changed.bs.select', function (e, d) {
                var id = $("#Contact").val();
                var data = _.findWhere(_this.employeesData, { ContactId: parseInt(id) });
                $("#Position").val(data["Position"]);
                $("#Email").val(data["Email"]);
            });
        },
        setStatusField: function () {
            var _this = this;
            $('#StatusId').selectpicker({
                dropupAuto: false
            });
            $('#StatusId').on('show.bs.select', function (e) {
                _this.model.resetField($('#StatusId'));
            });
        },
        setRepetingField: function () {
            var _this = this;
            $('#RepetingTypeId').selectpicker({
                dropupAuto: false
            });
            $('#RepetingTypeId').on('show.bs.select', function (e) {
                _this.model.resetField($('#RepetingTypeId'));
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

        isEdit: function () {
            var isEdit = this.model.get("Id") != null;
            return isEdit;
        },
        getRecipientsString: function () {
            var dataModels = this.model.get("Recipients");
            var list = [];
            for (var i = 0; i < dataModels.length; i++) {
                if ($.isEmpty(dataModels[i].Name)) {
                    list.push(dataModels[i].Email);
                } else {
                    list.push(dataModels[i].Name);
                }
            }
            if (list.length > 0) {
                return list.join("; ");
            }
            return "";
        },
        onSave: function () {
            var _this = this;
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);
                obj.RepetingTypeName = "";
                if ($("#RepetingTypeId").val().length > 0) {
                    obj.RepetingTypeName = $.trim($("option:selected", "#RepetingTypeId").html());
                }
                obj.StatusName = "";
                if ($("#StatusId").val().length > 0) {
                    obj.StatusName = $.trim($("option:selected", "#StatusId").html());
                }
                obj.RecipientsString = this.getRecipientsString();
                obj.Recipients = this.model.get("Recipients");
                obj.ConsumerId = this.parentView.model.get("ConsumerId");
                var isUpdate = this.isEdit();

                if (this.isEdit()) {
                    obj.UpdatedById = CurrentUserInfo.Id;
                    obj.DateUpdated = Utils.getCurrentDate();
                    obj.UpdatedByName = CurrentUserInfo.Name;
                    obj.AddedById = this.model.get("AddedById");
                    obj.DateCreated = this.model.get("DateCreated");
                    obj.AddedByName = this.model.get("AddedByName");

                } else {
                    obj.AddedById = CurrentUserInfo.Id;
                    obj.DateCreated = Utils.getCurrentDate();
                    obj.AddedByName = CurrentUserInfo.Name;
                    obj.UpdatedById = null;
                    obj.DateUpdated = "";
                    obj.UpdatedByName = "";
                }


                this.model.parseObj(obj);
                var returnObj = this.model.getObject();
                if ($.isEmpty(obj.ConsumerId)) {
                    this.parentView.onPopupSave(returnObj);
                } else {
                    var success = function (responce) {
                        returnObj["Id"] = responce.id;
                        returnObj["isUpdate"] = isUpdate;
                        _this.parentView.onPopupSave(returnObj);
                    };
                    this.model.postData(success);
                }

            }
        },
        getDataGrid: function () {

            var dataModels = this.model.get("Recipients");
            var dataGrid = {
                columns: [
                    { title: "Position" },
                    { title: "Name" },
                    { title: "Email" },
                    { title: "Date added" },
                    { title: "Added by" },
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
            this.dataTable = $('#recipients-grid').DataTable({
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
            if ($("#button-template-recipient").length != 0) {
                var html_btn = $("#button-template-recipient").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];

            data.push(model["Position"]);
            data.push(model["Name"]);
            data.push(model["Email"]);
            data.push(model["DateCreated"])
            data.push(model["AddedByName"])
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

        onAdd: function () {
            if (this.model.validateRecipient()) {
                var position = $("#Position").val();
                var name = $.trim($("option:selected", "#Contact").html());
                var email = $("#Email").val();
                var obj = { Email: email, Name: name, Position: position, AddedById: CurrentUserInfo.Id, DateCreated: Utils.getCurrentDate(), AddedByName: CurrentUserInfo.Name };
                var values = this.model.get("Recipients");
                var count = 1;
                if (!$.isEmptyObject(values)) count = values.length + 1;
                else values = [];
                obj.Id = count + ".new";
                values.push(obj);
                this.addRowToGrid(obj);
                this.model.set("Recipients", values);
                $("#Position").val("");
                $("#Email").val("");
                $("#Contact").val("").selectpicker('refresh');
            }
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

        onConfirm: function (id) {
            var values = this.model.get("Recipients");
            values = _.without(values, _.findWhere(values, { Id: id }));
            this.model.set("Recipients", values);
            this.removeRowGrid(id);
        }
    });

});