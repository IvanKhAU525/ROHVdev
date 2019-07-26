define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.note.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="note-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#note-view",
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
            var templateHtml = $("#pop-template-simple-note").html();
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
            this.model.setFormValidation();
            this.onRenderBase();
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
        SaveResultToModel: function () {
            var values = this.parentView.model.get("Notes");
            values.push(this.model.getObject());
            this.parentView.model.set("Notes", values);
            Utils.notify("The note has been successfully added.");

        },
        onSave: function () {
            var _this = this;
            if (this.model.isValid()) {
                var obj = Utils.serializeForm(this.model.formId);
                obj.AditionalInformation = $.isEmpty(this.parentView.model.get("StatusChanges")) ? null : JSON.stringify(this.parentView.model.get("StatusChanges"));    //TODO:NEED TO ADD HERE THE ADDTIONAL INFROMATIOn           
                obj.ConsumerId = this.parentView.model.get("ConsumerId");
                obj.AddedById = CurrentUserInfo.Id;
                obj.DateCreated = Utils.getCurrentDate();
                obj.AddedByName = CurrentUserInfo.Name;
                obj.UpdatedById = null;
                obj.DateUpdated = "";
                obj.Date = Utils.getCurrentDate();
                obj.UpdatedByName = "";
                obj.TypeId = 2;
                obj.TypeName = "Status";
                obj.TypeFromName = "";
                obj.ContactName = "";
                this.model.parseObj(obj);
                var returnObj = this.model.getObject();
                if ($.isEmpty(obj.ConsumerId)) {
                    _this.closeModal();
                } else {
                    var success = function (responce) {
                        _this.model.set("ConsumerNoteId", responce.id);
                        _this.SaveResultToModel();
                        _this.parentView.onPopupSaveNote();
                        _this.closeModal();
                    };
                    this.model.postSimpleData(success);
                }
            }
        }
    });/*.end module*/

});/*.end defined*/