function ToggleEdit() {
    var edit = document.getElementById("Edit");
    if (edit.style.display === "none") {
        edit.style.display = "block";
    } else {
        edit.style.display = "none";
    }

    var view = document.getElementById("View");
    if (view.style.display === "none") {
        view.style.display = "block";
    } else {
        view.style.display = "none";
    }
}

function CancelEdit() {
    var cancel = confirm('Cancel edit card?');
    if (cancel) {
        //Reset fields?
        ToggleEdit();
    }
}

function SubmitEdit() {
    var model = {
        ID: document.getElementById('fieldid').value,
        UserId: document.getElementById('fielduserid').value,
        DeckName: document.getElementById('fieldDeckname').value
    };

    $.ajax({
        method: "put",
        data: model,
        url: "/Deck/EditDeck/" + model.ID
    });

}