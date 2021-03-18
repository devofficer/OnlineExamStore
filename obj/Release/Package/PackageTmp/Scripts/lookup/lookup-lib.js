(function ($) {

    $.fn.BindTable = function (name, method, url, funName, eventFun) {
        //debugger;
        $('#' + name).bootstrapTable({
            method: method,
            url: url,
            sortable: false,
            cache: false,
            //height: 400,
            //striped: true,
            pagination: true,
            pageSize: 10,
            pageList: [10, 20, 50],
            search: true,
            minimumCountColumns: 2,
            clickToSelect: true,
            columns: [
                {
                    field: 'LookupId',
                    title: 'Id',
                    align: 'right',
                    valign: 'bottom',
                    sortable: true,
                    visible: false,
                    switchable: false
                },
                {
                    field: 'ModuleCode',
                    title: 'Module Code',
                    align: 'left',
                    valign: 'top',
                    sortable: true
                },
                {
                    field: 'Text',
                    title: 'Text',
                    align: 'left',
                    valign: 'top',
                    sortable: true
                },
                {
                    field: 'Value',
                    title: 'Value',
                    align: 'left',
                    valign: 'top',
                    sortable: true
                },
                {
                    field: 'Parent',
                    title: 'Parent',
                    align: 'left',
                    valign: 'top',
                    sortable: true
                },
                {
                    title: 'Action',
                    align: 'center',
                    valign: 'middle',
                    clickToSelect: false,
                    formatter: funName,
                    events: eventFun
                }
            ]
        });
    };
    $.fn.DestroyTable = function (name) {
        $('#' + name).bootstrapTable('destroy');
    };
    $.fn.callModalPopup = function (method, url, data, param) {
        debugger;
        $.ajax({
            type: method,
            url: url,
            data: data,
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $(this).DestroyTable('tab_' + param);
                debugger;
                $(this).BindTable('tab_' + param, 'GET', '/SystemSettings/GetLookups/?moduleCode=' + param, 'operate' + param, 'Events' + param);
                $('#hdn' + param).val(null);
                $('#text' + param).val(null);

            },
            error: function (response) {
                debugger;
                alert(response.responseText);
            }
        });
    };

}(jQuery));

