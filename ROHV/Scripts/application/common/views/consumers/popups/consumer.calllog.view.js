define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.calllog.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="call-log-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#call-log-view",
        employeesData: [],
        events: function () {
            return {
                "click #save-modal": "onSave"
            }
        },
        initialize: function (options) {
            this.parentView = options.parentView;
        },

        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        template: function (serialized_model) {
            var templateHtml = $("#pop-template-calllog").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            var tmp = this.model;
            this.model = new BaseModel();
            if (tmp != null) {
                this.model.set(tmp);
            }
            this.setDefaultData();
            return this.model;
        },
        onBeforeRender: function () {
        },

        onDomRefresh: function () {

        },
        onRender: function () {

            $('#CalledOn').datepicker({ autoclose: true });
            $('#CalledOn').inputmask();


            this.setEmployeeField();

            this.model.setFormValidation();
            this.onRenderBase();
        },
        setDefaultData: function () {
            var defaultContact = ContactInfo;
            if (defaultContact && this.model.get("ContactId") == null && !!defaultContact.ContactId) {
                this.model.set("ContactId", defaultContact.ContactId);
                this.model.set("ContactName", defaultContact.ContactName);                
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
            var isEdit = this.model.get("ConsumerContactCallId") != null;
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
                        returnObj["ConsumerContactCallId"] = responce.id;
                        returnObj["isUpdate"] = isUpdate;
                        _this.parentView.onPopupSave(returnObj);
                    };
                    this.model.postData(success);
                }



            }
        }
    });/*.end module*/

});/*.end defined*/