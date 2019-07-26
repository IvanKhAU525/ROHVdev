define(function (require) {

    require('backbone');
    require('underscore');
    var BaseModel = require('models/user.model');

    return Backbone.Collection.extend({

        model: BaseModel,
        url: "/api/usersapi/getusers",
        modelId: function (attrs) {
            return attrs.UserId;
        },
        addModel: function (model) {
            this.add(model);
        },
        updateModel: function (model) {
            this.remove(model);
            this.add(model);
        },
        deleteRequestById: function (id, success, error) {
            var model = this.get(id);
            model.deleteData(success, error);
        },
        deleteById: function (id) {
            var model = this.get(id);
            this.remove(model);
        }

    });

});