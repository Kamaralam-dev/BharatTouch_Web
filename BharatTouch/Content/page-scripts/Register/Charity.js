var CharityRegister = function () {
    var me = this;
    this.ChartieRegister = null;

    var upsertCharityForm = function () {
        $("#Charityform").validate({
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
                itemtype: { required: true },

            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;

                var fileUpload = $("#charityfile").get(0);
                var files = fileUpload.files;

                if (files.length == 0) {
                    alert("Charity proof is required.");
                    $(".loading").hide('');
                    return false;
                }

                formData = new FormData($('form').get(0));

                // Looping over all files and add it to FormData object
                for (var i = 0; i < files.length; i++) {
                    formData.append(files[i].name, files[i]);
                }

                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });

                $(".error-message").hide();
                $(".loading").show();
                $("#btnRegisterCharity").text("Processing...");
                submitCharityForm(f, formData);

            }
        });
    }

    var submitCharityForm = function (f, formData) {
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
                    $("#charityfile").val('');
                    $('#btnRegisterCharity').text("Register");
                }
                else {
                    $(".error-message").html(data.Data);
                    $(".error-message").show();
                    $(".loading").hide('');
                    $('#btnRegisterCharity').text("Register");
                }

            },
            error: handleAjaxError()
        });
    }

    this.init = function () {

        upsertCharityForm();
    }
}