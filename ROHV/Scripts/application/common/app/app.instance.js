
define(function (require) {
    /**
     * Instantiate the application
     */
    
    var App = require('app/app.main');    
    Utils.initValudationPlugin();
    return new App();
});