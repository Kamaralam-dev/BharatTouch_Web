var listTeam = function () {
    
    var me = this;
    this.getTeamUrl = null;
    this.deleteTeamUrl = null;
    this.upsertTeamUrl = null;
    this.pageSize = null;

    var loadTeamList = function () {
        $("#tableTeams").DataTable({
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
                "url": me.getTeamUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "TeamId", "name": "TeamId", "autoWidth": true, "visible": false },
                { "data": "MemberName", "name": "MemberName", "autoWidth": true},
                { "data": "Position", "name": "Position", "autoWidth": true},
                { "data": "Email", "name": "Email", "autoWidth": true },
                { "data": "Phone", "name": "Phone", "autoWidth": true },
                {
                    "data": null,
                    "name": "OrderNo",
                    "autoWidth": true,
                    "className": "left",
                    "width": "10%",
                    render: function (data, type, row) {                        
                        return "<input type='text' style='width:65px;' class='form-control SortOrder' value='" + data.OrderNo +"' data-id='" + data.TeamId + "' />";
                    }
                },
                {
                    "data": null,
                    "name": "IsActive",
                    "autoWidth": true,
                    "className": "left",
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
                    "name": "Name",
                    "orderable": true,
                    "className": "center",
                    "width": "10%",
                    "visible": true,
                    render: function (data, type, row) {
                        var edit = ' <a href="javascript:void(0)" title="Edit Team" data-action="edit" data-id="' + data.TeamId + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.TeamId + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
                        return edit + DeleteButton;
                    }
                }
            ],
             "initComplete": function (settings, json) {
                 $(this.api().table().header()).find('th').css({ 'padding': '6px 18px' });
                 $(".SortOrder").change(function () {
                    
                     var teamId = $(this).data('id');
                     var sortOrder = $(this).val();
                     updateTeamOrder(teamId, sortOrder);
                 });
            }  
        });
    }

   
    var updateTeamOrder = function (id,orderno) {
        $.ajax({
            type: 'POST',
            url: '/Team/UpdateSortOrder',
            data: { id: id, orderNo: orderno},
            dataType: 'json',
            success: function (data, strStatus) {                
                showMessage(data.Message, data.Data, data.Type);               
            }
        });
    }

    this.init = function () {

        

        $("#tableTeams").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Team/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteTeamUrl, { id: id }, "tableTeams");
               
            }
        });

        loadTeamList();
    }
}