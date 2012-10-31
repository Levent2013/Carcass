
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

        // TinyMCE HTML Editor could be used for HTML Editors
        /*
        if ($.fn.tinymce) {
            $('textarea.html-editor').tinymce({
                // Location of TinyMCE script
                script_url: cs.BaseUrl + 'Content/tinymce/tiny_mce.js',
                theme: "advanced",
                skin: "carcass",
                skin_variant: "silver",
                plugins: "spellchecker,pagebreak,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

                theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,undo,redo,",
                theme_advanced_buttons2: "formatselect,fontselect,fontsizeselect",
                theme_advanced_buttons3: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote",
                theme_advanced_buttons4: "link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",
                theme_advanced_buttons5: "tablecontrols",
                theme_advanced_buttons6: "charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen",
                
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: true
            });
        }
        */

        $('textarea.html-editor')
            .addClass('input-block-level')
            .each(function () {
                $(this).wysihtml5({
                    "color": true,
                    "html": true,
                    "miniButtons": true,
                    stylesheets: [cs.BaseUrl + "Content/css/wysiwyg-color.css"]
                })
            });

            // Fix Bootstrap dropdowns on Android
        $(document).on('touchstart.dropdown', '.dropdown-menu', function(e) { e.stopPropagation(); });
            
    });
    
}(window.jQuery, window.Carcass, window);
