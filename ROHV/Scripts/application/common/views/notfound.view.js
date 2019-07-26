define(function (require) {

    require('marionette');
    require('underscore');

    return Backbone.Marionette.ItemView.extend({     
        initialize : function(options)
        {

        },
        events: function () {
            return {
                'click a': 'onAnchorClick'
            }
        },
        template: function (serialized_model) {
            var template = $("#not-found-template").html();
            return _.template(template);
        }, 

        onBeforeRender: function () {
            //Get data
        },
        onDomRefresh: function () {         
        },

        onRender: function () {
            // manipulate the `el` here. it's already
            // been rendered, and is full of the view's
            // HTML, ready to go.                     
            
        },
        onDestroy :function()
        {            
        },
        onAnchorClick: function (event) {

                // May not be compatible with older IE versions           
            var href = $(event.currentTarget).attr("href");           

            var AppInstance = require('app/app.instance');
            AppInstance.navRouter.navigate(href, { trigger: true });

                // returning false cancels the event
            return false;
        }

    });

});