
$(document).ready(function () {
    $(".datepicker").datepicker({
        format: "dd/mm/yyyy",
        todayBtn: "linked",
        multidateSeparator: "/",
        autoclose: true,
        todayHighlight: true
    });
    
    $('.dateOfBirth').datepicker({
        format: "dd/mm/yyyy",
        todayBtn: "linked",
        multidateSeparator: "/",
        autoclose: true,
        todayHighlight: true,
        endDate: '+0d',
        onRender: function (date) {
            return date.valueOf() > now.valueOf() ? 'disabled' : '';
        }
    });

    $('.datepicker-year').datepicker({
        format: "yyyy",
        viewMode: 'years',
        minViewMode: "years",
        autoclose: true
    });

    $('.datepicker-month-year').datepicker({
        format: "mm-yyyy",
        viewMode: 'months',
        minViewMode: "months",
        autoclose: true
    });

    $('.nano').nanoScroller({ scroll: 'top' });
});
