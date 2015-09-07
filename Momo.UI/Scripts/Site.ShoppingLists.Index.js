(function(app, $) {

    var _addListForm;

    app.modules.shoppingListsIndex = {
        init: function() {
            _addListForm = $('#addListForm');
            _addListForm.submit(onNewListSubmit);
        }
    };

    function onNewListSubmit(e) {
        e.preventDefault();
        app.post(_addListForm.attr('action'), _addListForm.serializeArray(), onNewListSubmitResult);
    }

    function onNewListSubmitResult(result) {
        if (!result.Success) {
            _addListForm
                .resetUnobtrusiveValidation()
                .appendValidationErrors(result.Errors);
            return;
        }

        _addListForm.resetUnobtrusiveValidation();
        _addListForm.find(':text').val('').get(0).blur();

        $('#myShoppingLists').append('<p><a href="' + result.ShoppingList.Url + '">' + result.ShoppingList.Name + '</a></p>');
        $('#noLists').hide();
    }

})(app, jQuery);
