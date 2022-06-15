// all custom JS here //
$(document).ready(function(){
	$(".mobileMenu").click(function(){
		$(".mobileMenuBox").show().css("left","0");
	});
	
	$(".mobile_head .close").click(function(){
		$(".mobileMenuBox").hide().css("left","-200%");
	});
	//Traveler home and Results
		$(".traveller").click(function(){
			$(".travllerBox").slideDown();
		});
		$(".done_Btn").click(function(){
			$(".travllerBox").slideUp();
		});
		
		$(document).on('click touch', function(event) {
        if (!$(event.target).parents().addBack().is('.traveller, .currencyClick')) {
            $('.travllerBox').slideUp();
			 $('.currencyBox ul').hide();
        }
    });
		 $('.travllerBox, .currencyClick, .currencyClick').on('click touch', function(event) {
        event.stopPropagation();
    });
	
			
	$(".currencyClick").click(function(){
			$(this).next().slideToggle();
		});
		
		
	
	//Slick destination slider for home page
	
	$('.destination_slider').slick({
  dots: false,
  infinite: false,
  speed: 300,
  slidesToShow: 4,
  slidesToScroll: 1,
  responsive: [
    {
      breakpoint: 1024,
      settings: {
        slidesToShow: 3,
        slidesToScroll: 1,
        infinite: true,
		 arrows: false,
        dots: true

      }
    },
    {
      breakpoint: 767,
      settings: {
		arrows: false,
		centerMode: true,
		centerPadding: '0px',
		infinite: true,
		slidesToShow: 2,
		slidesToScroll: 1
		
      }
    },
    {
      breakpoint: 480,
      settings: {
        arrows: false,
		centerMode: true,
		centerPadding: '0px',
		infinite: true,
		slidesToShow: 1,
		slidesToScroll: 1

      }
    }
    // You can unslick at a given breakpoint now by adding:
    // settings: "unslick"
    // instead of a settings object
  ]
});


  //Slick service slider  for home page
	$('.service_slider').slick({
  dots: false,
  infinite: false,
  speed: 300,
  slidesToShow: 3,
  slidesToScroll: 3,
  responsive: [
 
  {
           breakpoint: 10000,
           settings: "unslick"
        },
    
  
    {
      breakpoint: 1024,
      settings: {
        slidesToShow: 3,
        slidesToScroll: 3,
        infinite: true,
        dots: true,
		settings: "slick"
      }
    },
    {
      breakpoint: 768,
      settings: {
        slidesToShow: 2,
        slidesToScroll: 1,
		dots: true,
      }
    },
    {
      breakpoint: 630,
      settings: {
        slidesToShow: 1,
        slidesToScroll: 1,
		dots: true,
      }
    }
    // You can unslick at a given breakpoint now by adding:
    // settings: "unslick"
    // instead of a settings object
  ]
});

	
	//
	});//Document close
	
$(document).ready(function () {
    //Slick Most popular  for airline page
    $('.popular_slider').slick({
        dots: false,
        infinite: false,
        speed: 300,
        slidesToShow: 4,
        slidesToScroll: 1,
        responsive: [

            // {
            //   breakpoint: 10000,
            //   settings: "unslick"
            //},


            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true,
                    settings: "slick"
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    dots: true,
                }
            },
            {
                breakpoint: 630,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    dots: true,
                }
            }
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });
    //End most popular slider
});

 //
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

(function ($) {
    $.fn.serializeFormJSON = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
})(jQuery);
!function (n) { "use strict"; n.fn.idle = function (e) { var t, i, o = { idle: 6e4, events: "mousemove keydown mousedown touchstart", onIdle: function () { }, onActive: function () { }, onHide: function () { }, onShow: function () { }, keepTracking: !0, startAtIdle: !1, recurIdleCall: !1 }, c = e.startAtIdle || !1, d = !e.startAtIdle || !0, l = n.extend({}, o, e), u = null; return n(this).on("idle:stop", {}, function () { n(this).off(l.events), l.keepTracking = !1, t(u, l) }), t = function (n, e) { return c && (e.onActive.call(), c = !1), clearTimeout(n), e.keepTracking ? i(e) : void 0 }, i = function (n) { var e, t = n.recurIdleCall ? setInterval : setTimeout; return e = t(function () { c = !0, n.onIdle.call() }, n.idle) }, this.each(function () { u = i(l), n(this).on(l.events, function () { u = t(u, l) }), (l.onShow || l.onHide) && n(document).on("visibilitychange webkitvisibilitychange mozvisibilitychange msvisibilitychange", function () { document.hidden || document.webkitHidden || document.mozHidden || document.msHidden ? d && (d = !1, l.onHide.call()) : d || (d = !0, l.onShow.call()) }) }) } }(jQuery);

// all custom JS here //
$(document).ready(function () {

    //Traveler home and Results
    $(".traveller").click(function () {
        $(".travllerBox").slideDown();
    });
    $(".done_Btn").click(function () {
        $(".travllerBox").slideUp();
    });

    $(document).on('click touch', function (event) {
        if (!$(event.target).parents().addBack().is('.traveller, .currencyClick')) {
            $('.travllerBox').slideUp();
            $('.currencyBox ul').hide();
        }
    });
    //$('.travllerBox, .currencyClick, .currencyClick').on('click touch', function (event) {
    //    event.stopPropagation();
    //});


    //$(".currencyClick").click(function () {
    //    $(this).next().slideToggle();
    //});

    //Results page
    //start fare breakup
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
    // End fare breakup

    //Results page
    $(".mobile__filter ul li, .filter_link").click(function () {
        $(".leftCntr").addClass("open-filter");
        $(".results_body").addClass("open-model");

    });

    $(".close_filter").click(function () {
        $(".leftCntr").removeClass("open-filter");
        $(".results_body").removeClass("open-model");

    });


    //
    $(".flight_detail_btn").click(function () {
        $(".flightDetailWrapper").css("width", "100%")
        $("body").addClass("open-model")
    });

    $(".close_detail").click(function () {
        $(".flightDetailWrapper").css("width", "0%")
        $("body").removeClass("open-model")
    });

});
//Rang slider
$(function () {
    $(".slider-range").slider({
        range: true,
        min: 0,
        max: 500,
        values: [82, 580],
        slide: function (event, ui) {
            $("#amount").val("$" + ui.values[0] + " - $" + ui.values[1]);
        }
    });

});
//Rang slider
//Tab function
function clickTabShow(id, tab) {
    $("#departTabcontent").hide();
    $("#returnTabcontent").hide();
    $("#" + id).show();
    $(".tabclick").removeClass("active");
    $("#" + tab).addClass('active');
}

function currencyChange(currency) {
    $.ajax({
        url: DOMAIN_URL + "flights/getcurrency/" + currency,
        type: 'GET',
        contentType: "application/json; charset=utf-8", success: function (response) {
            if (response.IsSuccess) {
                $(".curClass").html(response.data.CurrencyType);
                $('#currencyul').hide();
                u = response.data.CurrencyPrice;
                r = response.data.CurrencySymbol;
                ct = response.data.CurrencyType;
                $(".chgCurrency").each(function (n, t) {
                    if ($(this).attr("default-price")) {
                        var i = $(this).attr("default-price");
                        $(this).html(r)
                    }
                });
                $(".chgcomplete").each(function (n, t) {
                    if ($(this).attr("default-price")) {
                        var i = $(this).attr("default-price");
                        var totalSub = ((i * u).toFixed(2).toString());
                        $(this).html(r + totalSub)
                    }
                });

                $(".chgCurrencyTwo").each(function (n, t) {
                    if ($(this).attr("default-price")) {
                        var i = $(this).attr("default-price");
                        var totalSub = ((i * u).toString().split(".")[0]);
                        $(this).html(totalSub);
                    }
                });
                $(".chgWithCurrency").each(function (n, t) {
                    if ($(this).attr("default-price")) {
                        var i = $(this).attr("default-price");
                        var totalSub = ((i * u).toString().split(".")[0]);
                        $(this).html(r + totalSub);
                    }
                });
                $(".chgCurrencySub").each(function (n, t) {
                    if ($(this).attr("default-price")) {
                        var i = $(this).attr("default-price");
                        var totalSub = ((i * u).toFixed(2).toString().split(".")[1]);
                        $(this).html("." + totalSub);
                    }
                });
                $(".chgdiscount").each(function (n, t) {
                    if ($(this).attr("default-price")) {
                        var i = $(this).attr("default-price");
                        var totalSub = ((i * u).toFixed(2).toString());
                        $(this).html('-' + r + totalSub)
                    }
                });
                $(".chgtype").each(function (n, t) {
                    $(this).html(ct)
                });
            }
        },
        start: function () { }, complete: function (data) { }
    });
}

$(document).ready(function () {
    $(".NewSearch").click(function () {
        window.location = "/";
    });
    $(".RefreshResults").click(function () {
        location.reload();
    });
    $('.relaunchSearch').click(function () {
        $('#flights').submit();
    })
});

function getReviews() {

    var url = DOMAIN_URL + "flights/getreviews";
    $.ajax({
        type: "POST",
        url: url,
        success: function (response) {
            if (response.IsSuccess) {
                $("#gReview").html(response.HtmlResponse);
                readMoetext();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function getDeals() {
    var url = DOMAIN_URL + "flights/getdeals";
    $.ajax({
        type: "POST",
        url: url,
        success: function (response) {
            if (response.IsSuccess) {
                $("#flightDeal").html(response.HtmlResponse);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function getCurrency() {
    var url = DOMAIN_URL + "flights/getcurrency";
    $.ajax({
        type: "POST",
        url: url,
        success: function (response) {
            if (response.IsSuccess) {                
                currencyChange(response.HtmlResponse)
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function getNearByAirport() {
    var cookieVal = jQuery.parseJSON(readCookie('searchPerms'));
    if (cookieVal == null && $('#OriginSearch').val() == "" && $('#DestinationSearch').val() == "") {

        var url = DOMAIN_URL + "flights/naer-by-airport";
        $.ajax({
            type: "POST",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.HtmlResponse.IsMobile) {
                        $('#OriginSearch').val(response.HtmlResponse.OriginSearch);
                        $('#Origin').val(response.HtmlResponse.Origin);
                        $('#inputOrigin').addClass('valid');
                        $('#inputOrigin').val(response.HtmlResponse.OriginSearch);
                    } else {
                        $('.origsuggestion').removeClass('d-none');
                        $('#OriginSearch').val(response.HtmlResponse.OriginSearch);
                    }
                    
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}

$(document).ready(function () {
    getNearByAirport();
    getCurrency();
    getReviews();
    getDeals();
});

function readMoetext() {
    var maxLength = 100;
    $(".show-read-more").each(function () {
        var myStr = $(this).text();
        if ($.trim(myStr).length > maxLength) {
            var newStr = myStr.substring(0, maxLength);
            var removedStr = myStr.substring(maxLength, $.trim(myStr).length);
            $(this).empty().html(newStr);
            $(this).append(' <a href="javascript:void(0);" style="text-decoration:none" class="read-more">read more...</a>');
            $(this).append('<span class="more-text">' + removedStr + '</span>');
        }
    });
    $(".read-more").click(function () {
        $(this).siblings(".more-text").contents().unwrap();
        $(this).remove();
    });
}
