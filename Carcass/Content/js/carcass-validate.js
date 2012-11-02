!function ($, cs, undefined) {
    if (!cs || !cs.util)
        return alert('carcass-util.js required')

    cs.loadNumberValue = function (element, value) {
        /// parse local numeric value
        var $element = $(element),
            decimalSep = $element.data('num-decimal-separator') || false,
            groupSep = $element.data('num-group-separator') || '';

        value = String(value).replace(groupSep, '');
        if (decimalSep !== false)
            value = value.replace(decimalSep, '.');

        return parseFloat(value);
    }

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
                   && (value == datepicker.formatDate(datepicker.date)
                       || ('0' + value) == datepicker.formatDate(datepicker.date));
        } else {
            return !/Invalid|NaN/.test(new Date(value));
        }
    };

    var _originalTimeValidator = $.validator.methods['time'];
    
    $.validator.methods['time'] = function (value, element) {
        if (this.optional(element))
            return true;
        
        var $element = $(element),
            timepicker = $element.data('timepicker') || $element.parent().data('timepicker');
        if (timepicker) {
            var valid = timepicker.parseTime(value).valueOf() == timepicker.time.valueOf()
                    && (value == timepicker.formatTime(timepicker.time)
                        || value == '0' + timepicker.formatTime(timepicker.time));
            return valid;

        } else if (_originalTimeValidator) {
            return _originalTimeValidator.call(this, value, element);
        } else {
            return true;
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
            return _originalNumberValidator ? _originalNumberValidator.call(this, value, element) : true;

        if (!validator) {
            decimalSep = cs.escapeRegex(decimalSep);
            groupSep = cs.escapeRegex(groupSep);
            var re = RegExp("^-?(?:\\d+|\\d{1,3}(?:" + groupSep + "\\d{3})+)(?:" + decimalSep + "\\d+)?$");
            $element.data('num-validator', validator = re);
        }
        
        return validator.test(value);
    };
    
    $.validator.methods['range'] = function (value, element, param) {
        value = cs.loadNumberValue(element, value);
        return this.optional(element) || (value >= param[0] && value <= param[1]);
    },

    // connect them to a css classes
    $.validator.addClassRules({
        "unsigned_int": { unsigned_int: true },
        "signed_int": { signed_int: true },
        "currency": { number: true }
    });

    $(document).ready(function () {
        
    });

}(window.jQuery, window.Carcass, undefined);