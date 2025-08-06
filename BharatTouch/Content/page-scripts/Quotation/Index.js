var listQuotation = function () {

    var me = this;
    this.getQuotationUrl = null;
    this.deleteQuotationUrl = null;
    this.upsertQuotationUrl = null;
    this.pageSize = null;

    var loadQuotationList = function () {
        $("#tableQuotations").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
            "order": [[0, "desc"]],
            "lengthMenu": [10,25, 50, 100],
            "pageLength": me.pageSize,
            "scrollCollapse": true,
            "autoWidth": true,
            "ajax": {
                "url": me.getQuotationUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "Title", "name": "Title", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Description", "name": "Description", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "IsActive",
                    "width": "10%",
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var isactive = "No";
                        if (data.IsActive) {
                            isactive = "Yes";
                        }
                        return isactive
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
                        var edit = ' <a href="javascript:void(0)" title="Edit Team" data-action="edit" data-id="' + data.QuotationId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.QuotationId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
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

        $("#tableQuotations").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Quotation/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteQuotationUrl, { id: id }, "tableQuotations");

            }
        });

        loadQuotationList();
    }
}