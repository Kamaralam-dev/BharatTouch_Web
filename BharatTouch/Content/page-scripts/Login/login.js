var loginUser = function () {
    var me = this;
    this.authUserUrl = null;

    var loginUserForm = function () {
        $("#loginForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Email' || element[0].name == 'Password') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                Email: { required: true, email: true },
                Password: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {

                        showMessage(data.Message, data.Data, data.Type);
                        //if (data.Success == true) {
                        //    window.location.href = "/home/index";
                        //}
                        if (data.Success == true && data.OptionalValue == "1") {
                            window.location.href = "/Users/index"
                        }
                        else if (data.Success == true) {
                            window.location.href = "/home/index";
                        }
                    },
                    error: handleAjaxError()
                });
            }
        });

    }

    this.init = function () {
        loginUserForm();
    }
}