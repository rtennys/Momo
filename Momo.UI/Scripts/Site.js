/******************************************************/
/******************************************************/
// prototypes

if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun /*, thisp*/) {
        var len = this.length >>> 0;
        if (typeof fun != "function")
            throw new TypeError();

        var res = [];
        var thisp = arguments[1];
        for (var i = 0; i < len; i++) {
            if (i in this) {
                var val = this[i]; // in case fun mutates this
                if (fun.call(thisp, val, i, this))
                    res.push(val);
            }
        }
        return res;
    };
}


/******************************************************/
/******************************************************/
// jquery extensions

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


/******************************************************/
/******************************************************/
// application namespace

app = {
    modules: {},

    urls: {},

    logger: (function () {
        if (!toastr) throw 'toastr plugin not referenced';

        toastr.options.timeOut = 3000;
        toastr.options.positionClass = 'toast-bottom-right';

        return {
            error: function (message, title) {
                toastr.error(message, title);
                log('Error: ' + message);
            },
            info: function (message, title) {
                toastr.info(message, title);
                log('Info: ' + message);
            },
            success: function (message, title) {
                toastr.success(message, title);
                log('Success: ' + message);
            },
            warning: function (message, title) {
                toastr.warning(message, title);
                log('Warning: ' + message);
            },
            logonly: log
        };

        function log() {
            var console = window.console;
            !!console && console.log && console.log.apply && console.log.apply(console, arguments);
        }

    })(),

    ajax: function (method, url, data, callback, type) {
        // shift arguments if data argument was omitted
        if ($.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = undefined;
        }

        return jQuery
            .ajax({ 'url': url, 'type': method || 'POST', 'dataType': type, 'data': data, 'cache': false })
            .done(function (result) {
                if (typeof callback === 'function') {
                    callback(result);
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                app.logger.error('Try reloading the page before trying again', errorThrown || textStatus);
            });
    },

    get: function (url, data, callback, type) {
        return app.ajax('GET', url, data, callback, type);
    },

    post: function (url, data, callback, type) {
        return app.ajax('POST', url, data, callback, type);
    },

    createDialogForm: function (loadUrl, callback) {
        var _dialog = $('<div style="min-width: 550px; font-size: .85em;">').hide().appendTo($('body'));

        _dialog.on('submit', 'form', function (e) {
            var form = $(this);

            e.preventDefault();

            $.validator.unobtrusive.parse(form);
            if (!form.valid()) {
                reposition();
                return false;
            }

            app.post(form.attr('action'), form.serializeArray(), function (result) {
                if (typeof result == 'string') {
                    _dialog.html(result);
                    reposition();
                } else {
                    _dialog.dialog('close');
                    callback(result);
                }
            });

            return false;
        });

        _dialog
            .dialog({ modal: true, width: 'auto', close: function () { _dialog.remove(); } })
            .append($('<p>').text('Loading...'))
            .load(loadUrl, function (response, status, xhr) {
                if (status != "error") {
                    reposition();
                    return;
                }

                app.logger.error(xhr.statusText, xhr.status);
            });

        function reposition() {
            // forces it to re-position considering any new content
            _dialog.dialog('option', 'position', _dialog.dialog('option', 'position'));
        }

        return _dialog;
    },

    debug: function (message) {
        if (window.console && 'debug' in window.console)
            console.debug(message);
        else if (window.console && 'log' in window.console)
            console.log(message);
        else
            $('<div>').text(message).appendTo('body');
        return this;
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


/******************************************************/
/******************************************************/
// dev helper

(function (window, $) {

    var $window, $screenSize;

    $(function () {

        $screenSize = $('.screenSize');
        if ($screenSize.length == 0) return;

        $window = $(window);

        setSize();
        $window.resize(setSize);

    });

    function setSize() {
        $screenSize.text($window.width() + ' x ' + $window.height() + '  (320 x 421 - iphone)');
    }

})(window, jQuery);
