

(function ($) {

    $(window).scroll(function () {    // this will work when your window scrolled.
        var height = $(window).scrollTop();  //getting the scrolling height of window
        if (height > 70) {
            $(".fluid_header_gn").css({ "position": "fixed", "width": "100%", "z-index": "100", "top": "0px", "box-shadow": "2px 4px 6px -1px rgba(0, 0, 0, 0.10)" });
        } else {
            $(".fluid_header_gn").css({ "position": "relative", "box-shadow": "none" });
        }
    });


    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() > $(document).height() - 43) {
            $('.blck_strip').css({ 'position': 'static' });
        } else {
            $('.blck_strip').css('position', 'fixed');
        }
    });
    $(".NewSearch").click(function () {
        window.location = "/";
    });
    $(".RefreshResults").click(function () {
        location.reload();
    });
    $('.relaunchSearch').click(function () {
        $('#flights').submit();
    })
    $("#btnSubscriber").click(function () {

        if (!isEmail($('#email').val())) {
            $('#email').focus();
            $('#email').css({ 'border': "solid 1px red", 'color': '#000' });
        }
        else {
            $.ajax(
                {
                    type: "POST",
                    url: DOMAIN_URL + "savealert",
                    data: {
                        email: $("#email").val(),
                        subscriptiontype: $("#subscriptionType").val()
                    },
                    success: function (result) {
                        $("#email").val("Successfully subscribe").css('color', 'green');
                        $('#email').css('border', "solid 1px green");
                    },
                    error: function (request, error) {
                        console.log(arguments);
                    },

                });
        }
    });
    $('[data-toggle="tooltip"]').tooltip();
    //slider
    $('.carousel[data-type="multi"] .item').each(function () {
        var next = $(this).next();
        if (!next.length) {
            next = $(this).siblings(':first');
        }
        next.children(':first-child').clone().appendTo($(this));

        for (var i = 0; i < 2; i++) {
            next = next.next();
            if (!next.length) {
                next = $(this).siblings(':first');
            }

            next.children(':first-child').clone().appendTo($(this));
        }
    });
    $("#toTop").click(function () {
        $("html, body").animate({ scrollTop: 0 }, 1000);
    });
    /*************************from data in serializeFormJSON************************************************/
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
$("#menu-hemburger").click(function () { $("#nav-mobile").fadeIn("slow"); $("body").css({"overflow":"hidden"});  });
$("#sigInPop").click(function () { $("#UsprSignin").fadeIn("slow"); });
$(".close-nav, .closepoup").click(function () { $("#UsprSignin, #nav-mobile").fadeOut("slow"); $("body").css({ "overflow": "visible" }); });

/***********jquery************************/
function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}
function newsLetter(id) {
    getEmail = '#' + id + ' .email';
    getsubscriptionType = '#' + id + ' .subscriptionType';
    if (!isEmail($(getEmail).val())) {
        $(getEmail).focus();
        $(getEmail).css({ 'border': "solid 1px red", 'color': 'red' });
    }
    else {
        try {
            var dataObject = {
                'event': 'email-subscription',
                'lead_category': "subscription",
                'lead_action': 'submit-subscription-form',
                'lead_label': $("#email").val() + "|" + $("#subscriptionType").val()
            };
            if (typeof dataLayer != 'undefined') {
                dataLayer.push(dataObject);
            }
        } catch (t) { }

        $.ajax(
            {
                type: "POST",
                url: DOMAIN_URL + "savealert",
                data: {
                    email: $(getEmail).val(),
                    subscriptiontype: $(getsubscriptionType).val()
                },
                success: function (result) {
                    $(getEmail).val("Successfully subscribe").css('color', 'green');
                    $(getEmail).css('border', "solid 1px green");
                },
                error: function (request, error) {
                    console.log(error);
                },

            });
    }
}

function openWindow(theURL, winName, features) {
    window.open(theURL, winName, features);
}


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
$(document).ready(function () {
    $('.collapse').on('shown.bs.collapse', function () {
        $(this).parent().find(".glyphicon-menu-down").removeClass("glyphicon-menu-down").addClass("glyphicon-menu-up");
    }).on('hidden.bs.collapse', function () {
        $(this).parent().find(".glyphicon-menu-up").removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down");
    });
});
$('.shwHideTxtAirline').click(function () {
    $('.slide_text').slideToggle('slow');
    var txtval = $('.shwHideTxtAirline').text();
    if (txtval.match(/show more/gi)) {
        $('.shwHideTxtAirline').html("Show less...");

    } else {
        $('.shwHideTxtAirline').html("Show more...");

    }
});
$(".fltDealWrapr .shwHideTxtDeal").click(function (e) {
    var dearlarHeith = $('.dealinnrwrapr').height();
    var txtdeal = $('.shwHideTxtDeal span').text();
    if (txtdeal.match(/show more/gi)) {
        $('.shwHideTxtDeal span').html("Show less...");

    } else {
        $('.shwHideTxtDeal span').html("Show more...");

    }
    if (dearlarHeith <= 250) {
        $(".dealinnrwrapr").animate({ "height": "100%" }, 800);
    }
    else {
        $(".dealinnrwrapr").animate({ "height": "240px" }, 800);
    }
});

//lan_pg_airline


//shwHideTxtFaq



$(".lan_pg_airline .shwHideTxtFaq").click(function (e) {
    var cllapseheight = $('.cllapseheight').height();
    var txtfaq = $('.shwHideTxtFaq span').text();


    if (txtfaq.match(/show more/gi)) {
        $('.shwHideTxtFaq span').html("Less more...");

    } else {
        $('.shwHideTxtFaq span').html("Show more...");

    }
    if (cllapseheight <= 250) {
        $(".lan_pg_airline .faq.cllapseheight").animate({ "height": "100%" }, 800);
    }
    else {
        $(".lan_pg_airline .faq.cllapseheight").animate({ "height": "200px" }, 800);
    }
});



/***********************validation Start***********************************/

$('#flightStatus').click(function () {
    $(".error-div").hide();
    settimediv('.error-div');
    // Get the Login Name value and trim it
    var Origin = $.trim($('.flt_sts_eng #OriginSearch').val());
    var Destination = $.trim($('.flt_sts_eng #DestinationSearch').val());
    var FlightDate = $.trim($('.flt_sts_eng #FlightDate').val());
    var FlightID = $.trim($('.flt_sts_eng #FlightID').val());
    var PNR = $.trim($('.flt_sts_eng #PNR').val());

    // Check if empty of not
    if (Origin === '' || (Origin.length) < 3) {
        $('.flt_sts_eng #OriginSearch').focus();
        $(".flt_sts_eng #Depart_error").css('display', 'flex');
        return false;
    }
    else if (Destination === '' || (Destination.length) < 3) {
        $('.flt_sts_eng #DestinationSearch').focus();
        $(".flt_sts_eng #Return_error").css('display', 'flex');
        return false;
    }
    else if (Destination === Origin) {
        $('.flt_sts_eng #DestinationSearch').focus();
        $(".flt_sts_eng #SameCity_error").css('display', 'flex');
        return false;
    }

    else if (FlightDate == '') {
        $('.flt_sts_eng #FlightDate').focus();
        $(".flt_sts_eng #FlightDate_error").css('display', 'flex');
        return false;
    }
    else if (FlightID == '') {
        $('.flt_sts_eng #FlightID').focus();
        $(".flt_sts_eng #FlightID_error").css('display', 'flex');
        return false;
    }
    else if (PNR == '') {
        $('.flt_sts_eng #PNR').focus();
        $(".flt_sts_eng #PNR_error").css('display', 'flex');
        return false;
    }
    $(".errMsgFltStats").css('display', 'flex')
    $("#loaderdiv").show();
    setTimeout(function () {
        $("#loaderdiv").hide();
        $(".errMsgFltStatsText").show();
    }, 3000); // <-- time in milliseconds

});
$('.cls_errmsg').click(function () {
    $(".errMsgFltStats").css('display', 'none');
    $(".errMsgFltStatsText").hide();
});

$('.show-deals').click(function () {
    $('.view-more').slideToggle('slow');
    var txtval = $('.show-deals').text();
    if (txtval.match(/View More Deal/gi)) {
        $('.show-deals').html("Show Less Deals");

    } else {
        $('.show-deals').html("View More Deal");

    }
});

$('.show-deal').click(function () {
    $('.view-more1').slideToggle('slow');
    var txtval = $('.show-deal').text();
    if (txtval.match(/View More Deal/gi)) {
        $('.show-deal').html("Show Less Deals");
    } else {
        $('.show-deal').html("View More Deal");
    }
});
function closecross() { $(".popup_bsc_eco").slideUp(); }
function basicEconony(_economy, _containerId) {
    $(".popup_bsc_eco").slideUp(); try {
        $.ajax({
            url: DOMAIN_URL + "flights/basiceconomy/" + _economy, type: 'GET', contentType: "application/json; charset=utf-8", success: function (response) {
                if (response.IsSuccess) { $("#bsc_ecn_btn" + _containerId).show().html(response.HtmlResponse); }
            },
            start: function () { }, complete: function (data) { }
        });
    } catch (e) { console.log("ISSUE:" + e.toString()); }
}
$('.clsbtn').click(function () {
    $('.indPopBannr').fadeOut(500);

});

/* SignIn  */

var modal = document.getElementById("sigpoup");
var btn = document.getElementById("Signin");
var span = document.getElementsByClassName("close-it")[0];
$('#Signin').click(function () {
    modal.style.display = "block";
    $(".login-highlight").css('display', 'none');
});
$('.close-it').click(function () {
    closesinup();
});
function closesinup() {
    backdata();
    modal.style.display = "none";
    $("#Message").val('');
    $("#Message").val('display', 'none');
    $("#Password").val("");
    $("#ConfirmPassword").val("");
    $("#email").val("");
    $(".login-highlight").css('display', 'block');
}
$('#login-next').click(function () {
    $("#Message").val('');
    if (!isEmail($('#email').val())) {
        $('#email').focus();
        $('#email').css({ 'border': "solid 1px red", 'color': 'red' });
        $('#emailError').css('display', 'block');
    }
    else {
        $('#waitresponse').css('display', 'block');
        $('#email').css({ 'border': "solid 1px #ccc", 'color': '#495057' });
        $('#emailError').css('display', 'none');
        $(".email").text($("#email").val());
        $.ajax({
            url: DOMAIN_URL + "userprofile/checkuserexists?emailid=" + $("#email").val(),
            method: "GET",
            contentType: "application/json;",
            success: function (data) {
                if (data.success) {
                    $(".first-sec").css('display', 'none');
                    $('#waitresponse').css('display', 'none');
                    $("#ConfirmPassword").css('display', 'none');
                    $("#Registerbut").css('display', 'none');
                    $("#loginbut").css('display', 'block');
                    $(".second-sec").css('display', 'block');
                }
                else {
                    $(".first-sec").css('display', 'none');
                    $('#waitresponse').css('display', 'none');
                    $("#ConfirmPassword").css('display', 'none');
                    $("#loginbut").css('display', 'none');
                    $("#Registerbut").css('display', 'block');
                    $(".second-sec").css('display', 'block');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    }
});
$('#back-sec').click(function () {
    backdata();
});
function backdata() {
    $("#Message").val('');
    $("#Message").val('display', 'none');
    $(".first-sec").css('display', 'block');
    $("#Password").val("");
    $(".second-sec").css('display', 'none');
}
$("#Registerbut").click(function () {
    if (!validatePass($('#txtPassword').val())) {
        $('#txtPassword').focus();
        $('.error').css('display', 'block');
        $('#txtPassword').css({ 'border': "solid 1px red", 'color': 'red' });
    }
    else {
        $('.error').css('display', 'none');
        $('#txtPassword').css({ 'border': 'solid 1px #ccc', 'color': '#495057' });
        var dta = {
            EmailId: $("#email").val(),
            Password: $("#txtPassword").val(),
            ConfirmPassword: $("#txtPassword").val()// $("#txtConfirmPassword").val()
        };
        $('#waitresponse').css('display', 'block');
        callSLogin("registeruser", dta);
    }
});
$("#loginbutton").click(function () {
    if (!validatePass($('#txtPassword').val())) {
        $('#txtPassword').focus();
        $('.error').css('display', 'block');
        $('#txtPassword').css({ 'border': "solid 1px red", 'color': 'red' });
    }
    else {
        $('.error').css('display', 'none');
        $('#txtPassword').css({ 'border': 'solid 1px #ccc', 'color': '#495057' });
        var dta = {
            EmailId: $("#email").val(),
            Password: $("#txtPassword").val()
        };
        $('#waitresponse').css('display', 'block');
        callSLogin("login", dta);
    }

});
function forgetPass() {
    $.ajax({
        url: DOMAIN_URL + "userprofile/forgetpassword?emailid=" + $("#email").val(),
        method: "GET",
        contentType: "application/json;",
        success: function (data) {
            if (data.Status.IsSuccess) {
                $("#Message").text(data.Result.Message);
                $("#Message").val('display', 'block');
            }
            else {
                $("#Message").text(data.Status.Error.Description);
                $("#Message").val('display', 'block');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}
function callSLogin(url, dta) {
    $.ajax({
        url: DOMAIN_URL + "userprofile/" + url,
        method: "POST",
        contentType: "application/json;",
        data: JSON.stringify(dta),
        success: function (data) {
            $('#waitresponse').css('display', 'none');
            if (data.Status.IsSuccess == true) {
                if (url == "login") {
                    $("#userName").text(data.Result.UserContext.LoginUserDetail.EmailId.split('@')[0]);
                    toast.loginSuccess(data.Result.UserContext.LoginUserDetail.EmailId.split('@')[0] + ', logged in successfully.');
                    $("#loginData").css('display', 'none');
                    $("#menuDrop").css('display', 'block');
                }
                if (url == "registeruser") {
                    toast.loginSuccess(data.Result.Message);
                    closesinup();
                }
            }
            else {
                $("#Message").text(data.Status.Error.Description);
                $("#Message").val('display', 'block');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}
toast = {
    loginSuccess: function (name) {
        this.offalert();
        $("body").append('<div id="loginAlert"><div class="alerts alerts-success alerts-notification"><i class="fa-login-success"></i> ' + name + '</div></div>');//<div id="loginAlert" class="login-mesge-box"><div class="meage-text">' + name + '</div></div>');            
    },
    offalert: function () {
        setTimeout(function () { $("#loginAlert").remove() }, 3000);
    }
}
$(document).ready(function () {
    if (readCookie('_up') != null) {
        var user = JSON.parse(decodeURIComponent(readCookie('_up')));
        if (user == null) {
            logout();
        }
        else {
            $("#userName").text(user.EmailId.split('@')[0]);
            $("#loginData").css('display', 'none');
            $("#menuDrop").css('display', 'block');
        }
    }
});
function logout() {
    var user = JSON.parse(decodeURIComponent(readCookie('_up')));
    if (user != null) {
        toast.loginSuccess(user.EmailId.split('@')[0] + ', loggout successfully');
    }
    eraseCookie('_upt');
    eraseCookie('_up');
    $("#menuDrop").css('display', 'none');
    $("#sigpoup").css('display', 'none');
    $("#loginData").css('display', 'block');
    closesinup();
}
//
function validatePass(pswd) {
    if (pswd.length < 8) {
        return false;
    }
    //if (!pswd.match(/[A-z]/)) {
    //    return false;
    //}
    //if (!pswd.match(/[A-Z]/)) {
    //    return false;
    //}
    //if (!pswd.match(/\d/)) {
    //    return false;
    //}
    return true;
}
function confirmvalidatePass(pswd, confpswd) {
    if (pswd != confpswd) {
        return false;
    }
    return true;
}
function openFilter() {
    $('#filter-mobile').slideToggle();
    $("html, body").animate({
        scrollTop: 0
    }, 1000)
    console.log($("#filter-mobile").scrollTop(300))
}
function closeFilter() {

    $('#filter-mobile').slideUp();
}

$(document).ready(function () {
    
    $('#modify-search').click(() => {
        $('#slide-search-engine').slideToggle();
    });
  


    $('#menu').bind('click', function (event) {
        $('#mainnav ul').slideToggle();
    });

    $(window).resize(function () {
        var w = $(window).width();
        if (w > 768) {
            $('#mainnav ul').removeAttr('style');
        }
    });

});

 
    var projectedTime = new Date().getTime() + 15 * 1000 * 60;
  var timeInterval =  setInterval(() => {
        var currentTime = new Date().getTime()
        var CountDownTime = projectedTime - currentTime;
        var Minute = Math.floor((CountDownTime % (1000 * 60 * 60)) / (1000 * 60))
        var second = Math.floor((CountDownTime % (1000 * 60)) / 1000);
        if ($("#countdown").length) {
            document.getElementById("countdown").innerHTML = Minute + "m " + second + "s";
        }
        if (CountDownTime <= 0) {
            clearInterval(timeInterval);
        }
    }, 1000)
 



$(document).ready(function () {
    $(".filter_show").click(function () {
        $(".filter_main").slideToggle("slow");
        $(this).text(function (i, v) {
            return v === 'Filter Show' ? 'Filter Hide' : 'Filter Show'
        })
    });
});

$(document).ready(function () {
    $(".modi_search").click(function () {
        $(".result_con").slideToggle("slow");
        $(this).text(function (i, v) {
            return v === 'Modify Search' ? ' Search Hide' : 'Modify Search'
        })
    });
});
$(document).ready(function () {
    setTimeout(function () {          
        $('#homepagePopup').modal('show');
    }, 5000);



    $('.trust-logo-slider .owl-carousel').owlCarousel({
        loop: true,
        margin: 15,
        responsiveClass: true,
        responsive: {
            0: {
                items: 2,
                nav: true
            },
            575: {
                items: 2,
                nav: true
            },


            600: {
                items:3,
                nav: true
            },
            767: {
                items: 4,
                nav: false
            },
            1000: {
                items: 5,
                nav: true,
                loop: false
            },
            1200: {
                items: 7,
                nav: true,
                loop: false
            }
        }
    });
    $(".trust-logo-slider .owl-prev").html('<i class="fa fa-angle-left" aria-hidden="true"></i>');
    $(".trust-logo-slider .owl-next").html('<i class="fa fa-angle-right" aria-hidden="true"></i>');
});

function HideDialog() {
    $("#bkgOverlay").fadeOut(400);
    $("#delayedPopup").fadeOut(300);
}
$("#btnClose").click(function (e) {
    HideDialog();
    e.preventDefault();
});    
// Chat script end here


$(".currcy_wrppr").click(function (event) {
    $(".crrncyBoxWrppr").css({ 'display': 'flex' });
    $("body").css({ 'overflow': 'hidden' });

  });

$(".close-crry, .crrncyBoxWrppr ul li").click(function (event) {
    $(".crrncyBoxWrppr").css({ 'display': 'none' });
    $("body").css({ 'overflow': 'visible' })
});


function currencyChange(currency) {
    $.ajax({
        url: DOMAIN_URL + "flights/getcurrency/" + currency,
        type: 'GET',
        contentType: "application/json; charset=utf-8", success: function (response) {
            if (response.IsSuccess) {

                //$(".currency").show().html(response.data.CurrencyPrice);
                $(".chgCurrencySym").html(response.data.CurrencySymbol);
                $(".chgCurrencyType").html(response.data.CurrencyType);
                $("#changeFlag").removeClass().addClass("icn_flg_sprite " + response.data.CurrencyType);
                u = response.data.CurrencyPrice;
                r = response.data.CurrencySymbol;
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
                        $(this).html('-'+r + totalSub)
                    }
                });
            }
        },
        start: function () { }, complete: function (data) { }
    });
}

function getReviews() {
    
    var url = DOMAIN_URL + "flights/getreview";
    $.ajax({
        type: "POST",        
        url: url,
        success: function (response) {
            if (response.IsSuccess) {
                $("#gReview").html(response.HtmlResponse);                
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

$(document).ready(function () {
    getReviews();
});