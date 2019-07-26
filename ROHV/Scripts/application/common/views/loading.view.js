define(function (require) {

    require('marionette');
    require('underscore');

    return Backbone.Marionette.ItemView.extend({     
        initialize : function(options)
        {

        },
        template: function (serialized_model) {
            var template = $("#loading-template").html();
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
        }

    });

});