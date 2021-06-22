//var smartQuizModule = angular.module("smartQuizModule", [, 'taskApp']);
taskApp.controller('QuizCtrl', function ($scope, $compile, $http, $q, $timeout, $uibModal, $window, $location, $element, Utils) {
    //debugger;
    $scope.answered = false;
    $scope.title = "loading question...";
    $scope.options = [];
    $scope.correctAnswer = false;
    $scope.working = false;
    $scope.userAnswer = '';
    $scope.pageIndex = 0;
    $scope.currentIndex = 0;
    $scope.name = '';
    $scope.questionCount = 0;
    $scope.index = 0;
    $scope.formatType = '';
    $scope.selected = { option: '' }
    $scope.duration = 0;
    $scope.questionPaperId = 0;
    $scope.questionId = 0;
    $scope.userId = '';
    $scope.subject = '';
    $scope.isStart = false;
    $scope.timerRunning = true;
    $scope.timerConsole = '';
    $scope.message = 'NO RECORD FOUND';
    $scope.questionCreatedBy = "";
    /*  TIMER CODE START HERE */
    $scope.startTimer = function () {
        $scope.$broadcast('timer-start');
        $scope.timerRunning = true;
        // console.log('timer-start');
    };
    $scope.stopTimer = function () {
        $scope.$broadcast('timer-stop');
        $scope.timerRunning = false;
    };
    $scope.resetTimer = function () {
        //debugger;
        $scope.$broadcast('timer-reset');
        //$scope.duration = 12000;
        // console.log('timer-reset');
    };
    $scope.$watch("duration", function (newValue, oldValue) {

        if (newValue != oldValue)
            $scope.duration = newValue;

        if ($scope.duration > 0)
            $scope.isStart = true;

        if ($scope.isStart == true && $scope.duration == 0) {
            debugger;
            console.log("save is called");
            $scope.message = 'Your time is up ! \n\n Let\'s see the Result & Statistics of the Test.';
            $scope.save($scope.questionPaperId, $scope.currentIndex, true, 0);
        }


    }, true);
    $scope.$watch("quizTimer", function (newValue, oldValue) {
        // console.log("duration:", newValue);
        //debugger;
    }, true);

    $scope.$on('timer-start', function (event, data) {
        // console.log('Timer start - data');
    });
    $scope.$on('timer-reset', function (event, data) {
        // // console.log('Timer reset - data');
    });
    $scope.$on('timer-stopped', function (event, data) {
        // console.log('Timer Stopped - data');
    });

    $scope.$on('timer-tick', function (event, args) {
        $timeout(function () {
            //debugger;
            // console.log('timer-tick');
            if ($scope.timerRunning && (args.millis / 1000) <= $scope.duration) {
                var myTime = args.millis / 1000;
                $scope.duration = myTime;
                $window.sessionStorage.setItem($scope.questionPaperId, myTime);
                // console.log("duration:", sessionStorage.getItem("duration"));
                $scope.$apply();
            }
        });
    });
    $scope.answer = function () {
        return $scope.correctAnswer ? 'correct' : 'incorrect';
    };
    $scope.nextQuestion = function (isNext) {
        //debugger;
        $scope.working = true;

        $scope.answered = false;
        $scope.title = "loading question...";
        $scope.options = [];
        //isNext = isNext == undefined ? 1 : 0;

        $http.get("/api/trivia", { params: { isNext: isNext } }).success(function (data, status, headers, config) {
            //debugger;
            $scope.options = data.options;
            $scope.title = data.title;
            $scope.answered = false;
            $scope.working = false;
        }).error(function (data, status, headers, config) {
            //debugger;
            $scope.title = "Oops... something went wrong";
            $scope.working = false;
        });
    };
    $scope.save = function (questionPaperId, currentIndex, isSave, isNext) {
        //debugger;
        //alert($scope.selected.option);
        var deferred = $q.defer();
        $scope.working = true;
        $scope.answered = false;
        $scope.title = "loading question...";
        $scope.currentIndex = currentIndex;
        $scope.cQuestions = [];/* If you have no data to put in yet. */

        if ($scope.formatType == 'CP' && isNext) {
            angular.forEach($scope.cquestions, function (item, index) {
                $scope.cQuestions.push({ Id: item.id, UserAnswer: item.userAnswer }); /* Object literal. */
            })
        }
        var data = {
            currentIndex: currentIndex,
            preIndex: $scope.pageIndex,
            formatType: $scope.formatType,
            isNext: isNext,
            isSubmit: isSave,
            cQuestions: $scope.cQuestions,
            userAnswer: $scope.selected.option == null ? "" : $scope.selected.option,
            duration: $scope.duration,
            attemptedDuration: parseInt(Utils.durationInSecond / 60, 10) // CONVERT SECONDS INTO MINTUES
        };

        $http.post('/api/smartquizapi/PostQuizData', data)
            .success(function (data, status, headers, config) {
                //$scope.correctAnswer = (data === true);
                //$scope.working = false;
                //debugger;
                if (isSave) {
                    $scope.title = data;
                    $scope.questions = null;
                    $scope.answered = false;
                    $scope.working = false;
                    //$location.path("/addtask");
                    $window.location.href = '/SmartQuiz/Summary/' + questionPaperId;
                }
                else {
                    $scope.questionPaperId = questionPaperId;
                    $scope.questions = data.questions;
                    $scope.cquestions = data.questions[0].childQuestions;

                    $scope.selected.option = $scope.questions[0].userAnswer; //RESET ANSWER
                    $scope.userAnswer = $scope.questions[0].userAnswer;
                    $scope.title = data.questions[0].title;
                    $scope.answered = false;
                    $scope.working = false;
                    $scope.questionCount = data.questionCount;
                    $scope.totalMarks = data.totalMarks;
                    //$scope.duration = data.duration;
                    $scope.pageIndex = data.questions[0].index;
                    $scope.questionId = data.questions[0].questionId;
                    $scope.name = data.name;
                    $scope.subject = data.subject;
                    deferred.resolve(data);
                }
            }).error(function (data, status, headers, config) {
                //debugger;
                $scope.title = "Oops... something went wrong";
                $scope.working = false;
                deferred.reject(data);
            });
        return deferred.promise;
    };
    $scope.sendAnswer = function (option) {
        //debugger;
        $scope.working = true;
        $scope.answered = true;

        $http.post('/api/smartquiz', { 'questionId': option.questionId, 'optionId': option.id }).success(function (data, status, headers, config) {
            $scope.correctAnswer = (data === true);
            $scope.working = false;
        }).error(function (data, status, headers, config) {
            $scope.title = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.init = function (questionPaperId, currentIndex, isSave, isNext) {
       // debugger;
        sessionStorage.clear();
        $scope.currentIndex = currentIndex;
        getQuestion(questionPaperId, currentIndex, isSave, isNext);
        //$scope.myXhr = getQuestion(questionPaperId, currentIndex, isSave, isNext);
        //return getQuestion(questionPaperId, currentIndex, isSave, isNext).then(function () {
        //    //debugger;
        //    //initTimer();
        //    //$scope.$broadcast('timer-reset');
        //    //$scope.$broadcast('timer-start');
        //    //var item = document.getElementById("quizTimer");
        //    //$element.find('#quizTimer').prop("autostart", true);

        //    //alert($element.find('#quizTimer').data('id'));
        //    //console.log('question returned to controller.');
        //}, function (data) {
        //    console.log('question retrieval failed.')
        //});
        //$timeout(function () {

        //$.when(getQuestion(questionPaperId, currentIndex, isSave, isNext),initTimer()).then(
        //    function () {
        //        debugger;
        //        alert('g');
        //    }, function (data) {
        //        console.log('question retrieval failed.')
        //    }
        //    );

        //});// timeout
        //debugger;
        //var myPromise = $scope.myXhr;
        //// wait until the promise return resolve or eject
        ////"then" has 2 functions (resolveFunction, rejectFunction)
        //myPromise.then(function (resolve) {
        //    debugger;
        //    //alert(resolve);
        //    //$scope.$broadcast('timer-reset');
        //    //$scope.$broadcast('timer-start');
        //}, function (reject) {
        //    alert(reject)
        //});
    }
    $scope.submitTestOptions = {
        //message: 'This is a message!',
        //title: 'The best title!',
        onEscape: function () {
            $log.info('Escape was pressed');
        },
        show: true,
        backdrop: false,
        closeButton: true,
        animate: true,
        //className: 'test-class',
        buttons: {
            warning: {
                label: "NO",
                className: "btn-default",
                callback: function () { }
            },
            success: {
                label: "YES",
                className: "btn-info",
                callback: function () {
                    //$scope.submitBtnPressed();
                }
            }
        }
    };
    function getQuestion(questionPaperId, currentIndex, isSave, isNext) {
        // var def = $q.defer();
        // debugger;
        $scope.working = true;
        $scope.answered = false;
        $scope.title = "loading question...";
        $scope.options = [];

        $http.get("/api/smartquizapi", {
            params:
                {
                    currentIndex: currentIndex,
                    questionPaperId: questionPaperId
                }
        }).success(function (data, status, headers, config) {
            //debugger;
            //deferred.resolve(data);
            //debugger;
            $scope.questionPaperId = questionPaperId;
            $scope.userId = data.userId;
            if ($window.sessionStorage.getItem(questionPaperId) != null && $window.sessionStorage.getItem(questionPaperId) > 1) {
                $scope.duration = parseInt($window.sessionStorage.getItem(questionPaperId));
            } else {
                $scope.duration = data.duration; // CONVERT MINUTES INTO SECOND
            }

            $scope.questions = data.questions;
            if (data.questions.length > 0) {
                $scope.cquestions = data.questions[0].childQuestions;
                $scope.pageIndex = $scope.index = data.questions[0].index;
                $scope.formatType = data.questions[0].formatType;
                $scope.title = data.questions[0].title;
                $scope.userAnswer = $scope.questions[0].userAnswer;
                $scope.name = data.name;
                $scope.subject = data.subject;
                $scope.questionId = $scope.questions[0].questionId;
            }
            //$scope.duration = data.duration;
            $scope.answered = false;
            $scope.working = false;
            $scope.questionCount = data.questionCount;
            $scope.totalMarks = data.totalMarks;
            //alert($scope.duration);
            //initTimer();
            // def.resolve(data);
            //$scope.startTimer();

        }).error(function (data, status, headers, config) {
            // debugger;
            // deferred.reject(data);
            $scope.title = "Oops... something went wrong";
            $scope.working = false;
            // def.reject("Failed to get question");
        }).then(function (data) {
            //debugger;
            //$timeout(function () {
            //    debugger;
            //    var endTime = data.data.duration;
            //    var timerScope = $scope.$new();
            //    var ss = $('<timer countdown="'+ endTime +'" interval="1000"> {{hhours}}:{{mminutes}}:{{sseconds}}</timer>')
            //    var timerNode = $('<timer language="en" countdown="' + endTime + '" interval="1000">{{days}} ימים, {{hours}} שעות,{{mminutes}} דקות,{{sseconds}} שניות</timer>');

            //    //var ctrl = $element.find('#countdowntimer');
            //    //var myEl = angular.element(document.querySelector('#countdowntimer'));
            //    //alert(myEl.attr('countdown'));
            //    //myEl.attr('countdown', endTime);
            //    //$compile(myEl)
            //    //$element.find('#countdowntimer').attrs('countdown', endTime);

            //    var el = angular.element("#countdowntimer");
            //    el.attr('countdown', endTime);
            //    $scope = el.scope();
            //    $injector = el.injector();
            //    $injector.invoke(function ($compile) {
            //        $compile(el)($scope)
            //    })
            //    //$compile(timerNode)(timerScope);

            //});
        });
        // return def.promise;
    }


    //init(1, 1, false, 0);
    // INITALIZE TIMER
    function initTimer() {
        // debugger;
        if ($scope.duration > 0) {
            $scope.resetTimer();
            $scope.startTimer();
        }
    }
    $timeout(function () {
        //alert("initTimer");
        initTimer();
    }, 2500);

    $scope.openModel = function () {
        $scope.modalInstance = $uibModal.open({
            templateUrl: '/templates/quizErrorUI.html',
            //controller: 'reportQuizErrorCtrl',//angular.element('#question-error'),

            controller: function ($uibModalInstance, $scope) {
                //debugger;
                $scope.ok = function () {
                    //$uibModalInstance.dismiss('cancel');
                };
                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
                $scope.save = function () {
                   // debugger;
                    if ($scope.questionErrorForm.$invalid == true && $scope.questionErrorForm.$dirty == true)
                        return false;
                    else {
                        $http(
                       {
                           url: "/api/smartquizapi/PostQuizError",
                           method: "POST",
                           params: {
                               discription: $scope.newError.Discription,
                               questionId: $scope.newError.QuestionId
                           }
                       })
                       .then(function (result) {
                           // Success
                           //debugger;
                           var newError = result.data;
                           alert(result.data);
                           $scope.newError.Discription = "";
                           //$('#question-error').modal('hide');
                           $uibModalInstance.dismiss('cancel');

                       }, function (e) {
                          // debugger;
                           //error
                           alert("Cann't save the question error.");
                       });
                    }
                }


            },
            scope: $scope // <-- I added this
        });
    }

})
.filter('formatTimer', function () {
    return function (input) {
        //debugger;
        function z(n) { return (n < 10 ? '0' : '') + n; }
        // alert(input);
        var seconds = input % 60;
        var minutes = Math.floor(input / 60);
        var hours = Math.floor(minutes / 60);

        //var hours = (input / 3600).toString().split('.')[0];
        //var minutes = ((input % 3600) / 60).toString().split('.')[0];
        //var seconds = ((input % 3600) % 60).toString();

        return (z(hours) + ':' + z(minutes) + ':' + z(seconds));
    };
});

taskApp.directive('timer', function () {

    return function (scope, element, attrs) {
        angular.element(document).ready(function () {
            //MANIPULATE THE DOM
            //attrs.autostart = true;
            //alert(attrs.autostart);
            //scope.$apply();
            //debugger;
        });
    };

});

taskApp.directive("mathjaxBind", function () {
    return {
        restrict: "A",
        controller: ["$scope", "$element", "$attrs", function ($scope, $element, $attrs) {
            $scope.$watch($attrs.mathjaxBind, function (value) {
                var $script = angular.element("<script type='math/tex'>")
                    .html(value == undefined ? "" : value);
                $element.html("");
                $element.append($script);
                MathJax.Hub.Queue(["Reprocess", MathJax.Hub, $element[0]]);
            });
        }]
    };
});



