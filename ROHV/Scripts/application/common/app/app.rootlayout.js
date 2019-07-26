define(function (require) {

    require('marionette');

    return Backbone.Marionette.LayoutView.extend({
        el: 'body',
        regions: function (options) {
            return {

                // You can use jquery format to select regions, I'm just using
                // tagnames for simplicity sake
                regionHeader: 'header',
                regionNav: 'aside.main-sidebar',
                regionMain: '.content-wrapper'

            };
        }
    });

});