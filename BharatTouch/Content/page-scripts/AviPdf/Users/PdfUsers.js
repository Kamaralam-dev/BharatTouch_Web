var listAviPdfUsers = function () {
    var me = this;
    this.getUserUrl = null;
    this.pageSize = null;
    this.weburl = null;

    var loadAviPdfUserList = function () {
        $("#tableAviPdfUsers").DataTable({
            "processing": true,
            "serverSide": true,
            "filter": true,
            "scrollX": "auto",
            "scrollCollapse": true,
            "lengthMenu": [10, 25, 50, 100],
            "pageLength": 10,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getUserUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "id", "autoWidth": true, "visible": false },
                { "data": "UserId", "name": "UserId", "autoWidth": true, "visible": false },
                { "data": "UserName", "name": "UserName", "autoWidth": true },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "DeviceID",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => data.DeviceID
                },{
                    "data": null,
                    "name": "DeviceType",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => data.DeviceType
                },
                {
                    "data": null,
                    "name": "Status",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => data.Status == true ? "Active" : "InActive"

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
            ],
        });
    }

    this.init = function () {
        loadAviPdfUserList();
    }
}