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
    var cancel = confirm('Cancel edit deck?');
    if (cancel) {
        //Reset fields?
        ToggleEdit();
    }
}

function SubmitEdit() {
    var model = {
        ID: document.getElementById('fieldid').value,
        DeckId: document.getElementById('fielduserid').value,
        Question: document.getElementById('fieldQuestion').value,
        Answer: document.getElementById('fieldAnswer').value,
        CardTags: document.getElementById('fieldtags').value
    };

    $.ajax({
        method: "put",
        data: model,
        url: "/Card/EditCard/" + model.ID
    });

}