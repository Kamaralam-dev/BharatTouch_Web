var featureNotification = function () {
    var me = this;

    var featureNotificationEmailForm = function () {

        $("#featureNotificationEmailForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'EmailSubject' || element[0].name == 'emailBody' || element[0].name == "emailDescription") {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                //EmailSubject: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var formData = new FormData();
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
                        $("#EmailSubject").val('');
                        $("#emailBody").val('');
                    },
                    error: function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        handleAjaxError()
                    }
                });
            }
        });
        
    }

    this.init = function () {
        ClassicEditor
            .create(document.querySelector('#emailBody'))
            .then(editor => {
                editor.ui.view.editable.element.style.height = '200px';
                console.log('CKEditor 5 is ready.');
            })
            .catch(error => {
                console.error('There was an error initializing CKEditor 5:', error);
            });
        featureNotificationEmailForm();
    }
}