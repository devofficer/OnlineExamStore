app.factory('Utils', function () {
    //debugger;
    var service = {
        durationInSecond: '',
        duration:0,
        setDurationInSecond: function (name) {
            // this is the trick to sync the data
            // so no need for a $watch function
            // call this from anywhere when you need to update durationInSecond
            angular.copy(name, service.durationInSecond);
        },
        setDuration: function (duration) {
            angular.copy(duration, service.duration);
        },
        isUndefinedOrNull: function (obj) {
            return !angular.isDefined(obj) || obj === null;
        }
    };

    function msToTime(ms) {
        var seconds = (ms / 1000);
        var minutes = parseInt(seconds / 60, 10);
        seconds = seconds % 60;
        var hours = parseInt(minutes / 60, 10);
        minutes = minutes % 60;
        return hours + ':' + minutes + ':' + seconds;
    }
    return service;
});