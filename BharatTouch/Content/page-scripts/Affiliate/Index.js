var listAffiliate = function () {

    var me = this;
    this.getAffiliateUrl = null;
    this.deleteAffiliateUrl = null;
    this.upsertAffiliateUrl = null;
    this.pageSize = null;

    var loadAffiliateList = function () {
        $("#tableAffiliates").DataTable({
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
                "url": me.getAffiliateUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "Company", "name": "Company", "autoWidth": true, "width": "45%", "className": "dt-body-left" }, 
                { "data": "Email", "name": "Email", "autoWidth": true, "width": "15%", "className": "dt-body-left"},
                { "data": "Phone", "name": "Phone", "autoWidth": true, "width": "10%", "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "IsActive",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    "width": "10%",
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
                {
                    "data": null,
                    "name": "Name",
                    "orderable": true,
                    "className": "center",
                    "width": "10%",
                    "visible": true,
                    render: function (data, type, row) {
                        var edit = ' <a href="javascript:void(0)" title="Edit Team" data-action="edit" data-id="' + data.AffiliateId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.AffiliateId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
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

        $("#tableAffiliates").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Affiliate/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteAffiliateUrl, { id: id }, "tableAffiliates");

            }
        });

        loadAffiliateList();
    }
}