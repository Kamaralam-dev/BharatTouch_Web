var DonorRegister = function () {
    var me = this;
    this.donorRegister = null;

    var upsertcontactForm = function () {
        $("#contactform").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                name: { required: true, letterswithspace: true },
                title: { required: true },
                email: { emailvalidatecustom: true, required: true },
                companyname: { required: true },
                Address1: { required: true },
                City: { required: true },
                State: { required: true },
                Zip: { required: true },
                Country: { required: true },
                phonenumber: { minlength: true, required: true },
                email: { required: true },

            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                
                $(".loading").show();
                $(".sent-message").hide();

                formData = new FormData($('form').get(0));
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });

                $(".error-message").hide();
                $(".loading").show();
                $("#btnRegisterDonor").text("Uploading...");
                submitDonorForm(f, formData);

            }
        });
    }

    var submitDonorForm = function (f, formData) {
        $.ajax({
            type: f[0].method,
            url: f[0].action,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                if (data.Success) {
                    $(".sent-message").show();
                    $(".loading").hide('');
                    $("#name").val('');
                    $("#title").val('');
                    $("#phonenumber").val('');
                    $("#companyname").val('');
                    //$("#address").val('');
                    $("#Address1").val('');
                    $("#Address2").val('');
                    $("#City").val('');
                    $("#State").val('');
                    $("#Zip").val('');
                    $("#Country").val('');
                    $("#AlternatePhone").val('');
                    $("#Facebook").val('');
                    $("#Twitter").val('');
                    $("#LinkedIn").val('');
                    $("#Website").val('');
                    $("#Teams").val('');
                    $("#Skype").val('');
                    $("#Zoom").val('');
                    $("#Instagram").val('');

                    $("#itemtype").val('');
                    $("#email").val('');
                    $('#btnRegisterDonor').text("Register");
                }
                else {
                    $('#btnRegisterDonor').text("Register");
                    $(".loading").hide('');
                    $(".error-message").html(data.Data);
                    $(".error-message").show();
                }

            },
            error: handleAjaxError()
        });
    }

    this.init = function () {

        upsertcontactForm();
    }
}