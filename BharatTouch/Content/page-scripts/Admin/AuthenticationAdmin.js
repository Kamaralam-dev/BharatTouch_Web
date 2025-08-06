var authenticationAdmin = function () {
    var me = this;
    this.authAdminUrl = null;

    var loginAdminForm = function () {
        
        $("#adminloginForm").validate({
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
                        debugger
                        showMessage(data.Message, data.Data, data.Type);

                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);

                        if (data.Success == true && data.OptionalValue == "SA") {
                            window.location.href = "/Admin/UserIndex";
                        }
                        else if (data.Success == true && data.OptionalValue=="BA") {
                            window.location.href = "/Admin/BlogIndex";                         
                        }
                        else if (data.Success == true && data.OptionalValue == "OA") {
                            window.location.href = "/Admin/OrderIndex";
                        }
                        else if (data.Success == true && data.OptionalValue == "LA") {
                            window.location.href = "/Admin/LeadIndex";
                        }
                      
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