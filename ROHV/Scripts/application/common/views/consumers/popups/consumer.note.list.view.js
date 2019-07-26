define(function (require) {

    require('marionette');
    require('underscore');
    var BaseModel = require('models/consumer.note.model'),
        ModalView = require('views/consumers/popups/consumer.note.simple.view'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="note-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#note-view",
        events: function () {
            return {
                "click [data-action=add]": "onAdd"
            }
        },
        template: function (serialized_model) {
            var templateHtml = $("#pop-template-notes-list").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function () {
            if ($.isEmpty(this.parentView.model.get("Notes"))) {
                this.parentView.model.set("Notes", []);
            }
            var values = this.parentView.model.get("Notes");
            _.each(values, function (model, key, list) {
                model["DateCreated"] = Utils.getDateText(model["DateCreated"]);
                model["Date"] = Utils.getDateText(model["Date"]);
                model["DateUpdated"] = Utils.getDateText(model["DateUpdated"]);
            });
            this.parentView.model.set("Notes", values);
            this.model = this.parentView.model;
            return this.parentView.model;
        },

        initialize: function (options) {
            this.parentView = options.parentView;
        },
        appendContainer: function () {
            $("#modal-message-container").append(this.container);
        },

        onBeforeRender: function () {
        },
        onDomRefresh: function () {

        },
        onRender: function () {
            this.setTable();
            this.onRenderBase();
        },
        getDataGrid: function () {

            var dataModels = _.filter(this.model.get("Notes"), { TypeId: 2 });
            var dataGrid = {
                columns: [
                    { title: "Status" },
                    { title: "Date" },
                    { title: "Notes" },
                    { title: "Added by" },
                    { title: "Updated by" }
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
        getAdditionalInformation: function (info) {
            if (!$.isEmpty(info)) {
                var additionalInfo = JSON.parse(info);
                var result = additionalInfo.From + "-->" + additionalInfo.To;
                return result;

            }
            return "No changes";
        },
        getDataForRow: function (model) {
            var tml_btn = null;
            var _this = this;
            var data = [];
            data.push(_this.getAdditionalInformation(model["AditionalInformation"]));
            data.push(model["Date"]);
            data.push(model["Notes"]);
            data.push(model["AddedByName"]);
            data.push(model["UpdatedByName"]);
            return data;
        },
        setTable: function () {
            var _this = this;
            this.data = this.getDataGrid();
            this.dataTable = $('#notes-popup-grid').DataTable({
                responsive: false,
                searching: false,
                paging: false,
                deferRender: true,
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                data: _this.data.dataSet,
                columns: _this.data.columns,
                language: {
                    emptyTable: "No records"
                }
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

        showModal: function () {

            var _this = this;
            this.$el.modal();

            this.$el.off('hidden.bs.modal');
            this.$el.on('hidden.bs.modal',
                    function () {
                        _this.destroy();
                    });
            this.$el.modal('show');

        },
        closeModal: function () {

            this.$el.modal('hide');
        },
        onAdd: function () {
            this.closeModal();
            var _this = this;
            setTimeout(function () {
                if ($.isEmpty(_this.model.get("ConsumerId"))) {
                    GlobalEvents.trigger("showMessage", { title: "Can't perform the action...", message: "Please select a consumer first", type_class: "alert-danger" });
                    return;
                }
                ModalView.prototype.appendContainer();
                _this.modal = new ModalView({ model: null, parentView: _this.parentView });
                _this.modal.render();
                _this.modal.showModal();
            }, 500);
        }
    });

});