define(function(require) {

    require('marionette');
    require('underscore');

    var BaseModel = require('models/employee.model'),
        BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="employee-form-view"  class="modal fade" tabindex="-1" role="dialog"></div>',
        el: "#employee-form-view",
        events: function() {
            return {
                "click #save-modal": "onSave",
                "click [data-action=remove-file-signature]": "onRemoveSignature"
            }
        },
        initialize: function(options) {
            this.parentView = options.parentView;
        },
        appendContainer: function() {
            $("#modal-message-container").append(this.container);
        },
        template: function(serialized_model) {
            var templateHtml = $("#employee-form-template").html();
            var template = _.template(templateHtml);
            return template({ model: serialized_model });
        },
        serializeData: function() {

            if (this.model == null || this.model == undefined) {
                this.model = new BaseModel();
            }
            return this.model;
        },
        onBeforeRender: function() {
        },

        onDomRefresh: function() {

        },
        onRender: function() {
            var isEdit = (this.model.get("ContactId") != null);
            this.model.setFormValidation(isEdit);

            this.setField('#State');
            this.setField('#CategoryId');
            this.setField('#ContactTypeId');
            this.setField('#DepartmentId');

            $("#Phone").inputmask({ "mask": "(999) 999-9999" });
            $("#MobilePhone").inputmask({ "mask": "(999) 999-9999" });
            $("#Fax").inputmask({ "mask": "(999) 999-9999" });
            this.onRenderBase();
            this.setFileEvent();
            this.setSignature(this.model.get("Signature"));
        },

        setFileEvent: function() {
            var _this = this;
            $(this.$el).off('change', '.btn-file :file').on('change',
                '.btn-file :file',
                function() {
                    var input = $(this);
                    var file = input.get(0).files[0];
                    if (file.type == "image/jpeg" || file.type == "image/png") {
                        _this.readFile(file);
                    } else {
                        var errorMessage =
                            "<span class='help-block'>The file has wrong format. Please select image</span>";
                        $("#signature-img").empty();
                        $("#signature-img").append(errorMessage);
                        _this.model.set("Signature", null);
                        $("#Signature").val("");
                    }
                });
        },
        readFile: function(file) {
            var _this = this;
            if (file != null) {
                var reader = new FileReader();
                reader.readAsDataURL(file);
                reader.addEventListener("load",
                    function() {
                        var fileData = reader.result;
                        _this.setSignature(fileData);
                        _this.model.set("Signature", fileData);
                    },
                    false);
            }
        },
        setSignature: function (data) {
            if (data != null) {
                var element = new Image();
                element.src = data;
                element.height = 60;
                $("#signature-img").empty();
                $("#signature-img").append(element);
                $('#signatureContainer').show();
            } else {
                $('#signatureContainer').hide();
            }
        },
        onRemoveSignature: function() {
            $("#signature-img").empty();
            this.model.set("Signature", null);
            $("#Signature").val("");
            $('#signatureContainer').hide();
        },
        resetField: function(elm) {
            //Fix for validation
            var parent = elm.parent().parent();
            $(parent).removeClass("has-error");
            $(".help-block", parent).hide();
        },
        setField: function(id) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false
            });
            $(id).on('show.bs.select',
                function(e) {
                    _this.resetField($(id));
                });
        },
        showModal: function() {
            var _this = this;
            this.$el.modal();

            this.$el.off('hidden.bs.modal');
            this.$el.on('hidden.bs.modal',
                function() {
                    _this.destroy();
                });
            this.$el.modal('show');

        },
        closeModal: function() {
            this.$el.modal('hide');
        },
        onSave: function() {
            var _this = this;
            if (this.model.isValid()) {
                var isEdit = this.model.get("ContactId") != null;
                if (isEdit) {
                    isEdit = this.model.get("ContactId") != -1;
                }
                var obj = Utils.serializeForm(this.model.formId);
                obj.IsUpdate = isEdit;
                if (!isEdit) {
                    obj.ContactId = -1;
                }
                this.model.parseObj(obj);

                var success = function(responce) {
                    _this.model.set("ContactId", responce.id);
                    _this.parentView.onPopupSave(_this.model);
                };
                var error = function() {
                    _this.closeModal();
                };
                this.model.postData(success, error);
            }
        }
    });

});