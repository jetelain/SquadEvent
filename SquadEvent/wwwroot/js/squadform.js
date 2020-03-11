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

function updateRole() {
    $('select.role').each(function () {
        var self = $(this);

        self.find('option[value!="0"]').removeAttr('disabled');

        $('select.role').not(this).each(function () {
            var val = $(this).val();
            if (val != '' && (Number(val) % 2) == 0) {
                self.find('option[value="' + val + '"]').attr('disabled', 'disabled');
            }
        });
    });
}

$(function () {
    $('select.role').each(function () {
        var fields = $(this).parent().parent().find('select.kit, input:text, select.userid');
        fields.prop('disabled', $(this).val() == '');
        $(this).on('change', function () { fields.prop('disabled', $(this).val() == ''); });
    });
    $('select.role:enabled option[value="0"]').prop('disabled', 'true');

    updateUserId();
    $('select.userid').on('change', updateUserId);

    updateRole();
    $('select.role').on('change', updateRole);
});
