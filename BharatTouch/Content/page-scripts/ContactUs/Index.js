var listContactUs = function () {

    var me = this;
    this.getContactUsUrl = null;
    this.deleteContactUsUrl = null;
    this.upsertContactUsUrl = null;
    this.pageSize = null;

    var loadContactUsList = function () {
        $("#tableContactUs").DataTable({
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
                "url": me.getContactUsUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "CompanyName", "name": "CompanyName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "ContactName", "name": "ContactName", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Title", "name": "Title", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Phone", "name": "Phone", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "QueryDate",
                    "width": "15%",
                    "className": "dt-body-left",
                    "visible": true,
                    render: function (data, type, row) {
                        var QueryDate = '';
                        if (data.QueryDate != null)
                            QueryDate = formatJsonDate(data.QueryDate);
                        return QueryDate
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
                        var edit = ' <a href="javascript:void(0)" title="Edit Team" data-action="edit" data-id="' + data.ContactUsId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.ContactUsId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
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

        $("#tableContactUs").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/ContactUs/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteContactUsUrl, { id: id }, "tableContactUs");

            }
        });

        loadContactUsList();
    }
}