var createUser = function () {
    var me = this;
    this.googleApiKey = null;
    this.googleAuthRedirectUri = null;
    this.googleClientId = null;
    this.pageUrl = null;
    this.WebUrl = null;
    this.createUserUrl = null;
    this.getScheduleOpenDaysUrl = null;
    this.nearByCompanyApiKey = null;
    this.coverImgApiKey = null;
    this.newsApiKey = null;
    this.bindCompanyTypeByParent = null;
    var isDuplicateEmail = false;
    var calendar = null;
    var openScheduleDates = null;
    var $croppablePaymentImage = null;
    var $croppableCoverImage = null;
    var multipleDatePickrInstance = null;
    var currentLat = null;
    var currentLon = null;
    var openWeekDays = [];
    var tokenClient = null;
    this.isTaxApplicable = false;
    this.orderUpsertFormValidator = null;
    this.nfcCardDetailFormValidator = null;
    this.orderPaymentFormValidator = null;
    this.RazorPayLatestOrderId = null;
    this.razorPayApiKey = null;
    this.razorPayInsatance = null;
    var w_current_fs, w_next_fs, w_previous_fs; //fieldsets
    var w_opacity;
    var w_current = 1;
    var w_steps = 0;
    this.w_next_formId = null;
    this.w_current_formId = null;

    //let currentStep = 1;
    this.isValidCoupon = false;
    var wizardData = {
        BasicData: {

        },
        payment: {
            InitialData: {

            },
            updatedData: {

            }
        }
    };

    var getFormInputByName = function (formId) {
        return (name) => getFormInput(formId, name);
    }

    function InitializeCommonDates(startDateSelector, EndDateSelector, format = "mm/yyyy", startView = "months", minViewMode = "months") {
        if (!$(startDateSelector).val().trim()) {
            $(EndDateSelector).val("").prop("disabled", true);
        } else {
            $(EndDateSelector).prop("disabled", false);
        }

        var options = { autoclose: true };
        if (format) {
            options.format = format;
        }
        if (startView) {
            options.startView = startView;
        }
        if (minViewMode) {
            options.minViewMode = minViewMode;
        }

        $(startDateSelector).datepicker(options).on('changeDate', function (selected) {
            let startDate = selected.date;
            if (startDate) {
                $(EndDateSelector).prop("disabled", false);
            } else {
                $(EndDateSelector).val("").prop("disabled", true);
            }
        });

        $(startDateSelector).on('input', function () {
            if (!this.value) { // If input is empty
                $(EndDateSelector).val("").prop("disabled", true);
            }
        });

        $(EndDateSelector).datepicker(options);

    }

    function removeAllModelDataKeys() {
        $.each($('#deleteModelData').data(), function (key) {
            $('#deleteModelData').removeData(key);
        });
    }

    function setAndShowDeleteModel(obj) {
        removeAllModelDataKeys();
        $("#deleteModelData").data(obj);
        $("#deleteModel").modal("show");
    }



    var createUserForm = function () {

        $("#CreateUserForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'FirstName' || element[0].name == 'EmailId' || element[0].name == 'Password') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                FirstName: { required: true, letterswithspace: true },
                EmailId: { required: true, emailvalidatecustom: true },
                Password: { required: true },
                ConfirmPassword: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;

                formData = new FormData($('form').get(0));

                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });
                submitUserForm(f, formData, isDuplicateEmail);

            }
        });
    }

    var submitUserForm = function (f, formData, isDuplicateEmail) {
        if (isDuplicateEmail && $("#UserId").val() == "0")
            return false;


        var Password = $("#txtRegPassword").val().trim();
        var confirmPassword = $("#txtRegConfirmPassword").val().trim();


        if (Password != confirmPassword) {
            showMessage("Warning!", "Password and confirm password not matched.", "notice");
            return false;
        }

        f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
        f.find(":submit").prop('disabled', true);
        $.ajax({
            type: f[0].method,
            url: f[0].action,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
                if (data.Success)
                    window.location.href = "/Users/Create/" + data.OptionalValue;
            },
            error: handleAjaxError(function () {
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
            })
        });
    }

    var personalInfoForm = function () {
        $("#PersonalInfoUserForm").on("keyup", "#FirstName", function () {
            $("#PreFirstName").html($(this).val());
        });

        $("#PersonalInfoUserForm").on("keyup", "#LastName", function () {
            $("#PreLastName").html($(this).val());
        });

        $("#PersonalInfoUserForm").on("keyup", "#Company", function () {
            $("#preCompanyName").html($(this).val());
        });
        
        $("#PersonalInfoUserForm").on("keyup", "#CurrentDesignation", function () {
            $("#preCurrentDesignation").html($(this).val());
        });

        //$("#PersonalInfoUserForm").on("keyup", "#Tagline", function () {
        //    var tagEle = $("#PreTagline");
        //    var tagVal = $(this).val();
        //    tagEle.data('typed-items', tagVal);
        //    //tagEle.parent().show();
        //    //tagEle.html(tagVal);
        //    //if (tagVal.trim() == 0) {
        //    //    tagEle.parent().hide();
        //    //}
        //});

        $("#PersonalInfoUserForm").on("keyup", "#Website", function () {
            var preWebEle = $("#PreWebsite");
            preWebEle.show();
            var website = $(this).val();
            if (website.trim() == 0) {
                preWebEle.hide();
            }
            //preWebEle.html(website.trim());
            if (preWebEle.is('a')) {
                if (website.startsWith("http")) {
                    preWebEle.attr("href", website.trim());
                    return;
                }
                preWebEle.attr("href", `http://${website.trim()}`);
            }
        });

        jQuery.validator.addMethod("notZero", function (value, element, param) {
            return value != "0"
        }, "Field is Required");

        $("#PersonalInfoUserForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'FirstName' || element[0].name == 'EmailId' || element[0].name == 'Company'
                    || element[0].name == 'CompanyType' || element[0].name == 'CompanyTypeId') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                FirstName: { required: true, letterswithspace: true },
                //Company: { required: true, message: "Business/Company name is required" }
                CompanyTypeParentId: {
                    required: true,
                    notZero: true
                },
                CompanyType: {
                    required: true
                    //letterswithspace: true
                },
                CompanyTypeId: {
                    required: true,
                    notZero: () => $("#ddlCompanyTypeId").val() != "36"
                }
            },
            submitHandler: function (form) {
                var f = $(form);
                const PersonalInfoUserForm = document.getElementById("PersonalInfoUserForm");
                var formData = new FormData(PersonalInfoUserForm);
                var data = f.serializeArray();
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);

                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        if (data.Success) {
                            window.location.reload();
                        }
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });
    }

    var paymentQrForm = function () {

        $("#paymentQrForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'UpiId' || element[0].name == 'PayeeName') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                UpiId: { required: true},
                PayeeName: { required: true}
            },
            message: {
                UpiId: {required: "UpiId is required."},
                PayeeName: { required: "Payee Name is required."}
            },
            submitHandler: function (form) {
                var f = $(form);
                const paymentQrForm = document.getElementById("paymentQrForm");
                var data = f.serializeArray();

                var formData;
                if ($croppablePaymentImage != undefined) {
                    formData = new FormData();
                    $croppablePaymentImage.cropper('getCroppedCanvas').toBlob((getCropped) => {
                        formData.append('paymentQr', getCropped);
                        $(data).each(function (index, element) {
                            formData.append(element.name, element.value);

                        });

                        submitPaymentQrForm(f, formData);
                    });
                }
                else {
                    formData = new FormData(paymentQrForm);
                    formData.append('paymentQr', $("#hdnImageUrl").val());
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                    });
                    submitPaymentQrForm(f, formData);
                }

            }
        });
    }

    onSelectPaymentFile = function (input) {

        if (input.files && input.files[0]) {
            $('#croppablePaymentImage').removeAttr('src');
            $('#croppablePaymentImage').cropper('destroy');
            var files = input.files;
            var formData = new FormData();
            if (files.length > 0) {
                for (var x = 0; x < files.length; x++) {
                    formData.append("file" + x, files[x]);
                }
            }
            $("#cropPaymentPrfLoader").css('display', 'inline-block');
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

                        $('#croppablePaymentImage').attr('src', data.Data.replace("~", "")).width(200).height(200);
                        $croppablePaymentImage = $('#croppablePaymentImage');
                        $croppablePaymentImage.cropper({
                            aspectRatio: 16 / 15
                        });
                        if ($('#croppablePaymentImage').attr('src') != '') {
                            $("#cropPaymentPrfLoader").css('display', 'none');
                            $('#cropPaymentImageContainer').show();
                        }
                    }
                    else {
                        $("#paymentQr").val('');
                        showMessage("Warning!", data.Data, "notice");
                        $("#cropPaymentPrfLoader").css('display', 'none');
                    }

                },
                error: handleAjaxError()
            });
        }
    }

    var submitPaymentQrForm = function (f, formData) {
        f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
        f.find(":submit").prop('disabled', true);

        $.ajax({
            type: f[0].method,
            url: f[0].action,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
                if (data.Success) {
                    window.location.reload();
                }
            },
            error: handleAjaxError(function () {
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
            })
        });
    }

    var contactInfoForm = function () {

        $("#ContactInfoUserForm").on("keyup", "#PersonalEmail", function () {
            var emailEle = $("#PreEmailId")
            var emailVal = $(this).val();

            emailEle.show();
            
            if (emailVal.trim().length == 0) {
                emailEle.attr("href", ``);
                emailEle.hide();
            } else {
                emailEle.attr("href", `mailto:${emailVal.trim()}`);
            }
        });

        $("#ContactInfoUserForm").validate({
            errorPlacement: function (error, element) {
                var elements = ['PersonalEmail', 'CountryId', 'Phone', 'WhatsAppCountryId', 'Whatsapp', 'WorkPhone', 'WorkPhoneCountryId', 'OtherPhoneCountryId', 'OtherPhone'];
                if (!!(elements.find(x => x == element[0].name))) {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }

            },
            rules: {
                PersonalEmail: { required: true },
                CountryId: { required: true },
                Phone: {
                    required: true,
                    minNumberlength: { countrySelector:"#ddlCountryId" },
                    maxNumberlength: { countrySelector:"#ddlCountryId" }
                },
                Whatsapp: {
                    required: false,
                    minNumberlength: { countrySelector: "#ddlWhatsAppCountryId", required: false },
                    maxNumberlength: { countrySelector: "#ddlWhatsAppCountryId", required: false }
                },
                WorkPhone: {
                    required: false,
                    minNumberlength: { countrySelector: "#ddlWorkPhoneCountryId", required: false },
                    maxNumberlength: { countrySelector: "#ddlWorkPhoneCountryId", required: false }
                },
                OtherPhone: {
                    required: false,
                    minNumberlength: { countrySelector: "#ddlOtherPhoneCountryId", required: false },
                    maxNumberlength: { countrySelector: "#ddlOtherPhoneCountryId", required: false }
                },
            },
            submitHandler: function (form) {
                function handleCountryId(name, dropdownSelector) {
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
                        case "CountryId":
                            return handleCountryId(item.name, "#ddlCountryId");
                        case "WhatsAppCountryId":
                            return handleCountryId(item.name, "#ddlWhatsAppCountryId");
                        case "WorkPhoneCountryId":
                            return handleCountryId(item.name, "#ddlWorkPhoneCountryId");
                        case "OtherPhoneCountryId":
                            return handleCountryId(item.name, "#ddlOtherPhoneCountryId");
                        default:
                            return item;
                    }
                });
                var isIndiaSelected = false;
                var countryId = data.find(x => x.name == "CountryId")?.value;
                if (countryId != null && countryId != undefined) {
                    isIndiaSelected = parseInt(countryId) == 5;
                }

                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        if (data.Success) {
                            if (isIndiaSelected) {
                                $(".adhaarSection").show();
                            } else {
                                $(".adhaarSection").hide();
                            }
                        }
                        showMessage(data.Message, data.Data, data.Type);
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

    function changePasswordForm() {
        $("#changePasswordForm").validate({
            errorPlacement: function (error, element) {
                var elements = ['UserId', 'Password', 'ConfirmPassword'];
                if (!!(elements.find(x => x == element[0].name))) {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }

            },
            rules: {
                UserId: { required: true },
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
                        if (data.Success) {
                            setTimeout(() => {
                                window.location.href = "/";
                            }, 500);
                        }
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });
    }

    function prependCountryCode() {
        var selectedOption = $('.ddlccode option:selected');
        var countryCode = selectedOption.val();
        var phone = $('#Phone').val();
        var phoneWithoutCode = phone.replace(/^(\+[\d]{1,3})/, '');

        $('#Phone').val(countryCode + ' ' + phoneWithoutCode.trim());
    }

    var aboutInfoForm = function () {

        $("#AboutInfoUserForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'AboutDescription' || element[0].name == 'SkillName1') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                AboutDescription: { required: true },
                //SkillName1: { required: true },
                //KnowledgePercent1: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                formData = new FormData();
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });

                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);

                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
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

    var bindProfessional = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#UserId").val() },
            url: '/Users/BindProfessional',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#prfessionalListContainer").html(data);
                $(".edit-professional").click(function () {
                    var professionalId = $(this).data('id');
                    openProfessionalModel(professionalId);
                });
                $(".delete-professional").click(function () {
                    var professionalId = $(this).data('id');
                    deleteExperience(professionalId, this);
                });
            },
            error: handleAjaxError(null, "Error while fetching Experience List")
        });
    }

    var openProfessionalModel = function (professionId) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: professionId },
            url: '/Users/OpenProfessionalModel',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#professionalFormContainer").html('');
                $("#professionalFormContainer").append(data);

                InitializeCommonDates("#profStartDate", "#profEndDate");

                saveProfessional();

                $("#btnCancelExperience").click(function () {
                    openProfessionalModel(0);
                });
            },
            error: function () {
                console.error("Error while initialized Professional Form");
            }
        });
    }

    var deletePersonalFile = function (type, path) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { type: type, path: path },
            url: '/Users/DeletePersonalFile',
            success: function (data, strStatus) {

                //saveProfessional();
            },
            error: handleAjaxError()
        });
    }

    var saveProfessional = function () {

        $("#upsertProfessioanlForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                Title: { required: true },
                Company: { required: true },
                StartDate: { required: true },
                City: { required: true },
                //StateName: { required: true },
                EndDate: {
                    required: false,
                    greaterThanStartDate: {
                        elem: "#profStartDate",
                        format: "MM/YYYY"
                    }
                },
                Country: { required: true }
            },
            messages: {
                EndDate: {
                    greaterThanStartDate: "End Date cannot be earlier than Start Date"
                }
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
                        bindProfessional();
                        clearProfessionalForm();
                        InitializeCommonDates("#profStartDate", "#profEndDate");
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


        $("#AddProfessionalModal")
            .modal({ show: true, backdrop: false })
            .on('click', '.mfp-close', function (e) {
                CloseModal();
            });
    }

    function maniPulatePreviewLink(ele, val) {
        ele.show();

        if (val.trim() === "") {
            ele.hide();
            ele.attr('href', val.trim());
            return;
        }

        if (val.trim().startsWith("http")) {
            ele.attr("href", val.trim());
            return;
        }
        ele.attr("href", `http://${val.trim() }`);
    }

    var saveSocialMedia = function () {

        $("#SocialInfoUserForm").on("keyup", "#LinkedIn", function () {
            var linkEle = $("#PreLinkedInLink");
            var value = $(this).val();
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").on("keyup", "#Twitter", function () {
            var linkEle = $("#PreTwitterLink");
            var value = $(this).val();
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").on("keyup", "#Facebook", function () {
            var linkEle = $("#PreFacebookLink");
            var value = $(this).val();
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").on("keyup", "#Instagram", function () {
            var linkEle = $("#PreInstagramLink");
            var value = $(this).val();
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").on("keyup", "#Skype", function () {
            var linkEle = $("#PreSkypeLink");
            var value = $(this).val();
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").on("keyup", "#Youtube1", function () {
            var linkEle = $("#PreYoutubeLink");
            var value = $(this).val();
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").on("keyup", "#Teams", function () {
            var linkEle = $("#PreTeamsLink");
            var value = $(this).val();
            if (value != null && value.trim() != "") {
                value = "sip:" + value;
            }
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").on("keyup", "#Snapchat", function () {
            var linkEle = $("#PreSnapchatLink");
            var value = $(this).val();
            maniPulatePreviewLink(linkEle, value);
        });

        $("#SocialInfoUserForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                LinkedIn: { required: false },

            },
            submitHandler: function (form) {
                var f = $(form);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);
                var data = f.serializeArray().map((item, i) => {
                    if (item.name == "Youtube1") {
                        var obj = { name: "Youtube", value: item.value };
                        return obj;
                    }
                    return item;
                });
                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
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

    var clearProfessionalForm = function () {
        $("#profTitle").val('');
        $("#profCompany").val('');
        $("#profStartDate").val('');
        $("#profEndDate").val('');
        $("#profCity").val('');
        $("#profState").val('');
        $("#profCountry").val('');
        $("#profWebsite").val('');
        $("#ProfessionId").val('');
        $("#profZip").val('');
        $("#profPhone").val('');
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
                $("#educationListContainer").html(data);
                $(".edit-education").click(function () {
                    var educationid = $(this).data('id');
                    openEducationModel(educationid);
                });
                $(".delete-education").click(function () {
                    var educationid = $(this).data('id');
                    deleteEducation(educationid, this);
                });
            },
            error: handleAjaxError(null, "Error While Fetching Education List.")
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
                $("#educationFormContainer").html('');
                $("#educationFormContainer").append(data);

                InitializeCommonDates("#eduStartDate", "#eduEndDate", "yyyy", "years", "years");
                //InitializeCommonDates("#eduStartDate", "#eduEndDate");

                //OpenModel("AddProfessionalModal");
                saveEducation();
                $("#btnCancelEducation").click(function () {
                    openEducationModel(0);
                });
            },
            error: function () {
                console.error("Error while Initializing Education Form.");
            }
        });
    }

    var saveEducation = function () {
        $("#upsertEducationForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                Degree: { required: true },
                Institute: { required: true },
                StartDate: { required: false },
                EndDate: {
                    required: false,
                    greaterThanStartDate: {
                        elem: "#eduStartDate",
                        format: "YYYY"
                        //format: 'MM/YYYY'
                    }
                }
                //Specialization: { required: true }
            },
            submitHandler: function (form) {
                debugger
                var f = $(form);
                var data = f.serializeArray();
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);
                data = data.map(x => {
                    if (x.name == "StartDate" || (x.name == "EndDate" && x.value != null && x.value.trim() != "")) {
                        var value = parseDateByFormat(x.value, 'YYYY');
                        if (value != null) {
                            value = value.toLocaleDateString();
                        }
                        return { name: x.name, value: value };
                    }
                    return x;
                })

                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        bindEducation();
                        openEducationModel();
                        //InitializeCommonDates("#eduStartDate", "#eduEndDate", "yyyy", "years", "years");
                        //InitializeCommonDates("#eduStartDate", "#eduEndDate");
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

    var clearEducationForm = function () {
        $("#eduEducationId").val('');
        $("#eduDegree").val('');
        $("#eduInstitute").val('');
        $("#eduStartDate").val('');
        $("#eduEndDate").val('');
        $("#eduSpecialization").val('');
        $("#eduMarks").val('');
    }

    var PortfolioImageForm = function () {

        $("#PortfolioImageForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'PortfolioImage') {
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
                const PortfolioImageForm = document.getElementById("PortfolioImageForm");
                var formData = new FormData(PortfolioImageForm);
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
                        $("#PortfolioImage").val('');
                        showMessage(data.Message, data.Data, data.Type);
                        bindPortfolioImages();
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


    var bindPortfolioImages = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#UserId").val() },
            url: '/Users/BindPortfolioImages',
            dataType: 'html',
            success: function (data, strStatus) {
                var fileInput = getFormInput("PortfolioImageForm", "PortfolioImage");
                var btn = $("#btnSubmitPortfolioForm");
                $("#portfolioImageContainer").html(data);
                $(".btnPortfolioDelete").click(function () {
                    var path = $(this).data('path');
                    setAndShowDeleteModel({ callback: () => deletePortfolioImage(path) });

                });
            },
            error: handleAjaxError(null, "Error While fetching Gallery Images")
        });
    }

    var deletePortfolioImage = function (filepath) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { path: filepath },
            url: '/Users/DeletePortfolioImage',
            dataType: 'html',
            success: function (data, strStatus) {
                bindPortfolioImages();
            },
            error: handleAjaxError()
        });
    }

    //team
    var TeamImageForm = function () {

        $("#TeamImageForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                Name: { required: true },
                designation: { required: true },
                TeamImage: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                const myForm = document.getElementById("TeamImageForm");
                var formData = new FormData(myForm);

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
                        $("#Name").val('');
                        $("#designation").val('');
                        $("#TeamImagePath").val('');
                        bindTeamImages();
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


    var bindTeamImages = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#hdnLoggedUserId").val() },
            url: '/Users/BindTeamImages',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#TeamImageContainer").html(data);
                $(".btnTeamDelete").click(function () {
                    var path = $(this).data('path');
                    var teamid = $(this).data('id');
                    setAndShowDeleteModel({
                        callback: () => {
                            deleteTeamImage(teamid, path);
                        }
                    });
                });
            },
            error: handleAjaxError(null, "Error while fetching Team Image list")
        });
    }

    var deleteTeamImage = function (TeamId, filepath) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { TeamId: TeamId, path: filepath },
            url: '/Users/DeleteTeam',
            dataType: 'html',
            success: function (data, strStatus) {
                bindTeamImages();
            },
            error: handleAjaxError()
        });
    }

    var matchYoutubeUrl = function (url) {
        //var p = /^(?:https?:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v=|watch\?.+&v=))((\w|-){11})(?:\S+)?$/;
        var p = /^(?:https?:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|shorts\/|watch\?v=|watch\?.+&v=))((\w|-){11})(?:\S+)?$/;
        var matches = url.match(p);
        if (matches) {
            return matches[1];
        }
        return false;
    }


    var validateYoutubeUrl = function () {
        if ($("#Youtube").val().trim() != "") {
            var id = matchYoutubeUrl($("#Youtube").val());
            if (id != false) {
                var validId = getVideoId($("#Youtube").val());
                $("#Youtube").val("https://www.youtube.com/embed/" + validId + "?autoplay=1&rel=0");
                return true;//valida
            } else {
                showMessage("Failed!", "Youtube url is not valid.", "error");
                return false;//alert('Incorrect URL');
            }
        }
        else {
            return true;
        }
    }

    var validateSectionYoutubeUrl = function () {
        if ($("#YouTubeUrl").val().trim() != "") {
            var id = matchYoutubeUrl($("#YouTubeUrl").val());
            if (id != false) {
                //var validId = getVideoId($("#YouTubeUrl").val());
                $("#YouTubeUrl").val("https://www.youtube.com/embed/" + id);// + "?autoplay=1&rel=0"
                return true;//valida
            } else {
                showMessage("Failed!", "Youtube url is not valid.", "error");
                return false;//alert('Incorrect URL');
            }
        }
        else {
            return true;
        }
    }

    var getVideoId = function (url) {
        const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
        const match = url?.match(regExp);

        return (match && match[2].length === 11)
            ? match[2]
            : null;
    }

    var deleteEducation = function (id, obj) {
        setAndShowDeleteModel({
            callback: function () {
                $(obj).find("span.lds-dual-ring-blue").css('display', 'inline-block');
                $(obj).addClass("disable-click");
                $.ajax({
                    type: 'GET',
                    cache: false,
                    data: { id: id },
                    url: '/Users/DeleteEducation',
                    dataType: 'json',
                    success: function (data, strStatus) {
                        bindEducation();
                        showMessage(data.Message, data.Data, data.Type);
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    },
                    error: handleAjaxError(function () {
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    })
                });
            }
        });

    }

    var deleteExperience = function (id, obj) {
        setAndShowDeleteModel({
            callback: function () {
                $(obj).find("span.lds-dual-ring-blue").css('display', 'inline-block');
                $(obj).addClass("disable-click");
                $.ajax({
                    type: 'GET',
                    cache: false,
                    data: { id: id },
                    url: '/Users/DeleteExpereince',
                    dataType: 'json',
                    success: function (data, strStatus) {
                        bindProfessional();
                        showMessage(data.Message, data.Data, data.Type);
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    },
                    error: handleAjaxError(function () {
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    })
                });
            } })
    }


    //blogs
    var bindBlog = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#UserId").val() },
            url: '/Users/BindBlog',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#blogListContainer").html(data);
                $(".edit-blog").click(function () {
                    var blogId = $(this).data('id');
                    openBlogModel(blogId);
                });
                $(".delete-blog").click(function () {
                    var blogId = $(this).data('id');
                    deleteBlog(blogId, this);
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
            url: '/Users/OpenBlogModel',
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
        $("#upsertBlogForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                BlogTitle: { required: true },
                BlogCategory: { required: true },
                BlogUrl: { required: true }
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
                        bindBlog();
                        openBlogModel(0);
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

    var deleteBlog = function (id, obj) {
        setAndShowDeleteModel({
            callback: function () {
                $(obj).find("span.lds-dual-ring-blue").css('display', 'inline-block');
                $(obj).addClass("disable-click");
                $.ajax({
                    type: 'GET',
                    cache: false,
                    data: { id: id },
                    url: '/Users/DeleteBlog',
                    dataType: 'json',
                    success: function (data, strStatus) {
                        bindBlog();
                        showMessage(data.Message, data.Data, data.Type);
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    },
                    error: handleAjaxError(function () {
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    })
                });
            }
        });
    }

    var clearBlogForm = function () {
        $("#BlogTitle").val('');
        $("#BlogCategory").val('');
        $("#BlogUrl").val('');
    }
    //blog end


    //youtube
    var bindYouTube = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#UserId").val() },
            url: '/Users/BindYouTube',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#youtubeListContainer").html(data);
                $(".edit-youtube").click(function () {
                    var YouTubeId = $(this).data('id');
                    openYouTubeModel(YouTubeId);
                });
                $(".delete-youtube").click(function () {
                    var YouTubeId = $(this).data('id');
                    deleteYouTube(YouTubeId, this);
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
            url: '/Users/OpenYouTubeModel',
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
        $("#upsertYouTubeForm").validate({
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
        setAndShowDeleteModel({
            callback: function () {
                $(obj).find("span.lds-dual-ring-blue").css('display', 'inline-block');
                $(obj).addClass("disable-click");
                $.ajax({
                    type: 'GET',
                    cache: false,
                    data: { id: id },
                    url: '/Users/DeleteYouTube',
                    dataType: 'json',
                    success: function (data, strStatus) {
                        bindYouTube();
                        showMessage(data.Message, data.Data, data.Type);
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    },
                    error: handleAjaxError(function () {
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    })
                });
            }
        });


    }

    var clearYouTubeForm = function () {
        $("#YouTubeTitle").val('');
        $("#YouTubeUrl").val('');
    }
    //youtube end


    // Schedule start
    var GetScheduleOpenDays = function (instance) {
        $.get(me.getScheduleOpenDaysUrl, { UserId: $('#hdnLoggedUserId').val() }, function (data) {
            if (data.Success) {
                openScheduleDates = null;
                openScheduleDates = data.Data.map(item => {
                    return new Date(item.Date);
                });
                instance.setDate(openScheduleDates);
            }

        })
    }

    var upsertScheduleOpenWeekDaysForm = function () {
        var changedChkEle = $(this);
        var changedChkName = changedChkEle.prop("name");
        var chkList = $("#dvOpenCalanderDaysChkContainer").find(".chkField");
        var data = {
            UserId: $('#hdnLoggedUserId').val()
        };
        chkList.each(function () {
            var eleName = $(this).prop("name");
            var isChecked = $(this).is(":checked");
            data[eleName] = isChecked;
        });

        $.post(
            "/Users/UpsertScheduleOpenWeekDays",
            data,
            function (result, strStatus) {
                showMessage(result.Message, result.Data, result.Type, 1500);
                if (result.Success) {
                    multipleDatePickerInit();
                    return;
                }
                changedChkEle.prop("checked", !data[changedChkName]);
            }
        ).fail(handleAjaxError(function () {
            changedChkEle.prop("checked", !data[changedChkName]);
        }));
    }

    var getWeekDays = function (onSuccess, onError) {
        openWeekDays = [];
        $.get(
            "/Users/GetScheduleOpenWeekDays",
            {
                UserId: $('#hdnLoggedUserId').val(),
                actionName: "BharatTouch/EditProfile/init"
            },
            function (result) {
                if (!result.Success && onError && typeof onError == "function") {
                    onError();
                }

                if (result.Success) {
                    var d = result.Data;
                    if (d.Sun) openWeekDays.push(0);
                    if (d.Mon) openWeekDays.push(1);
                    if (d.Tue) openWeekDays.push(2);
                    if (d.Wed) openWeekDays.push(3);
                    if (d.Thu) openWeekDays.push(4);
                    if (d.Fri) openWeekDays.push(5);
                    if (d.Sat) openWeekDays.push(6);

                    if (onSuccess && typeof onSuccess == "function") {
                        onSuccess(d);
                    }
                }
            }
        ).fail(function () {
            if (onError && typeof onError == "function") {
                onError();
            }
        });
    }

    var disableWeekDays = function (instance) {
        getWeekDays(function (d) {
            instance.set("disable", [
                function (date) {
                    const today = new Date();
                    const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
                    const endOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);
                    if (date < startOfMonth || date > endOfMonth) {
                        return true;
                    }
                    const day = date.getDay();
                    return !openWeekDays.includes(day);
                }
            ]);

            if (openWeekDays.length == 0) {
                $("#lblOpenWeekDayNoteLabel").hide();
                $("#lblClosedWeekDayNoteLabel").show();
                $("#multipleOpenDays").prop("disabled", true);
                $("#AddOpenScheduleDaysForm").find(":submit").prop("disabled", true);
            } else {
                $("#lblOpenWeekDayNoteLabel").show();
                $("#lblClosedWeekDayNoteLabel").hide();
                $("#multipleOpenDays").prop("disabled", false);
                $("#AddOpenScheduleDaysForm").find(":submit").prop("disabled", false);
            }
        });
    }

    var multipleDatePickerInit = function () {
        multipleDatePickrInstance = $("#multipleOpenDays").flatpickr({
            mode: "multiple",
            dateFormat: "m-d-Y",
            locale: {
                firstDayOfWeek: 1 // 0 = Sunday, 1 = Monday
            },
            onReady: function (selectedDates, dateStr, instance) {
                console.log("Flatpickr is ready!");
                disableWeekDays(instance);
                GetScheduleOpenDays(instance);
            },
            onOpen: function (selectedDates, dateStr, instance) {
                if (openWeekDays.length == 0) {
                    instance.close();
                }
            },
        });
    }

    var upsertMultipleDays = function () {
        $("#AddOpenScheduleDaysForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'FirstName') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var submitBtn = f.find(":submit");
                submitBtn.find("div.lds-dual-ring").css('display', 'inline-block');
                submitBtn.prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        submitBtn.find("div.lds-dual-ring").css('display', 'none');
                        submitBtn.prop('disabled', false);
                        if (data.Success) {
                            window.location.reload();
                        }
                    },
                    error: handleAjaxError(function () {
                        submitBtn.find("div.lds-dual-ring").css('display', 'none');
                        submitBtn.prop('disabled', false);
                    })
                });
            }
        });
    }

    var deleteScheduleOpenDay = function (dayId, obj) {
        setAndShowDeleteModel({
            callback: function () {
                $(obj).find("span.lds-dual-ring-blue").css('display', 'inline-block');
                $(obj).addClass("disable-click");
                $.ajax({
                    type: 'GET',
                    cache: false,
                    data: { dayId: dayId },
                    url: '/Users/DeleteScheduleOpenDay',
                    dataType: 'json',
                    success: function (data, strStatus) {
                        BindScheduleOpenDayList();
                        reinitializeMultipleDatePickr();
                        showMessage(data.Message, data.Data, data.Type);
                        $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                        $(obj).removeClass("disable-click");
                    },
                    error: handleAjaxError(
                        function () {
                            $(obj).find("span.lds-dual-ring-blue").css('display', 'none');
                            $(obj).removeClass("disable-click");
                        }
                    )
                });
            }
        });
    }

    var reinitializeMultipleDatePickr = function () {
        if (multipleDatePickrInstance) {
            multipleDatePickrInstance.clear();
            multipleDatePickrInstance.destroy();
        }
        openScheduleDates = null;
        multipleDatePickerInit();
        //GetScheduleOpenDays();
    }

    var BindScheduleOpenDayList = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { UserId: $("#hdnLoggedUserId").val() },
            url: '/Users/BindScheduleOpenDayList',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#scheduleOpenDayListContainer").html(data);
                $(".btnDeleteScheduleOpenDay").click(function () {
                    var dayId = $(this).data('id');
                    deleteScheduleOpenDay(dayId, this);
                });
            },
            error: handleAjaxError()
        });
    }
    // Schedule End

    var deletePaymentDetails = function (path, thumb) {
        $.ajax({
            type: 'GET',
            url: "/Users/DeletePaymentDetails?filename=" + path,
            dataType: 'json',
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                $("#PaymentDeleteModel").modal("hide");
                window.location.reload();
            },
            error: handleAjaxError()
        });
    }


    //var loadnews = function () {
    //    $.ajax({
    //        url: 'https://api.mediastack.com/v1/news',
    //        type: 'GET',
    //        data: {
    //            keywords: $("#CompanyType").val(),
    //            sort: "published_desc",
    //            languages: 'en',
    //            countries: 'ca,us,in',
    //            limit: 30,
    //            access_key: me.newsApiKey
    //        },
    //        success: function (response) {
    //            if (response?.data != null || response?.data != undefined || response?.data?.length > 0 ) {
    //                var newsCards = '';
    //                response.data.forEach(function (article) {
    //                    newsCards += `
    //                     <div class="swiper-slide">
    //                        <div class="news-card">
    //                          <img src="${article.image || '/Content/images/noimage.png'}" alt="${article.title}">
    //                          <h4>${article.title}</h3>
    //                          <p>${article.description || 'No description available.'}</p>
    //                          <p class="author">By ${article.author || 'Unknown'}</p>
    //                          <p class="date">Published on: ${new Date(article.published_at).toLocaleDateString()}</p>
    //                        </div>
    //                     </div>
    //                  `;
    //                });
    //                $('#newsSlidesWrapper').html(newsCards);
    //                startNewsSlider();
    //            }
    //        },
    //        error: handleAjaxError()
    //    });
    //}

    //function startNewsSlider() {
    //    var swiper = new Swiper(".news-slider", {
    //        "loop": true,
    //        "speed": 600,
    //        spaceBetween: 10,
    //        centeredSlides: true, 
    //        "autoplay": {
    //            "delay": 2500,
    //              disableOnInteraction: false
    //        },
    //        "slidesPerView": "1",
    //        "pagination": {
    //            "el": ".swiper-pagination",
    //            "type": "bullets",
    //            "clickable": true
    //        },
    //        "breakpoints": {
    //            320: {
    //                slidesPerView: 1,
    //                spaceBetween: 5
    //            },

    //            768: {
    //                slidesPerView: 2,
    //                spaceBetween: 10
    //            },
    //        }
    //    });

    //}

    //function getNearbyCompanyList() {
    //    var companyType = $("#CompanyType").val();
    //    if (currentLat && currentLon && companyType.trim() != "") {
    //        $.ajax({
    //            url: 'https://local-business-data.p.rapidapi.com/search-nearby',
    //            method: 'GET',
    //            data: {
    //                query: $("#CompanyType").val(),
    //                lat: currentLat,
    //                lng: currentLon,
    //                limit: 6,
    //                language: 'en',
    //                //region: 'in',
    //                extract_emails_and_contacts: false
    //            },
    //            headers: {
    //                'x-rapidapi-key': me.nearByCompanyApiKey,
    //                'x-rapidapi-host': 'local-business-data.p.rapidapi.com'
    //            },
    //            success: function (response) {
    //                if (response.status != "OK") {
    //                    return;
    //                }
    //                if (response.data?.length > 0) {
    //                    var list = "";
    //                    response.data.forEach((item) => {
    //                        list += `
    //                            <div class="col-lg-6">
    //                                <div class="resume-item">
    //                                    <h4>
    //                                        ${item.website != null ? `<a href="${item.website}" target="_blank" >${item.name}</a>` : item.name}
    //                                    </h4>
    //                                    <h5>${item.owner_name}</h5>
    //                                    <p><i class="bx bx-map text-primary"></i><em> ${item.address}</em></p>
    //                                    ${item.phone_number != null ? `<p><em><i class="bx bx-phone text-primary"></i></em> ${item.phone_number}</p>` : ''}
                                        
    //                                    <p>
    //                                        <a class="edit-professional" href="${item.place_link}" target="_blank"  style="color:darkblue;cursor:pointer;"><i class="bx bx-map-pin"></i>&nbsp; Location</a>
    //                                    </p>

    //                                </div>
    //                            </div>
    //                        `;
    //                    });

    //                    $("#nearByCompanyListContainer").html(list);
    //                }
    //            },
    //            error: function (xhr, status, error) {
    //                console.error('Request failed:', status, error);  // Log any errors
    //            }
    //        });
    //    }
    //}

    //function fetchCurrentLocation() {
    //    if (!navigator.geolocation) {
    //        // Geolocation not supported
    //        console.log('Geolocation is not supported by this browser.');
    //        return;
    //    }
    //    navigator.geolocation.getCurrentPosition(function (position) {
    //        currentLat = position.coords.latitude;
    //        currentLon = position.coords.longitude;
    //        getNearbyCompanyList();
    //    }, function (error) {
    //        // Handle error if geolocation is not available or user denies permission
    //        console.log("geoLocation is not available or user denied location permission.");
    //    });
    //}

    var uploadCardThemeBgImageForm = function () {
        $("#uploadCardThemeBgImgForm").validate({
            submitHandler: function (form) {
                var f = $(form);
                const myForm = document.getElementById("uploadCardThemeBgImgForm");
                var formData = new FormData(myForm);

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
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        window.location.reload();
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        })
    }

    function loadCardThemeTemplate(obj, starting) {
        $("#dvNoCardThemeFound").remove();
        var container = $("#cardThemeTemplateListContainer");
        function NoContentFunc() {
            if (starting || !starting && container.children().length == 0) {
                container.html(`
                    <div id="dvNoCardThemeFound" class="col-lg-12 col-md-12 col-sm-12 d-flex flex-column text-center my-2 p-2" >
                        <h4 style="margin-top: 20%;">No data found</h4>
                    </div>
                `);
                return true;
            }

            container.append(`
                <div id="dvNoCardThemeFound" class="col-lg-12 col-md-12 col-sm-12 d-flex flex-column text-center my-2 p-2">
                    <h4>No more data found</h4>
                </div>
            `);
        }

        if (obj) {
            obj.find("div.lds-dual-ring").css('display', 'inline-block');
            obj.prop('disabled', true);
        }
        var Page = $("#hdnCartTemplateListPage");
        if (starting) {
            container.html("");
            Page.val(1);
        }
        var nextPage = parseInt(Page.val(), 10) + 1;
        var Size = $("#hdnCartTemplateListPageSize");
        var SortBy = $("#hdnCartTemplateListSortBy");
        var SortOrder = $("#hdnCartTemplateListSortOrder");

        var isBusinessRelated = $("#chkBusinessRelatedThemes").is(":checked");
        var searchText = isBusinessRelated ? $("#hdnBusinessTypes").val() : "";

        $.post(
            '/CardTheme/FetchMoreThemeCards',
            {
                Page: Page.val(),
                Size: Size.val(),
                SortBy: SortBy.val(),
                SortOrder: SortOrder.val(),
                SearchText: searchText,
                actionName: "BharatTouch/EditProfile/FetchMoreThemeCards"
            },
            function (response, strStatus) {
                if (!response.Success || response.Data.length == 0) {
                    NoContentFunc();
                    if (obj) {
                        obj.find("div.lds-dual-ring").css('display', 'none');
                        obj.prop('disabled', false);
                    }
                    return;
                }
                var themeList = "";
                response.Data.forEach((themeItem) => {
                    themeList += selectThemeCardItem(themeItem);
                });

                container.append(themeList);

                Page.val(nextPage);
                if (obj) {
                    obj.find("div.lds-dual-ring").css('display', 'none');
                    obj.prop('disabled', false);
                }
            }
        ).fail(function () {
            if (obj) {
                obj.find("div.lds-dual-ring").css('display', 'none');
                obj.prop('disabled', false);
            }
        });
    }

    function selectThemeCardItem(themeItem) {
        var selectedId = $("#hdnSelectedThemeTemplateId").val();
        var isSelected = selectedId == themeItem.TemplateId;

        return `
            <div class="col-12 col-md-6 col-lg-4 card-theme-item">
                <div class="card shadow hover-card">
                    <div class="card-img-wrapper">
                        <img src="${themeItem.ImageUrl}" class="card-img-top img-fluid" alt="${themeItem.Name}">
                        <div class="overlay d-flex flex-column align-items-center justify-content-center">
                            <h5 class="text-white mb-3">${themeItem.Name}</h5>
                            ${
                                !isSelected ?
                                    `<a class="btn btn-primary select-card-theme" data-id="${themeItem.TemplateId}">
                                        <div class="lds-dual-ring"></div>
                                        &nbsp; Select                            
                                    </a>` : ""
                            }
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    function deleteFile(path, callback) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { filePath: path },
            url: '/Users/DeleteFile',
            dataType: 'json',
            success: function (data, strStatus) {
                if (callback) {
                    callback(data, strStatus);
                }
            },
            error: handleAjaxError()
        });
    }

    function onSelectCardTheme() {
        var obj = $(this);
        var TemplateId = obj.data("id");
        if (TemplateId == $("#hdnSelectedThemeTemplateId").val()) {
            return;
        }
        var UserId = $("#hdnLoggedUserId").val();
        var BgImg = $("#hdnSelectThemeBackgroundImg").val();
        $.post(
            "/CardTheme/UpsertUserCardThemeSetting",
            {
                TemplateId: TemplateId,
                UserId: UserId,
                BackgroundImg: BgImg,
                actionName: "BharatTouch/EditProfile/_SelectCardThemeFormModel/UpsertUserCardThemeSetting"
            },
            function (data) {
                showMessage(data.Message, data.Data, data.Type);
                obj.find("div.lds-dual-ring").css('display', 'none');
                obj.prop('disabled', false);
                if (data.Success) {
                    window.location.reload();
                }
            }
        ).fail(handleAjaxError(function () {
            obj.find("div.lds-dual-ring").css('display', 'none');
            obj.prop('disabled', false);
        }));
    }

    function removeCardThemeBg() {
        var data = $("#deleteModelData").data();
        $.post(
            "/CardTheme/RemoveCardThemeBgImg",
            {
                UserId: data.UserId,
                TemplateId: data.TemplateId,
                BackgroundImg: data.BackgroundImg,
                actionName: "BharatTouch/EditProfile/RemoveCardThemeBgImg"
            },
            function (data) {
                showMessage(data.Message, data.Data, data.Type);
                if (data.Success) {
                    window.location.reload();
                }
            }
        ).fail(handleAjaxError())
    }


    var coverImageForm = function () {
        $("#coverImageForm").validate({
            submitHandler: function (form) {
                var f = $(form);
                const myForm = document.getElementById("coverImageForm");
                var data = f.serializeArray();

                var formData;
                if ($croppableCoverImage != undefined) {
                    formData = new FormData();
                    $croppableCoverImage.cropper('getCroppedCanvas').toBlob((getCropped) => {
                        formData.append('coverImageFile', getCropped);
                        $(data).each(function (index, element) {
                            formData.append(element.name, element.value);

                        });
                        submitcoverImageForm(f, formData);
                    });
                }
                else {
                    var formData = new FormData(myForm);
                    submitcoverImageForm(f, formData);
                }


                //var f = $(form);
                //const myForm = document.getElementById("coverImageForm");
                //var formData = new FormData(myForm);
                //submitBtn = f.find(":submit");
                //submitBtn.find("div.lds-dual-ring").css('display', 'inline-block');
                //submitBtn.prop('disabled', true);

                //$.ajax({
                //    type: 'Post',
                //    url: f[0].action,
                //    data: formData,
                //    dataType: 'json',
                //    contentType: false,
                //    processData: false,
                //    success: function (data, strStatus) {
                //        showMessage(data.Message, data.Data, data.Type);
                //        submitBtn.find("div.lds-dual-ring").css('display', 'none');
                //        submitBtn.prop('disabled', false);
                //        if (data.Success) {
                //            window.location.reload();
                //        }
                //    },
                //    error: handleAjaxError(function () {
                //        submitBtn.find("div.lds-dual-ring").css('display', 'none');
                //        submitBtn.prop('disabled', false);
                //    })
                //});
            }
        })
    }

    var submitcoverImageForm = function (f, formData) {
        var submitBtn = f.find(":submit");
        submitBtn.find("div.lds-dual-ring").css('display', 'inline-block');
        submitBtn.prop('disabled', true);

        $.ajax({
            type: 'Post',
            url: f[0].action,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                submitBtn.find("div.lds-dual-ring").css('display', 'none');
                submitBtn.prop('disabled', false);
                if (data.Success) {
                    window.location.reload();
                }
            },
            error: handleAjaxError(function () {
                submitBtn.find("div.lds-dual-ring").css('display', 'none');
                submitBtn.prop('disabled', false);
            })
        });
    }

    onSelectCoverImageFile = async function (input, type = "file") {
        $(".coverImageSelectLoader").css('display', 'inline-block');
        $("#coverImageSelectionError").hide();
        $('#croppableCoverImage').removeAttr('src');
        $('#croppableCoverImage').cropper('destroy');
        if (type == "file") {
            if (input.files && input.files[0]) {
                var files = input.files;
                var formData = new FormData();
                var maxSize = 5 * 1024 * 1024;
                if (files.length > 0) {
                    for (var x = 0; x < files.length; x++) {
                        if (files[x].size > maxSize) {
                            $(".coverImageSelectLoader").css('display', 'none')
                            $("#coverImageSelectionError").html("The Image you selected exceeds the maximum allowed size of 5MB. Please choose a Image that is 5MB or smaller.");
                            $("#coverImageSelectionError").show();
                            return;
                        }
                        formData.append("file" + x, files[x]);
                    }
                }
                $("#cropCoverImagePrfLoader").css('display', 'inline-block');
                saveTempImage(formData, showCroppingCoverImage, () => $(".coverImageSelectLoader").css('display', 'none'));
            } else {
                $(".coverImageSelectLoader").css('display', 'none');
            }
        } else if (type == "link") {
            var res = await GetImageFromAnotherDomain(input);
            if (res.Success) {
                var files = res.Message;
                var formData = new FormData();
                if (files) {
                    formData.append("file", files);
                }
                $("#cropCoverImagePrfLoader").css('display', 'inline-block');
                saveTempImage(formData, showCroppingCoverImage, () => $(".coverImageSelectLoader").css('display', 'none'));
            } else {
                $(".coverImageSelectLoader").css('display', 'none')
            }
        }
    }
    var showCroppingCoverImage = function (data) {
        var modal = $("#cropCoverImageModal");
        if (data.Success) {

            var img = $('#croppableCoverImage');
            //img.attr('src', data.Data.replace("~", "")).width(200).height(200);
            img.attr('src', data.Data.replace("~", ""));
            img.css({
                "width": "100%",
                "object-fit": "cover",
                "display": "block",
                "max-width": "100%"
            });
            modal.modal("show");
            modal.on('shown.bs.modal', function () {
                img.on('load', function () {
                    $croppableCoverImage = img;
                    $croppableCoverImage.cropper('destroy'); // Destroy previous cropper instance
                    $croppableCoverImage.cropper({
                        aspectRatio: 16 / 7.5,
                        viewMode: 1,
                        autoCropArea: 1,
                        cropBoxResizable: false,
                        minContainerWidth: img.naturalWidth, // Keep width 100%
                        minCanvasWidth: img.naturalWidth, // Ensure full width
                        minCropBoxWidth: img.naturalWidth, // Force width to be full
                    });

                    // Show modal only after the image is set and cropper is initialized
                    if ($('#croppableCoverImage').attr('src') !== '') {
                            $("#cropCoverImagePrfLoader").css('display', 'none');
                            $('#cropCoverImageContainer').show();
                    }
                });

                // Trigger load manually if the image is already cached
                if (img[0].complete) {
                    img.trigger('load');
                }
            });
        }
        else {
            getFormInput("coverImage1Form", "coverImageFile").val('');
            showMessage("Warning!", data.Data, "notice");
            $("#cropCoverImagePrfLoader").css('display', 'none');
        }
        $(".coverImageSelectLoader").css('display', 'none');
    }

    var saveTempImage = function (formData, callback, errorCallback) {
        $.ajax({
            type: 'POST',
            url: '/Users/SaveTempProfileImage',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data, strStatus) {
                //hideLoader();
                if (callback) {
                    callback(data);
                }

            },
            error: handleAjaxError(function () {
                if (errorCallback) {
                    errorCallback();
                }
            })
        });
    }
    function fetchCoverImages(obj) {
        var businessType = $("#hdnBusinessType").val();
        if (obj) {
            obj.find("div.lds-dual-ring").css('display', 'inline-block');
            obj.prop('disabled', true);
        }
        var pageNo = $("#coverImgPageNo");
        var nextPageNo = parseInt(pageNo.val(), 10) + 1;
        $("#dvNoMoreCoverImage").remove();
        $.ajax({
            url: `https://api.pexels.com/v1/search?query=${businessType.trim() == "" ? "all" : businessType}&page=${pageNo.val()}&per_page=40`,
            method: 'GET',
            headers: {
                "Authorization": me.coverImgApiKey
            },
            success: function (data) {
                var d = data.photos;
                var container = $("#suggestedCoverImgContainer");
                if (d.length == 0) {
                    container.append(
                        `<div class="col-lg-12 col-md-12 col-sm-12 d-flex flex-column text-center my-2 p-2" id="dvNoMoreCoverImage">
                                 <h4 style="margin-top: 22%;">${container.children().length == 0 ? "No data found" : "No more data found"}</h4>
                         </div>`
                    );
                    if (obj) {
                        obj.find("div.lds-dual-ring").css('display', 'none');
                        obj.prop('disabled', false);
                    }
                    return;
                }
                var imgEles = d.map((item) => coverImageCardItem(item)).join("");
                container.append(imgEles);
                pageNo.val(nextPageNo);
                if (obj) {
                    obj.find("div.lds-dual-ring").css('display', 'none');
                    obj.prop('disabled', false);
                }
            },
            error: function () {
                if (obj) {
                    obj.find("div.lds-dual-ring").css('display', 'none');
                    obj.prop('disabled', false);
                }
            }
        });
    }

    function coverImageCardItem(item) {
        return `
            <div class="col-12 col-md-4 col-lg-3 mb-3">
                <div class="card hover-card-bottom h-100 border-r-1">
                    <img src="${item.src.tiny}" class="card-img-top" alt="${item.alt}">
                    <div class="card-overlay">
                        <a class="btn btn-primary btnSelectCoverImg"  data-path="${item.src.large2x}">
                            <div class="lds-dual-ring"></div>
                            &nbsp; Select
                        </a>
                    </div>
                </div>
            </div>
        `;
    }

    function RemoveCoverImage(data) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { UserId: data.UserId, path: data.path },
            url: '/Users/RemoveCoverImage',
            dataType: 'json',
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                if (data.Success) {
                    window.location.reload();
                }
            },
            error: handleAjaxError()
        });
    }

    function uploadProfileAndCoverImage(ProfileImage, CoverImage, obj) {
        if (obj) {
            obj.find("div.lds-dual-ring").css('display', 'inline-block');
            obj.prop('disabled', true);
        }
        var UserId = $("#hdnLoggedUserId").val();
        $.post(
            "/Users/UpsertUserProfileAndCoverImages",
            {
                UserId: UserId,
                ProfileImage: ProfileImage,
                CoverImage: CoverImage
            },
            function (data) {
                showMessage(data.Message, data.Data, data.Type);
                if (obj) {
                    obj.find("div.lds-dual-ring").css('display', 'none');
                    obj.prop('disabled', false);
                }
                if (data.Success) {
                    window.location.reload();
                }
            }
        ).fail(handleAjaxError(function () {
            if (obj) {
                obj.find("div.lds-dual-ring").css('display', 'none');
                obj.prop('disabled', false);
            }
        }));
    }

    var UpdateCoverImagePropertyForm = function () {

        $("#ddlBestFitCoverImage").on("change", function () {
            var value = $(this).val();
            $("#previewCoverImageStyle").removeClass("cover contain stretch custom-offset-fit").addClass(value);
        });

        $("#UpdateCoverImagePropertyForm").validate({
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
                        showMessage(data.Message, data.Data, data.Type, 2000);
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        if (data.Success) {
                            $("#hero").removeClass("cover contain stretch custom-offset-fit").addClass($("#ddlBestFitCoverImage").val());
                            $("#CoverImageStyleModal").modal("hide");
                        }
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        })
    }

    function openCoverImagePropertyModal() {
        var loggedUserId = $('#hdnLoggedUserId').val()
        $.ajax({
            type: 'GET',
            cache: false,
            data: { UserId: loggedUserId },
            url: '/Users/OpenCoverImagePropertyPreviewModal',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#CoverImageStyleModal").remove()
                $("body").append(data);
                $("#CoverImageStyleModal").modal('show');
                UpdateCoverImagePropertyForm();
            },
            error: handleAjaxError()
        });
    }

    async function LoadGoogleApi(whenDoneCallback) {
        var res = await gapi.load('client', async () => {
            const DISCOVERY_DOC = 'https://www.googleapis.com/discovery/v1/apis/calendar/v3/rest';
            var res = await gapi.client.init({
                apiKey: me.googleApiKey,
                discoveryDocs: [DISCOVERY_DOC],
            });
            if (whenDoneCallback) {
                await whenDoneCallback();
            }
        });
        console.log("GoogleApi Loaded");
    }

    async function isGoogleAccessTokenValid(token) {
        try {
            const request = {
                'calendarId': 'primary',
                'timeMin': (new Date()).toISOString(),
                'showDeleted': false,
                'singleEvents': true,
                'maxResults': 10,
                'orderBy': 'startTime',
                oauth_token: token
            };
            response = await gapi.client.calendar.events.list(request);
            return true;
        } catch (err) {
            return false;
        }
    }

    async function GoogleOAuthInit() {
        var userId = $("#hdnLoggedUserId").val();
        var btnGoogleSync = $("#btnSyncCalendarWithGoogle");
        var tokenEle = $("#hdnUserGoogleAccessToken");
        tokenClient = google.accounts.oauth2.initCodeClient({
            client_id: me.googleClientId,
            scope: 'https://www.googleapis.com/auth/calendar',
            ux_mode: 'redirect',
            //ux_mode: "popup",
            redirect_uri: me.googleAuthRedirectUri,
            response_type: 'code',
            state: `userId=${userId}&originalUrl=${me.pageUrl}&redirectUri=${me.googleAuthRedirectUri}`,
            access_type: 'offline',
            callback: (response) => {
                console.log("response", response);
                if (response.code) {
                    $.get("/OAuth/AuthenticateUserCodeAsync", { code: response.code, state: response.state }, function (data) {
                        if (!data.Success) {
                            showMessage(data.Message, data.Data, data.Type, 2000);
                        } else {
                            tokenEle.val(data.Data);
                            btnGoogleSync.hide();
                            showMessage(data.Message, "Sync process successfull completed!", data.Type, 2000);
                        }
                    }).fail(handleAjaxError());
                }
            },
        });
        await LoadGoogleApi(async function () {
            if (tokenEle.val().trim() == "") {
                btnGoogleSync.show();
                return;
            }
            var isValid = await isGoogleAccessTokenValid(tokenEle.val());
            if (isValid) {
                btnGoogleSync.hide();
                return;
            }
            $.get("/OAuth/RefreshGoogleAccessToken", { userId: userId }, function (data) {
                if (!data.Success) {
                    btnGoogleSync.show();
                    return;
                }

                btnGoogleSync.hide();
                tokenEle.val(data.Data);
            });
        });
    }

    async function isMicrosoftTokenValid(accessToken) {
        const apiUrl = 'https://graph.microsoft.com/v1.0/me/';
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
            });

            if (!response.ok) {
                return false;
            }
            return true;
        } catch (error) {
            console.error('Error checking token validity:', error);
            return false;
        }
    }

    async function MicrosoftOAuthInit() {

        var userId = $("#hdnLoggedUserId").val();
        var btnOutlookSync = $("#btnSyncCalendarWithOutlook");
        var tokenEle = $("#hdnUserMicrosoftAccessToken");
        if (tokenEle.val().trim() == "") {
            btnOutlookSync.show();
            return;
        }

        if (await isMicrosoftTokenValid(tokenEle.val())) {
            btnOutlookSync.hide();
            return;
        }

        $.get("/OAuth/RefreshAccessTokenMicrosoftAsync", { userId: userId }, function (data) {
            if (!data.Success) {
                btnOutlookSync.show();
                return;
            }

            btnOutlookSync.hide();
            tokenEle.val(data.Data);
        });
    }
    function BindDdlCounpanyType(pageLoad = false) {
        var f = $("#PersonalInfoUserForm");
        var ddl = $("#ddlCompanyTypeId");
        var txt = f.find("input[name=CompanyType]");
        ddl.show();
        txt.hide();
        var parent = $("#ddlCompanyTypeParentId");
        var id = parent.val();
        ddl.empty();
        ddl.append(
            $("<option></option>").val(0).html("select Company Type")
        );
        
        if (id == "36") {
            ddl.hide();
            ddl.append(
                $("<option></option>").val(id).html(parent.find(":selected").text())
            );
            ddl.val(id);
            txt.show();
            if (pageLoad) {
                var typeval = $("#hdnBusinessTypes").val();
                txt.val(typeval == null ? "" : typeval);
            } else {
                txt.val("");
            }
            return;
        } else if (id == "0") {
            return;
        }
        $.post(me.bindCompanyTypeByParent, { id: id }, function (res) {
            if (!res.Success) {
                ddl.val(0);
                return;
            }
            $.each(res.Data, function (k, v) {
                ddl.append(
                    $("<option></option>").val(v.BusinessTypeId).html(v.BusinessType)
                );
            });
            if (pageLoad && $("#hdnBusinessTypeId").val() != null) {
                ddl.val($("#hdnBusinessTypeId").val());
            } else {
                ddl.val(0);
            }
        }).fail(handleAjaxError())
    }

    function getFormInput(formId, inputName){
        return $('#' + formId).find(`input[name=${inputName}]`)
    }

    //Testimonial Section
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

    var bindClientTestimonialImages = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#UserId").val() },
            url: '/Users/BindClientTestimonials',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#ClientTestimonialsContainer").html(data);
                $(".btnClientTestimonialDelete").click(function () {
                    var clientId = $(this).data('id');
                    setAndShowDeleteModel({ callback: () => deleteClientTestimonial(clientId) });
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
            '/Users/DeleteClientTestimonial',
            { clientId: clientId, actionName: "BharatTouch/EditProfile" },
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
            '/Users/GetClientTestimonialBy_Id',
            { clientId: clientId, actionName: "BharatTouch/EditProfile" },
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
                $("#btnSubmitTestimonialText").html("Update");
            }
        ).fail(handleAjaxError());
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
        $("#btnSubmitTestimonialText").html("Add");
    }

    var openUserCertificationModel = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: '/Users/OpenUserCertificationModel',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#userCertificatonFormContainer").html('');
                $("#userCertificatonFormContainer").append(data);

                InitializeCommonDates("#DP_IssueDate", "#DP_ExpirationDate");

                upsertUserCertificationForm();
                $("#btnCancelUserCertification").click(function () {
                    openUserCertificationModel(0);
                });
            },
            error: function () {
                console.error("Error while bind Certification/Training Form");
            }
        });
    }

    var upsertUserCertificationForm = function () {
        $("#upsertUserCertificationForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                UserId: { required: true },
                CertificationName: { required: true },
                IssuingOrganization: { required: true },
                Specialization: { required: true },
                //IssueDate: { required: true },
                ExpirationDate: {
                    required: false,
                    greaterThanStartDate: {
                        elem: "#DP_IssueDate",
                        //format: "MM/dd/yyyy"
                        format: "MM/YYYY"
                    }
                },
                //CertifcateNumber: { required: true },
                //Description: { required: true },
                CertificateFilePath: {
                    required: () => (getFormInput("upsertUserCertificationForm", "CertificateFile").val() == null || getFormInput("upsertUserCertificationForm", "CertificateFile").val().trim() == "")
                }
            },
            messages: {
                ExpirationDate: {
                    greaterThanStartDate: "Expiration date should be greater than Issue date."
                },
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                const fData = document.getElementById("upsertUserCertificationForm");
                var formData = new FormData(fData);
                //$(data).each(function (index, element) {
                //    formData.append(element.name, element.value);
                //});
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);

                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        bindUserCertificationList();
                        openUserCertificationModel(0);
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

    var bindUserCertificationList = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { userId: $("#hdnLoggedUserId").val() },
            url: '/Users/BindUserCertifications',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#userCertificationListContainer").html(data);
                $(".delete-user-certification").click(function () {
                    var id = $(this).data('id');
                    setAndShowDeleteModel({ callback: () => deleteUserCertificationclientId(id) });
                });
                $(".edit-user-certification").click(function () {
                    var clientId = $(this).data('id');
                    openUserCertificationModel(clientId);
                });
            },
            error: function () {
                showMessage("Failed.", "Error while Fetching Certification List", "error");
            }
        });
    }

    function deleteUserCertificationclientId(id) {
        $.post(
            '/Users/DeleteUserCertification',
            { certificationId: id, actionName: "BharatTouch/EditProfile" },
            function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type, 2000);
                if (!data.Success) {
                    return;
                }
                else {
                    bindUserCertificationList();
                }
            }
        ).fail(handleAjaxError());
    }


    async function GetImageFromAnotherDomain(imageUrl) {
        try {
            const response = await fetch(imageUrl, { mode: 'cors' }); // Explicitly request CORS
            if (!response.ok) {
                return { Success: false, Message: `HTTP error! status: ${response.status}` }
            }
            const blob = await response.blob();
            const file = new File([blob], "downloaded-image.jpg", { type: blob.type });
            return { Success: true, Message: file }
        } catch (error) {
            return { Success: false, Message: error?.message ?? "Image download error." }
        }
    }


    async function downloadImageFromAnotherDomain(imageUrl, imageName = 'image.jpg') {
        try {
            var res = await GetImageFromAnotherDomain(imageUrl);
            if (res.Success) {
                //downloadFileByPath(res.Message, imageName);
                return true;
            }
            return false;
        } catch (error) {
            return false;
            console.error("Download failed:", error);
            console.error("Image download failed. Check console for details (likely CORS issue).");
        }
    }

    var downloadFileWithFileName = function (url, imageName) {
        const downloadLink = document.createElement('a');
        downloadLink.href = url;
        downloadLink.download = imageName;
        document.body.appendChild(downloadLink);
        downloadLink.click();
        URL.revokeObjectURL(url);
        document.body.removeChild(downloadLink);
    }


    //Adhaar Section

    var upsertAdhaarImageForm = function () {
        $("#AdhaarImageForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                const fData = document.getElementById("AdhaarImageForm");
                var formData = new FormData(fData);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);

                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data, strStatus) {
                            showMessage(data.Message, data.Data, data.Type);
                        if (!data.Success) {
                            f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                            f.find(":submit").prop('disabled', false);
                            return;
                        }
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        window.location.reload();
                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });

    }

    var deleteAdhaarImage = function (id) {
        var AdhaarFrontImgPath = id == 0 ? "" : getFormInput("AdhaarImageForm", "AdhaarFrontImgPath").val();
        var AdhaarBackImgPath = id == 1 ? "" : getFormInput("AdhaarImageForm", "AdhaarBackImgPath").val();

        $.post(
            "/Users/AdhaarUpdate",
            {
                AdhaarFrontImgPath: AdhaarFrontImgPath,
                AdhaarBackImgPath: AdhaarBackImgPath
            },
            function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                if (!data.Success) {
                    return;
                }
                window.location.reload();
            }
        ).fail(handleAjaxError());
    }

    var showHideSection = function (type) {
        $.ajax({
            type: 'GET',
            url: "/Users/showhideprofilesection?type=" + type,
            dataType: 'json',
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
            },
            error: handleAjaxError()
        });
    }

    var refreshLeads = function () {
        var id = $("#hdnLoggedUserId").val();
        $.get("/Users/UserLeadsList", { id: id }, function (data) {
            if (data) {
                var container = $("#leadsContainer");
                container.empty();
                container.append(data);
            }
        }).fail(function () {
            console.error("Error while Fetching Leads");
        });
    }

    var openOrderPaymentFormModel = function (UserId, fromLogin = false) {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/OpenOrderPaymentFormModal',
            dataType: 'html',
            data: { UserId: UserId },
            success: function (data, strStatus) {
                $("#orderPaymentFormModal").remove();
                $("#nfcCardOrderPaymentFormContainer").empty();
                $('#nfcCardOrderPaymentFormContainer').append(data);
                //createRazorPayOrder();
                submitOrderPaymentForm(fromLogin);
            },
            error: handleAjaxError()
        });
    }

    var submitOrderPaymentForm = function (fromLogin = false) {
        $("#orderPaymentForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'UserId' || element[0].name == 'OrderId' || element[0].name == 'PaymentMethodId') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                UserId: { required: true },
                OrderId: { required: true },
                //paymentScreenshot: { required: true },
                paymentScreenshot: { required: () => $("#IsRazorGateway").val() == "0" },
                PaymentMethodId: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                const PersonalInfoUserForm = document.getElementById("orderPaymentForm");
                var formData = new FormData(PersonalInfoUserForm);//$('form').get(0));

                if (fromLogin) {
                    formData.append("fromLogin", true);
                }
                var btn = $("#btnOrderCartFinish");
                btn.find("div.lds-dual-ring").css('display', 'inline-block');
                btn.prop('disabled', true);
                if ($("#IsRazorGateway").val() == "0") {
                    $.ajax({
                        type: f[0].method,
                        url: f[0].action,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (data, strStatus) {
                            showMessage(data.Message, data.Data, data.Type);
                            if (data.Success) {
                                $("#nfcCardCartWizardModal").modal('hide');
                                $(".modal-backdrop").remove();
                                if (fromLogin) {
                                    window.location.href = "/edit/" + data.OptionalValue;
                                }
                            }
                            btn.find("div.lds-dual-ring").css('display', 'none');
                            btn.prop('disabled', false);
                        },
                        error: function () {
                            btn.prop('disabled', false);
                            btn.find("div.lds-dual-ring").css('display', 'none');
                            handleAjaxError()
                        }
                    });
                } else {
                    createRazorPayOrder((orderId) => {
                        debugger
                        initializeRazorpay(orderId);
                        me.razorPayInsatance.open();
                    });
                    
                }
                
            }
        });
    }

    var openNfcCardCartWizard = function (UserId, fromSignup = false, Status) {
        debugger
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/OpenNfcCardCartWizard',
            dataType: 'html',
            data: { UserId: UserId, fromSignup: fromSignup, Status: Status },
            success: function (data, strStatus) {
                debugger
                $("#nfcCardCartWizardModal").remove();
                $(".modal-backdrop").remove();
                $('body').append(data);
                OpenModel("nfcCardCartWizardModal");
                //submitOrderPaymentForm(fromLogin);
            },
            error: handleAjaxError()
        });
    }

    function setProgressBar(curStep) {
        var steps = $("fieldset").length;
        var percent = parseFloat(100 / steps) * curStep;
        percent = percent.toFixed();
        $(".progress-bar").
            css("width", percent + "%");

    }

    function parseDateByFormat(dateStr, format) {
        try {
            var parts;
            switch (format) {
                case 'MM/YYYY':
                    parts = dateStr.split('/');
                    return new Date(parseInt(parts[1]), parseInt(parts[0]) - 1, 1);
                case 'YYYY-MM':
                    parts = dateStr.split('-');
                    return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, 1);
                case 'DD-MM-YYYY':
                    parts = dateStr.split('-');
                    return new Date(parseInt(parts[2]), parseInt(parts[1]) - 1, parseInt(parts[0]));
                case 'YYYY/MM/DD':
                    parts = dateStr.split('/');
                    return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
                case 'YYYY':
                    return new Date(parseInt(dateStr), 0, 1); // default to Jan 1st of the year
                default:
                    // fallback to Date constructor
                    var fallbackDate = new Date(dateStr);
                    return isNaN(fallbackDate) ? null : fallbackDate;
            }
        } catch (e) {
            return null;
        }
    }


    // New Wizard

    function showNextStep(stepToshown) {
        debugger
        var nextEle = $(`fieldset[data-id="${stepToshown}"]`);
        var currentEle = nextEle.prev();
        $("#progressbar li").eq($("fieldset").index(nextEle)).addClass("active");

        //show the next fieldset
        nextEle.show();
        //hide the current fieldset with style
        currentEle.animate({ opacity: 0 }, {
            step: function (now) {
                // for making fielset appear animation
                w_opacity = 1 - now;

                currentEle.css({
                    'display': 'none',
                    'position': 'relative'
                });

                nextEle.css({ 'opacity': w_opacity });
            },
            duration: 500
        });

        setProgressBar(stepToshown);
    }

    function showPreviousStep(stepToShown) {
        var currentEle = $(`fieldset[data-id="${stepToShown}"]`);
        var prevEle = currentEle.prev();

        //Remove class active
        $("#progressbar li").eq($("fieldset").index(currentEle)).removeClass("active");

        //show the previous fieldset
        prevEle.show();

        //hide the current fieldset with style
        currentEle.animate({ opacity: 0 }, {
            step: function (now) {
                // for making fielset appear animation
                w_opacity = 1 - now;

                currentEle.css({
                    'display': 'none',
                    'position': 'relative'
                });

                prevEle.css({ 'opacity': w_opacity });
            },
            duration: 500
        });

        setProgressBar(stepToshown);
    }

    function nextStep() {
        if (currentStep === 0) {
            wizardData.CardStyle = {
                CardColorId: document.querySelector('input[name="CardColorId"]:checked')?.value || '',
                CardColor: $('input[name="CardColorId"]:checked').data("color"),
                CardBgColor: $('input[name="CardColorId"]:checked').data("bg-color"),
                CardTxtColor: $('input[name="CardColorId"]:checked').data("txt-color"),
                CardFinishId: document.querySelector('input[name="CardFinishId"]:checked')?.value || '',
                CardFinish: $('input[name="CardFinishId"]:checked').data("finish-name"),
                IncludeMetalCard: $("#IncludeMetalCard").is(":checked") || false,
                CardType: $('input[name="CardTypeId"]:checked').data("type"),
                Price: $('input[name="CardTypeId"]:checked').data("price"),
            };
            wizardData.BasicData.CardColorId = document.querySelector('input[name="CardColorId"]:checked')?.value || '';
            wizardData.CardType = $('input[name="CardTypeId"]:checked').data("type");
            wizardData.Price = $('input[name="CardTypeId"]:checked').data("price");
        }

        if (currentStep === 1) {
            wizardData.CardOptions = {
                NfcCardLine1: document.getElementById('NfcCardLine1').value,
                NfcCardLine2: document.getElementById('NfcCardLine2').value,
                NfcCardLine3: document.getElementById('NfcCardLine3').value
            };
        }

        if (currentStep < 3) {
            currentStep++;
            //showNextStep(currentStep);
            showStep(currentStep);
        }

        console.log("Next stepWizard Data:", wizardData);
    }

    function prevStep() {
        if (currentStep == 3) {
            if (confirm("Are you sure to go to previous step?")) {
                if (currentStep > 0) {
                    currentStep--;
                    showStep(currentStep);
                }
            }
        } else {
            if (currentStep > 10) {
                currentStep--;
                showStep(currentStep);
            }
        }

    }

    function finishWizard() {
        wizardData.Address = {
            Shipping: {
                IsSelfPick: $("#IsSelfPick").is(":checked") || false,
                ShippingAddress1: document.getElementById('ShippingAddress1').value,
                ShippingAddress2: document.getElementById('ShippingAddress2').value,
                ShippingCity: document.getElementById('ShippingCity').value,
                ShippingState: document.getElementById('ShippingState').value,
                ShippingCountry: document.getElementById('ShippingCountry').value,
                ShippingZip: document.getElementById('ShippingZip').value
            },
            Billing: {
                chkSameShippingDetails: $("#chkSameShippingDetails").is(":checked") || false,
                BillingAddress1: document.getElementById('BillingAddress1').value,
                BillingAddress2: document.getElementById('BillingAddress2').value,
                BillingCity: document.getElementById('BillingCity').value,
                BillingState: document.getElementById('BillingState').value,
                BillingCountry: document.getElementById('BillingCountry').value,
                BillingZip: document.getElementById('BillingZip').value
            }
        };

        console.log("Wizard Data:", wizardData);
    }

    var initializeNewWizard = function () {
        setProgressBar(currentStep);
        wizardData.BasicData.UserId = parseInt($("#hdnLoggedUserId").val(), 10);
        wizardData.BasicData.OrderId = ($("#hdnOrderId").val() != null && $("#hdnOrderId").val().trim() != "") ? parseInt($("#hdnOrderId").val(), 10) : 0;
        currentStep = 0
        showStep(0);
        $(document).on("click", ".wizard-next", nextStep);

        $(document).on("click", ".wizard-prev", prevStep);

        updateNfcCardDetails();
        submitOrderUpsertForm();
        submitOrderPaymentForm();

    }

    // Start Nfc Card Details

    function setNfcCardDetailsPreview() {
        debugger
        $("#choosenCardColor").html("<span>Card Color</span>" + wizardData.CardStyle.CardColor);
        $("#choosenCardColorFinalStep").html(wizardData.CardStyle.CardColor);
        $("#choosenCardFinish").html("<span>Card Finish</span>" + wizardData.CardStyle.CardFinish);
        $("#choosenCardFinishFinalStep").html(wizardData.CardStyle.CardFinish);
        $("#choosenCardMaterial").html("<span>Card Material</span>" + (wizardData.CardStyle.IncludeMetalCard ? "Include Metal NFC Card" : "NFC Card"));
        $("#choosenCardMaterialFinalStep").html(wizardData.CardStyle.IncludeMetalCard ? "Include Metal NFC Card" : "NFC Card");
        $(".preview-nfc-card").css({
            "background-color": wizardData.CardStyle.CardBgColor,
            "color": wizardData.CardStyle.CardTxtColor
        });
        $(".info h1").css("color", wizardData.CardStyle.CardTxtColor);
        $(".info p").css("color", wizardData.CardStyle.CardTxtColor);

        if (wizardData.BasicData.CardColorId === "1") {
            // If Black color
            $(".nfcQrImage").attr("src", "/HomeAssets/img/qr_bt.jpg");
        } else {
            $(".nfcQrImage").attr("src", "/HomeAssets/img/transparentQrImage.png");
        }
    }

    var updateNfcCardDetails = function () {
        $("#nfcCardDetailForm").on("keyup", "#NfcCardLine1", function () {
            $(".textNfcCardLine1").html($(this).val());
        });

        $("#nfcCardDetailForm").on("keyup", "#NfcCardLine2", function () {
            $(".textNfcCardLine2").html($(this).val());
        });

        $("#nfcCardDetailForm").on("keyup", "#NfcCardLine3", function () {
            $(".textNfcCardLine3").html($(this).val());
        });

        $("#nfcCardDetailForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'UserId' || element[0].name == 'CardColorId' || element[0].name == 'NfcCardLine1') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                //UserId: { required: true },
                //CardColorId: { required: true },
                NfcCardLine1: { required: true },
            },
            messages: {
                NfcCardLine1: {
                    required: "Name is required."
                }
            },
            submitHandler: function (form) {
                nextStep();
            }
        });
    }
    // End Nfc Card Details

    // Start Shipping address
    var isShippingRequired = function () {
        var isSelf = $("#IsSelfPick").is(":checked");

        return !isSelf;
    }
    var submitOrderUpsertForm = function () {
        $("#orderUpsertForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'ShippingAddress1' || element[0].name == 'ShippingAddress2' || element[0].name == 'ShippingCity' ||
                    element[0].name == 'ShippingState' || element[0].name == 'ShippingCountry' || element[0].name == 'ShippingZip' || element[0].name == 'BillingAddress1' ||
                    element[0].name == 'BillingAddress2' || element[0].name == 'BillingCity' || element[0].name == 'BillingState' ||
                    element[0].name == 'BillingCountry' || element[0].name == 'BillingZip') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                ShippingAddress1: { required: isShippingRequired },
                ShippingCity: { required: isShippingRequired },
                ShippingState: { required: isShippingRequired },
                ShippingCountry: { required: isShippingRequired },
                ShippingZip: { required: isShippingRequired },
                BillingAddress1: { required: true },
                BillingCity: { required: true },
                BillingState: { required: true },
                BillingCountry: { required: true },
                BillingZip: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);


                finishWizard();
                if (confirm("Are you sure you have filled all details, to go to final step?")) {
                    $.ajax({
                        type: f[0].method,
                        url: f[0].action,
                        data: wizardData,
                        dataType: 'json',
                        success: function (res, strStatus) {
                            debugger
                            if (!res.Success) {
                                showMessage("Failed!", res.Message, res.Type);
                                return;
                            }
                            var isAnyPaymentDoneBefore = $("#hdnIsAnyPaymentDoneBefore").val() == "True";
                            

                            wizardData.payment.InitialData = {
                                PackageCost: res.Data.PackageCost,
                                ReferralDiscount: isAnyPaymentDoneBefore ? 0 : (res.Data.ReferralDiscount || 0),
                                ShippingCost: res.Data.ShippingCost || 0,
                                Tax: res.Data.Tax,
                                CouponDiscount: 0,
                                PaymentAmount: isAnyPaymentDoneBefore ? parseFloat(res.Data.PackageCost) : (parseFloat(res.Data.PackageCost) - (parseFloat(res.Data.ReferralDiscount) || 0)),
                                MetalCardAmount: 0
                            };

                            

                            wizardData.payment.updatedData = {
                                PackageCost: res.Data.PackageCost,
                                ReferralDiscount: isAnyPaymentDoneBefore ? 0 : (res.Data.ReferralDiscount || 0),
                                ShippingCost: res.Data.ShippingCost || 0,
                                Tax: res.Data.Tax,
                                TotalDiscount: res.Data.ReferralDiscount || 0,
                                DiscountedPrice: isAnyPaymentDoneBefore ? 0 : (parseFloat(res.Data.PackageCost) - (parseFloat(res.Data.ReferralDiscount) || 0)),
                                CouponDiscount: 0,
                                PaymentAmount: isAnyPaymentDoneBefore ? parseFloat(res.Data.PackageCost) : (parseFloat(res.Data.PackageCost) - (parseFloat(res.Data.ReferralDiscount) || 0)),
                                MetalCardAmount: 0
                            };
                            console.log("wizard data", wizardData);
                            displayPaymentDetails();
                            nextStep();
                        },
                        error: function () {
                            handleAjaxError()
                        }
                    });
                }

            }
        });
    }

    // End Shipping address

    // Final Step Payment


    var createRazorPayOrder = function (callback) {
        debugger
        var paymentAmount = wizardData.payment.updatedData.PaymentAmount;
        var orderId = wizardData.BasicData.OrderId;
        var UserId = wizardData.BasicData.UserId;
        if (!me.isValidCoupon && $("#DiscountCoupon").val() != "") {

            showMessage("Failed!", "Coupon code is not valid or expired", "error");
            return false;
        }
        $.post(
            "/Users/CreateRazorpayOrder",
            { PaymentAmount: paymentAmount, OrderId: orderId, UserId: UserId },
            function (res) {
                debugger
                if (!res.Success) {
                    showMessage(res.Message, res.Data, res.Type);
                    return;
                }

                me.RazorPayLatestOrderId = res.Data.RazorOorderId;
                console.log("create razor pay order result", res);
                wizardData.BasicData.RazorPayOrderId = res.Data.RazorOorderId;
                if (callback) {
                    callback(res.Data.RazorOorderId, res.Data.RazorOrder);
                }
            }
        ).fail(handleAjaxError(function () {
            debugger;
        }));
    }

    var initializeRazorpay = function (orderId, orderDetails) {
        console.log("Order Details", orderDetails);
        var options = {
            "key": me.razorPayApiKey, // Replace with your Key ID
            "amount": parseFloat(getFormInput("orderPaymentForm", "PaymentAmount").val()) * 100,  // Amount in paise      according to docs its mandatory but i think its optional
            "currency": "INR",
            "order_id": orderId,
            "name": "Bharat Touch - NFC Card Order",
            "description": "NFC Card Order",       // Optional
            "image": "/SmartTheme/images/bharattouchfavico.ico", // Optional path to your logo
            "handler": function (response) {
                console.log('handler response', response);
                $("#WantMetalCard").val(wizardData.CardStyle.IncludeMetalCard ?? false);
                const PersonalInfoUserForm = document.getElementById("orderPaymentForm");
                var formData = new FormData(PersonalInfoUserForm);
                formData.append("razorpay_payment_id", response.razorpay_payment_id);
                formData.append("razorpay_order_id", response.razorpay_order_id);
                formData.append("razorpay_signature", response.razorpay_signature);
                formData.append("OrderId", wizardData.BasicData.OrderId);
                formData.append("UserId", wizardData.BasicData.UserId);
                $.ajax({
                    url: '/Users/VerifyAndSaveNfcOrderPayment', // Your payment verification URL
                    type: 'POST',
                    dataType: 'json',
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        showMessage(data.Message, data.Data, data.Type);
                        if (!data.Success) {
                            me.razorPayInsatance = null;
                            return;
                        }
                        $("#paymentWizardModal").modal("hide");
                        window.location.reload();
                    },

                    error: handleAjaxError(function () {
                        me.razorPayInsatance = null;
                    })

                });
            },
            "modal": {
                "ondismiss": function (cancelRes) {
                    console.log("paymentCancelRes", cancelRes);
                    console.log("Payment popup closed by user");
                    showMessage("Cancelled", "You closed the payment popup before completing payment.", "error");

                    //var btn = $("#btnOrderCartFinish");
                    //btn.find("div.lds-dual-ring").css('display', 'none');
                    //btn.prop('disabled', false);
                }
            },
            "prefill": {
                "name": "",
                "email": "",
                "contact": ""
            },
            "theme": {
                "color": "#F3722C"
            }
        };

        me.razorPayInsatance = new Razorpay(options);

        me.razorPayInsatance.on('payment.failed', function (response) {
            console.log("Complete PaymentFailed Error", response);
            showMessage("Failed!", response.error.description, "error");

        });
    }

    var submitOrderPaymentForm = function () {
        $("#orderPaymentForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'UserId' || element[0].name == 'OrderId' || element[0].name == 'PaymentMethodId') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            submitHandler: function (form) {
                createRazorPayOrder((orderId, orderDetails) => {
                    initializeRazorpay(orderId, orderDetails);
                    me.razorPayInsatance.open();
                });
            }
        });
    }

    var displayPaymentDetails = function () {
        debugger
        var fe = getFormInputByName("orderPaymentForm");
        var d = wizardData.payment.updatedData;

        $("#mainPackageCost").html("Rs." + parseFloat(d.PackageCost));
        //$("#mainPackageCost").html("<del>Rs. " + parseFloat(d.PackageCost) + "</del>");   // for line-through

        $("#displayReferralDiscount").html("Rs. " + parseFloat(d.ReferralDiscount));

        if (d.ShippingCost == 0) {
            $("#displayShippingFee").html("Free");
        } else {
            $("#displayShippingFee").html("Rs. " + parseFloat(d.ShippingCost));
        }
        getFormInput("orderPaymentForm", "ShippingAmount").val(parseFloat(d.ShippingCost));

        if (d.MetalCardAmount != 0) {
            $("#metalCardPriceContainer").show();
        } else {
            $("#metalCardPriceContainer").hide();
        }
        var isAnyPaymentDoneBefore = $("#hdnIsAnyPaymentDoneBefore").val() == "True";
        if (isAnyPaymentDoneBefore) {
            d.CouponDiscount = 0;
            $("#couponCodeContainer").hide();
        } else {
            $("#couponCodeContainer").show();
        }
        $("#CouponDiscount").val(parseFloat(d.CouponDiscount));
        $("#displayCouponDiscount").html("Rs. " + parseFloat(d.CouponDiscount).toFixed(2));
        if (d.CouponDiscount == 0) {
            $("#CouponDiscountContainer").hide();
        }
        else {
            $("#CouponDiscountContainer").show();
        }

        d.subTotal = ((parseFloat(d.PackageCost) + parseFloat(d.ShippingCost) + parseFloat(d.MetalCardAmount)) - parseFloat(d.ReferralDiscount) - parseFloat(d.CouponDiscount)).toFixed(2);
        $("#displaySubTotal").html("Rs. " + d.subTotal);

        d.Tax = parseFloat(d.subTotal * 0.18).toFixed(2);

        if (me.isTaxApplicable == "False") {
            $("#taxDisplay").html("Included");
            getFormInput("orderPaymentForm", "Tax").val(0);
            d.PaymentAmount = parseFloat(d.subTotal).toFixed(2)
        } else {
            $("#taxDisplay").html("Rs. " + parseFloat(d.Tax));
            getFormInput("orderPaymentForm", "Tax").val(parseFloat(d.Tax));
            d.PaymentAmount = (parseFloat(d.subTotal) + parseFloat(d.Tax)).toFixed(2)
        }

        fe("PaymentAmount").val(d.PaymentAmount);
        $("#displayTotal").html("Rs. " + parseFloat(d.PaymentAmount));
    }

    var setDefaultDiscountPrice = function () {
        var d = wizardData.payment.InitialData;
        wizardData.payment.updatedData = {
            PackageCost: d.PackageCost,
            ReferralDiscount: d.ReferralDiscount,
            ShippingCost: d.ShippingCost,
            Tax: d.Tax,
            TotalDiscount: d.ReferralDiscount,
            DiscountedPrice: parseFloat(d.PackageCost) - (parseFloat(d.ReferralDiscount) || 0),
            CouponDiscount: 0,
            PaymentAmount: parseFloat(d.PackageCost) - (parseFloat(d.ReferralDiscount) || 0) + (parseFloat(d.ShippingCost) || 0),
            MetalCardAmount: d.MetalCardAmount
        };
        displayPaymentDetails();
    }

    var checkValidDiscountCoupon = function (event) {
        event.preventDefault();
        setDefaultDiscountPrice();

        var code = $("#DiscountCoupon").val();
        $.post("/Users/CheckDiscountCouponValid", { Code: code }, function (res) {

            var d = wizardData.payment.updatedData;
            debugger
            //$("#btnOrderCartFinish").prop("disabled", false);
            if (res.Success) {
                me.isValidCoupon = true;
                var percentOff = res.Data.PercentageOff;
                var amountOff = res.Data.AmountOff;

                if (percentOff != null && percentOff != 0) {
                    d.CouponDiscount = (parseFloat(d.DiscountedPrice) * parseFloat(parseFloat(percentOff) / 100)) || 0;
                    //d.DiscountedPrice = (parseFloat(d.DiscountedPrice) - (parseFloat(d.DiscountedPrice) * parseFloat(parseFloat(percentOff) / 100))).toFixed(2);
                    //d.TotalDiscount = ((parseFloat(d.DiscountedPrice) * parseFloat(parseFloat(percentOff) / 100)) + parseFloat(d.ReferralDiscount)).toFixed(2);
                }
                if (amountOff != null && amountOff != 0) {
                    d.CouponDiscount = parseFloat(amountOff) || 0;
                    //d.DiscountedPrice = (parseFloat(d.DiscountedPrice) - parseFloat(amountOff)).toFixed(2);
                    //d.TotalDiscount = (parseFloat(amountOff) + parseFloat(d.ReferralDiscount)).toFixed(2);
                }
                //d.Tax = parseFloat(parseFloat(d.DiscountedPrice) * 0.18).toFixed(2);
                $("#DiscountCoupon").prop("readonly", true);
                displayPaymentDetails();

                $("#btnApplyDiscountCoupon").hide();
                $("#btnCancelDiscountCoupon").show();
                return;
            }
            else {
                me.isValidCoupon = false;
                showMessage("Failed!", "Coupon code is not valid or expired", "error");
            }
            $("#btnApplyDiscountCoupon").hide();
            $("#btnCancelDiscountCoupon").show();

        }).fail(handleAjaxError(function () {
            me.isValidCoupon = false;
            showMessage("Failed!", "Coupon code is not valid or expired", "error");
            $("#btnApplyDiscountCoupon").hide();
            $("#btnCancelDiscountCoupon").show();
        }));
    }

    // End Final Step Payment


    this.init = function () {
        upsertMultipleDays();
        getWeekDays();
        multipleDatePickerInit();
        //BindScheduleOpenDayList();
        uploadCardThemeBgImageForm();
        fetchCoverImages(null);
        loadCardThemeTemplate(null, true);
        //fetchCurrentLocation();
        BindDdlCounpanyType(true);
        ClientTestimonialForm();
        bindClientTestimonialImages();
        openUserCertificationModel(0);
        bindUserCertificationList();
        upsertAdhaarImageForm();
        //loadnews();
        $("#ddlCompanyTypeParentId").on('change', () =>  BindDdlCounpanyType(false));
        $("#ddlCompanyTypeId").on('change', function () {
            if ($(this).val() != "0") {
                var value = $(this).find(':selected').text();
                $("#PersonalInfoUserForm").find("input[name=CompanyType]").val(value);
            } else {
                $("#PersonalInfoUserForm").find("input[name=CompanyType]").val("");
            }
        });

        $("#btndeleteAdhaarFront").click(function () {
            setAndShowDeleteModel({ callback: () => deleteAdhaarImage(0) })
        })

        $("#btndeleteAdhaarBack").click(function () {
            setAndShowDeleteModel({ callback: () => deleteAdhaarImage(1) })
        })


        ClassicEditor
            .create(
                document.querySelector('#AboutDescription')
                //{
                //    toolbar: ['imageUpload', '|', 'bold', 'italic', 'link', 'heading'],
                //    image: {
                //        toolbar: ['imageTextAlternative', 'imageStyle:full', 'imageStyle:side']
                //    },
                //    simpleUpload: {
                //        uploadUrl: '/Users/UploadImage', // Your MVC upload API
                //        withCredentials: false
                //    }
                //}
            )
            .then(editor => {
                editor.ui.view.editable.element.style.height = '400px';

                if (me.companyId > 0) {
                    editor.isReadOnly;
                    editor.enableReadOnlyMode('my-feature-id');
                    editor.isReadOnly; 
                }
               
                console.log('CKEditor 5 is ready.');
            })
            .catch(error => {
                console.error('There was an error initializing CKEditor 5:', error);
            });



        $("#btnClientTestimonialEditCancel").on('click', resetClientTestimonialForm);

        $("#dvOpenCalanderDaysChkContainer").on("change", ".chkField", upsertScheduleOpenWeekDaysForm);

        $("#btnOpenCoverImagePropertyModal").on("click", openCoverImagePropertyModal);

        $(window).on("load", async function () {
            debugger
            if (document.referrer === "") {
                console.log("User accessed the page directly.");
            } else if (document.referrer.toLowerCase() == me.WebUrl.toLowerCase() + "home/login" || document.referrer.toLowerCase() == me.WebUrl.toLowerCase()) {
                console.log("Authenticate From login");
                if ($("#hdnUserPaymentStatus").val() == 1){
                    $("#paymentWizardModal").modal("show");
                    $("#paymentWizardModal").on("shown.bs.modal", initializeNewWizard);
                }
            } else {
                console.log("User was redirected from:", document.referrer);
            }
            await GoogleOAuthInit()
            await MicrosoftOAuthInit();
        });

        $("#btnOrderNewCard").on('click', function () {
            debugger
            var btn = $(this);
            var loader = $(this).find("div.lds-dual-ring");
            var companyId = $("#hdnCompanyId").val();
            if (companyId != null && parseInt(companyId, 10) > 0) {
                return;
            }

            if ($("#hdnIsLastOrderPending").val() == "True" && parseInt($("#hdnOrderId").val(), 10) > 0) {
                $("#paymentWizardModal").modal("show");
                $("#paymentWizardModal").on("shown.bs.modal", initializeNewWizard);
            } else {
                loader.css('display', 'inline-block');
                btn.prop('disabled', true);
                $.post("/Users/CreateNewCardOrder", { UserId: $("#hdnLoggedUserId").val() }, function (res) {
                    loader.css('display', 'none');
                    btn.prop('disabled', false);
                    if (!res.Success) {
                        showMessage("Failed!", res.Message, res.Type);
                        return;
                    }
                    wizardData.BasicData.OrderId = res.Data;
                    $("#hdnOrderId").val(res.Data);
                    $("#hdnIsLastOrderPending").val("True");
                    $("#paymentWizardModal").modal("show");
                    $("#paymentWizardModal").on("shown.bs.modal", initializeNewWizard);
                }).fail(handleAjaxError(function () {
                    loader.css('display', 'none');
                    btn.prop('disabled', false);
                }));
            }
        });

        $("#btnSyncCalendarWithGoogle").on("click", function () {
            tokenClient.requestCode();
        });

        createUserForm();
        personalInfoForm();
        contactInfoForm();
        aboutInfoForm();
        openProfessionalModel('');
        bindProfessional();
        saveSocialMedia();
        openEducationModel();
        bindEducation();
        PortfolioImageForm();
        bindPortfolioImages();
        bindTeamImages();
        TeamImageForm();
        paymentQrForm();
        coverImageForm();

        //blog
        bindBlog();
        openBlogModel();

        //youtube
        bindYouTube();
        openYouTubeModel();


        $("#btnSocialLink").click(function () {
          return  validateYoutubeUrl();
        });
        //$("#btnUpdatePassword").click(function () {
        //    var oldPassword = $("#Password").val();
        //    var currentPassword = $("#CurrentPassword").val();
        //    var newPassword = $("#NewPassword").val();
        //    var confirmPassword = $("#ConfirmPassword").val();

        //    if (currentPassword == "") {
        //        showMessage("Warning!", "Enter current password.", "notice");
        //        return false;
        //    }

        //    if (newPassword == "") {
        //        showMessage("Warning!", "Enter new password.", "notice");
        //        return false;
        //    }


        //    if (confirmPassword == "") {
        //        showMessage("Warning!", "Enter confirm password.", "notice");
        //        return false;
        //    }

        //    if (oldPassword != currentPassword) {
        //        showMessage("Warning!", "Current password is wrong.", "notice");
        //        return false;
        //    }
        //    if (newPassword != confirmPassword) {
        //        showMessage("Warning!", "New password and confirm password not matched.", "notice");
        //        return false;
        //    }

        //    if (oldPassword == newPassword) {
        //        showMessage("Warning!", "Password should not be same as old password.", "notice");
        //        return false;
        //    }

        //    updatePassword($("#UserId").val(), newPassword);

        //});

        $("#createFormContainer").on("click", "button", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Users/index";
                    break;
            }
        });

        $("#btndeleteportfolio").on("click", function () {

            $("#exampleModal").modal("show");
            $("#txtfiletype").val("P");
            $("#txtfilepath").val($("#txtportfoliolink").val());

            console.log($("#txtportfoliolink").val());
            //href = "/Users/DeleteFileFromPath?type=P&fileName=@Model.PortfolioLink"
        });

        $("#btndeleteresume").on("click", function () {
            console.log("Test");
            $("#txtfiletype").val("R");
            $("#exampleModal").modal("show");
            $("#txtfilepath").val($("#txtresumelink").val());
            console.log($("#txtresumelink").val());
            //href="/Users/DeleteFileFromPath?type=R&fileName=@Model.ResumeLink"
        });

        $("#btndeletePaymentQr").on("click", function () {
            var obj = $(this);
            var path = obj.data("path");
            setAndShowDeleteModel({
                callback: () => deletePaymentDetails(path)
            });
        });

        $("#btndeleteservice").on("click", function () {
            console.log("Test");
            $("#txtfiletype").val("S");
            $("#exampleModal").modal("show");
            $("#txtfilepath").val($("#txtservicelink").val());

            console.log($("#txtservicelink").val());
            //href = "/Users/DeleteFileFromPath?type=P&fileName=@Model.PortfolioLink"
        });

        $("#btndeletemenulink").on("click", function () {
            console.log("Test");
            $("#txtfiletype").val("M");
            $("#exampleModal").modal("show");
            $("#txtfilepath").val($("#txtmenulink").val());
            console.log($("#txtmenulink").val());

            //href = "/Users/DeleteFileFromPath?type=P&fileName=@Model.PortfolioLink"
        });

        $("#btndeletefile").on("click", function () {
            $("#txtfiletype").val();
            $("#txtfilepath").val();
            $("#btndeletefile").find("div.lds-dual-ring").css('display', 'inline-block');
            $("#btndeletefile").prop('disabled', true);
            $.ajax({
                type: 'GET',
                url: "/Users/DeleteFileFromPath?type=" + $("#txtfiletype").val() + "&filename=" + $("#txtfilepath").val(),
                dataType: 'json',
                success: function (data, strStatus) {
                    showMessage(data.Message, data.Data, data.Type);
                    $("#btndeletefile").find("div.lds-dual-ring").css('display', 'none'); 
                    $("#btndeletefile").prop('disabled', false);
                    $("#exampleModal").modal("hide");
                    window.location.reload();
                },
                error: function () {
                    $("#btndeletefile").find("div.lds-dual-ring").css('display', 'none'); 
                    $("#btndeletefile").prop('disabled', false);
                    handleAjaxError()
                }
            });
        });

        $("#checkshowabout").on("change", function () {
            showHideSection('AB');
        });

        $("#checkshowexperience").on("change", function () {
            showHideSection("EX");
        });

        $("#checkshowskill").on("change", function () {
            showHideSection("SK");
        });

        $("#checkshoweducation").on("change", function () {
            showHideSection("ED");
        });

        $("#checkshowsocial").on("change", function () {
            showHideSection("SO");
        });

        $("#checkshowgallery").on("change", function () {
            showHideSection("GL");
        });
        $("#checkshowteam").on("change", function () {
            showHideSection("TM");
        });

        $("#checkshowtestimonial").on("change", function () {
            showHideSection("CTM");
        });

        $("#checkshowblog").on("change", function () {
            showHideSection("BL");
        });

        $("#checkshowyoutube").on("change", function () {
            showHideSection("YT");
        });

        $("#checkshowCertification").on("change", function () {
            showHideSection("UTC");
        });

        $("#checkshowAdhaar").on("change", function () {
            showHideSection("ADC");
        });

        //$(document).on("click", ".select-card-theme", onSelectCardTheme);
        $("#chkBusinessRelatedThemes").on("change", () => loadCardThemeTemplate(null, true));
        $("#btnLoadMoreCardTemplateTheme").on("click", function () {
            loadCardThemeTemplate($(this), false);
        });

        $("#btnRemoveCardThemeBgImg").on("click", function () {
            var obj = $(this);
            setAndShowDeleteModel({
                BackgroundImg: obj.data("path"),
                UserId: obj.data("userid"),
                TemplateId: obj.data("templateid"),
                callback: removeCardThemeBg
            });
        });

        $("#btnLoadMoreCoverImg").click(function () {
            fetchCoverImages($(this));
        });

        $(document).on("click", ".btnSelectCoverImg", function () {
            var ProfileImage = $("#hdnProfileImage").val();
            var CoverImage = $(this).data("path");
            //uploadProfileAndCoverImage(ProfileImage, CoverImage);
            onSelectCoverImageFile(CoverImage, "link");
        });
        
        $(document).on("click", ".btnDeleteLead", function () {
            var id = $(this).data("id");
            setAndShowDeleteModel({
                callback: () => {
                    $.post("/Users/DeleteLeadById", { LeadId: id }, function (data) {
                        showMessage(data.Message, data.Data, data.Type);
                        if (data.Success) {
                            refreshLeads();
                        }
                    })
                }
            });
        });

        $("#btnRemoveCoverImg").on('click', function () {
            var path = $(this).data("path");
            var UserId = $(this).data("userid");
            setAndShowDeleteModel({ callback: () => RemoveCoverImage({ UserId: UserId, path: path }) })
        });

        $('#btnDeleteModel').on('click', function () {
            var callback = $('#deleteModelData').data('callback');
            var data = $('#deleteModelData').data();
            if (typeof callback === 'function') {
                $("#deleteModel").modal("hide");
                callback(data);
            }
        });
        //document.addEventListener('DOMContentLoaded', function () {
        //    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        //    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        //        return new bootstrap.Tooltip(tooltipTriggerEl);
        //    });
        //});

        //prependCountryCode();
        //$('#CountryCode').change(prependCountryCode);

        $(".custom-date-picker").datepicker({
            autoclose: true,
            todayHighlight: 'TRUE',
            format: "dd/mm/yyyy"
        });

        $.validator.addMethod("customRegex", function (value, element, param) {
            return this.optional(element) || param.test(value);
        }, "Please enter a valid value.");

        $.validator.addMethod("greaterThanStartDate", function (value, element, param) {
            var startDate = $(param.elem).val();
            var format = param.format || 'MM/YYYY';
            if (!startDate || !value) {
                return true;
            }
            startDate = parseDateByFormat(startDate, format);
            var endDate = parseDateByFormat(value, format);
            if (!startDate || !endDate) {
                return false;
            }
            return endDate >= startDate;
            //return new Date(value) >= new Date(startDate);
        }, "End Date cannot be earlier than Start Date");

        changePasswordForm();

        $(document).on('change', 'input[name=rdProfileTemplate]', function () {
            var loader = $("#selectProfileTemplateLoader");
            loader.css('display', 'inline-block');
            var selectedTemplate = $("#selectedProfileTemplateId");
            var id = $(this).attr('id');
            var templateId = id.split('_')[1];
            $.post("/Users/ChangeUserProfileTemplate", { ProfileTemplateId: templateId }, function (res) {
                showMessage(res.Message, res.Data, res.Type);
                loader.css('display', 'none');
                if (res.Success) {
                    window.location.reload();
                } else {
                    $("#template_" + selectedTemplate.val()).prop("checked", true);
                }
            }).fail(handleAjaxError(function () {
                $("#template_" + selectedTemplate.val()).prop("checked", true);
                loader.css('display', 'none');
            }));
        });


        $(document).on('change', "#chkSameShippingDetails", function () {
            var isChecked = $(this).is(':checked');
            var fetchInput = getFormInputByName("orderUpsertForm");

            function syncBillingWithShipping() {
                if ($("#chkSameShippingDetails").is(":checked")) {
                    fetchInput("BillingAddress1").val(fetchInput("ShippingAddress1").val()).prop("readonly", true);
                    fetchInput("BillingAddress2").val(fetchInput("ShippingAddress2").val()).prop("readonly", true);
                    fetchInput("BillingCity").val(fetchInput("ShippingCity").val()).prop("readonly", true);
                    fetchInput("BillingState").val(fetchInput("ShippingState").val()).prop("readonly", true);
                    fetchInput("BillingCountry").val(fetchInput("ShippingCountry").val()).prop("readonly", true);
                    fetchInput("BillingZip").val(fetchInput("ShippingZip").val()).prop("readonly", true);
                }
            }

            if (isChecked) {
                syncBillingWithShipping();

                $("#ShippingAddress1, #ShippingAddress2, #ShippingCity, #ShippingState, #ShippingCountry, #ShippingZip").on("input", syncBillingWithShipping);
            } else {
                fetchInput("BillingAddress1").prop("readonly", false);
                fetchInput("BillingAddress2").prop("readonly", false);
                fetchInput("BillingCity").prop("readonly", false);
                fetchInput("BillingState").prop("readonly", false);
                fetchInput("BillingCountry").prop("readonly", false);
                fetchInput("BillingZip").prop("readonly", false);

                $("#ShippingAddress1, #ShippingAddress2, #ShippingCity, #ShippingState, #ShippingCountry, #ShippingZip").off("input", syncBillingWithShipping);
            }
        });

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

        $("#btnNextSelectNfc").click(function () {
            nextStep();
            setNfcCardDetailsPreview();
        });

        $("#btnApplyDiscountCoupon").on("click", checkValidDiscountCoupon);
        $("#btnCancelDiscountCoupon").on("click", function (event) {
            event.preventDefault();
            $("#DiscountCoupon").prop("readonly", false);
            $("#DiscountCoupon").val("");
            setDefaultDiscountPrice();
            $("#btnCancelDiscountCoupon").hide();
            $("#btnApplyDiscountCoupon").show();
        });
    }
}