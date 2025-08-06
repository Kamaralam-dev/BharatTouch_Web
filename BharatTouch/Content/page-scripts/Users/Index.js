var listUser = function () {
    var me = this;
    this.getUserUrl = null;
    this.deleteUserUrl = null;
    this.editUserUrl = null;
    this.userId = null;
    this.pageSize = null;
    this.weburl = null;
    var emailis = "";

    var loadUserList = function () {
        $("#tableUser").DataTable({
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
                "url": me.getUserUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "UserId", "name": "UserId", "autoWidth": true, "visible": false },
                {
                    "data": null,
                    "name": "",
                    "orderable": false,
                    "className": "center",
                    "autoWidth": true,
                    "width": "2%",
                    render: function (data, type, row) {
                        return '<input type="checkbox" class="to-emails" data-email="' + data.EmailId + '" />&nbsp;'
                    }
                },
                {
                    "data": null,
                    "name": "Displayname",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        return (data.Displayname != null) ? data.Displayname : "";
                    }
                },
                {
                    "data": null,
                    "name": "Displayname",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var profile = me.weburl + data.Displayname;
                        //if (data.Displayname != "" && data.Displayname != null) {
                        //    profile = 'http://aviecard.com/' + data.Displayname;
                        //}
                        return `<a target="_blank" href="${profile}">${data.Displayname}</a>`
                    }
                },
                { "data": "EmailId", "name": "EmailId", "autoWidth": true, "className": "dt-body-left" },
                { 
                    "data": null, 
                    "name": "Phone",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => `${(data.NumberCode != null) ? data.NumberCode : ""} ${(data.Phone != null) ? data.Phone : ""}`
                },
                {
                    "data": null,
                    "name": "IsActive",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => data.IsActive == true ? "Active" : "InActive"
                    
                },
                {
                    "data": null,
                    "name": "PageViewed",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var PageViewed = 0;
                        if (data.PageViewed != null)
                            PageViewed = data.PageViewed;
                        return PageViewed
                    }
                },
                {
                    "data": null,
                    "name": "LastLogin",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        return (data.LastLogin != null) ? formatJsonDate(data.LastLogin) : ""
                    }
                },
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
                //{
                //    "data": null,
                //    "name": "",
                //    "orderable": false,
                //    "className": "center",
                //    "autoWidth": true,
                //    "width": "10%",
                //    render: function (data, type, row) {
                //        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.UserId + '" ><i class="fa fa-pencil font20"></i></a> &nbsp;'
                //        var DeleteButton = ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.UserId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
                //        if (me.userId == data.UserId) {
                //            return EditButtons + '<a href="javascript:void(0)" data-action="access-notify"> <i class="fa fa-trash-o" ></i></a>';
                //        }
                //        else {
                //            return EditButtons + DeleteButton;
                //        }
                //    }
                //}
            ],
            "initComplete": function (settings, json) {
                $(".to-emails").change(function () {
                    emailis = "";
                    $('.to-emails').each(function (i, obj) {
                        if ($(this).is(":checked")) {
                            emailis += $(this).data('email') + ",";
                        }
                    });
                });
            }
        });
    }

    var openEmailModel = function (emails) {
        if (emails == "")
        {
            showMessage("Warning!", "Select atleast one email.", "notice");
            return false;
        }
        //$(obj).find("div.lds-dual-ring").css('display', 'inline-block');
        //$(obj).prop('disabled', true);
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/EmailModel',
            data: { emails: emails },
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                OpenModel("userEmailModal");

                ClassicEditor
                    .create(document.querySelector('#emailBody'))
                    .then(editor => {
                        editor.ui.view.editable.element.style.height = '200px';
                        console.log('CKEditor 5 is ready.');
                    })
                    .catch(error => {
                        console.error('There was an error initializing CKEditor 5:', error);
                    });
                sendEmailForm();
            },
            error: function () {
                //$(obj).find("div.lds-dual-ring").css('display', 'none');
                //$(obj).prop('disabled', false);
                handleAjaxError()
            }
        });
    }

    var sendEmailForm = function () {

        $("#emailUserForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'EmailSubject' || element[0].name == 'emailBody') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                EmailSubject: { required: true }
            },
            submitHandler: function (form) {             
                var f = $(form);
                var data = f.serializeArray();
                var formData;
                formData = new FormData();
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
                        $("#userEmailModal").modal('hide');
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        $("#EmailSubject").val('');
                        $("#emailBody").val('');
                        $("#EmailIds").val('');
                        setTimeout(function () {
                            window.location.reload();
                        }, 3000);
                       
                    },
                    error: function () {
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                        handleAjaxError()
                    }
                });
            }
        });

        $("#userEmailModal")
            .modal({ show: true, backdrop: false })
            .on('click', '.mfp-close', function (e) {
                CloseModal();
            });
    }

    this.init = function () {

        //$("#tableUser").on("click", "a", function () {
        //    var action = $(this).data("action");
        //    var id = $(this).data('id');
        //    switch (action) {
        //        case "edit":
        //            window.location.href = "/Users/Create/" + id;
        //            break;
        //        case "delete":
        //            deleteRecord(me.deleteUserUrl, { id: id }, "tableUser");
        //            break;
        //        case "access-notify":
        //            accessNotify();
        //            break;
        //    }
        //});


        $("#btnSendEmail").click(function () {
            var filter = $(this).val();
            //alert(emailis.replace(/,\s*$/, ""))
            openEmailModel(emailis);

        });

        loadUserList();
    }
}