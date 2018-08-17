$(document).ready(function () {

    // Reads the file from the computer, and writes its content in the corresponding text area.
    $("#nodesFile").on('change', function () {
        readFile("nodesFile", "nodesTextArea");
    });

    // Reads the file from the computer, and writes its content in the corresponding text area.
    $("#targetsFile").on('change', function () {
        readFile("targetsFile", "targetsTextArea");
    });

    // Reads the file from the computer, and writes its content in the corresponding text area.
    $("#drugtargetsFile").on('change', function () {
        readFile("drugtargetsFile", "drugtargetsTextArea");
    });

    // If one of the parameters was not correct, and the page reloads, then reshow all the fields.
    $(function () {
        var value = $('#selectAlgorithm').val();
        if (value === "genetic") {
            $('.genetic-parameters-form').collapse("show");
            $('.greedy-parameters-form').collapse("hide");
        } else if (value === "greedy") {
            $('.genetic-parameters-form').collapse("hide");
            $('.greedy-parameters-form').collapse("show");
        } else {
            $('.genetic-parameters-form').collapse("hide");
            $('.greedy-parameters-form').collapse("hide");
        }
    });

    // If the network is provided as seed, show the field asking how to generate the network.
    $("#isSeed").on('change', function () {
        if (document.getElementById('isSeed').checked) {
            $('#generationType').collapse("show");
        } else {
            $('#generationType').collapse("hide");
        }

    });

    // If a new type of algorithm is selected, then show the corresponding fields.
    $("#selectAlgorithm").on('change', function () {
        var value = $('#selectAlgorithm').val();
        if (value === "genetic") {
            $('.genetic-parameters-form').collapse("show");
            $('.greedy-parameters-form').collapse("hide");
        } else if (value === "greedy") {
            $('.genetic-parameters-form').collapse("hide");
            $('.greedy-parameters-form').collapse("show");
        } else {
            $('.genetic-parameters-form').collapse("hide");
            $('.greedy-parameters-form').collapse("hide");
        }
    });

    // Makes all the parameters of the unselected algorithm null.
    $("#createNew").on("click", function () {
        var value = $('#selectAlgorithm').val();
        if (value === "genetic") {
            $('.greedy-parameters').val("");
        }
        else if (value === "greedy") {
            $('.genetic-parameters').val("");
        }
        else {
            $('.genetic-parameters').val("");
            $('.greedy-parameters').val("");
        }
    });
});

function readFile(fileInput, textArea) {
    var selectedFile = document.getElementById(fileInput).files[0];
    var fileReader = new FileReader();
    fileReader.onload = function (e) {
        fileValues = e.target.result;
        if (fileValues.includes("graphml")) {
            var xmlDoc = $.parseXML(fileValues),
                $xml = $(xmlDoc),
                $edges = $xml.find("edge");

            $.each($edges, function () {
                $("#" + textArea).append($(this).attr("source") + "\t" + $(this).attr("target") + "\n");
            });
        }
        else {
            $("#" + textArea).val(fileValues);
        }
    };
    fileReader.readAsText(selectedFile);
}

function readGraphml(fileInput, textArea) {
    var selectedFile = document.getElementById(fileInput).files[0];
    var fileReader = new FileReader();
    fileReader.onload = function (e) {
        fileValues = e.target.result;
        var xmlDoc = $.parseXML(fileValues),
            $xml = $(xmlDoc),
            $edges = $xml.find("edge");

        $.each($edges, function () {
            $("#" + textArea).append($(this).attr("source") + "\t" + $(this).attr("target") + "\n");
        });

    };
    fileReader.readAsText(selectedFile);
}
