var login = function () {
    var loginUserForm = function () {
        $("#loginForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Email' || element[0].name == 'Password') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                Email: { required: true, email: true },
                Password: { required: true }
            },
            submitHandler: function (form) {
                //  alert("3");
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
                        debugger;

                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        showMessage(data.Message, data.Data, data.Type);
                        if (data.Success == true) {
                            window.location.href = "/edit/" + data.OptionalValue;// /rnaura/john.cena;
                            return;
                        }
                    },
                    error: function () {
                        alert("5");
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        handleAjaxError()
                    }
                });
            }
        });

    }

    this.init = function () {
        loginUserForm();
    }
}