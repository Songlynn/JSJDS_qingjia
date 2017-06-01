using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FineUIMvc;
using qingjia_MVC.Controllers;

namespace qingjia_MVC.Areas.Leave.Controllers
{
    public class LeaveFormController : BaseController
    {
        // GET: Leave/LeaveForm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LeaveInfo_T()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Button1_Click()
        {
            ShowNotify("你点击了位于第二个标签中的一个按钮！");

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Button3_Click(int activeIndex)
        {
            int nextIndex = activeIndex + 1;

            if (nextIndex >= 3)
            {
                nextIndex = 0;
            }

            UIHelper.TabStrip("TabStrip1").ActiveTabIndex(nextIndex);

            return UIHelper.Result();
        }
    }
}