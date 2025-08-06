var listOrder = function () {

    var me = this;
    this.getOrderUrl = null;
    this.deleteOrderUrl = null;
    this.upsertOrderUrl = null;
    this.pageSize = null;

    var loadOrderList = function () {
        $("#tableOrders").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
            "order": [[0, "desc"]],
            "lengthMenu": [10, 25, 50, 100],
            "pageLength": me.pageSize,
            "scrollCollapse": true,
            "autoWidth": true,
            "ajax": {
                "url": me.getOrderUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "OrderId", "name": "OrderId", "autoWidth": true, "visible": false, "className": "dt-body-left" },
                { "data": "Donar", "name": "Donar", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "OrderDate",
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var OrderDate = '';
                        if (data.OrderDate != null)
                            OrderDate = formatJsonDate(data.OrderDate);
                        return OrderDate
                    }
                },
                { "data": "OrderAmount", "name": "OrderAmount", "autoWidth": true, "className": "dt-body-left" },
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
                { "data": "CreatedByName", "name": "CreatedByName", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "Name",
                    "orderable": true,
                    "className": "center",
                    "width": "10%",
                    "visible": true,
                    render: function (data, type, row) {
                        var edit = ' <a href="javascript:void(0)" title="Edit Order" data-action="edit" data-id="' + data.OrderId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.OrderId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
                        return edit + DeleteButton;
                    }
                }
            ],
            "initComplete": function (settings, json) {
                $(this.api().table().header()).find('th').css({ 'padding': '6px 18px' });
            }  
        });
    }

    this.init = function () {

        $("#tableOrders").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Order/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteOrderUrl, { id: id }, "tableOrders");

            }
        });

        loadOrderList();
    }
}