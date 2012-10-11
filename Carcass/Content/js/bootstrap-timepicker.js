/* =========================================================
 * Timepicker for Bootstrap 
 * =========================================================
 * Main idea got from bootstrap-datepicker.js
 * http://www.eyecon.ro/bootstrap-datepicker
 * ========================================================= */

!function( $, cs ) {
    if (!cs || !cs.util)
        return alert('carcass-util.js required')

    var messages = {
        en: { am: "am", pm: "pm", now: "Now", hour: 'Hour', minute: 'Minute' }
    }
    
    // Picker object
    var Timepicker = function(element, options) {
		var that = this;

		this.element = $(element);
		this.language = options.language || this.element.data('time-language') || "en";
		this.language = this.language in messages ? this.language : "en";
		this.format = Globals.parseFormat(options.format || this.element.data('time-format') || 'h:mm tt');
        this.isInput = this.element.is('input');
        this.component = this.element.is('.timepicker-control') ? this.element.find('.add-on') : false;
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

		this.by5minutes = false;
		if ('by5minutes' in options)
		    this.by5minutes = options.by5minutes;
		else if ('timeBy5minutes' in this.element.data())
		    this.by5minutes = this.element.data('time-by-5minutes');
		
		this.picker = $(Globals.formatTemplate(this))
							.appendTo('body')
							.on({ click: $.proxy(this.click, this) });

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

		_setValue: function () {
		    var formatted = Globals.formatTime(this.time, this.format, this.language);
			if (!this.isInput) {
				this.element.find('input').prop('value', formatted);
			} else {
				this.element.prop('value', formatted);
			}
		},

		getValue: function () {
		    return this.component
                ? this.element.find('input').prop('value')
		        : this.element.prop('value');
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
		    var value = this.isInput ? this.element.prop('value')
                    : this.element.find('input').prop('value');
		    this.time = Globals.parseTime(value, this.format, this.language);
		    this.viewTime = value ? this.time : null;
			this.fill();
		},

		fill: function(skipActivation) {
		    skipActivation = skipActivation && true;
		    var d = null, h = -1, m = -1, s = -1;

		    if (this.viewTime) {
		        d = new Date(this.viewTime),
                    h = d.getHours(),
                    m = d.getMinutes(),
                    s = d.getSeconds();
		    }

		    if (!skipActivation || !d) {
		        $('.active', this.picker).removeClass('active');
		        if (d) {
		            $('.hour[data-value=' + h + ']', this.picker).addClass('active');
		            $('.minute[data-value=' + m + ']', this.picker).addClass('active');
		        }
		    }
		},

		click: function(e) {
			e.stopPropagation();
			e.preventDefault();
			var target = $(e.target).closest('.timeitem'),
		        picker = target.parents('.timepicker:first');
			
		    var d = new Date(this.viewTime || this.time),
               curHour = d.getHours(),
               curMinute = d.getMinutes(),
               curSecond = d.getSeconds();

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
		    tm.setHours(h || 0);
		    tm.setMinutes(m || 0);
		    tm.setSeconds(s || 0);
		    Globals.resetDate(tm);

		    this.time = this.viewTime = tm;

			this.fill(true);
			this._setValue();
			this.element.trigger({ type: 'changeTime', time: this.time });
			var element = this.isInput ? this.element : this.element.find('input');
			if (element.length) {
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
	    validParts: /hh?|HH?|mm?|MM?|tt?|TT?/g,
		nonpunctuation: /[^ -\/:-@\[-`{-~\t\n\r]+/g,

		parseFormat: function (format) {
			// IE treats \0 as a string end in inputs (truncating the value),
		    // so it's a bad format delimiter, anyway
		    format = String(format).toLowerCase();
			var separators = format.replace(this.validParts, '\0').split('\0'),
				parts = format.match(this.validParts);
			if (!separators || !separators.length || !parts || parts.length == 0)
				throw new Error("Invalid time format.");

			var is24h = true;
			for (var i = 0; i < parts.length; ++i) {
			    if (parts[i].match(/tt?/i)) {
			        is24h = false;
			        break;
			    }
			}
				
			return {
			    "separators": separators, 
			    "parts": parts, 
			    "is24h": is24h
			};
		},

		resetDate: function (date) {
		    date.setFullYear(2001);
		    date.setMonth(1);
		    date.setDate(1);
		    date.setSeconds(0);
		    date.setMilliseconds(0);
		},

		parseTime: function (time, format, language) {
		    if (time instanceof Date) return time;
			
		    var parts = time && time.match(this.nonpunctuation) || [],
				date = new Date(),
				parsed = {},
				setters_order = ['hh', 'h', 'mm', 'm', 's', 'ss', 'tt', 't'],
				setters_map = {
				    h: function (d, v) { return d.setHours(v); },
				    m: function(d,v) { return d.setMinutes(v); },
				    s: function (d, v) { return d.setSeconds(v); },
				    t: function (d, v) {
				        v = String(v).toLowerCase();
				        if (v == messages[language].am) {
				            if (d.getHours() == 12)
				                return d.setHours(0);
				        } else {
				            var h = d.getHours();
				            if (h < 12)
				                return d.setHours(h + 12);
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
			this.resetDate(date);
			
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

		formatTime: function (date, format, language) {
		    if (!date)
		        return '';

		    var hh = date.getHours(),
                mm = date.getMinutes(),
                ss = date.getSeconds(),
                tt = '',
                localMessages = $.fn.timepicker.messages[language];

		    if (!format.is24h) {
		        if (h < 12) {
		            tt = localMessages.am;
		            if (h == 0)
		                h = 12;
		        } else {
		            tt = localMessages.pm;
		            if (h > 12)
		                h -= 12;
		        }
		    }

		    var val = {
		        "hh": hh, "h": hh,
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

		HeadTemplate: '<thead>'+
							'<tr>'+
								'<th colspan="2" class="hour-header">{hour}</th>' +
                                '<th class="separator"></th>' +
                                '<th colspan="5" class="minute-header">{minute}</th>' +
							'</tr>'+
						'</thead>',
		HeadTemplate5Min: '<thead>' +
							'<tr>' +
								'<th colspan="4" class="hour-header">{hour}</th>' +
                                '<th class="separator"></th>' +
                                '<th colspan="2" class="minute-header">{minute}</th>' +
							'</tr>' +
						'</thead>',
		FootTemplate: '<tfoot><tr><th colspan="8" class="now"></th></tr></tfoot>',

		formatTemplate: function (timepicker) {
            var cells = [],
		        localMessages = $.fn.timepicker.messages[timepicker.language];

            function formatAmPmHour(value, pm) {
                var text = String(value);
                if (!pm && x == 0 && value == 0)
                    text = 12;
                else if (pm && value > 12)
                    text = value - 12;

                text += cs.encodeHtml(pm ? localMessages.pm : localMessages.am);
                return text;
            }

            if (timepicker.by5minutes) {

                for (var y = 0; y < 6; ++y) {
                    cells.push('<tr>');
                    for (var x = 0; x < 7; ++x) {
                        if (x == 4) {
                            cells.push('<td class="separator">');
                        } else {
                            var value = x < 4 ? (x * 6 + y) : (y * 5 + (x - 5) * 30);
                            cells.push(cs.format('<td class="timeitem {0}" data-value="{1}">',
                                x < 4 ? 'hour' : 'minute',
                                value));

                            var text = value;
                            if (!timepicker.format.is24h && x < 4)
                                text = formatAmPmHour(value, x > 1);
                            
                            cells.push(text);
                        }

                        cells.push('</td>');
                    }
                    cells.push('</tr>');
                }

            } else {
                for (var y = 0; y < 12; ++y) {
                    cells.push('<tr>');
                    for (var x = 0; x < 8; ++x) {
                        if (x == 2) {
                            cells.push('<td class="separator">');
                        } else {
                            var value = x < 2 ? (x == 0 ? y : 12 + y) : (y + (x - 3) * 12);
                            cells.push(cs.format('<td class="timeitem {0}" data-value="{1}">',
                                x < 2 ? 'hour' : 'minute',
                                value));

                            var text = value;
                            if (!timepicker.format.is24h && x < 2)
                                text = formatAmPmHour(value, x > 0);

                            cells.push(text);
                        }

                        cells.push('</td>');
                    }
                    cells.push('</tr>');
                }
            }

		    return '<div class="timepicker dropdown-menu">' +
                        '<table class="table-condensed">' +
                            cs.format(timepicker.by5minutes ? Globals.HeadTemplate5Min : Globals.HeadTemplate, localMessages) +
                            '<tbody>' +
                            cells.join('') +
                            '</tbody>' +
                            cs.format(Globals.FootTemplate, localMessages) +
                        '</table>' +
                    '</div>';
		}
	};
}(window.jQuery, window.Carcass);
