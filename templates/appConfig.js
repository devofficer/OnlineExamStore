var app = angular.module("mytest", ["ngJaxBind"]);
app.controller("displayQuestionController", function ($scope, $http) {
    debugger;
    
})
app.directive("mathjaxBind", function () {
    return {
        restrict: "A",
        controller: ["$scope", "$element", "$attrs",
            function ($scope, $element, $attrs) {
                debugger;
                $scope.$watch($attrs.mathjaxBind, function (texExpression) {
                    $element.html(texExpression);
                    MathJax.Hub.Queue(["Typeset", MathJax.Hub, $element[0]]);
                });
            }]
    };
});