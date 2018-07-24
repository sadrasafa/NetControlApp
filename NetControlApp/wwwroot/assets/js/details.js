$(document).ready(function () {
    $('.asd, .dit').on('click', function () {
        var clicked = $(this);
            
            //$(this.className + '> .collapseDiv').collapse("show");
            $('.collapseDiv').collapse('hide');
            $(this).find('.collapseDiv').collapse('show');
        
        
    });
    $('.icon').on('click', function (e) {
        e.stopPropagation();
    });

});