define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.rate.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="rate-view"></div>',
        el: "#rate-view",
        events: function () {
            return {
                "click #save-rate-hours": "onSave"
            }
        },
        initialize: function (options) {

        },
        template: function (serialized_model) {
            var templateHtml = $("#rate-tab-template").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            this.localModel = new BaseModel();
            if (this.model == null) {
                this.model = new BaseModel();
            }
            var fullObj = this.model.getObject();
            var result = { "Rate": null, "MaxHoursPerWeek": null, "MaxHoursPerYear": null, "RateNote": null };
            var listNames = ["Rate", "MaxHoursPerWeek", "MaxHoursPerYear", "RateNote"];
            _.filter(fullObj,
                function (element, key) {
                    if ($.inArray(key, listNames) != -1) {
                        result[key] = element;
                    }
                    return $.inArray(key, listNames) != -1;
                });

            this.localModel.set(result);
            return this.model;
        },
        appendContainer: function () {
            $("#rate-hours-tab").append(this.container);
        },

        onBeforeRender: function () {
        },

        onDomRefresh: function () {
            if (this.model.get("ConsumerId") != null) {
                $("#save-rate-hours-container").show();
            } else {
                $("#save-rate-hours-container").hide();
            }
            this.localModel.setFormValidation();
        },

        onRender: function () {
            this.onRenderBase();
        },
        setModel: function (model) {
            this.model = model;
            this.render();
        },
        onSave: function (isValidPrevious) {

            var isValid = false;
            var _this = this;
            if (this.localModel.isValid()) {
                var obj = Utils.serializeForm(this.localModel.formId);
                obj["ConsumerId"] = this.model.get("ConsumerId");
                this.localModel.parseObj(obj);
                obj = this.localModel.getObject();

                if ($.isEmpty(obj.ConsumerId)) {
                    this.model.set(obj);
                } else {
                    var success = function () {
                        Utils.notify("The rate has been successfully updated.");
                        _this.model.set(obj);
                    };
                    this.localModel.postData(success);
                }
                isValid = true;
            }
            else {
                if (isValidPrevious) {
                    $('.nav-tabs a[href="#rate-hours-tab"]').tab('show');
                    Utils.scrollTo($('.nav-tabs a[href="#rate-hours-tab"]'));
                }
            }
            if (typeof (isValidPrevious) === "boolean") {
                return isValid;
            }
            return false;
        }
    });

});