define(function (require) {

    require('marionette');
    require('underscore');

    return Backbone.Marionette.ItemView.extend({

        container: '<div id="modal-message-dialog" class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#modal-message-dialog",
        events: function () {
            return {
            }
        },
        appendContainer: function () {
            $("#modal-confirm-message-container").html(this.container);
        },
        initialize: function () {
        },

        template: function (serialized_model) {
            var templateHtml = $("#message-template").html();
            var template = _.template(templateHtml);            
            return template(serialized_model);
        },
        serializeData: function () {
            return this.model;
        },
        onBeforeRender: function () { },
        onDomRefresh: function () { },

        onRender: function () {
        },
        onDestroy: function () {
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
        }

    });

});