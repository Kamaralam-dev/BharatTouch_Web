var upsertFaq = function () {
    var me = this;
    this.upsertFaqUrl = null;
    this.deleteFaqUrl = null;
    var IsNewSave = 0;
    this.loggedUser = null;

    var upsertFaqForm = function () {
        $("#upsertFaqForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                Faq: { required: true },
                Answer: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                formData = new FormData($('form').get(0));
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                    if (element.name == "Answer") {
                        var Answer = CKEDITOR.instances.Answer.getData();
                        formData.append('Ans', Answer);
                    }
                    else {
                        formData.append(element.name, element.value);
                    }
                });
                submitFaqForm(f, formData);
            }
        });
    }

    var submitFaqForm = function (f, formData) {
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
                    window.location.href = "/Faq/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Faq/Upsert/" + data.OptionalValue;
                }
            },
            error: handleAjaxError()
        });
    }


    this.init = function () {

        CKEDITOR.replace('Answer');
        CKEDITOR.config.entities = false;
        CKEDITOR.config.htmlEncodeOutput = false;
        CKEDITOR.config.autoParagraph = false;

        upsertFaqForm();
        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Faq/index";
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;


            }
        });
    }
}

