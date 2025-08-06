var upsertAccount = function () {
    var me = this;
    this.upsertAccountUrl = null;
    this.deleteAccountPhoneUrl = null;
    this.deleteAccountUrl = null;
    var $croppableImage = null;
    var IsNewSave = 0;
    this.loggedUser = null;

    var upsertAccountForm = function () {
        $("#upsertAccountForm").validate({
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
                OwnerId: { required: true },
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
                        });

                        submitAccountForm(f, formData);
                    });
                }
                else {
                    formData = new FormData($('form').get(0));
                    formData.append('ImageUrl', $("#hdnImageUrl").val());
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                    });
                    submitAccountForm(f, formData);
                }
            }
        });
    }

    var submitAccountForm = function (f, formData) {
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
                    window.location.href = "/Account/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Account/Upsert/" + data.OptionalValue;
                }
            },
            error: handleAjaxError()
        });
    }

    var addPhoneType = function () {
        CloneRows('#multiPhoneTable');
    }

    var deleteAccountPhone = function (object, id) {

        var bodyRows = $("#multiPhoneTable tbody tr").length;
        if (bodyRows == 1 && id == 0) {
            window.location.reload();
        }

        if (id > 0) {
            /*if contact detail saved in db,delete it from db directy*/
            $.ajax({
                type: 'GET',
                url: me.deleteAccountPhoneUrl,
                data: { id: id },
                dataType: 'json',
                success: function (data, strStatus) {
                    $(object).closest('tr').remove();
                    showMessage(data.Message, data.Data, data.Type);
                    if (bodyRows == 1) {
                        window.location.reload();
                    }
                    else {
                        setRowIndex('#multiPhoneTable');
                    }
                }
            });
        }
        else {
            /*if contact detail not exists in db,delete it from html table only*/
            $(object).closest('tr').remove();
            setRowIndex('#multiPhoneTable');
        }
    }

    var addSocialType = function () {
        CloneRows('#multiSocialTable');
    }

    var deleteAccountSocial = function (object, id) {
        var bodyRows = $("#multiSocialTable tbody tr").length;
        var bodyRows = $("#multiPhoneTable tbody tr").length;
        if (bodyRows == 1 && id == 0) {
            window.location.reload();
        }
        if (id > 0) {
            /*if contact detail saved in db,delete it from db directy*/
            $.ajax({
                type: 'GET',
                url: me.deleteAccountSocialUrl,
                data: { id: id },
                dataType: 'json',
                success: function (data, strStatus) {
                    $(object).closest('tr').remove();
                    showMessage(data.Message, data.Data, data.Type);
                    if (bodyRows == 1) {
                        window.location.reload();
                    }
                    else {
                        setRowIndex('#multiSocialTable');
                    }
                }
            });
        }
        else {
            /*if contact detail not exists in db,delete it from html table only*/
            $(object).closest('tr').remove();
            setRowIndex('#multiSocialTable');
        }
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
            }
            $.ajax({
                type: 'POST',
                url: '/Account/SaveTempFile',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data, strStatus) {
                    $('#cropImageContainer').show();
                    $('#croppableImage').attr('src', data.Data.replace("~", "")).width(150).height(150);
                    $croppableImage = $('#croppableImage');
                    $croppableImage.cropper({
                        viewMode: 3,
                        aspectRatio: 16 / 9,
                        background: false,
                        minContainerHeight: 208,
                        minContainerWidth: 390,
                        crop: function (event) {
                        }
                    });
                },
                error: handleAjaxError()
            });
        }
    }

    var saveNotes = function (noteId, accountId) {
        //var notes = $("#AccountNotes").val();
        var notes = CKEDITOR.instances.AccountNotes.getData();
        if (notes != "") {
            $.ajax({
                type: 'GET',
                cache: false,
                data: { noteId: noteId, accountId: accountId, notes: notes },
                url: '/Account/SaveNotes',
                dataType: 'json',
                success: function (data, strStatus) {
                    showMessage(data.Message, data.Data, data.Type);
                    bindNotes(accountId);
                    //$("#AccountNotes").val("");
                    CKEDITOR.instances.AccountNotes.setData('');
                    $("#hdnNoteId").val("0");
                },
                error: handleAjaxError()
            });
        }
        else {
            showMessage("Failed", "Notes is required.", "error");
        }
    }

    //note is activity
    var bindNotes = function (accountId) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { accountId: accountId },
            url: '/Account/BindNotes',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#noteList").html(data);
            },
            error: handleAjaxError()
        });
    }


    var getNotes = function (accountNoteId) {
        $("#hdnNoteId").val(accountNoteId);
        $.ajax({
            type: 'GET',
            cache: false,
            data: { accountNoteId: accountNoteId },
            url: '/Account/GetNotes',
            dataType: 'json',
            success: function (data, strStatus) {
                //$("#AccountNotes").val(data.Data);
                CKEDITOR.instances.AccountNotes.setData(data.Data);
            },
            error: handleAjaxError()
        });
    }

    var DeleteNotification = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: '/Account/AccountDeleteNotification',
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                OpenModel("AccountDeleteModal");
                $('#AccountDeleteContainer').on("click", "#IsAccountDeleteGranted", function () {
                    $.ajax({
                        type: 'POST',
                        cache: false,
                        data: { id: id },
                        url: me.deleteAccountUrl,
                        dataType: 'json',
                        success: function (data, strStatus) {
                            showMessage(data.Message, data.Data, data.Type);
                            CloseModal();
                            //$('#tableAccounts').DataTable().ajax.reload();
                        },
                        error: handleAjaxError()
                    });
                });
            },
            error: handleAjaxError()
        });
    }

    var deleteNotes = function (noteId, accountId) {
        var r = confirm("Are you sure to delete this document!");
        if (r == true) {
            $.ajax({
                type: 'POST',
                cache: false,
                data: { accountNoteId: noteId },
                url: '/Account/DeleteNote',
                dataType: 'json',
                success: function (data, strStatus) {
                    showMessage(data.Message, data.Data, data.Type);
                    bindNotes(accountId);
                },
                error: handleAjaxError()
            });
        }
    }

    var deleteDocument = function (documentId, accountId, docname) {
        var r = confirm("Are you sure to delete this document!");
        if (r == true) {
            $.ajax({
                type: 'POST',
                cache: false,
                data: { documentId: documentId, accountid: accountId, docname: docname },
                url: '/Account/DeleteDocument',
                dataType: 'json',
                success: function (data, strStatus) {
                    showMessage(data.Message, data.Data, data.Type);
                    bindNotes(accountId);
                },
                error: handleAjaxError()
            });
        }
      

    }

    this.init = function () {

        CKEDITOR.replace('AccountNotes');
        CKEDITOR.config.entities = false;
        CKEDITOR.config.htmlEncodeOutput = false;
        

        /*default owner will be selected*/
        if ($("#AccountId").val() == "0") {
            $("#OwnerId").val(me.loggedUser);
        }

        upsertAccountForm();
        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Account";
                    break;
                case "Delete":
                    DeleteNotification(id);
                    break;
                case "addPhoneTypes":
                    addPhoneType();
                    break;
                case "deleteRow":
                    deleteAccountPhone(this, id);
                    break;
                case "addSocialTypes":
                    addSocialType();
                    break;
                case "deleteRowSocial":
                    deleteAccountSocial(this, id);
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;
                case "savenotes":
                    saveNotes($("#hdnNoteId").val(), id);
                    break;
                case "editnotes":
                    getNotes(id);
                    break;
                case "deletenote":
                    var accountid = $(this).data('accountid');
                    deleteNotes(id, accountid);
                    break;
                case "cancelnote":
                    $("#hdnNoteId").val("0");
                    //$("#AccountNotes").val("");
                    CKEDITOR.instances.AccountNotes.setData('');
                    break;
                case "deletedocument":
                    var accountid = $(this).data('accountid');
                    var docname = $(this).data('docname');
                    deleteDocument(id, accountid, docname);
                    break;
                case "downloadfile":
                    var dataPath = $(this).data('path');
                    downloadFileByPath(dataPath);
                    break;
            }
        });

        // downloadFileByPath(data.Data);

        $('#IsSameAsBillingAddress').on('change', function () {
            if ($(this).is(':checked')) {
                var Address1 = $('#Address1').val(),
                    Address2 = $('#Address2').val(),
                    City = $('#City').val(),
                    State = $('#State').val(),
                    Zip = $('#Zip').val(),
                    CountryId = $('#CountryId option:selected').val();

                $('#ShippingAddress1').val(Address1);
                $('#ShippingAddress2').val(Address2);
                $('#ShippingCity').val(City);
                $('#ShippingState').val(State);
                $('#ShippingZip').val(Zip);
                $('#ShippingCountryId').val(CountryId);
            }
            else {
                $('#ShippingAddress1').val('');
                $('#ShippingAddress2').val('');
                $('#ShippingCity').val('');
                $('#ShippingState').val('');
                $('#ShippingZip').val('');
                $('#ShippingCountryId').val(0);
            }
        });

        $('#btnUploadDoc').click(function () {
            debugger;
            // Checking whether FormData is available in browser  
            if (window.FormData !== undefined) {

                var fileUpload = $("#DocFileSelect").get(0);
                var files = fileUpload.files;

                // Create FormData object  
                var fileData = new FormData();

                // Looping over all files and add it to FormData object  
                for (var i = 0; i < files.length; i++) {
                    fileData.append(files[i].name, files[i]);
                }

                // Adding one more key to FormData object  
                fileData.append('DocDescription', $("#DocDescription").val());
                fileData.append('accountId', $("#AccountId").val());

                $.ajax({
                    url: '/Account/UploadActvityFiles',
                    type: "POST",
                    contentType: false, // Not to set any content header  
                    processData: false, // Not to process data  
                    data: fileData,
                    success: function (result) {
                     
                        bindNotes($("#AccountId").val());
                        if (result == "0") {
                            showMessage("Success", "Document uploaded successfully.", "success");
                            $("#DocDescription").val('');
                            $("#DocFileSelect").val('');
                        }
                        
                    },
                    error: function (err) {
                        alert(err.statusText);
                    }
                });
            } else {
                alert("FormData is not supported.");
            }
        });
    }
}

