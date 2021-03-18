//angular.module('QuizApp', ['timer'])
angular.module('QuizApp')
    .controller('PollingController', ['$scope', '$timeout', 'Utils', function ($scope, $timeout, Utils) {
    $scope.timerRunning = true;
    $scope.timerConsole = '';
    $scope.durationInSecond = 0;
    $scope.timerType = '';
    $scope.durationHistory = 0;
    //debugger;
    //alert(Utils.durationInSecond);

    $scope.startTimer = function () {
        $scope.$broadcast('timer-start');
        $scope.timerRunning = true;
    };

    $scope.stopTimer = function () {
        $scope.$broadcast('timer-stop');
        $scope.timerRunning = false;
    };
    init();
        function init() {
            debugger;
            alert("polling-controller: init");
            if (localStorage.getItem("durationInSecond") == null || localStorage.getItem("durationInSecond") == 0) {
                $scope.durationInSecond = Utils.duration;
            }
            else {
                if (localStorage.getItem("durationInSecond") > 1) {
                    // alert(localStorage.getItem("durationInSecond"));
                    $scope.durationInSecond = parseInt(localStorage.getItem("durationInSecond"));
                    // $scope.$apply();
                    //$scope.durationInSecond=1500;
                } else {
                    $scope.durationInSecond = Utils.duration;
                }
            }
        //console.log(localStorage.getItem("durationInSecond"));

    }
    $scope.$watch("durationInSecond", function (newValue, oldValue) {
        //console.log( "durationHistory:", newValue );
        if (newValue != oldValue)
            $scope.durationInSecond = newValue;
    }, true);

    function msToTime(ms) {
        var seconds = (ms / 1000);
        var minutes = parseInt(seconds / 60, 10);
        seconds = seconds % 60;
        var hours = parseInt(minutes / 60, 10);
        minutes = minutes % 60;

        return hours + ':' + minutes + ':' + seconds;
    }

    $scope.$on('timer-tick', function (event, args) {
        $timeout(function () {
            debugger;
            if ($scope.timerRunning && (args.millis / 1000) <= $scope.durationInSecond) {
                //$scope.$apply($scope.timeOver);
                var myTime = args.millis / 1000;
                //$scope.timerConsole +=  'seconds = ' + myTime +'\n';
                $scope.timerConsole += $scope.timerType + ' - event.name = ' + event.name + ', timeoutId = ' + args.timeoutId + ', millis = ' + args.millis + ', time=' + msToTime(args.millis) + ', seconds = ' + myTime + '\n';
                Utils.durationInSecond = $scope.durationInSecond = myTime;
                localStorage.setItem("durationInSecond", $scope.durationInSecond);
                //console.log( "durationHistory:", myTime);
                $scope.$apply();
            }

            //$scope.$apply();
            //alert(myTime);
        });
    });
}]);