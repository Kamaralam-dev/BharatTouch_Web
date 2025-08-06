var bulkOrderUpsert = function () {
    this.captchaSiteKey = null;
    var me = this;

    //bulkorder upsert
    var createBulkOrderForm = function () {

        if (grecaptcha != undefined && grecaptcha != null) {
            bulkOrderFormWidgetId = grecaptcha.render('grecaptchaOrder', {
                sitekey: me.captchaSiteKey,
                type: 'image',
                callback: function (token) {
                    $('#hdngrecaptchaResponseOrder').val(token);
                },
                'expired-callback': function () {
                    $('#hdngrecaptchaResponseOrder').val("");
                },
                'error-callback': function () {
                    $('#hdngrecaptchaResponseOrder').val("");
                }
            });
        }

        $("#CreateBulkOrderForm").validate({
            errorPlacement: function (error, element) {
                error.insertAfter(element);
            },
            rules: {
                CompanyName: { required: true },
                Email: { required: true, emailvalidatecustom: true },
                PhoneNo: { required: true },
                ContactPerson: { required: true },
                MinOrder: {
                    required: true,
                    min: 1,              
                    digits: true,        
                    range: [1, 1000]     
                },
            },
            messages: {
                CompanyName: {
                    required: "Company Name is required."
                },
                Email: {
                    required: "Email is required.",
                    emailvalidatecustom: "Please Enter a valid email."
                },
                PhoneNo: {
                    required: "Phone is required."
                },
                ContactPerson: {
                    required: "Contact Person is required.",
                },
                MinOrder: {
                    required: "MinOrder is required.",
                }
            },
            submitHandler: function (form) {
                debugger;
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                data = data.map(function (item, i) {
                    debugger;
                    if (item.name == 'g-recaptcha-response') {
                        item.name = 'grecaptchaResponse';
                        response = item.value;
                    }
                    return item;
                });
                formData = new FormData($('form').get(0));

                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });
                submitBulkOrderForm(f, formData);

            }
        });
    }

    var submitBulkOrderForm = function (f, formData) {
        debugger;

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
                if (grecaptcha != undefined && grecaptcha != null) {
                    grecaptcha.reset(bulkOrderFormWidgetId);
                }
                showMessage(data.Message, data.Data, data.Type);
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
                if (data.Success)
                    f[0].reset();
            },
            error: handleAjaxError(function () {
                f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                f.find(":submit").prop('disabled', false);
            })
        });
    }

    window.onload = function () {
        createBulkOrderForm();
    };

    this.init = function () {

        // createBulkOrderForm(); // bulk order

    }

}



