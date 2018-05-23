//将表单格式化成Json
(function ($) {
    $.fn.serializeJson = function () {
        var serializeObj = {};
        var array = this.serializeArray();
        var str = this.serialize();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                if ($.isArray(serializeObj[this.name])) {
                    serializeObj[this.name].push(this.value);
                } else {
                    serializeObj[this.name] = [serializeObj[this.name], this.value];
                }
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        return serializeObj;
    };
})(jQuery);

function Check(regStr, valueStr) {
    var reg = new RegExp(regStr);
    return reg.test(valueStr);
}

function StartValidate(formName) {
    $('#' + formName).validate({
        errorElement: 'span',
        errorClass: 'text text-danger animated bounceIn pull-right small',
        //errorClass: 'label label-danger animated bounceIn pull-right',
        focusInvalid: false,
        highlight: function (element) {
            console.log($(element).hasClass());
            $(element).closest('.form-group').addClass('has-error');
        },
        success: function (label) {
            label.closest('.form-group').removeClass('has-error');
            label.remove();
        },
        errorPlacement: function (error, element) {
            error.insertBefore(element);
        }
    });

}

function GetAlertDiv(msg, alertClass) {
    return $('<div class="alert ' + alertClass + '" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>' + msg + '</div>');
}