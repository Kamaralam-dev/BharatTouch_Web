var upsertSponsor = function () {
    var me = this;
    this.upsertSponsorUrl = null;
    this.deleteSponsorUrl = null;
    var IsNewSave = 0;
    this.loggedUser = null;
    var $croppableImage = null;

    var upsertSponsorForm = function () {
        $("#upsertSponsorForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                Name: { notspecialchars: true, required: true },                
                Email: { emailvalidatecustom: true },
                Phone: { minlength: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;

                if ($croppableImage != undefined) {
                    formData = new FormData();
                    $croppableImage.cropper('getCroppedCanvas').toBlob((getCropped) => {
                        formData.append('ImageUrl', getCropped);
                        $(data).each(function (index, element) {
                            formData.append(element.name, element.value);
                            //if (element.name == "Biography") {
                            //    var Biography = CKEDITOR.instances.Biography.getData();
                            //    formData.append('Bio', Biography);
                            //}
                            //else {
                            //    formData.append(element.name, element.value);
                            //}
                        });
                        submitSponsorForm(f, formData);
                    });
                } else {
                    formData = new FormData($('form').get(0));
                    formData.append('ImageUrl', $("#hdnImageUrl").val());
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                        //if (element.name == "Biography") {
                        //    var Biography = CKEDITOR.instances.Biography.getData();
                        //    formData.append('Bio', Biography);
                        //}
                        //else {
                        //    formData.append(element.name, element.value);
                        //}
                    });
                    submitSponsorForm(f, formData);
                }
            }
        });
    }

    var submitSponsorForm = function (f, formData) {
        $.ajax({
            type: f[0].method,
            url: f[0].action,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                if (IsNewSave == 1) {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Sponsor/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Sponsor/Upsert/" + data.OptionalValue;
                }
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
                formData.append('SponsorId', $("#Id").val());
            }
            $.ajax({
                type: 'POST',
                url: '/Sponsor/SaveSponsorFile',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data, strStatus) {
                    $('#cropImageContainer').show();
                    $('#croppableImage').attr('src', data.Data.replace("~", "")).width(150).height(150);
                    $croppableImage = $('#croppableImage');
                    $('#uploadFile').show();
                    $croppableImage.cropper({
                        viewMode: 1,
                        aspectRatio: 8 / 9,
                        minContainerHeight: 400,
                        crop: function (event) {
                        }
                    });
                },
                error: handleAjaxError()
            });
        }
    }


    this.init = function () {
        
        CKEDITOR.config.entities = false;
        CKEDITOR.config.htmlEncodeOutput = false;
        CKEDITOR.config.autoParagraph = false;

        upsertSponsorForm();
        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Sponsor/index";
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;


            }
        });
    }
}

