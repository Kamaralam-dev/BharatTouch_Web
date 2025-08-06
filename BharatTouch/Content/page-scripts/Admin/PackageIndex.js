var listPackages_admin = function () {
    var me = this;
    this.getPackageUrl = null;
    this.deletePackageUrl = null;
    this.addEditPackageUrl = null;
    this.getUserHavePackage = null;

    var loadPackage = function () {
        $("#tablePackage_Admin").DataTable({
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
                "url": me.getPackageUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "PackageId", "name": "PackageId", "autoWidth": true, "className": "dt-body-left" },
                { "data": "PackageName", "name": "PackageName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "PackageCost", "name": "PackageCost", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null, "name": "UserCount", "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        var userCount = 0;
                        if (data.UserCount != null)
                            userCount = data.UserCount;

                        return ' <a href="javascript:void(0)" style="cursor:pointer;text-decoration:underline;" data-action="usercount" data-packagename="' + data.PackageName + '" data-id="' + data.PackageId + '" >' + userCount + '</a> &nbsp;'

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
                        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.PackageId + '" ><i style="color:green;" class="fa fa-pencil"></i></a> &nbsp;'
                        var DeleteButton = ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.PackageId + '" class="delete-row"><i style="color:red;" class="fa fa-trash"></i></a>'
                        return EditButtons + DeleteButton;
                    }
                },

            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    $("#btnAddPackage").click(function (e) {
        openAddEditPackageModal();
    });

    function openAddEditPackageModal(id) {

        $.post(me.addEditPackageUrl, { id: id }, (data) => {

            $("#ModaladdEditPackage .modal-body").empty();
            $("#ModaladdEditPackage .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditPackage'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    function openUserModal(id, packagename) {
        $('#userModal').modal('show'); 
        $("#tableUser_Admin").DataTable({
            destroy: true,
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
                "url": me.getUserHavePackage,
                "type": "POST",
                "datatype": "json",
                "data": { packageId: id } 
            },
            "columns": [
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
                    "data": null,
                    "name": "Phone",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => `${(data.NumberCode != null) ? data.NumberCode : ""} ${(data.Phone != null) ? data.Phone : ""}`
                },

            ],
            "initComplete": function (settings, json) {
                $(".modal-title").text("Package : " + packagename);
            }
        });
    }

    this.init = function () {

        $("#tablePackage_Admin").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            var packagename = $(this).data('packagename');

            switch (action) {
                case "usercount":
                    openUserModal(id, packagename);
                    break;
                case "edit":
                    openAddEditPackageModal(id);
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deletePackageUrl, { id: id }, "tablePackage_Admin");
                    break;
            }
        });


        loadPackage();
    }
}