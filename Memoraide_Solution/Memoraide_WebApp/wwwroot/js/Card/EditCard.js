function CancelEdit() {
    var cancel = confirm('Cancel edit card?');
    if (cancel) {
        var jqxhr = $.ajax("/Card/ViewCard/" + document.getElementById("fieldid").value);
    }
}