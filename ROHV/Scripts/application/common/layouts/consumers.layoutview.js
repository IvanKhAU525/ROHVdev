define(function (require) {

    require('marionette');
    require('underscore');
    var SearchingView = require('views/consumers/consumers.searching.view'),
         ConsumerInfoView = require('views/consumers/consumers.consumer.info.view'),
         AdditionalInfoView = require('layouts/consumers.additional.info.layoutview'),
         LoadingView = require('views/loading.view'),
         ConsumerModel = require('models/consumer.model'),
         BaseModel = require('models/base.model');


    return Marionette.LayoutView.extend({

        searchingView: null,
        consumerInfoView: null,
        additionalInfoView: null,
        regions: function (options) {
            return {
                searching: '#searchingContainer',
                consumerInfo: '#consumerInfoContainer',
                additionalInfo: '#additionalInfoContainer',
                loadingContainer: "#loading-container"

            };
        },
        childEvents: {
            'update:full-refresh-data': 'onUpdateData',
            'save:consumer': 'onSave',
            'delete:consumer': 'onDelete',
            'update:notes': 'onUpdateNotes'
        },
        initialize: function () {

        },
        template: function (serialized_model) {
            var template = Marionette.TemplateCache.get("consumers");
            return template;
        },
        setRegions: function () {
            var _this = this;
            this.getRegion("loadingContainer").show(new LoadingView());

            setTimeout(function () {

                if (_this.model == null) {
                    _this.model = new ConsumerModel();
                }
                SearchingView.prototype.appendContainer();
                _this.searchingView = new SearchingView({ model: _this.model });
                _this.getRegion("searching").show(_this.searchingView);

                ConsumerInfoView.prototype.appendContainer();
                _this.consumerInfoView = new ConsumerInfoView({ model: _this.model });
                _this.getRegion("consumerInfo").show(_this.consumerInfoView);

                AdditionalInfoView.prototype.appendContainer();
                _this.additionalInfoView = new AdditionalInfoView({ model: _this.model });
                _this.getRegion("additionalInfo").show(_this.additionalInfoView);
                _this.additionalInfoView.setRegions();


                _this.getRegion("loadingContainer").reset();
                $("#main-container").show();
            }, 10);

        },
        onUpdateNotes: function () {
            this.additionalInfoView.onUpdateNotes();
        },
        onUpdateData: function () {
            var _this = this;
            var consumerId = this.searchingView.getSelectedConsumerId();
            if (consumerId == null || consumerId.length == 0) {

                _this._model = new ConsumerModel();
                _this.consumerInfoView.setModel(_this._model);
                _this.additionalInfoView.setModel(_this._model);

            } else {
                this.loadConsumerById(consumerId);
               
            }
        },
        loadConsumerById: function (consumerId) {
            if (consumerId) {
                var _this = this;
                //Need to update all information from data
                _this._model = new ConsumerModel();
                var success = function () {
                    _this.consumerInfoView.setModel(_this._model);
                    _this.additionalInfoView.setModel(_this._model);
                };
                _this._model.getModel(consumerId, success);
            }
        },
        onSave: function () {
            var isValid = true;
            if (!this.consumerInfoView.onSave(isValid)) {
                isValid = false;
            }
            if (!this.additionalInfoView.onSave(isValid)) {
                isValid = false;
            }
        },
        onDelete: function ()
        {            
            this.searchingView.onReset();            
        }
    });

});