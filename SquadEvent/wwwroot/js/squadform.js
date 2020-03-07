
$(function () {
    $('select.role').each(function () {
        var fields = $(this).parent().parent().find('select.kit, input:text');
        fields.prop('disabled', $(this).val() == '');
        $(this).on('change', function () { fields.prop('disabled', $(this).val() == ''); });
    });
    $('select.role:enabled option[value="0"]').prop('disabled', 'true');
});
