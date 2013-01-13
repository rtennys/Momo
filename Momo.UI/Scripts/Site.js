﻿/******************************************************/
/******************************************************/
// prototypes

String.prototype.format = function() {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function(match, number) {
        return typeof args[number] != 'undefined' ? args[number] : match;
    });
};

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

(function(app, $, ko) {

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
    (function () {

        var vm = {
            listItems: ko.observableArray(),
            itemToEdit: ko.observable(),
            newItemName: ko.observable(),
            hideZero: ko.observable(false),
            hidePicked: ko.observable(false),
            onPickedChange: onPickedChange,
            onEditItem: onEditItem,
            onEditItemSubmit: onEditItemSubmit,
            onAddItemSubmit: onAddItemSubmit
        };

        $document.on({ pageinit: onInit, pageshow: onShow }, '.shoppinglists-show');

        function onInit() {
            ko.applyBindings(vm);
        }

        function onShow() {
            vm.listItems([]);
            $('#loading-msg').show();
            $.get(url('loaditems'), function (listItems) {
                vm.listItems($.map(listItems, extendItem).sort(itemComparer));

                $('#items-container').trigger('create').fadeIn('fast');
                $('#loading-msg').hide();
            });
        }
        
        function onPickedChange(listItem, e) {
            var cb = $(e.currentTarget);
            app.post(url('changepicked'), { id: listItem.Id(), picked: cb.is(':checked') });
            cb.parents('.ui-focus').removeClass('ui-focus');
        }
        
        function onEditItem(listItem, e) {
            var popup = $('#edit-item-container'),
                form = popup.find('form');

            e.currentTarget.blur();

            vm.itemToEdit(listItem);
            form.resetUnobtrusiveValidation();

            popup
                .show()
                .one('popupbeforeposition', function () { $('.ui-popup-screen').off(); })
                .one('popupafteropen', function () { $('[name="Aisle"]', this).focus().select(); })
                .popup('open');
        }

        function onEditItemSubmit() {
            var popup = $('#edit-item-container'),
                form = popup.find('form');

            if (!form.valid()) return;

            app.post(form.attr('action'), form.toObject(), function (result) {
                if (!result.Success) {
                    form
                        .resetUnobtrusiveValidation()
                        .appendValidationErrors(result.Errors);
                    return;
                }

                vm.listItems.remove(vm.itemToEdit());
                vm.listItems.push(vm.itemToEdit());
                vm.listItems.sort(itemComparer);
                refreshListview();

                vm.itemToEdit(null);

                popup.popup('close');
            });
        }

        function onAddItemSubmit(form) {
            form = $(form);
            app.post(form.attr('action'), form.toObject(), function (result) {
                if (!result.Success) {
                    form.appendValidationErrors(result.Errors);
                    return;
                }

                vm.newItemName(null);
                form.resetUnobtrusiveValidation();

                vm.listItems.push(extendItem(result.Item));
                vm.listItems.sort(itemComparer);
                refreshListview();
            });
        }


        function url(actionAndQuery) {
            return document.URL + '/' + actionAndQuery;
        }

        function extendItem(jsItem) {
            var item = ko.mapping.fromJS(jsItem);

            item.htmlName = ko.computed(function () {
                return 'item-' + item.Id();
            });

            item.isVisible = ko.computed(function () {
                if (item.Quantity() == 0 && vm.hideZero()) return false;
                if (item.Picked() && vm.hidePicked()) return false;
                return true;
            });

            item.showDivider = ko.computed(function () {
                var idx = vm.listItems.indexOf(item),
                    aisle = item.Aisle();

                if (idx < 0) return false;

                if (idx < 1 || vm.listItems()[idx - 1].Aisle() != aisle) {
                    var items = vm.listItems().filter(function(value) {
                        return value.Aisle() === aisle && value.isVisible();
                    });

                    return items.length > 0;
                }

                return false;
            });

            return item;
        }

        function itemComparer(a, b) {
            var aisleDiff = a.Aisle() - b.Aisle();
            if (aisleDiff !== 0) return aisleDiff;

            a = a.Name().toLowerCase();
            b = b.Name().toLowerCase();

            if (a < b) return -1;
            if (a > b) return 1;
            return 0;
        }
        
        function refreshListview() {
            $('#items-container ul')
                .listview('refresh')
                .find('li')
                .trigger('create');
        }

    })();

})(app, jQuery, ko);
