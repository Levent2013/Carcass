!function ($) {
    // FileInput class definition
    var input = '.input-file input',
        FileInput = function (el) {
            $(el).on('change', input, this.showInfo);
        }


    function _getFileInputFiles(fileInput) {
        fileInput = $(fileInput);
        var entries = fileInput.prop('webkitEntries') || fileInput.prop('entries'),
            files, value;

        // TODO: Apply async _handleFileTreeEntries (from jQuery File Upload Plugin)
        // if (entries && entries.length)
        //      return _handleFileTreeEntries(entries);
        
        files = $.makeArray(fileInput.prop('files'));

        if (!files.length) {
            value = fileInput.prop('value');
            if (!value)
                return [];

            // Return name with path information removed:
            files = [{ name: value.replace(/^.*\\/, ''), size: -1}];
        }

        return files;
    };

    FileInput.prototype.showInfo = function (e) {
        var $this = $(this)
          , selector = $this.attr('data-target')
          , $info = selector ? $(selector) : $this.parent().parent().find('.input-file-info');

        if (!$info.length)
            return;

        var files = _getFileInputFiles(this),
            names = $.map(files, function (elem, ndx) { return elem.name; });
        $info.html('').text(names.join(', '));
    }

    // FileInput plugin definition
    $.fn.fileInput = function (option) {
        return this.each(function () {
            var $this = $(this),
                data = $this.data('fileInput');
            if (!data)
                $this.data('fileInput', (data = new FileInput(this)));
            if (typeof option == 'string')
                data[option].call($this);
        })
    }

    $.fn.fileInput.Constructor = FileInput
    
    // FileInput Data-API

    $(function () {
        $('body').on('change.input-file.data-api', input, FileInput.prototype.showInfo)
    })

}(window.jQuery);