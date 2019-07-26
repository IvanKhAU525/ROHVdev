define(function (require) {

    require('marionette');
    require('underscore');

    return Backbone.Marionette.ItemView.extend({

        container: '<div id="modal-confirm-dialog" class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#modal-confirm-dialog",
        events: function () {
            return {
                'click [data-action="confirm"]': "onConfirm"
            }
        },
        appendContainer: function () {
            $("#modal-confirm-message-container").html(this.container);
        },
        initialize: function () {
        },

        template: function (serialized_model) {
            var templateHtml = $("#confirm-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {            
            if ($.isEmptyObject(this.model)) {
                this.model = {};
                this.model["message"] = "Are you sure you want to delete this item?";
                this.model["title"] = "Delete?";
            }
            return this.model;
        },
        onBeforeRender: function () { },
        onDomRefresh: function () { },

        onRender: function () {
        },
        onDestroy: function () {
        },
        showModal: function (obj) {
            this.id = obj.id;
            this.parentView = obj.parentView;
            this.type = obj.type;
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
        onConfirm: function () {
            this.closeModal();
            var _this = this;
            setTimeout(function () {
                _this.parentView.onConfirm(_this.id, _this.type);
            }, 200);
        }
    });

});