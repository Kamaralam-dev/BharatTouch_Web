var upsertContactUs = function () {
    var me = this;
    this.upsertContactUsUrl = null;
    this.deleteContactUsUrl = null;
    var IsNewSave = 0;
    this.loggedUser = null;

    var upsertContactUsForm = function () {
        $("#upsertContactUsForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                ContactName: { required: true },
                Email: { required: true, emailvalidatecustom: true },
                Query: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                formData = new FormData($('form').get(0));
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                });
                submitContactUsForm(f, formData);
            }
        });
    }

    var submitContactUsForm = function (f, formData) {
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
                    window.location.href = "/ContactUs/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/ContactUs/Upsert/" + data.OptionalValue;
                }
            },
            error: handleAjaxError()
        });
    }

    this.init = function () {

        upsertContactUsForm();
        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/ContactUs/index";
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;


            }
        });
    }
}

