(function(app, $) {

    var _vm, _editDialog;

    app.modules.shoppingListsShow = { init: init };

    function init() {
        _vm = {
            noItemsMsg: ko.observable('Loading...'),
            listItems: ko.observableArray([]),
            itemToEdit: ko.observable(),
            hideZeros: ko.observable(true),
            hidePicked: ko.observable(true),
            onChangePicked: onChangePicked,
            onEditItemClick: onEditItemClick,
            onEditItemSubmit: onEditItemSubmit,
            onForgetItemClick: onForgetItemClick
        };

        _editDialog = $('#edit-item-container').dialog({
            autoOpen: false,
            modal: true,
            width: 'auto'
        });

        _vm.noItemsVisible = ko.computed(function () {
            return _vm.listItems().filter(function (item) { return item.isVisible(); }).length == 0;
        });

        _vm.estimatedTotal = ko.computed(function () {
            var total = 0.0;
            $.each(_vm.listItems(), function () {
                total += this.Picked() ? 0 : parseFloat(this.Quantity()) * parseFloat(this.Price());
            });
            return total;
        });

        ko.applyBindings(_vm);

        setTimeout(function () {
            app.post(app.urls.show, function (listItems) {
                _vm.listItems($.map(listItems, extendItem).sort(itemComparer));
                _vm.noItemsMsg('Nothing needed!');
                $('#items-container').show();
            });
        }, 500);
    }

    function onChangePicked(listItem) {
        app.post(app.urls.changePicked, { id: listItem.Id(), picked: listItem.Picked(), '__RequestVerificationToken': $('[name="__RequestVerificationToken"]').val() });
        return true;
    }

    function onEditItemClick(listItem, e) {
        var popup = $('#edit-item-container'),
            form = popup.find('form'),
            isQtyEdit = $('[data-bind*="Quantity"]', e.currentTarget).length > 0;

        e.currentTarget.blur();

        _vm.itemToEdit(listItem);
        form.resetUnobtrusiveValidation();

        _editDialog
            .one('dialogopen', function () {
                var field = isQtyEdit ? 'Quantity' : 'Aisle', element = $('[name="' + field + '"]', _editDialog);
                setTimeout(function () { element.focus().select(); }, 200);
            })
            .dialog('open');
    }

    function onEditItemSubmit() {
        var form = _editDialog.find('form');

        if (!form.valid()) return;

        app.post(form.attr('action'), form.serializeArray(), function (result) {
            if (!result.Success) {
                form
                    .resetUnobtrusiveValidation()
                    .appendValidationErrors(result.Errors);
                return;
            }

            _editDialog.dialog('close');
            _vm.itemToEdit(null);

            _vm.listItems.sort(itemComparer);
        });
    }

    function onForgetItemClick(listItem) {
        if (!confirm('Delete this item from the list?')) return;

        app.post(app.urls.deleteItem, { id: listItem.Id(), '__RequestVerificationToken': $('[name="__RequestVerificationToken"]').val() });

        $('#edit-item-container').dialog('close');
        _vm.itemToEdit(null);
        _vm.listItems.remove(listItem);
    }


    function extendItem(jsItem) {
        var item = ko.mapping.fromJS(jsItem);

        item.isVisible = ko.computed(function() {
            if (item.Quantity() == 0 && _vm.hideZeros()) return false;
            if (item.Picked() && _vm.hidePicked()) return false;
            return true;
        });

        item.showDivider = ko.computed(function() {
            var idx = _vm.listItems.indexOf(item),
                aisle = +item.Aisle();

            if (idx < 0) return false;

            if (idx < 1 || +_vm.listItems()[idx - 1].Aisle() != aisle) {
                var items = _vm.listItems().filter(function (value) {
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

})(app, jQuery);
