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
        MultiAirines: 4,
        PhoneOnly: 5
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
    ApplyMatrixFilter: {
        ContractType: "",
        IsMatrixFilter: false,
        Stops: "",
        IsAirlineClick: false
    },
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
    Airlines: [],
    DepartureAirports: [],
    ReturnAirports: [],
    setTab: function (_tab) {
        this.arilineFilterByTab(_tab);
        this.Tab = _tab;
        this.applyFilter(this.Tab, false);
        $(".flexi-date li").removeClass("active");
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
            case this.EnumTabs.PhoneOnly:
                $(".PhoneOnly").addClass("active");
                break;
            default:
        }

    },
    applySetTab: function (_tab) {
        this.ApplyMatrixFilter.IsMatrixFilter = false;
        this.Airlines = null;
        $('.txt_clr').removeClass('active')
        this.setTab(_tab);
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
    SetapplyMatrixFilter: function (_airlines, _contractType, _isAirlineClicked) {
        $("input:checkbox[name=Airlines]:checked").each(function () {
            $(this).prop('checked', false);
        });
        $("input:checkbox[name=Stops]:checked").each(function () {
            $(this).prop('checked', false);
        });
        var airlinesArr = [_airlines];
        contractFilter.Airlines = airlinesArr;
        this.ApplyMatrixFilter.ContractType = _contractType;
        this.ApplyMatrixFilter.IsMatrixFilter = true;
        this.ApplyMatrixFilter.IsAirlineClicked = _isAirlineClicked;
        this.setTab(0);
    },
    resetStops: function () {
        $("input:checkbox[name=Stops]:checked").each(function () {
            $(this).prop('checked', false);
        });
        this.getStops();
        this.applyFilter(this.Tab, false);
    },
    getStops: function () {
        var stopArr = [];
        $("input:checkbox[name=Stops]:checked").each(function () {
            stopArr.push($(this).val());
            $('#f-' + $(this).val()).show();
        });
        $("input:checkbox[name=Stops]:not(:checked)").each(function () {
            $('#f-' + $(this).val()).hide();
        });
        var cc = 0;
        $("input:checkbox[name=Stops]").each(function () {
            if ($(this).is(':checked')) {
                cc = cc + 1;
            }
        });

        if (cc > 1) {
            $('#clear__all').show();
        } else {
            $('#clear__all').hide();
        }

        if (stopArr.length == 0) {
            stopArr = null;
        }
        this.Stops = stopArr;
    },
    setStops: function () {
        this.clearMatrixFilter();
        this.getStops();
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
        this.Airlines = airlinesArr;
    },
    setAirlines: function () {
        this.getAirlines();
        this.applyFilter(this.Tab, false);
    },
    resetSlider: function () {
        $(".ui-slider").each(function () {
            //var options = $(this).slider('option');
            //$(this).slider('values', [options.min, options.max]);

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
        this.ApplyMatrixFilter.IsMatrixFilter = false;
        this.Stops = null;
        this.Airlines = null;
        $('.txt_clr').removeClass('active')
        $("input:checkbox[name=Airlines]:checked").each(function () {
            $(this).prop('checked', false);
        });
        $("input:checkbox[name=DepartureAirports]:checked").each(function () {
            $(this).prop('checked', false);
        });
        $("input:checkbox[name=ReturnAirports]:checked").each(function () {
            $(this).prop('checked', false);
        });
        $("input:checkbox[name=Stops]:checked").each(function () {
            $(this).prop('checked', false);
        });
        this.resetSlider();
        return true;
    },
    clearMatrixFilter: function () {
        $('.txt_clr').removeClass('active')
        this.ApplyMatrixFilter.IsMatrixFilter = false;
        this.Airlines = null;
        this.Stops = null;
    },
    resetFilter: function () {
        this.clearAllFilter();
        this.setTab(0)
        this.applyFilter(0, false);
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
                            $("#cuurentCount").html(response.TotalResult);
                        }
                        enablePricePopup();
                    }
                    else {
                        if (isScroll != null && isScroll == true) {
                            $("#containerListing").append(response.HtmlResponse);
                        }
                        else {
                            $("#containerListing").html(response.HtmlResponse);
                            $("#cuurentCount").html(response.TotalResult);

                        }
                        enablePricePopup();
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
                    scroll_sidebar();
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
            Airlines: this.Airlines,
            Stops: this.Stops,
            MinPrice: this.MinPrice,
            MaxPrice: this.MaxPrice,
            OutboundDepTime: contractFilter.OutboundDepTime,
            OutboundArrTime: contractFilter.OutboundArrTime,
            OutboundDuration: contractFilter.OutboundDuration,
            InboundDepTime: null,
            InboundArrTime: null,
            InboundDuration: null,
            InboundDuration: null,
            ApplyMatrixFilter: this.ApplyMatrixFilter,
            DepartureAirports: this.DepartureAirports,
            ReturnAirports: this.ReturnAirports

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
    removeStopFilter: function (_id) {
        $('#' + _id).prop('checked', false);
        this.getStops();
        this.applyFilter(this.Tab, false);
    },
    resetDepartureAirports: function () {
        $("input:checkbox[name=DepartureAirports]:checked").each(function () {
            $(this).prop('checked', false);
        });
        this.applyFilter(this.Tab, false);
    },
    getDepartureAirports: function () {
        debugger;
        var depAirportsArr = [];
        $("input:checkbox[name=DepartureAirports]:checked").each(function () {
            depAirportsArr.push($(this).val());
        });
        if (depAirportsArr.length == 0) {
            depAirportsArr = null;
        }
        this.DepartureAirports = depAirportsArr;
    },
    setDepartureAirports: function () {
        this.getDepartureAirports();
        this.applyFilter(this.Tab, false);
    },
    resetReturnAirports: function () {
        $("input:checkbox[name=ReturnAirports]:checked").each(function () {
            $(this).prop('checked', false);
        });
        this.applyFilter(this.Tab, false);
    },
    getReturnAirports: function () {
        var retAirportsArr = [];
        $("input:checkbox[name=ReturnAirports]:checked").each(function () {
            retAirportsArr.push($(this).val());
        });
        if (retAirportsArr.length == 0) {
            retAirportsArr = null;
        }
        this.ReturnAirports = depAirportsArr;
    },
    setReturnAirports: function () {
        this.getReturnAirports();
        this.applyFilter(this.Tab, false);
    },
    resetOutboudDepartureTime: function () {
        this.OutboundDepTime.Min = parseInt($("#onBoundTimeRange").attr("data-default-min"), 10);
        this.OutboundDepTime.Max = parseInt($("#onBoundTimeRange").attr("data-default-max"), 10);
        this.applyFilter(this.Tab, false);
    },
    resetInboundDepartureTime: function () {
        if (this.TripType == this.EnumTripType.RoundTrip) {
            this.InboundDepTime.Min = parseInt($("#inBoundTimeRange").attr("data-default-min"), 10);
            this.InboundDepTime.Max = parseInt($("#inBoundTimeRange").attr("data-default-max"), 10);
            this.applyFilter(this.Tab, false);
        }
    }
};
function enablePricePopup() {
    $(".fare__detail").click(function () {
        $(".fare_breakup_detail").hide();
        $(this).next().slideDown();
    });
    $(".close_price_breakup").click(function () {
        $(".fare_breakup_detail").slideUp();
    });

    $(document).on('click touch', function (event) {
        if (!$(event.target).parents().addBack().is('.fare__detail')) {
            $('.fare_breakup_detail').slideUp();
        }
    });
    $('.fare_breakup_detail').on('click touch', function (event) {
        event.stopPropagation();
    });
}
function applyJquery() {
    //-------------------------------------------------------
    IsScrollEventFire = false;
    $(window).scroll(function () {
        if (!IsScrollEventFire) {
            if (($(window).scrollTop() + 558) >= $(document).height() - $(window).height()) {
                IsScrollEventFire = true;
                contractFilter.scrollVerticalPosition();
            }
        }
    });

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

    $('#departTimeFilter li').click(function (e) {
        if ($(this).attr('class') == 'selected') {
            $(this).removeClass('selected').siblings().removeClass('selected');
            contractFilter.resetOutboudDepartureTime();
        }
        else {
            $(this).addClass('selected').siblings().removeClass('selected');
        }

    });
    $('#returnTimeFilter li').click(function (e) {
        if ($(this).attr('class') == 'selected') {
            $(this).removeClass('selected').siblings().removeClass('selected');
            contractFilter.resetInboundDepartureTime();
        }
        else {
            $(this).addClass('selected').siblings().removeClass('selected');
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
            $("#onBoundTimeRange").val('Depart - ' + hours1 + ':' + minutes1 + ' ' + hours2 + ':' + minutes2);

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
            $("#inBoundTimeRange").val('Depart - ' + hours1 + ':' + minutes1 + ' ' + hours2 + ':' + minutes2);

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

    $('.FrMtrxWrppr .owl-prev').html('<i class="icon-arrow-left"></i>');
    $('.owl-next').html('<i class="icon-arrow-right"></i>');

    $(".txt_clr").click(function () {
        $('.txt_clr').removeClass('active')
        $(this).addClass('active')
    });
    $(".MtrxLogHd ").click(function () {
        $('.MtrxInnBxWppr').children('div').removeClass('active')
        $(this).parent().children('div').addClass('active')
    });
    $("#matRset ").click(function () {
        $('.MtrxInnBxWppr').children('div').removeClass('active')
        $('.txt_clr').removeClass('active')
    });

    $('.matrix__slider').slick({
        dots: false,
        infinite: false,
        speed: 300,
        slidesToShow: 5,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1100,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    infinite: true,
                    arrows: true,
                    dots: false

                }
            },
            {
                breakpoint: 767,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    arrows: true,
                    dots: false


                }
            },
            {
                breakpoint: 480,
                settings: {
                    arrows: false,
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    infinite: true,
                    arrows: true,
                    dots: false


                }
            }
        ]
    });

    enablePricePopup();

}
$(document).idle({
    onIdle: function () {
        $('#ListingIdlePopup').show();
    },
    idle: 900000
});

$(window).scroll(function () {
    var height = $(window).scrollTop();
    if (height > 164) {
        $("#listingtoptab").css({ "position": "sticky", "top": "0px", "z-index": "1039" });
    } else {
        $("#listingtoptab").css({ "position": "static" });
    }
});


function GetContractDetails(Guid, cid) {
    //var $myGroup = $('.flight-full-detail');
    //$myGroup.find('.collapse.in').collapse('hide');
    $.ajax({
        url: DOMAIN_URL + "flights/getcontractdetails",
        type: 'POST',
        data: JSON.stringify({ Guid: Guid, ContractId: cid }),
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response != null && !response.IsContextExist) {
                $('#ContractDetails' + cid + " .data").html(response.HtmlResponse)
                $(".flightDetailWrapper").css("width", "100%");
                $("body").addClass("open-model");
                $(".fare__detail").click(function () {
                    $(".fare_breakup_detail").hide();
                    $(this).next().slideDown();
                });
                $(".close_price_breakup").click(function () {
                    $(".fare_breakup_detail").slideUp();
                });
            } else {
                alert('error')
            }
        },
        complete: function (data) {
            $("#loadingDiv").css('display', "none");
        },
        beforeSend: function () {
            $("#loadingDiv").css('display', "block");
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
}

$('.close-sidebar').click(function () {
    $('.result-bg').removeClass('push');
    $('.page_overlay').hide();
    $('#leftCntr').removeClass('open');
});
$('.filter-btn, .modifyFilter').click(function () {
    $('#leftCntr').addClass('open');
    $('.result-bg').addClass('push');
    $('.page_overlay').show();
});



function closeDetailPopup() {
    $(".flightDetailWrapper").css("width", "0%")
    $("body").removeClass("open-model")
}
function isEmailItin(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

$("#btnSentItin").click(function () {

    if (!isEmailItin($('#itniEmail').val()) || $('#itniEmail').val().length > 200) {
        $('#itniEmail').focus();
        $('#itniEmail').css({ 'border': "solid 1px red", 'color': '#000' });
    }
    else {
        $.ajax(
            {
                type: "POST",
                url: DOMAIN_URL + "flights/sentitinerary",
                data: {
                    email: $("#itniEmail").val(),
                    itinGuid: $("#itinGuid").val(),
                    ContractId: $("#emailcontractId").val()
                },
                success: function (result) {
                    $("#sentBody").hide();
                    $('#sentSuccess').show();
                },
                error: function (request, error) {
                    $("#sentBody").hide();
                    $('#sentSuccess').show();
                },

            });
    }
});

function openSentItin(guid, contractId) {
    $('#itinGuid').val(guid);
    $('#emailcontractId').val(contractId);
    $('#emailItin').show();
    $("#sentBody").show();
    $('#sentSuccess').hide();

}


function scroll_sidebar() {

    var ww = $(window).width() || $(document).width();
    if (ww >= 768) {
        var scroll = $(window).scrollTop() || $(document).scrollTop();
        var top = $(".fixedSec").offset().top;
        var sidebarHeight = $(".fixedSec > *").height();
        var catContentSec = $(".clsContentCenter").height();
        var bottom = top + catContentSec - sidebarHeight;
        var sidebarWidth = $(".fixedSec").width();
        $(".fixedSec > *").css("width", sidebarWidth);

        if (scroll >= top && scroll <= bottom) {
            $(".fixedSec > *").addClass("leftBar").css({
                position: "fixed",
                top: "0"
            });
        } else if (scroll >= bottom) {
            $(".fixedSec > *").addClass("leftBar").css({
                position: "absolute",
                top: catContentSec - sidebarHeight - 40 + "px"
            });
        }
        else {
            $(".fixedSec > *").removeClass("leftBar").css({
                position: "absolute",
                top: "0"
            });
        }
    }
    if (ww <= 767) {
        $(".fixedSec > *").removeAttr("style");
    }
}
$(window).on("load scroll resize", scroll_sidebar);