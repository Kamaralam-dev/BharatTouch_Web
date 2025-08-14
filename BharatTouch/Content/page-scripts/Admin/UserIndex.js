var listUser_admin = function () {
    var me = this;
    this.getUserUrl = null;
    this.deleteUserUrl = null;
    this.getAllUserHistory = null;
    this.getUserReferredByCode = null;
    this.updateUserStatus = null;
    this.packageModal = null;
    this.updatePackage = null;
    this.getPackageHistoryByUserId = null;
    this.weburl = null;
    this.paymentDetailsModalUrl = null;
    this.orderDetailsModalUrl = null;
    this.GenerateInvoiceUrl = null;
    this.SendInvoiceEmailToUser = null;
    this.SaveInvoicePdf = null;
    this.addEditUserByAdmin = null;

    var loadUserList = function () {
        $("#tableUser_Admin").DataTable({
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
                "url": me.getUserUrl,
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "UserId", "name": "UserId", "autoWidth": true, "visible": true },
                {
                    "data": null,
                    "name": "FirstName",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var lastName = row.LastName == null ? "" : row.LastName;
                        return data.FirstName + " " + lastName;
                    }
                },
                 { "data": "EmailId", "name": "EmailId", "autoWidth": true, "className": "dt-body-left" },
                { "data": "Password", "name": "Password", "autoWidth": true, width: "10%", "className": "dt-body-left", render: (data) => "<a class='text-primary' style='cursor:pointer;' data-action='password' data-type='password' data-password='" + data + "'>See password</a>" },
                {
                    "data": null,
                    "name": "Phone",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => `${(data.NumberCode != null) ? data.NumberCode : ""} ${(data.Phone != null) ? data.Phone : ""}`
                },

                {
                    "data": null,
                    "name": "PageViewed",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var PageViewed = 0;
                        if (data.PageViewed != null)
                            PageViewed = data.PageViewed;

                        return ' <a href="javascript:void(0)" style="cursor:pointer;text-decoration:underline;" data-action="pageviewed" data-id="' + data.UserId + '" >' + PageViewed + '</a> &nbsp;'

                    }
                },
                {
                    "data": null,
                    "name": "ReferredUserCount",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        return ' <a href="javascript:void(0)" style="cursor:pointer;text-decoration:underline;" data-action="referal" data-referalcode="' + data.ReferalCode + '" >' + data.ReferredUserCount + '</a> &nbsp;'

                    }
                },
                { "data": "ReferredByUserName", "name": "ReferredByUserName", "autoWidth": true, "sorting": false, "className": "dt-body-left" },
                { "data": "City", "name": "City", "autoWidth": true, "sorting": false, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "OrderNo",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        if (data.OrderNo == null || data.OrderNo == "")
                            return "";

                        return ' <a href="javascript:void(0)" style="cursor:pointer;text-decoration:underline;" data-action="order-details" data-order-no="' + data.OrderNo + '" >' + data.OrderNo + '</a> &nbsp;'

                    }
                },
                {
                    "data": null,
                    "name": "IsActive",
                    "width": "10%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        if (data.IsActive) {
                            return ' <input type="checkbox" class="chkUpdateStatus" style="cursor:pointer;text-decoration:underline;" checked data-id="' + data.UserId + '"  />'
                        }
                        return ' <input type="checkbox" class="chkUpdateStatus"  style="cursor:pointer;text-decoration:underline;" data-id="' + data.UserId + '" />'

                    }
                },
                {
                    "data": null,
                    "name": "",
                    "width": "5%",
                    "visible": true,
                    "className": "dt-body-left",
                    "sorting": false,
                    render: function (data, type, row) {
                        var codeOrName = data.UrlCode;

                        if (data.Displayname && data.Displayname.trim() !== "") {
                            codeOrName = data.Displayname;
                        }


                        var DetailButtons = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="view" data-id="' + data.Displayname + '" ><i style="color:blue;" class="fa fa-search"></i>&nbsp;View User</a></li>'
                        var EditButtons = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="edit" data-codeorname="' + codeOrName + '" ><i style="color:green;" class="fa fa-pencil"></i>&nbsp;Edit User</a> </li>'
                        var DeleteButton = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="delete" data-id="' + data.UserId + '" ><i style="color:red;" class="fa fa-trash"></i>&nbsp;Delete User</a></li>'
                        //var UpdatePackageButton = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="package" data-id="' + data.UserId + '" data-package-id="' + data.PackageId + '" ><i style="color:#fc7c3f;" class="fa fa-exchange-alt"></i>&nbsp;Change Plan</a> </li>'
                       // var PackageHistoryButton = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="packageHistory" data-id="' + data.UserId + '" ><i style="color:black;" class="fa fa-history"></i>&nbsp;Plan Change History</a> </li>'
                        var PaymentDetailsButton = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="payment-details" data-id="' + data.UserId + '" ><i style="color:red;" class="fa fa-dollar"></i>&nbsp;Payment Details</a></li>'
                        var CardPrintDetailsButton = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="card-print-details" data-id="' + data.OrderId + '" ><i style="color:blue;" class="fa fa-credit-card"></i>&nbsp;Card Print Details</a></li>'
                        var openLatestInvoiceButton = ' <li><a href="javascript:void(0)" class="dropdown-item" data-action="latest-invoice" data-id="' + data.LatestInvoiceId + '" ><i style="color:green;" class="fa fa-receipt"></i>&nbsp;Latest Order Invoice</a></li>'

                        var actionBtns = DetailButtons + EditButtons + DeleteButton  + PaymentDetailsButton + CardPrintDetailsButton;
                        if (data.LatestInvoiceId != null && data.LatestInvoiceId != 0) {
                            actionBtns += openLatestInvoiceButton;
                        }

                        var actions = `
                                <div class="filter">
                                      <a class="icon" href="#" data-bs-toggle="dropdown" aria-expanded="false"><i class="bi bi-three-dots"></i></a>
                                      <ul class="dropdown-menu dropdown-menu-end dropdown-menu-arrow">
                                        ${actionBtns}
                                      </ul>
                                </div>
                            `;
                        return actions;
                    }
                },
                

            ],
            "initComplete": function (settings, json) {
               
            }
        });
    }

    function pageViewedModal(id) {

        $('#pageModal').modal('show'); // Use Bootstrap's modal show method

        $("#tableUserHistory_Admin").DataTable({
            destroy: true,
            "processing": true,
            "serverSide": false,
            "filter": true,
            "scrollX": false,
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getAllUserHistory, 
                "type": "POST",
                "datatype": "json",
                "data": { userId: id } 
            },
            "columns": [
                {
                    "data": "UserName", 
                    "name": "UserName", 
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": "IPAddress", 
                    "name": "IPAddress",
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": "BrowserName", 
                    "name": "BrowserName",
                    "autoWidth": true,
                    "className": "dt-body-left",

                },
                {
                    "data": "BrowserVersion", 
                    "name": "BrowserVersion",
                    "autoWidth": true,
                   
                    "className": "dt-body-left",

                },
                {
                    "data": "PlateForm", 
                    "name": "PlateForm",
                    "width": "10%",
                    "autoWidth": true,
                    "className": "dt-body-left",

                },
                {
                    "data": "ViewDate",
                    "name": "ViewDate",
                    "width": "10%",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    "render": function (data, type, row) {
                        // Extract timestamp from the string /Date(1740803734043)/
                        var timestamp = data.match(/\/Date\((\d+)\)\//);

                        if (timestamp && timestamp[1]) {
                            // Convert to a JavaScript Date object
                            var date = new Date(parseInt(timestamp[1]));
                            // Format the date to a readable string
                            return date.toLocaleString();
                        }
                        return data; // Return original data if no valid timestamp found
                    }
                }


            ],
            "initComplete": function (settings, json) {

            }
        });
    }
    
    function PackageHistoryModal(id) {
        $('#userPackageHistoryModal').modal('show'); // Use Bootstrap's modal show method
        $("#tableUsersPackageHistory_Admin").DataTable({
            destroy: true,
            "processing": true,
            "serverSide": true,
            "filter": true,
            //"scrollX": false,
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getPackageHistoryByUserId, 
                "type": "POST",
                "datatype": "json",
                "data": {UserId: id}                
                //"data": function (d) {
                //    d.UserId = id
                //}
            },
            "columns": [
                {
                    "data": "ChangeHistoryId", 
                    "name": "ChangeHistoryId", 
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": "PackageName", 
                    "name": "PackageName", 
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": "PackageCost", 
                    "name": "PackageCost", 
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": "OldPackageName", 
                    "name": "OldPackageName",
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": "OldPackageCost", 
                    "name": "OldPackageCost",
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": "FullName", 
                    "name": "FullName",
                    "autoWidth": true,
                    "className": "dt-body-left",

                },
                {
                    "data": "ChangedByFullName", 
                    "name": "ChangedByFullName",
                    "autoWidth": true,
                   
                    "className": "dt-body-left",

                },
                {
                    "data": "IsAdminChangedBy", 
                    "name": "IsAdminChangedBy",
                    "width": "10%",
                    "autoWidth": true,
                    "className": "dt-body-left",

                },
                {
                    "data": "CreatedOn",
                    "name": "CreatedOn",
                    "width": "10%",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    "render": function (data, type, row) {
                        return formatJsonDate(data);
                    }
                }


            ],
            "initComplete": function (settings, json) {

            }
        });
    }

    function redirectToSite(id) {
        window.open(me.weburl + id, "_blank");
    }

    function userReferredByCodeModal(referalcode) {

        $('#userReferalCodeModal').modal('show'); 

        $("#tableUserRefferedBy_Admin").DataTable({
             destroy: true, 
            "processing": true, 
            "serverSide": false, 
            "filter": true, 
            "scrollX": false,
            "scrollCollapse": true,
            "lengthMenu": [25, 50, 100],
            "pageLength": 25,
            "autoWidth": true,
            "order": [[0, "desc"]],
            "ajax": {
                "url": me.getUserReferredByCode,
                "type": "POST",
                "datatype": "json",
                "data": { referalCode: referalcode }
            },
            "columns": [
                {
                    "data": "UserId",
                    "name": "UserId",
                    "autoWidth": true,
                    "className": "dt-body-left",
                },
                {
                    "data": null,
                    "name": "FirstName",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: function (data, type, row) {
                        var lastName = row.LastName == null ? "" : row.LastName;
                        return data.FirstName + " " + lastName;
                    }
                },
                { "data": "EmailId", "name": "EmailId", "autoWidth": true, "className": "dt-body-left" },
                {
                    "data": null,
                    "name": "Phone",
                    "autoWidth": true,
                    "className": "dt-body-left",
                    render: (data) => `${(data.NumberCode != null) ? data.NumberCode : ""} ${(data.Phone != null) ? data.Phone : ""}`
                },

            ],
            "initComplete": function (settings, json) {

                
            }
        });
    }

    function ReloadDataTable(tableId) {
        $('#' + tableId).DataTable().ajax.reload(null, false);
    }

    function openPackagemodel(userId, packageId) {
        $("#hdnSelectedUserId").val(userId);
        $.ajax({
            type: 'GET',
            cache: false,
            data: { UserId: userId, PackageId: packageId },
            url: me.packageModal,
            dataType: 'html',
            success: function (data, strStatus) {
                $("#updatePackageFormContainer").html('');
                $("#updatePackageFormContainer").append(data);
                $("#userPackageModal").modal("show");
            },
            error: function () {
                console.error("Error while opening plan modal");
            }
        });
    }

    function paymentDetailsModal(id) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { UserId: id },
            url: me.paymentDetailsModalUrl,
            dataType: 'html',
            success: function (data, strStatus) {
                debugger
                $("#paymentDetailsContainer").html('');
                $("#paymentDetailsContainer").append(data);
                $("#paymentDetailsModal").modal("show");
            },
            error: function () {
                console.error("Error while opening payment details modal");
            }
        });
    }
    
    function orderDetailsModal(orderNo) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { orderNo: orderNo },
            url: me.orderDetailsModalUrl,
            dataType: 'html',
            success: function (data, strStatus) {
                debugger
                $("#orderDetailsContainer").html('');
                $("#orderDetailsContainer").append(data);
                $("#orderDetailsModal").modal("show");
            },
            error: function () {
                console.error("Error while opening order details modal");
            }
        });
    }

    function showHidePassword(obj) {
        const type = obj.data('type');
        const password = obj.data('password');

        if (type === 'password') {
            obj.html(password);
            obj.data('type', 'text');
        } else {
            obj.html('See password');
            obj.data('type', 'password');
        }
    }

    function convertAndDownloadHtmlToPdf(containerEle, fileName, callback) {
        debugger
        html2pdf().from(containerEle).set({
            margin: 0,
            filename: fileName,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: {
                scale: 2,
                useCORS: true, // 👈 This is important
                allowTaint: true
            },
            jsPDF: { unit: 'in', format: 'a4', orientation: 'portrait' }
        }).save().then((res) => {
            if (callback) {
                callback(res);
            }
        }).catch(err => {
            if (callback) {
                callback(err);
            }
        });
    }

    function printPDF(pdfUrl, callback) {
        // Create an iframe
        var iframe = document.createElement('iframe');
        iframe.style.display = 'none'; // Hide the iframe
        iframe.src = pdfUrl;   //Set source to PDF path or URL

        document.body.appendChild(iframe);


        // Wait for the iframe to load before printing
        iframe.onload = function () {
            try {
                iframe.contentWindow.print();
            } catch (e) {
                console.error("Error printing PDF:", e);
            } finally {
                if (callback) {
                    callback();
                }
            }
        };
    }

    function convertAndSaveHtmlAsPdf(containerEle, callback, errCallback) {
        html2pdf().from(containerEle).set({
            margin: 0,
            filename: 'my_document.pdf',
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: {
                scale: 2,
                useCORS: true, // 👈 This is important
                allowTaint: true
            },
            jsPDF: {
                unit: 'mm',
                width: 594,   // Double the default width of A4 in mm
                height: 841,
                //format: 'a4',
                orientation: 'portrait'
            }
        }).outputPdf('blob')
            .then(function (pdfBlob) {
                debugger
                // Convert Blob to File:
                const pdfFile = new File([pdfBlob], 'my_document.pdf', { type: 'application/pdf' });
                if (callback) {
                    callback(pdfFile);
                }
            }).catch(() => {
                if (errCallback) {
                    errCallback();
                }
            });
    }
    function openInvoice(id) {
        var loader = $("#Progressbar");
        loader.show();
        $.post(me.GenerateInvoiceUrl, { InvoiceId: id }, function (res) {
            if (!res.Success) {
                showMessage(res.Message, res.Data, res.Type);
                loader.hide();
                return;
            }
            loader.hide();
            var htmlContentEle = document.getElementById('invoicePdfHtmlContent');
            $("#invoicePdfHtmlContent").empty();
            $("#invoicePdfHtmlContent").html(res.Data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModalInvoicePreview'), {
                keyboard: true
            });
            dynamicModal.show();
            $("#ModalInvoicePreview").on("shown.bs.modal", function () {
                $("#ModalInvoicePreview").on('click', "#btnPdfDownload", function () {
                    loader.show();
                    convertAndDownloadHtmlToPdf(htmlContentEle, `invoice_${res.OptionalValue}.pdf`, () => loader.hide());
                });
                $("#ModalInvoicePreview").on('click', "#btnPdfPrint", function () {
                    loader.show();
                    convertAndSaveHtmlAsPdf(
                        htmlContentEle,
                        function (file) {
                            var formdata = new FormData();
                            formdata.append("InvoiceId", id);
                            formdata.append("pdfFile", file);
                            $.ajax({
                                type: "POST",
                                url: me.SaveInvoicePdf,
                                data: formdata,
                                dataType: 'json',
                                contentType: false,
                                processData: false,
                                success: function (data, strStatus) {
                                    if (!data.Success) {
                                        showMessage(data.Message, data.Data, data.Type);
                                        loader.hide();
                                    }
                                    printPDF(data.Data, () => loader.hide());
                                },
                                error: handleAjaxError(function () {
                                    loader.hide();
                                })
                            });
                        },
                        () => loader.hide()
                    )
                });
                $("#ModalInvoicePreview").on('click', "#btnSendPdfMail", function () {
                    loader.show();
                    convertAndSaveHtmlAsPdf(
                        htmlContentEle,
                        function (file) {
                            debugger
                            var formdata = new FormData();
                            formdata.append("InvoiceId", id);
                            formdata.append("pdfFile", file);
                            $.ajax({
                                type: "POST",
                                url: me.SendInvoiceEmailToUser,
                                data: formdata,
                                dataType: 'json',
                                contentType: false,
                                processData: false,
                                success: function (data, strStatus) {
                                    debugger
                                    showMessage(data.Message, data.Data, data.Type);
                                    loader.hide();
                                },
                                error: handleAjaxError(function () {
                                    loader.hide();
                                })
                            });
                        },
                        () => loader.hide()
                    )
                });
            });
            //openPDFViewerModel(res.Data, "Invoice");
        }).fail(handleAjaxError(() => {
            loader.hide();
        }))
    }

    $("#btnAddNewUserByAdmin").on("click", function () {
        $.post(me.addEditUserByAdmin, { id: 0 }, (data) => {

            $("#ModaladdEditUserByAdmin .modal-body").empty();
            $("#ModaladdEditUserByAdmin .modal-body").html(data);
            var dynamicModal = new bootstrap.Modal(document.getElementById('ModaladdEditUserByAdmin'), {
                keyboard: true
            });
            dynamicModal.show();

        }).fail((err) => {
            alert(err.responseText);
        });
    });


    this.init = function () {

        $(document).on('change', ".chkUpdateStatus", function () {
            var obj = $(this);
            var id = obj.data('id');
            var isChecked = obj.is(":checked");
            $.post(
                me.updateUserStatus,
                { UserId: id, IsActive: isChecked },
                function (data) {
                    showMessage(data.Message, data.Data, data.Type);
                    if (data.Success) {
                        ReloadDataTable("tableUse_Admin");
                    } else {
                        obj.prop('checked', !isChecked);
                    }
                }
            ).fail(handleAjaxError());
        });

        $("#tableUser_Admin").on("click", "a", function () {
            var obj = $(this);
            var action = $(this).data("action");
            var id = $(this).data('id');
            var referalcode = $(this).data('referalcode');
            var codeorname = $(this).data('codeorname');
            switch (action) {
                case "view":
                    redirectToSite(id);
                    break;
                case "edit":
                    //window.location.href = "/Admin/AddEditUser?code=" + codeorname;
                      window.location.href = "/AddEditUser/" + codeorname
                    //window.location.href = "/AddEditUser/" + codeorname + "?Flag=1";

                    break;
                case "pageviewed":
                    pageViewedModal(id);
                    break;
                case "referal":
                    userReferredByCodeModal(referalcode.trim());
                    break;
                case "delete":
                    deleteRecord_Sweet(me.deleteUserUrl, { id: id }, "tableUser_Admin");
                    break;
                case "package":
                    var packageId = $(this).data('package-id');
                    openPackagemodel(id, packageId);
                    break;
                case "packageHistory":
                    PackageHistoryModal(id);
                    break;
                case "payment-details":
                    paymentDetailsModal(id);
                    break;
                case "password":
                    showHidePassword(obj);
                    break;
                case "card-print-details":
                    window.open(me.weburl + "/NFC/Preview/" + id, "_blank");
                    break;
                case "latest-invoice":
                    openInvoice(id);
                    break;
                case "order-details":
                    var orderNo = $(this).data('order-no');
                    orderDetailsModal(orderNo);
                    break;
            }
        });

        $(document).on('click', '#btnSelectPackage', function () {
            var userId = $("#hdnSelectedUserId").val();
            var id = $(this).data("id");
            $.post(me.updatePackage, { UserId: userId, PackageId: id }, function (res) {
                showMessage(res.Message, res.Data, res.Type);
                if (res.Success) {
                    $("#userPackageModal").modal("hide");
                    ReloadDataTable("tableUser_Admin");
                }
            })
        });

        loadUserList();
    }
}