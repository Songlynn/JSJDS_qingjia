using FineUIMvc;
using qingjia_MVC.Controllers;
using qingjia_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace qingjia_MVC.Areas.Message.Controllers
{
    public class DeleteMessageController : BaseController
    {
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();
        private int type_short = 4;
        private int type_long = 5;
        private int type_vacation = 6;

        // GET: Message/DeleteMessage
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 加载表格
        /// </summary>
        /// <returns></returns>
        public ActionResult _NameContent()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();

            return PartialView("_NameContent", LoadNameData(UserID));
        }

        /// <summary>
        /// 数据库读取数据
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        protected List<vw_LeaveList> LoadNameData(string UserID)
        {
            var list = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.StateLeave == "1" && vw_LeaveList.StateBack == "0") orderby vw_LeaveList.SubmitTime descending orderby vw_LeaveList.LeaveType select vw_LeaveList;
            return list.ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnSearch_Click(FormCollection form)
        {
            string teacher_name = db.T_Teacher.Find(Session["UserID"].ToString()).Name;
            DateTime today = DateTime.Today;
            #region 短期请假
            /*
            var l_short = (from l in db.vw_LeaveList
                           where l.TypeID == type_short && l.StateLeave == "1" && l.StateBack == "0" && l.ST_Teacher == teacher_name
                           select l.ST_Name)
                        .Distinct();
            */
            var l_short = (from l in db.vw_LeaveList
                           where l.TypeID == type_short && l.StateLeave == "1" && l.StateBack == "0" && l.ST_Teacher == teacher_name && l.TimeBack.ToString().StartsWith(today.ToString())
                           select l.ST_Name)
                        .Distinct();
            StringBuilder short_name = new StringBuilder();
            if (l_short.Count() > 0)
            {
                foreach (var item in l_short)
                {
                    short_name.Append(item.ToString());
                    short_name.Append("；");
                }
            }
            else
            {
                short_name.Append("无");
            }
            UIHelper.Label("lblShort").Text(short_name.ToString());
            #endregion

            #region 长期请假
            var l_long = (from l in db.vw_LeaveList
                          where l.TypeID == type_long && l.StateLeave == "1" && l.StateBack == "0" && l.ST_Teacher == teacher_name && l.TimeBack.ToString().StartsWith(today.ToString())
                          select l.ST_Name)
                          .Distinct();
            StringBuilder long_name = new StringBuilder();
            if (l_long.Count() > 0)
            {
                foreach (var item in l_long)
                {
                    long_name.Append(item.ToString());
                    long_name.Append("；");
                }
            }
            else
            {
                long_name.Append("无");
            }
            UIHelper.Label("lblLong").Text(long_name.ToString());
            #endregion

            #region 晚点名请假
            var l_night = (from l in db.vw_LeaveList
                           where l.LeaveType.StartsWith("晚点名请假") && l.StateLeave == "1" && l.StateBack == "0" && l.ST_Teacher == teacher_name && l.TimeBack.ToString().StartsWith(today.ToString())
                           select l.ST_Name)
                          .Distinct();
            StringBuilder night_name = new StringBuilder();
            if (l_night.Count() > 0)
            {
                foreach (var item in l_night)
                {
                    night_name.Append(item.ToString());
                    night_name.Append("；");
                }
            }
            else
            {
                night_name.Append("无");
            }
            UIHelper.Label("lblNight").Text(night_name.ToString());
            #endregion

            #region 节假日请假
            var l_vacation = (from l in db.vw_LeaveList
                              where l.TypeID == type_vacation && l.StateLeave == "1" && l.StateBack == "0" && l.ST_Teacher == teacher_name && l.TimeBack.ToString().StartsWith(today.ToString())
                              select l.ST_Name)
                          .Distinct();
            StringBuilder vacation_name = new StringBuilder();
            if (l_vacation.Count() > 0)
            {
                foreach (var item in l_vacation)
                {
                    vacation_name.Append(item.ToString());
                    vacation_name.Append("；");
                }
            }
            else
            {
                vacation_name.Append("无");
            }
            UIHelper.Label("lblVacation").Text(vacation_name.ToString());
            #endregion

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnEmail_Click(FormCollection form)
        {
            return UIHelper.Result();
        }
    }
}