var listLeads_admin = function () {
    var me = this;
    this.getAllLeadsAdmin = null;
    this.deleteLeadsAdmin = null;
    this.getLeadsAdminById = null;
    this.addEditLeadsCommunicationAdmin = null;
    this.getOrderDetail = null;

    var loadLeadsList = function () {
        $("#tableLeads_Admin").DataTable({
            "processing": false, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": false,
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getAllLeadsAdmin,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "LeadId", "name": "LeadId", "autoWidth": true, "visible": true },
                {
                    "data": "OrderId", "name": "OrderId", "autoWidth": true, "visible": true,
                    render: function (data, type, row) {
                        if (row.OrderId > 0) {
                            return '<a href="javascript:void(0)" data-action="orderdetail" style="cursor:pointer;text-decoration:underline;color:green;" data-id="' + row.OrderId + '"><b>' + row.OrderId + '</b></a>';
                        }
                        else {
                            return '';
                        }
                    }
                },
                {
                    "data": null, "name": null, "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return (row.Firstname ?? "") + " " + (row.Lastname ?? "");
                    }
                },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null, "name": null, "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return (row.NumberCode ?? '') + (row.Phone ? ' ' + row.Phone : '');
                    }
                },
                
                { "data": "Source", "name": "Source", "autoWidth": true, "className": "dt-body-left" },
                { "data": "CurrentStatus", "name": "CurrentStatus", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null, "name": "CreatedOn", "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return momentDateFormatter(row.CreatedOn, "MM/DD/YYYY");
                    }
                },
                //{ "data": "CreatedByName", "name": "CreatedByName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "AssignedToName", "name": "AssignedToName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Company", "name": "Company", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null, "name": null, "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return '<a target="_blank" href="/Admin/LeadCommunicationDetail?leadId=' + row.LeadId + '" style="cursor:pointer;text-decoration:underline;color:green;" data-id="' + data.LeadId + '"><b>Lead Communication</b></a>';
                    }
                },
                {
                    "data": null, "name": null, "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        var edit = '<a href="javascript:void(0)" data-action="edit" style="cursor:pointer;text-decoration:underline;color:green;" data-id="' + data.LeadId + '"><b>Edit</b></a>';
                        var del = '<a href="javascript:void(0)" data-action="delete" style="cursor:pointer;text-decoration:underline;color:red;" data-id="' + data.LeadId + '"><b>Delete</b></a>';
                        return edit + " | " + del;
                    }
                },

            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    $("#btnAddLeads").on("click", function () {
        openAddEditLeadsModal();
    });

    function openAddEditLeadsModal(id) {
        $.post(me.getLeadsAdminById, { id: id }, (data) => {

            $("#modalAddEditLeads .modal-body").empty();
            $("#modalAddEditLeads .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('modalAddEditLeads'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    function openOrderDeatilModal(orderId) {
        $.post(me.getOrderDetail, { orderId: orderId }, (data) => {

            $("#modalOrderDetail .modal-body").empty();
            $("#modalOrderDetail .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('modalOrderDetail'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    this.init = function () {

        $("#tableLeads_Admin").on("click", "a", function () {
            var obj = $(this);
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "orderdetail":
                    openOrderDeatilModal(id);
                    break;
                case "edit":
                    openAddEditLeadsModal(id);
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deleteLeadsAdmin, { id: id }, "tableLeads_Admin");
                    break;
            }
        });


        loadLeadsList();
    }
}