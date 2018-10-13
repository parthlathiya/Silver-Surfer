     //function DisableBackButton() {
     //  window.history.forward()
     // }
     //DisableBackButton();
     //window.onload = DisableBackButton;
     //window.onpageshow = function(evt) { if (evt.persisted) DisableBackButton() }
     //window.onunload = function() { void (0) }


history.pushState(null, null, location.href);
window.onpopstate = function () {
    history.go(1);
};
$(document).ready(function () {


    var f1 = 0,
        f2 = 0;

    if ($("#Email").val() == "") {
        f1 = 0;
        $("#EmailError").html("Please enter email address");
    }
    else {
        f1 = 1;
        $("#EmailError").html("");
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


    $("#Email")
        .keyup(function () {
            if ($("#Email").val() == "") {
                f1 = 0;
                $("#EmailError").html("Please enter email address");
            }
            else {
                f1 = 1;
                $("#EmailError").html("");
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
            
            if (f1 && f2 ) {

                $('#loginbtn').removeAttr('disabled');
   
            }
            else
                $('#loginbtn').attr('disabled', 'disabled');


        });



});
