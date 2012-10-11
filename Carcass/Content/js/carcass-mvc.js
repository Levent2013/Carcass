
window.Carcass = window.Carcass || {};

!function ($, cs, window) {
    // make global
    window.cs = cs;

    $.extend(cs,
    {
        mvc: true,
        DateTimeSeparator: " "
    });
    
    function _dateTimeLoad (e) {
        var $control = $(this).parents('.datetime:first'),
            datepicker = $('.date', $control).data('datepicker'),
            timepicker = $('.time', $control).data('timepicker'),
            value = $.trim((datepicker && datepicker.getValue()) + cs.DateTimeSeparator + (timepicker && timepicker.getValue()));

        $('input[type=hidden]', $control).prop('value', value);
    }
    
    // initialize Carcass forms components
    $(document).ready(function () {
        $('.input-append.date, .input-prepend.date, input.date').datepicker({ "autoclose": true, "todayBtn": true });
        $('.input-append.time, .input-prepend.time, input.time').timepicker({ "autoclose": true, "by5minutes": true });
        $('.datetime input[type=text]').bind('change', _dateTimeLoad);
    });
    
}(window.jQuery, window.Carcass, window);
