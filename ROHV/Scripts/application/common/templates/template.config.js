define(function (require) {
    require('marionette');

    Marionette.TemplateCache.prototype.loadTemplate = function (templateId, options) {
        
        var template = '',
          url = "/template/"+ templateId;

        // Load the template by fetching the URL content synchronously.
        Backbone.$.ajax({
            async: false,
            url: url,
            success: function (templateHtml) {
                template = templateHtml;
            }
        });

        return template;
    }

    Marionette.TemplateCache.prototype.compileTemplate = function (rawTemplate, options) {
        // use Handlebars.js to compile the template
        return rawTemplate;//Handlebars.compile(rawTemplate);
    }

});