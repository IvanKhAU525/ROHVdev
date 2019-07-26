define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.document.model'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/file.upload.base.view');

    return BaseView.extend({

        container: '<div id="document-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#document-view",
        events: function () {
            return {
                "click #save-modal": "onSave",
                "click [data-action=add]": "onAdd",
                "click [data-action=delete]": "onDelete",
                "click [data-action=download-file-popup]": "onDowload"
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
            var templateHtml = $("#pop-template-document").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
            }
            var values = this.model.get("Notes");
            _.each(values, function (model, key, list) {
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
            });
            this.model.set("Notes", values);

            this.model.set("DateDocument", Utils.getDateText(this.model.get("DateDocument")));
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {

            this.model.setFormValidation();
            this.setField('#DocumentTypeId');
            this.setField('#DocumentStatusId');
            this.setEmployeeField();
            this.setFileEvent();
            this.setNotesTable();
            $('#DateDocument').datepicker({ autoclose: true });
            $('#DateDocument').inputmask();
            this.onRenderBase();
            this.setFileName();
            this.setupDropzone('#dropzone');
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
        setEmployeeField: function () {
            var _this = this;
            var consumerId = this.parentView.model.get("ConsumerId");
            $('#EmployeeId').selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/searchingemployeebyconsumer/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}',
                            consumerId: consumerId
                        };
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Select employee name...'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
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
                processData: function () { },
                preserveSelected: true,
                clearOnEmpty: true,
                log: false,
                requestDelay: 100
            });

            $("#EmployeeId").on('show.bs.select', function (e) {
                _this.model.resetField($("#EmployeeId"));
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
            var isEdit = this.model.get("EmployeeDocumentId") != null;
            return isEdit;
        },
        onSave: function () {
            var _this = this;
            if (this.model.isValid()) {
                var isEdit = this.isEdit();
                var obj = Utils.serializeForm(this.model.formId);
                obj.EmployeeName = "";
                if ($("#EmployeeId").val().length > 0) {
                    obj.EmployeeName = $.trim($("option:selected", "#EmployeeId").html());
                    obj.EmployeeCompanyName = $.trim($("option:selected", "#EmployeeId").attr("data-subtext"));
                }
                obj.DocumentTypeName = "";
                if ($("#DocumentTypeId").val().length > 0) {
                    obj.DocumentTypeName = $.trim($("option:selected", "#DocumentTypeId").html());
                }

                obj.DocumentTypeColor = $("option:selected", "#DocumentTypeId").attr("document-type-color");

                obj.DocumentStatusName = "";
                if ($("#DocumentStatusId").val().length > 0) {
                    obj.DocumentStatusName = $.trim($("option:selected", "#DocumentStatusId").html());
                }
                obj.ConsumerId = this.parentView.model.get("ConsumerId");
                if (isEdit) {
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
                obj.Notes = this.model.get("Notes");
                for (var i = 0; i < obj.Notes.length; i++) {
                    obj.Notes[i]["EmployeeDocumentNoteId"] = i;
                }
                obj.NumberNotes = obj.Notes.length;
                obj.FileData = null;
                var needToSaveFile = this.fileData != null;
                var saveFileCallBack = function () {
                    _this.model.parseObj(obj);                   
                    var returnObj = _this.model.getObject();
                    if ($.isEmpty(obj.ConsumerId)) {
                        _this.parentView.onPopupSave(returnObj);
                    }
                    else {
                        var success = function (responce) {
                            returnObj["EmployeeDocumentId"] = responce.id;
                            returnObj["isUpdate"] = isEdit;
                            if (needToSaveFile) {
                                returnObj["FileData"] = null;
                                returnObj["DocumentPath"] = responce.filePath;
                            }
                            obj.FileData = null;
                            
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

        getDataGrid: function () {

            var dataModels = this.model.get("Notes");
            var dataGrid = {
                columns: [
                    { title: "Note" },
                    { title: "Date" },
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
        setNotesTable: function () {
            this.model.setNoteFormValidation();
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#document-notes-grid').DataTable({
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
            if ($("#button-template-document-notes").length != 0) {
                var html_btn = $("#button-template-document-notes").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];

            data.push(model["Note"]);
            data.push(model["DateCreated"]);
            data.push(model["AddedByName"]);
            if (tml_btn != null) {
                var objToPass = { id: model["EmployeeDocumentNoteId"] };
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
            if (this.model.validateNote()) {
                var note = $("#DocumentNote").val();
                var obj = { AddedByName: CurrentUserInfo.Name, AddedById: CurrentUserInfo.Id, Note: note, DateCreated: Utils.getCurrentDate() };
                var values = this.model.get("Notes");
                var count = 1;
                if (!$.isEmptyObject(values)) count = values.length + 1;
                else values = [];
                obj.EmployeeDocumentNoteId = count + ".new";
                values.push(obj);
                this.addRowToGrid(obj);
                this.model.set("Notes", values);
                $("#DocumentNote").val("");
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
            var values = this.model.get("Notes");
            values = _.without(values, _.findWhere(values, { EmployeeDocumentNoteId: id }));
            this.model.set("Notes", values);
            this.removeRowGrid(id);
        },
        onDowload: function () {
            var id = this.model.get("EmployeeDocumentId");
            if ($.isNumeric(id)) {
                id = parseInt(id);
                var documents = this.model.get("Documents");
                var model = this.model;
                if (!$.isEmpty(model.get("DocumentPath"))) {
                    var win = window.open("/api/consumerdocumentsapi/getdocumenthandler?id=" + id, "_blank");
                    //  location.href = "/api/consumerdocumentsapi/getdocumenthandler?id=" + id
                } else {
                    GlobalEvents.trigger("showMessage", { title: "Error", message: "The file hasn't uploaded yet.", type_class: "alert-danger" });
                }
            } else {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "You can't download not saving document. Please save consumer info and try again.", type_class: "alert-danger" });
                return;
            }
            return false;
        },
        setFileName: function () {
            if (this.model) {
                var documentPath = this.model.get("DocumentPath");
                if (documentPath && documentPath.split('.')[1]) {
                    var fileName = this.model.get("EmployeeName").replace(', ', '_')
                    fileName += " (" + this.model.get("DocumentTypeName") + ")";
                    fileName += "." + documentPath.split('.')[1];
                    fileName = "<b>" + fileName + "</b>";
                    $("#file-name").html(fileName);
                }
            }
        }
    });

});