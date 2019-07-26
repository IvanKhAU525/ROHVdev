define(function (require) {

    require('marionette');
    require('underscore');
    var BaseView = require('views/base.view'),
        Dropzone = require('dropzone');

    return BaseView.extend({
        setFileEvent: function () {
            var _this = this;
            _this.fileData = null;            
        },
        readFileData: function (callBack) {
            if (!this.fileData) {
                setTimeout(callBack.bind(this), 0);
                return;
            }
            Utils.showOverlay();
            var reader = new FileReader();
            reader.readAsDataURL(this.fileData);
            reader.addEventListener("load", function () {
                callBack && callBack(reader.result);

            }, false);
        },
        readFileDataPromise: function () {
            var _this = this;
            return new Promise(function (fulfilled, rejected) {
                _this.readFileData(function (data) {
                    fulfilled(data)
                });

            });
        },        
        setupDropzone: function (dropzoneId) {
            var self = this;
            this.dropzone = new Dropzone(dropzoneId, {
                url: '/',
                addRemoveLinks: true,
                maxFiles: 1,
                thumbnailWidth: null,
                thumbnailHeight: null,
                init: function () {
                    this.on("maxfilesexceeded", function (file) {
                        this.removeAllFiles();
                        this.addFile(file);
                    });
                },
                success: function (file, response) {
                    self.fileData = file;
                }
            });
        }
    });/*.end module*/

});/*.end defined*/