var homeItems = function () {

    var me = this;

    var loadRequestItemList = function () {
        $("#tableRequest").DataTable({
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
                { "data": "RequestId", "name": "RequestId", "autoWidth": true, "visible": false, "className": "dt-body-left" },
                { "data": "ItemNumber", "name": "ItemNumber", "autoWidth": true, "className": "dt-body-left" },
                { "data": "ItemName", "name": "ItemName", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "RequestDate",
                    "autoWidth": true,
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var RequestDate = '';
                        if (data.RequestDate != null)
                            RequestDate = formatJsonDate(data.RequestDate);
                        return RequestDate

                    }
                },
                { "data": "RequestNote", "name": "RequestNote", "autoWidth": true, "className": "dt-body-left" },
                { "data": "RequestBy", "name": "RequestBy", "autoWidth": true, "className": "dt-body-left" },
                { "data": "RequestType", "name": "RequestType", "autoWidth": true, "className": "dt-body-left" },
                { "data": "DonatedBy", "name": "DonatedBy", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "Name",
                    "orderable": true,
                    "className": "center",
                    "width": "10%",
                    "visible": true,
                    render: function (data, type, row) {
                        var view = ' <a href="/home/Comments/' + data.RequestId + '" title="comments" data-id="' + data.RequestId + '" ><i class="fa fa-comments font20"></i></a>';
                        return view;
                    }
                }
            ],
            "initComplete": function (settings, json) {
                $(this.api().table().header()).find('th').css({ 'padding': '6px 18px' });
            }
        });
    }

    var openRequestItemModal = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { itemId: id },
            url: '/home/RequestItem',
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                OpenModel("RequestItemModal");
                saveRequestItem();
            },
            error: handleAjaxError()
        });
    }

    var openPurchaseItemModal = function (id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { itemId: id },
            url: '/home/PurchaseItem',
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                OpenModel("PurchesItemModal");
                saveRequestItem();
            },
            error: handleAjaxError()
        });
    }

    var FilterItemList = function (catid,city,state) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { categoryId: catid, city: city, state: state },
            url: '/home/ItemList',
            dataType: 'html',
            success: function (data, strStatus) {
                //debugger;
                var asd = $("#mediaContainer").length;
                $("#mediaContainer").html(data);
            },
            error: handleAjaxError()
        });
    }


    var saveRequestItem = function () {
        $("#upsertRequestItemForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent().parent());
            },
            rules: {
                //RequestNote: { required: true }
                ClaimByName: { required: true },
                ClaimByPhone: { required: true, minlength: true },
                ClaimByEmail: { required: true, emailvalidatecustom: true }
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
                        if (data.Success)
                            CloseModal();
                    },
                    error: handleAjaxError()
                });
            }
        });

        $("#RequestItemModal")
            .modal({ show: true, backdrop: false })
            .on('click', '.mfp-close', function (e) {
                CloseModal();
            });
    }

    var saveComments = function (commentId, requestId) {
        var notes = CKEDITOR.instances.RequestComments.getData();
        if (notes != "") {
            var notesData = new FormData();
            notesData.append('commentId', commentId);
            notesData.append('requestId', requestId);
            notesData.append('comments', notes);
            $.ajax({
                url: '/HOme/SaveComments',
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: notesData,
                success: function (data) {
                    CKEDITOR.instances.RequestComments.setData('');
                    bindComment(requestId);
                    showMessage(data.Message, data.Data, data.Type);
                },
                error: handleAjaxError()
            });
        }
        else {
            showMessage("Failed", "Notes is required.", "error");
        }
    }

    //note is activity
    var bindComment = function (requestId) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { requestId: requestId },
            url: '/Home/BindComments',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#commentList").html(data);
            },
            error: handleAjaxError()
        });
    }

    var GetCityBySate = function(state) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { state: state },
            url: '/Home/GetCity',
            dataType: 'json',
            success: function (data, strStatus) {
                $("#City").empty();
                $("#City").append($("<option></option>").val('').html("Select City"));
                $.each(data, function (key, value) {
                    for (var i = 0; value.length > i; i++) {
                        $("#City").append($("<option></option>").val(value[i].City).html(value[i].City));
                    }
                });
            },
            error: handleAjaxError()
        });
    }


    this.init = function () {

        CKEDITOR.replace('RequestComments');
        CKEDITOR.config.entities = false;
        CKEDITOR.config.htmlEncodeOutput = false;

        $("#createFormContainer").on("click", "a", function () {

            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "request-item":
                    openRequestItemModal(id);
                    break;
                case "Purches-item":
                    openPurchaseItemModal(id);
                    break;
                case "savenotes":
                    saveComments(0, id);
                    break;
            }
        });

        $("#Category").change(function () {            
            FilterItemList($(this).val(), $("#City").val(), $("#State").val());
        });

        $("#City").change(function () {
            FilterItemList($("#Category").val(), $(this).val(), $("#State").val());
        });

        $("#State").change(function () {
            $("#City").empty();
            GetCityBySate($(this).val());
            FilterItemList($("#Category").val(), $("#City").val(), $(this).val());
        });


        GetCityBySate($('#State').val());
        loadRequestItemList();
    }
}