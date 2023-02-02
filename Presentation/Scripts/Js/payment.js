focusId = null;
var isSaveContactData = true;
var isSaveTravelerData = true;
p = {
    Tabs: {
        finalSubmit: 0,
        ContactDetails: 1,
        Travelers: 2,
        Payment: 3
    },
    InsChecks: {
        isTravelIns: false,
        isTravelProtection: false
    },
    PriceChangeAction: {
        Pending: 0,
        Accepted: 1,
        Rejected: 2,
    },
    setFocusId: function (_id) {
        if (focusId == null) {
            focusId = _id;
        }
    },
    validatePayment: function (_tabs) {
        var isSuccess = true;
        focusId = null;
        try {
            var c = false, t = false, p = false, b = false;
            switch (_tabs) {
                case this.Tabs.ContactDetails:
                    if (!this.validateContactDetails()) {
                        $(focusId).focus();
                        isSuccess = false;
                        window.scrollTo(0, 0);
                    }
                    else {
                        $(".step_1").hide()
                        $(".step_2").show()
                        $(".step1li").removeClass('active').addClass('complete');
                        $(".step2li").addClass('active');

                    }
                    break;
                case this.Tabs.Travelers:
                    if (!this.validateTravelers(PAX_COUNTER, TRV_DATE)) {
                        $(focusId).focus();
                        isSuccess = false;
                    }
                    else {
                        $(".step_2").hide()
                        $(".step_3").show()
                        $(".step2li").removeClass('active').addClass('complete');
                        $(".step3li").addClass('active');
                        window.scrollTo(0, 0);
                    }
                    break;
                case this.Tabs.Payment:
                    if (!this.validateContactDetails()) {
                        $(focusId).focus();
                        isSuccess = false;
                    }
                    else if (!this.validateTravelers(PAX_COUNTER, TRV_DATE)) {
                        $('#pass_nav,#TravelerInfo').addClass('in active');
                        $(focusId).focus();
                        isSuccess = false;
                        $(".error-div").show().html("Before going to make payment please fill all the information previous tabs!");
                        $("#flgihtValidatonPopup").modal();
                    }
                    else {

                        $('.tab-content div,#pay_add_nav_tab li').removeClass('in active');
                        $('#paymentinfo').removeClass('disabled').addClass('onpaymentinfo');
                        $('#TravelerInfo,#Review').addClass('done');
                        $('#addrs_nav,#paymentinfo').addClass('in active');
                        window.scrollTo(0, 0);

                        //if (isSaveTravelerData) {
                        //    isSaveTravelerData = false;
                        //    tracking.populateLeadTraveller($('#Travellers_0__FirstName').val());
                        //}
                        saveIncompleteBookingData();
                    }
                    break;
                case this.Tabs.finalSubmit:
                    c = this.validateContactDetails();
                    t = this.validateTravelers(PAX_COUNTER, TRV_DATE);
                    p = this.validatePaymentMethod();
                    b = this.validateBillingDtails();
                    var tnc = true;
                    if ($("#TermCondition").prop("checked") != true) {
                        $("#error_TermCondition").css("display", "block");
                        this.setFocusId($("#TermCondition"));
                        tnc = false;
                    }
                    else {
                        $("#error_TermCondition").css("display", "none");
                    }
                    if ((!t || !p || !b || !c || !tnc) && focusId != null) {
                        $(focusId).focus();
                        isSuccess = false;
                    }
                    else {
                        $('#payment').submit();
                    }
                    break;
            }

        } catch (e) {
            console.log("validatePayment|Tabs:" + _tabs);
        }
        return isSuccess;
    },
    switchTab: function (id) {
        switch (id) {
            case 1:
                $(".step_2").hide()
                $(".step2li").removeClass('active').removeClass('complete');
                $(".step_3").hide()
                $(".step3li").removeClass('active').removeClass('complete');

                $(".step_1").show()
                $(".step1li").removeClass('complete').addClass('active');
                break;
            case 2:
                if (!this.validateContactDetails()) {
                    $(focusId).focus();
                    isSuccess = false;
                    window.scrollTo(0, 0);
                }
                else {
                    $(".step_1").hide()
                    $(".step1li").removeClass('active');
                    $(".step_3").hide()
                    $(".step3li").removeClass('active').removeClass('complete');

                    $(".step_2").show()
                    $(".step2li").removeClass('complete').addClass('active');
                    $(".step1li").addClass('complete');
                    window.scrollTo(0, 0);
                }
                break;
            case 3:
                if (!this.validateTravelers(PAX_COUNTER, TRV_DATE)) {
                    $(focusId).focus();
                    isSuccess = false;
                    window.scrollTo(0, 0);
                }
                else {
                    $(".step_1").hide()
                    $(".step1li").removeClass('active');
                    $(".step_2").hide()
                    $(".step2li").removeClass('active');

                    $(".step_3").show()
                    $(".step3li").addClass('active');
                    $(".step2li").addClass('complete');
                    window.scrollTo(0, 0);
                }
                break;
        }

    },
    validateBillingDtails: function () {
        var isValid = true;
        try {
            var tempField;
            var country = $("#BillingDetails_Country").val().trim().toUpperCase();
            if (parseInt(country, 10) == 0) {
                $("#error_BillingDetails_Country").css("display", "block");
                $("#error_BillingDetails_Country").html("Select Billing Country");
                $("#div_error_BillingDetails_Country").removeClass("is-sucess").addClass("is-error");
                isValid = false;
                this.setFocusId($("#BillingDetails_Country"));
            }

            tempField = $("#BillingDetails_AddressLine1").val().trim();
            if (tempField == "") {
                $("#error_BillingDetails_AddressLine1").css("display", "block");
                $("#error_BillingDetails_AddressLine1").html("Enter Billing Address");
                $("#div_error_BillingDetails_AddressLine1").removeClass("is-sucess").addClass("is-error");
                $("#i_error_BillingDetails_AddressLine1").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_AddressLine1"));
            }
            tempField = $("#BillingDetails_City").val().trim();
            if (tempField == "") {
                $("#error_BillingDetails_City").css("display", "block");
                $("#error_BillingDetails_City").html("Enter Billing City Name</b>.");
                $("#div_error_BillingDetails_City").removeClass("is-sucess").addClass("is-error");
                $("#i_error_BillingDetails_City").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_City"));
            }

            if (country == "CA" || country == "US") {
                tempField = $("#BillingDetails_State").val().trim();
            }
            else {
                tempField = $("#BillingDetails_StateName").val().trim();
            }
            if (tempField == "") {
                $("#error_BillingDetails_State").css("display", "block");
                $("#error_BillingDetails_State").html("Enter Billing State Name");
                $("#div_error_BillingDetails_State").removeClass("is-sucess").addClass("is-error");
                if (country != "CA" && country != "US") {
                    $("#i_error_BillingDetails_State").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                }

                isValid = false;
                this.setFocusId($("#BillingDetails_StateName"));
            }

            tempField = $("#BillingDetails_ZipCode").val().trim();
            if (tempField == "") {
                $("#error_BillingDetails_ZipCode").css("display", "block");
                $("#error_BillingDetails_ZipCode").html("Enter Billing Zip/Post Code");
                $("#div_error_BillingDetails_ZipCode").removeClass("is-sucess").addClass("is-error");
                $("#i_error_BillingDetails_ZipCode").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_ZipCode"));
            }
        } catch (e) {
            console.log("validatePaymentMethod|" + e.toString());
        }
        return isValid;
    },
    validateContactDetails: function () {
        var isValid = true;
        try {
            var tempField;
            tempField = $("#BillingDetails_BillingPhone").val().trim();
            if (tempField == "" || tempField.length < 4 || !validation.isNumber(tempField)) {
                $("#error_BillingDetails_BillingPhone").css("display", "block");
                $("#error_BillingDetails_BillingPhone").html("Enter Billing Phone Number");
                $("#div_error_BillingDetails_BillingPhone").removeClass("is-sucess").addClass("is-error");
                $("#i_error_BillingDetails_BillingPhone").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_BillingPhone"));
            }
            tempField = $("#BillingDetails_Email").val().trim();
            if (tempField == "" || (tempField != "" && !validateEmail(tempField))) {
                $("#error_BillingDetails_Email").css("display", "block");
                $("#error_BillingDetails_Email").html("Enter Email Address");
                $("#div_error_BillingDetails_Email").removeClass("is-sucess").addClass("is-error");
                $("#i_error_BillingDetails_Email").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_Email"));
            }
        } catch (e) {
            console.log("validateContactDetails|" + e.toString());
        }
        return isValid;
    },
    validateTravelers: function (_noPax, _trvDate) {
        var isValid = true;
        try {
            var tempField;
            for (i = 0; i <= _noPax - 1; i++) {
                tempField = $("#Travellers_" + i + "__PaxType").val().trim();
                var paxtype = parseInt(tempField, 10);

                tempField = $("#Travellers_" + i + "__FirstName").val().trim();
                if (tempField == "" || !validation.isString(tempField)) {
                    $("#error_FirstName_" + i).css("display", "block");
                    $("#div_error_FirstName_" + i).removeClass("is-sucess").addClass("is-error");
                    $("#i_error_FirstName_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                    $("#error_FirstName_" + i).html("Enter First Name");
                    isValid = false;
                    this.setFocusId($("#Travellers_" + i + "__FirstName"));
                }
                else {
                    $("#error_FirstName_" + i).css("display", "none");
                    $("#div_error_FirstName_" + i).removeClass("is-error").addClass("is-sucess");
                    $("#i_error_FirstName_" + i).removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon");
                }

                tempField = $("#Travellers_" + i + "__LastName").val().trim();
                if (tempField == "" || !validation.isString(tempField)) {
                    $("#error_LastName_" + i).css("display", "block");
                    $("#div_error_LastName_" + i).removeClass("is-sucess").addClass("is-error");
                    $("#i_error_LastName_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                    $("#error_LastName_" + i).html("Enter Last Name");
                    isValid = false;
                    this.setFocusId($("#Travellers_" + i + "__LastName"));
                }
                else {
                    $("#error_LastName_" + i).css("display", "none");
                    $("#div_error_LastName_" + i).removeClass("is-error").addClass("is-sucess");
                    $("#i_error_LastName_" + i).removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon");
                }

                if (!this.validateDOB(i, _trvDate, paxtype)) {
                    isValid = false;
                }

            }

        } catch (e) {
            console.log("validateTravelers|" + e.toString());
        }
        return isValid;
    },
    validateDOB: function (_index, _trvDate, _paxtype) {
        var isValid = true;
        try {
            var tempField = "";
            var trvCounter = _index + 1;
            var mm = parseInt($("#Travellers_" + i + "__DOBMonth").val(), 10);
            var dd = parseInt($("#Travellers_" + i + "__DOBDay").val(), 10);
            var yy = parseInt($("#Travellers_" + i + "__DOBYear").val(), 10);
            if (mm == 0) {
                $("#error_Month_" + i).css("display", "block");
                $("#error_Month_" + i).html("Select Month of Birth");
                $("#div_error_Month_" + i).removeClass("is-sucess").addClass("is-error");
                isValid = false;
                this.setFocusId($("#Travellers_" + i + "__DOBMonth"));
            }


            var tempField = $("#Travellers_" + i + "__DOBDay").val().trim();
            if (dd > daysInMonth(mm, yy) || tempField == "" || (tempField != "" && isNaN(tempField)) || (parseInt(tempField, 10) < 1 || parseInt(tempField, 10) > 31)) {
                $("#error_Date_" + i).css("display", "block");
                $("#error_Date_" + i).html("Enter Day of Birth");
                $("#div_error_Date_" + i).removeClass("is-sucess").addClass("is-error");
                $("#i_error_Date_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#Travellers_" + i + "__DOBDay"));
            }

            var tempField = $("#Travellers_" + i + "__DOBYear").val().trim();
            if (tempField == "" || (tempField != "" && tempField.length < 4) || (tempField != "" && isNaN(tempField))) {
                $("#error_Year_" + i).css("display", "block");
                $("#error_Year_" + i).html(" Enter Year Birth");
                $("#div_error_Year_" + i).removeClass("is-sucess").addClass("is-error");
                $("#i_error_Year_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#Travellers_" + i + "__DOBYear"));
            }

            var mdays = this.getDayOfMonth((mm - 1), yy);


            if (isValid) {
                if (dd > mdays) {
                    $("#error_Date_" + i).css("display", "block");
                    $("#error_Date_" + i).html("Please enter <b>Valid Day of Date Of Birth for traveler #" + trvCounter + "</b>.");
                    $("#div_error_Year_" + i).removeClass("is-sucess").addClass("is-error");
                    $("#i_error_Year_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                    isValid = false;
                    this.setFocusId($("#Travellers_" + i + "__DOBDay"));
                }
                else {
                    var dob = new Date(yy, (mm - 1), dd);
                    if (!this.isValidDobAgainstTravelDete(dob, _trvDate, _paxtype, i)) {
                        isValid = false;
                    }
                    else {
                        var yearDiff = this.getYearFromDate(dob, _trvDate);
                        switch (_paxtype) {
                            case 3: //child   2-12 731-4383
                                if (yearDiff < 730 || yearDiff > 4383) {
                                    $("#error_Year_" + i).css("display", "block");
                                    $("#error_Year_" + i).html("Age of Child, <b> on departure date, should be greater than or equal to 2 years and less than 12 years</b> for (Traveler #" + (i + 1) + ").");
                                    $("#div_error_Year_" + i).removeClass("is-sucess").addClass("is-error");
                                    $("#i_error_Year_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                                    isValid = false;
                                    this.setFocusId($("#Travellers_" + i + "__DOBYear"));
                                }
                                break;
                            case 4://inf on seat
                            case 5://inf on lap
                                if (yearDiff <= 0 || yearDiff >= 730) {
                                    $("#error_Year_" + i).css("display", "block");
                                    $("#error_Year_" + i).html("Age of Infant,  <b>on departure date, should not be greater than 2yrs</b> for (Traveler #" + (i + 1) + ").");
                                    $("#div_error_Year_" + i).removeClass("is-sucess").addClass("is-error");
                                    $("#i_error_Year_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                                    isValid = false;
                                    this.setFocusId($("#Travellers_" + i + "__DOBYear"));
                                }
                                break;
                            default:
                                if (yearDiff < 4383 || yearDiff > 36524) {
                                    $("#error_Year_" + i).css("display", "block");
                                    $("#error_Year_" + i).html("Please provide a <b>Valid Date Of Birth</b> for adult(Traveler #" + (i + 1) + ").");
                                    $("#div_error_Year_" + i).removeClass("is-sucess").addClass("is-error");
                                    $("#i_error_Year_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                                    isValid = false;
                                    this.setFocusId($("#Travellers_" + i + "__DOBYear"));
                                }
                                break;
                        }
                    }
                }

            }

        } catch (e) {
            console.log("validateDOB|" + e.toString());
        }
        return isValid;
    },
    isValidDobAgainstTravelDete: function (_dob, _trvDate, _paxtype, _index) {
        var isValid = true;
        try {
            if (_dob > _trvDate) {
                $("#error_Year_" + i).css("display", "block");
                $("#error_Year_" + i).html(" + The <b>date of birth</b> that you have entered for traveler # " + (_index + 1) + " <b>occurs in the future</b>. Please enter a valid date.");
                isValid = false;
                this.setFocusId($("#Travellers_" + i + "__DOBYear"));
            }

        } catch (e) {
            isValid = false;
            console.log("isValidDate|" + e.toString());
        }
        return isValid;
    },
    getDayOfMonth: function (_mm, _yyyy) {
        switch (_mm) {
            case 1:
                return (_yyyy % 4 == 0 && _yyyy % 100) || _yyyy % 400 == 0 ? 29 : 28;
            case 8: case 3: case 5: case 10:
                return 30;
            default:
                return 31
        }
    },
    getYearFromDate: function (_dob, _tdate) {

        var diff = parseInt((_tdate - _dob) / (1000 * 60 * 60 * 24));
        //diff /= (60 * 60 * 24);
        //return Math.abs(Math.round(diff / 365.25));
        return diff

    },
    validatePaymentMethod: function () {
        var isValid = true;
        try {
            var tempField;
            tempField = $("#BillingDetails_CardType").val().trim();
            if (parseInt(tempField, 10) == 0) {
                $("#error_CardType").css("display", "block");
                $("#error_CardType").html(" + Please provide your <b>payment method</b>.");
                isValid = false;
                this.setFocusId($("#BillingDetails_CardType"));
            }
            if (cardFormValidate() == 0) {
                isValid = false;
                this.setFocusId($("#BillingDetails_CardNumber"));
            }

            tempField = $("#BillingDetails_CCHolderName").val().trim();
            if (tempField == "") {
                $("#error_CCHolderName").css("display", "block");
                $("#error_CCHolderName").html("Enter Card Holder Name");
                $("#div_error_CCHolderName").removeClass("is-sucess").addClass("is-error");
                $("#i_error_CCHolderName").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_CCHolderName"));
            } else if (!hasWhiteSpace(tempField)) {
                $("#error_CCHolderNameWithSpace").css("display", "block");
                $("#error_CCHolderNameWithSpace").html("Account holder's name must contain First name and Last name<\/b>.")
                $("#div_error_CCHolderName").removeClass("is-sucess").addClass("is-error");
                $("#i_error_CCHolderName").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_CCHolderName"));
            }

            tempField = $("#BillingDetails_ExpiryMonth").val().trim();
            if (parseInt(tempField, 10) == 0) {
                $("#error_ExpiryMonth").css("display", "block");
                $("#error_ExpiryMonth").html("Select Card Expiration Month");
                $("#div_error_ExpiryMonth").removeClass("is-sucess").addClass("is-error");
                isValid = false;
                this.setFocusId($("#BillingDetails_ExpiryMonth"));
            }

            tempField = $("#BillingDetails_ExpiryYear").val().trim();
            if (parseInt(tempField, 10) == 0) {
                $("#error_ExpiryYear").css("display", "block");
                $("#error_ExpiryYear").html("Enter Card Expiration Year");
                $("#div_error_ExpiryYear").removeClass("is-sucess").addClass("is-error");
                $("#i_error_ExpiryYear").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_ExpiryYear"));
            }

            tempField = $("#BillingDetails_CVVNumber").val().trim();
            if (tempField == "") {
                $("#error_CVVNumber").css("display", "block");
                $("#error_CVVNumber").html("Enter Card Verification Number");
                $("#div_error_CVVNumber").removeClass("is-sucess").addClass("is-error");
                $("#i_error_CVVNumber").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                isValid = false;
                this.setFocusId($("#BillingDetails_CVVNumber"));
            }
        } catch (e) {
            console.log("validatePaymentMethod|" + e.toString());
        }
        return isValid;
    },
    getCardType: function (n) {
        return n.match("^(34|37)") ? 3 : n.match("^(51|52|53|54|55|22|23|24|25|26|27)") ? 2 : n.match("^(4)") ? 1 : n.match("^(300|301|302|303|304|305|36|38)") || n.match("^(2014|2149)") ? 4 : n.match("^(6011)") ? 5 : n.match("^(3)") || n.match("^(2131|1800)") ? 8 : void 0
    },
    acceptHigherPrice: function (id) {
        try {
            var url = DOMAIN_URL + "flights/rebookhigherprice/" + id;
            $.ajax({
                url: url,
                type: 'GET',
                success: function (response) {
                    if (response != null && !response.IsContextExist) {
                        window.location.href = "/";
                    } else if (response != null && response.IsContextExist) {
                        window.location.href = response.RedirectUrl;
                    }
                }
            });
        } catch (e) {
            console.log("acceptHigherPrice" + e.toString());
        }
    },
    addBookingFailure: function (_id, _action, _price) {
        try {
            var action;
            if (_action == this.PriceChangeAction.Accepted) {
                action = _price + '|Yes';
            } else {
                action = _price + '|No';
            }
            //tracking.populateActionSoldout(action)
            //var url = DOMAIN_URL + "flights/addbookingfailure";
            //$.ajax({
            //    url: url,
            //    type: 'POST',
            //    cache: false,
            //    dataType: "json",
            //    data: { Guid: _id, PriceChangeAction: _action },
            //    success: function (response) {
            //        if (response != null) {

            //        } else if (response != null) {

            //        }
            //    }
            //});
        } catch (e) {
            console.log("addBookingFailure" + e.toString());
        }
    }
};
function setStateOnCountry() {
    if ($("#BillingDetails_Country").val().toUpperCase() != "US" && $("#BillingDetails_Country").val().toUpperCase() != "CA") {
        $("#BillingDetails_StateName").css('display', 'block');
        $("#BillingDetails_State").css('display', 'none');
        $("#divState").removeClass('select-class');
    }
    else {
        $("#BillingDetails_StateName").css('display', 'none');
        $("#BillingDetails_State").css('display', 'block');
        $("#divState").removeClass('select-class').addClass('select-class');
        var url = DOMAIN_URL + "flights/getstates";
        var ddltarget = "#BillingDetails_State";
        jQuery.getJSON(url, { _countryCode: $("#BillingDetails_Country").val() }, function (data) {
            $(ddltarget).empty();
            $.each(data, function (index, optionData) {
                $(ddltarget).append("<option value='" + optionData["Code"] + "'>" + optionData["Name"] + "</option>");
                if (index == 0) {
                    $(ddltarget).val(optionData["Code"]);
                }
            });
        });
    }
}
$("#BillingDetails_Country").change(function () {
    setStateOnCountry();
});
$("#TermCondition").click(function () {
    if ($(this).prop("checked") != true) {
        $("#error_TermCondition").css("display", "block");
    }
    else {
        $("#error_TermCondition").css("display", "none");
    }
});
function onBlurValidateTraveller(n, t) {
    var u = n.id,
        i = parseInt(u.match(/\d+/), 10),
        r = $(n).val().trim();
    t = t.toLowerCase();
    switch (t) {
        case "firstname":
            r == "" ? ($("#error_FirstName_" + i).css("display", "block"), $("#error_FirstName_" + i).html("Enter First Name"), $("#div_error_FirstName_" + i).removeClass("is-sucess").addClass("is-error"), $("#i_error_FirstName_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_FirstName_" + i).css("display", "none"), $("#error_FirstName_" + i).html(""), $("#div_error_FirstName_" + i).removeClass("is-error").addClass("is-sucess"), $("#i_error_FirstName_" + i).removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"),saveIncompleteBookingData());
            break;
        case "lastname":
            r == "" ? ($("#error_LastName_" + i).css("display", "block"), $("#error_LastName_" + i).html("Enter Last Name"), $("#div_error_LastName_" + i).removeClass("is-sucess").addClass("is-error"), $("#i_error_LastName_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_LastName_" + i).css("display", "none"), $("#error_LastName_" + i).html(""), $("#div_error_LastName_" + i).removeClass("is-error").addClass("is-sucess"), $("#i_error_LastName_" + i).removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"),saveIncompleteBookingData());
            break;
        case "gender":
            parseInt(r, 10) == 0 ? ($("#error_Gender_" + i).css("display", "block"), $("#error_Gender_" + i).html("Choose Gender")) : ($("#error_Gender_" + i).css("display", "none"), $("#error_Gender_" + i).html(""))
            break;
        case "month":
            parseInt(r, 10) == 0 ? ($("#error_Month_" + i).css("display", "block"), $("#error_Month_" + i).html("Select Month Birth"), $("#div_error_Month_" + i).addClass("is-error")) : ($("#error_Month_" + i).css("display", "none"), $("#error_Month_" + i).html(""), $("#div_error_Month_" + i).removeClass("is-error"))
            break;
        case "day":
            (parseInt(r, 10) < 1 || parseInt(r, 10) > 31) ? ($("#error_Date_" + i).css("display", "block"), $("#error_Date_" + i).html("Enter Day Of Birth"), $("#div_error_Date_" + i).removeClass("is-sucess").addClass("is-error"), $("#i_error_Date_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_Date_" + i).css("display", "none"), $("#error_Date_" + i).html(""), $("#div_error_Date_" + i).removeClass("is-error").addClass("is-sucess"), $("#i_error_Date_" + i).removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"))
            break;
        case "year":
            r.length < 4 ? ($("#error_Year_" + i).css("display", "block"), $("#error_Year_" + i).html("Enter Year of Birth"), $("#div_error_Year_" + i).removeClass("is-sucess").addClass("is-error"), $("#i_error_Year_" + i).removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_Year_" + i).css("display", "none"), $("#error_Year_" + i).html(""), $("#div_error_Year_" + i).removeClass("is-error").addClass("is-sucess"), $("#i_error_Year_" + i).removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"))
            break;

    }
}
function onBlurPaymentDetails(n, t) {
    try {
        tempField = $(n).val().trim();
        switch (t) {
            case "cardtype":
                parseInt(tempField, 10) == 0 ? ($("#error_CardType").css("display", "block"), $("#error_CardType").html(" + Please provide your <b>payment method<\/b>.")) : ($("#error_CardType").css("display", "none"), $("#error_CardType").html(""));
                break;
            case "cardnumber":
                cardFormValidate();
                break;
            case "ccholdername":
                $("#error_CCHolderName").css("display", "none"), $("#error_CCHolderName").html("");
                $("#error_CCHolderNameWithSpace").css("display", "none"), $("#error_CCHolderNameWithSpace").html("");
                if (tempField == "") {
                    $("#error_CCHolderName").css("display", "block");
                    $("#error_CCHolderName").html("Enter Card Holder Name");
                    $("#div_error_CCHolderName").removeClass("is-sucess").addClass("is-error");
                    $("#i_error_CCHolderName").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                }
                else if (!hasWhiteSpace(tempField)) {
                    $("#error_CCHolderNameWithSpace").css("display", "block");
                    $("#error_CCHolderNameWithSpace").html("Account holder's name must contain First name and Last name")
                    $("#div_error_CCHolderName").removeClass("is-sucess").addClass("is-error");
                    $("#i_error_CCHolderName").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                }
                else {
                    $("#div_error_CCHolderName").removeClass("is-error").addClass("is-sucess");
                    $("#i_error_CCHolderName").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon");
                    saveIncompleteBookingData();
                }
                
                break;
            case "expirymonth":
                parseInt(tempField, 10) == 0 ? ($("#error_ExpiryMonth").css("display", "block"), $("#error_ExpiryMonth").html("Select Card Expiration Month"), $("#div_error_ExpiryMonth").removeClass("is-sucess").addClass("is-error")) : ($("#error_ExpiryMonth").css("display", "none"), $("#error_ExpiryMonth").html(""), $("#div_error_ExpiryMonth").removeClass("is-error").addClass("is_sucess"));
                break;
            case "expiryyear":
                parseInt(tempField, 10) == 0 ? ($("#error_ExpiryYear").css("display", "block"), $("#error_ExpiryYear").html("Select Card Expiration Year"), $("#div_error_ExpiryYear").removeClass("is-sucess").addClass("is-error")) : ($("#error_ExpiryYear").css("display", "none"), $("#error_ExpiryYear").html(""), $("#div_error_ExpiryYear").removeClass("is-error").addClass("is_sucess"));
                break;
            case "cvv":
                if (tempField == "") {
                    $("#error_CVVNumber").css("display", "block");
                    $("#error_CVVNumber").html("Enter Card Verification Number");
                    $("#div_error_CVVNumber").removeClass("is-sucess").addClass("is-error");
                    $("#i_error_CVVNumber").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                }
                else if (tempField.length <= 2) {
                    $("#error_CVVNumber").css("display", "block");
                    $("#error_CVVNumber").html("Enter Valid Card Verification Number");
                    $("#div_error_CVVNumber").removeClass("is-sucess").addClass("is-error");
                    $("#i_error_CVVNumber").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
                }
                else {
                    $("#error_CVVNumber").css("display", "none");
                    $("#error_CVVNumber").html("");
                    $("#div_error_CVVNumber").removeClass("is-error").addClass("is-sucess");
                    $("#i_error_CVVNumber").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon");
                }
        }
    } catch (i) {
        console.log("validatePaymentDetails" + i.toString())
    }
}
function onBlurValidateBilling(n, t) {
    t = t.toLowerCase();
    var i = $(n).val().trim();
    switch (t) {
        case "country":
            parseInt(i, 10) == 0 ? ($("#error_BillingDetails_Country").css("display", "block"), $("#error_BillingDetails_Country").html("Select Billing Country"), $("#div_error_BillingDetails_Country").removeClass("is-sucess").addClass("is-error")) : ($("#error_BillingDetails_Country").css("display", "none"), $("#error_BillingDetails_Country").html(""), $("#div_error_BillingDetails_Country").removeClass("is-error").addClass("is-sucess"));
            break;
        case "addressline1":
            i == "" ? ($("#error_BillingDetails_AddressLine1").css("display", "block"), $("#error_BillingDetails_AddressLine1").html("Enter Billing Address"), $("#div_error_BillingDetails_AddressLine1").removeClass("is-sucess").addClass("is-error"), $("#i_error_BillingDetails_AddressLine1").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_BillingDetails_AddressLine1").css("display", "none"), $("#error_BillingDetails_AddressLine1").html(""), $("#div_error_BillingDetails_AddressLine1").removeClass("is-error").addClass("is-sucess"), $("#i_error_BillingDetails_AddressLine1").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"));
            break;
        case "city":
            i == "" ? ($("#error_BillingDetails_City").css("display", "block"), $("#error_BillingDetails_City").html("Enter Billing City Name"), $("#div_error_BillingDetails_City").removeClass("is-sucess").addClass("is-error"), $("#i_error_BillingDetails_City").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_BillingDetails_City").css("display", "none"), $("#error_BillingDetails_City").html(""), $("#div_error_BillingDetails_City").removeClass("is-error").addClass("is-sucess"), $("#i_error_BillingDetails_City").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"));
            break;
        case "statename":
            i == "" ? ($("#error_BillingDetails_State").css("display", "block"), $("#error_BillingDetails_State").html("Enter Billing State Name"), $("#div_error_BillingDetails_State").removeClass("is-sucess").addClass("is-error"), $("#i_error_BillingDetails_State").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_BillingDetails_State").css("display", "none"), $("#error_BillingDetails_State").html(""), $("#div_error_BillingDetails_State").removeClass("is-error").addClass("is-sucess"), $("#i_error_BillingDetails_State").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"));
            break;
        case "zipcode":
            i == "" ? ($("#error_BillingDetails_ZipCode").css("display", "block"), $("#error_BillingDetails_ZipCode").html("Enter Billing Zip/Post Code"), $("#div_error_BillingDetails_ZipCode").removeClass("is-sucess").addClass("is-error"), $("#i_error_BillingDetails_ZipCode").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_BillingDetails_ZipCode").css("display", "none"), $("#error_BillingDetails_ZipCode").html(""), $("#div_error_BillingDetails_ZipCode").removeClass("is-error").addClass("is-sucess"), $("#i_error_BillingDetails_ZipCode").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"),saveIncompleteBookingData())
            break;

    }
}
function onBlurValidateContact(n, t) {
    t = t.toLowerCase();
    var i = $(n).val().trim();
    switch (t) {
        case "billingphone":
            i == "" || i.length < 4 || !validation.isNumber(i) ? ($("#error_BillingDetails_BillingPhone").css("display", "block"), $("#error_BillingDetails_BillingPhone").html("Enter Billing Phone Number"), $("#div_error_BillingDetails_BillingPhone").removeClass("is-sucess").addClass("is-error"), $("#i_error_BillingDetails_BillingPhone").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon")) : ($("#error_BillingDetails_BillingPhone").css("display", "none"), $("#error_BillingDetails_BillingPhone").html(""), $("#div_error_BillingDetails_BillingPhone").removeClass("is-error").addClass("is-sucess"), $("#i_error_BillingDetails_BillingPhone").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon"),saveIncompleteBookingData());
            break;
        case "email":
            $("#error_BillingDetails_ConfirmEmail").css("display", "none"), $("#error_BillingDetails_ConfirmEmail").html("");

            if (i != "" && (i == "" || validateEmail(i))) {
                $("#error_BillingDetails_Email").css("display", "none");
                $("#error_BillingDetails_Email").html("");
                $("#div_error_BillingDetails_Email").removeClass("is-error").addClass("is-sucess");
                $("#i_error_BillingDetails_Email").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon");
                saveIncompleteBookingData();
            }
            else {
                $("#error_BillingDetails_Email").css("display", "block");
                $("#error_BillingDetails_Email").html("Enter an Email Address");
                $("#div_error_BillingDetails_Email").removeClass("is-sucess").addClass("is-error");
                $("#i_error_BillingDetails_Email").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
            }
            break;


    }
}
function isNumberWithDot(n, t) {
    n = n ? n : window.event;
    var i = n.which ? n.which : n.keyCode;
    return i > 45 && i < 58 || i == 8 ? i === 46 && $(t).val().split(".").length === 2 ? !1 : !0 : !1
}
function isNumeric(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
};
function lettersWithCharOnly(n) {
    var regex = /^[a-zA-Z0-9]*$/;
    if (!regex.test(event.key)) {
        return false
    }
    else {
        return true
    }
}
function lettersWithSpaceOnly(n) {
    var regex = /^[a-zA-Z\s]*$/;
    if (!regex.test(event.key)) {
        return false
    }
    else {
        return true
    }
}
function lettersWithSpaceWithApostOnly(n) {
    var regex = /^[a-zA-Z\s']*$/;
    if (!regex.test(event.key)) {
        return false
    }
    else {
        return true
    }
}
function getValidPaxName(n) {
    var t = $(n).val().replace(/[^a-zA-Z'\s]/g, "");
    $(n).val(t)
}
function validateEmail(n) {
    return /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/.test(n)
}

function hasWhiteSpace(s) {
    return /\s/g.test(s);
}

var validation = {
    isEmailAddress: function (str) {
        var pattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
        return pattern.test(str);  // returns a boolean
    },
    isNotEmpty: function (str) {
        var pattern = /\S+/;
        return pattern.test(str);  // returns a boolean
    },
    isNumber: function (str) {
        var pattern = /^\d+$/;
        return pattern.test(str);  // returns a boolean
    },
    isConfirmMail: function (_email, _confirmemail) {
        return _email === _confirmemail;
    }, isString: function (str) {
        var pattern = /^[a-zA-Z\s]*$/;
        return pattern.test(str);  // returns a boolean
    }
};

$(document).idle({
    onIdle: function () {
        $('#PaymentIdlePopup').show();
    },
    idle: 900000
})

$('#BillingDetails_CardNumber').on('keypress change blur', function () {
    $(this).val(function (index, value) {
        return value.replace(/[^0-9]+/gi, '').replace(/(.{4})/g, '$1 ');
    });
});
$('#BillingDetails_CardNumber').on('copy cut paste', function () {
    setTimeout(function () {
        $('#BillingDetails_CardNumber').trigger("change");
    });
});
$(window).scroll(function () { var height = $(window).scrollTop(); if (height > 164) { $("#fix_rgt_ssl").css({ "position": "fixed", "top": "15px" }); } else { $("#fix_rgt_ssl").css({ "top": "164px", "position": "absolute" }); } });


/************************************************************/
(function () {
    var $, Range, Trie,
        indexOf = [].indexOf || function (item) { for (var i = 0, l = this.length; i < l; i++) { if (i in this && this[i] === item) return i; } return -1; };

    Trie = (function () {
        function Trie() {
            this.trie = {};
        }

        Trie.prototype.push = function (value) {
            var char, i, j, len, obj, ref, results;
            value = value.toString();
            obj = this.trie;
            ref = value.split('');
            results = [];
            for (i = j = 0, len = ref.length; j < len; i = ++j) {
                char = ref[i];
                if (obj[char] == null) {
                    if (i === (value.length - 1)) {
                        obj[char] = null;
                    } else {
                        obj[char] = {};
                    }
                }
                results.push(obj = obj[char]);
            }
            return results;
        };

        Trie.prototype.find = function (value) {
            var char, i, j, len, obj, ref;
            value = value.toString();
            obj = this.trie;
            ref = value.split('');
            for (i = j = 0, len = ref.length; j < len; i = ++j) {
                char = ref[i];
                if (obj.hasOwnProperty(char)) {
                    if (obj[char] === null) {
                        return true;
                    }
                } else {
                    return false;
                }
                obj = obj[char];
            }
        };

        return Trie;

    })();

    Range = (function () {
        function Range(trie1) {
            this.trie = trie1;
            if (this.trie.constructor !== Trie) {
                throw Error('Range constructor requires a Trie parameter');
            }
        }

        Range.rangeWithString = function (ranges) {
            var j, k, len, n, r, range, ref, ref1, trie;
            if (typeof ranges !== 'string') {
                throw Error('rangeWithString requires a string parameter');
            }
            ranges = ranges.replace(/ /g, '');
            ranges = ranges.split(',');
            trie = new Trie;
            for (j = 0, len = ranges.length; j < len; j++) {
                range = ranges[j];
                if (r = range.match(/^(\d+)-(\d+)$/)) {
                    for (n = k = ref = r[1], ref1 = r[2]; ref <= ref1 ? k <= ref1 : k >= ref1; n = ref <= ref1 ? ++k : --k) {
                        trie.push(n);
                    }
                } else if (range.match(/^\d+$/)) {
                    trie.push(range);
                } else {
                    throw Error("Invalid range '" + r + "'");
                }
            }
            return new Range(trie);
        };

        Range.prototype.match = function (number) {
            return this.trie.find(number);
        };

        return Range;

    })();


    /*
    jQuery Credit Card Validator 1.1
    THE SOFTWARE.
    */
    $ = jQuery;

    $.fn.validateCreditCard = function (callback, options) {
        var bind, card, card_type, card_types, get_card_type, is_valid_length, is_valid_luhn, j, len, normalize, ref, validate, validate_number;
        card_types = [
            {
                name: 'amex',
                range: '34,37',
                valid_length: [15]
            }, {
                name: 'diners_club_carte_blanche',
                range: '300-305',
                valid_length: [14]
            }, {
                name: 'diners_club_international',
                range: '36',
                valid_length: [14]
            }, {
                name: 'jcb',
                range: '3528-3589',
                valid_length: [16]
            }, {
                name: 'laser',
                range: '6304, 6706, 6709, 6771',
                valid_length: [16, 17, 18, 19]
            }, {
                name: 'visa_electron',
                range: '4026, 417500, 4508, 4844, 4913, 4917',
                valid_length: [16]
            }, {
                name: 'visa',
                range: '4',
                valid_length: [13, 14, 15, 16, 17, 18, 19]
            }, {
                name: 'mastercard',
                range: '51-55,2221-2720',
                valid_length: [16]
            }, {
                name: 'discover',
                range: '6011, 622126-622925, 644-649, 65',
                valid_length: [16]
            }, {
                name: 'dankort',
                range: '5019',
                valid_length: [16]
            }, {
                name: 'maestro',
                range: '50, 56-69',
                valid_length: [12, 13, 14, 15, 16, 17, 18, 19]
            }, {
                name: 'uatp',
                range: '1',
                valid_length: [15]
            }
        ];
        bind = false;
        if (callback) {
            if (typeof callback === 'object') {
                options = callback;
                bind = false;
                callback = null;
            } else if (typeof callback === 'function') {
                bind = true;
            }
        }
        if (options == null) {
            options = {};
        }
        if (options.accept == null) {
            options.accept = (function () {
                var j, len, results;
                results = [];
                for (j = 0, len = card_types.length; j < len; j++) {
                    card = card_types[j];
                    results.push(card.name);
                }
                return results;
            })();
        }
        ref = options.accept;
        for (j = 0, len = ref.length; j < len; j++) {
            card_type = ref[j];
            if (indexOf.call((function () {
                var k, len1, results;
                results = [];
                for (k = 0, len1 = card_types.length; k < len1; k++) {
                    card = card_types[k];
                    results.push(card.name);
                }
                return results;
            })(), card_type) < 0) {
                throw Error("Credit card type '" + card_type + "' is not supported");
            }
        }
        get_card_type = function (number) {
            var k, len1, r, ref1;
            ref1 = (function () {
                var l, len1, ref1, results;
                results = [];
                for (l = 0, len1 = card_types.length; l < len1; l++) {
                    card = card_types[l];
                    if (ref1 = card.name, indexOf.call(options.accept, ref1) >= 0) {
                        results.push(card);
                    }
                }
                return results;
            })();
            for (k = 0, len1 = ref1.length; k < len1; k++) {
                card_type = ref1[k];
                r = Range.rangeWithString(card_type.range);
                if (r.match(number)) {
                    return card_type;
                }
            }
            return null;
        };
        is_valid_luhn = function (number) {
            var digit, k, len1, n, ref1, sum;
            sum = 0;
            ref1 = number.split('').reverse();
            for (n = k = 0, len1 = ref1.length; k < len1; n = ++k) {
                digit = ref1[n];
                digit = +digit;
                if (n % 2) {
                    digit *= 2;
                    if (digit < 10) {
                        sum += digit;
                    } else {
                        sum += digit - 9;
                    }
                } else {
                    sum += digit;
                }
            }
            return sum % 10 === 0;
        };
        is_valid_length = function (number, card_type) {
            var ref1;
            return ref1 = number.length, indexOf.call(card_type.valid_length, ref1) >= 0;
        };
        validate_number = function (number) {
            var length_valid, luhn_valid;
            card_type = get_card_type(number);
            luhn_valid = false;
            length_valid = false;
            if (card_type != null) {
                luhn_valid = is_valid_luhn(number);
                length_valid = is_valid_length(number, card_type);
            }
            return {
                card_type: card_type,
                valid: luhn_valid && length_valid,
                luhn_valid: luhn_valid,
                length_valid: length_valid
            };
        };
        validate = (function (_this) {
            return function () {
                var number;
                number = normalize($(_this).val());
                return validate_number(number);
            };
        })(this);
        normalize = function (number) {
            return number.replace(/[ -]/g, '');
        };
        if (!bind) {
            return validate();
        }
        this.on('input.jccv', (function (_this) {
            return function () {
                $(_this).off('keyup.jccv');
                return callback.call(_this, validate());
            };
        })(this));
        this.on('keyup.jccv', (function (_this) {
            return function () {
                return callback.call(_this, validate());
            };
        })(this));
        callback.call(this, validate());
        return this;
    };

}).call(this);
function cardFormValidate() {
    var cardValid = 0;
    var selectVale = 0;
    //card number validation
    $('#BillingDetails_CardNumber').validateCreditCard(function (result) {
        
        var cardType = (result.card_type == null) ? '' : result.card_type.name;
        if (result.valid && result.length_valid && result.luhn_valid && cardType != "") {

            //  console.log('Card type: ' + (result.card_type == null ? '-' : result.card_type.name) + '<br>Valid: ' + result.valid + '<br>Length valid: ' + result.length_valid + '<br>Luhn valid: true ' + result.luhn_valid);
            $("#error_CardNumber").css("display", "none");
            $("#error_CardNumber").html("");
            $("#div_error_CardNumber").removeClass("is-error").addClass("is-sucess");
            $("#i_error_CardNumber").removeClass("bi bi-exclamation-circle icon").addClass("bi bi-check-circle icon");

            if (cardType == 'visa') {
                var backPosition = result.valid ? 'right  -36px' : 'right  1px';
                var selectVale = 1;
            } else if (cardType == 'visa_electron') {
                var backPosition = result.valid ? 'right  -224px' : 'right  1px';
                var selectVale = 6;
            } else if (cardType == 'mastercard') {
                var backPosition = result.valid ? 'right  -73px' : 'right  1px';
                var selectVale = 2;
            } else if (cardType == 'maestro') {
                var backPosition = result.valid ? 'right  -261px' : 'right  1px';
                var selectVale = 7;
            } else if (cardType == 'discover') {
                var backPosition = result.valid ? 'right  -185px' : 'right  1px';
                var selectVale = 5;
            } else if (cardType == 'amex') {
                var backPosition = result.valid ? 'right  -111px' : 'right  1px';
                var selectVale = 3;
            } else if (cardType == 'jcb') {
                var backPosition = result.valid ? 'right  -337px' : 'right  1px';
                var selectVale = 9;
            } else if (cardType == 'diners_club_carte_blanche' || cardType == 'diners_club_international') {
                var backPosition = result.valid ? 'right  -149px' : 'right  1px';
                var selectVale = 4;
            } else {
                var backPosition = result.valid ? 'right  1px' : 'right  1px';
                var selectVale = 0;
            }

            $('#BillingDetails_CardType').val(selectVale);
            $('#BillingDetails_CardNumber').css("background-position", backPosition);
            cardValid = 1;

        }
        else {
            $('#BillingDetails_CardType').val(0)
            console.log('Card type: ' + (result.card_type == null ? '-' : result.card_type.name) + '<br>Valid: ' + result.valid + '<br>Length valid: ' + result.length_valid + '<br>Luhn valid: false' + result.luhn_valid);
            $("#error_CardNumber").css("display", "block");
            $("#error_CardNumber").html("Enter Credit or Debit Card Number");
            $("#div_error_CardNumber").removeClass("is-sucess").addClass("is-error");
            $("#i_error_CardNumber").removeClass("bi bi-check-circle icon").addClass("bi bi-exclamation-circle icon");
            $('#BillingDetails_CardNumber').css("background-position", '2px 0px, 260px -61px');
            cardValid = 0;
        }
    });
    return cardValid;
}
$("body").delegate(".onTravelerInfo", "click", function () {
    return p.validatePayment(2);
});
$("body").delegate(".onpaymentinfo", "click", function () {
    return p.validatePayment(3);
});

function addContactData(guid, phone, email, countryCode, areaCode) {
    var url = DOMAIN_URL + "flights/add-contactdata";
    $.ajax({
        type: "POST",
        data: { Guid: guid, Phone: phone, Email: email, CountryCode: countryCode, AreaCode: areaCode },
        url: url,
        success: function (response) {
        },
        error: function (error) {
            console.log(error);
        }
    });
}
$('#BegProtecton .radio_cmmn').on('change', function () {
    var selValue = $("input[name='BagInsuranc.BagInsuranceType']:checked").val();
    var url = DOMAIN_URL + "flights/getbageinsuranceprice";
    $.ajax({
        type: "POST",
        data: { id: $('#guid').val(), bagInsuranceType: selValue },
        url: url,
        success: function (response) {
            if (response.IsSuccess) {
                $("#price12").html(response.HtmlResponse);
                $("#price13").html(response.HtmlResponse);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});
function daysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
}
function isValidDate(dateString) {
    var regEx = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateString.match(regEx)) return false;  // Invalid format
    var d = new Date(dateString);
    var dNum = d.getTime();
    if (!dNum && dNum !== 0) return false; // NaN value, Invalid date
    return d.toISOString().slice(0, 10) === dateString;
}
function valiDateFutureDate(futureDate, TodayDate) {
    debugger;
    if (Date.parse(futureDate) > Date.parse(TodayDate)) {
        return true;
    }
    return false;
}
$('.isNumericClass').on('copy cut paste', function () {
    setTimeout(function () {
        $('.isNumericClass').trigger("change");
    });
});
$('.isNumericClass').on('keypress change blur', function () {
    $(this).val(function (index, value) {
        return value.replace(/[^0-9]+/gi, '');
    });
});
$('.islettersWithSpaceOnly').on('copy cut paste', function () {
    setTimeout(function () {
        $('.islettersWithSpaceOnly').trigger("change");
    });
});
$('.islettersWithSpaceOnly').on('keypress change blur', function () {
    $(this).val(function (index, value) {
        return value.match(/^[a-zA-Z\s]*$/gi);
    });
});

$(".py_begshwhide").click(function () {
    $(".py_disc").slideToggle("slow");
    if ($(".py_begshwhide i").hasClass("glyphicon-menu-down")) {
        $(".py_begshwhide i").removeClass("glyphicon-menu-down").addClass("glyphicon-menu-up");
    } else {
        $(".py_begshwhide i").removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down");
    }
    var txtval = $('.py_begshwhide bdo').text();
    if (txtval.match(/show more/gi)) {
        $('.py_begshwhide bdo').html("Show Less");
    } else {
        $('.py_begshwhide bdo').html("Show More");
    }
});

$(".itanary-details-button").click(function () {
    $("#itenary-deail-toggle").slideToggle("slow");
    if ($(".itanary-details-button").hasClass("menu-icon")) {
        $(".itanary-details-button").removeClass("menu-icon");
    } else {
        $(".itanary-details-button").addClass("menu-icon");
    }
    var txtval = $('.itanary-details-button').text();
    if (txtval.match(/Show Details/gi)) {
        $('.itanary-details-button').html("Hide Details");
    } else {
        $('.itanary-details-button').html("Show Details");
    }
});

$(".itnery-toggle-btn").click(function () {
    $("#itnery-toggle-mbl").slideToggle("slow");
    $("#itenary-deail-toggle").slideUp("slow");
    $('.itanary-details-button').html("Show Details");
    $('.itanary-details-button').removeClass('menu-icon');
    if ($(".itnery-toggle-btn").hasClass("menu-toggle")) {
        $(".itnery-toggle-btn").removeClass("menu-toggle");
    } else {
        $(".itnery-toggle-btn").addClass("menu-toggle");
    }
});
//$("#mobile-next-step").click(function () {
//    if (true) {
//        $("#payment-info").show();
//        $("#itenary-info-detals").hide();
//    }

//});
$("#price-detial-modal").click(function () {
    $('#price-popup').modal('show')
});








$(".py_triphwhide").click(function () {
    $(".trip_disc").slideToggle("slow");
    if ($(".py_triphwhide i").hasClass("glyphicon-menu-down")) {
        $(".py_triphwhide i").removeClass("glyphicon-menu-down").addClass("glyphicon-menu-up");
    } else {
        $(".py_triphwhide i").removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down");
    }
    var txtval = $('.py_triphwhide bdo').text();
    if (txtval.match(/show more/gi)) {
        $('.py_triphwhide bdo').html("Show Less");
    } else {
        $('.py_triphwhide bdo').html("Show More");
    }
});

function shiftingdiv() {
    if ($(window).width() < 990) {
        $('#main_price').insertBefore('#add_block');
    }
}

$(document).ready(function () {
    shiftingdiv();
});
if ($(window).width() > 960) {
    $(window).scroll(function () {
        var scroll = $(window).scrollTop();

        if (scroll >= 140) {
            $(".right-section").addClass("fixed-right");
        } else {
            $(".right-section").removeClass("fixed-right");
        }

    });

}
$(".hide-toggle").click(function () {
    $(".toggle-section").slideToggle();
    $(this).toggleClass("show-toggle");


});

function populateTravellers(_noPax) {
    var nameLI = "";
    for (i = 0; i <= _noPax - 1; i++) {
        nameLI = nameLI + "<li> Pasenger " + (i + 1) + ": <strong>" + $("#Travellers_" + i + "__FirstName").val().trim() + " " + $("#Travellers_" + i + "__MiddleName").val().trim() + " " + $("#Travellers_" + i + "__LastName").val().trim() + "</strong> (make changes)</li>";
    }
    $("#ulTravellerList").html(nameLI);
}

$('#ExtendedCancellation .radio_cmmn').on('change', function () {
    var selValue = $("input[name='ExtendedCancellationOption']:checked").val();
    var url = DOMAIN_URL + "flights/getextendedcancellation";
    $.ajax({
        type: "POST",
        data: { id: $('#guid').val(), isChecked: selValue },
        url: url,
        success: function (response) {
            if (response.IsSuccess) {
                $("#price12").html(response.HtmlResponse);
                var scroll = $(window).scrollTop();

                if (scroll >= 140) {
                    $(".right-section").addClass("fixed-right");
                } else {
                    $(".right-section").removeClass("fixed-right");
                }
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});
$('#TravelProtectionIns .radio_cmmn').on('change', function () {
    extendedCancellation();
});

function saveIncompleteBookingData() {

    var url = DOMAIN_URL + "flights/save-incomplete-booking";

    $.ajax({
        type: "POST",
        data: $("#payment").serializeFormJSON(),
        url: url,
        success: function (response) {
            console.log(response);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//$(document).ready(function () {
//    extendedCancellation();
//});

function extendedCancellation() {
    var selValue = $("input[name='ExtendedCancellationOption']:checked").val();
    var url = DOMAIN_URL + "flights/getextendedcancellation";
    $.ajax({
        type: "POST",
        data: { id: $('#guid').val(), isChecked: selValue },
        url: url,
        success: function (response) {
            if (response.IsSuccess) {
                $("#price12").html(response.HtmlResponse);
                var scroll = $(window).scrollTop();

                if (scroll >= 140) {
                    $(".right-section").addClass("fixed-right");
                } else {
                    $(".right-section").removeClass("fixed-right");
                }
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function applyPromoCode() {
    var url = DOMAIN_URL + "flights/applypromocode";
    try {
        $.ajax({
            type: "POST",
            data: { id: $('#guid').val(), promoCode: $("#txtPromocode").val() },
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    $("#price12").html(response.HtmlResponse);
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    } catch (e) {
        console.log("Apply Promo Code" + e.toString());
    }
}
function removePromoCode() {
    var url = DOMAIN_URL + "flights/applypromocode";
    try {
        $.ajax({
            type: "POST",
            data: { id: $('#guid').val(), promoCode: '' },
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    $("#price12").html(response.HtmlResponse);
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    } catch (e) {
        console.log("Apply Promo Code" + e.toString());
    }
}

function applyCouponOnClick(coupon) {
    try {
        $("#txtPromocode").val(coupon);
        applyPromoCode();
    } catch (e) {
        console.log("Apply Promo Code" + e.toString());
    }
}

function step1() {
    $(".step_1").hide()
    $(".step_2").show()
    $(".step1li").removeClass('active').addClass('complete');
    $(".step2li").addClass('active');
}

function clickTabShow(id, tab) {
    $("#departTabcontent").hide();
    $("#returnTabcontent").hide();
    $("#" + id).show();
    $(".tabclick").removeClass("active");
    $("#" + tab).addClass('active');
}
$(document).ready(function () {

    $(".accordian-slide").hide();
    $('.accordian_toggle').click(function () {
        $(".accordian-slide").slideUp();
        $(".fa-angle-up").removeClass("fa-angle-up").addClass("fa-angle-down");
        if ($(this).parent().find(".accordian-slide").css("display") == "none") {
            $(this).parent().find(".accordian-slide").slideDown();
            $(this).find(".fa-angle-down").removeClass("fa-angle-down").addClass("fa-angle-up");
        } else {
            $(this).parent().find(".accordian-slide").slideUp();
            $(this).find(".fa-angle-up").removeClass("fa-angle-up").addClass("fa-angle-down");
        }
    });
});



