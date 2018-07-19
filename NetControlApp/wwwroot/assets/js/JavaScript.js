$(document).ready(function () {
    console.log("asd");
    var gen = true;

    window.setInterval(function () {
        repeat();
    }, 1000);
    $('#param1').val(Math.floor((Math.random() * (2 * 2147483646)) - 2147483646));
    $('#gparam1').val(Math.floor((Math.random() * (2 * 2147483646)) - 2147483646));
    $("#selectAlgo").on('click', function () {
        var value = $('#selectAlgo').val();
        if (value === "genetic") {
            //console.log("Gen");
            gen = true;
            $('#collapseDiv1').collapse("show");
            $('#collapseDiv2').collapse("hide");
        } else if (value === "greedy") {
            //console.log("Greedy");
            gen = false;
            $('#collapseDiv1').collapse("hide");
            $('#collapseDiv2').collapse("show");
        }
    });






});