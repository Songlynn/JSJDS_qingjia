/**
 * Created by lynn on 2017/4/5.
 */
$(document).ready(function () {
    var i = 0;
    $('.batch').each(function () {
        i++;
        var batch = $(this);
        var cbs = batch.find('input:checkbox');
        cbs.each(function (index) {
            var cb = $(this);
            if (cb.is(':checked')) {
                if (i != 1) {
                    var one = $('.batch').eq(0).find('input:checkbox').eq(index);
                    one.attr("disabled", "true");
                    one.parent().addClass('disabled');
                    one.next().css("color", "rgb(201,201,201)");
                }
                if (i != 2) {
                    var two = $('.batch').eq(1).find('input:checkbox').eq(index);
                    two.attr("disabled", "true");
                    two.parent().addClass('disabled');
                    two.next().css("color", "rgb(201,201,201)");
                }
                if (i != 3) {
                    var three = $('.batch').eq(2).find('input:checkbox').eq(index);
                    three.attr("disabled", "true");
                    three.parent().addClass('disabled');
                    three.next().css("color", "rgb(201,201,201)");
                }
            }
        });
    });
    
    $('input:checkbox').on("change", function () {
        var cb = $(this);
        var batches = cb.parents('.batch').siblings('.batch');
        var td = cb.parent().parent().index() + 1;
        var tr = cb.parent().parent().parent().index();
        var len = cb.parents('tr').children('td').length;
        var index = td + tr *  len;
        batches.each(function () {
            var batch = $(this);
            var other = batch.find('input:checkbox').eq(index - 1);
            if (cb.is(':checked')) {
                other.attr("disabled", "true");
                other.parent().addClass('disabled');
                other.next().css("color", "rgb(201,201,201)");
            }
            else {
                other.removeAttr("disabled");
                other.parent().removeClass('disabled');
                other.next().css("color", "#3F3F3F");
            }
        });
    });
    $('input[name=submit]').on("click", function (e) {
        var selected = true;
        var len = $('.batch').eq(0).find('input:checkbox').length;
        //检测是否有未选班级
        for (var i = 0; i < len; i++) {
            var one = $('.batch').eq(0).find('input:checkbox').eq(i),
                    two = $('.batch').eq(1).find('input:checkbox').eq(i),
                    three = $('.batch').eq(2).find('input:checkbox').eq(i);
            if ((one.is(':checked')) || (two.is(':checked')) || three.is(':checked')) {
                continue;
            }
            else {
                selected = false;
            }
        }
        if (!selected) {
            notify("有未选班级！",0);
            return false;
        }
        else {
            var one = false,
                two = false,
                three = false;
            var time1, time2, time3;
            var location1, location2, location3;
            $('.batch').eq(0).find('input:checkbox').each(function () {
                var cb = $(this);
                if (cb.is(':checked')) {
                    one = true;
                    return false;
                }
            });
            if (one) {
                time1 = F.ui.tpFirst.getValue();
                location1 = $('#Location1').val();
                if (time1 == "") {
                    //$('#tpFirst input').css("border", "2px solid rgb(238,79,93)");
                    //$('#tpFirst').parents('.form-group').addClass('has-error');
                    notify("请选择第一批点名时间！",0);
                    return false;
                }
                else if (location1 == "") {
                    //$('#Location1').css("border", "2px solid rgb(238,79,93)");
                    //$('#Location1').addClass('input-warning');
                    notify("请输入第一批点名地点！",0);
                    return false;
                }
            }
            $('.batch').eq(1).find('input:checkbox').each(function () {
                var cb = $(this);
                if (cb.is(':checked')) {
                    two = true;
                    return false;
                }
            });
            if (two) {
                time2 = F.ui.tpSecond.getValue();
                location2 = $('#Location2').val();
                if (time1 == "") {
                    //$('#tpSecond input').css("border", "2px solid rgb(238,79,93)");
                    notify("请选择第二批点名时间！",0);
                    return false;
                }
                else if (location2 == "") {
                    //$('#Location2').css("border", "2px solid rgb(238,79,93)");
                    notify("请输入第二批点名地点！",0);
                    return false;
                }
                else if (time2 < time1) {
                    notify("第二批点名时间应晚于第一批",0);
                    return false;
                }
            }
            $('.batch').eq(2).find('input:checkbox').each(function () {
                var cb = $(this);
                if (cb.is(':checked')) {
                    three = true;
                    return false;
                }
            });
           if (three) {
                time3 = F.ui.tpThird.getValue();
                var location3 = $('#Location3').val();
                if (time3 == "") {
                    //$('#tpThird input').css("border", "2px solid rgb(238,79,93)");
                    notify("请选择第三批点名时间！",0);
                    return false;
                }
                else if (location3 == "") {
                    //$('#Location3').css("border", "2px solid rgb(238,79,93)");
                    notify("请输入第三批点名地点！",0);
                    return false;
                }
                else if (time3 < time2) {
                    notify("第三批点名时间应晚于第二批",0);
                    return false;
                }
            }
        }
        e.preventDefault();

        
        var itemArray = [];
        var date = $('#dpDate-inputEl').val();
        var item;

        for (var i = 0; i < len; i++) {
            var one = $('.batch').eq(0).find('input:checkbox').eq(i),
                    two = $('.batch').eq(1).find('input:checkbox').eq(i),
                    three = $('.batch').eq(2).find('input:checkbox').eq(i);
            if (one.is(':checked')) {
                var className = one.val();
                var time = $('#tpFirst-inputEl').val();
                var location = $('#Location1').val();
                item = { "item_batch": "1", "item_name": className, "item_time": date + ' ' + time, "item_location": location };
            }
            if (two.is(':checked')) {
                var className = one.val();
                var time = $('#tpSecond-inputEl').val();
                var location = $('#Location2').val();
                item = { "item_batch": "2", "item_name": className, "item_time": date + ' ' + time, "item_location": location };
            }
            if (three.is(':checked')) {
                var className = one.val();
                var time = $('#tpThird-inputEl').val();
                var location = $('#Location3').val();
                item = { "item_batch": "3", "item_name": className, "item_time": date + ' ' + time, "item_location": location };
            }
            itemArray.push(item);
        }
        //console.log(itemArray);
        //notify('开始传值');
        $.ajax({
            type: "POST",
            url: "/Message/NightMessage/NightSet",
            //dataType: "json",//dataType代表预期返回的值类型，若为json，则必须返回json格式数据 否则不予解析
            contentType: "application/json",
            data: JSON.stringify(itemArray),
            success: function (data) {
                //console.log(data);
                notify(data,1);
            },
            error: function () {

            }
        });
    });
});
