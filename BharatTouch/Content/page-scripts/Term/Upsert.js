var upsertTerm = function () {
    var me = this;
    this.upsertTermUrl = null;
    this.GetTermUrl = null;
    this.loggedUser = null;

    var GetTermText = function () {
        CKEDITOR.instances.TermText.setData('');
        $.ajax({
            type: "POST",
            url: me.GetTermUrl,
            dataType: "json",
            success: function (res) {
                var text = res.data.TermText;
                setTimeout(function () {
                    CKEDITOR.instances.TermText.setData(text);
                }, 2000);
                
            }
        });
    }

    $('#UpsertTerm').click(function () {
        var Text = CKEDITOR.instances.TermText.getData();
        if (Text != "") {
            var TermTextData = new FormData();
            TermTextData.append('TermTxt', Text);
            $.ajax({
                url: '/Term/Upsert',
                type: "POST",
                contentType: false,
                processData: false,
                data: TermTextData,
                success: function (data) {
                    showMessage(data.Message, data.Data, data.Type);
                    window.location.href = "/Term/Upsert";
                }
            });
        }
        else {
            showMessage("Failed", "Term is required.", "error");
        }

    });

    this.init = function () {

        CKEDITOR.replace('TermText', {
            width: '100%',
            height: 450
        } );
        CKEDITOR.config.entities = false;
        CKEDITOR.config.htmlEncodeOutput = false;
        //CKEDITOR.resize('100%', '550')
        //CKEDITOR.resize('100%', '550')

        GetTermText();
        $("#upsertFormContainer").on("click", "button,a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {

            }
        });
    }
}

