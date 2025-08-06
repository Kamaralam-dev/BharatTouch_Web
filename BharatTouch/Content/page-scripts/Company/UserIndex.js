var listUser_Company = function () {
    var me = this;
    this.getUserUrl = null;
    this.deleteUserUrl = null;
    this.createUserUrl = null;
    this.loggeduserid = null;
    this.loggedcompanyid = null;

    var loadUserList = function () {
        $("#tableUser_Company").DataTable({
            "processing": true, // for show progress bar
            "serverSide": false, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": false,
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
                { "data": "UserId", "name": "UserId", "autoWidth": true, "visible": true },
                {
                    "data": null,
                    "name": "FirstName",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var lastName = row.LastName == null ? "" : row.LastName;
                        return data.FirstName + " " + lastName;
                    }
                },
                { "data": "EmailId", "name": "EmailId", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": "Phone",
                    "name": "Phone",
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": null,
                    "name": "IsActive",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        return row.IsActive == true ? "Yes" : "No";
                    }
                },
                {
                    "data": null,
                    "name": "IsCompanyAdmin",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        return row.IsCompanyAdmin == true ? "Yes" : "No";
                    }
                },
                {
                    "data": null,
                    "name": "",
                    "width": "5%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.UserId + '" ><i style="color:green;" class="fa fa-pencil"></i></a> &nbsp;'
                        var DeleteButton = ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.UserId + '" class="delete-row"><i style="color:red;" class="fa fa-trash"></i></a>'
                       

                        if (!row.IsCompanyAdmin) {
                            return EditButtons + DeleteButton;
                        }
                        else {
                            return EditButtons;
                        }
                    }
                },


            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    $("#btnAddNewUserByCompany").click(function (e) {
        openAddEditUserModal();
    });

    function openAddEditUserModal(id) {
        $.post(me.createUserUrl, { id: id }, (data) => {

            $("#ModaladdEditUserByCompany .modal-body").empty();
            $("#ModaladdEditUserByCompany .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditUserByCompany'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    this.init = function () {

        $("#tableUser_Company").on("click", "a", function () {
            var obj = $(this);
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    openAddEditUserModal(id);
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deleteUserUrl, { id: id }, "tableUser_Company");
                    break;
            }
        });

        loadUserList();
    }
}