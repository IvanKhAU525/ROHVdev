var GlobalEvents =
{
    setListener: function (name, fnc) {
        var globalChannel = Backbone.Wreqr.radio.channel('global');
        globalChannel.vent.on(name, fnc);
    },
    trigger: function (name, parrams) {
        var globalChannel = Backbone.Wreqr.radio.channel('global');
        globalChannel.vent.trigger(name, parrams);
    },
    setRequestListener: function (name, fnc) {
        var globalChannel = Backbone.Wreqr.radio.channel('global');
        globalChannel.reqres.setHandler(name, fnc);
    },
    triggerRequest: function (name, parrams) {
        var globalChannel = Backbone.Wreqr.radio.channel('global');
        return globalChannel.reqres.request(name, parrams);
    },
    reset: function () {
        var globalChannel = Backbone.Wreqr.radio.channel('global');
        globalChannel.reset();
    }

};

var GlobalData = {
    NoteTypes: {
        Simple: 1,
        Status: 2,
        PopupNote: 3
    }
};