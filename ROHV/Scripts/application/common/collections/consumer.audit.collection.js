define(function (require) {

    require('backbone');
    require('underscore');
  

    return Backbone.Collection.extend({
        url: "/api/consumerauditapi/getaudits"        
    });

});