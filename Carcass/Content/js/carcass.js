!function ($) {
    window.CS = window.CS || {};
    $.extend(window.CS,
    {

    });

    
    // initialize Carcass forms components
    $(document).ready(function () {
        $('.input-append.date, .input-prepend.date, input.date').datepicker({ "autoclose": true, "todayBtn": true });
        $('.input-append.time, .input-prepend.time, input.time').timepicker({ "autoclose": true });
    });
    

    // jQuery helpers
    $.extend({
        format: function (s, params, formatFun) {
            /// Works with simple numeric {0}, {1} params and named params {name}, {type}
            s = String(s);
            if (!s.length) return s;

            var _defaultFormatFun = function (val) {
                if (val == undefined || val == null)
                    return '';
                if (typeof (val) == 'string') {
                    return val;
                } else if (typeof (val) == 'number') {
                    return val;
                } else if (typeof (val) == 'object') {
                    // format Date as dd-MM-yyyy HH:mm
                    if (val instanceof Date) {
                        var dt = String(val.getDate()), mon = String(val.getMonth() + 1), year = val.getFullYear();
                        var h = String(val.getHours()), m = String(val.getMinutes());
                        if (dt.length == 1) dt = "0" + dt;
                        if (mon.length == 1) mon = "0" + mon;
                        if (h.length == 1) h = "0" + h;
                        if (m.length == 1) m = "0" + m;
                        return dt + '-' + mon + '-' + year + ' ' + h + ':' + m;
                    } else if (val instanceof Array) {
                        return val.join(',');
                    } else {
                        throw "Object serialization is not supported";
                    }
                } else {
                    return String(val);
                }
            }

            if (typeof (params) != 'object') {
                params = Array.prototype.slice.call(arguments, 1);
                formatFun = _defaultFormatFun;
            } else if (typeof (formatFun) != 'function') {
                formatFun = _defaultFormatFun;
            }

            for (var key in params)
                s = s.replace(new RegExp("(^|[^\\{])\\{" + key + "\\}([^\\}]|$)", "ig"), "$1" + formatFun(params[key]) + "$2");
            s = s.replace(new RegExp("(^|[^\\{])\\{[0-9a-z_]+\\}([^\\}]|$)", "ig"), "$1$2"); // remove optional values
            return s;
        },
        parseError: function (ex) {
            if (typeof (ex) == 'string')
                return ex;
            var msg = ex.message || ex.description || null;
            var code = ex.code || ex.number || -1;
            return msg ? '[' + ($.browser.msie ? this.hexString(code) : code) + "] " + msg : String(ex);
        },
        parseErrorMessage: function (ex) {
            if (typeof (ex) == 'string') return ex;
            return ex.message || ex.description || String(ex);
        },
        encodeHtml: function (str) {
            var res = str || "", escapables = ["&", "<", ">", "©"], escapes = ["&amp;", "&lt;", "&gt;", "&copy;"];
            for (var ndx = 0; ndx < escapables.length; ++ndx)
                res = res.replace(new RegExp(escapables[ndx], "g"), escapes[ndx]);
            return res;
        },
        encodeXml: function (str) {
            var res = str || "", escapables = ['&', '<', '>', '"', "'"], escapes = ["&amp;", "&lt;", "&gt;", "&quot;", "&apos;"];
            for (var ndx = 0; ndx < escapables.length; ++ndx)
                res = res.replace(new RegExp(escapables[ndx], "g"), escapes[ndx]);
            return res;
        },
        decodeHtml: function (str) {
            var res = str || "", escapables = ["&lt;", "&gt;", "&amp;"], escapes = ["<", ">", "&"];
            for (var ndx = 0; ndx < escapables.length; ++ndx)
                res = res.replace(new RegExp(escapables[ndx], "g"), escapes[ndx]);
            return res;
        },
        getScrollBarWidth: function () {
            if (!jQuery.__ScrollBarWidth) {
                if ($.browser.msie) {
                    var $textarea1 = $('<textarea cols="10" rows="2"></textarea>')
                            .css({ position: 'absolute', top: -1000, left: -1000 }).appendTo('body'),
                        $textarea2 = $('<textarea cols="10" rows="2" style="overflow: hidden;"></textarea>')
                            .css({ position: 'absolute', top: -1000, left: -1000 }).appendTo('body');
                    jQuery.__ScrollBarWidth = $textarea1.width() - $textarea2.width();
                    $textarea1.add($textarea2).remove();
                } else {
                    var $div = $('<div />')
                        .css({ width: 100, height: 100, overflow: 'auto', position: 'absolute', top: -1000, left: -1000 })
                        .prependTo('body').append('<div />').find('div')
                            .css({ width: '100%', height: 200 });
                    jQuery.__ScrollBarWidth = 100 - $div.width();
                    $div.parent().remove();
                }
            }
            return jQuery.__ScrollBarWidth;
        }
    });
    
}(window.jQuery);