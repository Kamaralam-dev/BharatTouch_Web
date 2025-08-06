var listStory = function () {

    var me = this;
    this.getStoryUrl = null;
    this.deleteStoryUrl = null;
    this.upsertStoryUrl = null;
    this.pageSize = null;

    var loadStoryList = function () {
        $("#tableStorys").DataTable({
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
                "url": me.getStoryUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "Title", "name": "Title", "autoWidth": true, "width": "40%", "className": "dt-body-left"},
                { "data": "RelatedTo", "name": "RelatedTo", "autoWidth": true, "width": "30%", "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "StoryDate",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var StoryDate = '';
                        if (data.StoryDate != null)
                            StoryDate = formatJsonDate(data.StoryDate);
                        return StoryDate
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
                        var edit = ' <a href="javascript:void(0)" title="Edit Team" data-action="edit" data-id="' + data.StoryId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.StoryId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
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

        $("#tableStorys").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Story/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteStoryUrl, { id: id }, "tableStorys");

            }
        });

        loadStoryList();
    }
}