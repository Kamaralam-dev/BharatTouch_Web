var listItem = function () {

    var me = this;
    this.getItemUrl = null;
    this.deleteItemUrl = null;
    this.upsertItemUrl = null;
    this.pageSize = null;

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
                { "data": "MarketValue", "name": "MarketValue", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Phone", "name": "Phone", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "Name",
                    "orderable": true,
                    "className": "center",
                    "width": "10%",
                    "visible": true,
                    render: function (data, type, row) {
                        var edit = ' <a href="javascript:void(0)" title="Edit Item" data-action="edit" data-id="' + data.ItemId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.ItemId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
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

        $("#tableItems").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Item/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteItemUrl, { id: id }, "tableItems");

            }
        });

        loadItemList();
    }
}