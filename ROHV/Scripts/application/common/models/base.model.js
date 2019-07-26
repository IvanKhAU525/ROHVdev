define(function (require) {

    require('backbone');
    require('underscore');

    return Backbone.Model.extend({
        getObject: function() {
            this.sanitize();

            return this.attributes;
        },
        delete: function (url, success, error) {
            this.save(null, {
                type: 'DELETE',
                wait: false,
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                url: url,
                success: function (model, response) {
                    if (success != null) {
                        success();
                    }
                }, error: function () {
                    if (error != null) {
                        error();
                    }
                }
            });
        },
        extendJQValidation: function () {
            if (this.getValidationRules) {
                var rules = this.getValidationRules().rules;
                if (rules) {
                    $.each(rules, function (index, value) {
                        if (value.required) {
                            $("label[for='" + index + "']").addClass("required-marker");
                        }
                    });
                }
            }
        },
        request: function (url, objToSend, success, error) {            
            this.formId = null;            
            this._sendData(url, objToSend, success, error); 
        },
        requestWithValidation: function (url, objToSend, success, error) {
            this._sendData(url, objToSend, success, error); 
          
        },
        save: function(attributes, options) {
            this.sanitize();
            Backbone.Model.prototype.save.call(this, attributes, options);
        },
        _sendData: function (url, objToSend, success, error) {
            this.save(null, {
                type: 'POST',
                wait: false,
                data: JSON.stringify(objToSend),
                contentType: 'application/json; charset=utf-8',
                url: url,
                success: function (model, response) {
                    if (success != null) {
                        success(response);
                    }
                }, error: function (model, response) {
                    if (error != null) {
                        error();
                    }
                }
            });
        },
        sanitize: function() {
            _.each(this.attributes, function(val, key) {
                if (typeof val === 'string') {
                    this.set(key, _.escape(val));
                }
            }, this);
        }
    });

});