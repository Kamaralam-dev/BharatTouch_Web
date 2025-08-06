var listCountry_admin = function () {
    var me = this;
    this.getCountryUrl = null;
    this.deleteCountryUrl = null;
    this.addEditCountryUrl = null;

    var loadCountry = function () {

        $("#tableCountry_Admin").DataTable({
            "processing": true, // for show progress bar
            "serverSide": false, // for process server side
            "filter": true, // this is for disable filter (search box)
            //"scrollX": "auto",
            "scrollCollapse": false,
            "lengthMenu": [25, 50, 75, 100],
            "pageLength": 25,
            "autoWidth": false,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getCountryUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "CountryId", "name": "CountryId", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Country", "name": "Country", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Abbreviation", "name": "Abbreviation", "autoWidth": true, "className": "dt-body-left" },
                { "data": "CountryCode", "name": "CountryCode", "autoWidth": true, "className": "dt-body-left" },
                { "data": "NumberCode", "name": "NumberCode", "autoWidth": true, "className": "dt-body-left" },

                {
                    "data": null,
                    "name": "",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        var EditButtons = ' <a href="javascript:void(0)" data-action="edit" data-id="' + data.CountryId + '" ><i style="color:green;" class="fa fa-pencil"></i></a> &nbsp;'
                        var DeleteButton = ' <a href="javascript:void(0)" data-action="delete" data-id="' + data.CountryId + '" class="delete-row"><i style="color:red;" class="fa fa-trash"></i></a>'
                        return EditButtons + DeleteButton;
                    }
                },

            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    $("#btnAddCountry").click(function (e) {
        openAddEditCountryModal();
    });

    function openAddEditCountryModal(id) {

        $.post(me.addEditCountryUrl, { id: id }, (data) => {

            $("#ModaladdEditCountry .modal-body").empty();
            $("#ModaladdEditCountry .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditCountry'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    }


    this.init = function () {

        $("#tableCountry_Admin").on("click", "a", function () {
            var action = $(this).data("action");
            var id = $(this).data('id');

            switch (action) {
                case "edit":
                    openAddEditCountryModal(id);
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deleteCountryUrl, { id: id }, "tableCountry_Admin");
                    break;
            }
        });


        loadCountry();
    }
}