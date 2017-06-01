using FineUIMvc;
using qingjia_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qingjia_MVC.Controllers;
using System.Data.Entity.Validation;

namespace qingjia_MVC.Areas.Message.Controllers
{
    public class DeadlineMessageController : BaseController
    {
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        private int type_night = 2;     //晚点名请假
        private int type_vacation = 1;  //节假日请假

        /// <summary>
        /// 测试使用
        /// </summary>
        protected void Login()
        {
            Session["UserID"] = "1214001";
            Session["UserName"] = "梁导";
            Session["Grade"] = "2014";
        }

        // GET: Message/DeadlineMessage
        public ActionResult Index()
        {
            //Login();
            LoadData();
            return View();
        }

        //Deadline 加载数据
        protected void LoadData()
        {
            string teacherid = Session["UserID"].ToString();
            string Grade = Session["Grade"].ToString();
            string date;
            string time;

            #region 晚点名时间
            var night = from d in db.T_Deadline
                        where d.TypeID == type_night && d.TeacherID == teacherid
                        select d.Time;
            ViewBag.DateNight = "";
            ViewBag.TimeNight = "";
            if (night.Count() > 0)
            {
                date = night.First().ToString("yyyy-MM-dd HH:mm").Substring(0, 10);
                time = night.First().ToString("yyyy-MM-dd HH:mm").Substring(11, 5);
                ViewBag.DateNight = date;
                ViewBag.TimeNight = time;
            }
            #endregion

            #region 节假日时间
            var deadline = from d in db.T_Deadline
                           where d.TypeID == type_vacation && d.TeacherID == teacherid
                           select d.Time;
            ViewBag.DateVacation = "";
            ViewBag.TimeVacation = "";

            var vacation = from T_Vacation in db.T_Vacation where (T_Vacation.TeacherID == teacherid && T_Vacation.Grade == Grade) orderby T_Vacation.ID descending select T_Vacation;
            if (!vacation.Any())
            {
                //表示节假日列表中尚不包含任何数据
                ViewBag.StartDate = "";
                ViewBag.StartTime = "";
                ViewBag.EndDate = "";
                ViewBag.EndTime = "";
            }
            else
            {
                //节假日表中存在数据，提取出最近的一条数据
                T_Vacation recentHoliday = vacation.First();
                DateTime startTime = (DateTime)recentHoliday.StartTime;
                DateTime endTime = (DateTime)recentHoliday.EndTime;
                ViewBag.StartDate = startTime.ToString("yyyy-MM-dd HH:mm").Substring(0, 10);
                ViewBag.StartTime = startTime.ToString("yyyy-MM-dd HH:mm").Substring(11, 5);
                ViewBag.EndDate = endTime.ToString("yyyy-MM-dd HH:mm").Substring(0, 10);
                ViewBag.EndTime = endTime.ToString("yyyy-MM-dd HH:mm").Substring(11, 5);
            }

            if (deadline.Count() > 0)
            {
                date = deadline.First().ToString("yyyy-MM-dd HH:mm").Substring(0, 10);
                time = deadline.First().ToString("yyyy-MM-dd HH:mm").Substring(11, 5);
                ViewBag.DateVacation = date;
                ViewBag.TimeVacation = time;
            }
            #endregion
        }

        #region 晚点名请假截止时间
        public string btnNight_Click()
        {
            string teacherid = Session["UserID"].ToString();
            T_Deadline t_deadline;
            string date = Request["dpNight"].ToString();
            string time = Request["tpNight"].ToString();
            string datetime = date + " " + time + ":00";
            DateTime d_time = Convert.ToDateTime(datetime);

            var night = from d in db.T_Deadline
                        where d.TypeID == type_night && d.TeacherID == teacherid
                        select d;

            if (night.Count() > 0)
            {
                t_deadline = night.First();
                t_deadline.Time = d_time;
                db.Entry(t_deadline).State = EntityState.Modified;
            }
            else
            {
                t_deadline = new T_Deadline();
                t_deadline.Time = d_time;
                t_deadline.TypeID = type_night;
                t_deadline.TeacherID = teacherid;

                db.T_Deadline.Add(t_deadline);
            }
            db.SaveChanges();
            LoadData();
            return "修改成功";
        }
        #endregion

        #region 设置节假日请假截止时间
        public string btnVacation_Click()
        {
            string teacherid = Session["UserID"].ToString();
            string Grade = Session["Grade"].ToString();

            string starttime = Request["starttime"].ToString();
            string endtime = Request["endtime"].ToString();
            string deadline = Request["deadline"].ToString();
            string AutoAudit = Request["AutoAudit"].ToString();

            //转换格式
            string DeadLine = deadline + ":00";
            string Start = starttime + ":00";
            string End = endtime + ":00";

            //String -> DateTime
            DateTime d_time = Convert.ToDateTime(DeadLine);
            DateTime start_time = Convert.ToDateTime(Start);
            DateTime end_time = Convert.ToDateTime(End);

            string vacationid = teacherid + DateTime.Now.ToString("yyyyMMdd");
            var vacationlist = from T_Vacation in db.T_Vacation where (T_Vacation.ID == vacationid) select T_Vacation;
            if (vacationlist.Any())
            {
                T_Vacation vacationmodel = db.T_Vacation.Find(vacationid);
                vacationmodel.StartTime = start_time;
                vacationmodel.EndTime = end_time;
                vacationmodel.AutoAudit = AutoAudit;
            }
            else
            {
                T_Vacation vacationmodel = new T_Vacation();
                vacationmodel.StartTime = start_time;
                vacationmodel.EndTime = end_time;
                vacationmodel.SubmitTime = DateTime.Now;
                vacationmodel.AutoAudit = AutoAudit;
                vacationmodel.ID = vacationid;
                vacationmodel.Grade = Grade;
                vacationmodel.TeacherID = teacherid;
                vacationmodel.Name = "";
                vacationmodel.Remark = "";
                db.T_Vacation.Add(vacationmodel);
            }


            var deadlinglist = from T_Deadline in db.T_Deadline where (T_Deadline.TeacherID == teacherid && T_Deadline.TypeID == type_vacation) select T_Deadline;
            if (deadlinglist.Any())
            {
                //包含数据
                int id = deadlinglist.First().ID;
                T_Deadline dl = db.T_Deadline.Find(id);
                if (dl.Time != d_time)
                {
                    dl.Time = d_time;
                }
            }
            else
            {
                T_Deadline dl = new T_Deadline();
                dl.TeacherID = teacherid;
                dl.TypeID = 1;
                dl.Time = d_time;
                db.T_Deadline.Add(dl);
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

            }
            return "修改成功";
        }
        #endregion
    }
}