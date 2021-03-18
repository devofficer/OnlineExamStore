var quizApp = angular.module("quizApp", ['ngRoute', 'timer', 'ngBootbox']);
quizApp.config(['$routeProvider', function ($routeProvider, $locationProvider) {
    $routeProvider.
      when('/test', {
          templateUrl: '/templates/test.html',
          controller: 'QuizController'
      }).
         when('/test/:id', {
             templateUrl: '/templates/test.html',
             controller: 'QuizController'
         }).
      when('/solution/:id', {
          templateUrl: '/templates/solution.html',
          controller: 'QuizSolutionController'
      });
    //$ngBootboxConfigProvider.addLocale('ex', { OK: 'OK', CANCEL: 'Avbryt', CONFIRM: 'Bekräfta' });
    // use the HTML5 History API
    //$locationProvider.html5Mode(true);
}]);

quizApp.run(function ($rootScope, $window, $q,$location, Auth) {

    $rootScope.$on('$routeChangeStart', function (event, next, prev) {
        //debugger;
        Auth.getPromise().then(function (data) {
            //debugger;
            if (data === "" || data === null || typeof data === "undefined") {
                console.log('DENY');
                event.preventDefault();
                $window.location.href = '/account/login';
            } else if (data.membershipPlan == "Trial") {
                debugger;
                if ($location.path().indexOf('test') > -1) {
                    // PASS
                } else {
                    console.log('DENY');
                    console.log('Current route name: ' + $location.path());
                    event.preventDefault();
                    $window.location.href = '/account/dashboard';
                }
                
            } else {
                console.log('Do NOTHING');
            }
        }).catch(function (ex) {
            //debugger;
        });
    });
}).factory('Auth', function ($http) {

    var thePromise = $http.get('/api/SmartQuizApi/GetLoggedInUserInfo').then(function (response) {
        return response.data;
    });

    var getPromise = function () {
        return thePromise;
    };
    return {
        getPromise: getPromise
    };
});