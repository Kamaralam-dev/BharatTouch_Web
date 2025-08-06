
//Settings
var DEFAULT_DATE_FORMAT = "mm/dd/yyyy";
var DEFAULT_DATE_FORMAT_MOMENT = "MM/DD/YYYY";
var DEFAULT_TIME_FORMAT_MOMENT = "HH:mm";
var DEFAULT_DATETIME_FORMAT_MOMENT = "MM/DD/YYYY HH:mm:ss";
var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
/**
 * show message
 * @param {any} heading
 * @param {any} text
 * @param {any} icon options include: info, warning, success, error
 * @param {any} hideAfter
 */
var showMessage = function (heading, message, type, hideAfter) {
    var options = { title: heading, text: message };
    if (hideAfter) {
        options.delay = hideAfter;
    }

    if (type == "notice") {
        PNotify.notice(options);
    }
    else if (type == "info") {
        PNotify.info(options);
    }
    else if (type == "success") {
        PNotify.success(options);
    }
    else if (type == "error") {
        PNotify.error(options);
    }
}

/**
 * show confirm popup
 * @param {any} title
 * @param {any} text
 * @param {any} func
 */
var deleteRecord = function (funcUrl, data, tableId) {
    var notice = PNotify.notice({
        title: "Confirmation Needed",
        text: "Are you sure to delete this record?",
        icon: 'brighttheme-icon-notice',
        hide: false,
        modules: {
            Confirm: {
                confirm: true
            },
            Buttons: {
                closer: false,
                sticker: false
            },
            History: {
                history: false
            }
        }
    });
    notice.on('pnotify.confirm', function () {
        DeleteDatatableRecordAndRefreshTable(funcUrl, data, tableId);
    });
    notice.on('pnotify.cancel', function () { });
}

function DeleteDatatableRecordAndRefreshTable(url, postData, tableId) {
    $.post(url, postData, function (data, status) {
        $('#' + tableId).DataTable().ajax.reload(null, false);
        showMessage(data.Message, data.Data, data.Type);
    }).fail(handleAjaxError());
}

function ReloadDataTable(tableId) {
    $('#' + tableId).DataTable().ajax.reload(null, false);
}

function GetAjaxDataPromise(url, postData) {
    var promise = $.post(url, postData, function (promise, status) {
        showMessage(data.Message, data.Data, data.Type);
    });
    return promise;
}

var setRowIndex = function (table) {
    var bodyRows = $(table + " tbody tr");
    for (var i = 0; i < bodyRows.length; i++) {
        var elements = $(bodyRows[i]).find("input,select,textarea");
        $(elements).each(function () {
            var el = $(this);
            var name = el.attr("name");
            name = name.replace(/\[.*?\]/g, "[" + i + "]");
            el.attr("name", name);
            if (el.is(':checkbox')) {
                el.attr("id", name);
                $(this).siblings("label").attr("for", name);
            }
        });
    }
}

var CloneRows = function (table) {
    var row = $(table + '>tbody>tr:first').clone();
    row.find("select").val("");
    row.find("a").attr("data-id", "0");
    row.find("span[id='trCounter']").text(($(table + '>tbody>tr').length + 1));
    row.find("input").val("");
    row.find("textarea").val("");
    row.appendTo(table);
    setRowIndex(table);
    return row;
}

var OpenModel = function (id) {
    //$.magnificPopup.open({
    //    items: {
    //        src: "#" + id,
    //        type: 'inline'
    //    }
    //});
    $("#"+id).modal('show'); 

}




var downloadFileByPath = function (filePath) {
    var a = document.createElement("a");
    a.setAttribute('href', filePath);
    a.setAttribute('download', '');
    a.setAttribute('target', '_self');
    a.click();
}

var CloseModal = function () {
    $(".mfp-hide").remove();
    $(".mfp-bgm").remove();
    $('body').removeClass('modal-open');
    $('body').removeAttr('style');
    $.magnificPopup.close();
}

function ValidateUploadFile(oInput) {
    if (oInput.type == "file") {
        var sFileName = oInput.value;
        if (sFileName.length > 0) {
            var blnValid = false;
            for (var j = 0; j < _validFileExtensions.length; j++) {
                var sCurExtension = _validFileExtensions[j];
                if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                    blnValid = true;
                    break;
                }
            }

            if (!blnValid) {
                showMessage("Warning!", "Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "), "notice");
                oInput.value = "";
                return false;
            }
        }
    }
}

var tryParseJson = function (text) {
    try {
        var json = $.parseJSON(text);
        return { success: true, obj: json };
    } catch (err) {
        return { success: false, obj: null };
    }
}

var handleAjaxError = function (callback, errorMessage) {
    var hideAfter = 10000;
    return function (xhr, status, error) {
        if (callback && typeof callback == "function") {
            callback(xhr, status, error);
        }

        if (xhr.responseJSON) {
            var data = xhr.responseJSON;
            showMessage(data.Message, data.Data, 'error', hideAfter);
            return;
        }
        else if (xhr.responseText) {
            var data = tryParseJson(xhr.responseText);
            if (data.success) {
                showMessage(data.obj.Message, data.obj.Data, 'error', hideAfter);
                return;
            }
        }

        switch (xhr.status) {
            case 400:
                showMessage("Bad Request", "Making sure you are requesting with the right parameters or link.", 'error', hideAfter);
                break;
            case 401:
                showMessage("Forbbiden", "Please login again with appropriate account.", 'error', hideAfter);
                break;
            case 404:
                showMessage("Not Found", "The resource rquested may have been removed.", 'error', hideAfter);
                break;
            case 413:
                showMessage("Size Error", "File size is too large.", 'error', hideAfter);
                break;
            default:
                showMessage("Unknown Error", errorMessage || "Unknown error occurred. Please try again later", 'error', hideAfter);
                break;
        }
    }

}
var rhte = function (placeToRound) {
    var fixed = placeToRound.toString().split('.').length < 2 ? 0 : placeToRound.toString().split('.')[1].length,
        numParts = {
            mvDec: (this / placeToRound).toFixed(this.toString().length).toString().split('.'),
            wholeNum: function () { return parseInt(this.mvDec[0], 10); },
            dec: function () { return this.mvDec.length > 1 ? parseFloat('0.' + this.mvDec[1]) : 0; },
            oddEven: function () { return (this.wholeNum() % 2 === 1) ? 1 : 0; }
        };

    if (numParts.dec() !== 0.5) {
        return (numParts.dec() > 0.5) ? parseFloat(((numParts.wholeNum() + 1) * placeToRound).toFixed(fixed)) : parseFloat((numParts.wholeNum() * placeToRound).toFixed(fixed));
    }
    else {
        if (numParts.oddEven() === 1) {
            return parseFloat(((numParts.wholeNum() + 1) * placeToRound).toFixed(fixed));
        }
        else {
            return parseFloat((numParts.wholeNum() * placeToRound).toFixed(fixed));
        }
    }
};

Number.prototype.rhte = rhte;
String.prototype.rhte = rhte;

Number.prototype.roundNumber = function () {
    return this.rhte(0.01).toFixed(2);
}

function SelectAllOrNone(formName, mainCkb) {
    if (mainCkb.checked) {
        $("#" + formName).find('input[type="checkbox"]').prop('checked', true);
    } else {
        $("#" + formName).find('input[type="checkbox"]').prop('checked', false);
    }
}

function BuildSelect(select, data, keyName, valueName, option) {
    if (option == null)
        option = "Please select"

    $(select).empty();
    $(select).append("<option value=''>" + option + "</option>");
    if (data == null) return;
    $(data).each(function () {
        $(select).append("<option value='" + eval(keyName) + "'>" + eval(valueName) + "</option>");
    });
}

/*Custom jquery validation methods*/
if (jQuery.validator != null) {
    jQuery.validator.addMethod("dateCompare", function (value, element, params) {
        var targetValue = $(params.target).val().trim();
        if (targetValue != "") {
            var self = moment(value, DEFAULT_DATE_FORMAT_MOMENT);
            var target = moment(targetValue, DEFAULT_DATE_FORMAT_MOMENT);
            return this.optional(element) || self >= target;
        } else {
            return true;
        }
    }, "End date must be later than or equals to start date");

    jQuery.validator.addMethod("timeCompare", function (value, element, params) {
        var ph = "04/07/2018 ";
        var phf = "MM/DD/YYYY ";
        var targetValue = $(params.target).val().trim();
        if (targetValue != "") {
            var self = moment(ph + value, phf + DEFAULT_TIME_FORMAT_MOMENT);
            var target = moment(ph + targetValue, phf + DEFAULT_TIME_FORMAT_MOMENT);
            return this.optional(element) || self >= target;
        } else {
            return true;
        }
    }, "End time must be later than or equals to start time");

    jQuery.validator.addMethod("datetimeCompare", function (value, element, params) {
        var phf = "MM/DD/YYYY ";
        var targetDate = $(params.targetDate).val().trim();
        var targetTime = $(params.targetTime).val().trim();
        var selfDate = $(params.selfDate).val().trim();
        var selfTime = $(params.selfTime).val().trim();
        if (targetDate != "" && targetTime != "") {
            var targetVal = targetDate + " " + targetTime;
            var selfVal = selfDate + " " + selfTime;
            var self = moment(selfVal, phf + DEFAULT_TIME_FORMAT_MOMENT);
            var target = moment(targetVal, phf + DEFAULT_TIME_FORMAT_MOMENT);
            return this.optional(element) || self >= target;
        } else {
            return true;
        }
    }, "End date must be later than or equals to start date");

    jQuery.validator.addMethod("digitsCompare", function (value, element, params) {
        var targetValue = $(params.target).val().trim();
        if (targetValue != "") {
            var self = parseInt(value);
            var target = parseInt(targetValue);
            return this.optional(element) || self >= target;
        } else {
            return true;
        }
    }, "Max value must be greater than or equals to min value");

    jQuery.validator.addMethod("letterswithspace", function (value, element) {
        return this.optional(element) || /^[a-z][a-z\s]*$/i.test(value);
    }, "Only letters are allowed.");

    jQuery.validator.addMethod("decimalnumber", function (value, element) {
        return this.optional(element) || /^[0-9]+(\.[0-9]{1,2})?$/i.test(value);
    }, "Only number or decimal are allowed.");

    jQuery.validator.addMethod("digitonly", function (value, element) {
        return this.optional(element) || /^[0-9]+$/i.test(value);
    }, "Only number are allowed.");

    jQuery.validator.addMethod("emailvalidatecustom", function (value, element) {
        return this.optional(element) || /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i.test(value);
    }, "Enter valid email address.");

    jQuery.validator.addMethod("notspecialchars", function (value, element) {
        return this.optional(element) || /^[A-Za-z0-9 ]+$/i.test(value);
    }, "Special characters not allowed");
    jQuery.validator.addMethod("minlength", function (value, element) {
        return this.optional(element) || value.length == 10 || (value.length > 10 && value.length <= 15)
    }, "Minimum 10 or max 15 digits required.");
    jQuery.validator.addMethod("phonelength", function (value, element) {
        return this.optional(element) || value.length == 10 || (value.length > 10 && value.length <= 15)
    }, "Minimum 10 and max 10 digits required.");
    jQuery.validator.addMethod("greaterofzero", function (value, element) {
        return this.optional(element) || value > 0;
    }, "Value should be greater than 0.");

    jQuery.validator.addMethod("customRegex", function (value, element, param) {
        return this.optional(element) || param.test(value);
    }, "Please enter a valid value.");
}

$(function () {
    $('.datepicker').attr("autocomplete", "off");
});
$(document).ajaxComplete(function (event, xhr, settings) {
    setTimeout(function () {
        $('.datepicker,.dpicker').attr("autocomplete", "off");
    }, 2000);
});


// Set and get cookie
function setCookie(key, value, expiry) {
    var expires = new Date();
    expires.setTime(expires.getTime() + (expiry * 1 * 60 * 60 * 1000));
    document.cookie = key + '=' + value + ';expires=' + expires.toUTCString();
}

function getCookie(key) {
    var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
    return keyValue ? keyValue[2] : null;
}

function eraseCookie(key) {
    var keyValue = getCookie(key);
    setCookie(key, keyValue, '-1');
}


function formatJsonDate(date) {
    var myNewDate = new Date(parseInt(date.substr(6)));
    return (myNewDate.getMonth() + 1) + "/" + myNewDate.getDate() + "/" + myNewDate.getFullYear()
}


function randomGenerator(length = 12) {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return result;
}

function getFormInput(formId, inputName) {
    return $('#' + formId).find(`input[name=${inputName}]`)
}

var getFormInputByName = function (formId) {
    return (name) => getFormInput(formId, name);
}