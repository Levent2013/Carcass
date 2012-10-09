!function ($) {
    window.CS = window.CS || {};
    $.extend(window.CS,
    {
        DateTimeSeparator: " ",

        _dateTimeLoad: function (e) {
            var $control = $(this).parents('.datetime:first'),
                datepicker = $('.date', $control).data('datepicker'),
                timepicker = $('.time', $control).data('timepicker'),
                value = $.trim((datepicker && datepicker.getValue()) + CS.DateTimeSeparator + (timepicker && timepicker.getValue()));

            $('input[type=hidden]', $control).prop('value', value);
        }
    });

    
    // initialize Carcass forms components
    $(document).ready(function () {
        $('.input-append.date, .input-prepend.date, input.date').datepicker({ "autoclose": true, "todayBtn": true });
        $('.input-append.time, .input-prepend.time, input.time').timepicker({ "autoclose": true, "by5minutes": true });
        $('.datetime .date').bind('changeDate', CS._dateTimeLoad);
        $('.datetime .time').bind('changeTime', CS._dateTimeLoad);
    });
    
}(window.jQuery);