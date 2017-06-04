(function (app, $) {

    var _searchForm, _searchDelay, _searchInput, _searchResultContainer, _noSearchResults, _addItemFormTemplate;

    app.modules.addListItemIndex = { init: init };

    function init() {
        _addItemFormTemplate = $('#addItemFormTemplate').remove();
        _searchResultContainer = $('#searchResultContainer');
        _noSearchResults = $('#noSearchResults');

        _searchForm = $('#searchForm');
        _searchForm.submit(onSearchFormSubmit);

        _searchDelay = new app.Delayed(search);

        _searchInput = $('#name');
        _searchInput.keyup(_searchDelay.execute);
        _searchInput.val('').focus();

        $('body').on('keydown', 'input', onKeydown);
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
        var form = _addItemFormTemplate.clone().show();
        form.removeAttr('id');
        form.find('input[name="quantity"]').val(result.Quantity);
        form.find('input[name="name"]').val(result.Name);
        form.find('span[name="name"]').text(result.Name).addClass(result.OnList ? 'list-item-status-exists' : result.IsNew ? 'list-item-status-new' : 'list-item-status-none');
        form.find('input[name="aisle"]').val(result.Aisle);
        form.submit(onAddItem);
        _searchResultContainer.append(form);
    }

    function onAddItem(e) {
        e.preventDefault();
        _searchInput.focus().select();
        var form = $(this);
        app.post(form.attr('action'), form.serializeArray(), function () {
            form.find('span[name="name"]').removeClass('list-item-status-none list-item-status-new').addClass('list-item-status-exists');
            setTimeout(function () { _searchInput.focus().select(); });
        });
    }

    function onSearchFormSubmit(e) {
        e.preventDefault();
        _searchDelay.execute();
        return false;
    }

    function onKeydown(e) {
        var upArrow = 38;
        var downArrow = 40;

        if (e.which !== upArrow && e.which !== downArrow) return true;

        var allForms = $('form');
        var index = allForms.index($(e.currentTarget).closest('form')) + (e.which === upArrow ? -1 : 1);

        if (allForms.length > index) {
            var nextQtyInput = allForms.eq(index).find('[name="quantity"]');
            nextQtyInput.focus().select();
            setTimeout(function () { nextQtyInput.focus().select(); });
        }

        e.preventDefault();
        return false;
    }

})(app, jQuery);
