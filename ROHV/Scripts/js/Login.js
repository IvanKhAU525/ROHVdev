var Login = {

    initPage: function () {
        Utils.initValudationPlugin();
        this.initValidationForm();
        this.eventSubmit();
    },
    initValidationForm: function () {
        $('form').validate({
            rules: {
                email: {
                    minlength: 3,
                    maxlength: 1024,
                    required: true,
                    email: true
                },
                password: {
                    minlength: 6,
                    maxlength: 1024,
                    required: true
                }
            }
        });
    },
    addGlobalError: function (msg) {
        var alert = '<div class="alert alert-danger fade in" role="alert">' + msg + '</div>';
        $("#alert-container").html(alert);
        $(".alert").alert();
    },
    submitAction: function () {
        $(".alert").alert("close");
        if ($('form').valid()) {
            var frm = $('form');
            var data = {};
            data["postData"] = Utils.serializeForm(frm);
            data["action"] = $('form').attr("action");
            
            Utils.ajax(data, function (result) {
                switch (result["result"]) {
                    case "ok": {
                        location.href = result["returnUrl"];
                        break;
                    }
                    case "warning":
                    case "lockedout":
                    case "fail":
                        {
                            Utils.hideLoader($("[data-action='submit']"));
                            Login.addGlobalError(result["error"]);
                            break;
                        }
                }
            }, function () {
            }, $("[data-action='submit']"));
        }
    },
    eventSubmit: function () {
        Utils.setEventEnterSubmit($('form'), function () {
            Login.submitAction();
            return false;
        });
        $("[data-action='submit']").click(function () {
            Login.submitAction();
            return false;
        });
    }
};
$(document).ready(function () {

    Login.initPage();
});