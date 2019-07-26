define(function (require) {

    require('marionette');
    var MessageDialog = require('views/message.view');

    return Marionette.Object.extend({

        initialize: function () {

            var _this = this;
            GlobalEvents.setListener("showMessage",
             function (obj) {
                 _this.showMessageDialog(obj)
             }

           );
        },
        showMessageDialog: function (obj) {
            MessageDialog.prototype.appendContainer();
            this.dialogMessage = new MessageDialog({ model: obj });
            this.dialogMessage.render();
            this.dialogMessage.showModal();
        }
    });

});