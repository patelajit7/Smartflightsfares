contractFilter = {
    EnumStops: {
        All: -1,
        NonStop: 0,
        OneStop: 1,
        MultiStop: 2
    },
    EnumTabs: {
        AllFlights: 0,
        ShortestDirectFlights: 1,
        NearbyFlights: 2,
        FlexFlights: 3,
        MultiAirines: 4
    },
    EnumTripType: {
        OneWay: 1,
        RoundTrip: 2
    },
    Guid: "",
    Tab: 0,
    TripType: 1,
    PageNumber: 0,
    MinPrice: 0,
    MaxPrice: 0,
    Stops: {
        NonStop: false,
        OneStop: false,
        MultiStop: false
    },
    OutboundDepTime: {
        Min: 0,
        Max: 1440
    },
    OutboundArrTime: {
        Min: 0,
        Max: 0
    },
    OutboundDuration: {
        Min: 0,
        Max: 0
    },
    InboundDepTime: {
        Min: 0,
        Max: 0
    },
    InboundArrTime: {
        Min: 0,
        Max: 0
    },
    InboundDuration: {
        Min: 0,
        Max: 0
    },
    setTab: function (_tab) {
        this.arilineFilterByTab(_tab);
        this.Tab = _tab;
        this.applyFilter(this.Tab, false);
        $(".flightTabs li").removeClass("active");
        switch (this.Tab) {
            case this.EnumTabs.AllFlights:
                $(".flights").addClass("active");
                break;
            case this.EnumTabs.ShortestDirectFlights:
                $(".ShotestFlight").addClass("active");
                break;
            case this.EnumTabs.NearbyFlights:
                $(".NearByAirport").addClass("active");
                break;
            case this.EnumTabs.FlexFlights:
                $(".AlternateDates").addClass("active");
                break;
            default:
        }

    },
    setPrice: function (_min, _max) {
        this.MinPrice = _min;
        this.MaxPrice = _max;
        this.applyFilter(this.Tab, false);
    },
    setOutboundDepTime: function (_min, _max) {
        this.OutboundDepTime.Min = _min;
        this.OutboundDepTime.Max = _max;
        this.applyFilter(this.Tab, false);
    },
    setOutboundArrTime: function (_min, _max) {
        this.OutboundArrTime.Min = _min;
        this.OutboundArrTime.Max = _max;
        this.applyFilter(this.Tab, false);
    },
    setOutboundDuration: function (_min, _max) {
        this.OutboundDuration.Min = _min;
        this.OutboundDuration.Max = _max;
        this.applyFilter(this.Tab, false);
    },
    setInboundDepTime: function (_min, _max) {

        this.InboundDepTime.Min = _min;
        this.InboundDepTime.Max = _max;
        this.applyFilter(this.Tab, false);
    },
    setInboundArrTime: function (_min, _max) {
        this.InboundArrTime.Min = _min;
        this.InboundArrTime.Max = _max;
        this.applyFilter(this.Tab, false);
    },
    setInboundDuration: function (_min, _max) {
        this.InboundDuration.Min = _min;
        this.InboundDuration.Max = _max;
        this.applyFilter(this.Tab, false);
    },
    resetStops: function () {
        $("input:checkbox[name=Stops]:checked").each(function () {
            $(this).prop('checked', false);
        });
        this.applyFilter(this.Tab, false);
    },
    getStops: function () {
        var stopArr = [];
        $("input:checkbox[name=Stops]:checked").each(function () {
            stopArr.push($(this).val());
        });
        if (stopArr.length == 0) {
            stopArr = null;
        }
        return stopArr;
    },
    setStops: function () {
        this.applyFilter(this.Tab, false);
    },
    resetAirlines: function () {
        $("input:checkbox[name=Airlines]:checked").each(function () {
            $(this).prop('checked', false);
        });
        this.applyFilter(this.Tab, false);
    },
    getAirlines: function () {
        var airlinesArr = [];
        $("input:checkbox[name=Airlines]:checked").each(function () {
            airlinesArr.push($(this).val());
        });
        if (airlinesArr.length == 0) {
            airlinesArr = null;
        }
        return airlinesArr;
    },
    setAirlines: function () {
        this.applyFilter(this.Tab, false);
    },
    resetSlider: function () {
        $(".ui-slider").each(function () {
            var options = $(this).slider('option');
            $(this).slider('values', [options.min, options.max]);

        });
    },
    setDefaultFilter: function (_guid, _tab, _page, _tripType, _minPrice, _maxPrice, _outboundDepMinTime, _outboundDepMaxTime, _outboundArrMinTime, _outboundArrMaxTime, _outboundMinDuration,
        _outboundMaxDuration, _inboundDepMinTime, _inboundDepMaxTime, _inboundArrMinTime, _inboundArrMaxTime, _inboundMinDuration, _inboundMaxDuration) {        
        this.Guid = _guid;
        this.Tab = _tab;
        this.PageNumber = _page;
        this.TripType = _tripType;
        this.MinPrice = _minPrice;
        this.MaxPrice = _maxPrice;
        this.OutboundDepTime.Min = _outboundDepMinTime;
        this.OutboundDepTime.Max = _outboundDepMaxTime;
        this.OutboundArrTime.Min = _outboundArrMinTime;
        this.OutboundArrTime.Max = _outboundArrMaxTime;

        this.OutboundDuration.Min = _outboundMinDuration;
        this.OutboundDuration.Max = _outboundMaxDuration;
        if (_tripType == this.EnumTripType.RoundTrip) {
            this.InboundDepTime.Min = _inboundDepMinTime;
            this.InboundDepTime.Max = _inboundDepMaxTime;
            this.InboundArrTime.Min = _inboundArrMinTime;
            this.InboundArrTime.Max = _inboundArrMaxTime;
            this.InboundDuration.Min = _inboundMinDuration;
            this.InboundDuration.Max = _inboundMaxDuration;
        }
    },
    clearAllFilter: function () {
        HitMoreResult = false;
        this.MinPrice = parseFloat($("#priceRange").attr("data-default-min"), 10).toFixed(2);
        this.MaxPrice = parseFloat($("#priceRange").attr("data-default-max"), 10).toFixed(2);
        this.OutboundDepTime.Min = parseInt($("#onBoundTimeRange").attr("data-default-min"), 10);
        this.OutboundDepTime.Max = parseInt($("#onBoundTimeRange").attr("data-default-max"), 10);
        this.OutboundArrTime.Min = parseInt($("#onBoundArrivalTimeRange").attr("data-default-min"), 10);
        this.OutboundArrTime.Max = parseInt($("#onBoundArrivalTimeRange").attr("data-default-max"), 10);

        if (this.TripType == this.EnumTripType.RoundTrip) {
            this.InboundDepTime.Min = parseInt($("#inBoundTimeRange").attr("data-default-min"), 10);
            this.InboundDepTime.Max = parseInt($("#inBoundTimeRange").attr("data-default-max"), 10);
            this.InboundArrTime.Min = parseInt($("#inBoundArrivalTimeRange").attr("data-default-min"), 10);
            this.InboundArrTime.Max = parseInt($("#inBoundArrivalTimeRange").attr("data-default-max"), 10);

        }
        $("input:checkbox[name=Airlines]:checked").each(function () {
            $(this).prop('checked', false);
        });
        $("input:checkbox[name=Stops]:checked").each(function () {
            $(this).prop('checked', false);
        });

        this.resetSlider();
        return true;
    },
    resetFilter: function () {
        this.clearAllFilter();
        this.applyFilter(this.Tab, false);
    },
    applyFilter: function (_tab, isScroll) {
        try {
            if (isScroll == false) {
                this.PageNumber = 1;
            }
            $.ajax({
                url: DOMAIN_URL + "flights/applycontractfilter",
                data: JSON.stringify(this.getFilterSearchRQ()),
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                success: function (response) {
                    if (response.IsSuccess) {
                        if (isScroll != null && isScroll == true) {
                            $("#containerListing").append(response.HtmlResponse);
                        }
                        else {
                            $("#containerListing").html(response.HtmlResponse);
                            $(".cuurentCount").html(response.TotalResult);
                        }

                    }
                    else {
                        if (isScroll != null && isScroll == true) {
                            $("#containerListing").append(response.HtmlResponse);
                        }
                        else {
                            $("#containerListing").html(response.HtmlResponse);
                            $(".cuurentCount").html(response.TotalResult);

                        }
                    }
                    if (response.IsScroll) {
                        HitMoreResult = true;
                    }
                    else {
                        HitMoreResult = false;
                    }
                },
                complete: function (data) {
                    IsScrollEventFire = false;
                    $("#loadingDiv").css('display', "none");
                },
                beforeSend: function () {
                    $("#loadingDiv").css('display', "block");
                }
            })
        } catch (e) {
            console.log("ISSUE:" + e.toString());
        }
    },
    getFilterSearchRQ: function () {
        var request = {
            Guid: this.Guid,
            Tab: this.Tab,
            PageNumber: this.PageNumber,
            Airlines: this.getAirlines(),
            Stops: this.getStops(),
            MinPrice: this.MinPrice,
            MaxPrice: this.MaxPrice,
            OutboundDepTime: contractFilter.OutboundDepTime,
            OutboundArrTime: contractFilter.OutboundArrTime,
            OutboundDuration: contractFilter.OutboundDuration,
            InboundDepTime: null,
            InboundArrTime: null,
            InboundDuration: null
        };
        if (this.TripType == this.EnumTripType.RoundTrip) {
            request.InboundDepTime = contractFilter.InboundDepTime;
            request.InboundArrTime = contractFilter.InboundArrTime;
            request.InboundDuration = contractFilter.InboundDuration
        }

        return request;
    },
    scrollVerticalPosition: function () {
        if (HitMoreResult) {
            this.PageNumber++;
            this.applyFilter(this.Tab, true);
        }
    },
    setVerificationContract: function (gid, id, ctype) {
        $.getJSON(DOMAIN_URL + 'flights/getselectedcontract/' + gid + '?cid=' + parseInt(id, 10), function (response) {
            if (response.IsContractExist == true) {
                window.location.href = response.RedirectUrl;
            }
            else {
                window.location.href = "/";
            }
        });
    },
    arilineFilterByTab: function (_tab) {
        if (_tab == 2) {
            $('.actual,.flexi').addClass('hidden');
            $('.nearby').removeClass('hidden');
        }
        else if (_tab == 3) {
            $('.actual,.nearby').addClass('hidden');
            $('.flexi').removeClass('hidden');
        } else {
            $('.nearby,.flexi').addClass('hidden');
            $('.actual').removeClass('hidden');
        }
    },

};


function applyJquery() {
    //-------------------------------------------------------
    IsScrollEventFire = false;
    $(window).scroll(function () {
        if (!IsScrollEventFire) {
            if (($(window).scrollTop() + 250) >= $(document).height() - $(window).height()) {
                IsScrollEventFire = true;
                contractFilter.scrollVerticalPosition();
            }
        }
    });


    /*************/

    $("#priceSlider").slider({
        range: true,
        min: parseFloat($("#priceRange").attr("data-default-min")),
        max: parseFloat($("#priceRange").attr("data-default-max")),
        values: [parseFloat($("#priceRange").attr("data-default-min")), parseFloat($("#priceRange").attr("data-default-max"))],
        stop: function (e, ui) {
            contractFilter.setPrice(ui.values[0], ui.values[1]);
        },
        slide: function (event, ui) {
            $("#priceLeft").html($("#priceRange").attr("data-default-currencySymbol") + (ui.values[0] * parseFloat($("#priceRange").attr("data-default-currencyPrice"))).toFixed(2));
            $("#priceRight").html($("#priceRange").attr("data-default-currencySymbol") + (ui.values[1] * parseFloat($("#priceRange").attr("data-default-currencyPrice"))).toFixed(2));
        }
    });


    $("#onBoundSlider").slider({
        range: true,
        min: parseInt($("#onBoundTimeRange").attr("data-min"), 10),
        max: parseInt($("#onBoundTimeRange").attr("data-max"), 10),
        step: 15,
        values: [parseInt($("#onBoundTimeRange").attr("data-default-min"), 10), parseInt($("#onBoundTimeRange").attr("data-default-max"), 10)],
        stop: function (e, ui) {
            contractFilter.setOutboundDepTime(ui.values[0], ui.values[1]);
        },
        slide: function (e, ui) {
            var hours1 = Math.floor(ui.values[0] / 60);
            var minutes1 = ui.values[0] - (hours1 * 60);
            if (hours1.length == 1) hours1 = '0' + hours1;
            if (minutes1.length == 1) minutes1 = '0' + minutes1;
            if (minutes1 == 0) minutes1 = '00';
            if (hours1 >= 12) {
                if (hours1 == 12) {
                    hours1 = hours1;
                    minutes1 = minutes1 + " PM";
                } else {
                    hours1 = hours1 - 12;
                    minutes1 = minutes1 + " PM";
                }
            } else {
                hours1 = hours1;
                minutes1 = minutes1 + " AM";
            }
            if (hours1 == 0) {
                hours1 = 12;
                minutes1 = minutes1;
            }

            var hours2 = Math.floor(ui.values[1] / 60);
            var minutes2 = ui.values[1] - (hours2 * 60);
            if (hours2.length == 1) hours2 = '0' + hours2;
            if (minutes2.length == 1) minutes2 = '0' + minutes2;
            if (minutes2 == 0) minutes2 = '00';
            if (hours2 >= 12) {
                if (hours2 == 12) {
                    hours2 = hours2;
                    minutes2 = minutes2 + " PM";
                } else if (hours2 == 24) {
                    hours2 = 11;
                    minutes2 = "59 PM";
                } else {
                    hours2 = hours2 - 12;
                    minutes2 = minutes2 + " PM";
                }
            } else {
                hours2 = hours2;
                minutes2 = minutes2 + " AM";
            }
            //$("#onBoundTimeRange").val('Depart - ' + hours1 + ':' + minutes1 + ' ' + hours2 + ':' + minutes2);
            $("#onBoundLeft").html(hours1 + ':' + minutes1);
            $("#onBoundRight").html(hours2 + ':' + minutes2);

        }
    });


    $("#onBoundArrivalSlider").slider({
        range: true,
        min: parseInt($("#onBoundArrivalTimeRange").attr("data-min"), 10),
        max: parseInt($("#onBoundArrivalTimeRange").attr("data-max"), 10),
        step: 15,
        values: [parseInt($("#onBoundArrivalTimeRange").attr("data-default-min"), 10), parseInt($("#onBoundArrivalTimeRange").attr("data-default-max"), 10)],
        stop: function (e, ui) {
            contractFilter.setOutboundArrTime(ui.values[0], ui.values[1]);
        },
        slide: function (e, ui) {
            var hours1 = Math.floor(ui.values[0] / 60);
            var minutes1 = ui.values[0] - (hours1 * 60);
            if (hours1.length == 1) hours1 = '0' + hours1;
            if (minutes1.length == 1) minutes1 = '0' + minutes1;
            if (minutes1 == 0) minutes1 = '00';
            if (hours1 >= 12) {
                if (hours1 == 12) {
                    hours1 = hours1;
                    minutes1 = minutes1 + " PM";
                } else {
                    hours1 = hours1 - 12;
                    minutes1 = minutes1 + " PM";
                }
            } else {
                hours1 = hours1;
                minutes1 = minutes1 + " AM";
            }
            if (hours1 == 0) {
                hours1 = 12;
                minutes1 = minutes1;
            }

            var hours2 = Math.floor(ui.values[1] / 60);
            var minutes2 = ui.values[1] - (hours2 * 60);
            if (hours2.length == 1) hours2 = '0' + hours2;
            if (minutes2.length == 1) minutes2 = '0' + minutes2;
            if (minutes2 == 0) minutes2 = '00';
            if (hours2 >= 12) {
                if (hours2 == 12) {
                    hours2 = hours2;
                    minutes2 = minutes2 + " PM";
                } else if (hours2 == 24) {
                    hours2 = 11;
                    minutes2 = "59 PM";
                } else {
                    hours2 = hours2 - 12;
                    minutes2 = minutes2 + " PM";
                }
            } else {
                hours2 = hours2;
                minutes2 = minutes2 + " AM";
            }

            $("#onBoundArrivalTimeRange").val('Arrival - ' + hours1 + ':' + minutes1 + ' ' + hours2 + ':' + minutes2);

        }
    });



    $("#inBoundSlider").slider({
        range: true,
        min: parseInt($("#inBoundTimeRange").attr("data-min"), 10),
        max: parseInt($("#inBoundTimeRange").attr("data-max"), 10),
        step: 15,
        values: [parseInt($("#inBoundTimeRange").attr("data-default-min"), 10), parseInt($("#inBoundTimeRange").attr("data-default-max"), 10)],
        stop: function (e, ui) {
            contractFilter.setInboundDepTime(ui.values[0], ui.values[1]);
        },
        slide: function (e, ui) {
            var hours1 = Math.floor(ui.values[0] / 60);
            var minutes1 = ui.values[0] - (hours1 * 60);
            if (hours1.length == 1) hours1 = '0' + hours1;
            if (minutes1.length == 1) minutes1 = '0' + minutes1;
            if (minutes1 == 0) minutes1 = '00';
            if (hours1 >= 12) {
                if (hours1 == 12) {
                    hours1 = hours1;
                    minutes1 = minutes1 + " PM";
                } else {
                    hours1 = hours1 - 12;
                    minutes1 = minutes1 + " PM";
                }
            } else {
                hours1 = hours1;
                minutes1 = minutes1 + " AM";
            }
            if (hours1 == 0) {
                hours1 = 12;
                minutes1 = minutes1;
            }

            var hours2 = Math.floor(ui.values[1] / 60);
            var minutes2 = ui.values[1] - (hours2 * 60);
            if (hours2.length == 1) hours2 = '0' + hours2;
            if (minutes2.length == 1) minutes2 = '0' + minutes2;
            if (minutes2 == 0) minutes2 = '00';
            if (hours2 >= 12) {
                if (hours2 == 12) {
                    hours2 = hours2;
                    minutes2 = minutes2 + " PM";
                } else if (hours2 == 24) {
                    hours2 = 11;
                    minutes2 = "59 PM";
                } else {
                    hours2 = hours2 - 12;
                    minutes2 = minutes2 + " PM";
                }
            } else {
                hours2 = hours2;
                minutes2 = minutes2 + " AM";
            }
            //$("#inBoundTimeRange").val('Depart - ' + hours1 + ':' + minutes1 + ' ' + hours2 + ':' + minutes2);
            $("#inBoundLeft").html(hours1 + ':' + minutes1);
            $("#inBoundRight").html(hours2 + ':' + minutes2);

        }
    });



    $("#inBoundArrivalSlider").slider({
        range: true,
        min: parseInt($("#inBoundArrivalTimeRange").attr("data-min"), 10),
        max: parseInt($("#inBoundArrivalTimeRange").attr("data-max"), 10),
        step: 15,
        values: [parseInt($("#inBoundArrivalTimeRange").attr("data-default-min"), 10), parseInt($("#inBoundArrivalTimeRange").attr("data-default-max"), 10)],
        stop: function (e, ui) {
            contractFilter.setInboundArrTime(ui.values[0], ui.values[1]);
        },
        slide: function (e, ui) {
            var hours1 = Math.floor(ui.values[0] / 60);
            var minutes1 = ui.values[0] - (hours1 * 60);
            if (hours1.length == 1) hours1 = '0' + hours1;
            if (minutes1.length == 1) minutes1 = '0' + minutes1;
            if (minutes1 == 0) minutes1 = '00';
            if (hours1 >= 12) {
                if (hours1 == 12) {
                    hours1 = hours1;
                    minutes1 = minutes1 + " PM";
                } else {
                    hours1 = hours1 - 12;
                    minutes1 = minutes1 + " PM";
                }
            } else {
                hours1 = hours1;
                minutes1 = minutes1 + " AM";
            }
            if (hours1 == 0) {
                hours1 = 12;
                minutes1 = minutes1;
            }

            var hours2 = Math.floor(ui.values[1] / 60);
            var minutes2 = ui.values[1] - (hours2 * 60);
            if (hours2.length == 1) hours2 = '0' + hours2;
            if (minutes2.length == 1) minutes2 = '0' + minutes2;
            if (minutes2 == 0) minutes2 = '00';
            if (hours2 >= 12) {
                if (hours2 == 12) {
                    hours2 = hours2;
                    minutes2 = minutes2 + " PM";
                } else if (hours2 == 24) {
                    hours2 = 11;
                    minutes2 = "59 PM";
                } else {
                    hours2 = hours2 - 12;
                    minutes2 = minutes2 + " PM";
                }
            } else {
                hours2 = hours2;
                minutes2 = minutes2 + " AM";
            }
            $("#inBoundArrivalTimeRange").val('Arrival - ' + hours1 + ':' + minutes1 + ' ' + hours2 + ':' + minutes2);

        }
    });
}

$(document).idle({
    onIdle: function () {
        $('#ListingIdlePopup').show();
    },
    idle: 900000
})
$("#showmoreFilters").click(function () {
    $(this).text(function (i, old) {
        return old == 'Show Arrival Times' ? 'Hide Arrival Times' : 'Show Arrival Times';
    });
});
$("#showmoreAirlinesActual").click(function () {
    $(this).text(function (i, old) {
        return old == 'Show More Flight' ? 'Hide More Flight' : 'Show More Flight';
    });
});
$("#showmoreAirlinesNearby ").click(function () {
    $(this).text(function (i, old) {
        return old == 'Show More Flight' ? 'Hide More Flight' : 'Show More Flight';
    });
});
$("#showmoreAirlinesFlexi").click(function () {
    $(this).text(function (i, old) {
        return old == 'Show More Flight' ? 'Hide More Flight' : 'Show More Flight';
    });
});
$(window).scroll(function () {
    var height = $(window).scrollTop();
    if (height > 44) {
        $("#listingtoptab").css({ "position": "sticky", "top": "44px", "z-index": "2000" });
    } else {
        $("#listingtoptab").css({ "position": "static" });
    }
});
$(document).ready(function () {
    var popupHeight = $(window).height();
    var imgHeight = $(window).height() - 273



    $(".pop_searc_sessn").height(popupHeight);
    $(".frInsMddl").height(imgHeight);



    //if (popupHeight > 650) {
    //    $(".frInsMddl").height("100%");
    //}
    var CloseCallPop = 0;
    $(".CloseCallPopFlag").click(function () {
        CloseCallPop++;
        if (CloseCallPop <= 1) {
            $('.CloseCallPopFlag').attr("onclick", "");
        } else {
            $('.CloseCallPopFlag').attr("data-dismiss", "modal");
        }
    });
});

//$(".first-pop-up").modal();
_closeCallPop = 0;
$(".first-close").click(function () {

    switch (_closeCallPop) {
        case 1:
            $('.first-close').removeAttr("href");
            $(".first-close").attr("data-dismiss", "modal");
            break;
        default:
            //setTimeout(function () {
            //    $("#phone_modal").modal("show");
            //}, 1000)
            //$(".smal_click").click(function () {
            //    $("#fit").show();
            //})
            break;
    }
    _closeCallPop++;
    });


//$(".newpopup").modal();
_closeCallPop = 0;
$(".new-close").click(function () {

    switch (_closeCallPop) {
        case 1:
            $('.new-close').removeAttr("href");
            $(".new-close").attr("data-dismiss", "modal");
            setTimeout(function () {
                $("#phone_modal").modal("show");
            }, 1000)
            break;
    }
    _closeCallPop++;
});

$(".smal_click").click(function () {
    $("#fit").css("display", "block")
    $("#call-top").css("display", "block")
});

function GetContractDetails(Guid, cid){
    
    $.ajax({
        url: DOMAIN_URL + "flights/getcontractdetails",
        type: 'POST',
        data: JSON.stringify({ Guid: Guid, ContractId: cid }),
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response != null && !response.IsContextExist) {

                $('#contractDetaildiv').html(response.HtmlResponse)
                $(".flightDetailWrapper").css("width", "100%")
                $(".detail_footer").css("right", "0px")
                $("body").addClass("open-model")
            } else {
                alert('error')
            }
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
}

function closeDetailPopup() {
    $(".flightDetailWrapper").css("width", "0%")
    $("body").removeClass("open-model")
    $(".detail_footer").css("right", "-200%")
}
function openFilterPopup(tab)
{
    $(".filtertab li").removeClass("active");
    $(".filtertab li a").removeClass("active");
    $(".filtertab div").removeClass("active");
    $(".filterCntr").addClass("open-filter");
    $(".filter_action_button").css("left", "0px")
    $(".results_body").addClass("open-model");    
    $("." + tab).addClass("active");
}
function closeFilterPopup() {
    $(".filterCntr").removeClass("open-filter");
    $(".results_body").removeClass("open-model");
    $(".filter_action_button").css("left", "-200%")
}

function clickTabShow(id, tab) {
    $("#departTabcontent").hide();
    $("#returnTabcontent").hide();
    $("#" + id).show();
    $(".tabclick").removeClass("active");
    $("#" + tab).addClass('active');
}
