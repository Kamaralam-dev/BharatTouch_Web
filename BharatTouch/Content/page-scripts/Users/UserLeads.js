var UserLeads = function () {
    var BindLeadList = function () {
        var userId = $("#hdnUserID").val();
        $("#tableLeads").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 75, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": `/Users/BindLeadList?UserId=${userId}`,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "LeadId", "name": "LeadId", "autoWidth": true, "visible": false },
                { "data": "UserId", "name": "UserId", "autoWidth": true, "visible": false },
                {
                    "data": null,
                    "name": "Name",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => data.Name
                },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "PhoneNo",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => data.PhoneNo
                },
                {
                    "data": null,
                    "name": "LeadType",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => data.LeadType
                },
                {
                    "data": null,
                    "name": "Date",
                    "autoWidth": true,
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var meetingDate = '';
                        if (data.Date != null)
                            meetingDate = formatJsonDate(data.Date);
                        return meetingDate
                    }
                },
                {
                    "data": null,
                    "name": "CreatedOn",
                    "autoWidth": true,
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
                    "name": "Action",
                    "orderable": false,
                    "searchable": false,
                    "autoWidth": true,
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        return `<a href="javascript:void(0)" class="btn m-2 btn-sm btn-outline-danger btnDeleteLead" data-id="${data.LeadId}"><i class="icofont-close"></i></a>`
                    }
                }
            ],
        });

    }

    function deleteLead(id) {
        $.post("/Users/DeleteLeadById", { LeadId: id }, function (data) {
            if (data.Success) {
                $("#tableLeads").DataTable().ajax.reload();
                $("#confirmationModel").modal("hide");
            }
        });
    }

    function removeAllModelDataKeys() {
        $.each($('#confirmationModelData').data(), function (key) {
            $('#confirmationModelData').removeData(key);
        });
    }

    function setAndShowConfirmationModel(obj, title, message) {
        removeAllModelDataKeys();
        $("#confirmationModelLabel").val(title ? title : "Confirm");
        $("#confirmationModelData").data(obj);
        $("#confirmationModel").modal("show");
    }

    this.init = function () {
        BindLeadList();

        $(document).on('click', '.btnDeleteLead', function () {
            var obj = $(this);
            var id = obj.data("id");
            setAndShowConfirmationModel({
                id: id,
                callback: function () {
                    deleteLead(id);
                }
            });

        });

        $('#btnconfirmationModel').on('click', function () {
            var callback = $('#confirmationModelData').data('callback');
            var data = $('#confirmationModelData').data();
            if (typeof callback === 'function') {
                callback(data);
            }
        });
    }
}