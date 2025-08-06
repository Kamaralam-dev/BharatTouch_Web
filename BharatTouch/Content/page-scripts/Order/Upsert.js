var upsertOrder = function () {
    var me = this;
    this.upsertOrderUrl = null;
    this.deleteOrderUrl = null;
    var IsNewSave = 0;
    this.loggedUser = null;
    

    var upsertOrderForm = function () {
        $("#upsertOrderForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                OrderTitle: { required: true },
                OrderAmount: { required: true },
                OrderDate: { required: true },
                DonorId: { required: true },
                ReceiverType: { required: true },
                PrimaryReceiverId: { required: true },
               
                OrderTitle: { required: true}
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                    formData = new FormData($('form').get(0));
                    $(data).each(function (index, element) {
                        formData.append(element.name, element.value);
                    });
                    submitOrderForm(f, formData);
                
            }
        });
    }

    var submitOrderForm = function (f, formData) {
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
                    window.location.href = "/Order/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Order/Upsert/" + data.OptionalValue;
                }
            },
            error: handleAjaxError()
        });
    }

    this.init = function () {
               
        $('#ReceiverType').change(function () {
            var receiverType = this.value == "" ? 0 : this.value;           
            $.ajax({
                type: "GET",
                cache: false,
                data: { receiverType: receiverType },
                url: '/Order/GetReceiverByType',
                dataType: 'json',
                success: function (data, strStatus) {
                    $("#PrimaryReceiverId").empty();
                    $("#PrimaryReceiverId").append($("<option></option>").val('').html("Receiver"));
                    $.each(data, function (key, value) {
                        for (var i = 0; value.length > i; i++) {
                            $("#PrimaryReceiverId").append($("<option></option>").val(value[i].UserId).html(value[i].UserName));
                        }
                    });

                },
            });
        });

        upsertOrderForm();

        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Order/index";
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;


            }
        });
    }
}

