using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FineUIMvc;
using qingjia_MVC;
using qingjia_MVC.Controllers;
using qingjia_MVC.Models;
using qingjia_MVC.Content;
using System.Data.Entity.Validation;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Data.Entity;

namespace qingjia_MVC.Areas.Leave.Controllers
{
    public class LeaveListModel
    {
        public List<LL_Table> TopTotalList { get; set; }
        public List<LL_Table> TopList { get; set; }
    }

    public class LeaveListController : BaseController
    {
        //实例化数据库
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        #region Index
        // GET: Leave/LeaveList
        public ActionResult Index()
        {
            //LoadData();
            return View();
        }

        public ActionResult GetLeaveListTable()
        {
            string UserID = Session["UserID"].ToString();
            var leaveList = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID) orderby vw_LeaveList.ID descending select vw_LeaveList;
            if (leaveList.Count() == 0)
            {
                ViewBag.LeaveListExist = false;
                return PartialView("_IndexTableList");
            }
            else
            {
                ViewBag.LeaveListExist = true;
                foreach (var ll in leaveList.Take(5).ToList())
                {
                    ll.StateLeave = getKey("leavetype", ll.StateLeave + ll.StateBack);
                }
                return PartialView("_IndexTableList", leaveList.Take(5).ToList());
            }
        }

        //Index 加载数据
        protected void LoadData()
        {
            //防止Session过期
            //try
            //{
            //    var leaveList = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == ST_Num) orderby vw_LeaveList.ID descending select vw_LeaveList;
            //    if (leaveList.Count() == 0)
            //    {
            //        ViewBag.LeaveListExist = false;
            //    }
            //    else
            //    {
            //        ViewBag.LeaveListExist = true;
            //        foreach (var ll in leaveList.Take(5).ToList())
            //        {
            //            ll.StateLeave = getKey("leavetype", ll.StateLeave + ll.StateBack);
            //        }
            //        ViewData["leaveList"] = leaveList.Take(5).ToList();
            //    }
            //}
            //catch
            //{

            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changeInfo()
        {
            ActiveWindow.HidePostBack();
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult window_leave_Close()
        {
            //弹出个人信息修改框
            UIHelper.Window("changeInfo").Hidden(false);
            return UIHelper.Result();
        }
        #endregion

        public string leaveAllow()
        {
            string type = Request["type"].ToString();
            string ST_Num = Session["UserID"].ToString();
            string ST_Teacher = (from stu in db.vw_Student where stu.ST_Num == ST_Num select stu.ST_TeacherID).First();

            if (type == "vacation")
            {
                var deadLine = from T_Deadline in db.T_Deadline where T_Deadline.TeacherID == ST_Teacher && T_Deadline.TypeID == 1 select T_Deadline;
                string deadLineTime = deadLine.ToList().First().Time.ToString();

                if (Convert.ToDateTime(deadLineTime) >= DateTime.Now)//截止时间大于当前时间
                {
                    return "成功";
                }
                else
                {
                    return "节假日去向填写已经截至！";
                }
            }
            else if (type == "night")
            {
                var deadline = from d in db.T_Deadline where d.TeacherID == ST_Teacher && d.TypeID == 2 select d.Time;
                if (deadline.Any() && deadline.First().Date >= DateTime.Now.Date)
                {
                    return "成功";
                }
                else
                {
                    return "晚点名请假已截止！";
                }
            }
            else if (type == "review")
            {
                var student = from vw_Student in db.vw_Student where vw_Student.ST_Num == ST_Num select vw_Student;
                vw_Student modelStudent = student.ToList().First();

                //此处为需要晚自习的年级
                if (modelStudent.ST_Grade != "2016")
                {
                    return "您没有早晚自习!";
                }
                else
                {
                    return "成功";
                }
            }
            else
            {
                return "成功";
            }


        }

        #region 离校请假

        //GET: Leave/LeaveList/leaveschool
        public ActionResult leaveschool()
        {
            leaveschool_LoadData();
            return View();
        }

        #region 短期请假
        //GET: Leave/LeaveList/leavelong
        public ActionResult leaveshort()
        {
            leaveschool_LoadData();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leaveshort_submit(FormCollection formInfo)
        {
            string LL_Type = formInfo["LL_Type"];
            string leaveDate = formInfo["leaveDate"];
            string leaveTime = formInfo["leaveTime"];
            string backDate = formInfo["backDate"];
            string backTime = formInfo["backTime"];
            string leaveReason = formInfo["leaveReason"];
            string leaveWay = null;
            string backWay = null;
            string address = null;
            string holidayType = null;

            leaveSchool_btnSubmit_Click(LL_Type, leaveDate, leaveTime, backDate, backTime, leaveReason, leaveWay, backWay, address, holidayType);
            return UIHelper.Result();
        }
        #endregion
        #region 长期请假
        //GET: Leave/LeaveList/leavelong
        public ActionResult leavelong()
        {
            leaveschool_LoadData();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leavelong_submit(FormCollection formInfo)
        {
            string LL_Type = formInfo["LL_Type"];
            string leaveDate = formInfo["leaveDate"];
            string leaveTime = formInfo["leaveTime"];
            string backDate = formInfo["backDate"];
            string backTime = formInfo["backTime"];
            string leaveReason = formInfo["leaveReason"];
            string leaveWay = null;
            string backWay = null;
            string address = null;
            string holidayType = null;

            leaveSchool_btnSubmit_Click(LL_Type, leaveDate, leaveTime, backDate, backTime, leaveReason, leaveWay, backWay, address, holidayType);
            return UIHelper.Result();
        }
        #endregion
        #region 节假日去向
        //GET: Leave/LeaveList/leavelong
        public ActionResult leavevacation()
        {
            leaveschool_LoadData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leavevacation_submit(FormCollection formInfo)
        {
            string LL_Type = formInfo["LL_Type"];
            string leaveDate = formInfo["leaveDate"];
            string leaveTime = formInfo["leaveTime"];
            string backDate = formInfo["backDate"];
            string backTime = formInfo["backTime"];
            string leaveReason = null;
            string leaveWay = formInfo["leaveWay"];
            string backWay = formInfo["backWay"];
            string address = formInfo["address"];
            string holidayType = formInfo["holidayType"];

            leaveSchool_btnSubmit_Click(LL_Type, leaveDate, leaveTime, backDate, backTime, leaveReason, leaveWay, backWay, address, holidayType);
            return UIHelper.Result();
        }
        #endregion

        //leaveschool 加载数据
        protected void leaveschool_LoadData()
        {
            string ST_Num = Session["UserID"].ToString();
            var ST_Info = from vw_Student in db.vw_Student where (vw_Student.ST_Num == ST_Num) select vw_Student;
            if (ST_Info.Any())
            {
                ViewData["LL_ST_Info"] = ST_Info.ToList().First();
            }

            var type = from T_Type in db.T_Type where (T_Type.FatherID == 1) select T_Type.Name;
            ViewBag.LeaveType = type.ToList();

            //ViewBag.holidayType
            List<string> holidayType = new List<string>();
            holidayType.Add("回家");
            holidayType.Add("旅游");
            holidayType.Add("因公外出");
            holidayType.Add("其他");
            ViewBag.ddl_holidayType = holidayType;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LL_TypeOnSelectedIndexChanged(string LL_Type)
        {
            var deadLine = from T_Deadline in db.T_Deadline where (T_Deadline.TypeID == 1) select T_Deadline;
            string deadLineTime = deadLine.ToList().First().Time.ToString();


            string text = LL_Type;
            if (LL_Type == "节假日请假")
            {
                if (Convert.ToDateTime(deadLineTime) >= DateTime.Now)//截止时间大于当前时间
                {
                    //节假日请假尚未截止
                    UIHelper.FormRow("holiday_formrow_1").Hidden(false);
                    UIHelper.FormRow("holiday_formrow_2").Hidden(false);
                    UIHelper.FormRow("holiday_formrow_3").Hidden(false);
                    UIHelper.TextArea("leaveReason").Hidden(true);
                }
                else
                {
                    ShowNotify("节假日请假已经截至！");
                    UIHelper.DropDownList("LL_Type").Reset();
                }
            }
            else if (LL_Type == "短期请假" || LL_Type == "长期请假")
            {
                UIHelper.FormRow("holiday_formrow_1").Hidden(true);
                UIHelper.FormRow("holiday_formrow_2").Hidden(true);
                UIHelper.FormRow("holiday_formrow_3").Hidden(true);
                UIHelper.TextArea("leaveReason").Hidden(false);
            }
            else if (LL_Type == "早晚自习请假")
            {
                string ST_Num = Session["UserID"].ToString();
                var student = from vw_Student in db.vw_Student where vw_Student.ST_Num == ST_Num select vw_Student;
                vw_Student modelStudent = student.ToList().First();

                //此处为需要晚自习的年级
                if (modelStudent.ST_Grade != "2016")
                {
                    ShowNotify("您没有早晚自习！");
                    UIHelper.DropDownList("LL_Type").Reset();
                }
            }
            return UIHelper.Result();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public void leaveSchool_btnSubmit_Click(string LL_Type, string leaveDate, string leaveTime, string backDate, string backTime, string leaveReason, string leaveWay, string backWay, string address, string holidayType)
        //public ActionResult leaveSchool_btnSubmit_Click(FormCollection formInfo)
        {
            //Alert.MessageBoxIcon可设置提示框图标样式
            //可选样式：None无 Information消息 Warning警告 Question问题 Error错误 Success成功
            //Alert.Target可设置显示提示框的位置
            //可选样式：Self当前页面 Parent父页面 Top顶层页面
            string ST_Num = Session["UserID"].ToString();
            //string LL_Type = formInfo["LL_Type"];
            //string leaveDate = formInfo["leaveDate"];
            //string leaveTime = formInfo["leaveTime"];
            //string backDate = formInfo["backDate"];
            //string backTime = formInfo["backTime"];
            //string leaveReason = formInfo["leaveReason"];
            //string leaveWay = formInfo["leaveWay"];
            //string backWay = formInfo["backWay"];
            //string address = formInfo["address"];
            //string holidayType = formInfo["holidayType"];

            string LV_Time_Go = leaveDate + " " + leaveTime + ":00";
            string LV_Time_Back = backDate + " " + backTime + ":00";

            if (Convert.ToDateTime(LV_Time_Go) < Convert.ToDateTime(LV_Time_Back))
            {
                DateTime time_go = Convert.ToDateTime(LV_Time_Go);
                DateTime time_back = Convert.ToDateTime(LV_Time_Back);
                TimeSpan time_days = time_back - time_go;
                int days = time_days.Days;

                if (LL_Type == "短期请假")
                {
                    if (days < 3)//短期请假小于三天
                    {
                        //生成请假单号
                        string LV_NUM = DateTime.Now.ToString("yyMMdd");
                        var exist = from T_LeaveList in db.T_LeaveList where (T_LeaveList.StudentID == ST_Num) && (((T_LeaveList.TimeLeave >= time_go) && (T_LeaveList.TimeLeave <= time_back)) || ((T_LeaveList.TimeBack >= time_go) && (T_LeaveList.TimeBack <= time_back)) || ((T_LeaveList.TimeLeave <= time_go) && (T_LeaveList.TimeBack >= time_back))) select T_LeaveList;

                        if (exist.Any())
                        {
                            foreach (qingjia_MVC.Models.T_LeaveList leaveList in exist.ToList())
                            {
                                if (leaveList.StateBack == "0")
                                {
                                    alertInfo("错误提示", "您已提交过此时间段的请假申请，请不要重复提交！", "Information");
                                    break;
                                }
                                else
                                {
                                    //插入数据库操作
                                    if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                                    {
                                        string script = String.Format("alert('请假申请成功！');");
                                        PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                            script);
                                    }
                                    else
                                    {
                                        alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");

                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //插入数据库操作
                            if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                            {
                                string script = String.Format("alert('请假申请成功');");
                                PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                    script);
                            }
                            else
                            {
                                alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                            }
                        }
                    }
                    else
                    {
                        alertInfo("错误提示", "短期请假不能超过3天!", "Error");
                    }
                }
                else if (LL_Type == "长期请假")
                {
                    if (days >= 3)//长期请假
                    {
                        //生成请假单号
                        string LV_NUM = DateTime.Now.ToString("yyMMdd");
                        var exist = from T_LeaveList in db.T_LeaveList where (T_LeaveList.StudentID == ST_Num) && (((T_LeaveList.TimeLeave >= time_go) && (T_LeaveList.TimeLeave <= time_back)) || ((T_LeaveList.TimeBack >= time_go) && (T_LeaveList.TimeBack <= time_back)) || ((T_LeaveList.TimeLeave <= time_go) && (T_LeaveList.TimeBack >= time_back))) select T_LeaveList;

                        if (exist.Any())
                        {
                            foreach (qingjia_MVC.Models.T_LeaveList leaveList in exist.ToList())
                            {
                                if (leaveList.StateBack == "1")
                                {
                                    alertInfo("错误提示", "您已提交过此时间段的请假申请，请不要重复提交！", "Information");
                                    break;
                                }
                                else
                                {
                                    //插入数据库操作
                                    if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                                    {
                                        string script = String.Format("alert('请假申请成功');");
                                        PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                            script);
                                    }
                                    else
                                    {
                                        alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //插入数据库操作
                            if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                            {
                                string script = String.Format("alert('请假申请成功');");
                                PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                    script);
                            }
                            else
                            {
                                alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                            }
                        }
                    }
                    else
                    {
                        alertInfo("错误提示", "长期请假短期请假不能少于3天!", "Error");
                    }
                }
                else if (LL_Type == "节假日请假")
                {
                    //生成请假单号
                    string LV_NUM = DateTime.Now.ToString("yyMMdd");
                    var exist = from T_LeaveList in db.T_LeaveList where (T_LeaveList.StudentID == ST_Num) && (((T_LeaveList.TimeLeave >= time_go) && (T_LeaveList.TimeLeave <= time_back)) || ((T_LeaveList.TimeBack >= time_go) && (T_LeaveList.TimeBack <= time_back)) || ((T_LeaveList.TimeLeave <= time_go) && (T_LeaveList.TimeBack >= time_back))) select T_LeaveList;

                    if (exist.Any())
                    {
                        foreach (qingjia_MVC.Models.T_LeaveList leaveList in exist.ToList())
                        {
                            if (leaveList.StateBack == "1")
                            {
                                alertInfo("错误提示", "您已提交过此时间段的请假申请，请不要重复提交！", "Information");
                                break;
                            }
                            else
                            {
                                //插入数据库操作
                                if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                                {
                                    string script = String.Format("alert('请假申请成功');");
                                    PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                        script);
                                }
                                else
                                {
                                    alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        //插入数据库操作
                        if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                        {
                            string script = String.Format("alert('请假申请成功');");
                            PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                script);
                        }
                        else
                        {
                            alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                        }
                    }
                }
                else
                {
                    //非离校请假类型
                }
            }
            else
            {
                alertInfo("错误提示", "请假开始时间不能小于结束时间!", "Error");
            }
            //return UIHelper.Result();
        }

        #endregion

        #region 特殊请假

        //GET: Leave/LeaveList/leavespecial
        public ActionResult leavespecial()
        {
            leavespecial_LoadData();
            return View();
        }

        #region 晚点名请假
        //GET: Leave/LeaveList/leavelong
        public ActionResult leavenight()
        {
            leavespecial_LoadData();
            //string ST_Num = Session["UserID"].ToString();
            //string ST_Teacher = (from stu in db.vw_Student where stu.ST_Num == ST_Num select stu.ST_TeacherID).First();
            //var deadline = from d in db.T_Deadline where d.TeacherID == ST_Teacher && d.TypeID == 2 select d.Time;
            //if (deadline.Any() && deadline.First().Date >= DateTime.Now.Date)
            //{
            //    leavespecial_LoadData();
            //}
            //else
            //{
            //    leavespecial_LoadData();
            //    ShowNotify("晚点名请假已截止！");
            //}
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leavenight_btnSubmit_Click(FormCollection formInfo)
        {
            string type = formInfo["LL_Type"];
            string LL_Type_Child = formInfo["LL_Type_Child"];
            string leaveDate = formInfo["leaveDate"];
            string backDate = formInfo["backDate"];
            string leaveReason = formInfo["leaveReason"];

            leaveSpecial_btnSubmit_Click(type, LL_Type_Child, leaveDate, backDate, leaveReason);
            return UIHelper.Result();
        }
        #endregion
        #region 早晚自习请假
        //GET: Leave/LeaveList/leavelong
        public ActionResult leavereview()
        {
            leavespecial_LoadData();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leavereview_btnSubmit_Click(FormCollection formInfo)
        {
            string type = formInfo["LL_Type"];
            string LL_Type_Child = formInfo["LL_Type_Child"];
            string leaveDate = formInfo["leaveDate"];
            string backDate = formInfo["backDate"];
            string leaveReason = formInfo["leaveReason"];

            leaveSpecial_btnSubmit_Click(type, LL_Type_Child, leaveDate, backDate, leaveReason);
            return UIHelper.Result();
        }
        #endregion

        public void leavespecial_LoadData()
        {
            //加载个人基本信息
            string ST_Num = Session["UserID"].ToString();
            var ST_Info = from vw_Student in db.vw_Student where (vw_Student.ST_Num == ST_Num) select vw_Student;
            if (ST_Info.Any())
            {
                ViewData["LL_ST_Info"] = ST_Info.ToList().First();
            }
            else
            {

            }

            //非数据库数据
            List<string> type = new List<string>();
            type.Add("晚点名请假");
            type.Add("早晚自习请假");
            ViewBag.LeaveType = type;

            //ViewBag.ChildType
            List<string> ChildType = new List<string>();
            ChildType.Add("公假");
            ChildType.Add("事假");
            ChildType.Add("病假");
            ViewBag.ChildType = ChildType;
        }

        public ActionResult leaveSpecial_btnSubmit_Click(string Type, string LL_Type_Child, string leaveDate, string backDate, string leaveReason)
        //public ActionResult leaveSpecial_btnSubmit_Click(FormCollection formInfo)
        {
            string ST_Num = Session["UserID"].ToString();
            string LL_Type = Type + "(" + LL_Type_Child + ")";
            string leaveTime = "00:00";
            string leaveWay = null;
            string backWay = null;
            string address = null;
            string holidayType = null;


            if (Type == "晚点名请假")
            {
                string LV_NUM = DateTime.Now.ToString("yymmdd");//流水号生成
                string studytime_sp = leaveDate + " " + leaveTime + ":00";
                DateTime time_go = Convert.ToDateTime(studytime_sp);
                DateTime time_back = time_go;
                var exist = from T_LeaveList in db.T_LeaveList where ((T_LeaveList.StudentID == ST_Num) && (T_LeaveList.TimeLeave == time_go) && (T_LeaveList.TypeID == 2)) select T_LeaveList;

                if (exist.Any())
                {
                    foreach (qingjia_MVC.Models.T_LeaveList leaveList in exist.ToList())
                    {
                        if (leaveList.StateBack == "0")
                        {
                            alertInfo("错误提示", "您已提交过此时间段的请假申请，请不要重复提交！", "Information");
                            break;
                        }
                        else
                        {
                            //插入数据库操作
                            if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                            {
                                string script = String.Format("alert('请假申请成功！');");
                                PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                    script);
                            }
                            else
                            {
                                alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                            }
                            break;
                        }
                    }
                }
                else
                {
                    //插入数据库操作
                    if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                    {
                        string script = String.Format("alert('请假申请成功');");
                        PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                            script);
                    }
                    else
                    {
                        alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                    }
                }
            }
            else if (Type == "早晚自习请假")
            {
                string LV_NUM = DateTime.Now.ToString("yymmdd");//流水号生成
                string studytime_sp = leaveDate + " " + leaveTime + ":00";
                DateTime time_go = Convert.ToDateTime(studytime_sp);
                DateTime time_back = time_go;
                var exist = from T_LeaveList in db.T_LeaveList where ((T_LeaveList.StudentID == ST_Num) && (T_LeaveList.TimeLeave == time_go) && (T_LeaveList.TypeID == 2)) select T_LeaveList;

                if (exist.Any())
                {
                    foreach (qingjia_MVC.Models.T_LeaveList leaveList in exist.ToList())
                    {
                        if (leaveList.StateBack == "0")
                        {
                            alertInfo("错误提示", "您已提交过此时间段的请假申请，请不要重复提交！", "Information");
                            break;
                        }
                        else
                        {
                            //插入数据库操作
                            if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                            {
                                string script = String.Format("alert('请假申请成功！');");
                                PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                    script);
                            }
                            else
                            {
                                alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                            }
                            break;
                        }
                    }
                }
                else
                {
                    //插入数据库操作
                    if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, leaveWay, backWay, address, holidayType, null, null, null) == 1)
                    {
                        string script = String.Format("alert('请假申请成功');");
                        PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                            script);
                    }
                    else
                    {
                        alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                    }
                }
            }
            return UIHelper.Result();
        }

        #endregion

        #region 上课请假备案

        //GET: Leave/LeaveList/leavelesson
        public ActionResult leavelesson()
        {
            leaveclass_LoadData();
            return View();
        }

        //leaveclass 页面加载数据
        public void leaveclass_LoadData()
        {
            //加载个人基本信息
            string ST_Num = Session["UserID"].ToString();
            var ST_Info = from vw_Student in db.vw_Student where (vw_Student.ST_Num == ST_Num) select vw_Student;
            if (ST_Info.Any())
            {
                ViewData["LL_ST_Info"] = ST_Info.ToList().First();
            }

            //非数据库数据
            List<string> type = new List<string>();
            type.Add("第一大节（08:00~09:40）");
            type.Add("第二大节（10:10~11:50）");
            type.Add("第三大节（14:00~15:30）");
            type.Add("第四大节（16:00~17:40）");
            type.Add("第五大节（18:30~21:40）");
            ViewBag.LeaveType = type;

            //ViewBag.ChildType
            List<string> ChildType = new List<string>();
            ChildType.Add("公假");
            ChildType.Add("事假");
            ChildType.Add("病假");
            ViewBag.ChildType = ChildType;
        }

        //leaveclass 页面提交操作
        public ActionResult leaveClass_btnSubmit_Click(FormCollection formInfo)
        {
            string ST_Num = Session["UserID"].ToString();
            string LL_Type = "上课请假备案(" + formInfo["LL_Type_Child"] + ")";
            string leaveDate = formInfo["leaveDate"];
            string leaveTime = formInfo["LL_Type"].ToString().Substring(5, 5);
            string backDate = formInfo["leaveDate"];
            string backTime = formInfo["LL_Type"].ToString().Substring(11, 5);
            string leaveReason = formInfo["leaveReason"];
            string lesson = "";

            //string lesson = formInfo["LL_Type"];
            if (formInfo["LL_Type"].ToString().Contains("第一大节"))
            {
                lesson = "1";
            }
            else if (formInfo["LL_Type"].ToString().Contains("第二大节"))
            {
                lesson = "2";
            }
            else if (formInfo["LL_Type"].ToString().Contains("第三大节"))
            {
                lesson = "3";
            }
            else if (formInfo["LL_Type"].ToString().Contains("第四大节"))
            {
                lesson = "4";
            }
            else if (formInfo["LL_Type"].ToString().Contains("第五大节"))
            {
                lesson = "5";
            }
            else
            {
                lesson = "";
            }

            string teacher = formInfo["txbTeacher"];

            string LV_NUM = DateTime.Now.ToString("yymmdd");//流水号生成
            DateTime time_go = Convert.ToDateTime(leaveDate + " " + leaveTime + ":00");
            DateTime time_back = Convert.ToDateTime(backDate + " " + backTime + ":00"); ;

            var exist = from T_LeaveList in db.T_LeaveList where ((T_LeaveList.StudentID == ST_Num) && (T_LeaveList.Lesson == lesson) && (T_LeaveList.TypeID == 3)) select T_LeaveList;

            if (exist.Any())
            {
                foreach (qingjia_MVC.Models.T_LeaveList leaveList in exist.ToList())
                {
                    if (leaveList.StateBack == "0")
                    {
                        alertInfo("错误提示", "您已提交过此时间段的请假申请，请不要重复提交！", "Information");
                        break;
                    }
                    else
                    {
                        //插入数据库操作
                        if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, null, null, null, null, null, lesson, teacher) == 1)
                        {
                            string script = String.Format("alert('请假申请成功！');");
                            PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                                script);
                        }
                        else
                        {
                            alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                        }
                        break;
                    }
                }
            }
            else
            {
                //插入数据库操作
                if (Insert_LeaveList(LV_NUM, ST_Num, LL_Type, time_go, time_back, leaveReason, null, null, null, null, null, lesson, teacher) == 1)
                {
                    string script = String.Format("alert('请假申请成功');");
                    PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                        script);
                }
                else
                {
                    alertInfo("提交失败", "数据库提交失败，请重新尝试!", "Information");
                }
            }
            return UIHelper.Result();
        }

        #endregion

        #region 通知方法

        /// <summary>
        /// Alert.MessageBoxIcon可设置提示框图标样式,可选样式：None无 Information消息 Warning警告 Question问题 Error错误 Success成功,Alert.Target可设置显示提示框的位置,可选样式：Self当前页面 Parent父页面 Top顶层页面
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">信息</param>
        /// <param name="icon">Icon类型</param>
        public void alertInfo(string title, string message, string icon)
        {
            Alert alert = new Alert();
            alert.Title = title;
            alert.Message = message;
            alert.MessageBoxIcon = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), icon, true);
            alert.Target = (Target)Enum.Parse(typeof(Target), "Self", true);
            alert.Show();
        }

        #endregion

        #region 插入T_LeaveList表

        /// <summary>
        /// 插入请假表
        /// </summary>
        /// <param name="LV_Num">请假单号</param>
        /// <param name="ST_Num">学号</param>
        /// <param name="LL_Type">请假类型名称</param>
        /// <param name="TimeLeave">请假时间</param>
        /// <param name="TimeBack">返校时间</param>
        /// <param name="LeaveReason">请假原因</param>
        /// <param name="leaveWay">离校方式</param>
        /// <param name="backWay">返校方式</param>
        /// <param name="leaveAddress">离校地址</param>
        /// <param name="holidayType">假期去向</param>
        /// <param name="notes">请假驳回理由</param>
        /// <param name="lesson">课程</param>
        /// <param name="teacher">老师</param>
        /// <returns>插入数据库结果</returns>
        protected int Insert_LeaveList(string LV_Num, string ST_Num, string LL_Type, DateTime TimeLeave, DateTime TimeBack, string LeaveReason, string leaveWay, string backWay, string leaveAddress, string holidayType, string notes, string lesson, string teacher)
        {
            string endString = "0001";
            var leavelist = from T_LeaveList in db.T_LeaveList where (T_LeaveList.ID.StartsWith(LV_Num)) orderby T_LeaveList.ID descending select T_LeaveList.ID;
            if (leavelist.Any())
            {
                string leaveNumTop = leavelist.First().ToString().Trim();
                int end = Convert.ToInt32(leaveNumTop.Substring(6, 4));
                end++;
                endString = end.ToString("0000");//按照此格式Tostring
            }
            //请假单号
            LV_Num += endString;
            //提交时间
            DateTime nowTime = DateTime.Now;
            //请假类型编号
            var type = from T_Type in db.T_Type where (T_Type.Name == LL_Type) select T_Type;

            int TypeID = (int)((T_Type)type.ToList().First()).FatherID;
            int TypeChildID = (int)((T_Type)type.ToList().First()).ID;

            T_LeaveList LL = new T_LeaveList();
            LL.ID = LV_Num;
            LL.StudentID = ST_Num;
            LL.Reason = LeaveReason;
            LL.SubmitTime = nowTime;
            LL.StateLeave = "0";
            LL.StateBack = "0";
            LL.Notes = notes;
            LL.TypeID = TypeID;
            LL.TypeChildID = TypeChildID;
            LL.TimeLeave = TimeLeave;
            LL.TimeBack = TimeBack;
            LL.LeaveWay = leaveWay;
            LL.BackWay = backWay;
            LL.Address = leaveAddress;
            LL.Lesson = lesson;
            LL.Teacher = teacher;
            db.T_LeaveList.Add(LL);
            try
            {
                return db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                return 0;
            }
        }

        #endregion

        #region LeaveList
        //GET: LeaveList/leavelist
        public ActionResult leavelist()
        {
            //Session["UserID"] = "1214001";
            //Session["RoleID"] = "3";
            string RoleID = Session["RoleID"].ToString();
            ViewBag.RoleID = RoleID;
            return View();
        }

        public ActionResult GetTable(string condition)
        {
            //获取当前用户信息
            string RoleID = Session["RoleID"].ToString();
            string UserID = Session["UserID"].ToString();
            if (condition == "" || condition == null)
            {
                ViewBag.condition = "AllLeave";
            }
            else
            {
                ViewBag.condition = condition;
            }
            ViewBag.RoleID = RoleID;
            return PartialView("_tablelist", Get_LL(UserID, RoleID, condition, 30));
        }

        /// <summary>
        /// 获取全部可查看请假记录数据
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="RoleID">角色ID</param>
        /// <param name="condition">请假记录类型条件</param>
        /// <returns></returns>
        public List<LL_Table> Get_LL(string UserID, string RoleID, string condition)
        {
            List<vw_LeaveList> LL_List = new List<vw_LeaveList>();

            if (RoleID == "1")//学生
            {
                var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID) orderby vw_LeaveList.SubmitTime descending select vw_LeaveList;
                LL_List = LL.ToList();
            }
            else if (RoleID == "2")//班级
            {
                //获取班级账号名称
                string className = Session["UserName"].ToString();

                var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_Class == className) orderby vw_LeaveList.SubmitTime descending select vw_LeaveList;
                LL_List = LL.ToList();
            }
            else if (RoleID == "3")//辅导员
            {
                var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID) orderby vw_LeaveList.SubmitTime descending select vw_LeaveList;
                LL_List = LL.ToList();
            }
            else
            {
                return null;
            }
            //统计各类请假次数
            LL_Count(LL_List);

            return changeLLModel(LL_List);
        }

        /// <summary>
        /// 获取请假记录数据
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="RoleID">角色ID</param>
        /// <param name="condition">请假类型条件</param>
        /// <param name="n">每类请假类型查询条数</param>
        /// <returns></returns>
        public LeaveListModel Get_LL(string UserID, string RoleID, string condition, int n)
        {
            LeaveListModel model = new LeaveListModel();//数据模型  用于_tablelist界面

            List<vw_LeaveList> LL_List = new List<vw_LeaveList>();//从数据库中提取出的请假记录集合
            List<vw_LeaveList> TopList = new List<vw_LeaveList>();//各种请假类型，各取前n条的集合
            List<vw_LeaveList> TopTotalList = new List<vw_LeaveList>();//全部请假记录的前那条集合

            #region 提取数据
            if (RoleID == "1")//学生
            {
                var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID) orderby vw_LeaveList.SubmitTime descending select vw_LeaveList;
                LL_List = LL.ToList();
            }
            else if (RoleID == "2")//班级
            {
                //获取班级账号名称
                string className = Session["UserName"].ToString();

                var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_Class == className) orderby vw_LeaveList.SubmitTime descending select vw_LeaveList;
                LL_List = LL.ToList();
            }
            else if (RoleID == "3")//辅导员
            {
                var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID) orderby vw_LeaveList.SubmitTime descending select vw_LeaveList;
                LL_List = LL.ToList();
            }
            else
            {
                return null;
            }
            #endregion

            #region 取前n条数据

            List<int> LLNum = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                LLNum.Add(0);
            }

            for (int i = 0; i < 5; i++)
            {
                LLNum[i] = 0;
            }

            foreach (vw_LeaveList llModel in LL_List)
            {
                if (llModel.LeaveType.ToString().Substring(0, 4) == "短期请假")
                {
                    if (LLNum[0] < n)
                    {
                        TopList.Add(llModel);
                        LLNum[0]++;
                        continue;
                    }
                }
                if (llModel.LeaveType.ToString().Substring(0, 4) == "长期请假")
                {
                    if (LLNum[1] < n)
                    {
                        TopList.Add(llModel);
                        LLNum[1]++;
                        continue;
                    }
                }
                if (llModel.LeaveType.ToString().Substring(0, 4) == "节假日请")
                {
                    if (LLNum[2] < n)
                    {
                        TopList.Add(llModel);
                        LLNum[2]++;
                        continue;
                    }
                }
                if (llModel.LeaveType.ToString().Substring(0, 4) == "晚点名请")
                {
                    if (LLNum[3] < n)
                    {
                        TopList.Add(llModel);
                        LLNum[3]++;
                        continue;
                    }
                }
                if (llModel.LeaveType.ToString().Substring(0, 4) == "上课请假")
                {
                    if (LLNum[4] < n)
                    {
                        TopList.Add(llModel);
                        LLNum[4]++;
                        continue;
                    }
                }

                if (LLNum[0] == n && LLNum[1] == n && LLNum[2] == n && LLNum[3] == n && LLNum[4] == n)
                {
                    break;
                }
            }

            //获取全部请假记录的前n条数据
            if (LL_List.Count > n)
            {
                for (int i = 0; i < n; i++)
                {
                    TopTotalList.Add(LL_List[i]);
                }
            }
            else
            {
                foreach (vw_LeaveList ll in LL_List)
                {
                    TopTotalList.Add(ll);
                }
            }
            #endregion

            model.TopTotalList = changeLLModel(TopTotalList);
            model.TopList = changeLLModel(TopList);

            return model;
        }

        /// <summary>
        /// vw_LeaveList 转为  LL_Table
        /// </summary>
        /// <param name="LL"></param>
        /// <returns></returns>
        protected List<LL_Table> changeLLModel(List<vw_LeaveList> LL)
        {
            List<LL_Table> _LL = new List<LL_Table>();
            foreach (vw_LeaveList vw_LL in LL)
            {
                LL_Table table = new LL_Table();
                table.ID = vw_LL.ID;
                table.Reason = vw_LL.Reason;
                table.StateLeave = vw_LL.StateLeave;
                table.StateBack = vw_LL.StateBack;
                table.Notes = vw_LL.Notes;
                table.TypeID = vw_LL.TypeID.ToString();
                table.SubmitTime = ((DateTime)vw_LL.SubmitTime).ToString("yyyy-MM-dd HH:mm:ss");
                table.TimeLeave = ((DateTime)vw_LL.TimeLeave).ToString("yyyy-MM-dd HH:mm:ss");
                table.TimeBack = ((DateTime)vw_LL.TimeBack).ToString("yyyy-MM-dd HH:mm:ss");
                table.LeaveWay = vw_LL.LeaveWay;
                table.BackWay = vw_LL.BackWay;
                table.Address = vw_LL.Address;
                table.TypeChildID = vw_LL.TypeChildID.ToString();
                table.Teacher = vw_LL.Teacher;
                table.ST_Name = vw_LL.ST_Name;
                table.ST_Tel = vw_LL.ST_Tel;
                table.ST_Grade = vw_LL.ST_Grade;
                table.ST_Class = vw_LL.ST_Class;
                table.ST_Teacher = vw_LL.ST_Teacher;
                table.StudentID = vw_LL.StudentID;
                table.LeaveType = vw_LL.LeaveType;
                table.AuditName = vw_LL.AuditName;
                table.Type = vw_LL.Type;

                //审批状态
                table.AuditState = "Error";
                if (vw_LL.StateLeave.ToString() == "0" && vw_LL.StateBack.ToString() == "0")
                {
                    table.AuditState = "待审核";
                }
                if (vw_LL.StateLeave.ToString() == "1" && vw_LL.StateBack.ToString() == "0")
                {
                    table.AuditState = "待销假";
                }
                if (vw_LL.StateLeave.ToString() == "1" && vw_LL.StateBack.ToString() == "1")
                {
                    table.AuditState = "已销假";
                }
                if (vw_LL.StateLeave.ToString() == "2" && vw_LL.StateBack.ToString() == "1")
                {
                    table.AuditState = "已驳回";
                }

                //请假课段属性
                table.Lesson = "";
                if (vw_LL.Lesson != null && vw_LL.Lesson != "" && vw_LL.Lesson.ToString() == "1")
                {
                    table.Lesson = "第一大节（08:00~09:40）";
                }
                if (vw_LL.Lesson != null && vw_LL.Lesson != "" && vw_LL.Lesson.ToString() == "2")
                {
                    table.Lesson = "第二大节（10:10~11:50）";
                }
                if (vw_LL.Lesson != null && vw_LL.Lesson != "" && vw_LL.Lesson.ToString() == "3")
                {
                    table.Lesson = "第三大节（14:00~15:40）";
                }
                if (vw_LL.Lesson != null && vw_LL.Lesson != "" && vw_LL.Lesson.ToString() == "4")
                {
                    table.Lesson = "第四大节（16:00~17:40）";
                }
                if (vw_LL.Lesson != null && vw_LL.Lesson != "" && vw_LL.Lesson.ToString() == "5")
                {
                    table.Lesson = "第五大节（18:30~21:40）";
                }

                _LL.Add(table);
            }
            return _LL;
        }

        /// <summary>
        /// 统计各类请假数目
        /// </summary>
        /// <param name="leavelist"></param>
        protected void LL_Count(List<vw_LeaveList> leavelist)
        {
            //string grade = Session["Grade"].ToString();

            //检索各类请假条数
            int totalNum = 0;
            int shortNum = 0;
            int longNum = 0;
            int holidayNum = 0;
            int nightNum = 0;
            int classNum = 0;

            //List 转换为 DataTable
            DataTable dtSource = new DataTable();
            dtSource = leavelist.ToDataTable(rec => new object[] { leavelist });

            #region 更改DataTable中某一列的属性
            DataTable dtClone = new DataTable();
            dtClone = dtSource.Clone();
            foreach (DataColumn col in dtClone.Columns)
            {
                if (col.ColumnName == "SubmitTime" || col.ColumnName == "TimeLeave" || col.ColumnName == "TimeBack")
                {
                    col.DataType = typeof(string);
                }
                if (col.ColumnName == "Lesson")
                {
                    col.DataType = typeof(string);
                }
            }

            DataColumn newCol = new DataColumn();
            newCol.ColumnName = "auditState";
            newCol.DataType = typeof(string);
            dtClone.Columns.Add(newCol);

            foreach (DataRow row in dtSource.Rows)
            {
                DataRow rowNew = dtClone.NewRow();
                rowNew["ID"] = row["ID"];
                rowNew["Reason"] = row["Reason"];
                rowNew["StateLeave"] = row["StateLeave"];
                rowNew["StateBack"] = row["StateBack"];
                rowNew["Notes"] = row["Notes"];
                rowNew["TypeID"] = row["TypeID"];
                rowNew["SubmitTime"] = ((DateTime)row["SubmitTime"]).ToString("yyyy-MM-dd HH:mm:ss");//按指定格式输出
                rowNew["TimeLeave"] = ((DateTime)row["TimeLeave"]).ToString("yyyy-MM-dd HH:mm:ss");
                rowNew["TimeBack"] = ((DateTime)row["TimeBack"]).ToString("yyyy-MM-dd HH:mm:ss");
                rowNew["LeaveWay"] = row["LeaveWay"];
                rowNew["BackWay"] = row["BackWay"];
                rowNew["Address"] = row["Address"];
                rowNew["TypeChildID"] = row["TypeChildID"];
                rowNew["Teacher"] = row["Teacher"];
                rowNew["ST_Name"] = row["ST_Name"];
                rowNew["ST_Tel"] = row["ST_Tel"];
                rowNew["ST_Grade"] = row["ST_Grade"];
                rowNew["ST_Class"] = row["ST_Class"];
                rowNew["ST_Teacher"] = row["ST_Teacher"];
                rowNew["StudentID"] = row["StudentID"];
                rowNew["LeaveType"] = row["LeaveType"];
                rowNew["AuditName"] = row["AuditName"];

                //审核状态属性
                rowNew["auditState"] = "Error";
                if (row["StateLeave"].ToString() == "0" && row["StateBack"].ToString() == "0")
                {
                    rowNew["auditState"] = "待审核";
                }
                if (row["StateLeave"].ToString() == "1" && row["StateBack"].ToString() == "0")
                {
                    rowNew["auditState"] = "待销假";
                }
                if (row["StateLeave"].ToString() == "1" && row["StateBack"].ToString() == "1")
                {
                    rowNew["auditState"] = "已销假";
                }
                if (row["StateLeave"].ToString() == "2" && row["StateBack"].ToString() == "1")
                {
                    rowNew["auditState"] = "已驳回";
                }

                //请假课段属性
                rowNew["Lesson"] = "";
                if (row["Lesson"].ToString() == "1")
                {
                    rowNew["Lesson"] = "第一大节（08:00~09:40）";
                }
                if (row["Lesson"].ToString() == "2")
                {
                    rowNew["Lesson"] = "第二大节（10:10~11:50）";
                }
                if (row["Lesson"].ToString() == "3")
                {
                    rowNew["Lesson"] = "第三大节（14:00~15:40）";
                }
                if (row["Lesson"].ToString() == "4")
                {
                    rowNew["Lesson"] = "第四大节（16:00~17:40）";
                }
                if (row["Lesson"].ToString() == "5")
                {
                    rowNew["Lesson"] = "第五大节（18:30~21:40）";
                }

                dtClone.Rows.Add(rowNew);
            }
            #endregion

            foreach (DataRow row in dtClone.Rows)
            {
                if (row["LeaveType"].ToString() == "短期请假")
                {
                    shortNum++;
                }
                if (row["LeaveType"].ToString() == "长期请假")
                {
                    longNum++;
                }
                if (row["LeaveType"].ToString() == "节假日请假")
                {
                    holidayNum++;
                }
                if (row["LeaveType"].ToString().Substring(0, 3) == "晚点名")
                {
                    nightNum++;
                }
                if (row["LeaveType"].ToString().Substring(0, 2) == "上课")
                {
                    classNum++;
                }
            }
            totalNum = shortNum + longNum + holidayNum + nightNum + classNum;

            ViewBag.totalNumLeave = totalNum;
            ViewBag.shortNumLeave = shortNum;
            ViewBag.longNumLeave = longNum;
            ViewBag.holidayNumLeave = holidayNum;
            ViewBag.nightNumLeave = nightNum;
            ViewBag.classNumLeave = classNum;
        }

        /// <summary>
        /// 统计各类请假数目
        /// </summary>
        /// <param name="leavelist"></param>
        protected void LL_Count(List<LL_Table> leavelist)
        {
            //检索各类请假条数
            int totalNum = 0;
            int shortNum = 0;
            int longNum = 0;
            int holidayNum = 0;
            int nightNum = 0;
            int classNum = 0;

            foreach (LL_Table item in leavelist)
            {
                if (item.LeaveType.ToString() == "短期请假")
                {
                    shortNum++;
                }
                if (item.LeaveType.ToString() == "长期请假")
                {
                    longNum++;
                }
                if (item.LeaveType.ToString() == "节假日请假")
                {
                    holidayNum++;
                }
                if (item.LeaveType.ToString().Substring(0, 3) == "晚点名")
                {
                    nightNum++;
                }
                if (item.LeaveType.ToString().Substring(0, 2) == "上课")
                {
                    classNum++;
                }
            }
            totalNum = shortNum + longNum + holidayNum + nightNum + classNum;

            ViewBag.totalNumLeave = totalNum;
            ViewBag.shortNumLeave = shortNum;
            ViewBag.longNumLeave = longNum;
            ViewBag.holidayNumLeave = holidayNum;
            ViewBag.nightNumLeave = nightNum;
            ViewBag.classNumLeave = classNum;
        }

        /// <summary>
        /// 搜索操作
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchClick()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();

            List<LL_Table> _LL = new List<LL_Table>();

            string search = "";
            if (Request["search"] != null)
            {
                search = Request["search"].ToString();
                ViewBag.SearchText = search;
                List<LL_Table> LL = Get_LL(UserID, RoleID, search);
                foreach (LL_Table item in LL)
                {
                    #region 检索关键字
                    bool flag = false;
                    if (item.Reason != null && item.Reason.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.SubmitTime != null && item.SubmitTime.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.ST_Name != null && item.ST_Name.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.ST_Class != null && item.ST_Class.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.LeaveType != null && item.LeaveType.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.AuditState != null && item.AuditState.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        _LL.Add(item);
                    }
                    #endregion
                }

                //统计各类请假类型的数量
                LL_Count(_LL);

                //返回前台选定的请假类型状态
                if (Request["condition"] == null)
                {
                    ViewBag.condition = "AllLeave";
                }
                else
                {
                    ViewBag.condition = Request["condition"].ToString();
                }

                LeaveListModel model = new LeaveListModel();
                model.TopList = _LL;
                model.TopTotalList = _LL;

                return PartialView("_tablelist", model);
            }
            else
            {
                //List<LL_Table> LL = Get_LL(UserID, RoleID, search);

                ////统计各类请假类型的数量
                //LL_Count(LL);

                //返回前台选定的请假类型状态
                if (Request["condition"] == null)
                {
                    ViewBag.condition = "AllLeave";
                }
                else
                {
                    ViewBag.condition = Request["condition"].ToString();
                }
                return PartialView("_tablelist", Get_LL(UserID, RoleID, search, 30));
            }
        }
        #endregion

        #region 随着页面滚动加载数据 -- 尚未完成
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ReLoadNext()
        {
            string leavetype = "";
            string index = "";
            string num = "";
            if (Request["leavetype"] == null)
            {
                leavetype = Request["leavetype"].ToString();
            }
            if (Request["index"] == null)
            {
                index = Request["index"].ToString();
            }
            if (Request["num"] == null)
            {
                num = Request["num"].ToString();
            }
            return LoadNextData(leavetype, index, num);
        }

        /// <summary>
        /// 加载后续数据
        /// </summary>
        /// <param name="leavetype">当前加载数据的请假类型</param>
        /// <param name="index">加载起始编号</param>
        /// <param name="num">加载数目</param>
        /// <returns></returns>
        protected ActionResult LoadNextData(string leavetype, string index, string num)
        {
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            int _index = Convert.ToInt32(index);
            int _num = Convert.ToInt32(num);

            #region 转换请假类型
            string _leavetype = "";
            if (leavetype == "AllLeave")
            {
                _leavetype = "";
            }
            if (leavetype == "ShortLeave")
            {
                _leavetype = "短期请假";
            }
            if (leavetype == "LongLeave")
            {
                _leavetype = "长期请假";
            }
            if (leavetype == "VacationLeave")
            {
                _leavetype = "节假日请假";
            }
            if (leavetype == "NightLeave")
            {
                _leavetype = "晚点名请假";

            }
            if (leavetype == "LessonLeave")
            {
                _leavetype = "上课请假";
            }
            #endregion

            ViewBag.LeaveType = leavetype;

            #region 获取数据
            if (RoleID == "1")//学生
            {
                if (_leavetype == "")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID) orderby vw_LeaveList.ID descending select vw_LeaveList;
                    LL.Skip(_index - 1).Take(_num);
                    return PartialView("_trlist", changeLLModel(LL.ToList()));
                }
                else
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID && vw_LeaveList.LeaveType.StartsWith(_leavetype)) orderby vw_LeaveList.ID descending select vw_LeaveList;
                    LL.Skip(_index - 1).Take(_num);
                    return PartialView("_trlist", changeLLModel(LL.ToList()));
                }
            }
            else if (RoleID == "2")//班级
            {
                //获取班级账号名称
                string className = Session["UserName"].ToString();

                if (_leavetype == "")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_Class == className) orderby vw_LeaveList.ID descending select vw_LeaveList;
                    LL.Skip(_index - 1).Take(_num);
                    return PartialView("_trlist", changeLLModel(LL.ToList()));
                }
                else
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_Class == className && vw_LeaveList.LeaveType.StartsWith(_leavetype)) orderby vw_LeaveList.ID descending select vw_LeaveList;
                    LL.Skip(_index - 1).Take(_num);
                    return PartialView("_trlist", changeLLModel(LL.ToList()));
                }
            }
            else if (RoleID == "3")//辅导员
            {
                if (_leavetype == "")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID) orderby vw_LeaveList.ID descending select vw_LeaveList;
                    LL.Skip(_index - 1).Take(_num);
                    return PartialView("_trlist", changeLLModel(LL.ToList()));
                }
                else
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.LeaveType.StartsWith(_leavetype)) orderby vw_LeaveList.ID descending select vw_LeaveList;
                    LL.Skip(_index - 1).Take(_num);
                    return PartialView("_trlist", changeLLModel(LL.ToList()));
                }
            }
            else
            {
                //未知错误
                return null;
            }
            #endregion
        }
        #endregion

        #region 修改晚点名批次
        public ActionResult changebatch()
        {
            changebatch_LoadData();
            return View();
        }

        public class Batchlist
        {
            private string _id;
            private string _name;
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            public Batchlist(string id, string name)
            {
                _id = id;
                _name = name;
            }
        }
        public void changebatch_LoadData()
        {
            string ST_Num = Session["UserID"].ToString();
            string ST_Class = "";
            string ST_Teacher = "";
            var ST_Info = from vw_Student in db.vw_Student where (vw_Student.ST_Num == ST_Num) select vw_Student;
            if (ST_Info.Any())
            {
                vw_Student stu = ST_Info.ToList().First();
                ViewData["LL_ST_Info"] = stu;
                ST_Class = stu.ST_Class;
                ST_Teacher = stu.ST_TeacherID;
            }
            //所在班级的晚点名批次
            var C_Batch = from vw_ClassBatch in db.vw_ClassBatch where (vw_ClassBatch.ClassName == ST_Class) select vw_ClassBatch;
            int batch = 0;
            if (C_Batch.Any())
            {
                vw_ClassBatch cb = C_Batch.ToList().First();
                //string date = string.Format("{0: yyyy-MM-dd HH:mm:ss}", cb.Datetime);
                DateTime date = Convert.ToDateTime(cb.Datetime);
                //学生修改批次
                var changeBatch = from T_ChangeBatch in db.T_ChangeBatch
                                  where T_ChangeBatch.StudentID == ST_Num && T_ChangeBatch.AuditState == "1" && DbFunctions.TruncateTime(T_ChangeBatch.Datetime) == date.Date
                                  select T_ChangeBatch;
                //DateTime Date = Convert.ToDateTime(changeBatch.ToList().Last().Datetime);
                if (changeBatch.Any())
                {
                    var change = changeBatch.ToList().First();
                    batch = (int)change.Batch;
                    ViewBag.Batch = getBatch(Convert.ToInt16(change.Batch));
                    ViewBag.BatchTime = string.Format("{0: yyyy-MM-dd HH:mm:ss}", change.Datetime).Substring(12, 5);
                    ViewBag.BatchDate = string.Format("{0: yyyy-MM-dd HH:mm:ss}", date).Substring(0, 11);
                }
                else
                {
                    batch = cb.Batch;
                    ViewBag.Batch = getBatch(Convert.ToInt16(cb.Batch));
                    ViewBag.BatchTime = string.Format("{0: yyyy-MM-dd HH:mm:ss}", date).Substring(12, 5);
                    ViewBag.BatchDate = string.Format("{0: yyyy-MM-dd HH:mm:ss}", date).Substring(0, 11);
                }

            }
            //可选晚点名批次列表
            var Batch = from T_Batch in db.T_Batch where T_Batch.TeacherID == ST_Teacher && T_Batch.Batch != batch orderby T_Batch.Batch select T_Batch;
            List<Batchlist> batchlist = new List<Batchlist>();
            foreach (T_Batch b in Batch)
            {
                string key = b.ID.ToString();
                StringBuilder value = new StringBuilder();
                value.Append(getBatch(Convert.ToInt16(b.Batch)));
                value.Append("（");
                value.Append(string.Format("{0: yyyy-MM-dd HH:mm:ss}", b.Datetime).Substring(12, 5));
                if (b.Location != null)
                {
                    value.Append(",");
                    value.Append(b.Location);
                }
                value.Append("）");
                batchlist.Add(new Batchlist(key, value.ToString()));
            }
            ViewBag.BatchList = batchlist;

        }
        public string getBatch(int batch)
        {
            string str = "";
            switch (batch)
            {
                case 1:
                    {
                        str = "第一批次";
                        break;
                    }
                case 2:
                    {
                        str = "第二批次";
                        break;
                    }
                case 3:
                    {
                        str = "第三批次";
                        break;
                    }
                default:
                    break;
            }
            return str;
        }
        public int getBatchNum(string batch)
        {
            int num = 0;
            switch (batch)
            {
                case "第一批次":
                    {
                        num = 1;
                        break;
                    }
                case "第二批次":
                    {
                        num = 2;
                        break;
                    }
                case "第三批次":
                    {
                        num = 3;
                        break;
                    }
                default:
                    break;
            }
            return num;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changebatch_btnSubmit_Click(FormCollection formInfo)
        {
            string ST_Num = Session["UserID"].ToString();
            string batchid = formInfo["Batchlist"];
            string reason = formInfo["Reason"];
            string datetime = formInfo["Date"];
            DateTime date = Convert.ToDateTime(datetime);
            var selectBatch = from T_Batch in db.T_Batch where (T_Batch.ID == new System.Guid(batchid)) select T_Batch;
            var stu = from vw_Student in db.vw_Student where (vw_Student.ST_Num == ST_Num) select vw_Student;
            if (selectBatch.Any())
            {
                T_Batch b = selectBatch.ToList().First();
                T_ChangeBatch cb;
                var stucb = from T_ChangeBatch in db.T_ChangeBatch where T_ChangeBatch.StudentID == ST_Num && DbFunctions.TruncateTime(T_ChangeBatch.Datetime) == date.Date select T_ChangeBatch;
                if (stucb.Any())
                {
                    cb = stucb.ToList().First();
                    cb.Batch = b.Batch;
                    cb.Datetime = b.Datetime;
                    cb.Reason = reason;
                    cb.TeacherID = stu.ToList().First().ST_TeacherID;
                    cb.SubmitTime = DateTime.Now;
                    cb.AuditState = "0";
                }
                else
                {
                    cb = new T_ChangeBatch();
                    cb.ID = System.Guid.NewGuid().ToString();
                    cb.StudentID = ST_Num;
                    cb.Batch = b.Batch;
                    cb.Datetime = b.Datetime;
                    cb.Reason = reason;
                    cb.TeacherID = stu.ToList().First().ST_TeacherID;
                    cb.SubmitTime = DateTime.Now;
                    cb.AuditState = "0";
                    db.T_ChangeBatch.Add(cb);
                }
                db.SaveChanges();
            }
            string script = String.Format("alert('请假申请成功');");
            PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference() +
                script);

            return UIHelper.Result();
        }

        #endregion

        #region 请假详情
        public ActionResult leavedetail()
        {
            string id = Request["ID"].ToString();
            List<vw_LeaveList> Info = db.vw_LeaveList.Where(ll => ll.ID == id).ToList();

            return View(Info);
        }

        #endregion

        public ActionResult deleteClick()
        {
            string LL_Num = "";//请假单号
            string condition = "";//前台请假类型状态
            if (Request["LL_id"] != null)
            {
                LL_Num = Request["LL_id"].ToString();
            }
            if (Request["LeaveType"] != null)
            {
                condition = Request["LeaveType"].ToString();
            }
            T_LeaveList leave = db.T_LeaveList.Find(LL_Num);
            db.T_LeaveList.Remove(leave);
            db.SaveChanges();

            return GetTable(condition);
        }
    }
}