'use strict';

$(document).ready(function () {

    var $slides = $('.con__slide').length,
        topAnimSpd = 650,
        textAnimSpd = 1000,
        nextSlideSpd = topAnimSpd + textAnimSpd,
        animating = true,
        animTime = 4000,
        curSlide = 1,
        nextSlide,
        scrolledUp;

    setTimeout(function () {
        animating = false;
    }, 2300);

    function navigateUp() {
        if (curSlide > 1) {
            scrolledUp = true;
            pagination(curSlide);
            curSlide--;
        }
    }

    function navigateDown() {
        if (curSlide < $slides) {
            scrolledUp = false;
            pagination(curSlide);
            curSlide++;
            console.log(curSlide);
        }
    }

    $(window).on('load', function () {
        $('.con__slide--1').addClass('active');
    });

    function pagination(slide, target) {
        animating = true;
        // Check if pagination was triggered by scroll/keys/arrows or direct click. If scroll/keys/arrows then check if scrolling was up or down.
        if (target === undefined) {
            nextSlide = scrolledUp ? slide - 1 : slide + 1;
        } else {
            nextSlide = target;
        }
        ////////// Slides //////////
        $('.con__slide--' + slide).removeClass('active');

        setTimeout(function () {
            $('.con__slide--' + nextSlide).addClass('active');
        }, nextSlideSpd);

        ////////// Nav //////////
        $('.con__nav-item--' + slide).removeClass('nav-active');
        $('.con__nav-item--' + nextSlide).addClass('nav-active');

        setTimeout(function () {
            animating = false;
        }, animTime);
    }

    // Mouse wheel trigger
    $(document).on('mousewheel DOMMouseScroll', function (e) {
        var delta = e.originalEvent.wheelDelta;
        if (animating) return;
        // Mouse Up
        if (delta > 0 || e.originalEvent.detail < 0) {
            navigateUp();
        } else {
            navigateDown();
        }
    });

    // Direct trigger
    $(document).on("click", ".con__nav-item:not(.nav-active)", function () {
        // Essential to convert target to a number with +, so curSlide would be a number
        var target = +$(this).attr('data-target');
        if (animating) return;
        pagination(curSlide, target);
        curSlide = target;
    });

    // Arrow trigger
    $(document).on('click', '.con__nav-scroll', function () {
        var target = $(this).attr('data-target');
        if (animating) return;
        if (target === 'up') {
            navigateUp();
        } else {
            navigateDown();
        }
    });

    // Key trigger
    $(document).on("keydown", function (e) {
        if (animating) return;
        if (e.which === 38) {
            navigateUp();
        } else if (e.which === 40) {
            navigateDown();
        }
    });

    var topLink = $(".con__slide--4-top-h-link"),
        botLink = $(".con__slide--4-bot-h-link");
    $(".con__slide--4-top-h-link, .con__slide--4-bot-h-link").on({
        mouseenter: function mouseenter() {
            topLink.css('text-decoration', 'underline');
            botLink.css('text-decoration', 'underline');
        },
        mouseleave: function mouseleave() {
            topLink.css('text-decoration', 'none');
            botLink.css('text-decoration', 'none');
        }
    });
});