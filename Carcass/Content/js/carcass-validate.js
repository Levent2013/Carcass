!function ($) {

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

    // connect them to a css classes
    $.validator.addClassRules({
        "unsignedInt": { unsignedInt: true },
        "signedInt": { signedInt: true }
    });

    $(document).ready(function () {
      
    });


}(window.jQuery);