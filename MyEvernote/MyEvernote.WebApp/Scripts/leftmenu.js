var acik = false;
$(document).ready(function () {

    $("#kulakcik").mouseover(function () {
        if (!acik) {
            $("#leftmenu").animate({ left: "0px" }, "slow");
            $("#kulakcik").css("background-image", "url(left-arrow-orange.png)");
            acik = true;
        }
        else {
            $("#leftmenu").animate({ left: "-80px" }, "slow");
            $("#kulakcik").css("background-image", "url(right-arrow-orange.png)");
            acik = false;
        }
    });
});