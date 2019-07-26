define(function (require) {

    require('marionette');
    require('underscore');
    var ApprovedTabView = require('views/consumers/consumers.tab.approved.service.view'),
        CallLogsTabView = require('views/consumers/consumers.tab.call.logs.view'),
        DocumentsTabView = require('views/consumers/consumers.tab.documents.view'),
        EmployeesTabView = require('views/consumers/consumers.tab.employees.view'),
        HabPlansTabView = require('views/consumers/consumers.tab.habplans.view'),
        NotesTabView = require('views/consumers/consumers.tab.notes.view'),
        NotificationsTabView = require('views/consumers/consumers.tab.notifications.view'),
        PrintDocumentsTabView = require('views/consumers/consumers.tab.print.documents.view'),
        UploadedFileTabView = require('views/consumers/consumers.tab.uploaded.files.view'),
        RateTabView = require('views/consumers/consumers.tab.rate.view');

    return Marionette.LayoutView.extend({

        container: '<div id="additional-info-layout"></div>',
        el: "#additional-info-layout",
        approvedTabView: null,
        callLogsTabView: null,
        documentsTabView: null,
        employeesTabView: null,
        habPlansTabView: null,
        notesTabView: null,
        notificationsTabView: null,
        printDocumentsTabView: null,
        uploadedFileTabView: null,
        rateTabView: null,

        regions: function (options) {
            return {
                approvedServiceTab: '#approved-service-tab',
                callLogsTab: '#call-logs-tab',
                documentsTab: '#documents-tab',
                employeesTab: '#employees-tab',
                habPlansTab: '#hab-plans-tab',
                notesTab: '#notes-tab',
                notificationsTab: '#notifications-settings-tab',
                printDocumentsTab: '#print-documents-tab',
                uploadedFileTab: '#uploaded-file-tab',
                rateTab: '#rate-hours-tab'
            };
        },
        childEvents: {
            'change:employee-list': 'onChangeEmployeeList'
        },
        appendContainer: function () {
            $("#additionalInfoContainer").append(this.container);
        },
        initialize: function () {

        },
        template: function (serialized_model) {
            var templateHtml = $("#additional-info-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        serializeData: function () {
            return this.model;
        },
        setRegions: function () {

            ApprovedTabView.prototype.appendContainer();
            this.approvedTabView = new ApprovedTabView({ model: this.model });
            this.getRegion("approvedServiceTab").show(this.approvedTabView);

            CallLogsTabView.prototype.appendContainer();
            this.callLogsTabView = new CallLogsTabView({ model: this.model });
            this.getRegion("callLogsTab").show(this.callLogsTabView);

            DocumentsTabView.prototype.appendContainer();
            this.documentsTabView = new DocumentsTabView({ model: this.model });
            this.getRegion("documentsTab").show(this.documentsTabView);

            EmployeesTabView.prototype.appendContainer();
            this.employeesTabView = new EmployeesTabView({ model: this.model });
            this.getRegion("employeesTab").show(this.employeesTabView);

            HabPlansTabView.prototype.appendContainer();
            this.habPlansTabView = new HabPlansTabView({ model: this.model });
            this.getRegion("habPlansTab").show(this.habPlansTabView);

            NotesTabView.prototype.appendContainer();
            this.notesTabView = new NotesTabView({ model: this.model });
            this.getRegion("notesTab").show(this.notesTabView);

            NotificationsTabView.prototype.appendContainer();
            this.notificationsTabView = new NotificationsTabView({ model: this.model });
            this.getRegion("notificationsTab").show(this.notificationsTabView);

            PrintDocumentsTabView.prototype.appendContainer();
            this.printDocumentsTabView = new PrintDocumentsTabView({ model: this.model });
            this.getRegion("printDocumentsTab").show(this.printDocumentsTabView);

            RateTabView.prototype.appendContainer();
            this.rateTabView = new RateTabView({ model: this.model });
            this.getRegion("rateTab").show(this.rateTabView);

            UploadedFileTabView.prototype.appendContainer();
            this.uploadedFileTabView = new UploadedFileTabView({ model: this.model });
            this.getRegion("uploadedFileTab").show(this.uploadedFileTabView);

            this.tabChangesEvent();

        },
        tabChangesEvent: function () {
            var _this = this;
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var tabName = $(e.target).attr("href");
                switch (tabName) {
                    case "#employees-tab":
                        {
                            _this.employeesTabView.updateTable();
                            break;
                        }
                    case "#call-logs-tab":
                        {
                            _this.callLogsTabView.updateTable();
                            break;
                        }
                    case "#notes-tab":
                        {
                            _this.notesTabView.updateTable();
                            break;
                        }
                    case "#hab-plans-tab":
                        {
                            _this.habPlansTabView.updateTable();
                            break;
                        }
                    case "#documents-tab":
                        {
                            _this.documentsTabView.updateTable();
                            break;
                        }
                    case "#notifications-settings-tab":
                        {
                            _this.notificationsTabView.updateTable();
                            break;
                        }
                    case "#print-documents-tab":
                        {
                            _this.printDocumentsTabView.updateTable();
                            break;
                        }
                    case "#uploaded-file-tab":
                        {
                            _this.uploadedFileTabView.updateTable();
                            break;
                        }

                }
            });
        },
        setModel: function (model) {
            this.model = model;
            this.approvedTabView.setModel(model)
            this.printDocumentsTabView.setModel(model);
            this.callLogsTabView.setModel(model);
            this.rateTabView.setModel(model);
            this.employeesTabView.setModel(model);
            this.notesTabView.setModel(model);
            this.habPlansTabView.setModel(model);
            this.documentsTabView.setModel(model);
            this.notificationsTabView.setModel(model);
            this.uploadedFileTabView.setModel(model);
        },

        onSave: function (isValidPrevious) {
            if ($.isEmpty(this.model.get("ConsumerId"))) {
                var isValid = true;
                if (!this.rateTabView.onSave(isValidPrevious)) {
                    isValid = false;
                }
                return isValid;
            }
            return true;
        },
        onChangeEmployeeList: function () {
            this.approvedTabView.refillDirectWorkers();
        },
        onUpdateNotes: function () {
            this.notesTabView.reloadTable();
        }

    });

});