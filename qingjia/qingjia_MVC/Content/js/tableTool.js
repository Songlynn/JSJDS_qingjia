/**
 * Created by lynn on 2017/3/27.
 */
$(document).ready(function () {
    $('#content').on("click", ".table>thead>tr input:checkbox", function () {
        var all_input = $('.table>tbody').find('input:checkbox');
        if ($(this).is(':checked')) {
            all_input.prop("checked", true);
        }
        else {
            all_input.removeAttr('checked');
        }
    });
    $("#content").on("click", ".table>tbody>tr input[name='back']", function () {
        //获取行的请假单号
        var cb = $(this).parent().parent().find('input:checkbox');
        var rowid = cb.val();
        //alert(rowid);
        //F.ui.cancelWindow.show();
        BackClick(rowid);
    });

    $('#content ').on("click", ".table>tbody>tr input[name='agree']", function (e) {
        //获取行的请假单号
        var cb = $(this).parent().parent().find('input:checkbox');
        var rowid = cb.val();
        //alert(rowid);
        AgreeClick(rowid);
    });
    $("#content").on("click", "table>tbody>tr>td>input", function (e) {
        e.stopPropagation();
    });
});