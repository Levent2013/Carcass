!function ($) {
    window.CS = window.CS || {};
    $.extend(window.CS,
    {

    });


    // initialize Carcass forms components

    // TODO: modify date validator instead
    $('.dateValue, .dateValue input').removeData('val').removeData('val-date');
    $('.dateValue').datepicker();
    
}(window.jQuery);