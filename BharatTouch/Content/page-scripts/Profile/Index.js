var ProfileForm = function () {
    var me = this;
    this.captchaSiteKey = null;
    this.googleApiKey = null;
    var calendar = null;
    var openDates = null;
    var openWeekDays = [];
    this.openUpiAppUrl = null;
    this.getScheduleOpenDaysUrl = null;
    this.getMeetingRequestModelUrl = null;
    var sendEmailContactProfileWidgetId = null;
    var isValidGoogleAccessToken = false;
    var isValidMicrosoftAccessToken = false;


    var SendContactEmail = function () {
        if (grecaptcha != undefined && grecaptcha != null) {
            sendEmailContactProfileWidgetId = grecaptcha.render('grecaptchaProfileContact', {
                sitekey: me.captchaSiteKey,
                type: 'image',
                callback: function (token) {
                    $('#hdngrecaptchaResponseProfileContactForm').val(token);
                },
                'expired-callback': function () {
                    $('#hdngrecaptchaResponseProfileContactForm').val("");
                },
                'error-callback': function () {
                    $('#hdngrecaptchaResponseContactForm').val("");
                }
            });
        }

        jQuery.validator.addMethod("minNumberlength", function (value, element) {
            var countryValue = $("#ddlCountryCode").val();
            if (!countryValue) return false;

            var splittedArr = countryValue.split(";"),
                minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return value.length >= minLength;
        }, function (params, element) {
            var countryValue = $("#ddlCountryCode").val();
            var splittedArr = countryValue.split(";");
            var minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return `Minimum ${minLength} digits required.`;
        });

        jQuery.validator.addMethod("maxNumberlength", function (value, element) {
            var countryValue = $("#ddlCountryCode").val();
            if (!countryValue) return false;

            var splittedArr = countryValue.split(";"),
                maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return value.length <= maxLength; //
        }, function (params, element) {
            var countryValue = $("#ddlCountryCode").val();
            var splittedArr = countryValue.split(";");
            var maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return `Maximum ${maxLength} digits required.`;
        }),

        $("#SendContactEmailForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'Name' || element[0].name == 'Email' || element[0].name == 'PhoneNo' || element[0].name == 'LeadTypeId'
                    || element[0].name == 'Message' || element[0].name == 'PersonalEmail' || element[0].name == 'UserID' || element[0].name == 'CountryCode') {
                    error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            },
            rules: {
                Name: { required: true },
                Email: { required: true },
                Message: { required: true },
                PhoneNo: {
                    required: true,
                    minNumberlength: true, 
                    maxNumberlength: true 
                },
                CountryCode: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var data = f.serializeArray();
                var response = null;
                data = data.map(function (item, i) {
                    if (item.name == 'g-recaptcha-response') {
                        item.name = 'grecaptchaResponse';
                        response = item.value;
                    }

                    if (item.name == "PhoneNo") {
                        var code = $("#ddlCountryCode").val();///+91;10;10
                        if (code.split(';').length == 3) {
                            var mergedCode = code.split(';')[0] + item.value;
                            var obj = { name: item.name, value: mergedCode };
                            return obj;
                        }
                    }
                    return item;
                });
                if (response === null || response.length === 0) {
                    showMessage('Notice!', 'Please verify that you are not a robot.', 'notice');
                    return;
                }
                f.find(":submit").find("div.lds-dual-ring").css('display', 'inline-block');
                f.find(":submit").prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: data,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        $("#Name").val('');
                        $("#Email").val('');
                        $("#Message").val('');
                        $("#PhoneNo").val('');
                        $("#ddlCountryCode").val('');
                        if (grecaptcha != undefined && grecaptcha != null) {
                            grecaptcha.reset(sendEmailContactProfileWidgetId);
                        }
                        showMessage(data.Message, data.Data, data.Type);
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    },
                    error: handleAjaxError(function () {
                        if (grecaptcha != undefined && grecaptcha != null) {
                            grecaptcha.reset(sendEmailContactProfileWidgetId);
                        }
                        f.find(":submit").find("div.lds-dual-ring").css('display', 'none');
                        f.find(":submit").prop('disabled', false);
                    })
                });
            }
        });
    }

    var generateQr = function () {

        $.ajax({
            type: 'GET',
            data: { urlCode: $("#hdnUrlCode").val() },
            url: "/Users/GenerateQRCde",
            dataType: 'json',
            success: function (data) {
                $("#imgQrCode").hide();
                if (data.Success) {
                    $("#imgQrCode").show();
                    $("#imgQrCode").attr("src", data.OptionalValue);
                }
            },
            error: function () {
                console.error("Error While generate QR Code.");
            }
        });
    }

    var openPDFViewerModel = function (pdfPath, pdfLabel, obj) {
        //show loading
        obj.find("div.lds-dual-ring-blue").css('display', 'block');
        obj.prop('disabled', true);

        var pdfModal = $("#PDFViewerModal");
        if (pdfModal != null) {
            pdfModal.remove();
        }
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/OpenServiceCatalogModel',
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                ininPDFViewerPlugin(
                    pdfPath,
                    () => {
                        OpenModel("PDFViewerModal");
                        $("#pdfViewerModalLabel").text(pdfLabel);
                        $("#btnPdfDownload").attr("href", `/Users/DownloadFilesFromPath?fileName=${pdfPath}`);
                        $("#btnPdfDownload").show();
                        //hide loading
                        obj.find("div.lds-dual-ring-blue").css('display', 'none');
                        obj.prop('disabled', false);
                    }
                );
                //OpenModel("PDFViewerModal");
                //$("#pdfViewerModalLabel").text(pdfLabel);
            },
            error: function () {
                handleAjaxError()
                //hide loading
                obj.find("div.lds-dual-ring-blue").css('display', 'none');
                obj.prop('disabled', false);
            }
        });    
    }

    var ininPDFViewerPlugin = function (path, callback)
    {
        const url = path;
        console.log(url);
        let pdfDoc = null,
            pageNum = 1,
            pageIsRendering = false,
            pageNumIsPending = null;

        const scale = 3,
            canvas = document.querySelector('#pdf-render'),
            ctx = canvas.getContext('2d');

        // Render the page
        const renderPage = num => {
            pageIsRendering = true;

            // Get page
            pdfDoc.getPage(num).then(page => {
                // Set scale
                const viewport = page.getViewport({ scale });
                canvas.height = viewport.height;
                canvas.width = viewport.width;

                const renderCtx = {
                    canvasContext: ctx,
                    viewport
                };

                page.render(renderCtx).promise.then(() => {
                    pageIsRendering = false;

                    if (pageNumIsPending !== null) {
                        renderPage(pageNumIsPending);
                        pageNumIsPending = null;
                    }
                });

                // Output current page
                document.querySelector('#page-num').textContent = num;
                if (callback) {
                    callback()
                }
            });
        };

        // Check for pages rendering
        const queueRenderPage = num => {
            if (pageIsRendering) {
                pageNumIsPending = num;
            } else {
                renderPage(num);
            }
        };

        // Show Prev Page
        const showPrevPage = () => {
            if (pageNum <= 1) {
                return;
            }
            pageNum--;
            queueRenderPage(pageNum);
        };

        // Show Next Page
        const showNextPage = () => {
            if (pageNum >= pdfDoc.numPages) {
                return;
            }
            pageNum++;
            queueRenderPage(pageNum);
        };

        // Get Document
        pdfjsLib
            .getDocument(url)
            .promise.then(pdfDoc_ => {
                pdfDoc = pdfDoc_;

                document.querySelector('#page-count').textContent = pdfDoc.numPages;

                renderPage(pageNum);
            })
            .catch(err => {
                console.log(err);
                // Display error
                const div = document.createElement('div');
                div.className = 'text-danger';
                div.appendChild(document.createTextNode(err.message));
                document.querySelector('body').insertBefore(div, document.querySelector('#canvascontainer'), );
                // Remove top bar
                document.querySelector('.top-bar').style.display = 'none';
            });

        // Button Events
        document.querySelector('#prev-page').addEventListener('click', showPrevPage);
        document.querySelector('#next-page').addEventListener('click', showNextPage);
    }

    var saveViewHistory = async function () {
        //try {
        //    if (!navigator.geolocation) {
        //        viewHistoryAjaxReq("", "");
        //        console.log('Geolocation is not supported by this browser.');
        //        return;
        //    }
        //    navigator.geolocation.getCurrentPosition(function (position) {
        //        LocationLat = position.coords.latitude;
        //        LocationLon = position.coords.longitude;
        //        viewHistoryAjaxReq(LocationLat, LocationLon);
        //    }, function (error) {
        //        viewHistoryAjaxReq("", "");
        //        console.log("geoLocation is not available or user denied location permission.");
        //    });

        //}
        //catch (er) {
        //    console.log("Error occurred while save view history", er);
        //}
        viewHistoryAjaxReq();
    }

    var viewHistoryAjaxReq = async function (lat = "", long = "") {
        try {
            var ui = {};
            if (navigator.userAgentData && navigator.userAgentData.getHighEntropyValues) {
                ui = await navigator.userAgentData.getHighEntropyValues([
                    'platform', 'platformVersion', 'architecture', 'model'
                ]);
            }

            var displayName = $("#hdnUrlCode").val();
            var IPAddress = $("#hdnIpAddress").val();
            var BrowserName = $("#hdnbrowserName").val();
            var BrowserVersion = $("#hdnbrowserVersion").val();
            var PlateForm = (ui && ui.platform) ? ui.platform : $("#hdnbrowserPlateForm").val();
            var IsMobile = navigator.userAgentData ? navigator.userAgentData.mobile : /Mobi|Android/i.test(navigator.userAgent);
            var ModelName = (ui && ui.model) ? ui.model : "";
            var UserAgent = navigator.userAgent;

            var City = "";
            var State = "";
            var Country = "";
            if (lat != "" && long != "") {
                try {
                    const geolocationResponse = await fetch(`https://nominatim.openstreetmap.org/reverse?lat=${lat}&lon=${long}&format=json`);
                    const geolocationData = await geolocationResponse.json();
                    City = geolocationData.address.city || "";
                    State = geolocationData.address.state || "";
                    Country = geolocationData.address.country || "";
                }
                catch (er) { }

            }

            $.get(
                "/Users/InsertViewHistory",
                {
                    displayName: displayName,
                    IPAddress: IPAddress,
                    BrowserName: BrowserName,
                    BrowserVersion: BrowserVersion,
                    PlateForm: PlateForm,
                    IsMobile: IsMobile,
                    ModelName: ModelName,
                    UserAgent: UserAgent,
                    LocationLat: lat,
                    LocationLon: long,
                    City: City,
                    State: State,
                    Country: Country
                },
                function (data) {
                    if (data.Success) {
                        console.log("browser history saved");
                    }
                    else {
                        console.log("browser history save failed");
                    }
                }
            ).fail(function () {
                console.error("Error while insert profile view History");
            });
            
        }
        catch (er) {
            console.log(er);
        }
    }


    function openWordToPdfViewerModel(path, label, obj) {
        //show loading
        obj.find("div.lds-dual-ring-blue").css('display', 'block');
        obj.prop('disabled', true);

        var pdfModal = $("#PDFViewerModal");
        if (pdfModal != null) {
            pdfModal.remove();
        }
        $.ajax({
            type: 'POST',
            url: '/Users/ConvertWordToPdf',
            data: { path: path },
            dataType: 'json',
            success: function (data, strStatus) {
                console.log("converted word path", data);
                if (data.Success == true) {
                    openPDFViewerModel(data.Data, "Resume", obj);
                } else {
                    showMessage('Failed!', data.Data, data.Type);
                    //hide loading
                    obj.find("div.lds-dual-ring-blue").css('display', 'none');
                    obj.prop('disabled', false);
                }
            },
            error: function () {
                handleAjaxError();
                //hide loading
                obj.find("div.lds-dual-ring-blue").css('display', 'none');
                obj.prop('disabled', false);
            }
        });    
    }

    function openImageModel(path, label, obj) {
        //show loading
        obj.find("div.lds-dual-ring-blue").css('display', 'block');
        obj.prop('disabled', true);

        var pdfModal = $("#PDFViewerModal");
        if (pdfModal != null) {
            pdfModal.remove();
        }
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Users/OpenServiceCatalogModel',
            dataType: 'html',
            success: function (data, strStatus) {
                $("body").append(data);
                $('#PDFViewerModal .modal-body').empty();
                
                const imgElement = $('<img>')
                    .attr('src', path)
                    .addClass('img-fluid')
                    .css({ width: '100%', height: 'auto' });

                $('#PDFViewerModal .modal-body').append(imgElement);
                OpenModel("PDFViewerModal");
                $("#pdfViewerModalLabel").text(label);

                //hide loading
                obj.find("div.lds-dual-ring-blue").css('display', 'none');
                obj.prop('disabled', false);
            },
            error: function () {
                handleAjaxError()
                //hide loading
                obj.find("div.lds-dual-ring-blue").css('display', 'none');
                obj.prop('disabled', false);
            }
        });    
    }

    function normalizeDate(date) {
        date = new Date(date);
        var year = date.getFullYear();
        var month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-based
        var day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    function isTodayOrCurrentMonthFuture(date) {
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const inputDate = new Date(date);
        inputDate.setHours(0, 0, 0, 0);

        //return inputDate >= today && today.getMonth() === inputDate.getMonth();
        return inputDate >= today;
    }

    var GetScheduleOpenDays = function (callBack) {
        $.get(me.getScheduleOpenDaysUrl, { UserId: $('#hdnProfileOwnerUserId').val() }, function (data) {
            if (data.Success) {
                openDates = data.Data.map(item => {
                    return new Date(item.Date);
                });
            }

            if (callBack) {
                callBack(data);
            }
        }).fail(function () {
            console.error("Error while fetching open Week Days");
        })
    }

    var getEnableWeekDays = function () {
        $.get(
            "/Users/GetScheduleOpenWeekDays",
            {
                UserId: $('#hdnProfileOwnerUserId').val(),
                actionName: "BharatTouch/Profile/init"
            },
            function (result) {
                if (result.Success) {
                    var d = result.Data;
                    if (d.Sun) openWeekDays.push(0);
                    if (d.Mon) openWeekDays.push(1);
                    if (d.Tue) openWeekDays.push(2);
                    if (d.Wed) openWeekDays.push(3);
                    if (d.Thu) openWeekDays.push(4);
                    if (d.Fri) openWeekDays.push(5);
                    if (d.Sat) openWeekDays.push(6);
                }
            }
        ).fail(function () {
            console.error("Error while fetching Open Week Days");
        })
    }

    var calendarInit = function () {
        var calendarEl = document.getElementById('calendar');
        if (calendarEl != null) {
            var today = new Date();
            calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                firstDay: 1, // 0 = Sunday, 1 = Monday
                headerToolbar: {
                    right: 'prev,next today'
                },
                //validRange: {
                //    start: today // Restricts previous months
                //},
                dayCellDidMount: function (info) {
                    var date = new Date(info.date);

                    if (openWeekDays && openWeekDays.length > 0 && openWeekDays.includes(date.getDay()) && isTodayOrCurrentMonthFuture(info.date)) {
                        info.el.style.backgroundColor = '#1efa0436';
                        info.el.style.cursor = "pointer";
                    }

                    //var isOpenDate = !!(openDates.find(item => normalizeDate(item) == normalizeDate(info.date)));
                    //if (isTodayOrCurrentMonthFuture(info.date) && isOpenDate) {
                    //    info.el.style.backgroundColor = '#1efa0436';
                    //    info.el.style.cursor = "pointer";
                    //}
                },
                dateClick: function (info) {
                    if (openWeekDays && openWeekDays.length > 0) {
                        var isOpenDate = !!(openWeekDays.find(item => item === (new Date(info.dateStr)).getDay()));
                        if (isOpenDate && isTodayOrCurrentMonthFuture(info.dateStr)) {
                            openMeetingFormModel(info.dateStr);
                        }
                    }

                    //if (Array.isArray(openDates) && openDates.length > 0) {
                    //    var isOpenDate = !!(openDates.find(item => normalizeDate(item) == normalizeDate(info.dateStr)));
                    //    if (isOpenDate && isTodayOrCurrentMonthFuture(info.dateStr)) {
                    //        openMeetingFormModel(info.dateStr);
                    //    }
                    //}
                },
                datesSet: changeCalendarDatesColor
            });
            calendar.render();
        }
    }

    var changeCalendarDatesColor = function () {
        var calendarEl = document.getElementById('calendar');
        var allDateCells = calendarEl.querySelectorAll('[data-date]');
        allDateCells.forEach(function (cell) {
            cell.style.backgroundColor = '';
            cell.style.cursor = "";
        });

        if (Array.isArray(openWeekDays) && openWeekDays.length > 0) {
            var dayCells = document.querySelectorAll('.fc-daygrid-day');

            dayCells.forEach(function (cell) {
                var dateStr = cell.getAttribute('data-date');
                var date = new Date(dateStr);

                if (openWeekDays.includes(date.getDay()) && isTodayOrCurrentMonthFuture(date)) {
                    cell.style.backgroundColor = '#1efa0436';
                    cell.style.cursor = "pointer";
                }
            });
        }

        //if (Array.isArray(openDates) && openDates.length > 0) {

        //    openDates.forEach(function (dateStr) {
        //        if (!isTodayOrCurrentMonthFuture(dateStr)) {
        //            return;
        //        }
        //        var formattedDate = normalizeDate(dateStr);
        //        var cell = calendarEl.querySelector(`[data-date="${formattedDate}"]`);

        //        if (cell) {
        //            cell.style.backgroundColor = '#1efa0436';
        //            cell.style.cursor = "pointer";
        //        }
        //    });
        //}
    }

    var createEvent = async function (d) {
        var data = d.reduce((obj, item) => {
            obj[item.name] = item.value;
            return obj;
        }, {});

        if (isValidMicrosoftAccessToken) {
            const outlookEvent = {
                subject: 'Meeting request',
                body: {
                    contentType: 'HTML',
                    content: `<!DOCTYPE html>
                                <html>
                                <head>
                                    <title>Meeting requested</title>
                                </head>
                                <body>
                                    <p><strong>Name:</strong> ${data.Name}</p>
                                    <p><strong>Email:</strong> ${data.Email}</p>
                                    <p><strong>Phone:</strong> ${data.PhoneNo}</p>
                                    <p><strong>Message:</strong></p>
                                    <p>${data.Message}</p>
                                </body>
                                </html>`,
                },
                start: {
                    dateTime: new Date(data.Date),
                    timeZone: "Asia/Kolkata",
                },
                end: {
                    dateTime: new Date(data.Date),
                    timeZone: "Asia/Kolkata",
                },
                attendees: [
                    {
                        emailAddress: {
                            address: data.Email,
                        },
                        type: 'required',
                    },
                ],
            };

            $.ajax({
                url: 'https://graph.microsoft.com/v1.0/me/calendar/events',
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + $("#hdnUserMicrosoftAccessToken").val(),
                    'Content-Type': 'application/json',
                },
                data: JSON.stringify(outlookEvent),
                success: function (data) {
                    console.log('outlook Event created successfully:');
                },
                error: function (error) {
                    console.log('outlook Error creating event:', error);
                },
            });
        }

        if (isValidGoogleAccessToken) {
            const googleEvent = {
                'summary': 'Meeting request',
                //'location': '800 Howard St., San Francisco, CA 94103',
                'description': `Meeting requested by:\nName: ${data.Name}\nEmail: ${data.Email}\nPhone: ${data.PhoneNo}\n\nMessage: ${data.Message}`,
                'start': {
                    'dateTime': new Date(data.Date),
                    'timeZone': 'Asia/Kolkata'
                },
                'end': {
                    'dateTime': new Date(data.Date),
                    'timeZone': "Asia/Kolkata"
                },
                'recurrence': [
                    'RRULE:FREQ=DAILY;COUNT=1'    // Count means how many events created in single request.
                ],
                'attendees': [
                    { 'email': data.Email }
                ],
                'reminders': {
                    'useDefault': false,
                    'overrides': [
                        { 'method': 'email', 'minutes': 24 * 60 },
                        { 'method': 'popup', 'minutes': 30 }
                    ]
                }
            };

            try {
                await gapi.client.calendar.events.insert({
                    'calendarId': 'primary',
                    'resource': googleEvent,
                    oauth_token: $("#hdnUserGoogleAccessToken").val(),
                });

                console.log("Google event created.");
            } catch (err) {
                console.error("Error creating google event", err);
            }

        }

    }

    var meetingRequestForm = function () {

        var sendMeetingRequestEmailWidgetId = null;

        if (grecaptcha != undefined && grecaptcha != null) {
            sendMeetingRequestEmailWidgetId = grecaptcha.render('grecaptchaSendEmailMeetingRequest', {
                sitekey: me.captchaSiteKey,
                type: 'image',
                callback: function (token) {
                    $('#hdngrecaptchaResponseMeetingRequestForm').val(token);
                },
                'expired-callback': function () {
                    $('#hdngrecaptchaResponseMeetingRequestForm').val("");
                },
                'error-callback': function () {
                    $('#hdngrecaptchaResponseMeetingRequestForm').val("");
                }
            });
        }

        var submitBtn = $('#btnSubmitMeetingRequestForm');
        submitBtn.on('click', function () {
            $('#meetingRequestForm').submit();
        });


        jQuery.validator.addMethod("minNumlength", function (value, element) {
            var countryValue = $("#ddlMeetingCountryCode").val();
            if (!countryValue) return false;

            var splittedArr = countryValue.split(";"),
                minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return value.length >= minLength;
        }, function (params, element) {
            var countryValue = $("#ddlMeetingCountryCode").val();
            if (!countryValue) return "Minimum 10 digits required.";
            var splittedArr = countryValue.split(";");
            var minLength = splittedArr.length > 1 ? parseInt(splittedArr[1], 10) || 10 : 10;

            return `Minimum ${minLength} digits required.`;
        });

        jQuery.validator.addMethod("maxNumlength", function (value, element) {
            var countryValue = $("#ddlMeetingCountryCode").val();
            if (!countryValue) return false;

            var splittedArr = countryValue.split(";"),
                maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return value.length <= maxLength; //
        }, function (params, element) {
            var countryValue = $("#ddlMeetingCountryCode").val();
            if (!countryValue) return "Maximum 10 digits required.";
            var splittedArr = countryValue.split(";");
            var maxLength = splittedArr.length > 2 ? parseInt(splittedArr[2], 10) || 10 : 10;

            return `Maximum ${maxLength} digits required.`;
        }),


        $("#meetingRequestForm").validate({
            errorPlacement: function (error, element) {
                if (element[0].name == 'UserId' || element[0].name == 'Name' || element[0].name == 'Email'
                    || element[0].name == 'PhoneNo' || element[0].name == 'Message' || element[0].name == 'MeetingCountryCode' ) {
                        error.appendTo(element.parent());
                }
                else {
                    error.appendTo(element.parent().parent());
                }
            },
            rules: {
                UserId: { required: true },
                Name: { required: true },
                Email: { required: true },
                Message: { required: true },
                PhoneNo: {
                    required: true,
                    minNumlength: true,
                    maxNumlength: true
                },
                MeetingCountryCode: { required: true }
            },
            submitHandler: function (form) {
                var f = $(form);
                var formData = f.serializeArray();

                var response = null;
                formData = formData.map(function (item, i) {
                    if (item.name == 'g-recaptcha-response') {
                        item.name = 'grecaptchaResponse';
                        response = item.value;
                    }
                    if (item.name == "PhoneNo") {
                        var code = $("#ddlMeetingCountryCode").val();///+91;10;10
                        if (code.split(';').length == 3) {
                            var mergedCode = code.split(';')[0] + item.value;
                            var obj = { name: item.name, value: mergedCode };
                            return obj;
                        }
                    }
                    return item;
                });
                if (response === null || response.length === 0) {
                    showMessage('Notice!', 'Please verify that you are not a robot.', 'notice');
                    return;
                }

                submitBtn.find("div.lds-dual-ring-blue").css('display', 'inline-block');
                submitBtn.prop('disabled', true);
                $.ajax({
                    type: f[0].method,
                    url: f[0].action,
                    data: formData,
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        submitBtn.find("div.lds-dual-ring-blue").css('display', 'none');
                        submitBtn.prop('disabled', false);

                        if (grecaptcha != undefined && grecaptcha != null) {
                            grecaptcha.reset(sendMeetingRequestEmailWidgetId);
                        }

                        if (data.Success) {
                            setTimeout(() => {
                                createEvent(formData);
                                $('#MeetingRequestFormModel').modal('hide');
                            }, 10);
                        }
                    },
                    error: function () {
                        submitBtn.find("div.lds-dual-ring-blue").css('display', 'none');
                        submitBtn.prop('disabled', false);
                        if (grecaptcha != undefined && grecaptcha != null) {
                            grecaptcha.reset(sendMeetingRequestEmailWidgetId);
                        }
                        handleAjaxError()
                    }
                });
            }
        });
    }

    var openMeetingFormModel = function (date) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { UserId: $('#hdnProfileOwnerUserId').val(), date: normalizeDate(date) },
            url: me.getMeetingRequestModelUrl,
            dataType: 'html',
            success: function (data, strStatus) {
                $("#meetingFormModelContainer").html('');
                $("#meetingFormModelContainer").append(data);
                $('#MeetingRequestFormModel').modal('show');
                $('#MeetingRequestFormModel').on('shown.bs.modal', () => setTimeout(meetingRequestForm, 500));
                //meetingRequestForm();
            },
            error: handleAjaxError()
        });
    }

    function removeAllModelDataKeys() {
        $.each($('#deleteModelData').data(), function (key) {
            $('#deleteModelData').removeData(key);
        });
    }

    function setAndShowDeleteModel(obj) {
        removeAllModelDataKeys();
        $("#deleteModelData").data(obj);
        $("#deleteModel").modal("show");
    }

    async function LoadGoogleApi(whenDoneCallback) {
        var res = await gapi.load('client', async () => {
            const DISCOVERY_DOC = 'https://www.googleapis.com/discovery/v1/apis/calendar/v3/rest';
            var res = await gapi.client.init({
                apiKey: me.googleApiKey,
                discoveryDocs: [DISCOVERY_DOC],
            });
            if (whenDoneCallback) {
                await whenDoneCallback();
            }
            console.log("initializeGoogleApiClient Res", res);
        });
        console.log("GoogleApi Loaded", res);
    }

    async function isAccessTokenValid(token) {
        try {
            const request = {
                'calendarId': 'primary',
                'timeMin': (new Date()).toISOString(),
                'showDeleted': false,
                'singleEvents': true,
                'maxResults': 10,
                'orderBy': 'startTime',
                oauth_token: token
            };
            response = await gapi.client.calendar.events.list(request);
            console.log("validity check", response);
            return true;
        } catch (err) {
            console.log('error token', err);
            return false;
        }
    }

    async function GoogleOAuthInit() {
        var tokenEle = $("#hdnUserGoogleAccessToken");
        var userId = $("#hdnProfileOwnerUserId").val();

        await LoadGoogleApi(async function () {
            if (tokenEle.val().trim() == "") {
                isValidGoogleAccessToken = false;
                return;
            }
            var isValid = await isAccessTokenValid(tokenEle.val());
            if (isValid) {
                isValidGoogleAccessToken = true;
                return;
            }
            $.get("/OAuth/RefreshGoogleAccessToken", { userId: userId }, function (data) {
                if (!data.Success) {
                    isValidGoogleAccessToken = false;
                    return;
                }

                tokenEle.val(data.Data);
                isValidGoogleAccessToken = true;
            }).fail(function () {
                console.log("refresh token error.")
            });
        });
    }

    async function isMicrosoftTokenValid(accessToken) {
        const apiUrl = 'https://graph.microsoft.com/v1.0/me';
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
            });
            if (!response.ok) {
                return false;
            }

            return true;
        } catch (error) {
            console.error('Error checking token validity:', error);
            return false;
        }
    }

    async function MicrosoftOAuthInit() {
        var tokenEle = $("#hdnUserMicrosoftAccessToken");
        var userId = $("#hdnProfileOwnerUserId").val();

        if (tokenEle.val().trim() == "") {
            isValidMicrosoftAccessToken = false;
            return;
        }

        if (await isMicrosoftTokenValid(tokenEle.val())) {
            isValidMicrosoftAccessToken = true;
            return;
        }

        $.get("/OAuth/RefreshAccessTokenMicrosoftAsync", { userId: userId }, function (data) {
            if (!data.Success) {
                isValidMicrosoftAccessToken = false;
                return;
            }

            tokenEle.val(data.Data);
            isValidMicrosoftAccessToken = true;
        }).fail((err) => {
            console.log("refresh microsoft token error.", err);
        });
    }

    function TestimonialInit() {
        var loopEnabled = $("#hdnLoopClientTestimonial").val() === 'true';
        new Swiper('.testimonials-slider', {
            speed: 600,
            loop: $("#hdnLoogClientTestimonial").val(),
            autoplay: {
                delay: 5000,
                disableOnInteraction: false
            },
            slidesPerView: 'auto',
            initialSlide: 0,
            centeredSlides: !loopEnabled,
            pagination: {
                el: '.swiper-pagination',
                type: 'bullets',
                clickable: true
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    spaceBetween: 40
                },

                1200: {
                    slidesPerView: 3,
                }
            }
        });
    }

    function OpenMultipleContactModel() {
        var type = $(this).data("type");
        var modal = $("#multipleContactModal");
        var title = $("#multipleModalTitle");
        var phoneContainer = $("#multipleModelPhoneContainer");
        var locationContainer = $("#multipleModelLocationContainer");
        switch (type) {
            case 'phone':
                title.html("Contacts");
                locationContainer.hide();
                phoneContainer.show();
                modal.modal("show");
                break;
            case 'location':
                title.html("Location");
                phoneContainer.hide();
                locationContainer.show();
                modal.modal("show");
                break;
        }
    }

    this.init = function () {

        $("#openMultiContactModalLocation").on('click', OpenMultipleContactModel);
        $("#openMultiContactModalPhone").on('click', OpenMultipleContactModel);

        //var isProduction = window.location.hostname !== "localhost";

        //if (isProduction) {
        //    $(document).on("contextmenu", function (e) {
        //        e.preventDefault();
        //        console.log("Right-click is disabled in production.");
        //    });
        //}

        getEnableWeekDays();
        GetScheduleOpenDays();

        $(window).on('load', async () => {
            setTimeout(async function () {
                SendContactEmail();
                TestimonialInit();
                calendarInit();
                await GoogleOAuthInit();
                await MicrosoftOAuthInit();
                saveViewHistory();
            }, 100)
        });

        generateQr();

        $("#btnViewPortfolio").click(function () {
            var obj = $(this);
            var path = obj.data("path");
            openPDFViewerModel(path,"Portfolio", obj);
        });

        $("#paymentImgQrCode").click(function () {
            console.log('payment button clicked')
            window.open(me.openUpiAppUrl, "_blank");
        });

        $("#btnViewResume").click(function () {
            var obj = $(this);
            var path = obj.data("path");
            var extension = path.split('.').pop().toUpperCase();
            if (extension === "DOCX" || extension === "DOC") {
                openWordToPdfViewerModel(path, "Resume", obj);
            } else if (extension === "JPG" || extension === "JPEG" || extension === "PNG") {
                openImageModel(path, "Resume", obj)
            } else if (extension === "PDF") {
                openPDFViewerModel(path, "Resume", obj);
            }
        });

        $("#btnViewService").click(function () {
            var obj = $(this);
            var path = obj.data("path");
            openPDFViewerModel(path, "Service Catalogue", obj);
        });
        $("#btnViewMenuLink").click(function () {
            var obj = $(this);
            var path = obj.data("path");
            var extension = path.split('.').pop().toUpperCase();
            if (extension === "DOCX" || extension === "DOC") {
                openWordToPdfViewerModel(path, "Menu", obj);
            } else if (extension === "JPG" || extension === "JPEG" || extension === "PNG") {
                openImageModel(path, "Menu", obj)
            } else if (extension === "PDF") {
                openPDFViewerModel(path, "Menu", obj);
            }
        });

        $(document).on('click', '.viewUserCertificate', function () {
            var obj = $(this);
            var path = obj.data("path");
            var extension = path.split('.').pop().toUpperCase();
            if (extension === "JPG" || extension === "JPEG" || extension === "PNG") {
                openImageModel(path, "User Certificate", obj)
            } else if (extension === "PDF") {
                openPDFViewerModel(path, "User Certificate", obj);
            }
        });

        $(document).on('click', '.btnDeleteLead', function () {
            var obj = $(this);
            setAndShowDeleteModel({
                id: obj.data("id"),
                callback: deleteLead
            });

        });

        $('#btnDeleteModel').on('click', function () {
            var callback = $('#deleteModelData').data('callback');
            var data = $('#deleteModelData').data();
            if (typeof callback === 'function') {
                callback(data);
            }
        });

        //function isValidGST(gstin) {
        //    const gstRegex = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$/;
        //    return gstRegex.test(gstin.toUpperCase());
        //}
        
    }
}