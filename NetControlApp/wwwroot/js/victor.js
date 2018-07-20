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
            $('.genetic-parameters').collapse("show");
            $('.greedy-parameters').collapse("hide");
            $('.genetic-greedy-parameters').collapse("show");
        } else if (value === "greedy") {
            $('.genetic-parameters').collapse("hide");
            $('.greedy-parameters').collapse("show");
            $('.genetic-greedy-parameters').collapse("show");
        } else {
            $('.genetic-parameters').collapse("hide");
            $('.greedy-parameters').collapse("hide");
            $('.genetic-greedy-parameters').collapse("hide");
        }
    });

    // If a new type of algorithm is selected, then show the corresponding fields.
    $("#selectAlgorithm").on('change', function () {
        var value = $('#selectAlgorithm').val();
        if (value === "genetic") {
            $('.genetic-parameters').collapse("show");
            $('.greedy-parameters').collapse("hide");
            $('.genetic-greedy-parameters').collapse("show");
        } else if (value === "greedy") {
            $('.genetic-parameters').collapse("hide");
            $('.greedy-parameters').collapse("show");
            $('.genetic-greedy-parameters').collapse("show");
        } else {
            $('.genetic-parameters').collapse("hide");
            $('.greedy-parameters').collapse("hide");
            $('.genetic-greedy-parameters').collapse("hide");
        }
    });
});

function readFile(fileInput, textArea) {
    var selectedFile = document.getElementById(fileInput).files[0];
    var fileReader = new FileReader();
    fileReader.onload = function (e) {
        fileValues = e.target.result;
        $("#" + textArea).val(fileValues);
    };
    fileReader.readAsText(selectedFile);
}