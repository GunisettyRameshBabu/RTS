$.validator.addMethod('duplicateemailvalidator', function (value, element, params) {
    alert(value);
    return true;
}, '');

$.validator.unobtrusive.adapters.add('duplicateemailvalidator', function (options) {
    options.rules['duplicateemailvalidator'] = {};
    options.messages['duplicateemailvalidator'] = options.message;
});