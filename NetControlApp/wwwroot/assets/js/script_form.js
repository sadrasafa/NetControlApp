$(document).ready(function () {
var seeds;
var targetProteins;
var fileValues;
var gen=true;
var whatType="seeds";
/*var $ = function(selector){
    return document.querySelector(selector);
};*/
$('#param1').val(Math.floor((Math.random() * (2*2147483646)) - 2147483646));
$('#gparam1').val(Math.floor((Math.random() * (2*2147483646)) - 2147483646));
$('.fa').tooltip();
function repeat() {
        var patern=/,\n*|\n/;
        var patern2=/\d+$/;
        //document.getElementById("copy").innerText=number.toString();
        document.getElementById("copy").innerText=($("#user_input").val().split(patern).length-1)+" proteins";
}
function repeat2() {
    var patern=/,\n*|\n/;
    document.getElementById("tpcopy").innerText=($("#target_proteins").val().split(patern).length-1)+" targets";
}
$('#collapseDiv1').on('shown.bs.collapse', function () {
    console.log("Opened 1");
    //document.getElementById("gen").style.border = "solid #FF473B";
    $("#gen").addClass("active");
    $("#greedy").removeClass("active");
    $('#collapseDiv2').collapse("hide");
    $('#greedy').prop("disabled",true);
    gen=true;
});
$('#collapseDiv1').on('hidden.bs.collapse', function () {
    console.log("Closed 1");
   //document.getElementById("gen").style.border = "";
    
    $('#greedy').prop("disabled",false);
});
$('#collapseDiv2').on('shown.bs.collapse', function () {
    console.log("Opened 2");
    //document.getElementById("greedy").style.border = "solid #FF473B";
    $("#greedy").addClass("active");
    $("#gen").removeClass("active");
    $('#collapseDiv1').collapse("hide");
    $('#gen').prop("disabled",true);
    gen=false;
});
    $('#collapseDiv2').on('hidden.bs.collapse', function () {
    console.log("Closed 2");
        //document.getElementById("greedy").style.border = "";
    $('#gen').prop("disabled",false);
});
//#ff2315
$("#datafile").on('change',function(){
    readFile();
    whatType="graph";
});

/*window.onscroll = function (prevScrollPos) {
    var currentScrollPos = window.pageYOffset;
    if(prevScrollPos>currentScrollPos) {
        document.getElementById("HidePic")

    }
}*/
window.setInterval(function(){
        repeat();
        repeat2();
        },500);
function readTextArea(file){
    var data=file.val();
    return data
}
function readFile(){
    var selectedFile = document.getElementById('datafile').files[0];
    var fileReader = new FileReader();
    fileReader.onload =function(e){
        fileValues = e.target.result;
        $("#user_input").val(fileValues);
        document.getElementById("copy").innerText=$("#user_input").val().split("\n").length+' lines';
    };
    fileReader.readAsText(selectedFile);
}
//Main Function
$('form').on('submit', Main);

function Main(e){
    /*readFile();*/
    e.preventDefault();

    seeds = readTextArea($("#user_input"));
    targetProteins = readTextArea($("#target_proteins"));
    var drugTargets = readTextArea($('#drug_target'));
    var formName = readTextArea($('#form_name'));
    var doContact = $('#formCheck').is(":checked");
    var string= seeds+targetProteins;
    var param=[];
    var algoType;
    if(gen===true){
        algoType="gen";
        var el;
        for(var i=0; i<9; i++){
            el=document.getElementById('param'+(i+1)).value;
            param[i]=el;
        }
    }else{
        var el;
        for(var i=0; i<5; i++){
            el=document.getElementById('gparam'+(i+1)).value;
            param[i]=el;
        }
        algoType="greedy";
    }
    var stringParam = param.join();
    var json_obj = {
        "runName" : formName,
        "network" : {
            "type" : whatType,
            "nodes" : seeds,
        },
        "targets" : targetProteins,
        "drug_targets" : drugTargets,
        "userID": null ,
        "do_contact" : doContact,
        "algorithm" : {
            "type": algoType,
            "param": stringParam,
        }
    };
    var dictstring = JSON.stringify(json_obj);
    console.log(dictstring);
    var xhr = new XMLHttpRequest();
    var url = "/api";
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(dictstring);
    //fs.writeFile('file.json', dictstring, function (err) {
    //    if (err) return console.log(err);
    //    console.log('It worked > file.json');
    //});
    //console.log(json_obj);

    }
});



