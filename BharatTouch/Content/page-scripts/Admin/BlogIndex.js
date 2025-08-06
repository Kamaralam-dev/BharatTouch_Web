var listBlog_admin = function () {
    var me = this;
    this.getAllBlogUrl = null;
    this.deleteBlogUrl = null;
    this.addEditBlogUrl = null;

    var loadBlog = function () {

        $("#tableBlog_Admin").DataTable({
            "processing": false, // for show progress bar
            "serverSide": false, // for process server side
            "filter": true, // this is for disable filter (search box)
            //"scrollX": "auto",
            "scrollCollapse": false,
            "lengthMenu": [25, 50, 75, 100],
            "pageLength": 25,
            "autoWidth": false,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getAllBlogUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "BlogId", "name": "BlogId", "autoWidth": true, "className": "dt-body-left" },
                { "data": "BlogTitle", "name": "BlogTitle", "autoWidth": true, "className": "dt-body-left" },
               /* { "data": "BlogDescription", "name": "BlogDescription", "autoWidth": true, "className": "dt-body-left" },*/
                { "data": "BlogKeywords", "name": "BlogKeywords", "autoWidth": true, "className": "dt-body-left" },
                { "data": "BlogTagLine", "name": "BlogTagLine", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": "IsActive", "name": "IsActive", "autoWidth": true, "className": "dt-body-left",
                    render: function (data, type, row) {
                        return row.IsActive == true ? "Yes" : "No";
                    }
                },
                {
                    "data": null,
                    "name": "",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.BlogId + '" ><i style="color:green;" class="fa fa-pencil"></i></a> &nbsp;'
                        var DeleteButton = ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.BlogId + '" class="delete-row"><i style="color:red;" class="fa fa-trash"></i></a>'
                        return EditButtons + DeleteButton;
                    }
                },
            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    $("#btnAddBlog").click(function (e) {
        openAddEditBlogModal();
    });

    function openAddEditBlogModal(id) {

        $.post(me.addEditBlogUrl, { id: id }, (data) => {

            $("#ModaladdEditBlog .modal-body").empty();
            $("#ModaladdEditBlog .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditBlog'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    this.init = function () {

        $("#tableBlog_Admin").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');

            switch (action) {
                case "edit":
                    openAddEditBlogModal(id);
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deleteBlogUrl, { id: id }, "tableBlog_Admin");
                    break;
            }
        });


        loadBlog();
    }
}