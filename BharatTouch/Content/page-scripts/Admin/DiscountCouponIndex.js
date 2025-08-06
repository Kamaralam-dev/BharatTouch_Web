var DiscountCouponJS = function () {
    var me = this;
    this.fetchDiscountCouponListUrl = null;
    this.deleteDiscountCouponUrl = null;
    this.OpenDiscountCouponFormModalUrl = null;

    function parseDateByFormat(dateStr, format) {
        try {
            var parts;
            switch (format) {
                case 'MM/YYYY':
                    parts = dateStr.split('/');
                    return new Date(parseInt(parts[1]), parseInt(parts[0]) - 1, 1);
                case 'YYYY-MM':
                    parts = dateStr.split('-');
                    return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, 1);
                case 'DD-MM-YYYY':
                    parts = dateStr.split('-');
                    return new Date(parseInt(parts[2]), parseInt(parts[1]) - 1, parseInt(parts[0]));
                case 'YYYY/MM/DD':
                    parts = dateStr.split('/');
                    return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
                case 'MM/dd/yyyy':
                    parts = dateStr.split('/');
                    return new Date(parseInt(parts[2]), parseInt(parts[0]) - 1, parseInt(parts[1]));
                case 'YYYY':
                    return new Date(parseInt(dateStr), 0, 1); // default to Jan 1st of the year
                default:
                    // fallback to Date constructor
                    var fallbackDate = new Date(dateStr);
                    return isNaN(fallbackDate) ? null : fallbackDate;
            }
        } catch (e) {
            return null;
        }
    }


    var loadDiscountCouponList = function () {
        $("#tableDiscountCoupons").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "scrollX": false,
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.fetchDiscountCouponListUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "DiscountCouponId", "name": "DiscountCouponId", "autoWidth": true, "visible": true },
                {
                    "data": null,
                    "name": "CouponName",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        return data.CouponName;
                    }
                },
                { "data": "Code", "name": "Code", "autoWidth": true, "className": "dt-body-left" },
                { "data": "StartDate", "name": "StartDate", "autoWidth": true, "className": "dt-body-left", render: (jsonDate) => formatJsonDate(jsonDate) },
                { "data": "EndDate", "name": "EndDate", "autoWidth": true, "className": "dt-body-left", render: (jsonDate) => formatJsonDate(jsonDate) },
                { "data": "PercentageOff", "name": "PercentageOff", "autoWidth": true, "className": "dt-body-left" },
                { "data": "AmountOff", "name": "AmountOff", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "IsActive",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        return data.IsActive ? "Active" : "InActive";
                        //if (data.IsActive) {
                        //    return ' <input type="checkbox" class="chkUpdateStatus" style="cursor:pointer;text-decoration:underline;" checked data-id="' + data.DiscountCouponId + '"  />'
                        //}
                        //return ' <input type="checkbox" class="chkUpdateStatus"  style="cursor:pointer;text-decoration:underline;" data-id="' + data.DiscountCouponId + '" />'

                    }
                },
                { "data": "CreatedByName", "name": "CreatedByName", "autoWidth": true, "className": "dt-body-left" },
                { "data": null, "name": "", "autoWidth": true, "className": "dt-body-left", render: (data) => formatJsonDate(data.CreatedOn) },
                {
                    "data": null,
                    "name": "",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        var EditButtons = ' <a  data-bs-toggle="tooltip" data-bs-placement="top" title="Edit" data-action="edit" data-id="' + data.DiscountCouponId + '" style="cursor: pointer;" ><i style="color:blue;" class="fa fa-pencil"></i></a> &nbsp;'
                        var DeleteButtons = ' <a  data-bs-toggle="tooltip" data-bs-placement="top" title="Delete" data-action="delete" data-id="' + data.DiscountCouponId + '" style="cursor: pointer;" ><i style="color:red;" class="fa fa-trash"></i></a>'
                        return EditButtons + DeleteButtons;
                    }
                },


            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    this.SubmitDiscountCouponForm = function () {
        $("#discountCouponForm").submit();
    }

    UpsertDiscountCoupon = function () {

        $.validator.addMethod("eitherPercentageOrAmount", function (value, element) {
            var percentage = $('#PercentageOff').val();
            var amount = $('#AmountOff').val();

            return (percentage !== "" && percentage != 0) || (amount !== "" && amount != 0);
        }, "Either Percentage Off or Amount Off must be filled.");

        $.validator.addMethod("greaterThanStartDate", function (value, element, param) {
            debugger
            var startDate = $(param.elem).val();
            var format = param.format || 'MM/dd/yyyy';
            if (!startDate || !value) {
                return false;
            }
            startDate = parseDateByFormat(startDate, format);
            var endDate = parseDateByFormat(value, format);
            if (!startDate || !endDate) {
                return false;
            }
            return endDate >= startDate;
            //return new Date(value) >= new Date(startDate);
        }, "End Date cannot be earlier than Start Date");

        $("#discountCouponForm").validate({
            rules: {
                CouponName: { required: true },
                Code: { required: true },
                StartDate: { required: true },
                EndDate: {
                    required: true,
                    greaterThanStartDate: {
                        elem: "#dpStartDate",
                        format: "MM/dd/yyyy"
                    }
                },
                PercentageOff: {
                    number: true,
                    eitherPercentageOrAmount: true
                },
                AmountOff: {
                    number: true,
                    eitherPercentageOrAmount: true
                }
            },
            submitHandler: function (form) {

                var f = $(form);
                var data = f.serializeArray();

                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {

                        showMessage((data.Success ? "Success" : "Failed"), data.Data, data.Type);
                        setTimeout(function () {

                            if (data.Success) {
                                $('#ModaladdEditDiscountCoupon').modal('hide');
                                $("#tableDiscountCoupons").DataTable().ajax.reload();
                            }
                        }, 500)
                    },
                    error: handleAjaxError()
                });
            }
        });
    }

    $("#btnAddDiscountCoupon").click(function (e) {

        openAddEditDiscountCouponModal(null);
    });

    function openAddEditDiscountCouponModal(id) {

        $.post(me.OpenDiscountCouponFormModalUrl, { id: id }, (data) => {

            $("#ModaladdEditDiscountCoupon .modal-body").empty();
            $("#ModaladdEditDiscountCoupon .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditDiscountCoupon'), {
                keyboard: true
            });
            dynamicModal.show();
        }).fail((err) => {
            alert(err.responseText);
        });
    }

    function InitializeCommonDates(startDateSelector, EndDateSelector, format, startView, minViewMode) {
        if (!$(startDateSelector).val().trim()) {
            $(EndDateSelector).val("").prop("disabled", true);
        } else {
            $(EndDateSelector).prop("disabled", false);
        }

        var options = { autoclose: true };
        if (format) {
            options.format = format;
        }
        if (startView) {
            options.startView = startView;
        }
        if (minViewMode) {
            options.minViewMode = minViewMode;
        }

        $(startDateSelector).datepicker(options).on('changeDate', function (selected) {
            let startDate = selected.date;
            if (startDate) {
                $(EndDateSelector).prop("disabled", false);
            } else {
                $(EndDateSelector).prop("disabled", true);
            }
        });

        $(startDateSelector).on('input', function () {
            if (!this.value) { // If input is empty
                $(EndDateSelector).val("").prop("disabled", true);
            }
        });

        $(EndDateSelector).datepicker(options);

    }

    $("#ModaladdEditDiscountCoupon").on('shown.bs.modal', function () {
        InitializeCommonDates("#dpStartDate", "#dpEndDate");
        UpsertDiscountCoupon();
    });

    this.init = function () {

        $("#tableDiscountCoupons").on("click", "a", function () {
            debugger
            var action = $(this).data("action");
            var id = $(this).data('id');
            switch (action) {
                case "delete":
                    deleteRecord(me.deleteDiscountCouponUrl, { id: id }, "tableDiscountCoupons");
                    break;
                case "edit":
                    openAddEditDiscountCouponModal(id);
                    break;
            }
        });

        loadDiscountCouponList();
    }
}