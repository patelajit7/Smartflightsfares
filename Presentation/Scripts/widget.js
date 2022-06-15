(function ($) {
    "use strict";

    var selectFrom = false;
    var selectTo = false;
    $('#OriginSearch').autocomplete({
        source: function (request, response) {
            var searchText = document.getElementById('OriginSearch').value.replace(/[^\w\s]/gi, '');
            $.ajax({
                url: DOMAIN_URL + "flights/airport-suggestion",
                data: "{id:'" + (searchText) + "'}",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data.ResultList, function (item) {
                        return {
                            label: item.AutoSuggestion,
                            label1: item.Code
                        }
                    }));
                }
            })
        },
        open: function (event, ui) {
            selectFrom = true;
            if (navigator.userAgent.match(/(iPod|iPhone|iPad)/)) {
                $('.ui-autocomplete').off('menufocus hover mouseover');
            }
        },
        select: function (event, ui) {
            searchengine.valiDateDestination();
            $('.origsuggestion').removeClass('d-none');
            $("#Origin").val(ui.item.label1);
            $(this).val(ui.item.label);
            selectFrom = false;
            return false;
        },
        close: function (event, ui) {
            selectFrom = false;
        },
        minLength: 1,
        autoFocus: true
    }).blur(function () {
        if (selectFrom) {
            $("#OriginSearch").val($('ul.ui-autocomplete li:first a').text());
        }
    });


    $('#DestinationSearch').autocomplete({
        minLength: 1,
        source: function (request, response) {
            var searchText = document.getElementById('DestinationSearch').value.replace(/[^\w\s]/gi, '');
            $.ajax({
                url: DOMAIN_URL + "flights/airport-suggestion",
                data: "{id:'" + (searchText) + "'}",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data.ResultList, function (item) {
                        return {
                            label: item.AutoSuggestion,
                            label1: item.Code
                        }
                    }));
                }
            })
        },
        open: function (event, ui) {
            selectFrom = true;
            if (navigator.userAgent.match(/(iPod|iPhone|iPad)/)) {
                $('.ui-autocomplete').off('menufocus hover mouseover');
            }
        },
        select: function (event, ui) {
            $('.destinationggestion').removeClass('d-none');
            $("#Destination").val(ui.item.label1);
            $(this).val(ui.item.label);
            selectFrom = false;
            return false;
        },
        close: function (event, ui) {
            selectFrom = false;
        },
        autoFocus: true
    }).blur(function () {
        if (selectFrom) {
            $("#DestinationSearch").val($('ul.ui-autocomplete li:first a').text());
        }
    });


    /***********************validation Start***********************************/

    $('#flights').submit(function () {

        $(".error-div").hide();
        settimediv('.error-div');
        settimediv('.error-div1');
        // Get the Login Name value and trim it
        var Origin = $.trim($('#OriginSearch').val());
        var Destination = $.trim($('#DestinationSearch').val());
        var trip_type = $('input[name=TripType]:checked').val();
        var DepartDate = $.trim($('#Departure').val());
        var ReturnDate = $.trim($('#Return').val());
        //Show popup for invalid date search
        if (!ShowValidSearchPopup(DepartDate)) {
            return false;
        }
        var OriginSearch = $.trim($('#Origin').val());
        var DestinationSearch = $.trim($('#Destination').val());
        var Adult = $.trim($('#Adult').val());
        var Child = $.trim($('#Child').val());
        var InfantOnLap = $.trim($('#InfantOnLap').val());
        var InfantOnSeat = $.trim($('#InfantOnSeat').val());
        var DepartDatedata = "";
        var utmSource = $.trim($('#utm_source').val());
        var utmMedium = $.trim($('#utm_medium').val());
        // Check if empty of not
        if (Origin === '' || (Origin.length) < 3) {
            $('#OriginSearch').focus();
            $(".inputOrigin").addClass('error');
            return false;
        }
        else if (Destination === '' || (Destination.length) < 3) {
            $('#DestinationSearch').focus();
            $(".inputDestination").addClass('error');
            $("#Return_error").html('Please select an destination');
            return false;
        }
        else if (Destination === Origin) {
            $('#DestinationSearch').focus();
            $(".inputDestination").addClass('error');
            $("#Return_error").html('Please enter a different Origin and Destination/City airport!');
            return false;
        }

        else if (enums.PaxValue.Adult < (enums.PaxValue.InfantOnLap + enums.PaxValue.InfantOnSeat)) {
            $("#TotalPax_error").css('display', 'flex');
            return false;
        }

        else if (trip_type != "") {

            if (trip_type === "1") {

                if (DepartDate === '') {
                    $('#Departure').focus();
                    $(".inputDepart").addClass('error');
                    return false;
                }
            }
            else {
                var trip_typeData = "return";
                if (DepartDate === '') {
                    $('#Departure').focus();
                    $(".inputDepart").addClass('error');
                    return false;
                } else {
                    DepartDatedata = searchengine.gerDateFormat(DepartDate);
                }
                if (ReturnDate === '') {
                    $('#Return').focus();
                    $(".inputReturn").addClass('error');
                    return false;
                } else {
                    ReturnDate = searchengine.gerDateFormat(ReturnDate);
                }
            }
        }
        window.open(DOMAIN_URL + "flights/find-result/?origin=" + OriginSearch + "&destination=" + DestinationSearch + "&adults=" + Adult + "&children=" + Child + "&infants=" + InfantOnSeat + "&type=" + trip_typeData + "&departure=" + DepartDatedata + "&return=" + ReturnDate + "&cabin=economy&airline=&utm_source=" + utmSource + "&utm_medium" + utmMedium);
        
        return false;

    });
    /***********************validation End***********************************/


    /***********************popup open Start***********************************/
    $("#Departure").click(function (event) {
        event.stopPropagation();
        var t = $(this);
        $("#js-filterOptins").show(), showFilterContainer(".dateFilter"), filterOptionsPositionTop(t), filterOptionsPositionLeft(t)
    });
    $("#Return").click(function (event) {
        event.stopPropagation();
        var t = $(this);
        $("#js-filterOptins").show(), showFilterContainer(".dateFilterReturn"), filterOptionsPositionTop(t), filterOptionsPositionLeft(t)
    });
    $("#paxCounter").click(function (event) {
        event.stopPropagation();
        var t = $(this);
        $("#js-filterOptins").show(), showFilterContainer(".paxFilter"), filterOptionsPositionTop(t), filterOptionsPositionLeft(t)
    });
    $("#cabinDiv").click(function (event) {
        event.stopPropagation();
        var t = $(this);
        $("#js-filterOptins").show(), showFilterContainer(".cabinDiv"), filterOptionsPositionTop(t), filterOptionsPositionLeft(t)
    });

    $('#js-filterOptins').click(function (e) {
        e.stopPropagation();
    });

    $(document).click(function () {
        $('#js-filterOptins').hide();
    });
    $('.paxFilterDone').click(function (e) {
        $('#js-filterOptins').hide();
    });
    var weekday = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    $('#Departure').change(function () {
        $('#Departure').val($(this).val());
        $('#Return').datepicker("option", "minDate", $(this).val());
        searchengine.valiDateReturnDate();

    });
    $('#Return').change(function () {
        $('#Return').val($(this).val());
        searchengine.openReturnBox();
    });

    var noMonth = parseInt($(window).width(), 10) > 990 ? 1 : 1;
    $(document).ready(function () {
        $('#Departure').datepicker({
            dateFormat: 'M dd yy',
            numberOfMonths: numberOfMonthValue,
            showButtonPanel: true,
            minDate: 0,
            maxDate: "+11M",
            autoclose: true,
            defaultDate: searchengine.checkDptMinDate(),
        });
        $('#Return').datepicker({
            dateFormat: 'M dd yy',
            numberOfMonths: numberOfMonthValue,
            autoclose: true,
            maxDate: "+11M",
            minDate: $('#Departure').val(),
            defaultDate: searchengine.checkRtnMinDate(),
        });

    });

    $('.cabin li').click(function (e) {
        $('#cabinDivVal').html(searchengine.getcabin($('.cabin :checked').val()));
    });
    $('.origsuggestion ').click(function (e) {
        $('#OriginSearch').val('').focus();
        $(this).addClass('d-none');
    });
    $('.destinationggestion').click(function (e) {
        $('#DestinationSearch').val('').focus();
        $(this).addClass('d-none');
    });
    $('.srch_eng_bxes_btn,.modi-serach-btn').click(function (e) {
        $("#flights").submit()
    });
    $(".more-option").click(function () {
        if ($(".more-option a").text() == 'Advanced Options') {
            $(".more-option a").text('Hide Options');
        }
        else {
            $(".more-option a").text('Advanced Options');
        }
        $(".modi-mor-opt").slideToggle("slow");
    });
    $('.actv_clor_input').click(function (e) {
        $('.srch_eng_bxes, .subs_input_bx, .srch_airLineBx ').removeClass('active');
        $(this).parent().addClass('active');
    });
    $('.actv_clor_input').blur(function (e) {
        $('.srch_eng_bxes, .subs_input_bx, .srch_airLineBx ').removeClass('active');
    });
})(jQuery);

function filterOptionsPositionTop(t) {
    var e = t.offset().top + 50;
    $(".filterOptins").css("top", e).slideDown()
}
function filterOptionsPositionLeft(t) {
    $(".filterOptins").css({
        left: t.offset().left + t.width() / 2 - $("#js-filterOptins").width() / 2
    })
}
function showFilterContainer(t) {
    $(".filterOptins > div > div").hide(), $(t).show(), $(".filterOptions").show()
}

function closeFilterOptions() {
    $(".filterOptions").hide(), $(".reasonFilter").hide()
}

/***********************search box***********************************/
function callme(param) {

    var one = "OneWay";
    var round = "RoundTrip";
    var multi = "MultiCity";
    if (param == one) {
        searchengine.closeReturnBox();
    }
    else {
        searchengine.openReturnBox();
    }
}

function settimediv(data) {
    setTimeout(function () {
        $(data).fadeOut(1500);
        $('.label').removeClass('error');
        $('.label1').removeClass('error');
    }, 3000);

}
enums = {
    cabinType: {
        Economy: 2, //
        Business: 4,
        First: 6
    },
    PaxValue: {
        Adult: parseInt($('#Adult').val()), //
        Child: parseInt($('#Child').val()),
        InfantOnLap: parseInt($('#InfantOnLap').val()),
        InfantOnSeat: parseInt($('#InfantOnSeat').val())
    },
};
searchengine = {
    totalpax: enums.PaxValue.Adult + enums.PaxValue.Child + enums.PaxValue.InfantOnLap + enums.PaxValue.InfantOnSeat,
    validateFlight: function (id) {
        this.flight.Adult
    },
    settimediv: function (data) {
        setTimeout(function () {
            $(data).fadeOut(1500);
        }, 3000);
    },
    getcabin: function (id) {
        var _cabin = "";
        switch (parseInt(id, 10)) {
            case enums.cabinType.Economy:
                _cabin = "Economy";
                break;
            case enums.cabinType.Business:
                _cabin = "Business";
                break;
            case enums.cabinType.First:
                _cabin = "First";
                break;
            default:
                _cabin = "Economy";
                break;
        }
        return _cabin;
    },
    getcabinName: function (id) {
        var _cabin = "";
        switch (id) {
            case "Economy":
                _cabin = "Economy";
                break;
            case "PremiumEconomy":
                _cabin = "Premium Economy";
                break;
            case "Business":
                _cabin = "Business";
                break;
            case "First":
                _cabin = "First";
                break;
            default:
                _cabin = "Economy";
                break;
        }
        return _cabin;
    },
    getPax: function (id) {
    },
    closeReturnBox: function () {
        //$(".arrowimg,.rtndispl,.calen-sec-return, .two").css("display", "none");
        //$(".one").css("width", "100%");
        $('#Return').val("");
        $("#Return").prop("disabled", true);
        //$('.RoundTrip').removeClass('active');
        //$('.OneWay').addClass('active');

        $('.RoundTrip .form-check-input').prop("checked", false);
        $('.OneWay .form-check-input').prop("checked", true);
        //$('.bkkRtrnDte').css({ "display": "block" });
    },
    openReturnBox: function () {

        //$('.rtndispl,.calen-sec-return, .two').css({ "display": "block" });
        //$(".one").css("width", "50%");
        //$('.OneWay').removeClass('active');
        //$('.RoundTrip').addClass('active');
        $('.OneWay .form-check-input').prop("checked", false);
        $('.RoundTrip .form-check-input').prop("checked", true);
        $('.bkkRtrnDte').css({ "display": "none" })
        $("#Return").prop("disabled", false);
    },
    increment: function (data) {
        if (data == "Adult") {

            if (this.totalpax == 9) {
                settimediv('.error-div');
                $("#Class_error").show();
            }
            else {
                enums.PaxValue.Adult++;
                this.totalpax++;
                $('#' + data).val(enums.PaxValue.Adult);
                $('.' + data).html(enums.PaxValue.Adult);
            }
        }
        else if (data == "Child") {
            if (this.totalpax == 9) {
                settimediv('.error-div');
                $("#Class_error").show();
            }
            else {
                enums.PaxValue.Child++;
                this.totalpax++;
                $('#' + data).val(enums.PaxValue.Child);
                $('.' + data).html(enums.PaxValue.Child);
            }
        }
        else if (data == "InfantOnSeat") {
            if (this.totalpax == 9) {
                settimediv('.error-div');
                $("#Class_error").show();
            }
            else if (enums.PaxValue.InfantOnSeat + enums.PaxValue.InfantOnLap < enums.PaxValue.Adult) {

                enums.PaxValue.InfantOnSeat++;
                this.totalpax++;
                $('#' + data).val(enums.PaxValue.InfantOnSeat);
                $('.' + data).html(enums.PaxValue.InfantOnSeat);
            }
            else {
                settimediv('.error-div');
                $("#Infant_error").show();
            }
        }
        else if (data == "InfantOnLap") {
            if (this.totalpax == 9) {
                settimediv('.error-div');
                $("#Class_error").show();
            }
            else if (enums.PaxValue.InfantOnLap + enums.PaxValue.InfantOnSeat < enums.PaxValue.Adult) {

                enums.PaxValue.InfantOnLap++;
                this.totalpax++;
                $('#' + data).val(enums.PaxValue.InfantOnLap);
                $('.' + data).html(enums.PaxValue.InfantOnLap);
            }
            else {
                settimediv('.error-div');
                $("#Infant_error").show();
            }
        }

        $('#paxCounterVal').val(this.totalpax + ' Traveler(s), ' + this.getcabinName($('input[name=Cabin]:checked').val()));
        $('#tpax').html(this.totalpax);

    },
    decrement: function (data) {
        if (data == "Adult") {
            if (enums.PaxValue.Adult != 1) {
                enums.PaxValue.Adult--;
                this.totalpax--;
                $('#' + data).val(enums.PaxValue.Adult);
                $('.' + data).html(enums.PaxValue.Adult);
            }
        }
        else if (data == "Child") {
            if (enums.PaxValue.Child != 0) {
                enums.PaxValue.Child--;
                this.totalpax--;
                $('#' + data).val(enums.PaxValue.Child);
                $('.' + data).html(enums.PaxValue.Child);
            }
        }
        else if (data == "InfantOnSeat") {
            if (enums.PaxValue.InfantOnSeat != 0) {
                enums.PaxValue.InfantOnSeat--;
                this.totalpax--;
                $('#' + data).val(enums.PaxValue.InfantOnSeat);
                $('.' + data).html(enums.PaxValue.InfantOnSeat);
            }
        }
        else if (data == "InfantOnLap") {
            if (enums.PaxValue.InfantOnLap != 0) {
                enums.PaxValue.InfantOnLap--;
                this.totalpax--;
                $('#' + data).val(enums.PaxValue.InfantOnLap);
                $('.' + data).html(enums.PaxValue.InfantOnLap);
            }
        }
        $('#paxCounterVal').val(this.totalpax + ' Traveler(s), ' + this.getcabinName($('input[name=Cabin]:checked').val()));
        $('#tpax').html(this.totalpax);
    },
    valiDateFutureDate: function (futureDate, TodayDate) {
        if (Date.parse(futureDate) > Date.parse(TodayDate)) {
            return true;
        }
        return false;
    },
    cookieCheck: function () {
        var weekday = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
        try {

            if (($('#OriginSearch').val() == "" || $('#OriginSearch').val() != "") && $('#DestinationSearch').val() == "" && $('#Departure').val() == "" && $('#Return').val() == "") {
                var cookieVal = jQuery.parseJSON(readCookie('searchPerms'));
                if (cookieVal != null) {

                    if (cookieVal.DestinationSearch != null && cookieVal.OriginSearch != null) {
                        $('.origsuggestion').removeClass('d-none');
                        $('#OriginSearch').val(cookieVal.OriginSearch);
                        $('#Origin').val(cookieVal.Origin);
                        $('.destinationggestion').removeClass('d-none');
                        $('#DestinationSearch').val(cookieVal.DestinationSearch);
                        $('#Destination').val(cookieVal.Destination);
                    }
                    if (cookieVal.TripType == 1) {
                        if (searchengine.valiDateFutureDate(cookieVal.Departure, new Date())) {
                            $('#Departure').val(cookieVal.Departure);
                            $('.Departure').text(cookieVal.Departure);
                            $('#oneway').attr('checked', 'checked');
                            callme('OneWay');
                        }
                    }
                    else {
                        if (searchengine.valiDateFutureDate(cookieVal.Departure, new Date())) {
                            $('#Departure').val(cookieVal.Departure);
                            $('.Departure').text(cookieVal.Departure);
                        }
                        if (searchengine.valiDateFutureDate(cookieVal.Return, new Date())) {
                            $('#Return').val(cookieVal.Return);
                            $('.Return').text(cookieVal.Return);
                        }
                        $('#TripType').attr('checked', 'checked');
                    }
                }
            }
        } catch (e) {
            console.log("Search cookies");
        }

    },
    valiDateReturnDate: function () {
        if ($('input[name=TripType]:checked').val() == 2) {
            if ($('#Return').val() != "") {
                if (searchengine.valiDateFutureDate($('#Departure').val(), $('#Return').val())) {
                    $('#Return').val('');
                    $('#Return').trigger('click');
                }
            }
            else {
                $('#Return').trigger('click')
            }

        }
    },
    valiDateDestination: function () {
        if ($('#DestinationSearch').val() == "") {
            $('#DestinationSearch').focus();
        }
    },
    checkDptMinDate: function () {
        var cookieVal = jQuery.parseJSON(readCookie('searchPerms'));
        if (cookieVal != null) {
            if (searchengine.valiDateFutureDate(cookieVal.Departure, new Date())) {
                return new Date(cookieVal.Departure);
            }
        }
        return new Date();
    },
    checkRtnMinDate: function () {
        var cookieVal = jQuery.parseJSON(readCookie('searchPerms'));
        if (cookieVal != null) {
            if (searchengine.valiDateFutureDate(cookieVal.Return, new Date())) {
                return new Date(cookieVal.Return);
            }
        } else if ($('#Departure').val() == "") {
            return new Date($('#Departure').val());
        }
        return new Date();
    },
    gerDateFormat: function (getDate) {
        var today = new Date(getDate);
        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        today = mm + '-' + dd + '-' + yyyy;
        return today;
    },
};
$('#OriginSearch').focus(function () { if ($("#OriginSearch").attr("placeholder") == "Origin city/airport") { $("#OriginSearch").attr("placeholder", ""); } });
$('#OriginSearch').blur(function () { if ($("#OriginSearch").attr("placeholder") == "") { $("#OriginSearch").attr("placeholder", "Origin city/airport"); } });
$('#DestinationSearch').focus(function () { if ($('#DestinationSearch').attr("placeholder") == "Destination city/airport") { $('#DestinationSearch').attr("placeholder", ""); } });
$('#DestinationSearch').blur(function () { if ($('#DestinationSearch').attr("placeholder") == "") { $('#DestinationSearch').attr("placeholder", "Destination city/airport"); } });
$('.totalCount').html(searchengine.totalpax);
//searchengine.cookieCheck();
$('#paxCounterVal').html(searchengine.totalpax + ' Traveler(s), ' + searchengine.getcabinName($('#Cabin').val()));
function addOfferContactData(guid, phone, email, countryCode, areaCode) {

    phone = $("#mobile-number").val();
    countryCode = $("#countryCode").val();
    if (phone == "" || phone.length < 10) {
        $(".phonError").css("border", "solid 1px red").css("border-bottom", "2px solid red");
        $("#mobile-number").focus();
    }
    else {
        $("#offcont").hide();
        var url = DOMAIN_URL + "flights/add-offercontactdata";
        $.ajax({
            type: "POST",
            data: { Guid: guid, Phone: phone, Email: email, CountryCode: countryCode, AreaCode: areaCode },
            url: url,
            success: function (response) {
                $("#offerSuccess").show();
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}

function ShowValidSearchPopup(DepartDate) {
    var hour = C_D.getHours();
    var minute = C_D.getMinutes();
    var seconds = C_D.getSeconds();
    DepartDate = DepartDate + ' ' + hour + ':' + minute + ':' + seconds;
    var depDate = new Date(DepartDate);
    //if (depDate <= C_D || 1!=1) {
    //    $('#Departure-popup').modal('show');
    //    return false;
    //} else {
    //    $('#Departure-popup').modal('hide');
    //    return true;
    //}
    return true;
}
function selectChanged(val) {

    $('#paxCounterVal').html(searchengine.totalpax + ' Traveler(s), ' + searchengine.getcabinName($('#Cabin').val()));
}

$(function () {
    $('input[type=radio][name=Cabin]').change(function () {
        $('#paxCounterVal').val(searchengine.totalpax + ' Traveler(s), ' + searchengine.getcabinName($('input[name=Cabin]:checked').val()));
    });
})

// Cookies
function createCookie(name, value, days) {

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";

    document.cookie = name + "=" + value + expires + "; path=/";
}
function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

$('.flightsButton').click(function (e) {
    $("#flights").submit()
});