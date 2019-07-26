define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.printdocument.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="print-document-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#print-document-view",
        employeesData: [],
        events: function () {
            return {
                "click #save-modal": "onSave",
                "click [data-action=remove-valued-outcome]": "onRemoveValuedOutcome",
                "click [data-action=add-valued-outcome]": "onAddValuedOutcome",
                "click [data-action=add-new-action]": "onAddNewAction",
                "click [data-action=remove-action]": "OnRemoveAction"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        template: function (serialized_model) {

            var templateHtml = $("#pop-template-printdocument").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model.model, hanPlans: serialized_model.hanPlans });
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
            }
            return { model: this.model, hanPlans: this.parentView.model.get("HabPlans") };
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {

            $('#EffectiveDate').datepicker({ autoclose: true });
            $('#EffectiveDate').inputmask();

            this.setEmployeeField();
            this.setField("#ServiceTypeId");
            this.setField("#CopyFromHabPlanId");
            this.setEventForCopyData();
            this.model.setFormValidation();
            this.setVisibilityField($("#ServiceTypeId").val());

            this.setVauedOutcome(this.model.get("ValuedOutcomes"));
            this.onRenderBase();
        },
        setEventForCopyData: function () {
            var _this = this;
            $("#CopyFromHabPlanId").on('changed.bs.select', function (e) {

                //Get all valued outcomes
                var selected = $(this).val();
                var value = _.find(_this.parentView.model.get("HabPlans"), { ConsumerHabPlanId: parseInt(selected) });
                if (!$.isEmptyObject(value)) {
                    _this.setVauedOutcome(value["ValuedOutcomes"]);
                }
            });
        },
        setField: function (id) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false
            });
            $(id).off('show.bs.select');
            $(id).on('show.bs.select', function (e) {
                _this.model.resetField($(id));
            });
            $(id).off('changed.bs.select');
            $(id).on('changed.bs.select', function (e) {

                _this.setVisibilityField($(this).val());
            });

        },
        setVisibilityField: function (serviceTypeId) {
            switch (serviceTypeId) {
                case "1":
                case "2":
                    {
                        $(".respite-hide").show();
                        break;
                    }
                case "3":
                    {
                        $(".respite-hide").hide();
                        break;
                    }

            }
        },
        setEmployeeField: function () {
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
                    emptyTitle: 'Select worker name'
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
            var isEdit = this.model.get("ConsumerPrintDocumentId") != null;
            return isEdit;
        },
        onSave: function () {
            var _this = this;
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);                
                obj.ContactName = "";
                if ($("#ContactId").val().length > 0) {
                    obj.ContactName = $.trim($("option:selected", "#ContactId").html());
                }
                obj.ServiceTypeName = "";
                if ($("#ServiceTypeId").val().length > 0) {
                    var selectedService = $("option:selected", "#ServiceTypeId");
                    obj.ServiceTypeName = $.trim(selectedService.html());
                    var serviceTitle = selectedService.attr("title");
                    if (!serviceTitle) serviceTitle = null;
                    else serviceTitle = $.trim(serviceTitle);
                    obj.ServiceTypeTitle = serviceTitle;
                }
                obj.ConsumerId = this.parentView.model.get("ConsumerId");                
                var isUpdate = this.isEdit();
                if (this.isEdit()) {
                    obj.UpdatedById = CurrentUserInfo.Id;
                    obj.DateUpdated = Utils.getCurrentDate();
                    obj.UpdatedByName = CurrentUserInfo.Name;
                    obj.AddedById = this.model.get("AddedById");
                    obj.DateCreated = this.model.get("DateCreated");
                    obj.AddedByName = this.model.get("AddedByName");
                    obj.StatusName = this.model.get("StatusName");

                } else {
                    obj.AddedById = CurrentUserInfo.Id;
                    obj.DateCreated = Utils.getCurrentDate();
                    obj.AddedByName = CurrentUserInfo.Name;
                    obj.UpdatedById = null;
                    obj.DateUpdated = "";
                    obj.UpdatedByName = "";
                    obj.StatusName = "Ready";
                }
                obj.ValuedOutcomes = this.serializedValuedOutcomes();

                this.model.parseObj(obj);
                var returnObj = this.model.getObject();
                if ($.isEmpty(obj.ConsumerId)) {
                    this.parentView.onPopupSave(returnObj);
                } else {
                    var success = function (responce) {
                        returnObj["ConsumerPrintDocumentId"] = responce.id;
                        returnObj["isUpdate"] = isUpdate;
                        _this.parentView.onPopupSave(returnObj);
                    };
                    this.model.postData(success);
                }

            }
        },
        /*----------VALUED OUTCOME functionality-----------------------------*/
        setVauedOutcome: function (valuedOutcomesData) {

            var valuesOutcomes = valuedOutcomesData;

            $("[data-container=valued-outcomes]").html("");
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
                    var templateHtml = $("#template-print-document-action").html();
                    var template = _.template(templateHtml);
                    actionHTML += template({ num: j + 1, parentId: parentId, model: serveAction });
                }
                var templateHtml = $("#template-print-document-valued-outcome").html();
                var template = _.template(templateHtml);
                var valuesOutcomeHTML = template({ num: i + 1, model: valuesOutcome, htmlActions: actionHTML });
                html += valuesOutcomeHTML;
            }
            $("[data-container=valued-outcomes]").html(html);
            var _this = this;
            $("input", "[data-container=valued-outcomes]").each(function () {
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
            var templateHtml = $("#template-print-document-valued-outcome").html();
            var template = _.template(templateHtml);
            var valuesOutcomeHTML = template({ num: count, model: valuesOutcome, htmlActions: "" });

            $("[data-container=valued-outcomes]").append(valuesOutcomeHTML);
            Utils.scrollToInside($("[data-container=valued-outcomes]"), $("[data-container-valued-id='" + valuesOutcome.Id + "']"));
            values.push(valuesOutcome);
            this.model.set("ValuedOutcomes", values);

            var _this = this;
            $("input", "[data-container-valued-id='" + valuesOutcome.Id + "']").each(function () {
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
                    var templateHtml = $("#template-print-document-action").html();
                    var template = _.template(templateHtml);
                    var actionHtml = template({ num: count, model: action, parentId: parentId });
                    $("[data-container=actions]", "[data-container-valued-id='" + parentId + "']").append(actionHtml);
                    serveActions.push(action);
                    values[i]["ServeActions"] = serveActions;
                }
            }
            this.model.set("ValuedOutcomes", values);
            var container = $("[data-container-action-id='" + insertedId + "']", "[data-container-valued-id='" + parentId + "']")
            this.model.setValidationRulesForValued($("input", container));
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
        serializedValuedOutcomes: function () {
            var obj = Utils.serializeForm("#print-document-valued-form");
            var valudeOutcomes = [];
            for (var key in obj) {
                var str = obj[key];
                if (key.indexOf("valued-outcome") != -1) {
                    var id = key.replace("valued-outcome-", "");
                    var findModel = _.find(valudeOutcomes, { Id: id });
                    var ServeActions = [];
                    var model = { Id: id, ValuedOutcome: str, ServeActions: ServeActions };
                    if (findModel != undefined) {
                        ServeActions = findModel["ServeActions"];
                        model["ServeActions"] = ServeActions;
                        _.extend(_.findWhere(valudeOutcomes, { Id: id }), model);
                    } else {
                        valudeOutcomes.push(model);
                    }
                }
                if (key.indexOf("action") != -1) {
                    var parentId = key.substring(3, key.indexOf("-action"));
                    var id = key.replace("vo-" + parentId + "-action-", "");

                    var findModel = _.find(valudeOutcomes, { Id: parentId });
                    var modelToInsert = { Id: id, ServeAndAction: str };
                    if (findModel == undefined) {
                        findModel = { Id: parentId, ValuedOutcome: "", ServeActions: [] };
                        findModel["ServeActions"].push(modelToInsert);
                        valudeOutcomes.push(findModel);
                    } else {
                        findModel["ServeActions"].push(modelToInsert);
                        _.extend(_.findWhere(valudeOutcomes, { Id: parentId }), findModel);
                    }
                }
            }
            return valudeOutcomes;
        }

    });/*.end module*/

});/*.end defined*/