var ListSetting = function () {
    var me = this;
    this.getSettingUrl = null;
    this.deleteSettingUrl = null;
    this.upsertSettingUrl = null;
    this.pageSize = null;

    var loadSettingList = function () {
       
        $("#tableSettings").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
            "scrollCollapse": true,
            "lengthMenu": [10, 25, 50, 100],           
            "autoWidth": true,
            "order": [[0, "asc"]],
            "ajax": {
                "url": me.getSettingUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "SettingId", "name": "SettingId", "autoWidth": true, "visible": false },
                { "data": "Name", "name": "Name", "autoWidth": true },
                { "data": "DisplayAs", "name": "DisplayAs", "autoWidth": true },
                {
                    "data": null,
                    "name": "",
                    "orderable": false,
                    "className": "center",
                    "autoWidth": true,
                    render: function (data, type, row) {
                        var actionButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.SettingId + '" ><i class="fa fa-pencil"></i></a> &nbsp;' +
                            ' <a href="javascript:void(0)" style="display:none;" data-action="delete" data-id="' + data.SettingId + '" class="delete-row"><i class="fa fa-trash-o"></i></a>'
                        return actionButtons;
                    }
                }
            ],
            "initComplete": function (settings, json) {
            }
        });
    }

    var openSettingDataModel = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: me.upsertSettingUrl,
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                OpenModel("SettingModal");
                saveSetting();
            },
            error: handleAjaxError()
        });
    }

    var saveSetting = function () {
        $("#upsertSettingForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                //Name: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: f.serializeArray(),
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        $('#tableSettings').DataTable().ajax.reload();
                        CloseModal();
                    },
                    error: handleAjaxError()
                });
            }
        });

        $("#SettingModal")
            .modal({ show: true, backdrop: false })
            .on('click', '.mfp-close', function (e) {
                CloseModal();
            });
    }

    $('#OpenSettingModal').click(function () {
        openSettingDataModel();
    });

    this.init = function () {
        $("#tableSettings").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    openSettingDataModel(id);
                    break;
                case "delete":
                    deleteRecord(me.deleteSettingUrl, { id: id }, "tableSetting");
                    break;
            }
        });
        loadSettingList();
    }
}