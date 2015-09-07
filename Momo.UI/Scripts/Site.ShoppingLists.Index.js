(function(app, $) {

    var addListForm = $('#addListForm');

    addListForm.submit(onNewListSubmit);

    function onNewListSubmit(e) {
        e.preventDefault();
        app.post(addListForm.attr('action'), addListForm.serializeArray(), onNewListSubmitResult);
    }

    function onNewListSubmitResult(result) {
        if (!result.Success) {
            addListForm
                .resetUnobtrusiveValidation()
                .appendValidationErrors(result.Errors);
            return;
        }

        addListForm.resetUnobtrusiveValidation();
        addListForm.find(':text').val('').get(0).blur();

        $('#myShoppingLists').append('<p><a href="' + result.ShoppingList.Url + '">' + result.ShoppingList.Name + '</a></p>');
        $('#noLists').hide();
    }

})(app, jQuery);
