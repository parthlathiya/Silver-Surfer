

$(document).ready(function () {
    
            var dateback = new Date(new Date().setFullYear(new Date().getFullYear() - 18));
       
            $("#DOB").datepicker({
                    onSelect: function (formattedDate, date, inst) {
                        if ($("#DOB").val() == "") {
                            f5 = 0;
                            $("#DOBError").html("Please enter date of birth");
                        }
                        else {
                            f5 = 1;
                            $("#DOBError").html("");
                        }
                        console.log('ss' + f1 + f2 + f3 + f4 + f5 + f6 + f7 );
                        if (f1 && f2 && f3 && f4 && f5 && f6 && f7) {

                            $('#subbtn').removeAttr('disabled');
                        }
                        else
                            $('#subbtn').attr('disabled', 'disabled');
                    },
                    showButtonPanel: false,
                    dateFormat: "dd-mm-yy",
                    maxDate: dateback,
                    autoclose: true,
            });
          
    var f1 = 0,
        f2 = 0,
        f3 = 0,
        f4 = 0,
        f5 = 0,
        f6 = 0,
        f7 = 0;

    $("#Name")
        .keyup(function () {
                    if ($("#Name").val() == "") {
                        f1 = 0;
                        $("#NameError").html("Please enter name");
                    }
                    else {
                        f1 = 1;
                        $("#NameError").html("");
                    }
                });

            $("#ContactNo")
                .keyup(function () {
                    if ($("#ContactNo").val() == "") {
                        f2 = 0;
                        $("#ContactNoError").html("Please enter contact number");
                    }
                    else if ($("#ContactNo").val().length != 10) {
                        f2 = 0;
                        $("#ContactNoError").html("Please enter correct contact number, It should have 10 digits");
                    }
                    else if ($("#ContactNo").val().charAt(0) == '0') {
                        f2 = 0;
                        $("#ContactNoError").html("Please enter correct contact number, It can not starts with 0");
                    }
                    else {
                        f2 = 1;
                        $("#ContactNoError").html("");
                    }
                });


            $("#Email")
                .keyup(function () {
                    var patt = new RegExp(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i);
                    if ($("#Email").val() == "") {
                        f3 = 0;
                        $("#EmailError").html("Please enter email address");
                    }
                    else if (!patt.test($("#Email").val())) {
                        f3 = 0;
                        $("#EmailError").html("Please enter valid email address");
                    }
                    else {
                        f3 = 1;
                        $("#EmailError").html("");
                    }

                });

            $("#Address")
                .keyup(function () {
                    if ($("#Address").val() == "") {
                        f4 = 0;
                        $("#AddressError").html("Please enter address");
                    }
                    else {
                        f4 = 1;
                        $("#AddressError").html("");
                    }

                });

    $("#DOB")
        .change(function () {

                    if ($("#DOB").val() == "") {
                        f5 = 0;
                        $("#DOBError").html("Please enter date of birth");
                    }
                    else {
                        f5 = 1;
                        $("#DOBError").html("");
            }
            
            if (f1 && f2 && f3 && f4 && f5 && f6 && f7) {

                $('#subbtn').removeAttr('disabled');
                //$('#subbtn').tooltip('hide')
            }
            else
                $('#subbtn').attr('disabled', 'disabled');
          
                });

            $("#Password")
                .keyup(function () {
                    var patte = new RegExp(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,16})/);
                    if ($("#Password").val() == "") {
                        f6 = 0;
                        $("#PasswordError").html("Please enter password");
                    }
                    else if (!patte.test($("#Password").val())) {
                        f6 = 0;
                        $("#PasswordError").html("Password is not according to constraint<br\> Password should have atleast one uppercase, one lowercase character and one number. Length should be between 8-16.");
                    }
                    else {
                        f6 = 1;
                        $("#PasswordError").html("");
                    }
                });

            $("#RePassword")
                .keyup(function () {
                    if ($("#RePassword").val() == "") {
                        f7 = 0;
                        $("#RePasswordError").html("Please enter confirm password");
                    }
                    else if ($("#Password").val() != $("#RePassword").val()) {
                        f7 = 0;
                        $("#RePasswordError").html("Passwords didn't match");
                    }
                    else {
                        f7 = 1;
                        $("#RePasswordError").html("");
                    }
                });

            

    $(".form-control")
                .keyup(function () {
                    console.log('ss' + f1 + f2 + f3 + f4 + f5 + f6 + f7 );
                       //console.log('ssss' + f1 + f2);
                    if (f1 && f2 && f3 && f4 && f5 && f6 && f7) {
                  
                        $('#subbtn').removeAttr('disabled');
                        //$('#subbtn').tooltip('hide')
                    }
                    else
                        $('#subbtn').attr('disabled', 'disabled');
                    // $('#subbtn').tooltip('show')

                });



        });
