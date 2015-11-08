(function (app, $) {

    var _searchForm, _searchDelay, _searchInput, _searchResultContainer, _noSearchResults, _addItemFormTemplate;

    app.modules.addListItemIndex = { init: init };

    function init() {
        _addItemFormTemplate = $('#addItemFormTemplate');
        _searchResultContainer = $('#searchResultContainer');
        _noSearchResults = $('#noSearchResults');

        _searchForm = $('#searchForm');
        _searchForm.submit(onSearchFormSubmit);

        _searchDelay = new app.Delayed(search);

        _searchInput = $('#name');
        _searchInput.keyup(_searchDelay.execute);
        _searchInput.val('').focus();
    }

    function search() {
        _searchResultContainer.html('');
        _noSearchResults.hide();

        if (_searchInput.val().length < 3) {
            _noSearchResults.show();
            return;
        }

        app.post(_searchForm.attr('action'), _searchForm.serializeArray(), function (result) {
            for (var i = 0; i < result.length; i++) {
                addSearchResult(result[i]);
            }
        });
    }

    function addSearchResult(result) {
        var addItemForm = _addItemFormTemplate.clone().show();
        addItemForm.find('input[name="quantity"]').val(result.Quantity);
        addItemForm.find('input[name="name"]').val(result.Name);
        addItemForm.find('span[name="name"]').text(result.Name);
        addItemForm.find('input[name="aisle"]').val(result.Aisle);
        if (result.OnList)
            addItemForm.removeClass("not-on-list");
        addItemForm.submit(onAddItem);
        _searchResultContainer.append(addItemForm);
    }

    function onAddItem(e) {
        e.preventDefault();
        var form = $(this);
        app.post(form.attr('action'), form.serializeArray(), function () {
            form.removeClass("not-on-list");
            setTimeout(function () { _searchInput.focus().select(); }, 100);
        });
    }

    function onSearchFormSubmit(e) {
        e.preventDefault();
        _searchDelay.execute();
        return false;
    }

})(app, jQuery);
