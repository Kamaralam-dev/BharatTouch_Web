var listCompany_admin = function () {
    var me = this;
    this.getCompanyUrl = null;
    this.deleteCompanyUrl = null;
    this.addEditCompanyUrl = null;
    this.getUserHaveCompany = null;

    var loadCompany = function () {
        $("#tableCompany_Admin").DataTable({
            "processing": true, // for show progress bar
            "serverSide": false, // for process server side
            "filter": true, // this is for disable filter (search box)
            //"scrollX": "auto",
            "scrollCollapse": false,
            "lengthMenu": [25, 50, 75, 100],
            "pageLength": 25,
            "autoWidth": false,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getCompanyUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "CompanyId", "name": "CompanyId", "autoWidth": true, "className": "dt-body-left" },
                { "data": "CompanyName", "name": "CompanyName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Phone", "name": "Phone", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": "IsActive", "name": "IsActive", "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return row.IsActive == true ? "Yes" : "No";
                    }
                },
               
                {
                    "data": null,
                    "name": "",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.CompanyId + '" ><i style="color:green;" class="fa fa-pencil"></i></a> &nbsp;'
                        var DeleteButton = ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.CompanyId + '" class="delete-row"><i style="color:red;" class="fa fa-trash"></i></a>'
                        return EditButtons + DeleteButton;
                    }
                },

            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    $("#btnAddCompany").click(function (e) {
        openAddEditCompanyModal();
    });

    function openAddEditCompanyModal(id) {

        $.post(me.addEditCompanyUrl, { id: id }, (data) => {

            $("#ModaladdEditCompany .modal-body").empty();
            $("#ModaladdEditCompany .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditCompany'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    this.init = function () {

        $("#tableCompany_Admin").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            var Companyname = $(this).data('Companyname');

            switch (action) {
                case "edit":
                    openAddEditCompanyModal(id);
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deleteCompanyUrl, { id: id }, "tableCompany_Admin");
                    break;
            }
        });


        loadCompany();
    }
}