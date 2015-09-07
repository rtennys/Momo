/******************************************************/
/******************************************************/
// knockout extensions

// money
(function () {

    $.extend(ko.bindingHandlers, {
        moneyValue: {
            init: ko.bindingHandlers.value.init,
            update: function (element, valueAccessor) {
                var formattedValue = formatValue(valueAccessor);
                ko.bindingHandlers.value.update(element, function () { return formattedValue; });
            }
        },
        moneyText: {
            init: ko.bindingHandlers.text.init,
            update: function (element, valueAccessor) {
                var formattedValue = formatText(valueAccessor);
                ko.bindingHandlers.text.update(element, function () { return formattedValue; });
            }
        }
    });

    function formatValue(valueAccessor) {
        return parseFloat(ko.utils.unwrapObservable(valueAccessor())).toFixed(2);
    }

    function formatText(valueAccessor) {
        return '$' + addCommas(formatValue(valueAccessor));
    }

    function addCommas(value) {
        var rx = /(\d+)(\d{3})/;
        return value.replace(/^\d+/, function (w) {
            while (rx.test(w)) {
                w = w.replace(rx, '$1,$2');
            }
            return w;
        });
    }

})();
