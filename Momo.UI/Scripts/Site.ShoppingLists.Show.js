﻿(function (app, $) {
    var _editDialog;

    app.modules.shoppingListsShow = { init: init };

    function init() {
        $('#hideZeros').click(onHideZeros);
        $('#hidePicked').click(onHidePicked);

        _editDialog = $('#edit-item-container').dialog({
            autoOpen: false,
            modal: true,
            width: 'auto'
        });

        if (_editDialog.length === 0) return; // doesn't have write access

        $(':checkbox[name="picked"]').click(onChangePicked);
        $('.list-item-name').click(onNameClicked);
        $('.list-item-quantity').click(onQuantityClicked);
        $('#editItemForm', _editDialog).submit(onEditItemSubmit);
        $('#deleteItemForm', _editDialog).submit(onDeleteItemForm);
    }

    function onChangePicked() {
        var $this = $(this);
        var picked = $this.is(':checked');

        app.post(app.urls.changePicked, { id: $this.data('id'), picked: picked, '__RequestVerificationToken': $('[name="__RequestVerificationToken"]').val() });

        if (picked)
            $this.closest('.item-container').addClass('picked');
        else
            $this.closest('.item-container').removeClass('picked');

        showHideAisle($this.closest('.aisle-container'));
    }

    function onHideZeros() {
        $('#items-container').toggleClass('hide-zero');
        showHideAisles();
    }

    function onHidePicked() {
        $('#items-container').toggleClass('hide-picked');
        showHideAisles();
    }

    function onNameClicked(e) {
        openEditDialog(e, false);
    }

    function onQuantityClicked(e) {
        openEditDialog(e, true);
    }

    function onEditItemSubmit(e) {
        e.preventDefault();
        var form = $(this);

        if (!form.valid()) return;

        app.post(form.attr('action'), form.serializeArray(), function (result) {
            if (!result.Success) {
                form
                    .resetUnobtrusiveValidation()
                    .appendValidationErrors(result.Errors);
                return;
            }

            (window || document).location.reload(true);
        });
    }

    function onDeleteItemForm(e) {
        e.preventDefault();

        if (!confirm('Delete this item from the list?')) return;

        var form = $(this);
        app.post(form.attr('action'), form.serializeArray(), function (result) {
            if (!result.Success) {
                form
                    .resetUnobtrusiveValidation()
                    .appendValidationErrors(result.Errors);
                return;
            }

            _editDialog.dialog('close');
            removeItem($('[name="Id"]', form).val());
        });
    }

    function showHideAisles() {
        $('.aisle-container').each(function () {
            showHideAisle($(this));
        });
    }

    function showHideAisle(aisle) {
        if (aisle.children('.item-container:not(.zero,.picked)').length === 0)
            aisle.addClass('nothing-needed');
        else
            aisle.removeClass('nothing-needed');
    }

    function openEditDialog(e, isQtyEdit) {
        var itemContainer = $(e.currentTarget).closest('.item-container');

        e.currentTarget.blur();

        $('[name="Id"]', _editDialog).val(itemContainer.data('id'));
        $('[name="Name"]', _editDialog).val(itemContainer.data('name'));
        $('[name="Quantity"]', _editDialog).val(itemContainer.data('quantity'));
        $('[name="Aisle"]', _editDialog).val(itemContainer.data('aisle'));

        $('form', _editDialog).resetUnobtrusiveValidation();

        _editDialog
            .one('dialogopen', function () {
                var field = isQtyEdit ? 'Quantity' : 'Aisle', element = $('[name="' + field + '"]', _editDialog);
                setTimeout(function () { element.focus().select(); }, 100);
            })
            .dialog('open');
    }

    function removeItem(itemId) {
        var itemContainer = $('#item_' + itemId),
            aisleContainer = itemContainer.closest('.aisle-container');

        itemContainer.remove();
        if (aisleContainer.children('.item-container').length === 0)
            aisleContainer.remove();
    }

})(app, jQuery);
