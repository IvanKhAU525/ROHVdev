define(function (require) {

    require('marionette');

    return Backbone.Marionette.Controller.extend({

        appInstance: null,
        initialize: function () {

            this.appInstance = require('app/app.instance');

        },
        setActiveItemNav: function (href) {
            var target = $("a[href=" + href + "]", ".sidebar-menu");
            $("li", $(target).parent().parent()).removeClass("active");
            $(target).parent().addClass("active");
        },
        consumers: function (id) {
            var ConsumerContentView = require('layouts/consumers.layoutview');
            this.appInstance.showLoadingView();
            this.setActiveItemNav("consumers");
            var _this = this;
            setTimeout(function () {
                var consumerPage = new ConsumerContentView();
                _this.appInstance.showView(consumerPage);
                consumerPage.setRegions();
                consumerPage.loadConsumerById(id);
            }, 100);
        },
        employees: function () {
            var _this = this;
            this.appInstance.showLoadingView();
            this.setActiveItemNav("employees");
            setTimeout(function () {
                var EmployeesView = require('views/employees/employees.view');
                var employeePage = new EmployeesView();
                _this.appInstance.showView(employeePage);                
            }, 100);
        },
        advocates: function () {
            this.appInstance.showLoadingView();
            this.setActiveItemNav("advocates");
            var _this = this;
            setTimeout(function () {
                var AdvocatesView = require('views/advocates/advocates.view');
                var advocatePage = new AdvocatesView();
                _this.appInstance.showView(advocatePage);
            });
        },
        systemusers: function () {
            var _this = this;
            this.appInstance.showLoadingView();
            this.setActiveItemNav("system-users");

            var UsersCollection = require('collections/users.collection');
            var usersCollection = new UsersCollection();
            var SystemUsersView = require('views/users/users.view');
            SystemUsersView.prototype.fetchData(usersCollection,
                function () {
                    //SystemUsersView.prototype.appendContainer();
                    var usersPage = new SystemUsersView({ model: usersCollection });
                    _this.appInstance.showView(usersPage);
                },
                function () {
                    _this.appInstance.showErrorView();
                }
            );
        },
        consumeraudit: function () {
            var _this = this;
            this.appInstance.showLoadingView();
            this.setActiveItemNav("consumer-audit");
        
                var AuditsCollection = require('collections/consumer.audit.collection');
                var audits = new AuditsCollection();
                var ConsumerAuditView = require('views/consumer.audit/consumer.audit.view');

                ConsumerAuditView.prototype.fetchData(audits,
                    function () {
                        var consumerAuditPage = new ConsumerAuditView({ model: audits });
                        _this.appInstance.showView(consumerAuditPage);
                    },
                    function () {
                        _this.appInstance.showErrorView();
                    }
                );
            
        },
        notFound: function () {
            this.appInstance.showLoadingView();
            this.appInstance.showNotFoundView();
        },
        onBeforeDestroy: function (arg1, arg2) {
            // put custom code here, before destroying this controller
        },

        onDestroy: function (arg1, arg2) {
            // put custom code here, to destroy this controller
        }
    });

});