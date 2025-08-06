var upsertTestimonial = function () {
    var me = this;
    this.upsertTestimonialUrl = null;
    this.deleteTestimonialUrl = null;
    var IsNewSave = 0;
    this.loggedUser = null;

    var upsertTestimonialForm = function () {
        $("#upsertTestimonialForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name') {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                Title: { required: true },
                Description: { required: true },
                GivenBy: { required: true },
                GivenByTitle: { required: true },
                GivenByCompany: { required: true },
                IsActive: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                formData = new FormData($('form').get(0));
                $(data).each(function (index, element) {
                    formData.append(element.name, element.value);
                    if (element.name == "Description") {
                        var text = CKEDITOR.instances.Description.getData();
                        formData.append('desc', text);
                    }
                    else {
                        formData.append(element.name, element.value);
                    }

                });
                submitTestimonialForm(f, formData);
            }
        });
    }

    var submitTestimonialForm = function (f, formData) {
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
                    window.location.href = "/Testimonial/Upsert";
                }
                else {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Testimonial/Upsert/" + data.OptionalValue;
                }
            },
            error: handleAjaxError()
        });
    }


    this.init = function () {

        CKEDITOR.replace('Description');
        CKEDITOR.config.entities = false;
        CKEDITOR.config.htmlEncodeOutput = false;

        upsertTestimonialForm();
        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "cancel":
                    window.location.href = "/Testimonial/index";
                    break;
                case "savenew":
                    IsNewSave = 1;
                    break;


            }
        });
    }
}

