var companyLogo = function () {
    var me = this;
    this.uploadLogoUrl = null;
    var $croppableImage = null;
    var IsNewSave = 0;
    this.loggedUser = null;
    this.UserId = 0;

    var uploadForm = function () {
        $("#uploadImageForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
            },
            submitHandler: function (form) {
                //showLoader();
                var f = $(form);
               

                var data = f.serializeArray();
                var formData;
                if ($croppableImage != undefined) {
                    formData = new FormData();
                    $croppableImage.cropper('getCroppedCanvas').toBlob((getCropped) => {
                        formData.append('ImageUrl', getCropped);
                        $(data).each(function (index, element) {
                            formData.append(element.name, element.value);

                        });

                        submitUploadForm(f, formData);
                    });
                }
                else {
                    formData = new FormData($('form').get(0));
                    formData.append('ImageUrl', $("#hdnImageUrl").val());
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                    });
                    submitUploadForm(f, formData);
                }
            }
        });
    }

    var submitUploadForm = function (f, formData) {
        debugger

        f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
        f.find(":submit").prop('disabled', true);
        var url = f[0].action;
        if (me.UserId != 0) {
            url = me.uploadLogoUrl;
            formData.append("UserId", me.UserId);
        }

        $.ajax({
            type: f[0].method,
            url: url,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                
                showMessage(data.Message, data.Data, data.Type);

                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
                    
                if (data.Success) {
                    setTimeout(function () {
                        window.location.reload();
                    }, 500);
                }
            },
            error: handleAjaxError(function () {
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
            })
        });
    }



    onSelectFile = function (input) {

        ValidateUploadFile(input);

        if (input.files && input.files[0]) {
            $('#croppableImage').removeAttr('src');
            $('#croppableImage').cropper('destroy');
            var files = input.files;
            var formData = new FormData();
            if (files.length > 0) {
                for (var x = 0; x < files.length; x++) {
                    formData.append("file" + x, files[x]);
                }
            }
            $("#cropPrfLoader").css('display', 'inline-block');   
            $.ajax({
                type: 'POST',
                url: '/Users/SaveTempProfileImage',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data, strStatus) {
                    //hideLoader();
                    if (data.Success) {
                       
                        $('#croppableImage').attr('src', data.Data.replace("~", "")).width(200).height(200);
                        $croppableImage = $('#croppableImage');
                        $croppableImage.cropper({
                            aspectRatio: 16 / 15
                        });
                        if ($('#croppableImage').attr('src') != '') {
                            $("#cropPrfLoader").css('display', 'none');
                            $('#cropImageContainer').show();
                        }
                    }
                    else {
                        $("#ImageUrl").val('');
                        showMessage("Warning!", data.Data, "notice");
                        $("#cropPrfLoader").css('display', 'none');   
                    }

                },
                error: handleAjaxError()
            });
        }
    }

    this.init = function () {
        uploadForm();
    }
}

