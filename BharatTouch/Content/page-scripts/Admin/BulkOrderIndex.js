var listBulkOrder_admin = function () {
    var me = this;
    this.getBulkOrderUrl = null;

    var loadBulkOrder = function () {

        $("#tableBulkOrder_Admin").DataTable({
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
                "url": me.getBulkOrderUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "OrderRequestId", "name": "OrderRequestId", "autoWidth": true, "className": "dt-body-left" },
                { "data": "CompanyName", "name": "CompanyName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                { "data": "PhoneNo", "name": "PhoneNo", "autoWidth": true, "className": "dt-body-left" },
                { "data": "ContactPerson", "name": "ContactPerson", "autoWidth": true, "className": "dt-body-left" },
                { "data": "MinOrder", "name": "MinOrder", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Message", "name": "Message", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null, "name": null, "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        debugger
                        if (row.IsConvertedToLead == null || row.IsConvertedToLead == false) {
                            return '<a href="javascript:void(0)" data-action="lead" style="cursor:pointer;text-decoration:underline;" data-id="' + data.OrderRequestId + '"><b>Convert to lead</b></a>';
                        }
                        else {
                            return "";
                        }
                    }
                },
            ],
            "initComplete": function (settings, json) {

            }
        });
    }


    function openLeadConvertFromBulkOrderModal(id) {
        Swal.fire({
            title: "Are you sure?",
            text: "You want to convert order to lead.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, Go for it!",
            width: "800px",
        }).then((result) => {
            if (result.isConfirmed) {
                SaveLeadConvertFromOrderClick(id);
            }
        });

    }

    function SaveLeadConvertFromOrderClick(id) {
        $.post(
            me.saveLeadConvertFromBulkOrder,
            { orderRequestId: id },
            function (data) {
                showMessage(data.Message, data.Data, data.Type);
                $("#tableBulkOrder_Admin").DataTable().ajax.reload();
                Swal.close();
            }
        ).fail(handleAjaxError());
    }

    this.init = function () {

        loadBulkOrder();

        $("#tableBulkOrder_Admin").on("click", "a", function () {
            var obj = $(this);
            var action = $(this).data("action");
            var id = $(this).data('id');
            var flag = $(this).data('flag');
            switch (action) {
                case "lead":
                    openLeadConvertFromBulkOrderModal(id);
                    break;

            }
        });
    }
}