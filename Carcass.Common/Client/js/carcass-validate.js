!function ($) {
    
    function escapeRegex(pattern) {
        if(!pattern)
            return pattern;
        var res = String(pattern),
            escapables = ['.', '?', '+', '\\'],
            escapes = ['\\.', '\\?', '\\+', '\\\\'];
        
        for (var ndx = 0; ndx < escapables.length; ++ndx)
            res = res.replace(escapables[ndx], escapes[ndx]);

        return res;
    }
    
    // Add validation methods for unsigned/signed integers
    $.validator.addMethod("unsignedInt", function (value, element) {
        return (this.optional && this.optional(element))
            || /^(?:\d+|\d{1,3}(?:[, ]\d{3})+)$/.test(value);
        }, "Please enter a positive integer value."
    );

    $.validator.addMethod("signedInt", function (value, element) {
        return (this.optional && this.optional(element))
            || /^[\-\+]?(?:\d+|\d{1,3}(?:[, ]\d{3})+)$/.test(value);
        }, "Please enter an integer value, sign allowed."
    );

    $.validator.methods['date'] = function (value, element) {
        if (this.optional(element))
            return true;

        var $element = $(element),
            datepicker = $element.data('datepicker') || $element.parent().data('datepicker');

        if (datepicker) {
            return datepicker.parseDate(value).valueOf() == datepicker.date.valueOf()
                    && value == datepicker.formatDate(datepicker.date);
        } else {
            return !/Invalid|NaN/.test(new Date(value));
        }
    };

    var _originalNumberValidator = $.validator.methods['number'];

    $.validator.methods['number'] = function (value, element) {
        if (this.optional(element))
            return true;

        var $element = $(element);
        if ($element.hasClass('unsignedInt') || $element.hasClass('signedInt'))
            return true; // value will be checked by appropriate rule

        var validate = $element.data('carcass-val') && true,
            decimalSep = $element.data('num-decimal-separator') || "",
            groupSep = $element.data('num-group-separator') || "",
            validator =  $element.data('num-validator');

        if (!validate)
            return _originalNumberValidator.call(this, value, element);
        if (!validator) {
            decimalSep = escapeRegex(decimalSep);
            groupSep = escapeRegex(groupSep);
            var re = RegExp("^-?(?:\\d+|\\d{1,3}(?:" + groupSep + "\\d{3})+)(?:" + decimalSep + "\\d+)?$");
            $element.data('num-validator', validator = re);
        }

        return validator.test(value);
    };

    

    // connect them to a css classes
    $.validator.addClassRules({
        "unsignedInt": { unsignedInt: true },
        "signedInt": { signedInt: true }
    });

    $(document).ready(function () {
      
    });


}(window.jQuery);