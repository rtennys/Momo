(function (app, $, ko) {

    app.viewModel = {
        noItemsMsg: ko.observable(),
        shoppingLists: ko.observableArray(),
        sharedLists: ko.observableArray(),
        newListName: ko.observable(),
        onNewListSubmit: onNewListSubmit
    };

    ko.applyBindings(app.viewModel);

    app.viewModel.noItemsMsg('Loading...');

    app.post(app.urls.index, {}, function (result) {
        app.viewModel.shoppingLists($.map(result.ShoppingLists, ko.mapping.fromJS));
        app.viewModel.sharedLists($.map(result.SharedLists, ko.mapping.fromJS));
        app.viewModel.noItemsMsg('No shopping lists found');
    });

    function onNewListSubmit(form) {
        form = $(form);
        app.post(form.attr('action'), form.serializeArray(), function (result) {
            if (!result.Success) {
                form
                    .resetUnobtrusiveValidation()
                    .appendValidationErrors(result.Errors);

                return;
            }

            app.viewModel.newListName(null);
            form.resetUnobtrusiveValidation();
            $(':text').get(0).blur();

            app.viewModel.shoppingLists.push(ko.mapping.fromJS(result.ShoppingList));
        });
    }

})(app, jQuery, ko);
