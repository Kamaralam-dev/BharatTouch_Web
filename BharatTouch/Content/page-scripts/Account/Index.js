var listAccount = function () {
    
    var me = this;
    this.getAccountUrl = null;
    this.deleteAccountUrl = null;
    this.upsertAccountUrl = null;
    this.pageSize = null;

    var loadAccountList = function () {
        $("#tableAccounts").DataTable({
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
                "url": me.getAccountUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                {
                    "data": null,
                    "name": "Name",
                    "orderable": true,
                    "className": "left",
                    "autoWidth": true,
                    "visible": true,
                    render: function (data, type, row) {
                        return ' <a href="javascript:void(0)" title="Edit account" data-action="edit" data-id="' + data.AccountId + '" >' + data.Name + '</a>';                        
                    }
                },
                { "data": "Address1", "name": "Address1", "autoWidth": true, "visible": false },
                { "data": "City", "name": "City", "autoWidth": true, "className": "center", "visible": true },
                { "data": "State", "name": "State", "autoWidth": true, "className": "center", "visible": true },
                { "data": "CountryName", "name": "CountryName", "autoWidth": true, "className": "center", "visible": true },
                { "data": "Email", "name": "Email", "autoWidth": true, "className": "center", "visible": true },
                { "data": "Phone", "name": "Phone", "autoWidth": true, "visible": true },
                { "data": "Source", "name": "Source", "autoWidth": true, "visible": true },
                { "data": "Owner", "name": "Owner", "autoWidth": true, "visible": true },
                {
                    "data": null,
                    "name": "CreatedOn",
                    "autoWidth": true,
                    "visible": true,
                    render: function (data, type, row) {
                        var CreatedOn = '';
                        if (data.CreatedOn != null)
                            CreatedOn = formatJsonDate(data.CreatedOn);
                        return CreatedOn
                    }
                },
            ],
            "initComplete": function (settings, json) {               
                //if (accountShowColumnIndexes != "") {
                //    $("#tableAccounts").DataTable().columns([accountShowColumnIndexes]).visible(true);
                //}
                //if (accountHideColumnIndexes != "") {
                //    $("#tableAccounts").DataTable().columns([accountHideColumnIndexes]).visible(false);
                //}
            }
        });
    }

    //var accountDeleteNotification = function (id) {
    //    $.ajax({
    //        type: 'GET',
    //        cache: false,
    //        data: { id: id },
    //        url: '/Account/AccountDeleteNotification',
    //        dataType: 'html',
    //        success: function (data, strStatus) {
    //            $("body").append(data);
    //            OpenModel("AccountDeleteModal");
    //            $('#AccountDeleteContainer').on("click", "#IsAccountDeleteGranted", function () {
    //                $.ajax({
    //                    type: 'POST',
    //                    cache: false,
    //                    data: { id: id },
    //                    url: me.deleteAccountUrl,
    //                    dataType: 'json',
    //                    success: function (data, strStatus) {
    //                        showMessage(data.Message, data.Data, data.Type);
    //                        CloseModal();
    //                        $('#tableAccounts').DataTable().ajax.reload();
    //                    },
    //                    error: handleAjaxError()
    //                });
    //            });
    //        },
    //        error: handleAjaxError()
    //    });
    //}

    this.init = function () {

        $("#tableAccounts").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Account/Upsert/" + id;
                    break;
                //case "delete":
                //    accountDeleteNotification(id);
                //    break;
                case "redirect":
                    window.location.href = "/Contact?accountId=" + id;
                    break;
                case "invoice":
                    window.location.href = "/Invoice/Index?accountId=" + id + "&saleId=0&bkurl=account";
                    break;
            }
        });

        loadAccountList();
    }
}