var smartSignupWizard = function () {
    var me = this;
    this.captchaSiteKey = null;
    var isValidReferralCode = false;
    var isDuplicateEmail = false;
    var isDuplicateDisplayName = false;
    var signupFormWidgetId = null;
    this.razorPayApiKey = null;
    this.RazorPayLatestOrderId = null;
    this.razorPayInsatance = null;
    this.isTaxApplicable = false;

    this.isValidCoupon = false;
    var wizardData = {
        payment: {
            InitialData: {

            },
            updatedData: {

            }
        }
    };

    function getFormInput(formId, inputName) {
        return $('#' + formId).find(`input[name=${inputName}]`)
    }

    var getFormInputByName = function (formId) {
        return (name) => getFormInput(formId, name);
    }

    // wizard steps

    //function showStep(step) {
    //    document.querySelectorAll('.step').forEach((div, index) => {
    //        div.classList.remove('active');
    //        if (index === step - 1) div.classList.add('active');
    //    });
    //}

    function nextStep() {
        if (currentStep === 1) {
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

        if (currentStep === 2) {
            wizardData.CardOptions = {
                NfcCardLine1: document.getElementById('NfcCardLine1').value,
                NfcCardLine2: document.getElementById('NfcCardLine2').value,
                NfcCardLine3: document.getElementById('NfcCardLine3').value
            };
        }

        if (currentStep < 4) {
            currentStep++;
            showStep(currentStep);
        }


        console.log("Next stepWizard Data:", wizardData);
    }

    function prevStep() {
        if (currentStep == 4) {
            if (confirm("Are you sure to go to previous step?")) {
                if (currentStep > 0) {
                    currentStep--;
                    showStep(currentStep);
                }
            }
        } else {
            if (currentStep > 0) {
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

    // Start signup form validation
    var validateDuplicateDisplayName = function (name) {

        $.ajax({
            type: 'GET',
            data: { name: name },
            url: "/Users/CheckDisplayNameAvailability",
            dataType: 'json',
            success: function (data) {
                if (data.Data == "1") {
                    $("#Displayname").val("");
                    showMessage("Failed!", data.Message, "notice");
                }
                else {
                    $("#Displayname").val(name);
                }
            },
            error: handleAjaxError()
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

        $("#ddlCountryId").on('change', function () {
            var code = $(this).val();// countryId;10;10
            if (code.split(';').length == 3) {
                var id = code.split(';')[0];
                $("#CountryId").val(id);
            }
        });

        $("#CreateUserForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'FirstName' || element[0].name == 'EmailId' || element[0].name == 'Password' || element[0].name == 'Phone' || element[0].name == 'CountryId') {
                    error.appendTo(element.parent());
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
                chkPrivacyPolicy: { required: true },
                CountryId: { required: true },
                Phone: {
                    required: true,
                    minNumberlength: { countrySelector: "#ddlCountryId" },
                    maxNumberlength: { countrySelector: "#ddlCountryId" }
                },
            },
            messages: {
                FirstName: {
                    required: "First Name is required."
                },
                EmailId: {
                    required: "Email is required.",
                    emailvalidatecustom: "Please Enter a valid email."
                },
                Displayname: {
                    required: "Display Name is required."
                },
                Password: {
                    required: "Password is required.",
                    customRegex: "password should between 8 - 16 characters which contain minimum (1 lowercase letter, 1 uppercase letter and 1 number)."
                },
                ConfirmPassword: {
                    required: "Confirm password is required.",
                    equalTo: "Password and Confirm password must be same"
                },
                chkPrivacyPolicy: {
                    required: "Please accept privacy and policy."
                }
            },
            submitHandler: function (form) {
                debugger
                var f = $(form);
                var data = f.serializeArray();
                var response = null;
                data = data.map(function (item, i) {
                    if (item.name == 'g-recaptcha-response') {
                        item.name = 'grecaptchaResponse';
                        response = item.value;
                    }
                    //else if (item.name == "CountryId") {
                    //    var code = $("#ddlCountryId").val();// countryId;10;10
                    //    if (code.split(';').length == 3) {
                    //        var id = code.split(';')[0];
                    //        var obj = { name: "CountryId", value: id };
                    //        return obj;
                    //    }
                    //}
                    return item;
                });
                if (response === null || response.length === 0) {
                    showMessage('Notice!', 'Please verify that you are not a robot.', 'notice');
                    return;
                }

                if (!$("#chkPrivacyPolicy").is(":checked")) {
                    showMessage('Notice!', "Please agree to the Privacy Policy.", 'notice');
                    return false;
                }

                if (!isValidReferralCode && $("#ReferredByCode").val().trim() != "") {
                    showMessage('Notice!', "Invalid Referal Code", 'notice');
                    return false;
                }

                var formData;
                formData = new FormData($('form').get(0));
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });
                submitSignupForm(f, formData, isDuplicateEmail, getFormInput("CreateUserForm", "PackageId").val());

            }
        });
    }

    var submitSignupForm = function (f, formData, isDuplicateEmail, packageId) {
        debugger
        if (isDuplicateEmail && $("#UserId").val() == "0")
            return false;


        var Password = $("#txtRegPassword").val();
        var confirmPassword = $("#txtRegConfirmPassword").val();


        if (Password != confirmPassword) {
            showMessage("Warning!", "Password and confirm password not matched.", "notice");
            return false;
        }

        if ($("#Displayname").val() == "") {
            showMessage("Warning!", "Display name is required", "notice");
            return false;
        }
        if (isDuplicateDisplayName) {
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
                debugger
                if (grecaptcha != undefined && grecaptcha != null) {
                    grecaptcha.reset(signupFormWidgetId);
                }
                if (!data.Success) {
                    showMessage("Failed!", data.Message, data.Type);
                    f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                    f.find(":submit").prop('disabled', false);
                    return;
                }
                else {
                    showMessage("Success!", data.Message, data.Type);
                }
                wizardData.BasicData = {
                    UserId: data.Data.UserId,
                    OrderId: data.Data.OrderId
                };
                console.log(wizardData)
                $("#hdnUserId").val(data.Data.UserId);
                $("#hdnOrderId").val(data.Data.OrderId);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
                nextStep();
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
    var validateReferalCode = function () {
        var obj = getFormInput("CreateUserForm", "ReferredByCode");
        if (obj.val() != null && obj.val().trim() != "") {
            $.get("/Users/CheckValidReferalCode", { ReferredByCode: obj.val() }, function (res) {
                if (res.Success) {
                    isValidReferralCode = true;
                    return;
                }
                showMessage(res.Message, res.Data, res.Type);
            }).fail(function () {
                showMessage("Failed", "Some error occurred when validating referral code, please try again later.", "error");
            });
        }
    }
    // End signup form validation

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
    var submitOrderUpsertForm = function (fromLogin = false) {
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
                    f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                    f.find(":submit").prop('disabled', true);
                    $.ajax({
                        type: f[0].method,
                        url: f[0].action,
                        data: wizardData,
                        dataType: 'json',
                        success: function (res, strStatus) {
                            if (!res.Success) {
                                showMessage("Failed!", res.Message, res.Type);
                                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                                f.find(":submit").prop('disabled', false);
                                return;
                            }
                            wizardData.payment.InitialData = {
                                PackageCost: res.Data.PackageCost,
                                ReferralDiscount: res.Data.ReferralDiscount || 0,
                                ShippingCost: res.Data.ShippingCost || 0,
                                Tax: res.Data.Tax,
                                CouponDiscount: 0,
                                PaymentAmount: parseFloat(res.Data.PackageCost) - (parseFloat(res.Data.ReferralDiscount) || 0),
                                MetalCardAmount: 0
                            };

                            wizardData.payment.updatedData = {
                                PackageCost: res.Data.PackageCost,
                                ReferralDiscount: res.Data.ReferralDiscount || 0,
                                ShippingCost: res.Data.ShippingCost || 0,
                                Tax: res.Data.Tax,
                                TotalDiscount: res.Data.ReferralDiscount || 0,
                                DiscountedPrice: parseFloat(res.Data.PackageCost) - (parseFloat(res.Data.ReferralDiscount) || 0),
                                CouponDiscount: 0,
                                PaymentAmount: parseFloat(res.Data.PackageCost) - (parseFloat(res.Data.ReferralDiscount) || 0),
                                //MetalCardAmount: wizardData.CardStyle.IncludeMetalCard ? 3499 : 0
                                MetalCardAmount: 0
                            };
                            console.log("wizard data", wizardData);
                            displayPaymentDetails();
                            nextStep();
                            f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                            f.find(":submit").prop('disabled', false);
                        },
                        error: function () {
                            handleAjaxError()
                            f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                            f.find(":submit").prop('disabled', false);
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
        if (!me.isValidCoupon && $("#DiscountCoupon").val()!="") {
           
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
                    $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'none');
                    $("#orderPaymentForm").find(":submit").prop('disabled', false);
                    return;
                }

                me.RazorPayLatestOrderId = res.Data.RazorOorderId;
                console.log("create razor pay order result", res);
                wizardData.BasicData.RazorPayOrderId = res.Data.RazorOorderId;
                if (callback) {
                    callback(res.Data.RazorOorderId, res.Data.RazorOrder);
                }
                $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'none');
                $("#orderPaymentForm").find(":submit").prop('disabled', false);
            }
        ).fail(handleAjaxError(function () {
            debugger;
            $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'none');
            $("#orderPaymentForm").find(":submit").prop('disabled', false);
        }));
    }

    var initializeRazorpay = function (orderId, orderDetails) {
        debugger
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
                debugger
                console.log('handler response', response);
                $("#WantMetalCard").val(wizardData.CardStyle.IncludeMetalCard ?? false);
                const PersonalInfoUserForm = document.getElementById("orderPaymentForm");
                var formData = new FormData(PersonalInfoUserForm);
                formData.append("razorpay_payment_id", response.razorpay_payment_id);
                formData.append("razorpay_order_id", response.razorpay_order_id);
                formData.append("razorpay_signature", response.razorpay_signature);
                //formData.append("WantMetalCard", wizardData.CardStyle.IncludeMetalCard ?? false);
                formData.append("OrderId", wizardData.BasicData.OrderId);
                formData.append("UserId", wizardData.BasicData.UserId);
                //var btn = $("#btnOrderCartFinish");
                $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                $("#orderPaymentForm").find(":submit").prop('disabled', true);
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
                            $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'none');
                            $("#orderPaymentForm").find(":submit").prop('disabled', false);
                            return;
                        }

                        $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        $("#orderPaymentForm").find(":submit").prop('disabled', false);
                        window.location.href = "/Users/OrderPlacedView";
                    },

                    error: handleAjaxError(function () {
                        me.razorPayInsatance = null;
                        $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        $("#orderPaymentForm").find(":submit").prop('disabled', false);
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
                $("#orderPaymentForm").find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                $("#orderPaymentForm").find(":submit").prop('disabled', true);
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

        //if (d.MetalCardAmount != 0) {
        //    $("#metalCardPriceContainer").show();
        //} else {
        //    $("#metalCardPriceContainer").hide();
        //}

        $("#metalCardPriceContainer").hide();

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
                }
                if (amountOff != null && amountOff != 0) {
                    d.CouponDiscount = parseFloat(amountOff) || 0;
                }
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

        $("#btnShippingDetailSubmitValidate").click(function () {
            
            if (!$("#IsSelfPick").is(":checked")) {
                // Remove validation rules
                $("#ShippingAddress1, #ShippingCity, #ShippingState, #ShippingCountry, #ShippingZip").each(function () {
                    $(this).rules("add", {
                        required: true
                    });
                   
                });
            } else {
                // Add required rule back
                $("#ShippingAddress1, #ShippingCity, #ShippingState, #ShippingCountry, #ShippingZip").each(function () {
                    $(this).rules("remove", "required");
                });
            }


            //if ($("#IsSelfPick").is(":checked")) {
            //    alert("checked self")
            //        $("#ShippingAddress1, #ShippingCity, #ShippingState, #ShippingCountry, #ShippingZip").each(function () {
            //            $(this).valid();  // this uses the dynamic rule defined in .validate()
            //        });
            //    }
           
        });
       

        $(document).on('change', "#chkSameShippingDetails", function () {
            debugger
            var isChecked = $(this).is(':checked');
            var fetchInput = getFormInputByName("orderUpsertForm");

            function syncBillingWithShipping() {
                if ($("#chkSameShippingDetails").is(':checked')) {
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
        $(document).on('blur', '#ReferredByCode', validateReferalCode);

        $(document).on("input", '#ReferredByCode', function () {
            isValidReferralCode = false;
        })

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

        $(window).on('load', function () {
            signUpForm();
            var code = $("#hdnReferalCode").val();
            if (code != "") {
                document.getElementById("CreateUserForm").reset();
                getFormInput("CreateUserForm", "ReferredByCode").val(code);
                validateReferalCode();
            }
            updateNfcCardDetails();
            submitOrderUpsertForm();
            submitOrderPaymentForm();
        });
        $.validator.addMethod("customRegex", function (value, element, param) {
            return this.optional(element) || param.test(value);
        }, "Please enter a valid");

        //$("#btnSignupNext").click(function () {
        //    if ($("#privacy-policy").is(":checked")) {
        //        $("#CreateUserForm").submit();
        //    }
        //});

        $(".btnPrev").click(function () {
            prevStep();
        });

        $("#btnNextSelectNfc").click(function () {
            nextStep();
            setNfcCardDetailsPreview();
        });

        //$("#btnNextCardDetails").click(function () {
        //    $("#nfcCardDetailForm").submit();
        //});

        //$("#btnShippingNext").click(function () {
        //    finishWizard();
        //    if (confirm("Are you sure you have filled all details, to go to final step?")) {
        //        $("#orderUpsertForm").submit();
        //    }
        //});

        $("#btnApplyDiscountCoupon").on("click", checkValidDiscountCoupon);
        $("#btnCancelDiscountCoupon").on("click", function (event) {
            event.preventDefault();
            $("#DiscountCoupon").prop("readonly", false);
            $("#DiscountCoupon").val("");
            setDefaultDiscountPrice();
            $("#btnCancelDiscountCoupon").hide();
            $("#btnApplyDiscountCoupon").show();
        });

        $("#btnPlaceOrder").click(function () {
            $("#orderPaymentForm").submit();
        });

        $(document).on("click", "#termsOfServices_link", function () {
            var modal = $("#termsOfServicesModal");
            modal.modal('show');
        });


        $(document).on("click", "#privacyPolicy_link", function () {
            var modal = $("#privacyPolicyModal");
            $("#SignupModal").hide()
            modal.modal('show');
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

    }
}