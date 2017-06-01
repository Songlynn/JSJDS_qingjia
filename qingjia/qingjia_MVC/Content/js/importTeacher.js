var dataSize;     //数据总数
var pageSize;   //每页数据量
var pageCount;    //总页数
var curPage;      //当前页数
var tab;

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
    $('#content').on("click", ".table>tbody>tr", function () {
        var detail = $(this);
        var checkbox = detail.find('input:checkbox');
        if (checkbox.is(':checked')) {
            checkbox.removeAttr('checked');
        }
        else {
            checkbox.prop("checked", true);
        }
    });
    $("#content").on("click", ".table>tbody>tr input[name='delete']", function () {
        //获取行的请假单号
        var cb = $(this).parent().prev().find('input:checkbox');
        var rowid = cb.val();
        alert(rowid);
        //F.ui.cancelWindow.show();
    });

    $('#content ').on("click", ".table>tbody>tr input[name='audit']", function (e) {
        //获取行的请假单号
        var cb = $(this).parent().prev().prev().find('input:checkbox');
        var rowid = cb.val();
        alert(rowid);
        //AgreeClick(rowid);
    });
    $("#content").on("click", "table>tbody>tr>td>input", function (e) {
        e.stopPropagation();
    });
});

//function initial(topage) {
//    toPage(topage);
//    $('#content').on("click", '.tab-pane.fade.active ul.pagination>li:nth-child(1) a', function () {
//        toPage(1);
//    });
//    $('#content').on("click", '.tab-pane.fade.active ul.pagination>li:nth-child(2) a', function () {
//        if (curPage > 1) {
//            toPage(curPage - 1);
//        }
//    });
//    $('#content').on("click", '.tab-pane.fade.active ul.pagination>li:nth-last-child(1) a', function () {
//        toPage(pageCount);
//    });
//    $('#content').on("click", '.tab-pane.fade.active ul.pagination>li:nth-last-child(2) a', function (e) {
//        if (curPage < pageCount) {
//            toPage(curPage + 1);
//            //alert(curPage + 1);
//        }
//    });
//}


//function Pages(tr, size, cur) {
//    var trs = tr;                                             //行集合
//    dataSize = trs.length;                              //数据总数
//    pageSize = size;                                      //每页数据量
//    pageCount = Math.ceil(dataSize / pageSize);             //总页数
//    curPage = parseInt(cur);                                        //当前页数
//    hideAll(trs);
//    //toPage(pageSize,pageCount,curPage,1);
//    initial(1);
//}
//function hideAll(trs) {
//    trs.each(function () {
//        $(this).hide();
//    });
//}
//function createPage(before, text) {
//    var li;
//    if (text == curPage) {
//        li = $("<li class='pages active'><a onclick='toPage(" + text + ")'>" + text + "</a></li>");
//    }
//    else {
//        li = $("<li class='pages'><a onclick='toPage(" + text + ")'>" + text + "</a></li>");
//    }
//    $(before).before(li);
//}
//function setPages() {
//    var before = tab.find('ul.pagination>li:nth-last-child(3)');
//    tab.find('ul.pagination>li').remove(".pages");
//    if (pageCount <= 5) {
//        for (var i = 1; i <= pageCount; i++) {
//            createPage(before, i);
//        }
//    }
//    else {
//        if (curPage <= 3) {
//            for (var i = 1; i <= 5; i++) {
//                createPage(before, i);
//            }
//        }
//        else if (curPage >= pageCount - 2) {
//            for (var i = 0; i < 5; i++) {
//                createPage(before, pageCount - 4 + i);
//            }
//        }
//        else {
//            for (var i = 0; i < 5; i++) {
//                //alert(curPage);
//                createPage(before, curPage - 2 + i);
//            }
//        }
//    }

//}
////更多的隐藏与显示
//function checkMore() {
//    var premore = tab.find('ul.pagination>li:nth-child(3)'),
//        nextmore = tab.find('ul.pagination>li:nth-last-child(3)');
//    if (pageCount <= 5) {
//        premore.hide();
//        nextmore.hide();
//    }
//    else {
//        if (curPage > 3) {
//            premore.show();
//        }
//        else {
//            premore.hide();
//        }
//        if ((curPage < pageCount - 2) && (pageCount > 5)) {
//            nextmore.show();
//        }
//        else {
//            nextmore.hide();
//        }
//    }
//}

////隐藏当前页的tr，显示目标页的tr
//function toPage(topage) {
//    var curStart = (curPage - 1) * pageSize + 1,
//        curEnd = curPage * pageSize;
//    var toStart = (topage - 1) * pageSize + 1,
//        toEnd = topage * pageSize;
//    for (var i = curStart; i <= curEnd; i++) {
//        tab.find('.table>tbody>tr').eq(i - 1).hide();
//    }
//    for (var i = toStart; i <= toEnd; i++) {
//        tab.find('.table>tbody>tr').eq(i - 1).show();
//    }
//    curPage = topage;
//    setPages();
//    checkMore();
//}
