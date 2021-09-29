$(document).ready(function () {

    //disbaling some functions for Internet Explorer
    if ($.browser.msie) {
        $('#is-ajax').prop('checked', false);
        $('#for-is-ajax').hide();
        $('#toggle-fullscreen').hide();
        $('.login-box').find('.input-large').removeClass('span10');
    }

    //highlight current / active link
    $('ul.main-menu li a').each(function () {
        if ($($(this))[0].href == String(window.location))
            $(this).parent().addClass('active');
    });

    //animating menus on hover
    $('ul.main-menu li:not(.nav-header)').hover(function () {
        $(this).animate({ 'margin-left': '+=5' }, 300);
    },
        function () {
            $(this).animate({ 'margin-left': '-=5' }, 300);
        });

    //other things to do on document ready, seperated for ajax calls
    docReady();
});


function docReady() {
    //prevent # links from moving to top
    $('a[href="#"][data-top!=true]').click(function (e) {
        e.preventDefault();
    });

    //rich text editor
    //$.cleditor.defaultOptions.width = 740;
    //$.cleditor.defaultOptions.height = 300;
    //$('.cleditor').cleditor();

    //datepicker
    $('.datepicker').datepicker();

    $('[data-rel="chosen"],[rel="chosen"]').chosen();

    $('.btn-close').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().parent().fadeOut();
    });

    $('.btn-minimize').click(function (e) {
        e.preventDefault();
        var $target = $(this).parent().parent().next('.box-content');
        if ($target.is(':visible')) $('i', $(this)).removeClass('icon-chevron-up').addClass('icon-chevron-down');
        else $('i', $(this)).removeClass('icon-chevron-down').addClass('icon-chevron-up');
        $target.slideToggle();
    });
}
