define(function (require) {

    require('marionette');
    require('underscore');

    var ModalView = require('views/advocates/advocate.form.view'),
        DeleteDialog = require('views/confirm.view'),
        BaseModel = require('models/advocate.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="users-view"></div>',
        events: function () {
            return {
                "click [data-action=add]": "onAdd",
                "click [data-action=edit]": "onEdit",
                "click [data-action=delete]": "onDelete",

                "change #AdvocateName": "OnChangeAdvocateName",
                "keyup #AdvocateName": "OnChangeAdvocateName"
            }
        },
        initialize: function (options) {
            this.appInstance = require('app/app.instance');
        },
        template: function (serialized_model) {
            var template = Marionette.TemplateCache.get("advocates");;
            return template;
        },
        serializeData: function () {
            return this.model;
        },
        appendContainer: function () {
            $("#users-container").append(this.container);
        },      
        onBeforeRender: function () {
        },
        onDomRefresh: function () {
            this.setTable();
        },
        onRender: function () {
            this.onRenderBase();
        },      
        setTable: function () {
            var _this = this;                        
            this.dataTable = $('#advocates-grid').DataTable({
                processing: true,
                serverSide: true,
                responsive: false,
                searching: true,
                order: [[0, "asc"]],
                dom: '<"top"lp>rt<"bottom"i><"clear">',
                columns: [
                    { "data": "Name", "title": "Name","width": "200px" },
                    { "data": "Company", "title": "Company", "width": "200px" },
                    { "data": "Address", "title": "Address", "width": "200px" },
                    { "data": "City", "title": "City" },
                    { "data": "State", "title": "State" },
                    { "data": "Specialization", "title": "Specialization" },                    
                    { "data": "Phone", "title": "Phone" },
                    { "data": "Email", "title": "Email" },
                    { "data": "Action", "title": "Actions", "width": "120px", "orderable": false, contentPadding: "4px" }
                ],
                ajax: {
                    contentType: "application/json",
                    url: "/api/advocatesapi/getadvocates",
                    method: "POST",
                    data: function (d) {
                        return JSON.stringify(d);
                    }
                },
                rowCallback: function (row, data) {
                    //Need to set here the button                                     
                    var address = _this.getConcatenate(data["Address1"], data["Address2"]);
                    $('td:eq(0)', row).html(data["LastName"] + ", " + data["FirstName"]);
                    $('td:eq(2)', row).html(address);

                    if ($("#button-template-advocates").length != 0) {
                        var html_btn = $("#button-template-advocates").html();
                        tml_btn = _.template(html_btn);
                        var objToPass = { id: data["AdvocateId"] };
                        $('td:eq(8)', row).html(tml_btn(objToPass));
                    }                    
                },
                language: {
                    emptyTable: "No records"
                }
            });

        },
        getConcatenate: function (str1, str2, str3, str4)
        {
            var arr = [str1, str2, str3, str4];            
            var result = "";
            for (var i = 0; i < arr.length; i++) {
                var item = arr[i];
                if (!$.isEmpty(item)) {
                    if (result != "") result += ", ";
                    result += item;
                }
            }
            return result;
        },

        showForm: function (obj) {
            if (obj.action == "edit") {
                this.appInstance.navRouter.navigate('advocates/edit', { trigger: false });
            } else {
                this.appInstance.navRouter.navigate('advocates/add', { trigger: false });
            }
            var _this = this;
            var model = new BaseModel();
            var success = function () {
                ModalView.prototype.appendContainer();
                _this.modal = new ModalView({ model: model, parentView: _this });
                _this.modal.render();
                _this.modal.showModal();
            };
            var error = function () {
                GlobalEvents.trigger("showMessage", { title: "Failed to retrieve data", message: "Something is going wrong. Please reload the page and try again." });
            }
            if (obj.action == "edit") {
                model.getModel(obj.id, success, error);
            } else {                
                success();                
            }
        },        
        onAdd: function () {
            this.showForm({ id: "", action: "add" });
        },
        onEdit: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            this.showForm({ id: id, action: "edit" });

        },
        onPopupSave: function (model) {
            this.modal.closeModal();            
            if (!model.get("IsUpdate")) {                
                Utils.notify("The advocate has been successfully added.");
            } else {                
                Utils.notify("The advocate has been successfully saved.");
            }
            this.reloadTable();
        },
        onDelete: function (event) {
            var id = $(event.currentTarget).attr("data-id");
            if ($.isNumeric(id)) id = parseInt(id);

            DeleteDialog.prototype.appendContainer();
            this.dialogDelete = new DeleteDialog();
            this.dialogDelete.render();
            this.dialogDelete.showModal({ id: id, parentView: this });
        },
        onConfirm: function (id) {
            var _this = this;
            var success = function () {
                _this.reloadTable();
            };
            BaseModel.prototype.deleteData(id, success)
            
        },
       
        reloadTable: function () {
            var _this = this;                   
            this.dataTable.columns.adjust().draw();
        },
        OnChangeAdvocateNamee: function (event) {
            var _this = this;
            var text = $(event.currentTarget).val();

            var searchVal = this.dataTable.column(0).search();
            if (searchVal != text) {                
                if (this.timeSearch != null) {
                    clearTimeout(this.timeSearch);
                    _this.timeSearch = null;
                }
                _this.timeSearch = setTimeout(function () {                    
                    _this.dataTable.column(0).search(text).draw();                    
                }, 300);
            }
        }

    });

});