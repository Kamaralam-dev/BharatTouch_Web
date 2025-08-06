var listOrder_admin = function () {
    var me = this;
    this.getorderUrl = null;
    this.saveInPrinting = null;
    this.saveOrderShipped = null;
    this.addEditOrderShipped = null;
    this.saveLeadConvertFromOrder = null;

    var loadOrderList = function () {
        $("#tableOrder_Admin").DataTable({
            "processing": false, // for show progress bar
            "serverSide": false, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": false,
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getorderUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "OrderId", "name": "OrderId", "autoWidth": true, "visible": true },
                { "data": "OrderNo", "name": "OrderNo", "autoWidth": true, "className": "dt-body-left" },
                { "data": "CustomerName", "name": "CustomerName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "EmailId", "name": "EmailId", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null, "name": null, "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return (row.NumberCode ?? '') + (row.Phone ? ' ' + row.Phone : '');

                    }
                },
                {
                    "data": null, "name": "OrderDate", "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return momentDateFormatter(row.OrderDate, "MM/DD/YYYY");
                    }
                },
                { "data": "OrderStatus", "name": "OrderStatus", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null, "name": "TotalAmount", "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        // return "$" + row.TotalAmount;
                        return row.TotalAmount;
                    }
                },
                { "data": "PaymentMethod", "name": "PaymentMethod", "autoWidth": true, "className": "dt-body-left" },

                {
                    "data": null,
                    "name": "InPrinting",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    "render": function (data, type, row) {

                        var textPri = row.InPrinting == true ? "Yes" : "No";

                        if (row.OrderStatus.toLowerCase() == "captured") {
                            return '<a href="javascript:void(0)" data-action="inprinting" style="cursor:pointer;text-decoration:underline;" data-id="' + data.OrderId + '"><b>' + textPri + '</b></a>';
                        } else {
                            return textPri;
                        }
                    }
                },
                {
                    "data": null,
                    "name": "IsShipped",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    "render": function (data, type, row) {

                        var textship = row.IsShipped == true ? "Yes" : "No";
                        var flag = row.IsShipped == true ? 1 : 0;
                        if (row.OrderStatus.toLowerCase() == "captured") {
                            return '<a href="javascript:void(0)" data-action="isshipped" style="cursor:pointer;text-decoration:underline;" data-flag="' + flag + '" data-id="' + data.OrderId + '"><b>' + textship + '</b></a>';
                        } else {
                            return textship;
                        }
                    }
                },
                {
                    "data": null, "name": "IsSelfPick", "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return row.IsSelfPick == true ? "Yes" : "No";
                    }
                },
                {
                    "data": null, "name": null, "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        if (row.IsHide != "1") {
                            return '<a href="javascript:void(0)" data-action="lead" style="cursor:pointer;text-decoration:underline;" data-id="' + data.OrderId + '"><b>Convert to lead</b></a>';
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

    function openInPrintingSwalPopup(id) {
        Swal.fire({
            title: "Are you sure?",
            text: "You want to update the order status to 'In Printing'? This will confirm that the NFC card has been sent for printing and cannot be edited further.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, Go for it!",
            width: "800px",
        }).then((result) => {
            if (result.isConfirmed) {
                SaveInPrintingStatus(id);
            }
        });

    }

    function SaveInPrintingStatus(id) {
        $.post(
            me.saveInPrinting,
            { orderId: id },
            function (data) {
                showMessage(data.Message, data.Data, data.Type);
                ReloadDataTable("tableOrder_Admin");
                Swal.close();
            }
        ).fail(handleAjaxError());
    }

    function ReloadDataTable(tableId) {
        $('#' + tableId).DataTable().ajax.reload(null, false);
    }

    function openIsShippedModal(id, flag) {

        $.post(me.addEditOrderShipped, { orderId: id, flag: flag }, (data) => {

            $("#ModaladdEditOrderShipped .modal-body").empty();
            $("#ModaladdEditOrderShipped .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditOrderShipped'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    function openLeadConvertFromOrderModal(id) {
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
            me.saveLeadConvertFromOrder,
            { orderId: id },
            function (data) {
                showMessage(data.Message, data.Data, data.Type);
                ReloadDataTable("tableOrder_Admin");
                Swal.close();
            }
        ).fail(handleAjaxError());
    }

    this.init = function () {

        $("#tableOrder_Admin").on("click", "a", function () {
            var obj = $(this);
            var action = $(this).data("action");
            var id = $(this).data('id');
            var flag = $(this).data('flag');
            switch (action) {
                case "inprinting":
                    openInPrintingSwalPopup(id);
                    break;
                case "isshipped":
                    openIsShippedModal(id, flag);
                    break;
                case "lead":
                    openLeadConvertFromOrderModal(id);
                    break;

            }
        });


        loadOrderList();
    }
}