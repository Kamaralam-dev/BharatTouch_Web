var authenticationAdmin = function () {
    var me = this;
    this.authAdminUrl = null;

    var loginAdminForm = function () {
        debugger;
        $("#loginForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                Email: { required: true, email: true },
                Password: { required: true }
            },
            submitHandler: function (form) {
                debugger;
                var f = $(form);
                var data = f.serializeArray();
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {

                        showMessage(data.Message, data.Data, data.Type);

                        if (data.Success == true) {
                            window.location.href = "/Company/CompanyIndex";
                        }
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    },
                    error: function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        handleAjaxError()
                    }
                });
            }
        });

    }

    this.init = function () {

        loginAdminForm();

    }
}