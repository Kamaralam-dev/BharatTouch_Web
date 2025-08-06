var DeleteUserAccount = function () {
    var me = this;

    var validateForm = function () {

        $("#DeleteAviUserAccountForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'UserId') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                UserId: { required: true }
            },
            submitHandler: function (form) {
                $('#divResponseMsg').empty();
                var f = $(form);
                var data = f.serializeArray();
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        $('#divResponseMsg').html(data.Message);
                    },
                    error: handleAjaxError()
                });
            }
        });
    }

    this.init = function () {
        validateForm();
    }
}