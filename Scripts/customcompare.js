
$.validator.addMethod("genericcompare", function (value, element, params) {
    // debugger;
    var propelename = params.split(",")[0];
    var operName = params.split(",")[1];
    if (params == undefined || params == null || params.length == 0 || value == undefined || value == null || value.length == 0 || propelename == undefined || propelename == null || propelename.length == 0 || operName == undefined || operName == null || operName.length == 0)
        return true;
    var valueOther = $(propelename).val();
    var val1 = (isNaN(value) ? Date.parse(value) : eval(value));
    var val2 = (isNaN(valueOther) ? Date.parse(valueOther) : eval(valueOther));

    if (operName == "GreaterThan")
        return val1 > val2;
    if (operName == "LessThan")
        return val1 < val2;
    if (operName == "GreaterThanOrEqual")
        return val1 >= val2;
    if (operName == "LessThanOrEqual")
        return val1 <= val2;
});

$.validator.unobtrusive.adapters.add("genericcompare", ["comparetopropertyname", "operatorname"], function (options) {
    options.rules["genericcompare"] = "#" + options.params.comparetopropertyname + "," + options.params.operatorname;
    options.messages["genericcompare"] = options.message;
});

$.validator.addMethod("futuredate", function (value, element, params) {
    debugger;
    return Date.parse(value) > Date.today();
});

$.validator.unobtrusive.adapters.add("futuredate", [], function (options) {
    options.messages["futuredate"] = options.message;
});

$.validator.addMethod("pastdate", function (value, element, params) {
    debugger;
    return Date.parse(value) < Date.today();
});

$.validator.unobtrusive.adapters.add("pastdate", [], function (options) {
    options.messages["pastdate"] = options.message;
});

$.validator.addMethod('requiredif', function (value, element, parameters) {
   // debugger;
    var id = '#' + parameters['dependentproperty'];
    // get the target value (as a string, 
    // as that's what actual value will be)
    var targetvalue = parameters['targetvalue'];
    targetvalue = (targetvalue == null ? '' : targetvalue).toString();
    // get the actual value of the target control
    // note - this probably needs to cater for more 
    // control types, e.g. radios
    var control = $(id);
    var controltype = control.attr('type');
    var actualvalue =
        controltype === 'checkbox' ?
        control.attr('checked').toString() :
        control.val();

    switch (actualvalue) {
        case '4':
            actualvalue = 'Other';
        case '5':
            actualvalue = 'Unemployed';

        default:
    }

    // if the condition is true, reuse the existing 
    // required field validator functionality
    if ($.trim(targetvalue) === $.trim(actualvalue) || ($.trim(targetvalue) === '*' && $.trim(actualvalue) !== ''))
        return $.validator.methods.required.call(this, value, element, parameters);

    return true;
});

$.validator.unobtrusive.adapters.add('requiredif', ['dependentproperty', 'targetvalue'], function (options) {
    options.rules['requiredif'] = {
        dependentproperty: options.params['dependentproperty'],
        targetvalue: options.params['targetvalue']
    };
    options.messages['requiredif'] = options.message;
});

