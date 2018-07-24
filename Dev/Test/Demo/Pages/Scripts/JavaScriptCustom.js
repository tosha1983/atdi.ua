function IsOneDecimalPoint(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode; // restrict user to type only one . point in number
    var parts = evt.srcElement.value.split('.');
    if(parts.length > 1 && charCode==46)
        return false;
    return true;
}

function checkDec(el) {
    var ex = /^[0-9]+\.?[0-9]*$/;
    if (ex.test(el.value) == false) {
        alert('Incorrect Number');
    }
}



window.alert = function ConfirmDialog(message) {
    $('<div></div>').appendTo('body')
                    .html('<div><h6>' + message + '</h6></div>')
                    .dialog({
                        resizable: false,
                        height: "auto",
                        width: 400,
                        modal: true,
                        buttons: {
                            "Так": function () {
                                $(this).dialog("close");
                            }
                        },
                        close: function (event, ui) {
                            $(this).remove();
                        }
                    });
};


function ConfirmDialogDelete(message, UniqueID) {
    $('<div></div>').appendTo('body')
                    .html('<div><h6>' + message + '</h6></div>')
                    .dialog({
                        resizable: false,
                        height: "auto",
                        width: 400,
                        modal: true,
                        buttons: {
                            "Так": function () {
                                var confirm_value_delete = document.createElement("INPUT");
                                confirm_value_delete.type = "hidden";
                                confirm_value_delete.name = "confirm_value_delete";
                                confirm_value_delete.value = "Yes";
                                document.forms[0].appendChild(confirm_value_delete);
                                __doPostBack(UniqueID, 'OnClick');
                                $(this).dialog("close");
                            },
                            "Скасувати": function () {
                                $(this).dialog("close");
                            }
                        },
                        close: function (event, ui) {
                            $(this).remove();
                        }
                    });
};


function ConfirmAgree() {
    var confirm_value_agree = document.createElement("INPUT");
    confirm_value_agree.type = "hidden";
    confirm_value_agree.name = "confirm_value_agree";
    if (confirm("You agree to the terms?")) {
        confirm_value_agree.value = "Yes";
    } else {
        confirm_value_agree.value = "No";
    }
    document.forms[0].appendChild(confirm_value_agree);
}

function ConfirmSave() {
           var confirm_value_save = document.createElement("INPUT");
           confirm_value_save.type = "hidden";
           confirm_value_save.name = "confirm_value_save";
           if (confirm("Do you want a save data?")) {
               confirm_value_save.value = "Yes";
           } else {
               confirm_value_save.value = "No";
           }
           document.forms[0].appendChild(confirm_value_save);
       }

function ConfirmDelete(UniqueID) {
    ConfirmDialogDelete("Ви хочете видалити запис?", UniqueID);
}

function ConfirmNew() {
    var confirm_value_new = document.createElement("INPUT");
    confirm_value_new.type = "hidden";
    confirm_value_new.name = "confirm_value_new";
    if (confirm("Do you want a create new data?")) {
        confirm_value_new.value = "Yes";
    } else {
        confirm_value_new.value = "No";
    }
    document.forms[0].appendChild(confirm_value_new);
}

function ConfirmComplete() {
    var conf_value_compl = document.createElement("INPUT");
    conf_value_compl.type = "hidden";
    conf_value_compl.name = "conf_value_compl";
    if (confirm("You want to complete data entry?  Warning: After pressing the button in this package can not add and edit data!")) {
        conf_value_compl.value = "Yes";
    } else {
        conf_value_compl.value = "No";
    }
    document.forms[0].appendChild(conf_value_compl);
}

function ConfirmMoveRec() {
    var confirm_value_TR = document.createElement("INPUT");
    confirm_value_TR.type = "hidden";
    confirm_value_TR.name = "confirm_value_TR";
    if (confirm("Copy transmitter, antenna and feeder parameters from Transmitter to Receiver if the details are the same?")) {
        confirm_value_TR.value = "Yes";
    } else {
        confirm_value_TR.value = "No";
    }
    document.forms[0].appendChild(confirm_value_TR);
}

function ConfirmMoveTrans() {
    var confirm_value_RT = document.createElement("INPUT");
    confirm_value_RT.type = "hidden";
    confirm_value_RT.name = "confirm_value_RT";
    if (confirm("Copy transmitter, antenna and feeder parameters from Receiver to Transmitter if the details are the same?")) {
        confirm_value_RT.value = "Yes";
    } else {
        confirm_value_RT.value = "No";
    }
    document.forms[0].appendChild(confirm_value_RT);
}

