require.config({
    urlArgs: "t=v2",
    baseUrl: "/scripts/application/common/",
    paths: {
        'jquery': '/scripts/libs/jquery/dist/jquery',
        'jquery.validate': '/scripts/libs/jquery-validation/dist/jquery.validate.min',
        'jquery.validate.ext': '/scripts/libs/jquery-validation/dist/additional-methods.min',

        'slim.scroll': '/scripts/libs/slimScroll/jquery.slimscroll.min',
        'app.admin': '/scripts/application/app',
        'fast.click': '/scripts/libs/fastclick/lib/fastclick.min',

        'bootstrap': '/scripts/libs/bootstrap/dist/js/bootstrap.min',
        'backbone': '/scripts/libs/backbone/backbone-min',
        'marionette': '/scripts/libs/backbone.marionette/lib/backbone.marionette.min',
        'backbone.wreqr': '/scripts/libs/backbone.wreqr/lib/backbone.wreqr.min',
        'backbone.babysitter': '/scripts/libs/backbone.babysitter/lib/backbone.babysitter.min',
        'underscore': '/scripts/libs/underscore/underscore-min',
        'dropzone': '/scripts/libs/dropzone/dist/dropzone-amd-module',

        'datatables.net': '/scripts/libs/datatables.net/js/jquery.dataTables',
        'datatables.net-bs': '/scripts/libs/datatables.net-bs/js/dataTables.bootstrap.min',
        'datatables.net-responsive': '/scripts/libs/datatables.net-responsive/js/dataTables.responsive.min',
        'datatables.net-responsive-bs': '/scripts/libs/datatables.net-responsive-bs/js/responsive.bootstrap.min',
        'datatables.net-select': '/scripts/libs/datatables.net-select/js/dataTables.select.min',
        'bootstrap-select': '/scripts/libs/bootstrap-select/dist/js/bootstrap-select',
        'bootstrap-switch': '/scripts/libs/bootstrap-switch/dist/js/bootstrap-switch.min',

        'remarkable-bootstrap-notify': '/scripts/libs/remarkable-bootstrap-notify/dist/bootstrap-notify.min',
        'ajax-bootstrap-select': '/scripts/libs/ajax-bootstrap-select/dist/js/ajax-bootstrap-select',
        'bootstrap-datepicker': '/scripts/libs/bootstrap-datepicker/dist/js/bootstrap-datepicker',
        'input-mask': '/scripts/libs/jquery.inputmask/dist/min/jquery.inputmask.bundle.min',
        'dateformat': '/scripts/libs/date-steroids/dist/date'
    },
    shim: {
        'jquery': {
            exports: '$'
        },
        'bootstrap': {
            deps: ['jquery']
        },

        'underscore': {
            exports: '_'
        },
        'backbone': {
            deps: ['jquery', 'underscore'],
            exports: 'Backbone'
        },
        'marionette': {
            deps: ['jquery', 'backbone', 'backbone.wreqr', 'backbone.babysitter'],
            exports: 'Marionette'
        },
        'fast.click': {
            deps: ['jquery']
        },
        'jquery.validate': {
            deps: ['jquery']
        },
        'slim.scroll': {
            deps: ['jquery']
        },
        'app.admin': {
            deps: ['jquery', 'bootstrap', 'slim.scroll', 'fast.click']
        },
        'jquery.validate.ext': {
            deps: ['jquery', 'jquery.validate']
        },
        'datatables.net-bs': {
            deps: ['jquery', 'bootstrap']
        },
        'datatables.net-responsive-bs': {
            deps: ['jquery', 'bootstrap']
        },
        'datatables.net-select': {
            deps: ['jquery', 'bootstrap']
        },
        'remarkable-bootstrap-notify':
        {
            deps: ['jquery', 'bootstrap']
        },
        'bootstrap-select':
        {
            deps: ['bootstrap']
        },
        'bootstrap-switch':
        {
            deps: ['bootstrap']
        },
        'ajax-bootstrap-select':
        {
            deps: ['bootstrap-select']
        },
        'bootstrap-datepicker':
        {
            deps: ['bootstrap']
        },
        'input-mask':
        {
            deps: ['jquery']
        },
        'dateformat':
        {
            deps: ['jquery']
        }
    }
});

require(['app/app.instance', 'app.admin', 'jquery', 'marionette', 'jquery.validate.ext', 'datatables.net-bs',
        'datatables.net-responsive-bs', 'datatables.net-select', 'remarkable-bootstrap-notify', 'Utils/GlobalEvents',
        'Utils/Utils', 'bootstrap-select', 'ajax-bootstrap-select', 'bootstrap-datepicker', 'input-mask', 'dateformat'],

        function (app) {
            app.start();
        }

);