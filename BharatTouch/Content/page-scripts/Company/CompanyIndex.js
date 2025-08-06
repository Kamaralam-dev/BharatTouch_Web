var listCompany = function () {
    var me = this;

    function onSaveCompanyContact() {

        $("#contactInfoCompanyForm").validate({
            rules: {
                CompanyName: { required: true },
                Phone: {
                    required: true,
                    minNumberlength: { countrySelector: "#ddlPhoneCountryId" },
                    maxNumberlength: { countrySelector: "#ddlPhoneCountryId" }
                },
                Email: { required: true },
            },
            submitHandler: function (form) {
                debugger
                function handleCountryId(name, dropdownSelector) {
                    debugger;
                    var code = $(dropdownSelector).val();// countryId;10;10
                    if (code.split(';').length == 3) {
                        var id = code.split(';')[0];
                        var obj = { name: name, value: id };
                        return obj;
                    }
                }

                var f = $(form);
                var data = f.serializeArray();

                data = data.map(function (item, i) {
                    switch (item.name) {
                        case "PhoneCountryId":
                            return handleCountryId(item.name, "#ddlPhoneCountryId");
                        default:
                            return item;
                    }
                }).filter(x => x !== undefined && x !== null);
                var isIndiaSelected = false;
                var countryId = data.find(x => x.name == "PhoneCountryId")?.value;
                if (countryId != null && countryId != undefined) {
                    isIndiaSelected = parseInt(countryId, 10) == 5;
                }

                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {

                        showMessage((data.Success ? "Success" : "Failed"), data.Data, data.Type);
                        setTimeout(function () {

                            if (data.Success) {
                                window.location.reload();
                            }
                        }, 500)
                    },
                    error: handleAjaxError()
                });
            }
        });
    }

    function onSaveCompanySocialMedia() {

        $("#socialMediaInfoCompanyForm").validate({
            submitHandler: function (form) {

                var f = $(form);
                var data = f.serializeArray();

                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {

                        showMessage((data.Success ? "Success" : "Failed"), data.Data, data.Type);
                        setTimeout(function () {

                            if (data.Success) {
                                window.location.reload();
                            }
                        }, 500)
                    },
                    error: handleAjaxError()
                });
            }
        });
    }

    function onSaveCompanyAbout() {

        $("#aboutInfoCompanyForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                AboutDescription: {
                    required: true
                },
            },
            submitHandler: function (form) {
                debugger;
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                formData = new FormData();
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });


                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data, strStatus) {

                        showMessage((data.Success ? "Success" : "Failed"), data.Data, data.Type);
                        setTimeout(function () {

                            if (data.Success) {
                                window.location.reload();
                            }
                        }, 500)
                    },
                    error: handleAjaxError((ex) => {
                        debugger;
                    })
                });
            }
        });
    }

    function onSelectCoverImageFile() {
        var fileInput = document.getElementById('CoverImagepath');
        var file = fileInput.files[0];

        if (file) {
            var reader = new FileReader();

            reader.onload = function (event) {
                Swal.fire({
                    title: 'Crop Your Cover Image',
                    html: `
                    <div>
                        <img id="imagePreview" src="${event.target.result}" style="width: 100%; height: auto;"/>
                    </div>
                    <div class="d-flex justify-content-center mt-3">
                        <button id="uploadButton" class="btn btn-primary me-2">
                            <div class="lds-dual-ring" style="display:none; margin-right: 8px;"></div> Upload
                        </button>
                        <button id="cancelButton" class="btn btn-secondary">Cancel</button>
                    </div>
                `,
                    showConfirmButton: false,
                    showCancelButton: false,
                    // customClass use karein modal ko responsive banane ke liye
                    customClass: {
                        popup: 'swal-wide'
                    },
                    didOpen: () => {
                        const image = document.getElementById('imagePreview');
                        const cropper = new Cropper(image, {
                            aspectRatio: 16 / 7.5,
                            viewMode: 1,
                            autoCropArea: 1, // Yeh crop area ko image ke 100% par set kar dega
                            cropBoxResizable: false, // Ise false rakha hai aapke purane code ke hisab se
                            zoomable: true // Zoom ko enable rakha hai
                        });


                        document.getElementById('uploadButton').addEventListener('click', function () {
                            const btn = this;
                            const spinner = btn.querySelector('.lds-dual-ring');

                            spinner.style.display = 'inline-block';
                            btn.disabled = true;

                            const croppedCanvas = cropper.getCroppedCanvas();
                            if (croppedCanvas) {
                                croppedCanvas.toBlob(function (blob) {
                                    onSaveCompanyCoverImage(blob).finally(() => {
                                        spinner.style.display = 'none';
                                        btn.disabled = false;
                                        Swal.close(); // Close modal after upload
                                    });
                                }, 'image/jpeg');
                            } else {
                                spinner.style.display = 'none';
                                btn.disabled = false;
                            }
                        });

                        document.getElementById('cancelButton').addEventListener('click', function () {
                            Swal.close();
                        });
                    }
                });
            };

            reader.readAsDataURL(file);
        }
    }

    var ClientTestimonialForm = function () {

        $("#ClientTestimonialForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'ClientName' || element[0].name == 'Designation' || element[0].name == 'UserId' ||
                    element[0].name == 'CompanyName' || element[0].name == 'Testimonial' || element[0].name == 'CLientImagePath') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                ClientName: { required: true },
                //Designation: { required: true },
                //CompanyName: { required: true },
                Testimonial: { required: true },
                CLientImagePath: {
                    required: () => {
                        var pic = getFormInput("ClientTestimonialForm", "PicOfClient").val();
                        if (pic != undefined && pic != null && pic.trim() != "") {
                            return false;
                        } else {
                            return true;
                        }
                    }
                }
            },
            submitHandler: function (form) {
                var f = $(form);
                const myForm = document.getElementById("ClientTestimonialForm");
                var formData = new FormData(myForm);
                debugger;
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);

                $.ajax({
                    type: 'Post',
                    url: f[0].action,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        resetClientTestimonialForm();
                        bindClientTestimonialImages();
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });
    }

    function resetClientTestimonialForm() {
        var formId = "ClientTestimonialForm";
        var userId = $("#hdnLoggedUserId").val();
        getFormInput(formId, "UserId").val(userId);
        getFormInput(formId, "Client_Id").val("");
        getFormInput(formId, "ClientName").val("");
        getFormInput(formId, "Designation").val("");
        getFormInput(formId, "CompanyName").val("");
        $("#txtTestimonial").val("");
        getFormInput(formId, "PicOfClient").val("");
        getFormInput(formId, "CLientImagePath").val("");
        $("#btnSubmitTestimonialText").html("Add Testimonial");
    }

    var bindClientTestimonialImages = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Company/BindClientTestimonials',
            dataType: 'html',
            success: function (data, strStatus) {
                debugger;
                $("#ClientTestimonialsContainer").html(data);
                $(".btnClientTestimonialDelete").click(function () {
                    var clientId = $(this).data('id');
                    Swal.fire({
                        title: 'Are you sure?',
                        text: "Do you really want to delete this?",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        confirmButtonText: 'Yes, remove it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            deleteClientTestimonial(clientId)
                        }

                    });
                });
                $(".btnClientTestimonialEdit").click(function () {
                    var clientId = $(this).data('id');
                    editClickClientTestimonial(clientId);
                });
            },
            error: function () {
                console.error("Error while fetching Testimonial Images.")
            }
        });
    }

    function deleteClientTestimonial(clientId) {
        $.post(
            '/Company/DeleteClientTestimonial',
            { clientId: clientId, actionName: "BharatTouch/Company/CompanyIndex" },
            function (data, strStatus) {
                if (!data.Success) {
                    showMessage(data.Message, data.Data, data.Type, 2000);
                    return;
                }
                bindClientTestimonialImages();
            }
        ).fail(handleAjaxError());
    }
    function editClickClientTestimonial(clientId) {
        $.get(
            '/Company/GetClientTestimonialBy_Id',
            { clientId: clientId, actionName: "BharatTouch/Company/CompanyIndex" },
            function (data) {
                if (!data.Success) {
                    showMessage(data.Message, data.Data, data.Type, 2000);
                    return;
                }
                var formId = "ClientTestimonialForm";
                getFormInput(formId, "UserId").val(data.Data.UserId);
                getFormInput(formId, "Client_Id").val(data.Data.Client_Id);
                getFormInput(formId, "ClientName").val(data.Data.ClientName);
                getFormInput(formId, "Designation").val(data.Data.Designation);
                getFormInput(formId, "CompanyName").val(data.Data.CompanyName);
                $("#txtTestimonial").val(data.Data.Testimonial);
                getFormInput(formId, "PicOfClient").val(data.Data.PicOfClient);
                getFormInput(formId, "CLientImagePath").val("");
                $("#btnSubmitTestimonialText").html("Update Testimonial");
            }
        ).fail(handleAjaxError());
    }

    function onSaveCompanyCoverImage(blob) {
        var formData = new FormData();
        formData.append('CoverImagePath', blob);
        $.ajax({
            type: "POST",
            url: "/Company/SaveOrUpdateCoverImageDetail",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                showMessage(data.Success ? "Success" : "Failed", data.Data, data.Type);
                if (data.Success) {
                    setTimeout(() => window.location.href = window.location.href, 500);
                }
            },
            error: function (xhr, status, error) {
                console.error("Ajax error:", error);
            }
        });

        return false;
    }

    $("#btnRemoveCoverImg").on("click", function () {
        // Show confirm popup
        Swal.fire({
            title: 'Are you sure?',
            text: "Do you really want to remove the cover image?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, remove it!'
        }).then((result) => {
            if (result.isConfirmed) {
                var id = $(this).data("id");

                $.ajax({
                    type: "POST",
                    url: "/Company/DeleteCompanyCoverImage",
                    data: { id: id },
                    dataType: 'json',
                    success: function (data) {
                        showMessage(data.Success ? "Success" : "Failed", data.Data, data.Type);
                        if (data.Success) {
                            setTimeout(() => window.location.reload(), 500);
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Ajax error:", error);
                    }
                });
            }
        });
    });

    $("#btnSubmitPortfolioForm").on("click", function (e) {
        e.preventDefault();

        var form = $("#PortfolioImageCompanyForm");

        if (!form.data('validator')) {
            form.validate({
                errorPlacement: function (error, element) {
                    error.appendTo(element.parent());
                },
                rules: {
                    PortfolioImage: {
                        required: true
                    }
                }
            });
        }

        if (form.valid()) {
            var PortfolioImageForm = document.getElementById("PortfolioImageCompanyForm");
            var formData = new FormData(PortfolioImageForm);

            var submitBtn = $(this);
            submitBtn.find("div.lds-dual-ring").css('display', 'inline-block');
            submitBtn.prop('disabled', true);

            $.ajax({
                type: 'POST',
                url: form.attr('action'),
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {
                    $("#PortfolioImage").val('');
                    showMessage(data.Message, data.Data, data.Type);
                    bindPortfolioImages();

                    submitBtn.find("div.lds-dual-ring").css('display', 'none');
                    submitBtn.prop('disabled', false);
                },
                error: handleAjaxError(function () {
                    submitBtn.find("div.lds-dual-ring").css('display', 'none');
                    submitBtn.prop('disabled', false);
                })
            });
        }
    });

    var bindPortfolioImages = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Company/BindPortfolioImages',
            dataType: 'html',
            success: function (data, strStatus) {
                var fileInput = getFormInput("PortfolioImageForm", "PortfolioImage");
                var btn = $("#btnSubmitPortfolioForm");
                $("#portfolioImageContainer").html(data);
                $(".btnPortfolioDelete").click(function () {
                    var path = $(this).data('path');
                    var that = this;

                    Swal.fire({
                        title: 'Are you sure?',
                        text: "Do you really want to remove image?",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        confirmButtonText: 'Yes, remove it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            deletePortfolioImage(path);
                        }

                    });
                });

            },
            error: handleAjaxError(null, "Error While fetching Gallery Images")
        });
    }

    function getFormInput(formId, inputName) {
        return $('#' + formId).find(`input[name=${inputName}]`)
    }

    var deletePortfolioImage = function (filepath) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { path: filepath },
            url: '/Company/DeletePortfolioImage',
            dataType: 'html',
            success: function (data, strStatus) {
                bindPortfolioImages();
            },
            error: handleAjaxError()
        });
    }

    //youtube
    var bindYouTube = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Company/BindYouTubeMethod',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#youtubeListContainer").html(data);
                $(".edit-youtube").click(function () {
                    var YouTubeId = $(this).data('id');
                    openYouTubeModel(YouTubeId);
                });
                $(".delete-youtube").click(function () {
                    var YouTubeId = $(this).data('id');
                    var obj = $(this);

                    Swal.fire({
                        title: 'Are you sure?',
                        text: "Do you really want to delete this?",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        confirmButtonText: 'Yes, remove it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            deleteYouTube(YouTubeId, obj)
                        }

                    });

                });

            },
            error: handleAjaxError(null, "Error while fetching Youtube videos list.")
        });
    }

    var openYouTubeModel = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: '/Company/OpenYouTubeModelMethod',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#youtubeFormContainer").html('');
                $("#youtubeFormContainer").append(data);
                saveYouTube();
                $("#btnCancelYouTube").click(function () {
                    openYouTubeModel(0);
                });
                $("#btnYoutubeSave").click(function () {
                    return validateSectionYoutubeUrl();
                });

            },
            error: function () {
                console.error("Error while initializig Youtube Video Form.")
            }
        });
    }

    var saveYouTube = function () {
        $("#upsertYouTubeCompanyForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                YouTubeTitle: { required: true },
                YouTubeCategory: { required: true },
                YouTubeUrl: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);

                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: f.serializeArray(),
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        bindYouTube();
                        openYouTubeModel(0);
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        clearYouTubeForm();
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });

    }

    var deleteYouTube = function (id, obj) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: '/Company/DeleteYouTubeMethod',
            dataType: 'json',
            success: function (data, strStatus) {
                bindYouTube();
                showMessage(data.Message, data.Data, data.Type);
                $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                $(obj).removeClass("disable-click");
                clearYouTubeForm();
            },
            error: handleAjaxError(function () {
                $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                $(obj).removeClass("disable-click");
            })
        });


    }

    var clearYouTubeForm = function () {
        $("#YouTubeTitle").val('');
        $("#YouTubeUrl").val('');
    }
    //youtube end

    //blogs
    var bindBlog = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Company/BindBlogMethod',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#blogListContainer").html(data);
                $(".edit-blog").click(function () {
                    var blogId = $(this).data('id');
                    openBlogModel(blogId);
                });
                $(".delete-blog").click(function () {
                    var blogId = $(this).data('id');

                    Swal.fire({
                        title: 'Are you sure?',
                        text: "Do you really want to delete this?",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        confirmButtonText: 'Yes, remove it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            deleteBlog(blogId, this);
                        }

                    });
                });
            },
            error: handleAjaxError(null, "Error While fetching Blog list.")
        });
    }

    var openBlogModel = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: '/Company/OpenBlogModelMethod',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#blogFormContainer").html('');
                $("#blogFormContainer").append(data);
                saveBlog();
                $("#btnCancelBlog").click(function () {
                    openBlogModel(0);
                });
            },
            error: function () {
                console.error("Error while initialized Blog Form.")
            }
        });
    }

    var saveBlog = function () {

        $("#upsertBlogCompanyForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                BlogTitle: { required: true },
                BlogCategory: { required: true },
                BlogUrl: { required: true }
            },
            submitHandler: function (form) {
                debugger
                var f = $(form);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);
                var data = f.serializeArray();
                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        bindBlog();
                        openBlogModel(0);
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        clearBlogForm();
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });
    }

    var deleteBlog = function (id, obj) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: '/Company/DeleteBlogMethod',
            dataType: 'json',
            success: function (data, strStatus) {
                bindBlog();
                showMessage(data.Message, data.Data, data.Type);
                $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                $(obj).removeClass("disable-click");
                clearBlogForm();
            },
            error: handleAjaxError(function () {
                $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                $(obj).removeClass("disable-click");
            })
        });
    }

    var clearBlogForm = function () {
        $("#BlogTitle").val('');
        $("#BlogCategory").val('');
        $("#BlogUrl").val('');
    }
    //blog end

    var changeCompanyAdminPassword = function () {
        $("#changePasswordCompanyForm").validate({
            errorPlacement: function (error, element) {
                var elements = ['Password', 'ConfirmPassword'];
                if (!!(elements.find(x => x == element[0].name))) {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }

            },
            rules: {
                Password: {
                    required: true,
                    customRegex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&#]{8,16}$/
                },
                ConfirmPassword: {
                    required: true,
                    equalTo: "#Password"
                },
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

                        showMessage(data.Message, data.Data, data.Type);
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        getFormInput("changePasswordCompanyForm", "Password").val("");
                        getFormInput("changePasswordCompanyForm", "ConfirmPassword").val("");
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });
    }

    this.init = function () {

        ClassicEditor
            .create(document.querySelector('#AboutDescription'))
            .then(editor => {
                editor.ui.view.editable.element.style.height = '300px;';
                console.log('CKEditor 5 is ready.');
            })
            .catch(error => {
                console.error('There was an error initializing CKEditor 5:', error);
            });

        $("#btnClientTestimonialEditCancel").on('click', resetClientTestimonialForm);

        $("#CoverImagepath").on("change", function () {
            onSelectCoverImageFile();
        });

        onSaveCompanyContact();
        onSaveCompanySocialMedia();
        onSaveCompanyAbout();
        changeCompanyAdminPassword();
        ClientTestimonialForm();

        bindPortfolioImages();
        openYouTubeModel();
        bindYouTube();
        openBlogModel();
        bindBlog();
        bindClientTestimonialImages();



        jQuery.validator.addMethod("minNumberlength", function (value, element, params) {
            var countryValue = $(params.countrySelector).val();
            if (params.required != undefined && params.required == false) {
                if (value.trim() == "") {
                    return true;
                }
            }
            if (!countryValue) return false;
            var splittedArr = countryValue.split(";");
            var minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return value.length >= minLength;
        }, function (params, element) {
            var countryValue = $(params.countrySelector).val();
            var splittedArr = countryValue.split(";");
            var minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return `Minimum ${minLength} digits required.`;
        });

        jQuery.validator.addMethod("maxNumberlength", function (value, element, params) {
            var countryValue = $(params.countrySelector).val();
            if (params.required != undefined && params.required == false) {
                if (value.trim() == "") {
                    return true;
                }
            }
            if (!countryValue) return false;

            var splittedArr = countryValue.split(";");
            var maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return value.length <= maxLength;
        }, function (params, element) {
            var countryValue = $(params.countrySelector).val();
            var splittedArr = countryValue.split(";");
            var maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return `Maximum ${maxLength} digits required.`;
        });
    }

}