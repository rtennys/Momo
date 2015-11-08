﻿(function (app, $) {

    var _searchInput, _searchForm, _searchResultContainer, _addItemFormTemplate;

    app.modules.addListItemIndex = { init: init };

    function init() {
        _addItemFormTemplate = $('#addItemFormTemplate');
        _searchResultContainer = $('#searchResultContainer');
        _searchForm = $('#searchForm');
        _searchInput = $('#name');
        _searchInput.keyup(new app.Delayed(search).execute);
        _searchInput.val('').focus();
    }

    function search() {
        _searchResultContainer.html('');
        if (_searchInput.val().length < 3) return;
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
        addItemForm.find('span[name="aisle"]').text(result.Aisle);
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
            _searchInput.focus();
        });
    }

})(app, jQuery);
