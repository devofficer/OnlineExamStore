//display-question.js   
var app = angular.module("mytest", []);

app.controller("displayQuestionController", function ($scope, $http) {
    debugger;
    $scope.isBusy = true;
    $scope.newError = {
        QuestionId: "",
        UserId: "",
        Discription: ""
    };
    $scope.save = function () {
        debugger;
        if ($scope.questionErrorForm.$invalid == true && $scope.questionErrorForm.$dirty == true)
            return false;
        else {
            $http(
           {
               url: "/MyTest/SaveQuestionError",
               method: "POST",
               params: {
                   discription: $scope.newError.Discription,
                   questionId: $scope.newError.QuestionId,
                   userId: $scope.newError.UserId
               }
           })
           .then(function (result) {
               // Success
               debugger;
               var newError = result.data;
               alert(result.data);
               $scope.newError.Discription = "";
               $('#question-error').modal('hide');

           }, function (e) {
               debugger;
               //error
               alert("Cann't save the question error.");
           })
           .then(function () {
               debugger;
               $scope.isBusy = false;
           })
        }
    }
});