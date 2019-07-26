define(function(require) {
    require('backbone');
    require('underscore');

    var baseModel = require('models/base.model');

    return baseModel.extend({
        initialize: function() {},

        parseObj: function (obj) {
            const objToSet = {
                types: obj.types
            };

            this.set(objToSet);
        },
    });
});