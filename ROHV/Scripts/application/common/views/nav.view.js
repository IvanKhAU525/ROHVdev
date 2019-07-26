define(function (require) {


    require('marionette');
    require('underscore');

    return Backbone.Marionette.ItemView.extend({

        /**
         * @property    {String}    nav     this will be a nav element
         */
        el: '.slimScrollDiv',

        template: false,

        /**
         * 
         * @returns {Object}
         */
        events: function () {
            return {
                'click a': 'onAnchorClick'
            }
        },

        /**
         * 
         * @param   {Event} event
         * @returns {Boolean}
         */      

        onAnchorClick: function (event) {
        
            // May not be compatible with older IE versions           
            var href = $(event.currentTarget).attr("href");
            if (href.indexOf("logoff") != -1) return true;
            if (href.indexOf("javascript") != -1) return true;
            if ($.isEmpty(href)) return true;

            $('.dropdown.open .dropdown-toggle').dropdown('toggle');

            var AppInstance = require('app/app.instance');
            AppInstance.navRouter.navigate(href, { trigger: true });

            // returning false cancels the event
            return false;
        }

    });

});