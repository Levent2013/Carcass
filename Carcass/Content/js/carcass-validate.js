!function ($, cs, undefined) {
    if (!cs || !cs.util)
        return alert('carcass-util.js required')

    // Add validation methods for unsigned/signed integers
    $.validator.addMethod("unsigned_int", function (value, element) {
        return (this.optional && this.optional(element))
            || /^(?:\d+|\d{1,3}(?:[, ]\d{3})+)$/.test(value);
        }, "Please enter a positive integer value."
    );

    $.validator.addMethod("signed_int", function (value, element) {
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
        if ($element.hasClass('unsigned_int') || $element.hasClass('signed_int'))
            return true; // value will be checked by appropriate rule

        var validate = $element.data('carcass-val') && true,
            decimalSep = $element.data('num-decimal-separator') || "",
            groupSep = $element.data('num-group-separator') || "",
            validator =  $element.data('num-validator');

        if (!validate)
            return _originalNumberValidator.call(this, value, element);

        if (!validator) {
            decimalSep = cs.escapeRegex(decimalSep);
            groupSep = cs.escapeRegex(groupSep);
            var re = RegExp("^-?(?:\\d+|\\d{1,3}(?:" + groupSep + "\\d{3})+)(?:" + decimalSep + "\\d+)?$");
            $element.data('num-validator', validator = re);
        }
        
        return validator.test(value);
    };

    

    // connect them to a css classes
    $.validator.addClassRules({
        "unsigned_int": { unsigned_int: true },
        "signed_int": { signed_int: true }
    });

    $(document).ready(function () {
      
    });

}(window.jQuery, window.Carcass, undefined);