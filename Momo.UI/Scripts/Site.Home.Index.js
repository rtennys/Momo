$(function() {

    var module = app.modules.homeIndex = {
        shoppingLists: $('#shoppingLists')
    };

    setTimeout(function () { module.shoppingLists.text('woot'); }, 1000);

});
