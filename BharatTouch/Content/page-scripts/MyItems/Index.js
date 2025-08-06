var listItem = function () {

    var me = this;
    this.getItemUrl = null;
    this.deleteItemUrl = null;
    this.upsertItemUrl = null;
    this.pageSize = null;
    this.LoggedUserId = 0;
    this.IsDonor = null;

    var loadItemList = function () {
        $("#tableItems").DataTable({
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
                "url": me.getItemUrl,
                "type": "POST",
                "datatype": "json",
                "data": { "donorId": me.LoggedUserId },
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "ItemId", "name": "ItemId", "autoWidth": true, "visible": false, "className": "dt-body-left" },
                { "data": "ItemName", "name": "ItemName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Category", "name": "Category", "autoWidth": true, "className": "dt-body-left" },
                { "data": "SubCategory", "name": "SubCategory", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Donor", "name": "Donor", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Quantity", "name": "Quantity", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Condition", "name": "Condition", "autoWidth": true, "className": "dt-body-left" },
                { "data": "MarketValue", "name": "MarketValue", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Phone", "name": "Phone", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "ClaimByDate",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var ClaimByDate = '';
                        if (data.ClaimByDate != null)
                            ClaimByDate = formatJsonDate(data.ClaimByDate);
                        return ClaimByDate

                    }
                },
                {
                    "data": null,
                    "name": "Name",
                    "orderable": true,
                    "className": "center",
                    "width": "10%",
                    "visible": true,
                    render: function (data, type, row) {

                        if (me.IsDonor.toLowerCase() == "donor") {
                            var edit = ' <a href="javascript:void(0)" title="Edit Item" data-action="edit" data-id="' + data.ItemId + '" ><i class="fa fa-pencil font20"></i></a>';
                            var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.ItemId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
                            return edit + DeleteButton;
                        }
                        else {
                            return ' <a href="javascript:void(0)" title="View Item" data-action="view" data-id="' + data.ItemId + '" ><i class="fa fa-eye font20"></i></a>';
                        }
                    }
                }
            ],
            "initComplete": function (settings, json) {
                $(this.api().table().header()).find('th').css({ 'padding': '6px 18px' });
                //if (me.IsDonor.toLowerCase() == "donor") {
                //    $("#tableItems").DataTable().column(8).visible(true);
                //}
                //else {
                //    $("#tableItems").DataTable().column(8).visible(false);
                //}
            }
        });
    }

    this.init = function () {

        $("#tableItems").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/myItem/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteItemUrl, { id: id }, "tableItems");
                    break;
                case "view":
                    window.location.href = "/myItem/Upsert?id=" + id+"&v=1";
            }
        });

        loadItemList();
    }
}