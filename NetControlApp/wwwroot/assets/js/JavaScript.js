$(document).ready(function () {
    console.log("asd");
    var isFormChanged;
    var gen = true;

    //window.setInterval(function () {
    //    repeat();
    //}, 1000);

    $("#datafile").on('change', function () {
        readFile("datafile", "user_input");
        $("#algoParams").trigger("change");
    });
    $("#datafileTargets").on('change', function () {
        readFile("datafileTargets", "target_proteins");
        $("#algoParams").trigger("change");
    });
    $("#datafileDrugs").on('change', function () {
        readFile("datafileDrugs", "target_drugs");
        $("#algoParams").trigger("change");
    });
    function readFile(fileInput, textArea) {
        var selectedFile = document.getElementById(fileInput).files[0];
        var fileReader = new FileReader();
        fileReader.onload = function (e) {
            fileValues = e.target.result;
            $("#" + textArea).val(fileValues);
            //document.getElementById("copy").innerText = $("#user_input").val().split("\n").length + ' lines';
        };
        fileReader.readAsText(selectedFile);
    }
    $('#param1').val(Math.floor((Math.random() * (2 * 2147483646)) - 2147483646));
    $('#gparam1').val(Math.floor((Math.random() * (2 * 2147483646)) - 2147483646));
    $("#selectAlgo").on('change', function () {
        var value = $('#selectAlgo').val();
        if (value === "genetic") {
            //console.log("Gen");
            gen = true;
            isFormChanged = true;
            window.scrollTo(0, 0);
            //console.log(gen);
            $('#collapseDiv1').collapse("show");
            $('#collapseDiv2').collapse("hide");
            $("#algoParams").trigger("change");
        } else if (value === "greedy") {
            //console.log("Greedy");
            gen = false;
            isFormChanged = true;
            //console.log(gen);
            window.scrollTo(0, 0);
            $('#collapseDiv1').collapse("hide");
            $('#collapseDiv2').collapse("show");
            $("#algoParams").trigger("change");
        } else if (value === "def") {
            gen = true;

            $('#collapseDiv1').collapse("hide");
            $('#collapseDiv2').collapse("hide");
            $("#algoParams").trigger("change");
        }
    });
    $("input").change(function () {
        console.log($(this).attr('name'));
        isFormChanged = true;

        var param = [];
        if (isFormChanged === true && gen === true) {
            var el;
            console.log("changed-gen");
            for (var i = 0; i < 9; i++) {
                el = document.getElementById('param' + (i + 1)).value;
                param[i] = el;
            }
            var string = param.join(';');

        } else if (isFormChanged === true && gen === false) {
            var el;
            console.log("changed-greed");
            for (var i = 0; i < 5; i++) {
                el = document.getElementById('gparam' + (i + 1)).value;
                param[i] = el;
            }
            var string = param.join(';');
        }
        console.log(gen);
        $("#algoParams").val(string);
    });

});