function CancelEdit() {
    var cancel = confirm('Cancel edit deck?');
    if (cancel) {
        var jqxhr = $.ajax("/Deck/ViewDeck/" + document.getElementById("fieldid").value);
    }
}