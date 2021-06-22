quizApp.controller('QuizSolutionController', function ($scope, $rootScope, $sce, $q, $timeout, $filter, $routeParams, $templateCache, $http) {
    $scope.skipCount = 0;
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
    $scope.questionPaperId = 0;
    $scope.selectedOptions = [];
    $scope.currentQuestion = {};
    $scope.questionImageUrl = "";
    $scope.questionId = 0;
    $scope.questionSectionId = 0;
    $scope.questionTitle = "";
    $scope.questionPaperId = $routeParams.id;
    $scope.questionCreatedBy = "";
    init();

    function init() {
        loadQuiz();
    }
    $scope.isNullOrEmptyOrUndefined = function (value) {
        if (value === "" || value === null || typeof value === "undefined") {
            return true;
        }
    }
    $scope.isCurrentQuestion = function (parentIndex, index) {
        return ($scope.selectedParentIndex === parentIndex && $scope.selectedIndex === index) ? true : false;
    }
    $scope.isIncorrect = function (parentIndex, index) {
        var result = false;
        if ($scope.test != "") {
            //debugger;
            if ($scope.test.sections[parentIndex - 1] != null && $scope.test.sections[parentIndex - 1].questions[index - 1] != null) {
                var ques = $scope.test.sections[parentIndex - 1].questions[index - 1];
                var userOptionId = !$scope.isNullOrEmptyOrUndefined(ques.userOption) ? parseInt(ques.userOption) : 0;
                var options = ques.options;
                var foundItem = $filter('filter')(options, { id: userOptionId }, true)[0];
                if (!$scope.isNullOrEmptyOrUndefined(foundItem) && foundItem.isRightOption == false) {
                    result = true;
                }
            }
        }
        return result;
    }
    $scope.isCorrect = function (parentIndex, index) {
        var result = false;
        if ($scope.test != "") {
            //debugger;
            if ($scope.test.sections[parentIndex - 1] != null && $scope.test.sections[parentIndex - 1].questions[index - 1] != null) {
                var ques = $scope.test.sections[parentIndex - 1].questions[index - 1];
                var userOptionId = !$scope.isNullOrEmptyOrUndefined(ques.userOption) ? parseInt(ques.userOption) : 0;
                var options = ques.options;
                var foundItem = $filter('filter')(options, { isRightOption: true }, true)[0];
                if (!$scope.isNullOrEmptyOrUndefined(foundItem) && foundItem.isRightOption == true && userOptionId == foundItem.id) {
                    result = true;
                }
            }
        }
        return result;
    }
    $scope.isIncorrectCountBySection = function (index) {
        var count = 0;
        var questions = $scope.test.sections[index].questions;
        angular.forEach(questions, function (ques, index) {
            var options = ques.options;
            var userOptionId = !$scope.isNullOrEmptyOrUndefined(ques.userOption) ? parseInt(ques.userOption) : 0;
            var foundItem = $filter('filter')(options, { id: userOptionId }, true)[0];
            if (!$scope.isNullOrEmptyOrUndefined(foundItem) && foundItem.isRightOption == false) {
                count = count + 1;
            }
        })
        return count;
    }
    $scope.correctCountBySection = function (index) {
        var count = 0;
        var questions = $scope.test.sections[index].questions;
        angular.forEach(questions, function (ques, index) {
            var options = ques.options;
            var userOptionId = !$scope.isNullOrEmptyOrUndefined(ques.userOption) ? parseInt(ques.userOption) : 0;
            var foundItem = $filter('filter')(options, { isRightOption: true }, true)[0];
            if (!$scope.isNullOrEmptyOrUndefined(foundItem) && foundItem.isRightOption == true && userOptionId == foundItem.id) {
                count = count + 1;
            }
        })
        return count;
    }
    $scope.unAttemptedCountBySection = function (index) {
        var count = 0;
        var questions = $scope.test.sections[index].questions;
        angular.forEach(questions, function (ques, index) {
            var options = ques.options;
            var userOptionId = !$scope.isNullOrEmptyOrUndefined(ques.userOption) ? parseInt(ques.userOption) : 0;
            if (userOptionId == 0) {
                count = count + 1;
            }
        })
        return count;
    }
    $scope.totalObtainedMarksBySection = function (index) {
        var count = 0;
        var questions = $scope.test.sections[index].questions;
        angular.forEach(questions, function (ques, index) {
            var options = ques.options;
            var userOptionId = !$scope.isNullOrEmptyOrUndefined(ques.userOption) ? parseInt(ques.userOption) : 0;
            var foundItem = $filter('filter')(options, { isRightOption: true }, true)[0];
            if (!$scope.isNullOrEmptyOrUndefined(foundItem) && foundItem.isRightOption == true && userOptionId == foundItem.id) {
                count = ques.mark + count;
            }
        })
        return count;
    }
    $scope.isUnattempted = function (parentIndex, index) {
        var result = false;
        if ($scope.test != "") {
            if ($scope.test.sections[parentIndex - 1] != null && $scope.test.sections[parentIndex - 1].questions[index - 1] != null) {
                var ques = $scope.test.sections[parentIndex - 1].questions[index - 1];
                var userOptionId = !$scope.isNullOrEmptyOrUndefined(ques.userOption) ? parseInt(ques.userOption) : 0;
                if (userOptionId == 0) {
                    result = true;
                }
            }
        }
        return result;
    }
    function setCurrentQuestion(parentIndex, index) {
        $scope.currentQuestion = $scope.test.sections[parentIndex].questions[index];
        var sectionObj = $scope.test.sections[parentIndex];
        var questionObj = $scope.test.sections[parentIndex].questions[index];

        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, selectedIndex: index }, true)[0];
        if (foundItem) {
           //DO NOTHING
        } else {
            $scope.selectedOptions.push({
                selectedParentIndex: parentIndex,
                selectedIndex: index,
                sectionId: sectionObj.id,
                questionId: questionObj.id,
                isReported: false,
            });
        }
        if ($scope.currentQuestion != null) {
            $scope.questionTitle = $sce.trustAsHtml($scope.currentQuestion.title);
            $scope.questionId = $scope.currentQuestion.id;
            $scope.questionSectionId = $scope.currentQuestion.sectionId;
            $scope.questionCreatedBy = $scope.currentQuestion.createdBy;
            //alert($scope.questionTitle);
        }
    }
    function setCurrentSelection(parentIndex, index) {
        var foundItem = $filter('filter')($scope.selectedOptions, { selectedParentIndex: parentIndex, selectedIndex: index }, true)[0];
        if (foundItem) {
            $scope.selectedOption = foundItem.option;
        }
    }
    $scope.sideBarItemClicked = function (parentIndex, index) {
        $scope.selectedParentIndex = parentIndex;
        $scope.selectedIndex = index;
        setCurrentQuestion($scope.selectedParentIndex-1, $scope.selectedIndex-1);
    }
    $scope.jumpToQuestion = function (parentIndex, index) {
        $scope.selectedParentIndex = parentIndex;
        $scope.selectedIndex = index;
        setCurrentQuestion($scope.selectedParentIndex-1, $scope.selectedIndex-1);
    }
    $scope.getOptions = function () {
        var result = null;
        if ($scope.test != "") {
            if ($scope.test.sections[$scope.selectedParentIndex - 1] != null && $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1] != null) {
                var options = $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1].options;
                result = options;
            }
        }
        return result;
    }
    $rootScope.$watch(function () {
        MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        return true;
    });
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

    

    function getQuestionDesc1 () {
        //debugger;
        //var result = "";
        if ($scope.test != "") {
            if ($scope.test.sections[$scope.selectedParentIndex - 1] != null && $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1] != null) {
                var ques = $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1];
                //result = $sce.trustAsHtml(ques.title);
                $scope.questionTitle = ques.title; //$sce.trustAsHtml(ques.title);
                alert($scope.questionTitle);
            }
        }
        //return result;
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
        if ($scope.currentQuestion.imagePath!=null)
            return true;
        else {
            return false;
        }
    }
    $scope.report = function (reportType, reportText) {
        debugger;
        var ss = $scope.selectedOption;
        $scope.isReporting = true;
        //alert("reportType " + reportType + " reportText " + reportText + " questionId " + $scope.currentQuestion.id)

        //$http({
        //                  url: "/api/smartquizapi/PostQuizError",
        //                  method: "POST",
        //                  params: {
        //                      discription: $scope.newError.Discription,
        //                      questionId: $scope.newError.QuestionId
        //                  }
        //              })
        //              .then(function (result) {
        //                  // Success
        //                  debugger;
        //                  var newError = result.data;
        //                  alert(result.data);
        //                  $scope.newError.Discription = "";
        //                  //$('#question-error').modal('hide');
        //                  $uibModalInstance.dismiss('cancel');

        //              }, function (e) {
        //                  debugger;
        //                  //error
        //                  alert("Cann't save the question error.");
        //              });
        var foundItem = $filter('filter')($scope.selectedOptions, { questionId: $scope.currentQuestion.id }, true)[0];
        if (foundItem) {
            foundItem.isReported = true
        } else {
            $scope.selectedOptions.push({
                selectedParentIndex: parentIndex,
                selectedIndex: index,
                sectionId: sectionObj.id,
                questionId: questionObj.id,
                isReported: true
            });
        }
        $scope.isReported = true;
    }
    $scope.hasReported = function () {
        $scope.isReporting = false;
        var foundItem = $filter('filter')($scope.selectedOptions, { questionId: $scope.currentQuestion.id }, true)[0];
        if (foundItem) {
            return foundItem.isReported;
        }
        else {
            return false;
        }
    }
    $scope.isCorrectOption = function (option) {

        if (option.isRightOption) {
            $scope.rightOption = option.id;
        }
        return (option.isRightOption) ? true : false;
    }
    $scope.isInCorrectOption = function (option) {
        var result = false;
        //debugger;
        var optionId = option.id;
        var ques = $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1];
        var userOptionId = parseInt(ques.userOption);
        if (optionId == userOptionId && option.isRightOption == false) {
            result = true;
        }
        else {
            result = false;
        }

        return (result == true) ? true : false;
    }
    $scope.getSolutionDesc = function () {
        //debugger;
        var result = "";
        if ($scope.test != "") {
            if ($scope.test.sections[$scope.selectedParentIndex - 1] != null && $scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1] != null) {
                result = $sce.trustAsHtml($scope.test.sections[$scope.selectedParentIndex - 1].questions[$scope.selectedIndex - 1].solution);
            }
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
            if ($scope.selectedIndex < $scope.questionCount) {
                if ($scope.selectedIndex < $scope.questionCount) {
                    $scope.selectedIndex = $scope.selectedIndex + 1;
                }
                else if ($scope.selectedIndex == $scope.questionCount) {
                    $scope.selectedIndex = 1;
                    $scope.selectedParentIndex = $scope.selectedParentIndex + 1;
                }
            }
            else if ($scope.selectedParentIndex == $scope.sectionCount) {
                $scope.selectedIndex = $scope.selectedIndex + 1;
            }

        } else {
            if ($scope.selectedParentIndex <= $scope.sectionCount) {
                if ($scope.selectedIndex > 1 && $scope.selectedIndex <= $scope.questionCount) {
                    $scope.selectedIndex = $scope.selectedIndex - 1;
                }
                else if ($scope.selectedIndex == 1) {
                    $scope.selectedParentIndex = $scope.selectedParentIndex - 1;
                    $scope.selectedIndex = $scope.questionCount;
                }
            }
            else if ($scope.selectedParentIndex == 1) {
                $scope.selectedIndex = $scope.selectedIndex - 1;
            }
        }
        setCurrentQuestion($scope.selectedParentIndex-1, $scope.selectedIndex-1);
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

    //Retrive data from database************
    function loadQuiz() {
       // debugger;
        //var def = $q.defer();
        $http.get("/api/SmartQuizApi/GetQuizById", {
            params:
            {
                questionPaperId: $scope.questionPaperId
            }
        }).success(function (data, status, headers, config) {
            debugger;
            // deferred.resolve(data);
            $scope.test = data;
            $scope.questionCount = data.questionCount;
            if ($scope.test != "") {
                debugger;
                $scope.duration = $scope.test.duration;
                setCurrentQuestion($scope.selectedParentIndex - 1, $scope.selectedIndex - 1);
               // setCurrentOptions($scope.selectedParentIndex - 1, $scope.selectedIndex - 1, null);

                $scope.sectionCount = $scope.test.sections.length;
                //if ($scope.selectedParentIndex > 0)
                //    $scope.questionCount = $scope.test.sections[$scope.selectedParentIndex - 1].questions.length;
                //else {
                //    $scope.questionCount = $scope.test.sections[0].questions.length;
                //}
                $scope.questionCount = data.questionCount;
            }
        }).error(function (data, status, headers, config) {
            //debugger;
            //deferred.reject(data);
        });
    };

    //$timeout(function () {
    //    getQuestionDesc1();
    //}, 3500);
   
     $scope.customDialogButtons = {
         warning: {
             label: "Warning!",
             className: "btn-warning"
         },
         success: {
             label: "Success!",
             className: "btn-success"
         },
         danger: {
             label: "Danger!",
             className: "btn-danger"
         },
         main: {
             label: "Click ME!",
             className: "btn-primary"
         }
    };

});