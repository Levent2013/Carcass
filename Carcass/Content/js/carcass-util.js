window.Carcass = window.Carcass || {};

!function ($, cs, undefined) {

    function _defaultFormatFun(val) {
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
    
    $.extend(cs,
    {
        util: true,
        returnFalse: function () { return false; },
        returnTrue: function () { return true; },

        format: function (s, params, formatFun) {
            /// Works with simple numeric {0}, {1} params and named params {name}, {type}
            s = String(s);
            if (!s.length) return s;

            if (typeof (params) != 'object') {
                params = Array.prototype.slice.call(arguments, 1);
                formatFun = _defaultFormatFun;
            } else if (typeof (formatFun) != 'function') {
                formatFun = _defaultFormatFun;
            }

            for (var key in params)
                s = s.replace(new RegExp("(^|[^\\{])\\{" + key + "\\}([^\\}]|$)", "ig"), "$1" + formatFun(params[key]) + "$2");
            // remove optional values
            s = s.replace(new RegExp("(^|[^\\{])\\{[0-9a-z_]+\\}([^\\}]|$)", "ig"), "$1$2"); 
            return s;
        },
        
        dump: function(obj, opt, lv) {
            if (typeof(opt) === "number") opt = { maxLevel: opt };
            opt = $.extend({
                expandFunc: false,
                maxLevel: 2
            }, opt || {});

            if (lv === undefined) lv = 0;
            if (lv >= opt.maxLevel) return "";

            var prefix = "", l = lv;
            while (l--)
                prefix += "\t";
            if (obj == null)
                return prefix + "[null]";

            if (typeof (obj) == "object") {
                if (obj instanceof Date) {
                    return " " + obj.toString();
                    
                } else {
                    var isArray = obj instanceof Array;
                    var res = (lv ? "\r\n" + prefix + (isArray ? "[" : "{") : "");
                    var propsLoaded = false;

                    for (var item in obj) {
                        var dmp = this.dump(obj[item], opt, lv + 1);
                        res += "\r\n" + prefix + item + ": " + dmp;
                        propsLoaded = true;
                    }

                    if (!propsLoaded && typeof (obj.toString) == "function")
                        res += "\r\n" + obj.toString();
                    res += (lv ? "\r\n" + prefix + (isArray ? "]" : "}") : "");
                }
            
                return res;

            } else if (typeof (obj) == "function") {
                return (opt.expandFunc ? obj.toString() : "[function]");
            }
            return (prefix + String(obj));
        },

        escapeRegex: function (pattern)
        {
            if(!pattern)
                return pattern;
            var res = String(pattern),
                escapables = ['\\', '.', '?', '+', String.fromCharCode(160), ' ', '\t'],
                escapes = ['\\\\', '\\.', '\\?', '\\+', '\\s', '\\s', '\\s'];
    
            for (var ndx = 0; ndx < escapables.length; ++ndx)
                res = res.replace(escapables[ndx], escapes[ndx]);
            return res;
        },

        hexString: function (number) {
            if (number < 0) number = 0xFFFFFFFF + number + 1;
            return number.toString(16).toUpperCase();
        },

        parseError: function (ex) {
            if (typeof (ex) == 'string')
                return ex;
            var msg = ex.message || ex.description || null;
            var code = ex.code || ex.number || -1;
            return msg ? '[' + code + "] " + msg : String(ex);
        },

        parseErrorMessage: function (ex) {
            if (typeof (ex) == 'string') return ex;
            return ex.message || ex.description || String(ex);
        },

        encodeHtml: function (str) {
            var res = String(str),
                escapables = ["&", "<", ">", "©"],
                escapes = ["&amp;", "&lt;", "&gt;", "&copy;"];
            if (!res) return res;
            for (var ndx = 0; ndx < escapables.length; ++ndx)
                res = res.replace(escapables[ndx], escapes[ndx]);
            return res;
        },

        encodeXml: function (str) {
            var res = String(str),
                escapables = ['&', '<', '>', '"', "'"],
                escapes = ["&amp;", "&lt;", "&gt;", "&quot;", "&apos;"];
            if (!res) return res;
            for (var ndx = 0; ndx < escapables.length; ++ndx)
                res = res.replace(escapables[ndx], escapes[ndx]);
            return res;
        },

        decodeHtml: function (str) {
            var res = String(str),
                escapables = ["&lt;", "&gt;", "&amp;"],
                escapes = ["<", ">", "&"];
            if (!res) return res;
            for (var ndx = 0; ndx < escapables.length; ++ndx)
                res = res.replace(escapables[ndx], escapes[ndx]);
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
    
}(window.jQuery, window.Carcass, undefined);