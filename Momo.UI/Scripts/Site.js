/******************************************************/
/******************************************************/
// prototypes

String.prototype.format = function() {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function(match, number) {
        return typeof args[number] != 'undefined' ? args[number] : match;
    });
};


/******************************************************/
/******************************************************/
// jquery extensions

$.fn.setToSizeOfWindow = function () {
    return this.each(function () {
        $(this).css({ width: $(window).width(), height: $(document).height() });
    });
};

$.fn.centerInScreen = function (width, height) {
    width = width || '85%';
    height = height || '85%';
    return this.each(function () {
        var element = $(this);
        element.css({ width: width, height: height });
        var top = ($(window).height() - element.outerHeight()) / 2;
        var left = ($(window).width() - element.outerWidth()) / 2;
        element.css({ top: top, left: left });
    });
};

$.fn.resetUnobtrusiveValidation = function () {
    return this.each(function () {
        var form = $(this);
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        form.find('[data-valmsg-summary=true]').addClass('validation-summary-valid').removeClass('validation-summary-errors').find('ul').empty();
        form.find('.input-validation-error').removeClass('input-validation-error');
        $.validator.unobtrusive.parse(form);
    });
};

$.fn.appendValidationErrors = function (errors) {
    return this.each(function () {
        var container = $(this).find('[data-valmsg-summary=true]'), list = container.find('ul');
        container.addClass('validation-summary-errors').removeClass('validation-summary-valid');
        $.each(errors, function (i, val) {
            $('<li />').html(val).appendTo(list);
        });
    });
};

$.fn.toObject = function () {
    /// <summary>Only call on form elements or form input elements</summary>

    var obj = {};

    this.each(function () {
        $.map($(this).serializeArray(), function (n) {
            obj[n.name] = n.value;
        });
    });

    return obj;
};



/******************************************************/
/******************************************************/
// application namespace

app = {
    webroot: '/', // override in _Layout for dynamic web root

    modules: {}, // add modules here for consistency and convenience in the debugger

    debug: function(message) {
        if (window.console && 'debug' in window.console)
            console.debug(message);
        else if (window.console && 'log' in window.console)
            console.log(message);
        else
            $('<div>').text(message).appendTo('body');
        return this;
    },

    overlay: (function () {
        var _overlayDiv;
        function getOverlayDiv() {
            if (!_overlayDiv)
                _overlayDiv = $('<div>').css({ position: 'absolute', top: '0', left: '0', 'z-index': '100', 'background-color': '#000', cursor: 'wait', filter: 'alpha(opacity=85)', '-moz-opacity': '0.85', opacity: '0.25', display: 'none' }).appendTo('body');
            return _overlayDiv;
        }

        return {
            show: function () { getOverlayDiv().setToSizeOfWindow().show(); return this; },
            hide: function () { getOverlayDiv().hide(); return this; },
            fadeIn: function (callback) { getOverlayDiv().setToSizeOfWindow().fadeIn(function () { (callback || function () { })(); }); return this; },
            fadeOut: function (callback) { getOverlayDiv().fadeOut(function () { (callback || function () { })(); }); return this; }
        };
    })(),

    post: function (url, data, callback) {
        $.post(url, $.extend({}, { '__RequestVerificationToken': $('[name="__RequestVerificationToken"]').val() }, data), function (returnedData) {
            if (typeof callback === "function") {
                callback(returnedData);
            }
        });
    },

    Delayed: function (callback, millisecondsToDelay) {
        /// <summary>
        ///     Use this to delay executing some code for specified period of time.
        ///     Useful in key events where you don't want events firing until there
        ///     is a pause in the typing.
        /// </summary>

        if (!(this instanceof app.Delayed)) throw 'call new';

        var timeout;

        this.execute = function () {
            window.clearTimeout(timeout);
            var _this = this;
            var args = arguments || [];
            timeout = window.setTimeout(function () { callback.apply(_this, args); }, millisecondsToDelay || 500);
        };

        this.cancel = function () {
            window.clearTimeout(timeout);
        };
    }
};

(function($) {

    var $window = $(window), $document = $(document);

    $document.on('click', '.smooth-scroll', function(e) {
        e.preventDefault();
        $('html, body').animate({ scrollTop: $(this.hash).offset().top }, 800);
    });

    $document.on('click', '.footer-toggle', function (e) {
        e.preventDefault();
        $('.site-footer').slideToggle();
        this.blur();
    });

    $(function() {

        function setSize() {
            $('.screenSize').text($window.width() + ' x ' + $window.height() + '  (320 x 421 - iphone)');
        }

        setSize();
        $window.resize(setSize);
        $document.on('pageshow', '.ui-page', setSize);

    });

    /******************************************************/
    // shoppinglists-show
    (function() {

        $document.on({ pageinit: onInit, pageshow: onShow }, '.shoppinglists-show');

        function onInit() {
            $(this).on('change', '[data-item-id]', function() {
                var cb = $(this);
                app.post(document.URL + '/changepicked', { id: cb.data('item-id'), picked: cb.is(':checked') });
                cb.parents('.ui-focus').removeClass('ui-focus');
            });

            $(this).on('click', '.ui-li-link-alt', function (e) {
                var editLink = $(this),
                    li = editLink.parents('li:first'),
                    popup = $('#edit-item-container'),
                    form = popup.find('form');

                form.attr('action', editLink.attr('href'));
                form.find('#Id').val(li.data('item-id'));
                form.find('#Name').val(li.data('item-name'));
                form.find('#Quantity').val(li.data('item-quantity'));
                form.find('#Aisle').val(li.data('item-aisle'));
                form.find('#Price').val(li.data('item-price'));
                form.resetUnobtrusiveValidation();

                popup.show().popup('open');
                setTimeout(function () { form.find('#Aisle').focus().select(); }, 100);

                this.blur();
                e.preventDefault();
                return false;
            });

            $('#edit-item-container').find('form').submit(function () {
                var form = $(this);

                if (form.valid()) {
                    app.post(form.attr('action'), form.toObject(), function (result) {
                        if (typeof result === 'string') {
                            $('#items-container').empty().html(result).trigger('create');
                            $('#edit-item-container').popup('close');
                        } else {
                            app.debug(result.Errors);
                            form.appendValidationErrors(result.Errors);
                        }
                    });
                }

                return false;
            });
        }

        function onShow() {
        }

    })();

    /******************************************************/
    // shoppinglists-additem
    (function() {

        $document.on({ pageinit: onInit, pageshow: onShow }, '.shoppinglists-additem');

        function onInit() {
            var page = $(this),
                suggestions = $('#suggestions', page),
                nameInput = suggestions.prev(),
                getSuggestions = new app.Delayed(function () {
                    var text = $(this).val();
                    if (text.length < 2) {
                        suggestions.html('');
                        suggestions.listview('refresh');
                    } else {
                        app.post(suggestions.data('getsuggestionsurl'), { search: text }, function (result) {
                            suggestions.empty();
                            suggestions.append($.map(result, function (value) {
                                return $('<li>')
                                    .attr('data-icon', 'false')
                                    .attr('data-listitem-name', value.Name)
                                    .attr('data-listitem-quantity', value.Quantity == 0 ? 1 : value.Quantity)
                                    .attr('data-listitem-aisle', value.Aisle)
                                    .attr('data-listitem-price', value.Price)
                                    .append($('<a href="#">').text(value.Name))
                                    .get();
                            }));
                            suggestions.listview('refresh');
                        });
                    }
                });

            $('input[type="text"]', page).attr('autocomplete', 'off');

            nameInput.on('input', getSuggestions.execute);

            suggestions.on('click', 'li', function () {
                var li = $(this);

                $('#Name', page).val(li.data('listitem-name'));
                $('#Quantity', page).val(li.data('listitem-quantity')).focus().select();
                $('#Aisle', page).val(li.data('listitem-aisle'));
                $('#Price', page).val(li.data('listitem-price'));

                suggestions.empty();
                suggestions.listview('refresh');
            });
        }

        function onShow() {
            var name = $('#Name', this);
            $('#suggestions', this).css({
                'position': 'absolute',
                'top': name.position().top + name.innerHeight(),
                'left': name.position().left,
                'width': name.outerWidth(),
                'z-index': 1,
                'background': '#efefef'
            });
            setTimeout(function() { name.focus(); }, 10);
        }

    })();

    /******************************************************/
    // shoppinglists-edititem
    (function() {

        $document.on({ pageinit: onInit, pageshow: onShow }, '.shoppinglists-edititem');

        function onInit() {
            $('input[type="text"]', this).attr('autocomplete', 'off');
        }

        function onShow() {
            var aisle = $('#Aisle', this);
            setTimeout(function () { aisle.focus().select(); }, 10);
        }

    })();

})(jQuery);
