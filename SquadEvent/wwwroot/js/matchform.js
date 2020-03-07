function syncMap(select) {
    var num = Number(select.attr('data-index'));
    var value = select.val();

    var a = $('select[name="' + $.escapeSelector('Match.Rounds[' + num + '].Sides[0].Faction')+'"]');
    var b = $('select[name="' + $.escapeSelector('Match.Rounds[' + num + '].Sides[1].Faction') + '"]');

    var map = mapsData[value];
    if (!map) {
        a.prop('disabled', false);
        b.prop('disabled', false);
        a.val('');
        b.val('');
    } else {
        a.prop('disabled', true);
        b.prop('disabled', true);
        if (num % 2 == 1) {
            a.val(map.right);
            b.val(map.left);
        }
        else {
            a.val(map.left);
            b.val(map.right);
        }
    }
    a.change();
    b.change();
}
var factionsImages = {
    '': '/img/factions/none.png',
    '0': '/img/factions/US.png',
    '1': '/img/factions/UK.png',
    '2': '/img/factions/RU.png',
    '3': '/img/factions/Ins.png',
    '4': '/img/factions/Irreg.png',
    '5': '/img/factions/CA.png'
};

$(function () {
    $('.map-input').each(function () { syncMap($(this)); })
    $('.map-input').change(function () { syncMap($(this)); })
    $('select.faction').each(function () {
        var img = $(this).parent().find('img');
        img.attr('src', factionsImages[$(this).val()]);
        $(this).on('change', function () { img.attr('src', factionsImages[$(this).val()]); });
    });
});
