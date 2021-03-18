app.controller("myCntrl", function ($scope, angularService, FileUploadService) {
    //$scope.divEmployee = false;
    //GetAllEmployee();    
    $scope.IsStaffAdmin = false;
    $scope.init = function (isStaffAdmin) {
        if (isStaffAdmin) {
            $scope.IsStaffAdmin = isStaffAdmin;
            GetAllEmployee();
        }
    };

    $scope.Message = "";
    $scope.FileInvalidMessage = "";
    $scope.SelectedFileForUpload = null;
    $scope.FileDescription = "";
    $scope.IsFormSubmitted = false;
    $scope.IsFileValid = false;
    $scope.IsFormValid = false;

    //File Validation
    $scope.ChechFileValid = function (file) {
        var isValid = false;
        if ($scope.Logo == null && $scope.SelectedFileForUpload != null) {
            if ($scope.SelectedFileForUpload != null) {
                if ((file.type == 'image/png' || file.type == 'image/jpeg' || file.type == 'image/gif') && file.size <= (512 * 1024)) {
                    $scope.FileInvalidMessage = "";
                    isValid = true;
                }
                else {
                    $scope.FileInvalidMessage = "Selected file is Invalid. (only file type png, jpeg and gif and 512 kb size allowed)";
                }
            }
            else {
                $scope.FileInvalidMessage = "Image required!";
            }

            $scope.IsFileValid = isValid;
        }
    };

    //File Select event 
    $scope.selectFileforUpload = function (file) {
        $scope.SelectedFileForUpload = file[0];
    }

    //==================================== SCHOOL SECTION===============================
    $scope.ValidateBannerFile = function (files) {
        var isValid = false;
        debugger;

        if (files != null && files.length > 0) {
            var fileObj = null;
            for (var i = 0; i < files.length; i++) {
                fileObj = files[i];
                if ((fileObj.type == 'image/png' || fileObj.type == 'image/jpeg' || fileObj.type == 'image/gif') && fileObj.size <= (512 * 1024)) {
                    $scope.FileInvalidMessage = "";
                    isValid = true;
                }
                else {
                    $scope.FileInvalidMessage = fileObj.name + ": Selected file is Invalid. (only file type png, jpeg and gif and 512 kb size allowed)";
                }
            }
        }
        else {
            $scope.FileInvalidMessage = "Image required!";
        }
        $scope.IsFileValid = isValid;
    };
    $scope.bannerUploadfiles = [];


    $scope.bannerFiles = function (files) {
        $scope.bannerUploadfiles = files;
    };

    $scope.LoadFileData = function () {
        $scope.ValidateBannerFile($scope.bannerUploadfiles);
        if ($scope.formBanner.$valid && $scope.IsFileValid) {
            var schoolId = document.getElementById("school").value;
            FileUploadService.UploadBanners($scope.bannerUploadfiles, schoolId).then(function (d) {
                if (d.Status && d.Message == "Done") {
                    alert("Banners has been added successfully. ");
                    $("#bannerFile").val("");
                    $('#school').find('option:first').attr('selected', 'selected');
                    $scope.submitted = false;
                    // $scope.avatarImage = "";
                    GetBannersById(schoolId);
                }
            });
        }
    };

    $scope.editEmployee = function (employee) {
        var getData = angularService.getEmployee(employee.Id);
        getData.then(function (emp) {
            $scope.employee = emp.data;
            $scope.employeeId = employee.Id;
            $scope.employeeName = employee.Name;
            $scope.Logo = employee.Logo;
            $scope.employeeDescription = employee.Description;
            $scope.employeeUrl = employee.Url;
            $scope.employeeTemplateName = employee.TemplateName;
            $scope.employeeAddress = employee.Address;
            $scope.employeeContactName = employee.ContactName;
            $scope.employeeContactPhone = employee.ContactPhone;
            $scope.employeeState = employee.State;
            $scope.employeeAction = employee.Action;
            $scope.Code = employee.SchoolCode;
            $scope.StateId = { "id": employee.State }
            getcitys(employee.State);
            $scope.employeeCity = employee.City;
            $scope.employeeStatus = employee.Status;
            $scope.Action = "Update";
            $scope.divEmployee = true;
            //$scope.divEmpList = false;
        }, function () {
            alert('Error in getting records');
        });
    }

    $scope.AddUpdateEmployee = function () {
        var Employee = {
            Name: $scope.employeeName,
            //Logo: $scope.employeeLogo,
            Description: $scope.employeeDescription,
            Url: $scope.employeeUrl,
            TemplateName: $scope.employeeTemplateName,
            Address: $scope.employeeAddress,
            ContactName: $scope.employeeContactName,
            ContactPhone: $scope.employeeContactPhone,
            City: $scope.employeeCity,
            State: $scope.employeeState,
            Status: $scope.employeeStatus,
            Action: $scope.employeeAction
        };

        $scope.ChechFileValid($scope.SelectedFileForUpload);

        if ($scope.formAdd.$valid && ($scope.IsFileValid || $scope.Logo != null)) {

            if ($scope.SelectedFileForUpload != null) {
                FileUploadService.UploadFile($scope.SelectedFileForUpload, $scope.employeeId).then(function (d) {

                    if (!d.Status && $scope.Logo == null) {
                        alert(d.Message);

                    }
                    else {
                        Employee.Logo = d.FileName;
                        Employee.Code = d.SchoolCode;
                        SaveUpdateRecord(Employee)
                    }
                });
            }
            else {
                Employee.Logo = $scope.Logo;
                SaveUpdateRecord(Employee)
            }
        }

        else {
            $scope.Message = "All the fields are required.";
            alert($scope.Message);
        }


    }

    function SaveUpdateRecord(Employee) {
        debugger;
        var getAction = $scope.Action;
        if (getAction == "Update") {
            Employee.Id = $scope.employeeId;
            var getData = angularService.updateEmp(Employee);
            getData.then(function (msg) {
                GetAllEmployee();
                $scope.divEmployee = false;
                $scope.divEmpList = true;
                alert(msg.data);
                ClearFields();
            }, function () {
                alert('Error is Updating');
            });
        } else {
            var getData = angularService.AddEmp(Employee);
            getData.then(function (msg) {
                GetAllEmployee();
                $scope.divEmployee = false;
                alert("School has been added successfully. ACADAStore staff will review & APPROVED the school to go live.");
                ClearFields();
            }, function () {
                alert('Error is Adding');
            });
        }
        $scope.Logo = "";
        $scope.SelectedFileForUpload = "";

    }

    $scope.AddEmployeeDiv = function () {
        //$scope.divEmpList = false;
        $scope.submitted = false;
        ClearFields();
        $scope.Action = "Add";
        $scope.divEmployee = true;
    }

    $scope.deleteEmployee = function (employee) {
        debugger;
        var getData = angularService.DeleteEmp(employee.Id);
        getData.then(function (msg) {
            GetAllEmployee();
            alert(msg.data);
        }, function () {
            alert('Error is Deleteing');
        });
    }

    //To Get All Records  
    function GetAllEmployee() {
        $scope.divEmpList = true;
        var getData = angularService.getEmployees();
        getData.then(function (emp) {
            $scope.employees = emp.data;
        }, function () {
            alert('Error is getting Employee records');
        });
    }
    //==========================================================
    $scope.deleteBanner = function (banner) {
        debugger;
        var getData = angularService.DeleteBannerById(banner.Id);
        getData.then(function (msg) {
            GetBannersById(banner.SchoolId);
            alert(msg.data);
        }, function () {
            alert('Error is Deleteing');
        });
    }

    $scope.editBanner = function (banner) {
        debugger;
        var getData = angularService.EditBannerById(banner);
        getData.then(function (msg) {
            GetBannersById(banner.SchoolId);
            alert(msg.data);
        }, function () {
            alert('Error is Editing');
        });
    }

    $scope.GetSelectedBanners = function () {
        GetBannersById(document.getElementById("school").value)
    }

    $scope.LoadBanners = function (schoolId) {
        debugger;
        var getData = angularService.loadBannersById(schoolId);
        getData.then(function (emp) {
            $scope.banners = emp.data;

            var schoolObj = angularService.getEmployee(schoolId);
            schoolObj.then(function (emp) {
                $scope.employeeDesc = emp.data.Description;
                $scope.employeeName = emp.data.Name;
                $scope.employeeUrl = emp.data.Url;
                $scope.employeeAdd = emp.data.Address;
                $scope.employeeContactName = emp.data.ContactName;
                $scope.employeeContactPhone = emp.data.ContactPhone;
                $scope.employeeState = emp.data.State;
                $scope.employeeCity = emp.data.City;
            }, function () {
                alert('Error in getting records');
            });
            LoadNewsById(schoolId);

        }, function (e) {
            debugger;
            alert(e.data);
            //alert('Error is getting Banner records');
        });
    }


    function GetBannersById(schoolId) {
        debugger;
        var getData = angularService.getBannersById(schoolId);
        getData.then(function (emp) {
            $scope.banners = emp.data;
        }, function (e) {
            debugger;
            alert(e.data);
            //alert('Error is getting Banner records');
        });
    }
    //=============================NEWS SECTION ======================//
    $scope.AddNews = function () {
        debugger;
        if ($scope.formNews.$valid) {
            var schoolId = document.getElementById("schoolNews").value;
            var schoolNews = {
                SchoolId: schoolId,
                Description: $scope.newsDescription
            };
            var getData = angularService.AddNews(schoolNews);
            getData.then(function (msg) {
                $scope.submitted = false;
                GetAllNewsById();
                $('#schoolNews').find('option:first').attr('selected', 'selected');
                $scope.newsDescription = "";
            }, function () {
                alert('Error is Updating');
            });
        }
        else {
            // Form is not valid
            $scope.Message = "All the fields are required.";
            alert($scope.Message);
        }
    };

    $scope.editNews = function (schoolNews) {
        var getData = angularService.EditNewsById(schoolNews);
        getData.then(function (msg) {
            GetAllNewsById();           
        }, function () {
            alert('Error is Editing');
        });
    }

    $scope.GetSelectedNews = function () {
        GetAllNewsById();
    }

    function GetAllNewsById() {
        debugger;
        var getData = angularService.getAllNewsById(document.getElementById("schoolNews").value);
        getData.then(function (news) {
            $scope.schoolNewsData = news.data;
        }, function (e) {
            debugger;
            alert(e.data);
        });
    };

    $scope.loadNews = function (schoolId) {
        debugger;
        LoadNewsById(schoolId);
    }

    function LoadNewsById(schoolId) {
        var getData = angularService.loadNewsById(schoolId);
        getData.then(function (news) {
            $scope.schoolNewsData = news.data;
        }, function (e) {
            debugger;
            alert(e.data);
            //alert('Error is getting Banner records');
        });
    }

    $scope.deleteNews = function (schoolNews) {
        debugger;
        var getData = angularService.DeleteNewsById(schoolNews.Id);
        getData.then(function (msg) {
            GetAllNewsById();
            alert(msg.data);
        }, function () {
            alert('Error is Deleteing');
        });
    }
    //===================== END NEWS SECTION HERE========================//

    $scope.GetSchools = function () {
        debugger;
        var getData = angularService.getEmployees();
        getData.then(function (schools) {
            $scope.schools = schools.data;
        }, function () {
            alert('Error is getting Employee records');
        });
    }

    function ClearFields() {
        $scope.submitted = false;
        $scope.employeeId = "";
        $scope.employeeName = "";
        $scope.employeeLogo = "";
        $scope.Logo = "";
        $scope.employeeDescription = "";
        $scope.employeeUrl = "";
        $scope.employeeTemplateName = "";
        $scope.employeeAddress = "";
        $scope.employeeContactName = "";
        $scope.employeeContactPhone = "";
        $scope.employeeCity = "";
        $scope.employeeState = "";
        $scope.employeeStatus = "";
        $scope.employeeAction = "";
        $scope.fileUpload = "";

        //var control = $("#schoolFile");
        //control.replaceWith(control = control.clone(true));

        $("#schoolFile").val("");
        //angular.forEach(angular.element("input[type='file']"), function (inputElem) {
        //    angular.element(inputElem).replaceWith(angular.element("input[type='file']").clone(true));
        //});
    }

    $scope.CancelEmployee = function () {
        //$scope.divEmpList = true;
        ClearFields();
        //$scope.divEmployee = false;

    }

    $scope.FillState = function () {
        var getData = angularService.getState();
        debugger;
        getData.then(function (emp) {
            $scope.emp = emp.data;
        }, function () {
            alert('Error in getting State record');
        });
    }

    $scope.GetSelectedState = function () {
        var getData = angularService.getCity(document.getElementById("state").value);
        getData.then(function (emp) {
            $scope.citys = emp.data;
        }, function () {
            alert('Error in getting City record');
        });
    }

    function getcitys(stateid) {
        var getData = angularService.getCity(stateid);
        getData.then(function (emp) {
            $scope.citys = emp.data;
        }, function () {
            alert('Error in getting City record');
        });
    }

    $scope.FillAction = function () {
        var getdata = angularService.getAction();
        getdata.then(function (emp) {
            $scope.actions = emp.data;
        },
            function () {
                alert('Error in getting Actions record');
            });
    }

});

// DATE FORMAT e.g. 05/11/2015
app.filter('dateFormat1', function ($filter) {
    debugger;
    return function (value) {
        if (value == null) { return ""; }

        if (value != null && value.length > 5) {
            var date = new Date(parseInt(value.substr(6)));            
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            return day + '/' + month + '/' + year;
        } else {
            return value;
        }

    };
});

app.filter('dateFormat2', function ($filter) {
    debugger;
    return function (value) {
        if (value == null) { return ""; }

        if (value != null && value.length > 5) {
            var DateStr = value.replace('\/Date(', '');
            DateStr = DateStr.replace(')\/', '');
            return $filter('date')(DateStr, "dd/MM/yyyy 'at' h:mma");
        } else {
            return value;
        }

    };
});

app.filter('strLimit', function ($filter) {
    debugger;
    return function (input, limit) {
        if (!input) return;
        if (input.length <= limit) {
            return input;
        }

        return $filter('limitTo')(input, limit) + '...';
    };
});

app.filter('tel', function () {
    return function (tel) {
        console.log(tel);
        if (!tel) { return ''; }

        var value = tel.toString().trim().replace(/^\+/, '');

        if (value.match(/[^0-9]/)) {
            return tel;
        }

        var country, city, number;

        switch (value.length) {
            case 1:
            case 2:
            case 3:
                city = value;
                break;

            default:
                city = value.slice(0, 3);
                number = value.slice(3);
        }

        if (number) {
            if (number.length > 3) {
                number = number.slice(0, 3) + '-' + number.slice(3, 7);
            }
            else {
                number = number;
            }

            return ("(" + city + ") " + number).trim();
        }
        else {
            return "(" + city;
        }

    };
});


app.factory('FileUploadService', function ($http, $q) { // explained abour controller and service in part 2

    var fac = {};
    fac.UploadFile = function (file, employeeId) {
        var formData = new FormData();

        formData.append("file", file);

        //We can send more data to server using append 
        employeeId = employeeId == 'undefined' ? null : employeeId;
        formData.append("employeeId", employeeId);

        var defer = $q.defer();
        $http.post("/School/SaveFiles", formData,
            {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            })
        .success(function (d) {
            defer.resolve(d);
        })
        .error(function () {
            defer.reject("File Upload Failed!");
        });

        return defer.promise;

    }

    fac.UploadBanners = function (file, employeeId) {
        debugger;
        var formData = new FormData();
        for (var i = 0; i < file.length; i++) {
            formData.append("file[" + i + "]", file[i]);
        }
        //We can send more data to server using append 
        employeeId = employeeId == 'undefined' ? null : employeeId;
        formData.append("employeeId", employeeId);
        var defer = $q.defer();
        $http.post("/School/UploadBanners", formData,
            {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            })
        .success(function (d) {
            defer.resolve(d);
        })
        .error(function () {
            defer.reject("File Upload Failed!");
        });

        return defer.promise;

    }
    return fac;

});

app.controller("mystate", function ($scope, angularService) {
    FillState();

    function FillState() {
        var getData = angularService.getState();
        debugger;
        getData.then(function (emp) {
            $scope.emp = emp.data;
        }, function () {
            alert('Error in getting records');
        });
    }

    $scope.GetSelectedState = function () {
        var getData = angularService.getCity(document.getElementById("state").value);
        getData.then(function (emp) {
            debugger;
            $scope.citys = emp.data;
        }, function () {
            alert('Error in getting records');
        });
    }
});

