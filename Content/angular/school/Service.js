app.service("angularService", function ($http) {

    //get All Schools
    this.getEmployees = function () {
        return $http.get("/School/GetAll");
    };
    //get Banners by schoolId
    this.getBannersById = function (schoolId) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/GetBannersById",
            params: {
                schoolId: schoolId
            }
        });
        return response;
    };
    //loadBannersById
    this.loadBannersById = function (schoolId) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/LoadBanners",
            params: {
                schoolId: schoolId
            }
        });
        return response;
    };

    //get all News by schoolId
    this.getAllNewsById = function (schoolId) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/GetAllNewsById",
            params: {
                schoolId: schoolId
            }
        });
        return response;
    };
    this.loadNewsById = function (schoolId) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/LoadNewsById",
            params: {
                schoolId: schoolId
            }
        });
        return response;
    };

    // get School By Id
    this.getEmployee = function (employeeID) {
        var response = $http({
            method: "post",
            url: "/School/GetSchoolById",
            params: {
                id: JSON.stringify(employeeID)
            }
        });
        return response;
    }

    // Update School 
    this.updateEmp = function (employee) {
        var response = $http({
            method: "post",
            url: "/School/UpdateSchool",
            data: JSON.stringify(employee),
            dataType: "json"
        });
        return response;
    }

    // Add School
    this.AddEmp = function (employee) {
        var response = $http({
            method: "post",
            url: "/School/AddSchool",
            data: JSON.stringify(employee),
            dataType: "json"
        });
        return response;
    }

    this.AddNews = function (schoolNews) {
        var response = $http({
            method: "post",
            url: "/School/AddNews",
            data: JSON.stringify(schoolNews),
            dataType: "json"
        });
        return response;
    }

    //Delete Employee
    this.DeleteEmp = function (employeeId) {
        var response = $http({
            method: "post",
            url: "/School/DeleteEmployee",
            params: {
                employeeId: JSON.stringify(employeeId)
            }
        });
        return response;
    }

    //==============================
    //Delete Employee

    
    this.DeleteNewsById = function (newsId) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/DeleteNewsById",
            params: {
                newsId: newsId
            }
        });
        return response;
    }
    this.EditNewsById = function (schoolNews) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/EditNewsById",
            data: JSON.stringify(schoolNews),
            dataType: "json"
        });
        return response;
    }

    this.DeleteBannerById = function (bannerId) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/DeleteBannerById",
            params: {
                bannerId: bannerId
            }
        });
        return response;
    }
    this.EditBannerById = function (banner) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/EditBannerById",
            data: JSON.stringify(banner),
            dataType: "json"
        });
        return response;
    }    
  
    this.getState = function () {
        return $http.get("/School/FillState");
    };

    this.getAction = function () {
        return $http.get("/School/FillAction");
    };


    this.getCity = function (stateid) {
        debugger;
        var response = $http({
            method: "post",
            url: "/School/FillCity",
            params: {
                StateId: stateid
            }
        });
        return response;
    }
});