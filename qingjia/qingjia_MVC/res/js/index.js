

// 点击主题仓库
function onThemeSelectClick(event) {
    var windowThemeRoller = F.ui.windowThemeRoller;
    windowThemeRoller.show();
}

// 点击加载动画
function onLoadingSelectClick(event) {
    var windowLoadingSelector = F.ui.windowLoadingSelector;
    windowLoadingSelector.show();
}

// 设置长期存在的Cookie
function setCookie(name, value) {
    F.cookie(name, value, {
        expires: 100  // 单位：天
    });
}

// 点击下一个主题
function onNextThemeClick(event) {
    var themes = [["default", "Default"], ["metro_blue", "Metro Blue"], ["metro_dark_blue", "Metro Dark Blue"],
                   ["metro_gray", "Metro Gray"], ["metro_green", "Metro Green"], ["metro_orange", "Metro Orange"],
                   ["black_tie", "Black Tie"], ["blitzer", "Blitzer"], ["cupertino", "Cupertino"], ["dark_hive", "Dark Hive"],
                   ["dot_luv", "Dot Luv"], ["eggplant", "Eggplant"], ["excite_bike", "Excite Bike"], ["flick", "Flick"],
                   ["hot_sneaks", "Hot Sneaks"], ["humanity", "Humanity"], ["le_frog", "Le Frog"], ["mint_choc", "Mint Choc"],
                   ["overcast", "Overcast"], ["pepper_grinder", "Pepper Grinder"], ["redmond", "Redmond"], ["smoothness", "Smoothness"],
                   ["south_street", "South Street"], ["start", "Start"], ["sunny", "Sunny"], ["swanky_purse", "Swanky Purse"],
                   ["trontastic", "Trontastic"], ["ui_darkness", "UI Darkness"], ["ui_lightness", "UI Lightness"], ["vader", "Vader"],
                   ["custom_default", "Custom Default"], ["bootstrap_pure", "Bootstrap Pure"]];

    var themeName = F.cookie('Theme_Mvc').toLowerCase();
    if (!themeName) {
        themeName = 'default';
    }

    var themeIndex = 0, themeCount = themes.length;
    $.each(themes, function (index, item) {
        if (item[0] === themeName) {
            themeIndex = index;
            return false; // break
        }
    });
    themeIndex++;

    if (themeIndex === themeCount) {
        themeIndex = 0;
    }

    var nextTheme = themes[themeIndex];
    var themeName = nextTheme[0];
    var themeTitle = nextTheme[1];

    setCookie('Theme_Mvc', themeName);
    setCookie('Theme_Mvc_Title', themeTitle);

    top.window.location.reload();
}

// 展开左侧面板
function expandLeftPanel() {
    var leftPanel = F.ui.leftPanel;
    var treeMenu = F.ui.treeMenu;

    if (COOKIE_MENUSTYLE === 'tree' || COOKIE_MENUSTYLE === 'tree_minimode') {
        treeMenu.miniMode = false;
        // 重新加载树菜单
        treeMenu.loadData();

        F.ui.leftPanelToolGear.show();
        F.ui.leftPanelBottomToolbar.show();
        F.ui.leftPanelToolCollapse.setIconFont('chevron-circle-left');

        leftPanel.el.removeClass('minimodeinside');
        leftPanel.setWidth(220);
    } else {
        leftPanel.expand();
    }
}


// 展开左侧面板
function collapseLeftPanel() {
    var leftPanel = F.ui.leftPanel;
    var treeMenu = F.ui.treeMenu;

    if (COOKIE_MENUSTYLE === 'tree' || COOKIE_MENUSTYLE === 'tree_minimode') {
        treeMenu.miniMode = true;
        treeMenu.miniModePopWidth = 300;
        // 重新加载树菜单
        treeMenu.loadData();

        F.ui.leftPanelToolGear.hide();
        F.ui.leftPanelBottomToolbar.hide();
        F.ui.leftPanelToolCollapse.setIconFont('chevron-circle-right');

        leftPanel.el.addClass('minimodeinside');
        leftPanel.setWidth(50);
    } else {
        leftPanel.collapse();
    }
}


// 自定义展开折叠工具图标
function onLeftPanelToolCollapseClick(event) {

    if (COOKIE_MENUSTYLE === 'tree' || COOKIE_MENUSTYLE === 'tree_minimode') {

        if (F.ui.treeMenu.miniMode) {
            expandLeftPanel();
        } else {
            collapseLeftPanel();
        }

    } else {
        F.ui.leftPanel.toggleCollapse();
    }
}

// 点击展开菜单
function onExpandAllClick(event) {
    F.ui.treeMenu.expandAll();
}

// 点击折叠菜单
function onCollapseAllClick(event) {
    F.ui.treeMenu.collapseAll();
}


// 点击仅显示最新示例
function onShowOnlyNewClick(event) {
    var checked = this.isChecked();
    // 改变Cookie的值（不要删除），HomeController中会根据Cookie值是否存在来设置默认值
    setCookie('ShowOnlyBase_Mvc', checked);
    top.window.location.reload();
}


function onSearchTrigger1Click(event) {
    F.removeCookie('SearchText_Mvc');
    top.window.location.reload();
}

function onSearchTrigger2Click(event) {
    setCookie('SearchText_Mvc', this.getValue());
    top.window.location.reload();
}

// 点击标题栏工具图标 - 查看源代码
function onToolSourceCodeClick(event) {
    var mainTabStrip = F.ui.mainTabStrip;
    var windowSourceCode = F.ui.windowSourceCode;


    var activeTab = mainTabStrip.getActiveTab();
    var iframeWnd, iframeUrl;
    if (activeTab.iframe) {
        iframeWnd = activeTab.getIFrameWindow();
        iframeUrl = activeTab.getIFrameUrl();
    }

    var files = [iframeUrl];
    var sourcefilesNode = $(iframeWnd.document).find('head meta[name=sourcefiles]');
    if (sourcefilesNode.length) {
        $.merge(files, sourcefilesNode.attr('content').split(';'));
    }
    windowSourceCode.show(F.baseUrl + 'Home/Source?files=' + encodeURIComponent(files.join(';')));

}

// 点击标题栏工具图标 - 刷新
function onToolRefreshClick(event) {
    var mainTabStrip = F.ui.mainTabStrip;

    var activeTab = mainTabStrip.getActiveTab();
    if (activeTab.iframe) {
        var iframeWnd = activeTab.getIFrameWindow();
        iframeWnd.location.reload();
    }
}

// 点击标题栏工具图标 - 在新标签页中打开
function onToolNewWindowClick(event) {
    var mainTabStrip = F.ui.mainTabStrip;

    var activeTab = mainTabStrip.getActiveTab();
    if (activeTab.iframe) {
        var iframeUrl = activeTab.getIFrameUrl();
        iframeUrl = iframeUrl.replace(/Mobile\/\?file=/ig, '');
        window.open(iframeUrl, '_blank');
    }
}

// 点击标题栏工具图标 - 最大化
function onToolMaximizeClick(event) {
    var topPanel = F.ui.topPanel;
    var leftPanel = F.ui.leftPanel;
    var bottomPanel = F.ui.bottomPanel;

    var currentTool = this;
    if (currentTool.iconFont.indexOf('expand') >= 0) {
        topPanel.collapse();
        bottomPanel.collapse();
        currentTool.setIconFont('compress');

        collapseLeftPanel();
    } else {
        topPanel.expand();
        bottomPanel.expand();
        currentTool.setIconFont('expand');

        expandLeftPanel();
    }
}


// 添加示例标签页（通过href在树中查找）
function addExampleTabByHref(href) {
    var leftPanel = F.ui.leftPanel;
    var firstChild = leftPanel.items[0];

    href = href.toLowerCase();

    // 在树数据中查找href对应的节点id
    function checkInsideTree(tree) {
        var found = false;
        tree.resolveNode(function (node) {
            var resolveHref = node.href;
            if (resolveHref) {
                resolveHref = resolveHref.toLowerCase();
                if (resolveHref.indexOf(href) >= 0) {

                    // 保证传入的id和点击树节点生成的id相同！！！
                    F.addMainTab(F.ui.mainTabStrip, {
                        id: node.id,
                        iframeUrl: node.href,
                        title: node.text,
                        icon: node.icon,
                        iconFont: node.iconFont
                    });

                    found = true;
                    return false; // break
                }
            }
        });
        return found;
    }


    if (firstChild.isType('tree')) {
        // 左侧为树控件
        checkInsideTree(firstChild);
    } else {
        // 左侧为树控件+手风琴控件
        $.each(firstChild.items, function (index, accordionpane) {
            if (checkInsideTree(accordionpane.items[0])) {
                return false; // break
            }
        });
    }
}


// 添加示例标签页
// id： 选项卡ID
// iframeUrl: 选项卡IFrame地址
// title： 选项卡标题
// icon： 选项卡图标
// createToolbar： 创建选项卡前的回调函数（接受tabOptions参数）
// refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame
// iconFont： 选项卡图标字体
function addExampleTab(tabOptions) {

    if (typeof (tabOptions) === 'string') {
        tabOptions = {
            id: arguments[0],
            iframeUrl: arguments[1],
            title: arguments[2],
            icon: arguments[3],
            createToolbar: arguments[4],
            refreshWhenExist: arguments[5],
            iconFont: arguments[6]
        };
    }

    F.addMainTab(F.ui.mainTabStrip, tabOptions);
}


// 移除选中标签页
function removeActiveTab() {
    var mainTabStrip = F.ui.mainTabStrip;

    var activeTab = mainTabStrip.getActiveTab();
    activeTab.hide();
}

// 获取当前激活选项卡的ID
function getActiveTabId() {
    var mainTabStrip = F.ui.mainTabStrip;

    var activeTab = mainTabStrip.getActiveTab();
    if (activeTab) {
        return activeTab.id;
    }
    return '';
}

// 激活选项卡，并刷新其中的内容，示例：表格控件->杂项->在新标签页中打开（关闭后刷新父选项卡）
function activeTabAndRefresh(tabId) {
    var mainTabStrip = F.ui.mainTabStrip;
    var targetTab = mainTabStrip.getTab(tabId);

    if (targetTab) {
        mainTabStrip.activeTab(targetTab);
        targetTab.refreshIFrame();
    }
}

// 激活选项卡，并刷新其中的内容，示例：表格控件->杂项->在新标签页中打开（关闭后更新父选项卡中的表格）
function activeTabAndUpdate(tabId, param1) {
    var mainTabStrip = F.ui.mainTabStrip;
    var targetTab = mainTabStrip.getTab(tabId);

    if (targetTab) {
        mainTabStrip.activeTab(targetTab);
        targetTab.getIFrameWindow().updatePage(param1);
    }
}

// 点击菜单样式
function onMenuStyleCheckChange(event, item, checked) {
    var menuStyle = item.getAttr('data-tag');

    setCookie('MenuStyle_Mvc', menuStyle);
    top.window.location.reload();
}

// 点击显示模式
function onMenuModeCheckChange(event, item, checked) {
    var menuMode = item.getAttr('data-tag');

    setCookie('MenuMode_Mvc', menuMode);
    top.window.location.reload();
}

// 点击语言
function onMenuLangCheckChange(event, item, checked) {
    var lang = item.getAttr('data-tag');

    setCookie('Language_Mvc', lang);
    top.window.location.reload();
}

function onSignOutClick() {
    notify('尚未实现');
}

function onUserProfileClick() {
    notify('尚未实现');
}

// 通知框
function notify(msg) {
    F.notify({
        message: msg,
        messageIcon: 'information',
        target: '_top',
        header: false,
        displayMilliseconds: 3 * 1000,
        positionX: 'center',
        positionY: 'center'
    });
}


F.ready(function () {

    var mainTabStrip = F.ui.mainTabStrip;
    var treeMenu = F.ui.treeMenu;

    if (!treeMenu) return;

    // 初始化主框架中的树(或者Accordion+Tree)和选项卡互动，以及地址栏的更新
    // treeMenu： 主框架中的树控件实例，或者内嵌树控件的手风琴控件实例
    // mainTabStrip： 选项卡实例
    // updateHash: 切换Tab时，是否更新地址栏Hash值（默认值：true）
    // refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame（默认值：false）
    // refreshWhenTabChange: 切换选项卡时，是否刷新内部IFrame（默认值：false）
    // maxTabCount: 最大允许打开的选项卡数量
    // maxTabMessage: 超过最大允许打开选项卡数量时的提示信息
    F.initTreeTabStrip(treeMenu, mainTabStrip, {
        maxTabCount: 10,
        maxTabMessage: '请先关闭一些选项卡（最多允许打开 10 个）！'
    });

    var themeTitle = F.cookie('Theme_Mvc_Title');
    var themeName = F.cookie('Theme_Mvc');
    if (themeTitle) {
        F.removeCookie('Theme_Mvc_Title');
        notify('主题更改为：' + themeTitle + '（' + themeName + '）');
    }

});

