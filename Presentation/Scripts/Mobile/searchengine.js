(function ($) {

    "use strict";

    // Single Select
    var selectFrom = false;
    var selectTo = false;
    $('#OriginSearchpopup').autocomplete({
        source: function (request, response) {
            var searchText = document.getElementById('OriginSearchpopup').value.replace(/[^\w\s]/gi, '');
            $.ajax({
                url: DOMAIN_URL + "flights/airport-suggestion",
                data: "{id:'" + (searchText) + "'}",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                success: function (data) {                    
                    response($.map(data.ResultList, function (item) {
                        return {
                            label: item.AutoSuggestion,
                            label1: item.Code,
                            label2: item.Name
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
            $('#OriginSearchpopup').val("");
            $('.OriginSearchName').val(ui.item.label);            
            $('#OriginSearch').val(ui.item.label);
            $("#Origin").val(ui.item.label1);            
            $('#inputOrigin').addClass('valid');
            searchengine.valiDateDestination();
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
            $("#OriginSearchpopup").val($('ul.ui-autocomplete li:first a').text());
        }
    });

    $('#DestinationSearchPopup').autocomplete({
        minLength: 1,
        source: function (request, response) {
            var searchText = document.getElementById('DestinationSearchPopup').value.replace(/[^\w\s]/gi, '');
            $.ajax({
                url: DOMAIN_URL + "flights/airport-suggestion",
                data: "{id:'" + (searchText) + "'}",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data.ResultList, function (item) {
                        return {
                            label: item.AutoSuggestion,
                            label1: item.Code,
                            label2: item.Name
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
            $('#DestinationSearchPopup').val("");
            $('.DestinationSearchName').val(ui.item.label);            
            $('#DestinationSearch').val(ui.item.label);
            $("#Destination").val(ui.item.label1);            
            $("#orginPopup").removeClass("active");
            $('#inputDestination').addClass('valid');
            searchengine.checkDate('orginPopup');

            selectFrom = false;
            return false;
        },
        close: function (event, ui) {
            selectFrom = false;
        },
        autoFocus: true
    }).blur(function () {
        if (selectFrom) {
            $("#DestinationSearchPopup").val($('ul.ui-autocomplete li:first a').text());
        }
    });

    $('#PreferredCarrier').autocomplete({
        source: function (request, response) {
            var searchText = document.getElementById('PreferredCarrier').value.replace(/[^\w\s]/gi, '');
            $.ajax({
                url: DOMAIN_URL + "flights/airline-suggestion/" + searchText,
                //data: "{id:'" + (searchText) + "'}",
                type: 'GET',
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    console.log(data.ResultList);
                    response($.map(data.ResultList, function (item) {
                        return {
                            label: item.Name,
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
            $(this).val(ui.item.label);
            selectFrom = false;
            return false;
        },
        close: function (event, ui) {
            selectFrom = false;
            $(this).val(ui.item.value)
        },
        minLength: 1,
        autoFocus: true
    }).blur(function () {
        if (selectFrom) {
            $("#Airline").val($('ul.ui-autocomplete li:first a').text());
        }
    });

/***********************validation Start***********************************/

    $('#flights').submit(function () {$('#tags').trigger("focus"); 
        $(".error-div").hide();
        // Get the Login Name value and trim it
        var Origin = $.trim($('#OriginSearch').val());
        var Destination = $.trim($('#DestinationSearch').val());
        var trip_type = $('input[name=TripType]:checked').val();
        var DepartDate = $.trim($('#Departure').val());
        var ReturnDate = $.trim($('#Return').val());
      //Show popup for invalid date search
        //if (!ShowValidSearchPopup(DepartDate)) {
        //    return false;
        //}
        // Check if empty of not
        if (Origin === '' || (Origin.length) < 3) {
            $(".error-div").show().html("Enter a valid <b>'Origin City'</b> or select from the list!");
            $("#flgihtValidatonPopup").show();
            return false;
        }
        else if (Destination === '' || (Destination.length) < 3) {
            $(".error-div").show().html("Enter a valid <b>'Destination City'</b> or select from the list!");
            $("#flgihtValidatonPopup").show();
            return false;
        }
        else if (Destination === Origin) {
            $(".error-div").show().html("Please ensure <b>'Origin city'</b> and <b>Destination city</b> are not the same!");
            $("#flgihtValidatonPopup").show();
            return false;
        }
        else if (enums.PaxValue.Adult < (enums.PaxValue.InfantOnLap + enums.PaxValue.InfantOnSeat)) {
            $(".error-div").show().html("Total <b>'Infant'</b> not more then <b>'Adult'</b>");
            $("#flgihtValidatonPopup").show();
            return false;
        }
        else if (trip_type != "") 
        {
                if (trip_type === "1") {

                    if (DepartDate === '') {
                        $(".error-div").show().html("Select a <b>'Depart Date'</b>");
                        $("#flgihtValidatonPopup").show();
                        return false;
                    }
                }
                else {
                    if (DepartDate === '') {
                        $(".error-div").show().html("Select a <b>'Depart Date'</b>");
                        $("#flgihtValidatonPopup").show();
                        return false;
                    }
                    if (ReturnDate === '') {
                        $(".error-div").show().html("Select a <b>'Return Date'</b> or select ONE WAY journey type.");
                        $("#flgihtValidatonPopup").show();
                        return false;
                    }
                }


        }
        var data = $(this).serializeFormJSON();
        createCookie('searchPerms', JSON.stringify(data), 7);       
    });
/***********************validation End***********************************/
/***********************datapicker start***********************************/

    $('#inline1').change(function () {
        $('#Departure').val($(this).val());
        $('.Departure').val($(this).val());
        $('#inline2').datepicker("option", "minDate", $(this).val());
        //$('.popupSearch').hide();
        $('#dateDeparture').addClass('valid');
        searchengine.valiDateReturnDate();
    });
    $('#inline2').change(function () {
        $('.Return').val($(this).val());
        $('#Return').val($(this).val());
        $('#dateReturn').addClass('valid');
        searchengine.openReturnBox();
    });

    $(document).ready(function () {
        $('#inline1').datepicker({
            dateFormat: 'M dd yy',
            numberOfMonths: 11,
            showCurrentAtPos: 0,
            showButtonPanel: true,
            minDate: 0,
            maxDate: "+11M",
            defaultDate: new Date(),
            autoclose: true,

        });
        $('#inline2').datepicker({
            dateFormat: 'M dd yy',
            numberOfMonths: 11,
            showCurrentAtPos: 0,
            autoclose: true,
            maxDate: "+11M",
            minDate: $('#Departure').val(),
        });
    });
    /***********************datapicker End***********************************/
    $('.gotohome').click(function () {
        searchengine.goToHome();
    });
    $('.searchTabList .from').click(function () {
        searchengine.clickFrom();
    });
    $('.searchTabList .to').click(function () {
        searchengine.clickTo();
    });
 
})(jQuery);


/***********************TRAVELERS***********************************/

function settimediv(data) {
    setTimeout(function () {
        $(data).fadeOut(1500);
    }, 3000);

}

/***********************Custome***********************************/
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
    validateFlight: function (id) {
        this.flight.Adult
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
    getPax: function (id) {
    },
    clickFrom: function () {
        $('#from').show();
        $('#to').hide();
        $('.ToFromMsg').html('Where are you leaving from?');
        //$('.fromValue').html($('#OriginSearch').val());
        //$('.toValue').html($('#DestinationSearch').val());
        $('#to').addClass('hidden');
        $('#from').removeClass('hidden');
        $("#orginPopup").addClass("active"); $('#OriginSearchpopup').focus(); $('#OriginSearchpopup').trigger('focus');
    },
    clickTo: function () {
        $('#from').hide();
        $('#to').show();
        $('.ToFromMsg').html('Where are you going?');
        $('.fromValue').html($('#OriginSearch').val());
        $('.toValue').html($('#DestinationSearch').val());
        $('#from').addClass('hidden');
        $('#to').removeClass('hidden');
        $("#orginPopup").addClass("active"); $('#DestinationSearchPopup').focus();
    },
    callme: function (param) {
        var one = "OneWay";
        var round = "RoundTrip";
        var multi = "MultiCity";
        if (param === one) {
            $('#TripType').val('ONEWAY');
            $('#DepartDate12').hide();
            $('.oneway').addClass('active');
            $('.rondtrip').removeClass('active');
            $('.triptypeval').html('Oneway');
            $('.Mobiletrip_type').hide();
            
        }
        else {
            $('#TripType').val('ROUNDTRIP');
            $('#DepartDate12').show();
            $('.rondtrip').addClass('active');
            $('.oneway').removeClass('active');
            $('.triptypeval').html('Round Trip');
            $('.Mobiletrip_type').hide();
            
        }
        
    },
    callClass: function (param) {
        var economy = "Economy";
        var business = "Business";
        var first = "First";
        if (param === economy) {
            $('.economyCoach').addClass('active');
            $('.clsbusiness').removeClass('active');
            $('.clsFirst').removeClass('active');
            $('.classType').html('Economy');
            $('.class-type').hide();
            $('#Cabin').val(1);
        }
        else if (param === business) {
            $('.economyCoach').removeClass('active');
            $('.clsbusiness').addClass('active');
            $('.clsFirst').removeClass('active');
            $('.classType').html('Business');
            $('.class-type').hide();
            $('#Cabin').val(4);
        }
        else if (param === first) {
            $('.economyCoach').removeClass('active');
            $('.clsbusiness').removeClass('active');
            $('.clsFirst').addClass('active');
            $('.classType').html('First');
            $('.class-type').hide();
            $('#Cabin').val(6);
        } else {
            $('.economyCoach').addClass('active');
            $('.clsbusiness').removeClass('active');
            $('.clsFirst').removeClass('active');
            $('.classType').html('Economy');
            $('.class-type').hide();
            $('#Cabin').val(1);
        }

    },
    checkDate: function (id) {
        if ($('#Departure').val() == "")
        {            
            $('#departPopup').addClass('active');
        }
    },
    checkDestination: function (id) {
        if ($('#Departure').val() == "") {
            $('#departPopup').addClass('active');
        }
    },
    showPopup: function (id) {
        $("#" + id).show();
    },
    hidePopup: function (id) {
        $('html,body').scrollTop(0);
        $("#" + id).hide();
    },
    closeReturnBox: function () {
        var trip_type = $('input[name=TripType]:checked').val();
        if (trip_type==2)
        {            
            $('#returnPopup').addClass('active');
        }
        //$('#TripType').val('1');
        //$('.Return ,.arrowimg').hide();
        //$('.returnBook').show();;
        $('.Return').val('Date');
        event.stopPropagation();
    },
    openReturnBox: function () {
        $('.open-calendarpopup').removeClass('active')
    },
    getCabinType: function (cabin) {
        var _cabin = "";
        switch (parseInt(cabin, 10)) {
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
    setCabin: function () {
        $('.cabin').html(searchengine.getCabinType($("input[name='Cabin']:checked").val()));
    },
    totalpax: enums.PaxValue.Adult + enums.PaxValue.Child + enums.PaxValue.InfantOnLap + enums.PaxValue.InfantOnSeat,
    increment: function (data) {
       
        if (data == "Adult") {

            if (this.totalpax == 9) {
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
            }
            else if (enums.PaxValue.InfantOnSeat + enums.PaxValue.InfantOnLap < enums.PaxValue.Adult) {
                enums.PaxValue.InfantOnSeat++;
                this.totalpax++;
                $('#' + data).val(enums.PaxValue.InfantOnSeat);
                $('.' + data).html(enums.PaxValue.InfantOnSeat);
            }
            else {
            }
        }
        else if (data == "InfantOnLap") {
            
            if (this.totalpax == 9) {
            }
            else if (enums.PaxValue.InfantOnLap + enums.PaxValue.InfantOnSeat < enums.PaxValue.Adult) {

                enums.PaxValue.InfantOnLap++;
                this.totalpax++;
                $('#' + data).val(enums.PaxValue.InfantOnLap);
                $('.' + data).html(enums.PaxValue.InfantOnLap);
            }
            else {
            }
        }

        $('#paxCounterVal').html(this.totalpax);
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
        $('#paxCounterVal').html(this.totalpax);
    },
    settimediv: function (data) {
        setTimeout(function () {
            $(data).fadeOut(1500);
        }, 3000);
    },
    swap: function () {
        var OriginSearch = $('#OriginSearch').val();
        var OriginSearchName = $('.OriginSearchName').html();
        var OriginSearchCountry = $('.OriginSearchCountry').html();
        if (OriginSearch != "" && $('#DestinationSearch').val()!="") {
            $('.OriginSearchName').html($('.DestinationSearchName').html());
            $('.OriginSearchCountry').html($('.DestinationSearchCountry').html());
            $('#OriginSearch').val($('#DestinationSearch').val());
            $('.DestinationSearchName').html(OriginSearchName);
            $('.DestinationSearchCountry').html(OriginSearchCountry);
            $('#DestinationSearch').val(OriginSearch);
        }
      
    },
    goToHome: function (id) {
        window.location.href = "/";
    },
    valiDateFutureDate: function (futureDate, TodayDate) {
        if (Date.parse(futureDate) > Date.parse(TodayDate)) {
            return true;
        }
        return false;
    },
    cookieCheck: function () {
        try {
            if (($('#OriginSearch').val() == "" || $('#OriginSearch').val() != "") && $('#DestinationSearch').val() == "" && $('#Departure').val() == "" && $('#Return').val() == "") {
            var cookieVal = jQuery.parseJSON(readCookie('searchPerms'));
            if (cookieVal != null) {
                if (cookieVal.DestinationSearch != null && cookieVal.OriginSearch != null) {                    
                    $('#OriginSearch').val(cookieVal.OriginSearch);
                    $('#Origin').val(cookieVal.Origin);
                    $('#inputOrigin').addClass('valid');
                    $('#inputOrigin').val(cookieVal.OriginSearch);

                    $('#DestinationSearch').val(cookieVal.DestinationSearch);
                    $('#Destination').val(cookieVal.Destination);
                    $('#inputDestination').addClass('valid');
                    $('#inputDestination').val(cookieVal.DestinationSearch);
                }
                if (cookieVal.TripType == "ONEWAY") {

                    if (searchengine.valiDateFutureDate(cookieVal.Departure, new Date())) {
                        $('#Departure').val(cookieVal.Departure);    
                        $('.Departure').val(cookieVal.Departure); 
                        $('#dateDeparture').addClass('valid');
                        $('#dateDeparture').val(cookieVal.Departure); 
                        searchengine.callme('OneWay');
                    }
                }
                else {
                    if (searchengine.valiDateFutureDate(cookieVal.Departure, new Date())) {
                        $('#Departure').val(cookieVal.Departure);
                        $('.Departure').text(cookieVal.Departure);
                        $('#dateDeparture').addClass('valid');
                        $('#dateDeparture').val(cookieVal.Departure); 
                        searchengine.callme('OneWay');
                    }
                    if (searchengine.valiDateFutureDate(cookieVal.Return, new Date())) {
                        $('#Return').val(cookieVal.Return);
                        $('.Return').text(cookieVal.Return);
                        $('#dateReturn').addClass('valid');
                        $('#dateReturn').val(cookieVal.Return); 
                        searchengine.callme('RoundTrip');
                    }
                }
            }
            }
        } catch (e) {
            console.log("Search cookies");
        }
    },
    valiDateReturnDate: function () {        
        if ($('#TripType').val() == 'ROUNDTRIP') {
            if ($('#Return').val() != "") {
                if (searchengine.valiDateFutureDate($('#Departure').val(), $('#Return').val())) {
                    $('#Return').val('');
                    $('.Return').html('Date');
                    $('#returnPopup').addClass('active');
                }
            }
            else {
                $('#returnPopup').addClass('active');
            }

        }
        else {
            $('.open-calendarpopup').removeClass('active');
        }
        event.stopPropagation();
    },
    valiDateDestination: function () {
        if ($('#DestinationSearch').val() == "") {
            searchengine.clickTo();
        } else {
            $("#orginPopup").removeClass("active");;
        }
    },
    hideAllPopup: function () {
        $(".Mobiletrip_type").hide();
        $("#travelersPopup").hide();
    }
};
searchengine.cookieCheck();
function ShowValidSearchPopup(DepartDate) {
    var hour = C_D.getHours();
    var minute = C_D.getMinutes();
    var seconds = C_D.getSeconds();
    DepartDate = DepartDate + ' ' + hour + ':' + minute + ':' + seconds;
    var depDate = new Date(DepartDate);
    if (depDate <= C_D) {
        $('#Departure-popup').modal('show');
        return false;
    } else {
        $('#Departure-popup').modal('hide');
        return true;
    }
}