var changeUserPasswordinit = function () {
    var me = this;

    $(document).on('click', '.toggle-password', function () {
        var input = $($(this).attr("toggle"));
        var isPassword = input.attr("type") === "password";
        input.attr("type", isPassword ? "text" : "password");
        $(this).toggleClass("fa-eye fa-eye-slash");
    });

    $("#btnResetUserPassword").on("click", function () {
        $("#changePasswordForm").validate({
            rules: {
                Password: {
                    required: true,
                    customRegex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&#]{8,16}$/                },
                ConfirmPassword: {
                    required: true,
                    equalTo: "#Password" // ensures they match
                }
            },
            messages: {
                ConfirmPassword: {
                    equalTo: "Passwords do not match."
                }
            },
            errorPlacement: function (error, element) {
                if (element.parent().hasClass('password-toggle-wrapper')) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();

                var token = f.find("input[name='AuthToken']").val();
                var password = f.find("input[name='Password']").val();
                var confirmPassword = f.find("input[name='ConfirmPassword']").val();

                if (!token) {
                    showMessage("Failed", "Reset token is missing or expired.", "error");
                    return;
                }

                if (password !== confirmPassword) {
                    showMessage("Failed", "Passwords do not match.", "error");
                    return;
                }
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        showMessage((data.Success ? "Success" : "Failed"), data.Data, data.Type);
                        if (data.Success) {
                            f[0].reset();
                            setTimeout(function () {
                                window.location.href = "/Home/Login";
                            }, 3000);
                        }
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });
    });

    

    this.init = function () {


    }

}