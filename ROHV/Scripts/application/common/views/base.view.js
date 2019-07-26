define(function (require) {

    require('marionette');
    require('underscore');

    return Backbone.Marionette.ItemView.extend({
        onRenderBase: function () {
            if (this && this.model && this.model.extendJQValidation){
                this.model.extendJQValidation();
            }
        },
    });/*.end module*/

});/*.end defined*/