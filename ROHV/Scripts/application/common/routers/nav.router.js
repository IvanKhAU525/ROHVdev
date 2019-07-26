define(function (require) {

    require('marionette');

    return Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            "": "consumers",
            "consumers": "consumers",
            "consumers/*action": "consumers",            
            "employees": "employees",
            "employees/*action": "employees",
            "advocates": "advocates",
            "advocates/*action": "advocates",
            "system-users": "systemusers",
            "system-users/*action": "systemusers",
            "consumer-audit": "consumeraudit",
            "consumer-audit/*action": "consumeraudit",
            '*notFound': 'notFound'
        },
        onRoute: function (name, path, arguments) {

        }
    });
});