﻿define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.habplan.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="hab-plan-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#hab-plan-view",
        employeesData: [],
        signatureConfirmedData: [],
        events: function () {
            return {
                "click #save-modal": "onSave",
                "click #save-as-new-modal": "onSaveAsNew",
                "click [data-action=remove-valued-outcome]": "onRemoveValuedOutcome",
                "click [data-action=add-valued-outcome]": "onAddValuedOutcome",
                "click [data-action=add-new-action]": "onAddNewAction",
                "click [data-action=remove-action]": "OnRemoveAction",
                "click [data-action=add-safeguard]": "onAddSafeguard",
                "click [data-action=remove-safeguard]": "onRemoveSafeguard"              
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        template: function (serialized_model) {
            var templateHtml = $("#pop-template-habplan").html();
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

            var container = $("#hab-plan-view");
            $('input[data-inputmask]:not([readonly])', container).datepicker({ autoclose: true });
            $('input[data-inputmask]:not([readonly])', container).inputmask();                

            this.setField("#StatusId");
            this.setField("#DurationId");
            this.setField("#FrequencyId");
            this.setHabServiceField();
            this.setCoordinatorField();
            this.model.setFormValidation();
            this.setVauedOutcome();
            this.setSafeguards();

            this.setNameField();

            this.onRenderBase();
        },
        setHabServiceField: function () {
            var id = "#HabServiceId";
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false
            });
            $(id).on('show.bs.select', function (e) {
                _this.model.resetField($(id));
            });
            $(id).on('changed.bs.select', function (e) {
                _this.setNameField();
                _this.setEnrollDate();
            });
        },
        setEnrollDate: function () {
            if (!this.isEdit()) {
                var id = parseInt($("#HabServiceId").val());
                var foundValue = _.find(this.parentView.model.get("ApprovedServices"), { ServiceId: id });
                if (!$.isEmptyObject(foundValue)) {
                    $("#EnrolmentDate").val(foundValue["EffectiveDate"]);
                } else {
                    $("#EnrolmentDate").val("");
                }
            }
        },       
        setSignatureDate: function (contactId) {

            var isSignConfirmed = contactId && this.isConfirmedSignature(contactId);

            if (isSignConfirmed && !$("#SignatureDate").val()) {
                $("#SignatureDate").val(Utils.getDateText(new Date()));
            }
            if (!isSignConfirmed) {
                $("#SignatureDate").val('');
            }
        },
        setNameField: function () {
            if (!this.isEdit()) {
                var firstName = this.parentView.model.get("FirstName");
                var lastName = this.parentView.model.get("LastName");
                var habservicename = "";
                if (!$.isEmpty($("#HabServiceId").val())) {
                    habservicename = $.trim($("option:selected", "#HabServiceId").html());
                }
                $("#Name").val(firstName + " " + lastName + " " + habservicename);
            }
        },
        setField: function (id) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false
            });
            $(id).on('show.bs.select', function (e) {
                _this.model.resetField($(id));
            });
            var val = this.model.get(id.replace("#", ""));
            if (val != null) {
                $(id).val(val);
                $(id).selectpicker("refresh");
            }
        },
        setCoordinatorField: function () {

            var _this = this;
            var id = "#CoordinatorId";
            $(id).selectpicker({
                dropupAuto: false
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/servicecoordinatorslist/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Select coordinator name'
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
                preserveSelected: false,
                clearOnEmpty: true,
                log: false,
                requestDelay: 100
            });

            var coordinator = this.model.get("Coordinator");
            if (coordinator != null) {
                this.employeesData[coordinator.ContactId] = coordinator;
                $(id).val(coordinator.ContactId);
                $(id).selectpicker("refresh");
                if (this.model.get("IsAutoSignature")) {
                    this.addConfirmationSignature(coordinator.ContactId);
                    this.showConfirmedSignatureMessage();
                }
            }

            this.setSignatureFields(coordinator);
            $(id).on('changed.bs.select', function (e) {
                var coordinator = _this.employeesData[$(id).val()];
                _this.resetSignatureConfirmation();
                _this.setSignatureFields(coordinator);
            });

        },
        addConfirmationSignature: function (contactId) {
            this.signatureConfirmedData[contactId] = true;
        },
        isConfirmedSignature: function (contactId) {
            return this.signatureConfirmedData[contactId] && $("#IsAutoSignature").is(":checked");
        },
        showConfirmedSignatureMessage: function () {
            this.showInline("#SignaturePassword-confirmed");
        },
        setSignatureFields: function (coordinator) {
            var _this = this;           
            var contactId = coordinator && coordinator.ContactId;
            _this.setSignatureDate(contactId);
            if (coordinator == null || !coordinator.IsHaveSignature) {
                $(".auto-signature").hide();
                return;
            }
            
            $(".auto-signature").show();
            $("#IsAutoSignature").off("click").on("click",
                function () {
                    if ($("#IsAutoSignature").is(":checked")) {
                        _this.setSignaturePasswordConfirmation();
                    } else {
                        _this.resetSignatureConfirmation();
                    }
                    _this.setSignatureDate(contactId);
                });
        },
        resetSignatureConfirmation: function () {
            $("#SignaturePassword-error").hide();
            $("#signature-password").hide();
            $("#IsAutoSignature").prop('checked', false);
            $("#SignaturePassword-confirmed").hide();
            $("#SignaturePassword").val("");
        },

        setSignaturePasswordConfirmation: function () {
            var _this = this;
            var contactId = $("#CoordinatorId").val();
            if (this.isConfirmedSignature(contactId)) {
                //Just show that password was confirmed               
                this.showConfirmedSignatureMessage();
                return;
            }
            this.showInline("#signature-password");
            $("#SignaturePassword-error").hide();
            $("[data-action='confirmPassword']").off("click").on("click",
                function () {
                    var password = $("#SignaturePassword").val();
                    var s = function (isOk) {
                        if (!isOk) {
                            $("#SignaturePassword-error").show();
                        } else {
                            $("#SignaturePassword-error").hide();
                            $("#signature-password").hide();
                            _this.addConfirmationSignature(contactId);
                            _this.showConfirmedSignatureMessage();
                        }
                        _this.setSignatureDate(contactId);
                    };
                    var e = function () { $("#SignaturePassword-error").show(); };
                    _this.model.checkPassword(password, contactId, s, e);
                });
        },
        showInline: function (id) {
            $(id).show();
            $(id).css("display", "inline-block");
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
            var isEdit = this.model.get("ConsumerHabPlanId") != null;
            return isEdit;
        },
        onSaveAsNew: function () {
            var _this = this;
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);
                obj.HabServiceName = "";
                if (!$.isEmpty($("#HabServiceId").val())) {
                    obj.HabServiceName = $.trim($("option:selected", "#HabServiceId").html());
                }
                obj.Coordinator = null;
                var coordinatorId = $("#CoordinatorId").val();
                if (!$.isEmpty(coordinatorId)) {
                    obj.Coordinator = _this.employeesData[coordinatorId];
                }
                obj.StatusName = "";
                if (!$.isEmpty($("#StatusId").val())) {
                    obj.StatusName = $.trim($("option:selected", "#StatusId").html());
                }
                obj.IsAproved = (obj.StatusName == "Approved");
                obj.ConsumerId = this.parentView.model.get("ConsumerId");


                if (obj.IsAutoSignature && !this.isConfirmedSignature(coordinatorId)) {
                    obj.IsAutoSignature = false;
                }

                obj.AddedById = CurrentUserInfo.Id;
                obj.DateCreated = Utils.getCurrentDate();
                obj.AddedByName = CurrentUserInfo.Name;
                obj.UpdatedById = null;
                obj.DateUpdated = "";
                obj.UpdatedByName = "";

                obj.ValuedOutcomes = this.serializedValuedOutcomes();
                obj.Safeguards = this.serializedSafeguards();
                var modelNew = new BaseModel();
                modelNew.parseObj(obj);
                var returnObj = modelNew.getObject();

                var isUpdate = false;
                if ($.isEmpty(obj.ConsumerId)) {
                    this.parentView.onPopupSave(returnObj);
                } else {
                    var success = function (responce) {
                        returnObj["ConsumerHabPlanId"] = responce.id;
                        returnObj["isUpdate"] = isUpdate;
                        _this.parentView.onPopupSave(returnObj);
                    };
                    modelNew.postData(success);
                }
            }
        },
        onSave: function () {

            var _this = this;
            if (this.model.isValid()) {

                var obj = Utils.serializeForm(this.model.formId);
                obj.HabServiceName = "";
                if (!$.isEmpty($("#HabServiceId").val())) {
                    obj.HabServiceName = $.trim($("option:selected", "#HabServiceId").html());
                }
                obj.Coordinator = null;
                var coordinatorId = $("#CoordinatorId").val();
                if (!$.isEmpty(coordinatorId)) {
                    obj.Coordinator = _this.employeesData[coordinatorId];
                }
                obj.StatusName = "";
                if (!$.isEmpty($("#StatusId").val())) {
                    obj.StatusName = $.trim($("option:selected", "#StatusId").html());
                }
                obj.IsAproved = (obj.StatusName == "Approved");
                obj.ConsumerId = this.parentView.model.get("ConsumerId");

                if (obj.IsAutoSignature && !this.isConfirmedSignature(coordinatorId)) {
                    obj.IsAutoSignature = false;
                }

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
                obj.ValuedOutcomes = this.serializedValuedOutcomes();
                obj.Safeguards = this.serializedSafeguards();
                this.model.parseObj(obj);
                var returnObj = this.model.getObject();

                var isUpdate = this.isEdit();
                if ($.isEmpty(obj.ConsumerId)) {
                    this.parentView.onPopupSave(returnObj);
                } else {
                    var success = function (responce) {
                        returnObj["ConsumerHabPlanId"] = responce.id;
                        returnObj["isUpdate"] = isUpdate;
                        _this.parentView.onPopupSave(returnObj);
                    };
                    this.model.postData(success);
                }
            }
        },
        /*----------VALUED OUTCOME functionality-----------------------------*/
        setVauedOutcome: function () {

            var valuesOutcomes = this.model.get("ValuedOutcomes");

            if ($.isEmptyObject(valuesOutcomes)) {
                //Set empty values             
                valuesOutcomes = [];
                this.model.set("ValuedOutcomes", valuesOutcomes);
            } else {
                $("[data-container=valued-outcomes]").css("height", "400px");
            }
            var html = "";
            for (var i = 0; i < valuesOutcomes.length; i++) {

                var valuesOutcome = valuesOutcomes[i];
                var serveActions = valuesOutcome["ServeActions"];
                var parentId = valuesOutcome["Id"];
                var actionHTML = "";
                for (var j = 0; j < serveActions.length; j++) {
                    var serveAction = serveActions[j];
                    var templateHtml = $("#template-hab-plan-action").html();
                    var template = _.template(templateHtml);
                    actionHTML += template({ num: j + 1, parentId: parentId, model: serveAction });
                }
                var templateHtml = $("#template-hab-plan-valued-outcome").html();
                var template = _.template(templateHtml);
                var valuesOutcomeHTML = template({ num: i + 1, model: valuesOutcome, htmlActions: actionHTML });
                html += valuesOutcomeHTML;
            }
            $("[data-container=valued-outcomes]").html(html);
            var _this = this;
            $("input[type=text]", "[data-container=valued-outcomes]").each(function () {
                _this.model.setValidationRulesForValued($(this));
            });
        },
        onRemoveValuedOutcome: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            $("[data-container-valued-id='" + id + "']").remove();

            var values = this.model.get("ValuedOutcomes");
            values = _.without(values, _.findWhere(values, { Id: id }));
            if (values.length == 0) {
                $("[data-container=valued-outcomes]").css("height", "auto");
            }
            this.model.set("ValuedOutcomes", values);
        },
        onAddValuedOutcome: function (event) {

            var values = this.model.get("ValuedOutcomes");

            var count = 1;
            if (!$.isEmptyObject(values)) count = values.length + 1;
            else {

                $("[data-container=valued-outcomes]").css("height", "400px");
                values = [];
            }

            var valuesOutcome = { Id: count + "_new", ValuedOutcome: "", ServeActions: [] };
            var templateHtml = $("#template-hab-plan-valued-outcome").html();
            var template = _.template(templateHtml);
            var valuesOutcomeHTML = template({ num: count, model: valuesOutcome, htmlActions: "" });

            $("[data-container=valued-outcomes]").append(valuesOutcomeHTML);
            Utils.scrollToInside($("[data-container=valued-outcomes]"), $("[data-container-valued-id='" + valuesOutcome.Id + "']"));
            values.push(valuesOutcome);
            this.model.set("ValuedOutcomes", values);

            var _this = this;
            $("input[type=text]", "[data-container-valued-id='" + valuesOutcome.Id + "']").each(function () {
                _this.model.setValidationRulesForValued($(this));
            });

        },
        onAddNewAction: function (event) {

            var parentId = $(event.currentTarget).attr("data-vo-id");
            var values = this.model.get("ValuedOutcomes");
            var insertedId = null;
            for (var i = 0; i < values.length; i++) {
                if (values[i]["Id"] + "" == parentId) {
                    var serveActions = values[i]["ServeActions"];

                    var count = 1;
                    if (!$.isEmptyObject(serveActions)) count = serveActions.length + 1;
                    else serveActions = [];

                    var action = { Id: count + "_new", ServeAndAction: "" };
                    insertedId = action.Id;
                    var templateHtml = $("#template-hab-plan-action").html();
                    var template = _.template(templateHtml);
                    var actionHtml = template({ num: count, model: action, parentId: parentId });
                    $("[data-container=actions]", "[data-container-valued-id='" + parentId + "']").append(actionHtml);
                    serveActions.push(action);
                    values[i]["ServeActions"] = serveActions;
                }
            }
            this.model.set("ValuedOutcomes", values);
            var container = $("[data-container-action-id='" + insertedId + "']", "[data-container-valued-id='" + parentId + "']")
            this.model.setValidationRulesForValued($("input[type=text]", container));
        },
        OnRemoveAction: function (event) {
            var parentId = $(event.currentTarget).attr("data-vo-id");
            var id = $(event.currentTarget).attr("data-id");
            var values = this.model.get("ValuedOutcomes");
            for (var i = 0; i < values.length; i++) {
                if (values[i]["Id"] + "" == parentId) {
                    var serveActions = values[i]["ServeActions"];
                    serveActions = _.without(serveActions, _.findWhere(serveActions, { Id: id }));
                    values[i]["ServeActions"] = serveActions;
                    $("[data-container-action-id='" + id + "']", "[data-container-valued-id='" + parentId + "']").remove();
                }
            }
            this.model.set("ValuedOutcomes", values);
        },
        getIdsByPrefix: function (obj, startWith) {
            var ids = Object.keys(obj).filter(function (x) { return x.startsWith(startWith)})
                .map(function (x) { return x.replace(startWith, "") });
            return ids;
        },
        serializedValuedOutcomes: function () {
            var _this = this;
            var obj = Utils.serializeForm("#hab-plan-valued-form");
            var valudeOutcomes = [];
            var valudeOutcomesIds = _this.getIdsByPrefix(obj, "valued-outcome-");
            valudeOutcomesIds.forEach(function (voId) {
                var id = voId;

                var newModel = { Id: id, ServeActions: [] }
                newModel.ValuedOutcome = obj["valued-outcome-" + id];
                newModel.IsIPOP = obj["is-ipop-" + id];
                newModel.CQLPOM = obj["cqlpom-goal-" + id];
                newModel.MyGoal = obj["mygoal-" + id];
                var actionIds = _this.getIdsByPrefix(obj, "vo-" + id + "-action-");               
                actionIds.forEach(function (actionId) {
                    var actionModel = { Id: actionId, ServeAndAction: obj["vo-" + id + "-action-" + actionId] };
                    newModel.ServeActions.push(actionModel);
                });
                valudeOutcomes.push(newModel);
            });
            return valudeOutcomes;
        },
        /*----------Safeguard functionality-----------------------------*/
        onAddSafeguard: function (event) {

            var values = this.model.get("Safeguards");

            var count = 1;
            if (!$.isEmptyObject(values)) count = values.length + 1;
            else {

                $("[data-container=safeguards]").css("height", "400px");
                values = [];
            }
            var safeguard = { Id: count + "_new", Item: "" };
            var templateHtml = $("#template-hab-plan-safeguard").html();
            var template = _.template(templateHtml);
            var valuesOutcomeHTML = template({ num: count, model: safeguard, htmlActions: "" });

            $("[data-container=safeguards]").append(valuesOutcomeHTML);
            Utils.scrollToInside($("[data-container=safeguards]"), $("[data-container-safeguard-id='" + safeguard.Id + "']"));
            values.push(safeguard);
            this.model.set("Safeguards", values);

            var _this = this;
            $("input", "[data-container-safeguard-id='" + safeguard.Id + "']").each(function () {
                _this.model.setValidationRulesForSafeguard($(this));
            });

        },
        onRemoveSafeguard: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            $("[data-container-safeguard-id='" + id + "']").remove();

            var values = this.model.get("Safeguards");
            values = _.without(values, _.findWhere(values, { Id: id }));
            if (values.length == 0) {
                $("[data-container=safeguards]").css("height", "auto");
            }
            this.model.set("Safeguards", values);
        },
        setSafeguards: function () {

            var safeguards = this.model.get("Safeguards");

            if ($.isEmptyObject(safeguards)) {
                //Set empty values             
                safeguards = [];
                this.model.set("Safeguards", safeguards);
            } else {
                $("[data-container=safeguards]").css("height", "400px");
            }
            var html = "";
            for (var i = 0; i < safeguards.length; i++) {

                var safeguard = safeguards[i];
                var templateHtml = $("#template-hab-plan-safeguard").html();
                var template = _.template(templateHtml);
                var safeguardHTML = template({ num: i + 1, model: safeguard });
                html += safeguardHTML;
            }
            $("[data-container=safeguards]").html(html);
            var _this = this;
            $("input", "[data-container=safeguards]").each(function () {
                _this.model.setValidationRulesForSafeguard($(this));
            });
        },

        serializedSafeguards: function () {
            var _this = this;
            var obj = Utils.serializeForm("#hab-plan-safeguard-form");
            var safeguards = [];
            var safeguardsIds = _this.getIdsByPrefix(obj, "safeguard-item-");
            safeguardsIds.forEach(function (sgId) {
                var id = sgId;

                var newModel = { Id: id }
                newModel.Item = obj["safeguard-item-" + id];
                newModel.Action = obj["safeguard-action-" + id];
                newModel.IsIPOP = obj["safeguard-is-ipop-" + id];
                safeguards.push(newModel);
            });
           
            return safeguards;
        },

    });/*.end module*/

});/*.end defined*/