var authenticationUser = function () {
    var me = this;
    this.authUserUrl = null;
    this.captchaSiteKey = null;
    this.googleClientId = null;
    this.microsoftClientId = null;
    this.webUrl = null;
    var isDuplicateEmail = false;
    var isDuplicateDisplayName = false;
    var signupFormWidgetId = null;
    var sendEmailContactHomeWidgetId = null;
    var googleClient = null;
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


    var loginUserForm = function () {
        debugger;
        $("#loginForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Email' || element[0].name == 'Password') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                Email: { required: true, email: true },
                Password: { required: true }
            },
            submitHandler: function (form) {
                debugger;
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
                        debugger;
                        if (data.Success == true) {
                            showMessage(data.Message, data.Data, data.Type);
                            f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                            f.find(":submit").prop('disabled', false);
                            window.location.href = "/edit/" + data.OptionalValue;// "/User/editprofile/" + data.OptionalValue;
                            return;
                        }
                        if (data.Success == false && data.Type == "error") {
                            showMessage(data.Message, data.Data, data.Type);
                            f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                            f.find(":submit").prop('disabled', false);
                            return;
                        }

                        var splittedValues = data.OptionalValue.split(";");
                        var UserId = splittedValues[0];
                        var status = splittedValues[1];
                        $("#loginModal").modal('hide');
                        $(".modal-backdrop").remove();
                        if (status == "1") {
                            openOrderUpsertModel(UserId, data.Data, true);
                        } else if (status == "2") {
                            openOrderPaymentFormModel(UserId, true);
                        }
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    },
                    error: function () {
                        debugger;
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        handleAjaxError()
                    }
                });
            }
        });

    }

    var signUpForm = function () {
        if (grecaptcha != undefined && grecaptcha != null) {
            signupFormWidgetId = grecaptcha.render('grecaptchaSignup', {
                sitekey: me.captchaSiteKey,
                type: 'image', 
                callback: function (token) {
                    $('#hdngrecaptchaResponseSignup').val(token);
                },
                'expired-callback': function () {
                    $('#hdngrecaptchaResponseSignup').val("");
                },
                'error-callback': function () {
                    $('#hdngrecaptchaResponseSignup').val("");
                }
            });
        }

        $("#CreateUserForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'FirstName' || element[0].name == 'EmailId' || element[0].name == 'Password') {
                    error.appendTo(element.parent().parent());
                }
                else if (element[0].name == "txtCaptchaInput" || element[0].name == "chkPolicy") {
                    error.appendTo(element.parent().parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                FirstName: { required: true, letterswithspace: true },
                EmailId: { required: true, emailvalidatecustom: true },
                Password: {
                    required: true,
                    customRegex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&#]{8,16}$/

                },
                //UserType: { required: true },
                ConfirmPassword: { required: true, equalTo: '#txtRegPassword' },
                Displayname: { required: true },
                PackageId: { required: true }
            },
            messages: {
                Password: {
                    customRegex: "password should between 8 - 16 characters which contain minimum (1 lowercase letter, 1 uppercase letter and 1 number)."
                },
                ConfirmPassword: {
                    equalTo: "Password and Confirm password must be same"
                }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var response = null;
                data = data.map(function (item, i) {
                    if (item.name == 'g-recaptcha-response') {
                        item.name = 'grecaptchaResponse';
                        response = item.value;
                    }
                    return item;
                });
                //var response = grecaptcha.getResponse(widgetId);
                if (response === null || response.length === 0) {
                    showMessage('Notice!', 'Please verify that you are not a robot.', 'notice');
                    return;
                }    
                var formData;
                formData = new FormData($('form').get(0));
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });
                submitSignupForm(f, formData, isDuplicateEmail, $('#CreateUserForm').find(`select[name=PackageId]`).val());

            }
        });
    }

    var submitSignupForm = function (f, formData, isDuplicateEmail, packageId) {
        if (isDuplicateEmail && $("#UserId").val() == "0")
            return false;


        var Password = $("#txtRegPassword").val();
        var confirmPassword = $("#txtRegConfirmPassword").val();


        if (Password != confirmPassword) {
            showMessage("Warning!", "Password and confirm password not matched.", "notice");
            return false;
        }

        if ($("#Displayname").val() == "")
        {
            showMessage("Warning!", "Display name is required", "notice");
            return false;
        }
        if (isDuplicateDisplayName)
        {
            showMessage("Warning!", "Display name is already exist", "notice");
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
                //if (grecaptcha != undefined && grecaptcha != null) {
                //    grecaptcha.reset(signupFormWidgetId);
                //}
                if (!data.Success) {
                    showMessage("Failed!", data.Message, data.Type);
                    f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                    f.find(":submit").prop('disabled', false);
                    return;
                } 

                //const CreateUserForm = document.getElementById("CreateUserForm");
                //CreateUserForm.reset();
                //$("#SignupModal").modal('hide');
                //$(".modal-backdrop").remove();

                openNfcCardCartWizard(data.Data, true, () => {
                    $("#SignupModal").modal('hide');
                    f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                    f.find(":submit").prop('disabled', false);
                });
                //openOrderUpsertModel(data.OptionalValue, data.Data, false);
                
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
            },
            error: function () {
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none'); handleAjaxError();
                f.find(":submit").prop('disabled', false);
                if (grecaptcha != undefined && grecaptcha != null) {
                    grecaptcha.reset(signupFormWidgetId);
                }
            }
        });
    }


    var openNfcCardCartWizard = function (obj, fromSignup = false, callback) {
        var UserId = obj.UserId;
        var OrderId = obj.OrderId;
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/OpenNfcCardCartWizard',
            dataType: 'html',
            data: { UserId: UserId, OrderId: OrderId },
            success: function (data, strStatus) {
                if (callback) {
                    callback();
                }
                $("#nfcCardCartWizardModal").remove();
                $(".modal-backdrop").remove();
                $('body').append(data);
                OpenModel("nfcCardCartWizardModal");
                //submitOrderPaymentForm(fromLogin);
            },
            error: handleAjaxError()
        });
    }
    function sendEmailContactForm() {
        if (grecaptcha != undefined && grecaptcha != null) {
            sendEmailContactHomeWidgetId = grecaptcha.render('grecaptchaHomeContact', {
                sitekey: me.captchaSiteKey,
                type: 'image', 
                callback: function (token) {
                    $('#hdngrecaptchaResponseContactForm').val(token);
                },
                'expired-callback': function () {
                    $('#hdngrecaptchaResponseContactForm').val("");
                },
                'error-callback': function () {
                    $('#hdngrecaptchaResponseContactForm').val("");
                }
            });
        }


        jQuery.validator.addMethod("minNumberlength", function (value, element) {
            var countryValue = $("#ddlCountryCode").val();
            if (!countryValue) return false;

            var splittedArr = countryValue.split(";"),
                minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return value.length >= minLength;
        }, function (params, element) {
            var countryValue = $("#ddlCountryCode").val();
            var splittedArr = countryValue.split(";");
            var minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return `Minimum ${minLength} digits required.`;
        });

        jQuery.validator.addMethod("maxNumberlength", function (value, element) {
            var countryValue = $("#ddlCountryCode").val();
            if (!countryValue) return false;

            var splittedArr = countryValue.split(";"),
                maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return value.length <= maxLength; //
        }, function (params, element) {
            var countryValue = $("#ddlCountryCode").val();
            var splittedArr = countryValue.split(";");
            var maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return `Maximum ${maxLength} digits required.`;
        }),

        $("#SendContactEmailFormHome").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name' || element[0].name == 'Email' || element[0].name == 'PhoneNo' || element[0].name == 'LeadTypeId'
                    || element[0].name == 'Message' ) {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                Name: { required: true },
                Email: { required: true },
                Message: { required: true },
                PhoneNo: {
                    required: true,
                    minNumberlength: true,
                    maxNumberlength: true
                },
                CountryCode: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var response = null;
                data = data.map(function (item, i) {
                    if (item.name == 'g-recaptcha-response') {
                        item.name = 'grecaptchaResponse';
                        response = item.value;
                    }
                    if (item.name == "PhoneNo") {
                        var code = $("#ddlCountryCode").val();///+91;10;10
                        if (code.split(';').length == 3) {
                            var mergedCode = code.split(';')[0] + item.value;
                            var obj = { name: item.name, value: mergedCode };
                            return obj;
                        }
                    }
                    return item;
                });
                if (response === null || response.length === 0) {
                    showMessage('Notice!', 'Please verify that you are not a robot.', 'notice');
                    return;
                }
                submitSendContactEmailForm(f, data);
            }
        });
    }

    function submitSendContactEmailForm(form, formData) {
        form.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
        form.find(":submit").prop('disabled', true);
        $.ajax({
            type: form[0].method,
            url: form[0].action,
            data: formData,
            dataType: 'json',
            success: function (data, strStatus) {
                $("#Name").val('');
                $("#Email").val('');
                $("#PhoneNo").val('');
                $("#Message").val('');
                $("#ddlCountryCode").val("");
                showMessage(data.Message, data.Data, data.Type);
                form.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                form.find(":submit").prop('disabled', false);
                //loadCaptcha();
                if (grecaptcha != undefined && grecaptcha != null) {
                    grecaptcha.reset(sendEmailContactHomeWidgetId);
                }
                $('#hdngrecaptchaResponseContactForm').val("");
            },
            error: function () {
                form.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                form.find(":submit").prop('disabled', false);
                if (grecaptcha != undefined && grecaptcha != null) {
                    grecaptcha.reset(sendEmailContactHomeWidgetId);
                }
                handleAjaxError();
            }
        });
    }

    var loginModelAfter = function () {
        if ($("#loginModal").length == 0) {
            $.ajax({
                type: 'GET',
                cache: false,
                url: '/home/LoginModel',
                dataType: 'html',
                success: function (data, strStatus) {
                    $("body").append(data);
                    if (grecaptcha != undefined && grecaptcha != null && signupFormWidgetId != null) {
                        grecaptcha.reset(signupFormWidgetId);
                    }
                    $("#SignupModal").modal('hide');
                    $(".modal-backdrop").remove();
                    OpenModel("loginModal");
                    loginUserForm();

                    $("#btnSignup_Login").click(function () {
                        signupModelAfter();
                    });
                    $("#btnForgotPassword_Login").click(function () {
                        openForgotPasswordModel();
                    });
                },
                error: handleAjaxError()
            });
        }
        else {
            if (grecaptcha != undefined && grecaptcha != null && signupFormWidgetId != null) {
                grecaptcha.reset(signupFormWidgetId);
            }
            $("#SignupModal").modal('hide');
            $(".modal-backdrop").remove();
            OpenModel("loginModal");
        }
    }

    var signupModelAfter = function () {
        if ($("#SignupModal").length == 0) {
            $.ajax({
                type: 'GET',
                cache: false,
                url: '/home/SignupModel',
                dataType: 'html',
                success: function (data, strStatus) {
                    $("body").append(data);
                    if (grecaptcha != undefined && grecaptcha != null && signupFormWidgetId != null) {
                        grecaptcha.reset(signupFormWidgetId);
                    }
                    $("#loginModal").modal('hide');
                    $(".modal-backdrop").remove();
                    OpenModel("SignupModal");
                    validateReferalCode();
                    signUpForm();
                    $("#btnLogin_SignUp").click(function () {
                        loginModelAfter();
                    });
                    $("#btnForgotPassword_Signup").click(function () {
                        openForgotPasswordModel();
                    });

                    $("#FirstName").blur(function () {
                        var name = $(this).val().trim();
                        if ($("#LastName").val() != "") {
                            name = name + "." + $("#LastName").val().trim();
                        }
                        validateDuplicateDisplayName(name);
                    });

                    $("#LastName").blur(function () {
                        var name = $("#FirstName").val().trim();
                        if ($("#LastName").val().trim() != "") {
                            name = name + "." + $(this).val().trim();
                        }
                        validateDuplicateDisplayName(name);
                    });

                    $("#Displayname").blur(function () {
                        var name = $(this).val().trim();
                        var spaceCheck = name.split(' ');
                        if (spaceCheck.length > 1) {
                            $("#Displayname").val("");
                            showMessage("Failed!", "Space not allowed", "notice");
                            return false;
                        }
                        validateDuplicateDisplayName(name);
                    });
                },
                error: handleAjaxError()
            });

        }
        else {
            $("#loginModal").modal('hide');
            $(".modal-backdrop").remove();
            if (grecaptcha != undefined && grecaptcha != null && signupFormWidgetId != null) {
                grecaptcha.reset(signupFormWidgetId);
            }
            OpenModel("SignupModal");
            validateReferalCode();
        }
    }

    var openOrderUpsertModel = function (UserId, Message, fromLogin = false) {
        $("#signUpSuccessMessageOrderModal").html(Message);
        submitOrderUpsertForm(fromLogin);
        //$.ajax({
        //    type: 'GET',
        //    cache: false,
        //    url: '/Users/OpenOrderModal',
        //    data: { UserId: UserId },
        //    dataType: 'html',
        //    success: function (res, strStatus) {
        //        $("#orderUpsertModal").remove();
        //        //$(".modal-backdrop").remove();
        //        $('body').append(res);
        //        $("#signUpSuccessMessageOrderModal").html(Message);
        //        OpenModel("orderUpsertModal");
        //        submitOrderUpsertForm(fromLogin);

        //    },
        //    error: handleAjaxError()
        //});
    }

    var submitOrderUpsertForm = function (fromLogin = false) {
        me.orderUpsertFormValidator = $("#orderUpsertForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'ShippingAddress1' || element[0].name == 'ShippingAddress2' || element[0].name == 'ShippingCity' ||
                    element[0].name == 'ShippingState' || element[0].name == 'ShippingCountry' || element[0].name == 'ShippingZip' || element[0].name == 'BillingAddress1' ||
                    element[0].name == 'BillingAddress2' || element[0].name == 'BillingCity' || element[0].name == 'BillingState' ||
                    element[0].name == 'BillingCountry' || element[0].name == 'BillingZip') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                ShippingAddress1: {required: true},
                //ShippingAddress2: {required: true},
                ShippingCity: {required: true},
                ShippingState: {required: true},
                ShippingCountry: {required: true},
                ShippingZip: { required: true },
                BillingAddress1: { required: true },
                BillingCity: { required: true },
                BillingState: { required: true },
                BillingCountry: { required: true },
                BillingZip: { required: true },
            },
            submitHandler: function (form) {

                var f = $(form);
                var data = f.serializeArray();
                for (var i = 0; i < data.length; i++) {
                    if (data[i].name === 'OrderId') {
                        data[i].value = $("#wizardOrderId").val();
                        break;
                    }
                }
                var btn = $("#btnOrderCartNext");
                var loader = btn.find("div.lds-dual-ring");
                loader.css('display', 'inline-block');
                btn.prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        if (data.Success) {
                            //$("#orderUpsertModal").modal('hide');
                            //$(".modal-backdrop").remove();
                            //var splittedVal = data.OptionalValue.split(";");
                            //var userId = splittedVal[0];
                            //var packageId = splittedVal[1];
                            //if (packageId == 3 || packageId == 4) {
                            //    openNfcCardSelectFormModel(userId);
                            //} else {
                            //    openNfcCardDetailsFormModel(userId);
                            //}
                            //$('.wizard-card').bootstrapWizard('next');
                            //var currentIndex = $('.wizard-card').bootstrapWizard('currentIndex');
                            //$('.wizard-card').bootstrapWizard('show', currentIndex + 1);
                            openNextWizardForm(me.w_next_formId);
                        } else {
                            showMessage(data.Message, data.Data, data.Type);
                        }
                        loader.css('display', 'none');
                        btn.prop('disabled', false);
                    },
                    error: function () {
                        btn.prop('disabled', false);
                        loader.css('display', 'none');
                        handleAjaxError()
                    }
                });
            }
        });
    }

    var openNfcCardSelectFormModel = function (UserId) {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/OpenSelectNfcCardModal',
            dataType: 'html',
            data: { UserId: UserId },
            success: function (data, strStatus) {
                $("#orderUpsertModal").remove();
                $("#nfcCardColorModal").remove();
                //$(".modal-backdrop").remove();
                $('body').append(data);
                OpenModel("nfcCardColorModal");
            },
            error: handleAjaxError()
        });
    }

    var selectNfcCard = function () {
        var loader = $("#btnOrderCartNext").find("div.lds-dual-ring");
        loader.css('display', 'inline-block');
        var selectedTemplate = $("#defaultSelectedNfcCardColor");
        //var elemid = $(this).attr('id');
        //var id = elemid.split('_')[1];
        //var userId = elemid.split('_')[2];
        var OrderId = $("#wizardOrderId").val();
        var obj = $(this);
        var id = obj.data('id');
        var userId = obj.data("userid");
        $.post(
            "/Users/selectNfcCardColor",
            {
                UserId: userId,
                CardColorId: id,
                OrderId: OrderId
            },
            function (res) {
                if (res.Success) {
                    selectedTemplate.val(id);
                    loader.css('display', 'none');
                    //$("#nfcCardColorModal").remove();
                    //var currentIndex = $('.wizard-card').bootstrapWizard('currentIndex');
                    //$('.wizard-card').bootstrapWizard('show', currentIndex + 1);

                    openNextWizardForm(me.w_next_formId);
                    openNfcCardDetailsFormModel(userId);
                    return;
                } else {
                    //$("#nfcCardColor_" + selectedTemplate.val() + "_" + userId).prop("checked", true);
                }
                loader.css('display', 'none');
                showMessage(res.Message, res.Data, res.Type);
            }
        )

    }

    var openNfcCardDetailsFormModel = function (UserId) {

        var OrderId = $("#wizardOrderId").val();
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/OpenNfcCardDetialsFormModal',
            dataType: 'html',
            data: { UserId: UserId, OrderId: OrderId },
            success: function (data, strStatus) {
                //$("#orderUpsertModal").remove();
                //$("#nfcCardColorModal").remove();
                //$("#nfcCardDetailFormModal").remove();
                //$(".modal-backdrop").remove();
                //$('body').append(data);
                //OpenModel("nfcCardDetailFormModal");
                $("#nfcCardDetailFormCloseContainer").empty();
                $("#nfcCardDetailFormCloseContainer").html(data);
                updateNfcCardDetails();
            },
            error: handleAjaxError()
        });
    }

    var updateNfcCardDetails = function () {

        $("#nfcCardDetailForm").on("keyup", "#NfcCardLine1", function () {
            $("#textNfcCardLine1").html($(this).val());
        });
        
        $("#nfcCardDetailForm").on("keyup", "#NfcCardLine2", function () {
            $("#textNfcCardLine2").html($(this).val());
        });
        
        $("#nfcCardDetailForm").on("keyup", "#NfcCardLine3", function () {
            $("#textNfcCardLine3").html($(this).val());
        });

        me.nfcCardDetailFormValidator = $("#nfcCardDetailForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'UserId' || element[0].name == 'CardColorId' || element[0].name == 'NfcCardLine1') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                UserId: { required: true },
                CardColorId: { required: true },
                NfcCardLine1: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var orderId = { "name": "OrderId", value: $("#wizardOrderId").val() };
                data.push(orderId);
                
                var btn = $("#btnOrderCartNext");
                btn.find("div.lds-dual-ring").css('display', 'inline-block');
                btn.prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        var userId = getFormInput("nfcCardDetailForm", "UserId").val();
                        
                        if (data.Success) {
                            //$("#nfcCardDetailFormModal").modal('hide');
                            //$(".modal-backdrop").remove();
                            //openOrderPaymentFormModel(userId);
                            //var currentIndex = $('.wizard-card').bootstrapWizard('currentIndex');
                            //$('.wizard-card').bootstrapWizard('show', currentIndex + 1);

                            openNextWizardForm(me.w_next_formId);
                        } else {
                            showMessage(data.Message, data.Data, data.Type);
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
            }
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
                //$(".modal-backdrop").remove();
                $("#nfcCardOrderPaymentFormContainer").empty();
                $('#nfcCardOrderPaymentFormContainer').append(data);
                //OpenModel("orderPaymentFormModal");
                //createRazorPayOrder();
                submitOrderPaymentForm(fromLogin);
            },
            error: handleAjaxError()
        });
    }

    var submitOrderPaymentForm = function (fromLogin = false) {
        me.orderPaymentFormValidator = $("#orderPaymentForm").validate({
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
                createRazorPayOrder((orderId, orderDetails) => {
                    initializeRazorpay(orderId, orderDetails);
                    me.razorPayInsatance.open();
                });
                //if ($("#IsRazorGateway").val() == "0") {
                //    $.ajax({
                //        type: f[0].method,
                //        url: f[0].action,
                //        data: formData,
                //        dataType: 'json',
                //        contentType: false,
                //        processData: false,
                //        success: function (data, strStatus) {
                //            showMessage(data.Message, data.Data, data.Type);
                //            if (data.Success) {
                //                //$("#orderPaymentFormModal").modal('hide');
                //                $("#nfcCardCartWizardModal").modal('hide');
                //                $(".modal-backdrop").remove();
                //                if (fromLogin) {
                //                    window.location.href = "/edit/" + data.OptionalValue;
                //                }
                //            }
                //            btn.find("div.lds-dual-ring").css('display', 'none');
                //            btn.prop('disabled', false);
                //        },
                //        error: function () {
                //            btn.prop('disabled', false);
                //            btn.find("div.lds-dual-ring").css('display', 'none');
                //            handleAjaxError()
                //        }
                //    });
                //} else {
                //    createRazorPayOrder((orderId, orderDetails) => {
                //        initializeRazorpay(orderId, orderDetails);
                //        me.razorPayInsatance.open();
                //    });
                   
                //}
            }
        });
    }

    var openForgotPasswordModel = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/home/ForgotPassword',
            dataType: 'html',
            success: function (data, strStatus) {

                if ($("#ForgotPasswordModal").length == 0) {
                    $("body").append(data);                   
                    submitForgotPassword();
                }
                $("#SignupModal").modal('hide');
                $("#loginModal").modal('hide');
                $(".modal-backdrop").remove();
                getFormInput("ForgotPasswordForm", "emailid").val("");
                OpenModel("ForgotPasswordModal");
                
            },
            error: handleAjaxError()
        });
    }
    
    var submitForgotPassword = function () {

        $("#ForgotPasswordForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'FirstName' || element[0].name == 'EmailId') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                emailid: { required: true, emailvalidatecustom: true  }
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
                        $("#ForgotPasswordModal").modal('hide');
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    },
                    error: function () {
                        f.find(":submit").prop('disabled', false);
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        handleAjaxError()
                    }
                });
            }
        });
    }

    var validateDuplicateDisplayName = function (name) {
       
            $.ajax({
                type: 'GET',
                data: { name: name },
                url: "/Users/CheckDisplayNameAvailability",
                dataType: 'json',
                success: function (data) {
                    if (data.Data == "1") {
                        $("#Displayname").val("");
                        showMessage("Failed!", data.Message,"notice");
                        //$("#isExistEmailError").text(data.Message);
                        isDuplicateDisplayName = true;
                    }
                    else {
                        $("#Displayname").val(name);
                        //$("#isExistEmailError").text("");
                        isDuplicateDisplayName = false;
                    }
                },
                error: handleAjaxError()
            });
       
    }

    var openSignUpModel = function (obj, id) {
        $(obj).find("div.lds-dual-ring").css('display', 'inline-block');
        $(obj).prop('disabled', true);
        var url = '/home/SignupModel';
        if (id != null && id != undefined) {
            url = url + "?PackageId=" + id;
        }

        $.ajax({
            type: 'GET',
            cache: false,
            url: url,
            dataType: 'html',
            success: function (data, strStatus) {

                if ($("#SignupModal").length == 0) {
                    $("body").append(data);
                    //initiate funciton
                    signUpForm();
                    authenticationEventListeners();
                }
                //loadSignupCaptcha();
                $("#loginModal").modal('hide');
                OpenModel("SignupModal");
                validateReferalCode();
                $(obj).find("div.lds-dual-ring").css('display', 'none');
                $(obj).prop('disabled', false);
            },
            error: function () {
                $(obj).find("div.lds-dual-ring").css('display', 'none');
                $(obj).prop('disabled', false);
                handleAjaxError()
            }
        });
    }

    var openLoginModel = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/home/LoginModel',
            dataType: 'html',
            success: function (data, strStatus) {

                if ($("#loginModal").length == 0) {
                    $("body").append(data);

                    //initiate funciton
                    loginUserForm();

                    //authenticationEventListeners();

                    $("#btnForgotPassword_Login").click(function () {
                        openForgotPasswordModel();
                    });
                    $("#btnSignup_Login").click(function () {
                        signupModelAfter();
                    });
                }
                $("#SignupModal").modal('hide');
                OpenModel("loginModal");
                $("#btnLogin").find("div.lds-dual-ring").css('display', 'none');
                $("#btnLogin").prop('disabled', false);
            },
            error: function () {
                $("#btnLogin").prop('disabled', false);
                $("#btnLogin").find("div.lds-dual-ring").css('display', 'none');
                handleAjaxError();

            }
        });
    }

    function authenticationEventListeners() {
        $(document).on('blur', '#ReferredByCode', validateReferalCode)

        $("#btnLogin_SignUp").click(function () {
            loginModelAfter();
        });

        $("#btnSignup_Login").click(function () {
            signupModelAfter();
        });

        $("#btnForgotPassword_Signup").click(function () {
            openForgotPasswordModel();
        });


        $("#FirstName").blur(function () {
            var name = $(this).val().trim();
            if ($("#LastName").val() != "") {
                name = name + "." + $("#LastName").val().trim();
            }
            validateDuplicateDisplayName(name);
        });

        $("#LastName").blur(function () {
            var name = $("#FirstName").val().trim();
            if ($("#LastName").val().trim() != "") {
                name = name + "." + $(this).val().trim();
            }
            validateDuplicateDisplayName(name);
        });

        $("#Displayname").blur(function () {
            var name = $(this).val().trim();
            var spaceCheck = name.split(' ');
            if (spaceCheck.length > 1) {
                $("#Displayname").val("");
                showMessage("Failed!", "Space not allowed", "notice");
                return false;
            }
            validateDuplicateDisplayName(name);
        });

    };

    function removeLastSlashFromUrl(url) {
        if (url.endsWith('/')) {
            return url.slice(0, url.length - 1);
        }
        return url;
    }

    function initializedGoogleClient() {
        googleClient = google.accounts.oauth2.initCodeClient({
            client_id: me.googleClientId,
            scope: 'openid profile email',
            ux_mode: "popup",
            redirect_uri: removeLastSlashFromUrl(me.webUrl),
            response_type: 'code',
            state: `redirectUri=${removeLastSlashFromUrl(me.webUrl)}`,
            callback: (response) => {
                if (response.code) {
                    $.get("/OAuth/SignInWithGoogleCodeAsync", { code: response.code, state: response.state }, function (data) {
                        showMessage(data.Message, data.Data, data.Type, 2000);
                        if (!data.Success) {
                            return;
                        }

                        window.location.href = "/edit/" + data.OptionalValue;
                        
                    }).fail(handleAjaxError());
                }
            },
        });

    }

    function SignInWithMicrosoft() {
        var redirectUri = me.webUrl + "OAuth/SignInWithMicrosoftCodeAsync";
        var scopes = "User.Read openid profile email";
        var signInWithMicrosoftUrl = `https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=${me.microsoftClientId}&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=code&scope=${encodeURIComponent(scopes)}&response_mode=query&state=${encodeURIComponent(redirectUri)}`

        var popupWidth = 600;
        var popupHeight = 500;
        var left = (window.innerWidth - popupWidth) / 2;
        var top = (window.innerHeight - popupHeight) / 2;

        window.open(signInWithMicrosoftUrl, "microsoftConsentPopup", `width=${popupWidth},height=${popupHeight},left=${left},top=${top}`);

        // Add an event listener to receive messages from the popup
        window.addEventListener('message', function (event) {
            if (event.data && event.data.type === 'microsoftAuthResponse') {
                var response = event.data.payload;
                showMessage(response.success ? "Congratulation!" : "Failed!", response.message, response.success ? "success" : "error");
                if (response.success) {
                    setTimeout(() => window.location.href = response.redirectUrl, 500);
                }
            }
        }, false);
    }

    function getFormInput(formId, inputName) {
        return $('#' + formId).find(`input[name=${inputName}]`)
    }

    var createUserNewsLetterForm = function () {

        $("#newsLetterForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Email') {
                    var ele = $("#newLetterFormError");
                    ele.empty();
                    error.appendTo(ele);
                    //error.appendTo(element.parent().parent().parent());
                }
            },
            unhighlight: function (element, errorClass, validClass) { // execute when error removed
                var ele = $("#newLetterFormError");
                ele.empty();
            },
            rules: {
                Email: { required: true, emailvalidatecustom: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        if (data.Success) {
                            showMessage(data.Message, data.Data, data.Type);
                        }
                        getFormInput("newsLetterForm", "Email").val("");
                    },
                    error: function (xhr, err) {
                        //showMessage("Failed", "Some error occrred.", "error");
                        getFormInput("newsLetterForm", "Email").val("");
                    }
                });
            }
        });
    }

    var getFormInputByName = function (formId) {
        return (name) => getFormInput(formId, name);
    }

    var validateReferalCode = function () {
        var obj = getFormInput("CreateUserForm", "ReferredByCode");
        if (obj.val() != null && obj.val().trim() != "") {
            $.get("/Users/CheckValidReferalCode", { ReferredByCode: obj.val() }, function (res) {
                if (res.Success) {
                    return;
                }
                showMessage(res.Message, res.Data, res.Type);
                obj.val("");
            }).fail(function () {
                showMessage("Failed", "Some error occurred when validating referral code, please try again later.", "error");
            });
        }
    }

    var getWizardFormValidator = function (id, UserId, Message = "", fromLogin = false) {
        //switch (id) {
        //    case 1:
        //        return { validator: me.orderUpsertFormValidator, form: $("#orderUpsertForm"), formInitializer: () => openOrderUpsertModel(UserId, Message, fromLogin) };
        //    case 3:
        //        return { validator: me.nfcCardDetailFormValidator, form: $("#nfcCardDetailForm"), formInitializer: updateNfcCardDetails };
        //    case 4:
        //        return { validator: me.orderPaymentFormValidator, form: $("#orderPaymentForm"), formInitializer: () => openOrderPaymentFormModel(UserId, fromLogin) };
        //    default:
        //        return null;
        //}
        switch (id) {
            case 2:
                return { validator: me.nfcCardDetailFormValidator, form: $("#nfcCardDetailForm"), formInitializer: () => openNfcCardDetailsFormModel(UserId) };
            case 3:
                return { validator: me.orderUpsertFormValidator, form: $("#orderUpsertForm"), formInitializer: () => openOrderUpsertModel(UserId, Message, fromLogin) };
            case 4:
                return { validator: me.orderPaymentFormValidator, form: $("#orderPaymentForm"), formInitializer: () => openOrderPaymentFormModel(UserId, fromLogin) };
            default:
                return null;
        }
    }

    var initializeNfcCartWizard = function () {
        $('.wizard-card').bootstrapWizard({
            //'tabClass': 'nav nav-pills',
            'tabClass': 'nav nav-pills nowrap',
            'nextSelector': '.btn-next',
            'previousSelector': '.btn-previous',

            onNext: function (tab, navigation, index) {
                var ele = $(".tab-pane.active");
                var formId = ele.data("id");
                var userId = $("#wizardUserId").val();

                if (formId == 2) {
                    return true;
                }

                var obj = getWizardFormValidator(formId, userId, false, false);
                if (obj == null) {
                    return true
                }

                if (!(obj.form.valid())) {
                    obj.validator.focusInvalid();
                    return false;
                }
                obj.form.submit();

                return false;
            },

            onInit: function (tab, navigation, index) {

                //check number of tabs and fill the entire row
                var $total = navigation.find('li').length;
                $width = 100 / $total;

                navigation.find('li').css('width', $width + '%');
                $("#btnOrderCartFinish").on('click', () => $("#orderPaymentForm").submit());
                //createRazorPayOrder();
            },

            onTabClick: function (tab, navigation, index) {
                return false;
            },

            onTabShow: function (tab, navigation, index) {
                
                var $total = navigation.find('li').length;
                var $current = index + 1;

                var $wizard = navigation.closest('.wizard-card');

                // If it's the last tab then hide the last button and show the finish instead
                if ($current >= $total) {
                    $($wizard).find('.btn-next').hide();
                    $($wizard).find('.btn-finish').show();
                } else {
                    $($wizard).find('.btn-next').show();
                    $($wizard).find('.btn-finish').hide();
                }

                //update progress
                var move_distance = 100 / $total;
                move_distance = move_distance * (index) + move_distance / 2;

                $wizard.find($('.progress-bar')).css({ width: move_distance + '%' });
                //e.relatedTarget // previous tab

                $wizard.find($('.wizard-card .nav-pills li.active a .icon-circle')).addClass('checked');

                var formId = tab.find('a').data('id');
                var userId = $("#wizardUserId").val();

                if (formId != 2) {
                    var obj = getWizardFormValidator(formId, userId, false, false);
                    if (obj != null) {
                        obj.formInitializer();
                    }
                }
            }
        });

        $('[data-toggle="wizard-radio"]').click(function () {
            wizard = $(this).closest('.wizard-card');
            wizard.find('[data-toggle="wizard-radio"]').removeClass('active');
            $(this).addClass('active');
            $(wizard).find('[type="radio"]').removeAttr('checked');
            $(this).find('[type="radio"]').attr('checked', 'true');
        });

        $('[data-toggle="wizard-checkbox"]').click(function () {
            if ($(this).hasClass('active')) {
                $(this).removeClass('active');
                $(this).find('[type="checkbox"]').removeAttr('checked');
            } else {
                $(this).addClass('active');
                $(this).find('[type="checkbox"]').attr('checked', 'true');
            }
        });

        $('.set-full-height').css('height', 'auto');
    }

    var defaultDiscountPrice = function () {
        debugger
        var discountCoupon = $("#DiscountCoupon").val();
        if (discountCoupon != null && discountCoupon.trim() != "") {
            $("#DiscountCoupon").val("");
            showMessage("Failed!", "Invalid Or expire Coupon", "error");
        }
        var mainPackageCostEle = $("#mainPackageCost");
        var isDiscountHidden = $("#isDiscountHidden").val();
        var f = getFormInputByName("orderPaymentForm");
        var currentDiscountPriceEle = f("DiscountedPackageCost");
        var currentTaxEle = f("Tax");
        var initialDiscountPackageCost = f("hdnInitialDiscountedPackageCode").val() || 0;
        var initialTax = f("hdnInitialTax").val() || 0;
        var shippingCharges = f("ShippingAmount").val() || 0;
        var wantMetalCard = $("#chkWantMetalCard").is(":checked");
        var displayDiscountEle = $("#discountedPackageCostDisplay");
        var displayTaxEle = $("#taxDisplay");
        var totalPrice = $("#totalOrderPrice");
        var discountPriceContainer = $("#discountPriceContainer");
        var paymentAmount = f("PaymentAmount");
        var initialShippingFee = parseFloat($("#hdnShippingAmount").val()) || 0;
        var isHandPick = $("#chkIsByHandPickup").is(":checked");
        currentDiscountPriceEle.val(initialDiscountPackageCost);
        currentTaxEle.val(initialTax);
        displayDiscountEle.html("Rs. " + parseFloat(initialDiscountPackageCost));
        displayTaxEle.html("Rs. " + parseFloat(initialTax));
        var amount = paymentAmount.val();
        if (isHandPick) {
            $("#ShippingAmount").val(0);
            shippingCharges = 0;
            $("#displayShippingFee").html(0);
        } else {
            $("#ShippingAmount").val(initialShippingFee);
            shippingCharges = initialShippingFee;
            $("#displayShippingFee").html(initialShippingFee);
        }
        if (wantMetalCard) {
            amount = (parseFloat(initialDiscountPackageCost) + parseFloat(initialTax) + parseFloat(shippingCharges) + 2499).toFixed(2);
            totalPrice.html(amount);
        } else {
            amount = (parseFloat(initialDiscountPackageCost) + parseFloat(initialTax) + parseFloat(shippingCharges)).toFixed(2)
            totalPrice.html(amount);
        }
        paymentAmount.val(amount);
        if (isDiscountHidden == "True") {
            mainPackageCostEle.removeClass("line-through");
            discountPriceContainer.removeClass("d-flex");
            discountPriceContainer.hide();
        } else {
            mainPackageCostEle.addClass("line-through");
            discountPriceContainer.addClass("d-flex");
            discountPriceContainer.show();
        }
        //$("#IsRazorGateway").val("0");
        //createRazorPayOrder();
    }

    var checkValidDiscountCoupon = function () {
        var mainPackageCostEle = $("#mainPackageCost");
        var isDiscountHidden = $("#isDiscountHidden").val();
        var f = getFormInputByName("orderPaymentForm");
        var initialDiscountPackageCost =   f("hdnInitialDiscountedPackageCode").val();
        var initialTax = f("hdnInitialTax").val();
        var shippingCharges = f("ShippingAmount").val();
        var wantMetalCard = $("#chkWantMetalCard").is(":checked");
        var displayDiscountEle = $("#discountedPackageCostDisplay");
        var displayTaxEle = $("#taxDisplay");
        var totalPrice = $("#totalOrderPrice");
        var currentDiscountPriceEle = f("DiscountedPackageCost");
        var currentTaxEle = f("Tax");
        var discountPriceContainer = $("#discountPriceContainer");
        var paymentAmount = f("PaymentAmount");
        var initialShippingFee = parseFloat($("#hdnShippingAmount").val()) || 0;
        var isHandPick = $("#chkIsByHandPickup").is(":checked");
        var code = $(this).val();
        $.post("/Users/CheckDiscountCouponValid", { Code: code }, function (res) {
            debugger
            $("#btnOrderCartFinish").prop("disabled", false);
            if (res.Success) {
                debugger
                $("#CouponDiscount").val(0);
                var percentOff = res.Data.PercentageOff;
                var amountOff = res.Data.AmountOff;
                var newPackageCost = parseFloat(initialDiscountPackageCost);
                if (percentOff != null && percentOff != 0) {
                    $("#CouponDiscount").val((parseFloat(initialDiscountPackageCost) * parseFloat(parseFloat(percentOff) / 100)));
                    newPackageCost = (parseFloat(initialDiscountPackageCost) - (parseFloat(initialDiscountPackageCost) * parseFloat(parseFloat(percentOff) / 100))).toFixed(2);
                }
                if (amountOff != null && amountOff != 0) {
                    $("#CouponDiscount").val(parseFloat(amountOff));
                    newPackageCost = (parseFloat(initialDiscountPackageCost) - parseFloat(amountOff)).toFixed(2);
                }
                var newTax = parseFloat(parseFloat(newPackageCost) * 0.18).toFixed(2);
                displayTaxEle.html("Rs. " + newTax);
                displayDiscountEle.html("Rs. " + newPackageCost);
                currentDiscountPriceEle.val(newPackageCost);
                currentTaxEle.val(newTax);
                var amount = paymentAmount.val();
                if (isHandPick) {
                    $("#ShippingAmount").val(0);
                    shippingCharges = 0;
                    $("#displayShippingFee").html(0);
                } else {
                    $("#ShippingAmount").val(initialShippingFee);
                    shippingCharges = initialShippingFee;
                    $("#displayShippingFee").html(initialShippingFee);
                }
                if (wantMetalCard) {
                    amount = (parseFloat(newPackageCost) + parseFloat(newTax) + parseFloat(shippingCharges) + 2499).toFixed(2)
                } else {
                    amount = (parseFloat(newPackageCost) + parseFloat(newTax) + parseFloat(shippingCharges)).toFixed(2);
                }
                totalPrice.html(amount);
                paymentAmount.val(amount);
                if (res.Data.DiscountCouponId == 1) {
                    $("#IsRazorGateway").val("1");
                    //createRazorPayOrder();
                }

                //createRazorPayOrder();
                mainPackageCostEle.addClass("line-through");
                discountPriceContainer.addClass("d-flex");
                discountPriceContainer.show();
                return;
            }

            defaultDiscountPrice();
        }).fail(handleAjaxError(function () {
            defaultDiscountPrice();
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
            "image": "/Content/images/favicon_bharatTouch.ico", // Optional path to your logo
            "handler": function (response) {
                console.log('handler response', response);
                const PersonalInfoUserForm = document.getElementById("orderPaymentForm");
                var formData = new FormData(PersonalInfoUserForm);
                formData.append("razorpay_payment_id", response.razorpay_payment_id);
                formData.append("razorpay_order_id", response.razorpay_order_id);
                formData.append("razorpay_signature", response.razorpay_signature);
                var btn = $("#btnOrderCartFinish");
                $.ajax({
                    url: '/Users/VerifyAndSaveNfcOrderPayment', // Your payment verification URL
                    type: 'POST',
                    dataType: 'json',
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        showMessage(data.Message, data.Data, data.Type);
                        if (data.Success) {
                            //$("#orderPaymentFormModal").modal('hide');
                            $("#nfcCardCartWizardModal").modal('hide');
                            $(".modal-backdrop").remove();
                        } else {
                            //createRazorPayOrder()
                            me.razorPayInsatance = null;
                        }
                        btn.find("div.lds-dual-ring").css('display', 'none');
                        btn.prop('disabled', false);
                    },

                    error: function (xhr, status, error) {
                        btn.prop('disabled', false);
                        btn.find("div.lds-dual-ring").css('display', 'none');
                        handleAjaxError()

                        //createRazorPayOrder()
                        me.razorPayInsatance = null;
                    }

                });
            },
            "modal": {
                "ondismiss": function (cancelRes) {
                    console.log("paymentCancelRes", cancelRes);
                    console.log("Payment popup closed by user");
                    showMessage("Cancelled", "You closed the payment popup before completing payment.", "error");

                    var btn = $("#btnOrderCartFinish");
                    btn.find("div.lds-dual-ring").css('display', 'none');
                    btn.prop('disabled', false);
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

            var btn = $("#btnOrderCartFinish");
            btn.find("div.lds-dual-ring").css('display', 'none');
            btn.prop('disabled', false);
        });
    }

    var createRazorPayOrder = function (callback) {
        var paymentAmount = getFormInput("orderPaymentForm", "PaymentAmount").val();
        var orderId = getFormInput("orderPaymentForm", "OrderId").val();
        var UserId = getFormInput("orderPaymentForm", "UserId").val();
        $.post(
            "/Users/CreateRazorpayOrder",
            { PaymentAmount: paymentAmount, OrderId: orderId, UserId: UserId },
            function (res) {
                if (!res.Success) {
                    showMessage(res.Message, res.Data, res.Type);
                    return;
                }

                me.RazorPayLatestOrderId = res.Data.RazorOorderId;
                console.log("create razor pay order result", res);
                if (callback) {
                    callback(res.Data.RazorOorderId, res.Data.RazorOrder);
                }
            }
        ).fail(handleAjaxError());
    }


    function setProgressBar(curStep) {
        var steps = $("fieldset").length;
        var percent = parseFloat(100 / steps) * curStep;
        percent = percent.toFixed();
        $(".progress-bar").
            css("width", percent + "%");

    }
    var openNextWizardForm = function (next_formId) {
        var nextEle = $(`fieldset[data-id="${next_formId}"]`);
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

        setProgressBar(++w_current);

        var userId = $("#wizardUserId").val();
        var obj = getWizardFormValidator(next_formId, userId, false, false);
        if (obj != null) {
            obj.formInitializer();
        }
        me.w_current_formId = next_formId
        me.w_next_formId = nextEle.next().data("id");
    }

    var initializeNfcCartWizard_v1 = function () {
        var zeroIndexedFormId = $('#msform fieldset').eq(0).data('id');
        me.current_formId = zeroIndexedFormId;
        var currEle = $(`fieldset[data-id="${zeroIndexedFormId}"]`);
        me.w_next_formId = currEle.next().data("id");
        

        if (zeroIndexedFormId != 1) {
            var obj = getWizardFormValidator(me.current_formId, $("#wizardUserId").val(), false, false);
            if (obj != null) {
                obj.formInitializer();
            }
        }
        setProgressBar(w_current);

        $(document).on("click", ".wizard-next", function () {

            w_current_fs = $(this).parent();
            w_next_fs = $(this).parent().next();
            me.w_current_formId = w_current_fs.data("id");
            me.w_next_formId = w_next_fs.data("id");
            var userId = $("#wizardUserId").val();

            var obj = getWizardFormValidator(me.w_current_formId, userId, false, false);
            if (obj == null) {
                openNextWizardForm(me.w_next_formId);
                return;
            }
            if (!(obj.form.valid())) {
                obj.validator.focusInvalid();
                return;
            }
            obj.form.submit();

        });


        $(document).on("click", ".wizard-prev", function () {

            w_current_fs = $(this).parent();
            w_previous_fs = $(this).parent().prev();

            //Remove class active
            $("#progressbar li").eq($("fieldset").index(w_current_fs)).removeClass("active");

            //show the previous fieldset
            w_previous_fs.show();

            //hide the current fieldset with style
            w_current_fs.animate({ opacity: 0 }, {
                step: function (now) {
                    // for making fielset appear animation
                    w_opacity = 1 - now;

                    w_current_fs.css({
                        'display': 'none',
                        'position': 'relative'
                    });

                    w_previous_fs.css({ 'opacity': w_opacity });
                },
                duration: 500
            });

            setProgressBar(--w_current);

            me.w_current_formId = w_previous_fs.data("id");
            me.w_next_formId = w_current_fs.data("id");
        });

    }

    var onChangeChkCalculateAmount = function () {
        var initialShippingFee = parseFloat($("#hdnShippingAmount").val()) || 0;
        var isHandPick = $("#chkIsByHandPickup").is(":checked");
        var totalEle = $("#totalOrderPrice");
        var paymentAmount = getFormInput("orderPaymentForm", "PaymentAmount");
        var discountedPackageCost = parseFloat($("#DiscountedPackageCost").val()) || 0;
        var tax = parseFloat($("#Tax").val()) || 0;
        var shippingCharges = parseFloat($("#ShippingAmount").val()) || 0;
        var wantMetalCard = $('#chkWantMetalCard').is(':checked');
        getFormInput("orderPaymentForm", "WantMetalCard").val(wantMetalCard);
        var amount = paymentAmount.val();
        if (isHandPick) {
            $("#ShippingAmount").val(0);
            shippingCharges = 0;
            $("#displayShippingFee").html(0);
        } else {
            $("#ShippingAmount").val(initialShippingFee);
            shippingCharges = initialShippingFee;
            $("#displayShippingFee").html(initialShippingFee);
        }
        if (wantMetalCard) {
            amount = (discountedPackageCost + tax + shippingCharges + 2499).toFixed(2);
        } else {
            amount = (discountedPackageCost + tax + shippingCharges).toFixed(2);
        }
        totalEle.html(amount);
        paymentAmount.val(amount);
    }

    this.init = function () {
        $(document).on("blur", "#DiscountCoupon", checkValidDiscountCoupon);
        $(document).on("input", "#DiscountCoupon", function () {
            $("#btnOrderCartFinish").prop("disabled", true);
        });
        $(window).on('load', function () {
            sendEmailContactForm();
            initializedGoogleClient();
            createUserNewsLetterForm();
            var code = $("#hdnReferalCode").val();
            if (code != "") {
                $("#btnHomeSignupPopup").click();
                $(document).on("shown.bs.modal", "#SignupModal", function () {
                    document.getElementById("CreateUserForm").reset();
                    getFormInput("CreateUserForm", "ReferredByCode").val(code);
                    validateReferalCode();
                });
            }
        });

        //$(document).on("shown.bs.modal", "#nfcCardCartWizardModal", initializeNfcCartWizard);
        $(document).on("shown.bs.modal", "#nfcCardCartWizardModal", initializeNfcCartWizard_v1);

        $(document).on("click", "#termsOfServices_link", function () {
            var modal = $("#termsOfServicesModal");
            $("#SignupModal").hide()
            modal.modal('show');
        });

        $(document).on('hidden.bs.modal', "#termsOfServicesModal", function () {
            $("#SignupModal").show();
            $("body").addClass("modal-open");
        });

        $(document).on("click", "#privacyPolicy_link", function () {
            var modal = $("#privacyPolicyModal");
            $("#SignupModal").hide()
            modal.modal('show');
        });

        $(document).on('hidden.bs.modal', "#privacyPolicyModal", function () {
            $("#SignupModal").show();
            $("body").addClass("modal-open");
        });

        $(document).on("click", ".btnSigninWithGoogle", function () {
            googleClient.requestCode();
        });


        $.validator.addMethod("customRegex", function (value, element, param) {
            return this.optional(element) || param.test(value);
        }, "Please enter a valid value.");

        //loginUserForm();


        $("#btnHomeSignupPopup").click(function () {
            $("#SignupModal").remove();
            if (grecaptcha != undefined && grecaptcha != null && signupFormWidgetId != null) {
                grecaptcha.reset(signupFormWidgetId);
            }
            openSignUpModel(this);
        });

        $("#btnLogin").click(function () {
            $(this).find("div.lds-dual-ring").css('display', 'inline-block');
            $(this).prop('disabled', true);
            openLoginModel();
        });

        $(document).on('click', '.btnPricingBookNow', function () {
            var id = $(this).data('id');
            $("#SignupModal").remove();
            if (grecaptcha != undefined && grecaptcha != null && signupFormWidgetId != null) {
                grecaptcha.reset(signupFormWidgetId);
            }
            openSignUpModel(this, id);
        });

        $(document).on("click", ".btnSigninWithMicrosoft", SignInWithMicrosoft);

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

        //signUpForm();

        //$(document).on('change', 'input[name=rdNfcCardSelect]', selectNfcCard);
        $(document).on('click', '.selectNfcCard', selectNfcCard);

        $(document).on('change', '#chkWantMetalCard', onChangeChkCalculateAmount);

        $(document).on('change', '#chkIsByHandPickup', onChangeChkCalculateAmount);

        $(document).on('click', '#btnNfcCardColorSelectNext', function () {
            var loader = $("#btnOrderCartNext").find("div.lds-dual-ring");
            loader.css('display', 'inline-block');
            var selectedTemplate = $("#defaultSelectedNfcCardColor");
            var id = $(this).data("cardcolorid");
            var userId = $(this).data("userid");
            $.post(
                "/Users/selectNfcCardColor",
                {
                    UserId: userId,
                    CardColorId: id
                },
                function (res) {
                    if (res.Success) {
                        selectedTemplate.val(id);
                        loader.css('display', 'none');
                        $("#nfcCardColorModal").remove();
                        //openNfcCardDetailsFormModel(userId);
                        var currentIndex = $('.wizard-card').bootstrapWizard('currentIndex');
                        $('.wizard-card').bootstrapWizard('show', currentIndex + 1);
                        return;
                    } else {
                        $("#nfcCardColor_" + selectedTemplate.val() + "_" + userId).prop("checked", true);
                    }
                    loader.css('display', 'none');
                    showMessage(res.Message, res.Data, res.Type);
                }
            )
        })


    }
}