var listSponsor = function () {
    
    var me = this;
    this.getSponsorUrl = null;
    this.deleteSponsorUrl = null;
    this.upsertSponsorUrl = null;
    this.pageSize = null;

    var loadSponsorList = function () {
        $("#tableSponsors").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": "auto",
            "order": [[6, "asc"]],
            "lengthMenu": [10, 25, 50, 100],
            "pageLength": me.pageSize,
            "scrollCollapse": true,
            "autoWidth": true,
            "ajax": {
                "url": me.getSponsorUrl,
                "type": "POST",
                "datatype": "json",
                "error": function (xhr, error, code) {
                    console.log(xhr);
                    console.log(code);
                },
            },
            "columns": [
                { "data": "Id", "name": "Id", "autoWidth": true, "visible": false },
                { "data": "Name", "name": "Name", "autoWidth": true },
                { "data": "Email", "name": "Email", "autoWidth": true },
                { "data": "Phone", "name": "Phone", "autoWidth": true },
                { "data": "Company", "name": "Company", "autoWidth": true},
                { "data": "Designation", "name": "Designation", "autoWidth": true },                
                {
                    "data": null,
                    "name": "SortOrder",
                    "autoWidth": true,
                    "className": "left",
                    "width": "10%",
                    render: function (data, type, row) {                        
                        return "<input type='text' style='width:65px;' class='form-control SortOrder' value='" + data.SortOrder +"' data-id='" + data.Id + "' />";
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
                        var edit = ' <a href="javascript:void(0)" title="Edit Sponsor" data-action="edit" data-id="' + data.Id + '" ><i class="fa fa-pencil font20"></i></a>';
                        var DeleteButton = '&nbsp;&nbsp; <a href="javascript:void(0)" data-action="delete" data-id="' + data.Id + '" class="delete-row"><i class="fa fa-trash-o font20"></i></a>'
                        return edit + DeleteButton;
                    }
                }
            ],
             "initComplete": function (settings, json) {
                 $(this.api().table().header()).find('th').css({ 'padding': '6px 18px' });
                 $(".SortOrder").change(function () {
                    
                     var SponsorId = $(this).data('id');
                     var sortOrder = $(this).val();
                     updateSponsorOrder(SponsorId, sortOrder);
                 });
            }  
        });
    }

   
    var updateSponsorOrder = function (id,orderno) {
        $.ajax({
            type: 'POST',
            url: '/Sponsor/UpdateSortOrder',
            data: { id: id, orderNo: orderno},
            dataType: 'json',
            success: function (data, strStatus) {                
                showMessage(data.Message, data.Data, data.Type);               
            }
        });
    }

    this.init = function () {

        

        $("#tableSponsors").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "edit":
                    window.location.href = "/Sponsor/Upsert/" + id;
                    break;
                case "delete":
                    deleteRecord(me.deleteSponsorUrl, { id: id }, "tableSponsors");
               
            }
        });

        loadSponsorList();
    }
}