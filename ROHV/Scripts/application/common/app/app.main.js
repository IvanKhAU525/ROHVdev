define(function (require) {
    require('marionette');
    require('templates/template.config');
    var RootView = require('app/app.rootlayout'),

            NavRouter = require('routers/nav.router');
    NavController = require('controllers/nav.controller'),
    NavView = require('views/nav.view'),

    ErrorView = require('views/error.view'),
    NotFoundView = require('views/notfound.view'),
    LoadingView = require('views/loading.view'),
    EventsController = require('controllers/events.controller');

    return Backbone.Marionette.Application.extend({

        initialize: function (options) {

        },

        showView: function (view) {
            this.rootView.getRegion("regionMain").show(view);
        },
        showErrorView: function () {
            this.rootView.getRegion("regionMain").show(new ErrorView());
        },
        showNotFoundView: function () {
            this.rootView.getRegion("regionMain").show(new NotFoundView());
        },
        showLoadingView: function () {
            this.rootView.getRegion("regionMain").show(new LoadingView());
        },
        start: function (options) {

            // Perform the default 'start' functionality
            Backbone.Marionette.Application.prototype.start.apply(this, [options]);

            //Set main root
            this.rootView = new RootView();
            this.events = new EventsController();
            // Add navigation router            

            this.navRouter = new NavRouter({ controller: new NavController() });

            this.rootView.getRegion("regionNav").attachView(new NavView());

            Backbone.history.start({ pushState: true });

            if (location.href.indexOf("home/") != -1) {
                this.navRouter.navigate('consumers', { trigger: true });
            }
        }
    });

});