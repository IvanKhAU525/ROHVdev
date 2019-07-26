define(function (require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.habplan.model'),
        ModalView = require('views/consumers/popups/consumer.habplan.view'),
        PdfDownloadView = require('views/consumers/popups/consumer.report.pdfdownload.view'),
        DeleteDialog = require('views/confirm.view'),
        HabReviews = require('views/consumers/consumers.tab.habreviews.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="hab-plans-view"></div>',
        el: "#hab-plans-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",
                "click [data-action=generate-pdf]": "onGeneratePdf",

                "click [data-action=add-review]": "onAddReview",
                "click [data-action=edit-review]": "onEditReview",
                "click [data-action=delete-review]": "onDeleteReview",
                "click [data-action=generate-review-pdf]": "onGenerateReviewsPdf"
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#hab-plans-tab-template").html();
            var template = _.template(templateHtml);
            return template(serialized_model);
        },
        initialize: function () {
            this.habReviews = new HabReviews();
        },
        serializeData: function () {       
            if ($.isEmpty(this.model.get("HabPlans"))) {
                this.model.set("HabPlans", []);
            }
            var values = this.model.get("HabPlans");
            _.each(values, function (model, key, list) {
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
                model["DatePlan"] = Utils.getDateText(model["DatePlan"]);
                model["SignatureDate"] = Utils.getDateText(model["SignatureDate"]);
                model["EffectivePlan"] = Utils.getDateText(model["EffectivePlan"]);
                model["EnrolmentDate"] = Utils.getDateText(model["EnrolmentDate"]);
                model["DateUpdated"] = Utils.getDateText(model["DateUpdated"]);
            });
            this.model.set("HabPlans", values);            
            return this.model;
        },
        appendContainer: function () {
            $("#hab-plans-tab").append(this.container);
        },

        onBeforeRender: function () {
        },
        onDomRefresh: function () {

        },
        onRender: function () {
            this.habReviews.model = this.model;
            this.setTable();
            this.habReviews.onRender();
            this.onRenderBase();
        },
        getDataGrid: function () {            
            var dataModels = this.model.get("HabPlans");
            var dataGrid = {
                columns: [
                    { title: "Name" },
                    { title: "Hab service" },
                    { title: "Status" },
                    { title: "Is approved?" },
                    { title: "Date of plan" },
                    { title: "Effective date" },
                    { title: "Coordinator" },
                    { title: "Added by" },
                    { title: "Updated by" },
                    { title: "Date updating" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels, function (model, key, list) {
                var data = _this.getDataForRow(model);
                dataGrid.dataSet.push(data);
            });
            return dataGrid;
        },
        getDataForRow: function (model) {
            
            var tml_btn = null;
            if ($("#button-template-hab-plan").length != 0) {
                var html_btn = $("#button-template-hab-plan").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["Name"]);
            data.push(model["HabServiceName"]);
            data.push(model["StatusName"]);
            data.push(model["IsAproved"]);
            data.push(model["DatePlan"]);
            data.push(model["EffectivePlan"]);
            data.push((model["Coordinator"]!=null?model["Coordinator"].Name:""));
            data.push(model["AddedByName"]);
            data.push(model["UpdatedByName"]);
            data.push(model["DateUpdated"]);
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerHabPlanId"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        setTable: function () {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#hab-plans-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                order: [[4, "desc"]],
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data.dataSet,
                columns: _this.data.columns,
                language: {
                    emptyTable: "No records"
                },
                scrollX:true
            });

        },

        addRowToGrid: function (model) {
            var rowData = this.getDataForRow(model);
            this.dataTable.row.add(rowData).draw(false);
            this.dataTable.columns.adjust().draw();
        },
        removeRowGrid: function (id) {
            var _this = this;
            _.each(this.dataTable.rows().ids(), function (item, key) {
                var dataArray = _this.dataTable.row(key).data();
                var action = _.last(dataArray);
                var idRow = $(action).attr("data-row-id");
                if (idRow == id) {
                    _this.dataTable.row(key).remove().draw(false);
                    return false;
                }
            });
        },
        updateRow: function (model) {
            var rowData = this.getDataForRow(model);
            this.dataTable.row(this.currentIdxIndex).data(rowData).draw(false);
            this.dataTable.columns.adjust().draw();
        },
        setModel: function (model) {
            this.model = model;
            this.render();
        },
        onAdd: function () {
            if ($.isEmpty(this.model.get("ConsumerId"))) {
                GlobalEvents.trigger("showMessage", { title: "Can't perform the action...", message: "Save consumer info first", type_class: "alert-danger" });
                return;
            }
            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: null, parentView: this });
            this.modal.render();
            this.modal.showModal();
        },
        onEdit: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var phones = this.model.get("HabPlans");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(phones, { ConsumerHabPlanId: id });

            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },
        onPopupSave: function (model) {
            this.modal.closeModal();
            var values = this.model.get("HabPlans");
            if (!model.isUpdate) {
                if ($.isEmpty(model.ConsumerHabPlanId)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.ConsumerHabPlanId = count + ".new";
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The print document has been successfully added.");
            }
            else {
                _.extend(_.findWhere(values, { ConsumerHabPlanId: model.ConsumerHabPlanId }), model);
                this.updateRow(model);
                Utils.notify("The approved service has been successfully updated.");
            }
            this.model.set("HabPlans", values);
        },
        onDelete: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });

        },
        onGeneratePdf: function (event) {
            const _this = this;
            const base = new BaseModel();
            const habId = $(event.currentTarget).attr("data-id");

            let success = function (response) {
                $(".overlay").hide();
               if (response.status == "ok") {
                   _this.handleSuccessResponsePdfDownload(response.reportTemplates, habId);
               } else {
                    GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: "Unexpected error...", type_class: "alert-danger" }); 
               }
            }

            let error = function () {
                $(".overlay").hide();
                GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: "Unexpected error...", type_class: "alert-danger" });
            }
            
            $(".overlay").show();
            base.request("/api/consumerhabplansapi/getavailablereporttemplates/", {habPlanId: habId}, success, error);
        },
        handleSuccessResponsePdfDownload: function(reportTemplates, habId) {
            if (reportTemplates.length > 1) {
                this.handleReportDownloadPopup({ model: { reportTemplates, reportId: habId, reportName: 'Hab plan' }, parentView: this});
            } else {
                this.generatePdf(habId, null);
            }
        },
        handleReportDownloadPopup: function(popupParams) {
            PdfDownloadView.prototype.appendContainer();
            this.modal = new PdfDownloadView(popupParams);
            this.modal.render();
            this.modal.showModal();
        },
        generatePdf: function (id, templateDate) {
            if ($.isNumeric(id)) {
                id = parseInt(id);
                var base = new BaseModel();
                $(".overlay").show();
                var success = function (response) {
                    $(".overlay").hide();
                    if (response.status == "ok") {
                        var win = window.open(response.url, "_blank");
                    } else {
                        GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: response.message, type_class: "alert-danger" });
                    }
                };
                var error = function () {
                    $(".overlay").hide();
                    GlobalEvents.trigger("showMessage", { title: "Failed to generate", message: "Unexpected error...", type_class: "alert-danger" });                    
                };
                var objToSend = { habPlanId: id, templateDate };
                base.request("/api/consumerhabplansapi/getpdfreport/", objToSend, success, error);
            } else {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "You can't generated PDF from not saving document. Please save consumer info and try again.", type_class: "alert-danger" });
                return;
            }
        },
        onConfirm: function (id) {

            var _this = this;
            var success = function () {
                var values = _this.model.get("HabPlans");
                values = _.without(values, _.findWhere(values, { ConsumerHabPlanId: id }));
                _this.model.set("HabPlans", values);
                _this.removeRowGrid(id);
            }
            var baseModel = new BaseModel();
            baseModel.deleteData(id, success);
        },
        reloadTable: function () {
            var _this = this;
            this.dataTable.clear().draw();
            var data = this.getDataGrid();
            _.each(data.dataSet, function (item) {
                _this.dataTable.row.add(item).draw(false);
            });
            this.dataTable.columns.adjust().draw();
        },
        updateTable: function () {
            this.dataTable.columns.adjust().draw();
            this.habReviews.updateTable();
        },

        onAddReview:function(event){
            this.habReviews.onAdd(event);
        },
        onEditReview: function (event) {
            this.habReviews.onEdit(event);
        },
        onDeleteReview: function (event) {
            this.habReviews.onDelete(event);
        },
        onGenerateReviewsPdf: function (event) {
            this.habReviews.onGeneratePdf(event);
        }

    });

});