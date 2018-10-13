

history.pushState(null, null, location.href);
window.onpopstate = function () {
    history.go(1);
};


$(document).ready(function () {


    var f1 = 0,
        f2 = 0;

    if ($("#Username").val() == "") {
        f1 = 0;
        $("#UsernameError").html("Please enter username");
    }
    else {
        f1 = 1;
        $("#UsernameError").html("");
    }



    if ($("#Password").val() == "") {
        f2 = 0;
        $("#PasswordError").html("Please enter password");
    }

    else {
        f2 = 1;
        $("#PasswordError").html("");
    }

    if (f1 && f2) {

        $('#loginbtn').removeAttr('disabled');

    }
    else
        $('#loginbtn').attr('disabled', 'disabled');


    $("#Username")
        .keyup(function () {
            if ($("#Username").val() == "") {
                f1 = 0;
                $("#UsernameError").html("Please enter username");
            }
            else {
                f1 = 1;
                $("#UsernameError").html("");
            }

        });



    $("#Password")
        .keyup(function () {
            if ($("#Password").val() == "") {
                f2 = 0;
                $("#PasswordError").html("Please enter password");
            }

            else {
                f2 = 1;
                $("#PasswordError").html("");
            }
        });


    $(".form-control")
        .keyup(function () {

            if (f1 && f2) {

                $('#loginbtn').removeAttr('disabled');

            }
            else
                $('#loginbtn').attr('disabled', 'disabled');


        });



});
