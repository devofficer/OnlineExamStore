//angular.module('taskApp', ['ui.router'])
var taskApp = angular.module("taskApp", [
    'ngRoute', // routing
    'angular-loading-bar',
    'timer',
    'ui.bootstrap',// ui-bootstrap (ex: carousel, pagination, dialog)
    'ngJaxBind'
]);

//Showing Routing
//taskApp.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
//    debugger;
//    //$routeProvider.when('/:id', // GET VALUE AFTER #/
//    //                    {
//    //                        templateUrl: '/task/TaskList',
//    //                        controller: 'ctrl1Controller'
//    //                    });
//    //$routeProvider.when('/task/index/:id',
//    //    {
//    //    templateUrl: '/task/TaskList',
//    //    controller: 'ctrl1Controller'
//    //});
//    //$routeProvider.when('/showtasks',
//    //                    {
//    //                        templateUrl: '/task/TaskList',
//    //                        //controller: 'ctrl1Controller'
//    //                    });
//    //$routeProvider.when('/addtask',
//    //                    {
//    //                        templateUrl: '/task/New',
//    //                        controller: 'ctrl2Controller'
//    //                    });
//    //$routeProvider.otherwise(
//    //                    {
//    //                        redirectTo: '/'
//    //                    });
//    //$locationProvider.html5Mode({
//    //    enabled: true,
//    //    requireBase: false
//    //});
//    //$locationProvider.html5Mode(true).hashPrefix('!')
//    //$routeProvider.
//    // when('/phones', { templateUrl: 'partials/phone-list.html', controller: PhoneListCtrl }).
//    // when('/phones/:phoneId', { templateUrl: 'partials/phone-detail.html', controller: PhoneDetailCtrl }).
//    // otherwise({ redirectTo: '/phones' });

//    //$locationProvider.html5Mode(true);
//}]);


