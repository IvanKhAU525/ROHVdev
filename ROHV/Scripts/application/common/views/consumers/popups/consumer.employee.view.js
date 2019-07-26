define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.employee.model'),
        BaseView = require('views/file.upload.base.view');

    return BaseView.extend({

        container: '<div id="employee-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#employee-view",
        employeesData: [],
        events: function () {
            return {
                "click #save-modal": "onSave",
                "click [data-action=download-file-popup]": "onDownload"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        template: function (serialized_model) {
            var templateHtml = $("#pop-template-employee").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
            }
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {
            $('#StartDate').datepicker({ autoclose: true });
            $('#StartDate').inputmask();
            $('#EndDate').datepicker({ autoclose: true });
            $('#EndDate').inputmask();
            this.setServiceField();
            this.setEmployee();
            this.model.setFormValidation();
            this.setFileEvent();
            this.setFileName();
            this.onRenderBase();
            this.setupDropzone('#dropzone');
        },
        setServiceField: function () {
            var _this = this;
            $('#ServiceId').selectpicker({
                dropupAuto: false
            });
            $('#ServiceId').on('show.bs.select', function (e) {
                _this.model.resetField($('#ServiceId'));
            });
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
            $('#ContactId').selectpicker({
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
            var isEdit = this.model.get("ConsumerEmployeeId") != null;
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
                obj.ContactName = "";
                if ($("#ContactId").val().length > 0) {
                    obj.ContactName = $.trim($("option:selected", "#ContactId").html());
                    obj.CompanyName = $.trim($("option:selected", "#ContactId").attr("data-subtext"));
                }
                if (!$.isEmptyObject(this.employeesData)) {
                    var worker = _.findWhere(this.employeesData, { ContactId: parseInt(obj.ContactId) });
                    obj.Email = worker.Email;
                }
                obj.ConsumerId = this.parentView.model.get("ConsumerId");          

                var isUpdate = this.isEdit();
                var needToSaveFile = this.fileData != null;

                obj.FileName = this.fileData ? this.fileData.name : this.model.get("FileName");

                var saveFileCallBack = function () {

                    _this.model.parseObj(obj);
                    var returnObj = _this.model.getObject();
                    if ($.isEmpty(obj.ConsumerId)) {
                        _this.parentView.onPopupSave(returnObj);
                    } else {
                        var success = function (responce) {
                            returnObj["ConsumerEmployeeId"] = responce.id;
                            returnObj["isUpdate"] = isUpdate;
                            returnObj["FileId"] = responce.fileId;
                            returnObj["FileName"] = responce.fileName;
                            _this.parentView.onPopupSave(returnObj);
                        };
                        _this.model.postData(success);
                    }
                };
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