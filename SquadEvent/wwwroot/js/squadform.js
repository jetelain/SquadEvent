function updateUserId() {
    $('select.userid').each(function () {
        var self = $(this);

        self.find('option').show();

        $('select.userid').not(this).each(function () {
            var val = $(this).val();
            if (val != '') {
                self.find('option[value="' + val + '"]').hide();
            }
        });
    });
}


$(function () {
    $('select.role').each(function () {
        var fields = $(this).parent().parent().find('select.kit, input:text');
        fields.prop('disabled', $(this).val() == '');
        $(this).on('change', function () { fields.prop('disabled', $(this).val() == ''); });
    });
    $('select.role:enabled option[value="0"]').prop('disabled', 'true');

    updateUserId();

    $('select.userid').on('change', updateUserId);
});
