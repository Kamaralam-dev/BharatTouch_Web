var listFaq = function () {

    var me = this;
    this.getFaqUrl = null;
    this.deleteFaqUrl = null;
    this.upsertFaqUrl = null;
    this.pageSize = null;

    var loadFaqList = function () {
        $("#tableFaqs").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
           
            "lengthMenu": [10, 25, 50, 100],
            "pageLength": me.pageSize,
            "scrollCollapse": true,
            "autoWidth": true,
            "ajax": {
                "url": me.getFaqUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "FaqId", "name": "FaqId", "width": "10%", "visible": false },
                { "data": "Faq", "name": "Faq", "width": "80%", "className": "dt-body-left" },
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
                        var edit = ' <a href="javascript:void(0)" title="Edit Team" data-action="edit" data-id="' + data.FaqId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.FaqId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
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

        $("#tableFaqs").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Faq/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteFaqUrl, { id: id }, "tableFaqs");

            }
        });

        loadFaqList();
    }
}