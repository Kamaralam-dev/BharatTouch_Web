var upsertPayment = function () {
    var me = this;
    this.upsertPaymentUrl = null;
    this.deletePaymentUrl = null;
    var IsNewSave = 0;
    this.loggedUser = null;


    var upsertPaymentForm = function () {
        $("#upsertPaymentForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                OrderId: { required: true },
                PaymentDate: { required: true },
                PaymentAmount: { required: true },
                PaymnetModeId: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                formData = new FormData($('form').get(0));
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });
                submitPaymentForm(f, formData);

            }
        });
    }

    var submitPaymentForm = function (f, formData) {
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
                    window.location.href = "/Payment/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Payment/Upsert/" + data.OptionalValue;
                }
            },
            error: handleAjaxError()
        });
    }

    this.init = function () {


        upsertPaymentForm();
        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Payment/index";
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;


            }
        });
    }
}

