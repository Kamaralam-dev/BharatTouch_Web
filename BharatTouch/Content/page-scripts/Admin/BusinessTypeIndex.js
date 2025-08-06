var listBusinessType_admin = function () {
    var me = this;
    this.getBusinessTypeUrl = null;
    this.deleteBusinessTypeUrl = null;
    this.addEditBusinessType = null;

    var loadBusinessType = function () {
        var table = $("#tableBusinessType_Admin").DataTable({
            "processing": true, // for show progress bar
            "serverSide": false, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": false,
            "scrollCollapse": false,
            "lengthMenu": [25, 50, 75, 100],
            "pageLength": 25,
            "autoWidth": false,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getBusinessTypeUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "BusinessTypeId", "name": "BusinessTypeId", "autoWidth": true, "className": "dt-body-left" },
                { "data": "ParentBusinessType", "name": "ParentBusinessType", "autoWidth": true, "className": "dt-body-left" },
                { "data": "BusinessType", "name": "BusinessType", "autoWidth": true, "className": "dt-body-left" },

                {
                    "data": null,
                    "name": "",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.BusinessTypeId + '" ><i style="color:green;" class="fa fa-pencil"></i></a> &nbsp;'
                        var DeleteButton = ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.BusinessTypeId + '" class="delete-row"><i style="color:red;" class="fa fa-trash"></i></a>'
                        return EditButtons + DeleteButton;
                    }
                },

            ],
            "initComplete": function (settings, json) {
                $('#ddlBusinessCategory').on('change', function () {
                    filterAndUpdateTable();
                });
            }
        });

        function filterData(apiData) {
            var selectedStatus = $('#ddlBusinessCategory').val();

            return apiData.filter(function (item) {

                var statusMatch = selectedStatus == '0' || item.ParentId == selectedStatus;

                return statusMatch;
            });
        }

        // Function to handle the filtering and reloading the table
        function filterAndUpdateTable() {
            var apiData = table.ajax.json().data; // Get the current data
            var filteredData = filterData(apiData); // Apply filters
            table.clear().rows.add(filteredData).draw(); // Reload table with filtered data
        }
    }

    $("#btnAddCategory").click(function (e) {
        openAddEditCategoryModal();
    });

    function openAddEditCategoryModal() {
        $.post("/Admin/AddEditBusinessType", { id: null, flag:1 }, (data) => {
           
            $("#ModaladdEditParentBusinessTypeCategory .modal-body").empty();
            $("#ModaladdEditParentBusinessTypeCategory .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditParentBusinessTypeCategory'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }

    $("#btnAddBusinessType").click(function (e) {

        var businessCategoryId = $("#ddlBusinessCategory").val();
        
        openAddEditBusinessTypeModal(null,businessCategoryId);
    });

    function openAddEditBusinessTypeModal(id, businessCategoryId) {

        $.post("/Admin/AddEditBusinessType", { id: id, flag: 0, businessCategoryId: businessCategoryId }, (data) => {
            
            $("#ModaladdEditBusinessType .modal-body").empty();
            $("#ModaladdEditBusinessType .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditBusinessType'), {
                keyboard: true
            });
            dynamicModal.show();
        }).fail((err) => {
            alert(err.responseText);
        });
    }

    this.init = function () {

        $("#tableBusinessType_Admin").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');

            switch (action) {
                case "edit":
                    openAddEditBusinessTypeModal(id);
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deleteBusinessTypeUrl, { id: id }, "tableBusinessType_Admin");
                    break;
            }
        });


        loadBusinessType();
    }
}