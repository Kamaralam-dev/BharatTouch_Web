var createUser = function () {
    var me = this;
    this.createUserUrl = null;
    var isDuplicateEmail = false;
    var $croppableImage = null;

    var createUserForm = function () {

        $("#CreateUserForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'FirstName' || element[0].name == 'Email' || element[0].name == 'Password') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                FirstName: { required: true, letterswithspace: true },
                Email: { required: true, emailvalidatecustom: true },
                Phone: { required: true, minlength: true },
                Tagline: { required: true,}
                //Zip: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
               
                    formData = new FormData($('form').get(0));
                    //formData.append('ImageUrl', $("#hdnImageUrl").val());
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                    });
                    submitUserForm(f, formData);
               
            }
            //submitHandler: function (form) {
            //    var f = $(form);
            //    var data = f.serializeArray();              
            //    if (isDuplicateEmail && $("#UserId").val() == "0")
            //        return false;

            //    $.ajax({
            //        type: f[0].method,
            //        url: f[0].action,
            //        data: data,
            //        dataType: 'json',
            //        success: function (data, strStatus) {
            //            showMessage(data.Message, data.Data, data.Type);
            //            window.location.href = "/Users";
            //        },
            //        error: handleAjaxError()
            //    });
            //}
        });
    }

    var submitUserForm = function (f, formData) {
        

        $.ajax({
            type: f[0].method,
            url: f[0].action,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                if (data.Success)
                    window.location.href = "/Users/Profile?code=" + data.OptionalValue;
            },
            error: handleAjaxError()
        });
    }

    onSelectFile = function (input) {

        ValidateUploadFile(input);

        if (input.files && input.files[0]) {
            $('#croppableImage').removeAttr('src');
            $('#croppableImage').cropper('destroy');

            var files = input.files;
            var formData = new FormData($('form').get(0));
            if (files.length > 0) {
                for (var x = 0; x < files.length; x++) {
                    formData.append("file" + x, files[x]);

                }
                formData.append('UserId', $("#UserId").val());
            }
            $.ajax({
                type: 'POST',
                url: '/Users/SaveUserFile',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data, strStatus) {
                    if (data.Success) {
                        $('#cropImageContainer').show();
                        $('#croppableImage').attr('src', data.Data.replace("~", "")).width(150).height(150);
                        $croppableImage = $('#croppableImage');
                        $('#uploadFile').show();
                        $croppableImage.cropper({
                            viewMode: 2,
                            aspectRatio: 16 / 9,
                            minContainerHeight: 250,
                            crop: function (event) {
                            }
                        });
                    }
                    else {
                        showMessage("Warning!", data.Message, "notice");
                        $("#UserImageUrl").val('');
                    }
                },
                error: handleAjaxError()
            });
        }
    }

    $("#Email").blur(function () {
        var email = $(this);
        validateDuplicateEmail(email);
    });

    var validateDuplicateEmail = function (email) {
        if ($("#UserId").val() == "0") {
            $.ajax({
                type: 'GET',
                data: { email: email.val() },
                url: "/Users/CheckEmailAvailability",
                dataType: 'json',
                success: function (data) {
                    if (data.Data == "1") {
                        $("#isExistEmailError").text(data.Message);
                        isDuplicateEmail = true;
                    }
                    else {
                        $("#isExistEmailError").text("");
                        isDuplicateEmail = false;
                    }
                },
                error: handleAjaxError()
            });
        }
    }

    //onSelectFile = function (input) {

    //    ValidateUploadFile(input);

    //    if (input.files && input.files[0]) {
    //        $('#croppableImage').removeAttr('src');
    //        $('#croppableImage').cropper('destroy');

    //        var files = input.files;
    //        var formData = new FormData($('form').get(0));
    //        if (files.length > 0) {
    //            for (var x = 0; x < files.length; x++) {
    //                formData.append("file" + x, files[x]);
    //            }
    //        }
    //        $.ajax({
    //            type: 'POST',
    //            url: '/Users/SaveTempProfileImage',
    //            data: formData,
    //            dataType: 'json',
    //            contentType: false,
    //            processData: false,
    //            success: function (data, strStatus) {
    //                $('#cropImageContainer').show();
    //                $('#croppableImage').attr('src', data.Data.replace("~", "")).width(200).height(200);
    //                $croppableImage = $('#croppableImage');
    //                $croppableImage.cropper({
    //                    aspectRatio: 16 / 9
    //                });
    //            },
    //            error: handleAjaxError()
    //        });
    //    }
    //}

    var updatePassword = function (userId, password) {
        $.ajax({
            type: 'POST',
            url: '/Users/UpdatePassword',
            data: { userId: userId, password: password },
            dataType: 'json',
            success: function (data, strStatus) {
                if (data.Success) {
                    $("#Password").val(password);
                }
                showMessage(data.Message, data.Data, data.Type);
            }
        });

    }

   

    //education

    var bindEducation = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#UserId").val() },
            url: '/Users/BindEducation',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#educationContainer").html(data);
            },
            error: handleAjaxError()
        });
    }

    var openEducationModel = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: '/Users/OpenEducationModel',
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                OpenModel("AddEducationModal");
                saveEducation();
            },
            error: handleAjaxError()
        });
    }

    var saveEducation = function () {
        $("#upsertEducationForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                Institute: { required: true },
                University: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: f.serializeArray(),
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        bindEducation();
                        CloseModal();
                    },
                    error: handleAjaxError()
                });
            }
        });


        $("#AddEducationModal")
            .modal({ show: true, backdrop: false })
            .on('click', '.mfp-close', function (e) {
                CloseModal();
            });
    }

    //end education
    this.init = function () {
        createUserForm();
        
        $("#btnAddProfessional").click(function () {
           // openProfessionalModel(0);
        });

        $("#btnAddEducation").click(function () {
            //openEducationModel(0);
        });

        $("#btnUpdatePassword").click(function () {
            var oldPassword = $("#Password").val();
            var currentPassword = $("#CurrentPassword").val();
            var newPassword = $("#NewPassword").val();
            var confirmPassword = $("#ConfirmPassword").val();

            if (currentPassword == "") {
                showMessage("Warning!", "Enter current password.", "notice");
                return false;
            }

            if (newPassword == "") {
                showMessage("Warning!", "Enter new password.", "notice");
                return false;
            }


            if (confirmPassword == "") {
                showMessage("Warning!", "Enter confirm password.", "notice");
                return false;
            }

            if (oldPassword != currentPassword) {
                showMessage("Warning!", "Current password is wrong.", "notice");
                return false;
            }
            if (newPassword != confirmPassword) {
                showMessage("Warning!", "New password and confirm password not matched.", "notice");
                return false;
            }

            if (oldPassword == newPassword) {
                showMessage("Warning!", "Password should not be same as old password.", "notice");
                return false;
            }

            updatePassword($("#UserId").val(), newPassword);

        });

        $("#createFormContainer").on("click", "button", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Users";
                    break;
            }
        });


    }
}