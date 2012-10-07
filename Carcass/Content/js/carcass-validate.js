!function ($) {

    // Add validation methods for unsigned/signed integers
    $.validator.addMethod("unsignedInt", function (value, element) {
        return (this.optional && this.optional(element))
            || /^(?:\d+|\d{1,3}(?:[, ]\d{3})+)$/.test(value);
        }, "Please enter an integer value."
    );

    $.validator.addMethod("signedInt", function (value, element) {
        return (this.optional && this.optional(element))
            || /^[\-\+]?(?:\d+|\d{1,3}(?:[, ]\d{3})+)$/.test(value);
        }, "Please enter an integer value, sign allowed."
    );

    // connect them to a css classes
    $.validator.addClassRules({
        unsignedInt: { unsignedInt: true },
        signedInt: { signedInt: true }
    });

}(window.jQuery);