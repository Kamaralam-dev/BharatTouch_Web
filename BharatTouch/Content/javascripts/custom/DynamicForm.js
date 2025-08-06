/*
Dynamic Http Form for Posting Data
By Roger,Nov.26,2012
*/
function DynamicForm() {
    var _me = this;
    var _formName, _url, _target;
    var _data = new Array();
    var _method = "Post";

    this.Method = function (val) {
        _method = val;
    }

    this.Init = function (formName, url, target) {
        _formName = formName;
        _url = url;
        _target = target;
    }

    this.GetAllData = function () {
        return _data;
    }

    this.AddData = function (key, value) {
        _data.push({ Key: key, Value: value });
    }

    this.Remove = function (key) {
        $(_data).each(function () {
            if (i.Key == key)
                i = null;
        });
    }

    this.GetValue = function (key) {
        $(_data).each(function () {
            if (i.Key == key)
                return i.Value;
        });
    }

    this.SetValue = function (key, newVal) {
        $(_data).each(function () {
            if (i.Key == key)
                i.Value = newVal;
        });
    }

    this.ToString = function () {
        var form = '<form method="' + _method + '" action="' + _url + '" name="' + _formName + '" id="' + _formName + '" target="' + _target + '">';
        $(_data).each(function () {
            form += '<input type="hidden" name="' + this.Key + '" value="' + this.Value + '" />';
        });
        form += '</form>';

        return form;
    }

    this.Submit = function () {
        $('#' + _formName).remove();
        $('body').append(_me.ToString());
        $('#' + _formName).submit();
    }
}