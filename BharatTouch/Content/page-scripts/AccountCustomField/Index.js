var ListCustomField = function () {
    var me = this;
    this.getCustomFieldUrl = null;
    this.deleteCustomFieldUrl = null;
    this.upsertCustomFieldUrl = null;
    this.AddCustomFieldUrl = null;

    var loadCustomFieldList = function () {
        var table = $("#tableCustomFields").DataTable({
            "rowReorder": true,
            "rowReorder": {
                "dataSrc": 'OrderIndex',
                "selector": 'tr'
            },
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
            "order": [[1, "desc"]],
            "scrollCollapse": true,
            "autoWidth": true,
            "paging": false,
            "ajax": {
                "url": me.getCustomFieldUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "OrderIndex", "name": "OrderIndex", "autoWidth": true, "visible": false },
                { "data": "AccountCustomFieldId", "name": "AccountCustomFieldId", "autoWidth": true, "visible": false },
                { "data": "DisplayName", "name": "DisplayName", "autoWidth": true },
                { "data": "Control", "name": "Control", "autoWidth": true, "className": "dt-body-left" },
                { "data": "DataType", "name": "DataType", "autoWidth": true },
                {
                    "data": null,
                    "name": "",
                    "orderable": false,
                    "className": "center",
                    "autoWidth": true,
                    render: function (data, type, row) {
                        var actionButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.AccountCustomFieldId + '" ><i class="fa fa-pencil"></i></a> &nbsp;' +
                            ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.AccountCustomFieldId + '" class="delete-row"><i class="fa fa-trash-o"></i></a>'
                        return actionButtons;
                    }
                }
            ]
        });

        $('#tableCustomFields').on('row-reorder.dt', function (dragEvent, data, nodes) {
            var arrSortOrder = [];
            for (var i = 0, ien = data.length; i < ien; i++) {
                var rowData = table.row(data[i].node).data();
                var objSortOrder = { AccountCustomFieldId: rowData["AccountCustomFieldId"], ToPosition: data[i].newData };
                arrSortOrder.push(objSortOrder);
            }

            var postData = JSON.stringify({ arrSortOrders: arrSortOrder });

            $.ajax({
                type: "POST",
                cache: false,
                contentType: "application/json; charset=utf-8",
                url: 'AccountCustomField/SetOrder',
                data: postData,
                dataType: "json",
                success: function (data, strStatus) {
                    if (data.Data != "0") {
                        $('#tableCustomFields').DataTable().ajax.reload();
                    }
                },
            });
        });
    }

    var openCustomDataModel = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { id: id },
            url: me.upsertCustomFieldUrl,
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                OpenModel("AddCustomFieldModal");
                saveCustomFields();
                onChangeOptions();
            },
            error: handleAjaxError()
        });
    }

    var saveCustomFields = function () {
        $("#upsertCustomFieldForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                DisplayName: { required: true },
                DataType: { required: true },
                Control: { required: true },
                ControlOption: { required: true }
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
                        $('#tableCustomFields').DataTable().ajax.reload();
                        CloseModal();
                    },
                    error: handleAjaxError()
                });
            }
        });


        $("#AddCustomFieldModal")
            .modal({ show: true, backdrop: false })
            .on('click', '.mfp-close', function (e) {
                CloseModal();
            });
    }

    var onChangeOptions = function () {
        /*show/hide options based on controls values on load */
        var ctl = $("#Control").val();
        if (ctl == "Select" || ctl == "MultiSelect" || ctl == "Radio" || ctl == "CheckBox") {
            $('.is-hide-option').removeClass('hidden');
        }
        else {
            $('.is-hide-option').addClass('hidden');
        }
        /*show hide options on change of controls types*/
        $('#Control').on('change', function () {
            var ctl = $(this).val();
            if (ctl == "Select" || ctl == "MultiSelect" || ctl == "Radio" || ctl == "CheckBox") {
                $('.is-hide-option').removeClass('hidden');
            }
            else {
                $('.is-hide-option').addClass('hidden');
            }
            $("#ControlOption").val("");
        });
    }

    $('#OpenCustomFieldModal').click(function () {
        $("#Control").val("");
        openCustomDataModel();
    });


    this.init = function () {
        $("#tableCustomFields").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    openCustomDataModel(id);
                    break;
                case "delete":
                    deleteRecord(me.deleteCustomFieldUrl, { id: id }, "tableCustomFields");
                    break;
            }
        });

        loadCustomFieldList();

    }
}