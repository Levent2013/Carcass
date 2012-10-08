/* =========================================================
 * Timepicker for Bootstrap 
 * =========================================================
 * Main idea got from bootstrap-datepicker.js
 * http://www.eyecon.ro/bootstrap-datepicker
 * ========================================================= */

!function( $ ) {

    var messages = {
        en: { am: "am", pm: "pm", now: "Now", hour: 'Hour', minute: 'Minute' }
    }
    
    // Picker object
    var Timepicker = function(element, options) {
		var that = this;

		this.element = $(element);
		this.language = options.language || this.element.data('time-language') || "en";
		this.language = this.language in messages ? this.language : "en";
		this.format = Globals.parseFormat(options.format||this.element.data('time-format')||'h:mm tt');
		this.picker = $(Globals.formatTemplate())
							.appendTo('body')
							.on({ click: $.proxy(this.click, this) });

		this.isInput = this.element.is('input');
		this.component = this.element.is('.time') ? this.element.find('.add-on') : false;
		this.hasInput = this.component && this.element.find('input').length;
		if(this.component && this.component.length === 0)
			this.component = false;

		if (this.isInput) {
			this.element.on({
				focus: $.proxy(this.show, this),
				keyup: $.proxy(this.update, this),
				keydown: $.proxy(this.keydown, this)
			});
		} else {
			if (this.component && this.hasInput){
				// For components that are not readonly, allow keyboard nav
				this.element.find('input').on({
					focus: $.proxy(this.show, this),
					keyup: $.proxy(this.update, this),
					keydown: $.proxy(this.keydown, this)
				});

				this.component.on('click', $.proxy(this.show, this));
			} else {
				this.element.on('click', $.proxy(this.show, this));
			}
		}

		$(document).on('mousedown', function (e) {
			// Clicked outside the timepicker, hide it
		    if ($(e.target).closest('.timepicker').length == 0) {
				that.hide();
			}
		});

		this.autoclose = false;
		if ('autoclose' in options) {
			this.autoclose = options.autoclose;
		} else if ('timeAutoclose' in this.element.data()) {
		    this.autoclose = this.element.data('time-autoclose');
		}

		this.keyboardNavigation = true;
		if ('keyboardNavigation' in options) {
			this.keyboardNavigation = options.keyboardNavigation;
		} else if ('timeKeyboardNavigation' in this.element.data()) {
		    this.keyboardNavigation = this.element.data('time-keyboard-navigation');
		}

		this.update();
	};

    Timepicker.prototype = {
		constructor: Timepicker,

		show: function(e) {
			this.picker.show();
			this.height = this.component ? this.component.outerHeight() : this.element.outerHeight();
			this.update();
			this.place();
			$(window).on('resize', $.proxy(this.place, this));
			if (e) {
				e.stopPropagation();
				e.preventDefault();
			}

			this.element.trigger({ type: 'show', time: this.time });
		},

		hide: function(e){
			this.picker.hide();
			$(window).off('resize', this.place);
			if (!this.isInput) 
				$(document).off('mousedown', this.hide);
			
			this.element.trigger({ type: 'hide', time: this.time });
		},

		setValue: function (value) {
		    var tm = value ? Globals.parseTime(value) : this.time;
		    var formatted = Globals.formatTime(tm, this.format, this.language);
			if (!this.isInput) {
				if (this.component){
					this.element.find('input').prop('value', formatted);
				}
				this.element.data('time', formatted);
			} else {
				this.element.prop('value', formatted);
			}
		},

	    parseTime: function (time) {
	        return Globals.parseTime(time, this.format, this.language);
		},

	    formatTime: function (time) {
	        return Globals.formatTime(time, this.format, this.language);
		},

		place: function(){
			var zIndex = parseInt(this.element.parents().filter(function() {
							return $(this).css('z-index') != 'auto';
						}).first().css('z-index')) + 10;
			var offset = this.component ? this.component.offset() : this.element.offset();
			this.picker.css({ top: offset.top + this.height, left: offset.left, zIndex: zIndex });
		},

		update: function () {
		    this.time = Globals.parseTime(
				this.isInput ? this.element.prop('value')
                    : this.element.find('input').prop('value') || this.element.data('time'),
				this.format, this.language
			);
		    
            
		    this.viewTime = this.time;
			this.fill();
		},

		fill: function(skipActivation) {
		    var d = new Date(this.viewTime),
				h = d.getUTCHours(),
				m = d.getUTCMinutes(),
                s = d.getUTCSeconds();
		    skipActivation = skipActivation && true;

		    // TODO: update cells text with 'am/pm' if needed

		    if (!skipActivation) {
		        $('.active', this.picker).removeClass('active');
		        $('.hour[data-value=' + h + ']', this.picker).addClass('active');
		        $('.minute[data-value=' + m + ']', this.picker).addClass('active');
		    }

		},

		click: function(e) {
			e.stopPropagation();
			e.preventDefault();
			var target = $(e.target).closest('.timeitem'),
		        picker = target.parents('.timepicker:first');
			
		    var d = new Date(this.viewTime || this.time),
               curHour = d.getUTCHours(),
               curMinute = d.getUTCMinutes(),
               curSecond = d.getUTCSeconds();

			if (target.length == 1) {
			    if (target.is('.hour') && !target.is('.disabled')) {
			        $('.active', picker).removeClass('active');
			        target.addClass('active');

			        var h = parseInt(target.data('value'), 10);
			        this._setTime(h, curMinute);
			    } else if (target.is('.minute') && !target.is('.disabled')) {
			        $('.minute.active', picker).removeClass('active');
			        target.addClass('active');

			        var m = parseInt(target.data('value'), 10);
			        this._setTime(curHour, m);
			    }
			}
		},

		_setTime: function (h, m, s) {
		    var tm = new Date();
		    tm.setUTCHours(h || 0);
		    tm.setUTCMinutes(m || 0);
		    tm.setUTCSeconds(s || 0);
		    
		    this.time = this.viewTime = tm;

			this.fill(true);
			this.setValue();
			this.element.trigger({ type: 'changeTime', time: this.time });

			var element;
			if (this.isInput) {
				element = this.element;
			} else if (this.component){
				element = this.element.find('input');
			}

			if (element) {
				element.change();
				if (this.picker.find('.active').length > 1 && this.autoclose)
                    this.hide();
			}
		},

		keydown: function(e){
			if (this.picker.is(':not(:visible)')){
				if (e.keyCode == 27) // allow escape to hide and re-show picker
					this.show();
				return;
			}

			switch(e.keyCode){
			    case 27: // escape
			    case 13: // enter
					this.hide();
					e.preventDefault();
					break;
				case 9: // tab
					this.hide();
					break;
			}
		}
	};

    $.fn.timepicker = function (option) {
		var args = Array.apply(null, arguments);
		args.shift();
		return this.each(function () {
			var $this = $(this),
				data = $this.data('timepicker'),
				options = typeof option == 'object' && option;
			if (!data) {
			    $this.data('timepicker', (data = new Timepicker(this, $.extend({}, $.fn.timepicker.defaults, options))));
			}
			if (typeof option == 'string' && typeof data[option] == 'function') {
				data[option].apply(data, args);
			}
		});
	};

	$.fn.timepicker.defaults = {};
	$.fn.timepicker.Constructor = Timepicker;
	$.fn.timepicker.messages = messages;
	
	var Globals = {
		validParts: /hh?|HH?|mm?|MM?/g,
		nonpunctuation: /[^ -\/:-@\[-`{-~\t\n\r]+/g,

		parseFormat: function (format) {
			// IE treats \0 as a string end in inputs (truncating the value),
			// so it's a bad format delimiter, anyway
			var separators = format.replace(this.validParts, '\0').split('\0'),
				parts = format.match(this.validParts);
			if (!separators || !separators.length || !parts || parts.length == 0)
				throw new Error("Invalid time format.");
			
			return {
			    "separators": separators, 
			    "parts": parts, 
			    "is24h": !('tt' in parts || 't' in parts)
			};
		},

		parseTime: function (time, format, language) {
			if (date instanceof Date) return date;
			
		    var parts = time && time.match(this.nonpunctuation) || [],
				date = new Date(),
				parsed = {},
				setters_order = ['hh', 'h', 'mm', 'm', 's', 'ss', 'tt', 't'],
				setters_map = {
				    h: function (d, v) { return d.setUTCHours(v); },
				    m: function(d,v) { return d.setUTCMinutes(v); },
				    s: function (d, v) { return d.setUTCSeconds(v); },
				    t: function (d, v) {
				        v = String(v).toLowerCase();
				        if (v == messages[language].am) {
				            if (d.getUTCHours() == 12)
				                return d.setUTCHours(0);
				        } else {
				            var h = d.getUTCHours();
				            if (h < 12)
				                return d.setUTCHours(h + 12);
				        }

				        return d;
				    }
				},
				val, filtered, part;

			setters_map['hh'] = setters_map['h'];
			setters_map['mm'] = setters_map['m'];
			setters_map['ss'] = setters_map['s'];
			setters_map['tt'] = setters_map['t'];

            // reset current date
			date.setUTCFullYear(2001);
			date.setUTCMonth(1);
			date.setUTCDate(1);

			if (parts.length == format.parts.length) {
				for (var i=0, cnt = format.parts.length; i < cnt; i++) {
				    part = format.parts[i];
				    val = part == 'tt' ? parts[i] : parseInt(parts[i], 10);
					parsed[part] = val;
				}

				for (var i=0, s; i<setters_order.length; i++){
					s = setters_order[i];
					if (s in parsed)
						setters_map[s](date, parsed[s])
				}
			}

			return date;
		},

		formatTime: function(date, format, language){
		    var hh = date.getUTCHours(),
                h = hh,
                mm = date.getUTCMinutes(),
                ss = date.getUTCSeconds(),
                tt = '';

		    if (!format.is24h) {
		        if (h < 12) {
		            tt = messages[language].am;
		            if (h == 0)
		                h = 12;
		        } else {
		            tt = messages[language].pm;
		            if (h > 12)
		                h -= 12;
		        }
		    }

		    var val = {
		        "hh": hh, "h": h,
		        "mm": mm, "m": mm,
		        "ss": ss, "s": ss,
                "tt": tt, "t": tt
		    };

		    val.ss = (val.ss < 10 ? '0' : '') + val.ss;
		    val.mm = (val.mm < 10 ? '0' : '') + val.mm;
		    val.hh = (val.hh < 10 ? '0' : '') + val.hh;

		    var time = [],
				seps = $.extend([], format.separators);
			for (var i=0, cnt = format.parts.length; i < cnt; i++) {
				if (seps.length)
				    time.push(seps.shift())
				time.push(val[format.parts[i]]);
			}

			return time.join('');
		},

		headTemplate: '<thead>'+
							'<tr>'+
								'<th colspan="2" class="hour-header">Hour</th>' +
                                '<th class="separator"></th>' +
                                '<th colspan="5" class="minute-header">Minute</th>' +
							'</tr>'+
						'</thead>',
		footTemplate: '<tfoot><tr><th colspan="7" class="now"></th></tr></tfoot>'
	};

	

	Globals.formatTemplate = function () {
	    var cells = [];
	    for (var y = 0; y < 12; ++y) {
	        cells.push('<tr>');
	        for (var x = 0; x < 8; ++x) {
	            if (x == 2) {
	                cells.push('<td class="separator">');
	            } else {
	                var value = x < 2 ? (x == 0 ? y : 12 + y) : (y + (x - 3) * 12);
	                cells.push($.format('<td class="timeitem {0}" data-value="{1}">',
                        x < 2 ? 'hour' : 'minute',
                        value));
	                cells.push(value);
	            }

	            cells.push('</td>');
	        }
	        cells.push('</tr>');
	    }

	    return '<div class="timepicker dropdown-menu">' +
                    '<table class="table-condensed">' +
                        Globals.headTemplate +
                        '<tbody>' +
                        cells.join('') +
                        '</tbody>' +
                        Globals.footTemplate +
                    '</table>' +
                '</div>';
	};
}( window.jQuery );
