var listCardTemplates = function () {
    var me = this;
    this.BindTemplatesUrl = null;
    this.DeleteTemplateUrl = null;
    this.OpenFormModelUrl = null;
    this.pageSize = null;

    var loadCardTemplatesList = function () {
        $("#tableCardTemplate").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.BindTemplatesUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "TemplateId", "name": "TemplateId", "autoWidth": true, "visible": false },
                {
                    "data": null,
                    "name": "ImageUrl",
                    "orderable": false,
                    "className": "center",
                    "autoWidth": true,
                    "width": "2%",
                    render: function (data, type, row) {
                        return `<img src="${data.ImageUrl}" width="150px" height="100px" />`;
                    }
                },
                { "data": "Name", "name": "Name", "autoWidth": true, "className": "dt-body-left" },
                { "data": "TemplateView", "name": "TemplateView", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Types", "name": "Types", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "CreatedOn",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var CreatedOn = '';
                        if (data.CreatedOn != null)
                            CreatedOn = formatJsonDate(data.CreatedOn);
                        return CreatedOn
                    }
                },
                {
                    "data": null,
                    "name": "",
                    "orderable": false,
                    "className": "center",
                    "autoWidth": true,
                    "width": "10%",
                    render: function (data, type, row) {
                        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.TemplateId + '" ><div class="lds-dual-ring"></div><i class="fa fa-pencil font20"></i></a> &nbsp;'
                        var DeleteButton = ' <a href="javascript:void(0)" class="text-danger" data-action="delete" data-id="' + data.TemplateId + '" class="delete-row"><i class="fa fa-trash font20"></i></a>'
                        return EditButtons + DeleteButton;
                    }
                }
            ],
        });
    }

    var openTemplateFormModel = function (TemplateId, obj) {
        $("#upsertCardTemplateFormModel").remove();
        obj.find("div.lds-dual-ring").css('display', 'inline-block');
        obj.prop('disabled', true);
        $.ajax({
            type: 'GET',
            cache: false,
            url: me.OpenFormModelUrl,
            data: { TemplateId: TemplateId },
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                obj.find("div.lds-dual-ring").css('display', 'none');
                obj.prop('disabled', false);
                OpenModel("upsertCardTemplateFormModel");
                UpsertTemplateForm();
            },
            error: handleAjaxError(function () {
                obj.find("div.lds-dual-ring").css('display', 'none');
                obj.prop('disabled', false);
            })
        });
    }

    var UpsertTemplateForm = function () {

        $("#upsertCardTemplateForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name' || element[0].name == 'TemplateView') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                Name: { required: true },
                TemplateView: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                const UpsertCardTemplateForm = document.getElementById("upsertCardTemplateForm");
                var formData = new FormData(UpsertCardTemplateForm);

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
                        $("#userEmailModal").modal('hide');
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        setTimeout(function () {
                            window.location.reload();
                        }, 3000);

                    },
                    error: handleAjaxError(function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });

    }

    this.init = function () {

        $("#tableCardTemplate").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    openTemplateFormModel(id, $(this));
                    break;
                case "delete":
                    deleteRecord(me.DeleteTemplateUrl, { TemplateId: id }, "tableCardTemplate");
                    break;
            }
        });


        $("#btnAddTemplate").click(function () {
            openTemplateFormModel(0, $(this));
        });

        loadCardTemplatesList();
    }
}