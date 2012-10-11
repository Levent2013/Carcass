!function ($) {
    // Useful jQuery helpers
    $.extend({
        disable: function () {
            return $(this).each(function () {
                var item = $(this);
                item.addClass('disabled').attr('disabled', true);
            });
        },

        enable: function () {
            return $(this).each(function () {
                var item = $(this);
                item.removeClass('disabled').removeAttr('disabled');
            });
        },

        clientSize: function () {
            var dimensions = { width: $(document).width(), height: $(document).height() };
            if (typeof window.innerWidth != 'undefined') {
                dimensions.width = window.innerWidth;
                dimensions.height = window.innerHeight;

            } else {
                if (document.documentElement && typeof document.documentElement.clientWidth != 'undefined'
                    && document.documentElement.clientWidth != 0) {
                    dimensions.width = document.documentElement.clientWidth;
                    dimensions.height = document.documentElement.clientHeight;

                } else if (document.body && typeof document.body.clientWidth != 'undefined') {
                    dimensions.width = document.body.clientWidth;
                    dimensions.height = document.body.clientHeight;
                }
            }

            return dimensions;
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
    
}(window.jQuery);