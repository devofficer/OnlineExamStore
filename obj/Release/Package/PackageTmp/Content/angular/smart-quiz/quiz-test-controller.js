quizApp.controller('QuizController', function ($scope, $rootScope, $sce, $q, $routeParams, $timeout, $window, $location, $filter, $templateCache, $http) {
    $scope.skipCount = true;
    $scope.isListViewEnabled = false;
    $scope.isAccordionEnabled = false;
    $scope.selectedParentIndex = 1;//parentIndex--> INDICATES SECTIONS
    $scope.selectedIndex = 1;//index--> INDICATES QUESTIONS
    $scope.rightOption = null;
    $scope.isQuestionPaperVisible = false;
    $scope.isReporting = false;
    $scope.isReported = false;
    $scope.reportText = "";
    $scope.sectionCount = 0;
    $scope.questionCount = 0;
    $scope.test = "";
    $scope.selectedOption = {};
    $scope.selectedOptions = [];
    $scope.selectedQuestions = [];
    $scope.currentQuestion = {};
    $scope.questionPaperId = 0;
    $scope.duration = 0;

    $scope.questionPaperId = $routeParams.id;

    $window.start = new Date().getTime();

    init();

    function init() {
        loadQuiz();
    }

    //$scope.exampleText = "$$\sum_{i=0}^n i^2 = \frac{(n^2+n)(2n+1)}{6}$$";
    // $scope.exampleText = "$$ \begin{matrix}    1 & x & x^2 \\    1 & y & y^2 \\    1 & z & z^2 \\        \end{matrix}    $$";

    $scope.isNullOrEmptyOrUndefined = function (value) {
        if (value === "" || value === null || typeof value === "undefined") {
            return true;
        }
    }
    $scope.isCurrentQuestion = function (parentIndex, index) {
        return ($scope.selectedParentIndex === parentIndex && $scope.selectedIndex === index) ? true : false;
    }
    $scope.isIncorrect = function (parentIndex, index) {
        //debugger;
        //filter the array
        var result = false;
        if ($scope.test != "") {
            var ques = $scope.test.sections[parentIndex - 1].questions[index - 1];
            var userOption = ques.userOption;
            var options = ques.options;
            var foundItem = $filter('filter')(options, { id: userOption }, true)[0];
            if (foundItem.isRightOption == false) {
                result = true;
            }
        }
        return result;
        //return foundItem.isRightOption ? false : true;
    }
    $scope.isIncorrectCountBySection = function (index) {
        //filter the array
        var count = 0;
        var questions = $scope.test.sections[index].questions;
        angular.forEach(questions, function (ques, index) {
            var options = ques.options;
            var userOption = ques.userOption;
            var foundItem = $filter('filter')(options, { id: userOption }, true)[0];
            if (foundItem.isRightOption == false) {
                count = count + 1;
            }
        })
        return count;
    }
    $scope.isCorrect = function (parentIndex, index) {
        //debugger;
        //filter the array
        var result = false;
        if ($scope.test != "") {
            var ques = $scope.test.sections[parentIndex - 1].questions[index - 1];
            var userOption = ques.userOption;

            var options = ques.options;
            var foundItem = $filter('filter')(options, { isRightOption: true }, true)[0];
            if (foundItem.isRightOption == true && userOption == foundItem.id) {
                result = true;
            }
        }
        return result;
        // return $scope.isNullOrEmptyOrUndefined(foundItem) ? false : true;
    }
    $scope.correctCountBySection = function (index) {
        //filter the array
        var count = 0;
        var questions = $scope.test.sections[index].questions;
        angular.forEach(questions, function (ques, index) {
            var options = ques.options;
            var userOption = ques.userOption;
            var foundItem = $filter('filter')(options, { isRightOption: true }, true)[0];
            if (foundItem.isRightOption == true && userOption == foundItem.id) {
                count = count + 1;
            }
        })
        return count;
    }
    $scope.isUnattempted = function (parentIndex, index) {
        // debugger;
        var result = false;
        if ($scope.test != "") {
            var foundItem = $filter('filter')($scope.test.unattemptedIndexes, { parentIndex: parentIndex, index: index }, true)[0];
            $scope.isNullOrEmptyOrUndefined(foundItem) ? result = false : result = true;
        }
        return result;
    }
    function setCurrentQuestion(parentIndex, index) {
        $scope.currentQuestion = $scope.test.sections[parentIndex].questions[index];

        var sectionObj = $scope.test.sections[parentIndex];
        var questionObj = $scope.test.sections[parentIndex].questions[index];

        var foundItem = $filter('filter')($scope.selectedQuestions, { selectedParentIndex: parentIndex, selectedIndex: index }, true)[0];
        if (foundItem) {
            //DO NOTHING
        } else {
            $scope.selectedQuestions.push({
                selectedParentIndex: parentIndex,
                selectedIndex: index,
                sectionId: sectionObj.id,
                questionId: questionObj.id,
                isReported: false,
            });
        }
        if ($scope.currentQuestion != null)
        {
            $scope.questionTitle = "";
            $scope.questionTitle = $sce.trustAsHtml($scope.currentQuestion.title); 
        }
            
        //alert($sce.trustAsHtml($scope.currentQuestion.title));
        // $scope.getQuestionDesc1();
    }
    function setCurrentSelection(parentIndex, index) {
        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, selectedIndex: index }, true)[0];
        if (foundItem) {
            $scope.selectedOption = foundItem.option;
        }
    }
    $scope.sideBarItemClicked = function (parentIndex, index) {
        navigationSettings(parentIndex, index);
    }
    $scope.questionPaperItemClicked = function (parentIndex, index) {
        navigationSettings(parentIndex, index);
    }
    $scope.jumpToQuestion = function (parentIndex, index) {
        navigationSettings(parentIndex, index);
    }

    function navigationSettings(parentIndex, index) {
        //debugger;
        $scope.skipCount = false;
        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: $scope.selectedParentIndex - 1, selectedIndex: $scope.selectedIndex - 1 }, true)[0];
        if (!foundItem) {
            setCurrentOptions($scope.selectedParentIndex - 1, $scope.selectedIndex - 1, null);
        }

        $scope.selectedParentIndex = parentIndex;
        $scope.selectedIndex = index;

        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: $scope.selectedParentIndex - 1, selectedIndex: $scope.selectedIndex - 1 }, true)[0];
        if (foundItem) {
            $scope.selectedOption = foundItem.option;
        }
        setCurrentQuestion(parentIndex - 1, index - 1);
    }

    $scope.getOptions = function () {
        var result = null;
        if ($scope.test != "") {
            if ($scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1].options.length > 0) {
                var options = $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1].options;
                result = options;
            }
        }
        return result;
    }
    $scope.getIndex = function (index) {
        //debugger;
        var result = "";
        switch (index) {
            case 1: result = "A"; break;
            case 2: result = "B"; break;
            case 3: result = "C"; break;
            case 4: result = "D"; break;
            case 5: result = "E"; break;
        }
        return result;
    }
    $scope.questionTitle = ''

    $scope.getQuestionDesc1 = function () {
        //debugger;
        var result = "";
        if ($scope.test != "") {
            var ques = $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1];
            $scope.questionTitle = $sce.trustAsHtml(ques.title);

            //alert($scope.questionTitle);
        }
        // return result;
    }
    $scope.getQuestionDesc = function (parentIndex, index) {
        var result = "";
        if ($scope.test != "") {
            var ques = $scope.test.sections[parentIndex].questions[index];
            result = $sce.trustAsHtml(ques.title);
        }
        return result;
    }
    $scope.hasComprehension = function () {
        //debugger;
        if ($scope.currentQuestion.imagePath != null)
            return true;
        else {
            return false;
        }
    }
    $scope.report = function (reportType, reportText) {
        //debugger;
        $scope.isReporting = true;
        //alert("reportType " + reportType + " reportText " + reportText + "questionId " + $scope.currentQuestion.id);

        $http({
            url: "/api/SmartQuizApi/PostQuizErrorV2",
            method: "POST",
            params: {
                reportText: reportText,
                reportType: reportType,
                questionId: $scope.currentQuestion.id
            }
        }).then(function (result) {
            // Success
            //debugger;
            var foundItem = $filter('filter')($scope.selectedQuestions, { questionId: $scope.currentQuestion.id }, true)[0];
            if (foundItem) {
                foundItem.isReported = true
            }
            $scope.isReported = true;
            //alert(result.data);
        }, function (e) {
            //debugger;
            //error
            alert("Cann't save the question error.");
        });
    }
    $scope.hasReported = function () {
        $scope.isReporting = false;
        var foundItem = $filter('filter')($scope.selectedQuestions, { questionId: $scope.currentQuestion.id }, true)[0];
        if (foundItem) {
            return foundItem.isReported;
        }
        else {
            return false;
        }
    }
    $scope.getSolutionDesc = function () {
        //debugger;
        var result = "";
        if ($scope.test != "") {
            result = $sce.trustAsHtml($scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1].solution);
        }
        return result;
    }
    $scope.isNumerical = function () {
        return false;
    }
    $scope.navBtnPressed = function (isMoveNext) {
        // NEXT     : True
        // PREVIOUS : False
        if (isMoveNext) {
            if ($scope.selectedParentIndex < $scope.sectionCount) {
                if ($scope.selectedIndex < $scope.questionCount) {
                    $scope.selectedIndex = $scope.selectedIndex + 1;
                }
                else if ($scope.selectedIndex = $scope.questionCount) {
                    $scope.selectedIndex = 1;
                    $scope.selectedParentIndex = $scope.selectedParentIndex + 1;
                }
            }
            else if ($scope.selectedParentIndex == $scope.sectionCount) {
                $scope.selectedIndex = $scope.selectedIndex + 1;
            }

        } else {
            if ($scope.selectedParentIndex <= $scope.sectionCount) {
                //debugger;
                if ($scope.selectedIndex > 1 && $scope.selectedIndex <= $scope.questionCount) {
                    $scope.selectedIndex = $scope.selectedIndex - 1;
                }
                else if ($scope.selectedIndex = 1) {
                    $scope.selectedParentIndex = $scope.selectedParentIndex - 1;
                    $scope.selectedIndex = $scope.questionCount;
                }
            }
            else if ($scope.selectedParentIndex = 1) {
                $scope.selectedIndex = $scope.selectedIndex - 1;
            }
        }

    }
    $scope.toggleListView = function (isDisplay) {
        $scope.isListViewEnabled = isDisplay;
    }
    $scope.toggleSideBar = function (isDisplay) {
        $scope.isListViewEnabled = isDisplay;
    }
    $scope.changeSection = function () {
        $scope.isAccordionEnabled = !$scope.isAccordionEnabled;
    }

    function setCurrentOptions(parentIndex, index, option) {
        //debugger;
        var sectionObj = $scope.test.sections[parentIndex];
        var questionObj = $scope.test.sections[parentIndex].questions[index];

        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, selectedIndex: index }, true)[0];
        if (foundItem) {
            foundItem.option = option;
            foundItem.userOption = option != null ? option.value : option
        } else {
            $scope.selectedOptions.push({
                selectedParentIndex: parentIndex,
                selectedIndex: index,
                sectionId: sectionObj.id,
                questionId: questionObj.id,
                format: questionObj.format,
                userOption: option != null ? option.value : null,
                option: option
            });
        }
    }
    //Retrive data from database************
    function loadQuiz() {
        //debugger;
        //var def = $q.defer();
        $http.get("/api/SmartQuizApi/GetQuizById", {
            params:
            {
                questionPaperId: $scope.questionPaperId
            }
        }).success(function (data, status, headers, config) {
            //debugger;
            // deferred.resolve(data);
            $scope.test = data;
            if ($scope.test != "") {
                //debugger;
                $scope.duration = $scope.test.duration;
                setCurrentQuestion($scope.selectedParentIndex - 1, $scope.selectedIndex - 1);
                setCurrentOptions($scope.selectedParentIndex - 1, $scope.selectedIndex - 1, null);

                $scope.sectionCount = $scope.test.sections.length;
                if ($scope.selectedParentIndex > 0)
                    $scope.questionCount = $scope.test.sections[$scope.selectedParentIndex - 1].questions.length;
                else {
                    $scope.questionCount = $scope.test.sections[0].questions.length;
                }
            }
        }).error(function (data, status, headers, config) {
            //debugger;
            //deferred.reject(data);
        });
    };

    // Test View
    $scope.setOption = function (option) {
        // debugger;
        $scope.selectedOption = option;
        setCurrentOptions($scope.selectedParentIndex - 1, $scope.selectedIndex - 1, option);

    }
    $scope.saveAndNextBtnPressed = function (isMoveNext) {
        // NEXT     : True
        // Boorked : False
        //debugger;
        saveAndNext();
    }
    function saveAndNext() {
        $scope.skipCount = false;
        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: $scope.selectedParentIndex - 1, selectedIndex: $scope.selectedIndex - 1 }, true)[0];
        if (!foundItem) {
            //$scope.selectedOption = {};
            setCurrentOptions($scope.selectedParentIndex - 1, $scope.selectedIndex - 1, null);
        }

        if ($scope.selectedParentIndex < $scope.sectionCount) {
            if ($scope.selectedIndex < $scope.questionCount) {
                $scope.selectedIndex = $scope.selectedIndex + 1;
            }
            else if ($scope.selectedIndex = $scope.questionCount) {
                $scope.selectedIndex = 1;
                $scope.selectedParentIndex = $scope.selectedParentIndex + 1;
            }
        }
        else if ($scope.selectedParentIndex == $scope.sectionCount) {
            $scope.selectedIndex = $scope.selectedIndex + 1;
        }


        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: $scope.selectedParentIndex - 1, selectedIndex: $scope.selectedIndex - 1 }, true)[0];
        if (foundItem) {
            $scope.selectedOption = foundItem.option;
        }
        setCurrentQuestion($scope.selectedParentIndex - 1, $scope.selectedIndex - 1);

        //$timeout(function () {
        //    MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        //});
    }
    $rootScope.$watch(function () {
        MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        return true;
    });
    $scope.clearResponse = function () {
        $scope.selectedOption = {};
    }
    $scope.bookmarkAndNextBtnPressed = function () {
        debugger;
        $scope.skipCount = false;
        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: $scope.selectedParentIndex - 1, selectedIndex: $scope.selectedIndex - 1 }, true)[0];
        if (!foundItem) {
            //$scope.selectedOption = {};
            setCurrentOptions($scope.selectedParentIndex - 1, $scope.selectedIndex - 1, null);
        }

        if ($scope.selectedOptions.length > 0) {
            var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: $scope.selectedParentIndex - 1, selectedIndex: $scope.selectedIndex - 1 }, true)[0];
            if (foundItem) {
                foundItem.isBookmarked = true;
            }
        }
        //saveAndNext();
        if ($scope.selectedParentIndex < $scope.sectionCount) {
            if ($scope.selectedIndex < $scope.questionCount) {
                $scope.selectedIndex = $scope.selectedIndex + 1;
            }
            else if ($scope.selectedIndex = $scope.questionCount) {
                $scope.selectedIndex = 1;
                $scope.selectedParentIndex = $scope.selectedParentIndex + 1;
            }
        }
        else if ($scope.selectedParentIndex == $scope.sectionCount) {
            $scope.selectedIndex = $scope.selectedIndex + 1;
        }
    }
    $scope.bookmarkBtnPressed = function () {
        debugger;
        var result = false;
        if ($scope.selectedOptions.length > 0) {

            var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: $scope.selectedParentIndex - 1, selectedIndex: $scope.selectedIndex - 1 }, true)[0];
            //debugger;
            if (foundItem) {
                foundItem.isBookmarked = true;
            }
        }
        return result;
    }
    $scope.isBookmarked = function (parentIndex, index) {

        var result = false;
        if ($scope.selectedOptions.length > 0) {

            var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, selectedIndex: index }, true)[0];
            //debugger;
            if (foundItem && foundItem.isBookmarked == true) {
                result = true;
            }
        }
        return result;
    }
    $scope.isAttempted = function (parentIndex, index) {
        var result = false;
        if ($scope.selectedOptions.length > 0) {
            // debugger;
            var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex - 1, selectedIndex: index - 1 }, true)[0];
            //debugger;
            if (foundItem && foundItem.userOption != null) {
                result = true;

            }
        }
        return result;
    }
    $scope.isSkipped = function (parentIndex, index) {
        //debugger;
        var result = false;
        if ($scope.skipCount == true) {
            result = false;
        } else {
            if ($scope.selectedOptions.length > 0) {
                var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, selectedIndex: index }, true)[0];
                if (foundItem != null && foundItem.userOption == null) {
                    result = true;
                }
            }
        }
        return result;
    }
    $scope.bookmarkCount = function (parentIndex) {
        //debugger;
        var bookmark = 0;
        var questions = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, isBookmarked: true }, true);
        if (questions != null) {
            bookmark = questions.length;
        }
        return bookmark;
    }
    $scope.attemptedCount = function (parentIndex) {
        //debugger;
        var attempted = 0;
        if ($scope.skipCount == true) {
            skippedCount = 0;
        } else {
            angular.forEach($scope.selectedOptions, function (item) {
                if (item.selectedParentIndex == parentIndex && !$scope.isNullOrEmptyOrUndefined(item.userOption)) {
                    attempted++;
                }
            });
        }
        return attempted;
    }
    $scope.skippedCount = function (parentIndex) {

        var skippedCount = 0;
        if ($scope.skipCount == true) {
            skippedCount = 0;
        } else {
            var questions = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, userOption: null }, true);
            if (questions != null) {
                skippedCount = questions.length;
                //console.log(skippedCount);
            }
        }
        return skippedCount;
    }

    $scope.timerRunning = true;
    $scope.timerConsole = '';
    /*  TIMER CODE START HERE */
    $scope.startTimer = function () {
        $scope.$broadcast('timer-start');
        $scope.timerRunning = true;
        // console.log('timer-start');
        $scope.isTimerPaused = false;
    };
    $scope.stopTimer = function () {
        $scope.$broadcast('timer-stop');
        $scope.timerRunning = false;
        $scope.isTimerPaused = true;
    };
    $scope.resetTimer = function () {
        //debugger;
        $scope.$broadcast('timer-reset');
        //$scope.duration = 12000;
        // console.log('timer-reset');
    };
    $scope.$on('timer-tick', function (event, args) {
        $timeout(function () {
            //debugger;
            //console.log('timer-tick');
            if ($scope.timerRunning && (args.millis / 1000) <= $scope.duration) {
                var myTime = args.millis / 1000;
                $scope.duration = myTime;
                $window.sessionStorage.setItem($scope.test.name, myTime);
                // console.log("duration:", sessionStorage.getItem("duration"));
                $scope.$apply();
            }
        });
    });
    $scope.$on('timer-start', function (event, data) {
        //console.log('Timer start - data');
    });
    $scope.$on('timer-reset', function (event, data) {
        //console.log('Timer reset - data');
    });
    $scope.$on('timer-stopped', function (event, data) {
        //console.log('Timer Stopped - data');
    });

    $scope.timesUp = function () {
        // debugger;
        if ($scope.duration > 0) {
            //alert('timesUp:' + $scope.duration);
            $scope.submitBtnPressed();
        }
    }

    $scope.submitBtnPressed = function () {
        //debugger;
        //var deferred = $q.defer();
        var testViewModel = {
            id: $scope.test.questionPaperId,
            name: $scope.test.name,
            quizViewModel: $scope.selectedOptions,
            duration: $scope.duration
        }
        $http(
          {
              url: "/api/SmartQuizApi/PostQuiz",
              method: "POST",
              data: testViewModel
          })
          .then(function (result) {
              // Success
              debugger;
              //$window.location.href = '/SmartQuiz/Summary/' + questionPaperId;
              if (result.statusText == 'OK') {
                  debugger;
                  //$location.url('/solution/' + $scope.questionPaperId);
                  $window.location.href = '/SmartQuiz/Summary/' + $scope.questionPaperId;
              }
          }, function (e) {
              //debugger;
              //error
              alert(e.exceptionMessage);
          });

    }

    $scope.submitTestOptions = {
        //message: 'This is a message!',
        //title: 'The best title!',
        onEscape: function () {
            //$log.info('Escape was pressed');
        },
        show: true,
        backdrop: false,
        closeButton: true,
        animate: true,
        //className: 'test-class',
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i> NO',
                className: "btn-default",
                callback: function () {
                    //debugger;
                }
            },
            success: {
                label: '<i class="fa fa-check"></i> YES',
                className: "btn-primary",
                callback: function () {
                    //debugger;
                    $scope.stopTimer();
                    $scope.submitBtnPressed();
                }
            }
        }
    };
    $scope.playPauseButtonOptions = {
        //message: 'This is a message!',
        //title: 'The best title!',
        onEscape: function () {
            //$log.info('Escape was pressed');
        },
        show: true,
        backdrop: false,
        closeButton: true,
        animate: true,
        //className: 'test-class',
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i> NO',
                className: "btn-default",
                callback: function () {
                    //debugger;
                }
            },
            success: {
                label: '<i class="fa fa-check"></i> YES',
                className: "btn-info",
                callback: function () {
                    //debugger;
                    $scope.pauseBtnPressed();
                }
            }
        }
    };
    $scope.isSectionActive = function (index) {
        return true;
    }
    function initTimer() {
        // debugger;
        if ($scope.duration > 0) {
            $scope.resetTimer();
            $scope.startTimer();
        }
    }
    
    $scope.isTimerPaused = false;
    $scope.pauseBtnPressed = function () {
        if ($scope.timerRunning == true) {
            $scope.stopTimer();
        } else {
            $scope.startTimer();
        }
        $scope.$apply();
    }

    $timeout(function () {
        initTimer();
        console.log('ENDING MEASUREMENT: ' + (new Date().getTime() - $window.start + 1000) + 'ms');
    }, (new Date().getTime() - $window.start + 1000), false);
});