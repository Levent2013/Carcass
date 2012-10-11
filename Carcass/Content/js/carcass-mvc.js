
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
            datepicker = $('.datepicker-control', $control).data('datepicker'),
            timepicker = $('.timepicker-control', $control).data('timepicker')
            value = '';
            if (datepicker)
                value += datepicker.getValue() || '';
            value += cs.DateTimeSeparator;
            if (timepicker)
                value += timepicker.getValue() || '';

        $('input[type=hidden]', $control).prop('value', value);
    }
    
    // initialize Carcass forms components
    $(document).ready(function () {
        $('.input-append.datepicker-control, .input-prepend.datepicker-control').datepicker({ "autoclose": true, "todayBtn": true, "language": cs.Language });
        $('.input-append.timepicker-control, .input-prepend.timepicker-control').timepicker({ "autoclose": true, "by5minutes": true, "language": cs.Language });
        $('.datetime input[type=text]').bind('change', _dateTimeLoad);
    });
    
}(window.jQuery, window.Carcass, window);
