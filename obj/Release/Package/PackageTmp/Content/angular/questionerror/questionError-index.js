// JavaScript source code
(function () {
    angular.module('bsTable', [])
   .directive('bsTableControl', function () {
       return {
           restrict: 'EA',
           scope: {
               options: '='
           },
           link: function (scope, element, attr) {
               var tableCreated = false;
               scope.$watch('options', function (newValue, oldValue) {
                   if (tableCreated && newValue === oldValue) return;
                   $(element).bootstrapTable('destroy');
                   if (newValue) {
                       $(element).bootstrapTable(scope.options);
                   }
                   tableCreated = typeof (newValue) !== 'undefined';
               });
               $(window).resize(function () {
                   if (tableCreated)
                       $(element).bootstrapTable('resetView');
               })
           }
       };
   })
})();

function operateFormatter(value, row, index) {
    return '<a class="Corrected btn btn-success btn-xs" title="Correct">Correct &nbsp;<span class="glyphicon glyphicon-pencil"></span></a>&nbsp; ' +
                   '<a class="Ignored btn btn-default btn-xs" title="Ignore">Ignore &nbsp;<span class="glyphicon glyphicon-pencil"></span></a>&nbsp;';
}
function questionIdHyperLink(value, row, index) {
    return '<a class="question" href="../Question/Edit/' + row.QuestionId + '" title="View detail">' + row.QuestionId + '</a>&nbsp;';
}
window.operateEvents = {
    'click .Corrected': function (e, value, row, index) {
        ajaxResultPost(row, "Corrected");
    },

    'click .Ignored': function (e, value, row, index) {
        ajaxResultPost(row, "Ignored");
    }
};
function operateFormatter1(value, row, index) {
    //debugger;
    if (value != null && value.length > 5) {
        var date = new Date(parseInt(value.substr(6)));
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        return day + '/' + month + '/' + year + ' ' + date.toLocaleTimeString();
    } else {
        return value;
    }
}
//http://wenzhixin.net.cn/p/bootstrap-table/docs/getting-started.html
angular.module('app', ['bsTable'])
.controller('MainController', function ($scope, $http) {
    $scope.workspaces = [];
    $scope.workspaces.push({ name: 'Workspace 1' });
    //$scope.workspaces.push({ name: 'Workspace 2' });
    //$scope.workspaces.push({ name: 'Workspace 3' });
    bindGrid();
    function bindGrid() {
        $scope.tableOptions = {

            method: 'get',
            url: '/QuestionError/GetAll',
            //queryParams: function (pageSize, pageNumber, searchText) {
            //    return {
            //        start: pageSize * (pageNumber - 1),
            //        length: pageSize
            //    };
            //},
            //data: wk.rows,
            rowStyle: function (row, index) {
                //debugger;
                if (row.ActionTaken == "Submitted")
                    return { classes: 'info' };
                else if (row.ActionTaken == "Ignored") {
                    return { classes: 'default' };
                }
                if (row.ActionTaken == "Corrected")
                    return { classes: 'success' };
                else {
                    return { classes: 'default' };
                }
            },
            cache: false,
            height: 'auto',//400,
            striped: true,
            pagination: true,
            pageSize: 10,
            pageList: [5, 10, 25, 50, 100, 200],
            search: true,
            showColumns: true,
            showRefresh: true,
            minimumCountColumns: 2,
            clickToSelect: true,
            showToggle: true,
            maintainSelected: true,
            columns: [
            {
                field: 'Id',
                title: '#',
                align: 'right',
                valign: 'bottom',
                sortable: true,
                switchable: false,
                visible: false
            }, {
                field: 'Index',
                title: 'S.No.',
                align: 'center',
                valign: 'bottom',
                sortable: true
            }, {
                field: 'QuestionId',
                title: 'Question Id',
                align: 'center',
                valign: 'bottom',
                sortable: true,
                formatter: questionIdHyperLink
            }, {
                field: 'UserId',
                title: 'User Id',
                align: 'center',
                valign: 'middle',
                sortable: true,
                visible: false
            }, {
                field: 'Description',
                title: 'Description',
                align: 'left',
                valign: 'top',
                sortable: true
            },

            {
                field: 'ActionTaken',
                title: 'Action Taken',
                align: 'left',
                valign: 'top',
                sortable: true
            },
            {
                field: 'CreatedOn',
                title: 'Created On',
                align: 'left',
                valign: 'top',
                sortable: true,
                formatter: operateFormatter1
            },
            {
                field: 'CreatedBy',
                title: 'Created By',
                align: 'left',
                valign: 'top',
                sortable: true
            },
            
            {
                title: 'Action Taken',
                formatter: operateFormatter,
                events: operateEvents

            }
            ]
        };
    }

    $scope.editListItem = function editListItem(row, actionTaken) {
        //debugger;
        //alert(wk.QuestionId);
        $http.post("/QuestionError/UpdateAction", { Id: row.Id, ActionTaken: actionTaken }).then(function (result) {
            //debugger;
            $scope.workspaces = result.data;
            bindGrid();
            //anguler.copy(result.data, $scope.data);
        },
          function (e) {
              debugger;
              //Error
          });
    };
    $scope.changeCurrentWorkspace = function (wk) {
        //debugger;
        $scope.currentWorkspace = wk;
    }


    //Select the workspace in document ready event
    $(document).ready(function () {
        $scope.changeCurrentWorkspace($scope.workspaces[0]);
        $scope.$apply();
    });

});

function ajaxResultPost(row, actionTaken) {
    debugger;
    var scope = angular.element(document.getElementById("questionErrorDiv")).scope();
    scope.$apply(function () {
        scope.editListItem(row, actionTaken);
    });
}