var upsertItem = function () {
    var me = this;
    this.upsertItemUrl = null;
    this.deleteItemUrl = null;
    var IsNewSave = 0;
    this.loggedUser = null;
    var $croppableImage = null;

    var upsertItemForm = function () {
        $("#upsertItemForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                SubCategoryId: { required: true },
                ItemName: { required: true },
                CategoryId: { required: true },
                DonerId: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;

                //if ($croppableImage != undefined) {
                //    formData = new FormData();
                //    $croppableImage.cropper('getCroppedCanvas').toBlob((getCropped) => {
                //        formData.append('ImageUrl', getCropped);
                //        $(data).each(function (index, element) {
                //            formData.append(element.name, element.value);
                //        });
                //        submitItemForm(f, formData);
                //    });
                //} else {
                    formData = new FormData($('form').get(0));
                    formData.append('ImageUrl', $("#hdnImageUrl").val());
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                    });
                    submitItemForm(f, formData);
                //}
            }
        });
    }

    var submitItemForm = function (f, formData) {
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
                    window.location.href = "/Item/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Item/Upsert/" + data.OptionalValue;
                }
            },
            error: handleAjaxError()
        });
    }


    $("#CategoryId").on("change", function () {
        var CategoryId = $("#CategoryId :selected").val();
        if (CategoryId != '') {
            $.ajax({
                type: "GET",
                cache: false,
                data: { CategoryId: CategoryId },
                url: '/Item/GetSubCategoryList',
                dataType: 'json',
                success: function (data, strStatus) {
                    $("#SubCategoryId").empty();
                    $("#SubCategoryId").append($("<option></option>").val('').html("Select one"));
                    $.each(data, function (key, value) {
                        for (var i = 0; value.length > i; i++) {
                            $("#SubCategoryId").append($("<option></option>").val(value[i].CategoryId).html(value[i].CategoryName));
                        }
                    });

                },
            });
        } else {
            $("#SubCategoryId").empty();
            $("#SubCategoryId").append($("<option></option>").val('').html("Select one"));
        }
    });

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
               // formData.append('ItemId', $("#ItemId").val());
                
            }
            $.ajax({
                type: 'POST',
                url: '/Item/SaveTempFile',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data, strStatus) {
                    if (data.Success) {
                        $('#cropImageContainer').show();
                        $('#croppableImage').attr('src', data.Data.replace("~", "")).width(150).height(150);
                        $croppableImage = $('#croppableImage');
                        $("#btnUploadImage").show();
                        $croppableImage.cropper({
                            viewMode: 2,
                            crop: function (event) {
                            }
                        });
                    }
                    else {
                        showMessage("Warning!", data.Message, "notice");
                        $("#ItemImageUrl").val('');
                    }
                },
                error: handleAjaxError()
            });
        }
    }

    var upsertItemImageForm = function () {
        $("#upsertItemImageForm").validate({
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
                        submitItemImageForm(f, formData);
                    });
                } else {
                    formData = new FormData($('form').get(0));
                    formData.append('ImageUrl', $("#hdnImageUrl").val());
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                    });
                    submitItemImageForm(f, formData);
                }
            }
        });
    }

    var submitItemImageForm = function (f, formData) {
        $.ajax({
            type: f[0].method,
            url: f[0].action,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                window.location.reload();
            },
            error: handleAjaxError()
        });
    }

    this.init = function () {

        $("#CategoryId").click(function () {
            var CategoryId = $("#CategoryId :selected").text();
            if (CategoryId == 'Others') {
                $(".SubCategoryField").hide();
                $(".ItemTypeField").show();
            }
            else {
                $(".SubCategoryField").show();
                $(".ItemTypeField").hide();
            }
        });

        upsertItemForm();
        upsertItemImageForm();

        if ($("#catId").val() != 0) {
            $("#CategoryId").val($("#catId").val()).click();
        }

        $('.delete-image').click(function () {
            var r = confirm("Are you sure to delete this image!");
            if (r == true) {
                var path = $(this).data('path');
                var formData;
                formData = new FormData();
                formData.append('path', path);
                //formData.append('OldImage', $(this).data('oldimage'));
                //formData.append('ItemId', $(this).data('id'));

                $.ajax({
                    type: 'POST',
                    url: '/Item/DeleteItem',
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data, strStatus) {
                        window.location.reload();
                    },
                    error: handleAjaxError()
                });
            }
        });

        $('.primary-image').click(function () {
            var path = $(this).data('filename');
            var id = $(this).data('id');

            $.ajax({
                type: 'POST',
                url: '/Item/SetPrimaryImage',
                data: { ItemId: id, filename: path},
                dataType: 'json',              
                success: function (data, strStatus) {
                    showMessage(data.Message, data.Data, data.Type);
                },
                error: handleAjaxError()
            });
        });

        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Item/index";
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;


            }
        });
    }
}

