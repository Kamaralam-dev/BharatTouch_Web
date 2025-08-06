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
            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    this.init = function () {

        loadBulkOrder();
    }
}