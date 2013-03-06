(function(app, $, ko) {

    var vm, page;

    function onInit() {
        page = $(this);
        vm = {
            noItemsMsg: ko.observable(),
            listItems: ko.observableArray(),
            itemToEdit: ko.observable(),
            newItemName: ko.observable(),
            hideZeros: ko.observable(true),
            hidePicked: ko.observable(true),
            onItemClick: onItemClick,
            onEditItemClick: onEditItemClick,
            onEditItemSubmit: onEditItemSubmit,
            onAddItemSubmit: onAddItemSubmit,
            onForgetItemClick: onForgetItemClick
        };

        vm.noItemsVisible = ko.computed(function() {
            return vm.listItems().filter(function(item) { return item.isVisible(); }).length == 0;
        });

        vm.estimatedTotal = ko.computed(function() {
            var total = 0.0;
            $.each(vm.listItems(), function() {
                total += parseFloat(this.Quantity()) * parseFloat(this.Price());
            });
            return total;
        });

        $('#items-container ul').removeClass('ui-corner-all').addClass('ui-corner-top');

        var runCheckboxradio = new app.Delayed(function() { $('#items-container :checkbox', page).checkboxradio(); }, 10);
        vm.hideZeros.subscribe(runCheckboxradio.execute);
        vm.hidePicked.subscribe(runCheckboxradio.execute);

        ko.applyBindings(vm, this);
    }

    function onShow() {
        vm.listItems([]);
        vm.noItemsMsg('Loading...');
        vm.hideZeros(true);
        vm.hidePicked(true);

        app.post(page.data('url'), {}, function(listItems) {
            vm.listItems($.map(listItems, extendItem).sort(itemComparer));

            $('#items-container :checkbox', page).checkboxradio();
            $('#items-container', page).show();

            vm.noItemsMsg('Nothing needed!');
        });
    }

    function onItemClick(listItem, e) {
        app.post(url('changepicked'), { id: listItem.Id(), picked: listItem.Picked() });
        $(e.currentTarget).parents('.ui-focus').removeClass('ui-focus');
    }

    function onEditItemClick(listItem, e) {
        var popup = $('#edit-item-container'),
            form = popup.find('form'),
            isQtyClick = $(e.currentTarget).is('span');

        e.currentTarget.blur();

        vm.itemToEdit(listItem);
        form.resetUnobtrusiveValidation();

        popup
            .show()
            .one('popupbeforeposition', function() { $('.ui-popup-screen').off(); })
            .one('popupafteropen', function() {
                var field = isQtyClick ? 'Quantity' : 'Aisle';
                var element = $('[name="' + field + '"]', this);
                setTimeout(function() {
                    element.focus().select();
                }, 200);
            })
            .popup('open');
    }

    function onEditItemSubmit() {
        var popup = $('#edit-item-container'),
            form = popup.find('form');

        if (!form.valid()) return;

        app.post(form.attr('action'), form.toObject(), function(result) {
            if (!result.Success) {
                form
                    .resetUnobtrusiveValidation()
                    .appendValidationErrors(result.Errors);
                return;
            }

            popup.popup('close');
            vm.itemToEdit(null);

            vm.listItems.sort(itemComparer);
        });
    }

    function onAddItemSubmit(form) {
        form = $(form);
        app.post(form.attr('action'), form.toObject(), function(result) {
            if (!result.Success) {
                form.appendValidationErrors(result.Errors);
                return;
            }

            vm.newItemName(null);
            form.resetUnobtrusiveValidation();

            var foundItem = vm.listItems().filter(function(item) { return item.Id() === result.Item.Id; });

            if (foundItem.length > 0) {
                ko.mapping.fromJS(result.Item, foundItem[0]);
            } else {
                vm.listItems.push(extendItem(result.Item));
                $('#items-container :checkbox', page).checkboxradio();
                vm.listItems.sort(itemComparer);
            }
        });
    }

    function onForgetItemClick(listItem) {
        if (!confirm('Delete this item from the list?')) return;

        app.post(url('deleteitem'), { id: listItem.Id() });

        $('#edit-item-container').popup('close');
        vm.itemToEdit(null);
        vm.listItems.remove(listItem);
    }


    function url(actionAndQuery) {
        return page.data('url') + '/' + actionAndQuery;
    }

    function extendItem(jsItem) {
        var item = ko.mapping.fromJS(jsItem);

        item.htmlName = ko.computed(function() {
            return 'item-' + item.Id();
        });

        item.isVisible = ko.computed(function() {
            if (item.Quantity() == 0 && vm.hideZeros()) return false;
            if (item.Picked() && vm.hidePicked()) return false;
            return true;
        });

        item.showDivider = ko.computed(function() {
            var idx = vm.listItems.indexOf(item),
                aisle = +item.Aisle();

            if (idx < 0) return false;

            if (idx < 1 || +vm.listItems()[idx - 1].Aisle() != aisle) {
                var items = vm.listItems().filter(function(value) {
                    return +value.Aisle() === aisle && value.isVisible();
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

})(app, jQuery, ko);
