var homeJs = function () {
    var me = this;
    var sendEmailContactHomeWidgetId = null;
    this.captchaSiteKey = null;

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
                        || element[0].name == 'Message') {
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
        $.ajax({
            type: form[0].method,
            url: form[0].action,
            data: formData,
            dataType: 'json',
            success: function (data, strStatus) {
                showMessage(data.Message, data.Data, data.Type);
                if (data.Success) {
                    $("#Name").val('');
                    $("#Email").val('');
                    $("#PhoneNo").val('');
                    $("#Message").val('');
                    $("#ddlCountryCode").val("");
                    if (grecaptcha != undefined && grecaptcha != null) {
                        grecaptcha.reset(sendEmailContactHomeWidgetId);
                    }
                    $('#hdngrecaptchaResponseContactForm').val("");
                }
            },
            error: function () {
                if (grecaptcha != undefined && grecaptcha != null) {
                    grecaptcha.reset(sendEmailContactHomeWidgetId);
                }
                handleAjaxError();
            }
        });
    }

    this.init = function () {
        $(window).on("load", sendEmailContactForm);
    }
}