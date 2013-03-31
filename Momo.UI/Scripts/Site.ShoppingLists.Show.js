(function(app, $, ko) {

    var vm = app.viewModel = {
        noItemsMsg: ko.observable('Loading...'),
        listItems: ko.observableArray([]),
        itemToEdit: ko.observable(),
        newItemName: ko.observable(),
        hideZeros: ko.observable(true),
        hidePicked: ko.observable(true),
        onItemClick: onItemClick,
        onEditItemClick: onEditItemClick,
        onEditItemSubmit: onEditItemSubmit,
        onAddItemSubmit: onAddItemSubmit,
        onForgetItemClick: onForgetItemClick
    },
        editDialog = $('#edit-item-container').dialog({
            autoOpen: false,
            modal: true,
            width: 'auto'
        });

    vm.noItemsVisible = ko.computed(function () {
        return vm.listItems().filter(function (item) { return item.isVisible(); }).length == 0;
    });

    vm.estimatedTotal = ko.computed(function () {
        var total = 0.0;
        $.each(vm.listItems(), function () {
            total += this.Picked() ? 0 : parseFloat(this.Quantity()) * parseFloat(this.Price());
        });
        return total;
    });

    ko.applyBindings(vm);

    $(function() {
        setTimeout(function() {

            app.post(app.urls.show, function (listItems) {
                vm.listItems($.map(listItems, extendItem).sort(itemComparer));
                vm.noItemsMsg('Nothing needed!');

                $('#items-container').show();

                var txtAddItem = $('#txtAddItem');
                txtAddItem.autocomplete({
                    delay: 500,
                    minLength: 3,
                    source: app.urls.autocomplete,
                    select: function (e, ui) {
                        txtAddItem.val(ui.item.value);
                        txtAddItem.parents('form:first').submit();
                    }
                });
            });

        }, 500);
    });


    function onItemClick(listItem, e) {
        app.post(app.urls.changePicked, { id: listItem.Id(), picked: listItem.Picked(), '__RequestVerificationToken': $('[name="__RequestVerificationToken"]').val() });
        return true;
    }

    function onEditItemClick(listItem, e) {
        var popup = $('#edit-item-container'),
            form = popup.find('form'),
            isQtyEdit = $('[data-bind*="Quantity"]', e.currentTarget).length > 0;

        e.currentTarget.blur();

        vm.itemToEdit(listItem);
        form.resetUnobtrusiveValidation();

        editDialog
            .one('dialogopen', function () {
                var field = isQtyEdit ? 'Quantity' : 'Aisle';
                var element = $('[name="' + field + '"]', editDialog);
                app.logger.logonly(field);
                app.logger.logonly(element);
                setTimeout(function () {
                    element.focus().select();
                }, 200);
            })
            .dialog('open');
    }

    function onEditItemSubmit() {
        var form = editDialog.find('form');

        if (!form.valid()) return;

        app.post(form.attr('action'), form.serializeArray(), function (result) {
            if (!result.Success) {
                form
                    .resetUnobtrusiveValidation()
                    .appendValidationErrors(result.Errors);
                return;
            }

            editDialog.dialog('close');
            vm.itemToEdit(null);

            vm.listItems.sort(itemComparer);
        });
    }

    function onAddItemSubmit(form) {
        form = $(form);
        app.post(form.attr('action'), form.serializeArray(), function (result) {
            if (!result.Success) {
                form.appendValidationErrors(result.Errors);
                return;
            }

            vm.newItemName(null);
            form.resetUnobtrusiveValidation();

            var foundItem = vm.listItems().filter(function (item) { return item.Id() === result.Item.Id; });

            if (foundItem.length > 0) {
                ko.mapping.fromJS(result.Item, foundItem[0]);
            } else {
                vm.listItems.push(extendItem(result.Item));
                vm.listItems.sort(itemComparer);
            }
        });
    }

    function onForgetItemClick(listItem) {
        if (!confirm('Delete this item from the list?')) return;

        app.post(app.urls.deleteItem, { id: listItem.Id(), '__RequestVerificationToken': $('[name="__RequestVerificationToken"]').val() });

        $('#edit-item-container').popup('close');
        vm.itemToEdit(null);
        vm.listItems.remove(listItem);
    }


    function extendItem(jsItem) {
        var item = ko.mapping.fromJS(jsItem);

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
                var items = vm.listItems().filter(function (value) {
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
