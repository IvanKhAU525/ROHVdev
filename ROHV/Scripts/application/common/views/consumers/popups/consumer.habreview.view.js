define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.habreview.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({
        container: '<div id="hab-review-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#hab-review-view",
        employeesData: [],
        signatureConfirmedData: [],
        forceNew: false,
        events: function () {
            return {
                "click #save-modal": "onSave",
                "click #save-as-new-modal": "onSaveAsNew"          
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        template: function (serialized_model) {
            var templateHtml = $("#pop-template-habreview").html();
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
            this.employeesData = [];
            this.signatureConfirmedData = [];

            this.setServiceField("#ServiceId");
            this.setEmployeeField("#CHCoordinatorId");
            this.setEmployeeField("#DHCoordinatorId");
            this.setEmployeeField("#MSCId");
            this.setStates();
            this.model.setFormValidation();
            var container = $("#hab-review-view");
            $('input[data-inputmask]:not([readonly])', container).datepicker({ autoclose: true });
            $('input[data-inputmask]:not([readonly])', container).inputmask();

            this.onRenderBase();
        },       
        setSignatureDate: function () {
            var isSignConfirmed = this.isSignatureConfirmed();

            if (isSignConfirmed && !$("#SignatureDate").val()) {
                $("#SignatureDate").val(Utils.getDateText(new Date()));
            }
            if (!isSignConfirmed) {
                $("#SignatureDate").val('');
            }
        },
        getStateList: function () {
            var issues = this.model.get("ConsumerHabReviewIssueStates");
            var stateList = [];
            for (var issue in issues) {
                stateList.push(issue);
            }
            return stateList;
        },
        setStates: function () {
            var stateList = this.getStateList();
            var issues = this.model.get("ConsumerHabReviewIssueStates");
            for (var i = 0; i < stateList.length; i++) {
                var stateName = stateList[i];
                var value = issues[stateName];
                $('input[name="' + stateName + '"][value="' + value + '"]').prop('checked', true);
            }
        },

        setServiceField: function (id) {
            var _this = this;
            $(id, "#hab-review-form").selectpicker({
                dropupAuto: false
            });
            $(id, "#hab-review-form").on('show.bs.select',
                function (e) {
                    _this.model.resetField($(id));
                });

            $(id, "#hab-review-form").on('changed.bs.select', function (e) {
                _this.resetSignatureConfirmation("CHCoordinator");
                _this.resetSignatureConfirmation("DHCoordinator");
                _this.setSignaturePassword("CHCoordinator");
                _this.setSignaturePassword("DHCoordinator");
            });

            var val = this.model.get(id.replace("#", ""));
            if (val != null) {
                $(id, "#hab-review-form").val(val);
                $(id, "#hab-review-form").selectpicker("refresh");
            }
        },

        setEmployeeField: function (id) {
            var _this = this;
            $(id, "#hab-review-form").selectpicker({
                dropupAuto: false
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
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            _this.employeesData[curr.ContactId] = curr;
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

            var modelName = this.getModelName(id);
            var val = this.model.get(modelName);
            if (val != null) {
                $(id, "#hab-review-form").val(val.ContactId);
                $(id, "#hab-review-form").selectpicker("refresh");
            }
            
        
            this.setSignatureData(id);
            this.setSignaturePassword(modelName);           
            $(id, "#hab-review-form").on('changed.bs.select', function (e) {
                _this.resetSignatureConfirmation(modelName);
                _this.setSignaturePassword(modelName);
            });
        },
        getContactId: function (modelName) {
            var id = "#" + modelName + "Id";
            var contactId = $(id, "#hab-review-form").val();
            return contactId;
        },
        isModelForSignature: function (modelName) {
            var id = "#" + modelName + "Id";
            var contactId = $(id, "#hab-review-form").val();
            if (!contactId) return false;
            var model = this.employeesData[contactId]; //this.model.get(modelName);
            var serviceId = $("#ServiceId", "#hab-review-form").val();
            var isCurrent = false;

            if (modelName.indexOf("CH") !== -1 && serviceId == 1) {
                isCurrent = true;
            }

            if (modelName.indexOf("DH") !== -1 && serviceId == 2) {
                isCurrent = true;
            }
            if (isCurrent && model != null && model.IsHaveSignature) {
                return true;
            }
            return false;
        },
        isSignatureConfirmed: function () {
            var serviceId = $("#ServiceId", "#hab-review-form").val();
            var id = $("#DHCoordinatorId", "#hab-review-form").val();
            var containerName = this.getSignatureContainer("DHCoordinator");
            if (serviceId == 1) {
                id = $("#CHCoordinatorId", "#hab-review-form").val();
                containerName = this.getSignatureContainer("CHCoordinator");
            }
            var isSetAutoSignature = $("#IsAutoSignature", containerName).is(":checked");
            return this.isConfirmedSignature(id) && isSetAutoSignature;
        },     
        setSignatureData: function (id) {

            var modelName = this.getModelName(id);
            var model = this.model.get(modelName);
            if (model != null) {
                this.employeesData[model.ContactId] = model;
                var isNeededModel = this.isModelForSignature(modelName);
                if (this.model.get("IsAutoSignature") && isNeededModel) {
                    this.addConfirmationSignature(model.ContactId);
                    this.showConfirmedSignatureMessage(modelName);
                }
            }
        },

        addConfirmationSignature: function (contactId) {
            this.signatureConfirmedData[contactId] = true;
        },

        isConfirmedSignature: function (contactId) {
            return this.signatureConfirmedData[contactId];
        },

        showConfirmedSignatureMessage: function (modelName) {
            var container = this.getSignatureContainer(modelName);
            this.showInline("#SignaturePassword-confirmed", container);
        },
        getSignatureContainer: function (modelName) {
            return "#" + modelName + "SignatureContainer";
        },
        getModelName: function (id) {

            return id.replace("#", "").replace("Id", "");
        },
        showInline: function (id, container) {
            $(id, container).show();
            $(id, container).css("display", "inline-block");
        },

        setSignaturePassword: function (modelName) {
            var _this = this;            
            _this.setSignatureDate();            
            var isNeededModel = this.isModelForSignature(modelName);
            var containerName = this.getSignatureContainer(modelName);
            if (!isNeededModel) {
                $(containerName).hide();
                return;
            }

            
            $(containerName).show();
            $("#IsAutoSignature", containerName).off("click").on("click",
                function () {
                    if ($("#IsAutoSignature", containerName).is(":checked")) {
                        _this.setSignaturePasswordConfirmation(modelName);
                    } else {
                        _this.resetSignatureConfirmation(modelName);
                    }
                    _this.setSignatureDate();
                });
        },
        resetSignatureConfirmation: function (modelName) {
            var containerName = this.getSignatureContainer(modelName);
            $("#SignaturePassword-error", containerName).hide();
            $("#signature-password", containerName).hide();
            $("#IsAutoSignature", containerName).prop('checked', false);
            $("#SignaturePassword-confirmed", containerName).hide();
            $("#SignaturePassword", containerName).val("");
        },

        setSignaturePasswordConfirmation: function (modelName) {
            var _this = this;
            var container = this.getSignatureContainer(modelName);
            var contactId = $("#" + modelName + "Id", "#hab-review-form").val();
            if (this.isConfirmedSignature(contactId)) {
                //Just show that password was confirmed
                this.showConfirmedSignatureMessage(modelName);
                return;
            }
            this.showInline("#signature-password", container);
            $("#SignaturePassword-error", container).hide();
            $("[data-action='confirmPassword']", container).off("click").on("click",
                function () {
                    var password = $("#SignaturePassword", container).val();
                    var s = function (isOk) {
                        if (!isOk) {
                            $("#SignaturePassword-error", container).show();
                        } else {
                            $("#SignaturePassword-error", container).hide();
                            $("#signature-password", container).hide();
                            _this.addConfirmationSignature(contactId);
                            _this.showConfirmedSignatureMessage(modelName);
                        }
                        _this.setSignatureDate();
                    };
                    var e = function () { $("#SignaturePassword-error", container).show(); };
                    _this.model.checkPassword(password, contactId, s, e);
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
            if (this.forceNew) return false;
            var isEdit = this.model.get("ConsumerHabReviewId") != null;
            return isEdit;
        },
        serializeIssues: function () {
            var stateList = this.getStateList();
            var result = {};
            for (var i = 0; i < stateList.length; i++) {
                var stateName = stateList[i];
                var value = $('input[name="' + stateName + '"]:checked').val();
                if ($.isEmpty(value)) value = null;
                else value = parseInt(value);
                result[stateName] = value;
            }
            return result;
        },
        onSaveAsNew: function () {
            this.forceNew = true;
            this.onSave();
            this.forceNew = false;
        },
        onSave: function () {

            var _this = this;
            if (this.model.isValid()) {

                var obj = Utils.serializeForm(this.model.formId);
                obj.ServiceName = "";
                if (!$.isEmpty($("#ServiceId", "#hab-review-form").val())) {
                    obj.ServiceName = $.trim($("option:selected", $("#ServiceId", "#hab-review-form")).html());
                }
                obj.CHCoordinator = null;
                var coordinatorId = $("#CHCoordinatorId", "#hab-review-form").val();
                if (!$.isEmpty(coordinatorId)) {
                    obj.CHCoordinator = _this.employeesData[coordinatorId];
                }
                obj.DHCoordinator = null;
                coordinatorId = $("#DHCoordinatorId", "#hab-review-form").val();
                if (!$.isEmpty(coordinatorId)) {
                    obj.DHCoordinator = _this.employeesData[coordinatorId];
                }
                obj.MSC = null;
                coordinatorId = $("#MSCId", "#hab-review-form").val();
                if (!$.isEmpty(coordinatorId)) {
                    obj.MSC = _this.employeesData[coordinatorId];
                }

                obj.IsAutoSignature = this.isSignatureConfirmed();


                obj.EmployeeName = "";
                if (!$.isEmpty($("#ContactId", "#hab-review-form").val())) {
                    obj.EmployeeName = $.trim($("option:selected", $("#ContactId", "#hab-review-form")).html());
                }
                obj.ConsumerId = this.parentView.model.get("ConsumerId");
                obj.ConsumerHabReviewIssueStates = this.serializeIssues();

                if (this.isEdit()) {
                    obj.UpdatedById = CurrentUserInfo.Id;
                    obj.DateUpdated = Utils.getCurrentDate();
                    obj.UpdatedByName = CurrentUserInfo.Name;
                    obj.AddedById = this.model.get("AddedById");
                    obj.DateCreated = this.model.get("DateCreated");
                    obj.AddedByName = this.model.get("AddedByName");
                    obj.ConsumerHabReviewId = this.model.get("ConsumerHabReviewId");
                } else {
                    obj.AddedById = CurrentUserInfo.Id;
                    obj.DateCreated = Utils.getCurrentDate();
                    obj.AddedByName = CurrentUserInfo.Name;
                    obj.UpdatedById = null;
                    obj.DateUpdated = "";
                    obj.UpdatedByName = "";
                    obj.ConsumerHabReviewId = null;
                }

                this.model.parseObj(obj);
                var returnObj = this.model.getObject();

                var isUpdate = this.isEdit();
                if ($.isEmpty(obj.ConsumerId)) {
                    this.parentView.onPopupSave(returnObj);
                } else {
                    var success = function (responce) {
                        returnObj["ConsumerHabReviewId"] = responce.id;
                        returnObj["isUpdate"] = isUpdate;
                        _this.parentView.onPopupSave(returnObj);
                    };
                    this.model.postData(success);
                }
            }
        }
    }); /*.end module*/

}); /*.end defined*/