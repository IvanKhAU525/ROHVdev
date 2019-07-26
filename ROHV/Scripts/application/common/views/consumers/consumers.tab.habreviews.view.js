define(function(require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/consumer.habreview.model'),
        ModalView = require('views/consumers/popups/consumer.habreview.view'),
        DeleteDialog = require('views/confirm.view'),
        BaseView = require('views/base.view');
        PdfDownloadView = require('views/consumers/popups/consumer.report.pdfdownload.view');

    return BaseView.extend({
        serializeData: function(model) {
            if ($.isEmpty(this.model.get("HabReviews"))) {
                this.model.set("HabReviews", []);
            }
            var values = this.model.get("HabReviews");
            _.each(values,
                function(model, key, list) {
                    model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
                    model["DateUpdated"] = Utils.getDateText(model["DateUpdated"]);
                    model["DateReview"] = Utils.getDateText(model["DateReview"]);
                    model["SignatureDate"] = Utils.getDateText(model["SignatureDate"]);
                });
            this.model.set("HabReviews", values);
            return this.model;
        },
        onRender: function() {
            this.serializeData(this.model);
            this.setTable();
            this.onRenderBase();
        },
        getDataGrid: function() {

            var dataModels = this.model.get("HabReviews");
            var dataGrid = {
                columns: [
                    { title: "Hab Service" },
                    { title: "Date Review" },
                    { title: "Coordiantor" },
                    { title: "MSC" },
                    { title: "Parent" },
                    { title: "Added by" },
                    { title: "Updated by" },
                    { title: "Date updating" },
                    { title: "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                dataSet: []
            };
            var _this = this;
            _.each(dataModels,
                function(model, key, list) {
                    var data = _this.getDataForRow(model);
                    dataGrid.dataSet.push(data);
                });
            return dataGrid;
        },
        getDataForRow: function(model) {
            var tml_btn = null;
            if ($("#button-template-hab-review").length != 0) {
                var html_btn = $("#button-template-hab-review").html();
                tml_btn = _.template(html_btn);
            }
            var data = [];
            data.push(model["ServiceName"]);
            data.push(model["DateReview"]);
            data.push(this.getCoordinatorName(model));
            data.push((model["MSC"]!=null?model["MSC"].Name:""));
            data.push(model["Parents"]);
            data.push(model["AddedByName"]);
            data.push(model["UpdatedByName"]);
            data.push(model["DateUpdated"]);
            if (tml_btn != null) {
                var objToPass = { id: model["ConsumerHabReviewId"] };
                data.push(tml_btn(objToPass));
            }
            return data;
        },
        getCoordinatorName: function(model) {
            if (model["ServiceId"] == 1) {
                if (model["CHCoordinator"] != null) {
                    return model["CHCoordinator"].Name;
                }
            }
            if (model["ServiceId"] == 2) {
                if (model["DHCoordinator"] != null) {
                    return model["DHCoordinator"].Name;
                }
            }
            return "";
        },
        setTable: function() {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#hab-reviews-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                order: [[2, "desc"]],
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data.dataSet,
                columns: _this.data.columns,
                language: {
                    emptyTable: "No records"
                },
                scrollX: true
            });
        },

        addRowToGrid: function(model) {
            var rowData = this.getDataForRow(model);
            this.dataTable.row.add(rowData).draw(false);
            this.dataTable.columns.adjust().draw();
        },

        removeRowGrid: function(id) {
            var _this = this;
            _.each(this.dataTable.rows().ids(),
                function(item, key) {
                    var dataArray = _this.dataTable.row(key).data();
                    var action = _.last(dataArray);
                    var idRow = $(action).attr("data-row-id");
                    if (idRow == id) {
                        _this.dataTable.row(key).remove().draw(false);
                        return false;
                    }
                });
        },
        updateRow: function(model) {
            var rowData = this.getDataForRow(model);
            this.dataTable.row(this.currentIdxIndex).data(rowData).draw(false);
            this.dataTable.columns.adjust().draw();
        },
        setModel: function(model) {
            this.model = model;
            this.render();
        },
        onAdd: function() {
            var _this = this;
            if ($.isEmpty(this.model.get("ConsumerId"))) {
                GlobalEvents.trigger("showMessage",
                    {
                        title: "Can't perform the action...",
                        message: "Select consumer first",
                        type_class: "alert-danger"
                    });
                return;
            }
            //GET DEFAULT OBJECT
            var consumerId = this.model.get("ConsumerId");
            var requestModel = new BaseModel();
            var success = function(response) {
                $(".overlay").hide();
                response.ConsumerHabReviewId = null;
                ModalView.prototype.appendContainer();
                _this.modal = new ModalView({ model: response, parentView: _this });
                _this.modal.render();
                _this.modal.showModal();
            };
            var error = function() {
                $(".overlay").hide();
                GlobalEvents.trigger("showMessage",
                    { title: "Failed to generate", message: "Unxpected error...", type_class: "alert-danger" });
            };
            var objToSend = { consumerId: consumerId };
            requestModel.request("/api/consumerhabreviewsapi/getdefaultdata/", objToSend, success, error);


        },
        onEdit: function(event) {
            var id = $(event.currentTarget).attr("data-id");
            var row = $(event.currentTarget).parent().parent().parent();
            this.currentIdxIndex = this.dataTable.row(row).index();
            var reviews = this.model.get("HabReviews");
            if ($.isNumeric(id)) id = parseInt(id);
            var model = _.find(reviews, { ConsumerHabReviewId: id });


            ModalView.prototype.appendContainer();
            this.modal = new ModalView({ model: model, parentView: this });
            this.modal.render();
            this.modal.showModal();

        },

        onPopupSave: function(model) {
            this.modal.closeModal();
            var values = this.model.get("HabReviews");
            if (!model.isUpdate) {
                if ($.isEmpty(model.ConsumerHabReviewId)) {
                    var count = 1;
                    if (!$.isEmptyObject(values)) count = values.length + 1;
                    else values = [];
                    model.ConsumerHabReviewId = count + ".new";
                }
                values.push(model);
                this.addRowToGrid(model);
                Utils.notify("The hab review has been successfully added.");
            } else {
                _.extend(_.findWhere(values, { ConsumerHabReviewId: model.ConsumerHabReviewId }), model);
                this.updateRow(model);
                Utils.notify("The hab review has been successfully updated.");
            }
            this.model.set("HabViews", values);
        },

        onDelete: function(event) {

            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });

        },
        onGeneratePdf: function(event) {
            var _this = this;
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) {
                id = parseInt(id);
                var base = new BaseModel();
                $(".overlay").show();
                var success = function(response) {
                    $(".overlay").hide();
                    if (response.status == "ok") {
                       // var win = window.open(response.url, "_blank");
                        _this.handleSuccessResponsePdfDownload(response.reportTemplates, id);
                    } else {
                        GlobalEvents.trigger("showMessage",
                            { title: "Failed to generate", message: response.message, type_class: "alert-danger" });
                    }
                };
                var error = function() {
                    $(".overlay").hide();
                    GlobalEvents.trigger("showMessage",
                        { title: "Failed to generate", message: "Unxpected error...", type_class: "alert-danger" });
                };
                var objToSend = { reportId: id };
                base.request("/api/consumerhabreviewsapi/getavailablereporttemplates/", objToSend, success, error);
            } else {
                GlobalEvents.trigger("showMessage",
                    {
                        title: "Error",
                        message:
                            "You can't generated PDF from not saving document. Please save consumer info and try again.",
                        type_class: "alert-danger"
                    });
                return;
            }

        },
        handleSuccessResponsePdfDownload: function (reportTemplates, reportId) {
            if (reportTemplates.length > 1) {                
                this.handleReportDownloadPopup({ model: { reportTemplates, reportId: reportId, reportName: 'Hab plan review' }, parentView: this });
            } else {
                this.generatePdf(reportId, null);
            }
        },
        handleReportDownloadPopup: function (popupParams) {
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
                var objToSend = { habReviewId: id, templateDate };
                base.request("/api/consumerhabreviewsapi/getpdfreport/", objToSend, success, error);
            } else {
                GlobalEvents.trigger("showMessage", { title: "Error", message: "You can't generated PDF from not saving document. Please save consumer info and try again.", type_class: "alert-danger" });
                return;
            }
        },
        onConfirm: function(id) {

            var _this = this;
            var success = function() {
                var values = _this.model.get("HabReviews");
                values = _.without(values, _.findWhere(values, { ConsumerHabReviewId: id }));
                _this.model.set("HabReviews", values);
                _this.removeRowGrid(id);
            }
            var baseModel = new BaseModel();
            baseModel.deleteData(id, success);
        },
        reloadTable: function() {
            var _this = this;
            this.dataTable.clear().draw();
            var data = this.getDataGrid();
            _.each(data.dataSet,
                function(item) {
                    _this.dataTable.row.add(item).draw(false);
                });
            this.dataTable.columns.adjust().draw();
        },
        updateTable: function() {
            this.dataTable.columns.adjust().draw();
        }

    });

});