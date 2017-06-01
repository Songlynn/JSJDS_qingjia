using qingjia_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace qingjia_MVC.Controllers
{
    public class BatchModel
    {
        public string batchName { get; set; }
        public string batchTime { get; set; }
        public string batchLoacation { get; set; }
        public List<string> batchClass { get; set; }
    }

    public class VacationModel
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DeadLine { get; set; }
    }

    public class LL_Count
    {
        public int leaveShortNum { get; set; }
        public int leaveLongNum { get; set; }
        public int leaveHolidayNum { get; set; }
        public int leaveNightNum { get; set; }
        public int leaveClassNum { get; set; }
        public int backShortNum { get; set; }
        public int backLongNum { get; set; }
        public int backHolidayNum { get; set; }
        public int backNightNum { get; set; }
    }

    public class MessageModel
    {
        public string title { get; set; }
        public string content { get; set; }
    }

    public class TeacherJsonModel
    {
        public string teacherID { get; set; }
        public string teacherName { get; set; }
        public string teacherSex { get; set; }
        public string teacherGrade { get; set; }
        public string teacherTel { get; set; }
        public string teacherEmail { get; set; }
    }


    public class HomeController : BaseController
    {
        //实例化数据库
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        // GET: Home
        public ActionResult Index()
        {
            HomeLoadData();
            return View();
        }

        protected void HomeLoadData()
        {
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            ViewBag.RoleID = RoleID;
            switch (RoleID)
            {
                case "0":
                    {
                        ViewBag.IFrameURL = "~/Home/Default_M";
                        ViewBag.UserName = "管理员";
                        break;
                    }
                case "1":
                    {
                        //学生首页
                        ViewBag.IFrameURL = "~/Home/Default_ST";
                        //菜单栏
                        //ViewBag.MenuID = "menu_stu";

                        var UserName = from T_Student in db.T_Student where (T_Student.ID == UserID) select T_Student.Name;
                        ViewBag.UserName = "学生-" + UserName.ToList().First().ToString();

                        break;
                    }
                case "2":
                    {
                        //班级账号首页、尚未完成

                        break;
                    }
                case "3":
                    {
                        //辅导员首页
                        ViewBag.IFrameURL = "~/Home/Default_T";
                        //菜单栏
                        //ViewBag.MenuID = "menu_teacher";

                        var UserName = from T_Teacher in db.T_Teacher where (T_Teacher.ID == UserID) select T_Teacher.Name;
                        ViewBag.UserName = "辅导员-" + UserName.ToList().First().ToString();

                        break;
                    }
                default:
                    break;
            }
        }

        public ActionResult Default_ST()
        {
            return View();
        }

        public ActionResult Default_T()
        {
            return View();
        }

        public ActionResult Default_M()
        {
            return View();
        }

        public ActionResult M_Info()
        {
            var teacherlist = from t in db.T_Teacher select t;
            return PartialView(teacherlist.ToList());
        }

        public ActionResult ST_UserInfo()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            string Grade = Session["Grade"].ToString();

            #region basic
            vw_Student studentmodel = (from vw_Student in db.vw_Student where (vw_Student.ST_Num == UserID) select vw_Student).ToList().First();
            ViewBag.BasicName = studentmodel.ST_Name;
            ViewBag.BasicClass = studentmodel.ST_Class;
            ViewBag.BasicGrade = Grade + "级";
            ViewBag.BasicNum = studentmodel.ST_Num;
            ViewBag.BasicTeacher = studentmodel.ST_Teacher;

            var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID) select vw_LeaveList;
            if (LL.Any())
            {
                int total = LL.Count();
                int leave = 0;
                int back = 0;
                foreach (vw_LeaveList llModel in LL)
                {
                    if (llModel.StateLeave == "1" && llModel.StateBack == "0")
                    {
                        back++;
                    }
                    if (llModel.StateLeave == "0" && llModel.StateBack == "0")
                    {
                        leave++;
                    }
                }
                ViewBag.LeaveTotal = total;
                ViewBag.LeaveLeave = leave;
                ViewBag.LeaveBack = back;
            }
            else
            {
                ViewBag.LeaveTotal = 0;
                ViewBag.LeaveLeave = 0;
                ViewBag.LeaveBack = 0;
            }
            #endregion

            return PartialView("_ST_UserInfo");
        }

        public ActionResult ST_NightInfo()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            string Grade = Session["Grade"].ToString();

            vw_Student studentmodel = (from vw_Student in db.vw_Student where (vw_Student.ST_Num == UserID) select vw_Student).ToList().First();

            var batchList = from vw_ClassBatch in db.vw_ClassBatch where (vw_ClassBatch.TeacherID == studentmodel.ST_TeacherID && vw_ClassBatch.Batch != null) select vw_ClassBatch;

            if (batchList.Any())
            {
                foreach (vw_ClassBatch batch in batchList)
                {
                    if (batch.ClassName == studentmodel.ST_Class)
                    {
                        ViewBag.NightBatch = batch.Batch;
                    }
                }
            }
            else
            {
                ViewBag.NightBatch = 0;
            }

            List<BatchModel> BatchModelList = new List<BatchModel>();

            BatchModel BatchModel_01 = new BatchModel();
            BatchModel_01.batchClass = new List<string>();
            BatchModel BatchModel_02 = new BatchModel();
            BatchModel_02.batchClass = new List<string>();
            BatchModel BatchModel_03 = new BatchModel();
            BatchModel_03.batchClass = new List<string>();
            foreach (vw_ClassBatch batch in batchList)
            {
                if (batch.Batch == 1)
                {
                    BatchModel_01.batchName = "1";
                    BatchModel_01.batchTime = ((DateTime)batch.Datetime).ToString("yyyy-MM-dd HH:mm:ss");
                    BatchModel_01.batchLoacation = batch.Location;
                    string className = batch.ClassName + " ";
                    BatchModel_01.batchClass.Add(className);
                }
                if (batch.Batch == 2)
                {
                    BatchModel_02.batchName = "2";
                    BatchModel_02.batchTime = ((DateTime)batch.Datetime).ToString("yyyy-MM-dd HH:mm:ss");
                    BatchModel_02.batchLoacation = batch.Location;
                    string className = batch.ClassName + " ";
                    BatchModel_02.batchClass.Add(className);
                }
                if (batch.Batch == 3)
                {
                    BatchModel_03.batchName = "3";
                    BatchModel_03.batchTime = ((DateTime)batch.Datetime).ToString("yyyy-MM-dd HH:mm:ss");
                    BatchModel_03.batchLoacation = batch.Location;
                    string className = batch.ClassName + " ";
                    BatchModel_03.batchClass.Add(className);
                }
            }
            if (BatchModel_01.batchClass.Count() != 0)
            {
                BatchModelList.Add(BatchModel_01);
            }
            if (BatchModel_02.batchClass.Count() != 0)
            {
                BatchModelList.Add(BatchModel_02);
            }
            if (BatchModel_03.batchClass.Count() != 0)
            {
                BatchModelList.Add(BatchModel_03);
            }

            DateTime deadLine = (from T_Deadline in db.T_Deadline where (T_Deadline.TeacherID == studentmodel.ST_TeacherID && T_Deadline.TypeID == 2) select T_Deadline.Time).ToList().First();
            ViewBag.NightDeadLine = deadLine.ToString();
            ViewBag.NightTeacher = studentmodel.ST_Teacher;
            return PartialView("_ST_NightInfo", BatchModelList);
        }

        public ActionResult T_NightInfo()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            string Grade = Session["Grade"].ToString();
            string UserName = Session["UserName"].ToString();

            var batchList = from vw_ClassBatch in db.vw_ClassBatch where (vw_ClassBatch.TeacherID == UserID && vw_ClassBatch.Batch != null) select vw_ClassBatch;

            List<BatchModel> BatchModelList = new List<BatchModel>();

            BatchModel BatchModel_01 = new BatchModel();
            BatchModel_01.batchClass = new List<string>();
            BatchModel BatchModel_02 = new BatchModel();
            BatchModel_02.batchClass = new List<string>();
            BatchModel BatchModel_03 = new BatchModel();
            BatchModel_03.batchClass = new List<string>();
            foreach (vw_ClassBatch batch in batchList)
            {
                if (batch.Batch == 1)
                {
                    BatchModel_01.batchName = "1";
                    BatchModel_01.batchTime = ((DateTime)batch.Datetime).ToString("yyyy-MM-dd HH:mm:ss");
                    BatchModel_01.batchLoacation = batch.Location;
                    string className = batch.ClassName + " ";
                    BatchModel_01.batchClass.Add(className);
                }
                if (batch.Batch == 2)
                {
                    BatchModel_02.batchName = "2";
                    BatchModel_02.batchTime = ((DateTime)batch.Datetime).ToString("yyyy-MM-dd HH:mm:ss");
                    BatchModel_02.batchLoacation = batch.Location;
                    string className = batch.ClassName + " ";
                    BatchModel_02.batchClass.Add(className);
                }
                if (batch.Batch == 3)
                {
                    BatchModel_03.batchName = "3";
                    BatchModel_03.batchTime = ((DateTime)batch.Datetime).ToString("yyyy-MM-dd HH:mm:ss");
                    BatchModel_03.batchLoacation = batch.Location;
                    string className = batch.ClassName + " ";
                    BatchModel_03.batchClass.Add(className);
                }
            }
            if (BatchModel_01.batchClass.Count() != 0)
            {
                BatchModelList.Add(BatchModel_01);
            }
            if (BatchModel_02.batchClass.Count() != 0)
            {
                BatchModelList.Add(BatchModel_02);
            }
            if (BatchModel_03.batchClass.Count() != 0)
            {
                BatchModelList.Add(BatchModel_03);
            }

            DateTime deadLine = (from T_Deadline in db.T_Deadline where (T_Deadline.TeacherID == UserID && T_Deadline.TypeID == 2) select T_Deadline.Time).ToList().First();
            ViewBag.NightDeadLine = deadLine.ToString();
            ViewBag.NightTeacher = UserName;
            return PartialView("_T_NightInfo", BatchModelList);
        }

        public ActionResult LeaveNum()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            string Grade = Session["Grade"].ToString();

            var LL_Leave = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.StateLeave == "0" && vw_LeaveList.StateBack == "0" && vw_LeaveList.ST_Grade == Grade) select vw_LeaveList;
            var LL_Back = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.StateLeave == "1" && vw_LeaveList.StateBack == "0" && vw_LeaveList.ST_Grade == Grade) select vw_LeaveList;


            #region 计数

            int leaveShortNum = 0;
            int leaveLongNum = 0;
            int leaveHolidayNum = 0;
            int leaveNightNum = 0;
            int leaveClassNum = 0;

            int backShortNum = 0;
            int backLongNum = 0;
            int backHolidayNum = 0;
            int backNightNum = 0;

            foreach (vw_LeaveList ll in LL_Leave)
            {
                if (ll.LeaveType.ToString().Substring(0, 4) == "短期请假")
                {
                    leaveShortNum++;
                }
                if (ll.LeaveType.ToString().Substring(0, 4) == "长期请假")
                {
                    leaveLongNum++;
                }
                if (ll.LeaveType.ToString().Substring(0, 4) == "节假日请")
                {
                    leaveHolidayNum++;
                }
                if (ll.LeaveType.ToString().Substring(0, 4) == "晚点名请")
                {
                    leaveNightNum++;
                }
                if (ll.LeaveType.ToString().Substring(0, 4) == "上课请假")
                {
                    leaveClassNum++;
                }
            }
            foreach (vw_LeaveList ll in LL_Back)
            {
                if (ll.LeaveType.ToString().Substring(0, 4) == "短期请假")
                {
                    backShortNum++;
                }
                if (ll.LeaveType.ToString().Substring(0, 4) == "长期请假")
                {
                    backLongNum++;
                }
                if (ll.LeaveType.ToString().Substring(0, 4) == "节假日请")
                {
                    backHolidayNum++;
                }
                if (ll.LeaveType.ToString().Substring(0, 4) == "晚点名请")
                {
                    backNightNum++;
                }
            }

            ViewBag.leaveShortNum = leaveShortNum;
            ViewBag.leaveLongNum = leaveLongNum;
            ViewBag.leaveHolidayNum = leaveHolidayNum;
            ViewBag.leaveNightNum = leaveNightNum;
            ViewBag.leaveClassNum = leaveClassNum;
            ViewBag.backShortNum = backShortNum;
            ViewBag.backLongNum = backLongNum;
            ViewBag.backHolidayNum = backHolidayNum;
            ViewBag.backNightNum = backNightNum;

            #endregion

            return PartialView("_LeaveNum");
        }

        public int isChangeBatch()
        {
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            DateTime datetime = (DateTime)((from b in db.vw_ClassBatch
                                            where b.TeacherID == UserID
                                            select b.Datetime)
                          .First());
            DateTime date = Convert.ToDateTime(datetime).Date;
            var changelist = from cb in db.vw_StudenBatch where cb.TeacherID == UserID && DbFunctions.TruncateTime(cb.Datetime) == date && cb.AuditState == "0" select cb;
            int num = changelist.Count();
            return num;
        }

        public ActionResult VacationInfo()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            string Grade = Session["Grade"].ToString();
            ViewBag.isGetVacation = "0";



            if (RoleID == "1")
            {
                vw_Student studentmodel = (from vw_Student in db.vw_Student where (vw_Student.ST_Num == UserID) select vw_Student).ToList().First();
                string teacherID = studentmodel.ST_TeacherID;
                string className = studentmodel.ST_Class;
                T_Class classModel = (T_Class)(from T_Class in db.T_Class where (T_Class.ClassName == className) select T_Class).ToList().First();
                if (classModel.MonitorID == UserID)
                {
                    //具有导出节假日去向权限
                    ViewBag.isGetVacation = "1";
                }
                DateTime deadLine = (from T_Deadline in db.T_Deadline where (T_Deadline.TeacherID == teacherID && T_Deadline.TypeID == 1) select T_Deadline.Time).ToList().First();
                if (deadLine >= DateTime.Now)
                {
                    //有节假日安排
                    T_Vacation vacation = (from T_Vacation in db.T_Vacation where (T_Vacation.TeacherID == teacherID && T_Vacation.Grade == Grade) orderby T_Vacation.ID descending select T_Vacation).ToList().First();
                    ViewBag.vacation = 1;
                    ViewBag.StartTime = ((DateTime)vacation.StartTime).ToString("yyyy-MM-dd HH:mm");
                    ViewBag.EndTime = ((DateTime)vacation.EndTime).ToString("yyyy-MM-dd HH:mm");
                    ViewBag.DeadLine = ((DateTime)deadLine).ToString("yyyy-MM-dd HH:mm");
                }
                else
                {
                    string now = DateTime.Now.ToString("yyyy-MM-dd");
                    DateTime timeNow = Convert.ToDateTime(now);
                    DataTable dtSource = GetVacationData();
                    if (dtSource == null)
                    {
                        ViewBag.vacation = 0;
                        ViewBag.StartTime = "无数据";
                        ViewBag.EndTime = "无数据";
                        ViewBag.DeadLine = "未设置";
                    }
                    else
                    {
                        DataView dataView = dtSource.DefaultView;
                        dataView.Sort = "StartTime asc";
                        dtSource = dataView.ToTable();

                        foreach (DataRow row in dtSource.Rows)
                        {
                            if (Convert.ToDateTime(row["StartTime"].ToString()) >= timeNow)
                            {
                                ViewBag.vacation = 0;
                                ViewBag.StartTime = row["StartTime"].ToString();
                                ViewBag.EndTime = row["EndTime"].ToString();
                                ViewBag.DeadLine = "未设置";
                                break;
                            }
                        }
                    }
                }
                return PartialView("_ST_VacationInfo");
            }
            else if (RoleID == "3")
            {
                //具有导出节假日去向权限
                ViewBag.isGetVacation = "1";

                string teacherID = UserID;
                DateTime deadLine = (from T_Deadline in db.T_Deadline where (T_Deadline.TeacherID == teacherID && T_Deadline.TypeID == 1) select T_Deadline.Time).ToList().First();
                if (deadLine >= DateTime.Now)
                {
                    //有节假日安排
                    T_Vacation vacation = (from T_Vacation in db.T_Vacation where (T_Vacation.TeacherID == teacherID && T_Vacation.Grade == Grade) orderby T_Vacation.ID descending select T_Vacation).ToList().First();
                    ViewBag.vacation = 1;
                    ViewBag.StartTime = ((DateTime)vacation.StartTime).ToString("yyyy-MM-dd HH:mm");
                    ViewBag.EndTime = ((DateTime)vacation.EndTime).ToString("yyyy-MM-dd HH:mm");
                    ViewBag.DeadLine = ((DateTime)deadLine).ToString("yyyy-MM-dd HH:mm");
                }
                else
                {
                    string now = DateTime.Now.ToString("yyyy-MM-dd");
                    DateTime timeNow = Convert.ToDateTime(now);
                    DataTable dtSource = GetVacationData();
                    if (dtSource == null)
                    {
                        ViewBag.vacation = 0;
                        ViewBag.StartTime = "无数据";
                        ViewBag.EndTime = "无数据";
                        ViewBag.DeadLine = "未设置";
                    }
                    else
                    {
                        DataView dataView = dtSource.DefaultView;
                        dataView.Sort = "StartTime asc";
                        dtSource = dataView.ToTable();

                        foreach (DataRow row in dtSource.Rows)
                        {
                            if (Convert.ToDateTime(row["StartTime"].ToString()) >= timeNow)
                            {
                                ViewBag.vacation = 0;
                                ViewBag.StartTime = row["StartTime"].ToString();
                                ViewBag.EndTime = row["EndTime"].ToString();
                                ViewBag.DeadLine = "未设置";
                                break;
                            }
                        }
                    }
                }
                return PartialView("_ST_VacationInfo");
            }
            else
            {
                return null;
            }
        }

        public DataTable GetVacationData()
        {
            string fileName = "VacationData.xlsx";
            return qingjia_MVC.Common.LoadVacationData.ImportExcelToDataTable(fileName);
        }

        public JsonResult GetMessageInfo()
        {
            string type = Request["type"].ToString();

            if (Session["RoleID"].ToString() != "3")
            {
                return null;
            }
            string UserID = Session["UserID"].ToString();
            string Grade = Session["Grade"].ToString();
            string RoleID = Session["RoleID"].ToString();

            JsonResult result = new JsonResult();

            if (type == "notice")
            {
                List<MessageModel> messageList = new List<MessageModel>();

                #region 节假日信息查询
                DateTime deadLine = (DateTime)((from T_Deadline in db.T_Deadline where (T_Deadline.TeacherID == UserID && T_Deadline.TypeID == 1) select T_Deadline.Time).ToList().First());
                if (deadLine < DateTime.Now)
                {
                    string now = DateTime.Now.ToString("yyyy-MM-dd");
                    DateTime timeNow = Convert.ToDateTime(now);
                    DataTable dtSource = GetVacationData();

                    if (dtSource == null)
                    {
                        //返回信息：尚未设置节假日安排
                        MessageModel model = new MessageModel();
                        model.title = "节假日提醒：</br>";
                        model.content = "尚未导入节假日信息安排表！</br>";
                        messageList.Add(model);
                    }
                    else
                    {
                        DataView dataView = dtSource.DefaultView;
                        dataView.Sort = "StartTime asc";
                        dtSource = dataView.ToTable();

                        foreach (DataRow row in dtSource.Rows)
                        {
                            if (Convert.ToDateTime(row["StartTime"].ToString()) >= timeNow)
                            {
                                string startTime = row["StartTime"].ToString();
                                string endTime = row["EndTime"].ToString();
                                //返回下次节假日安排的时间
                                DateTime nextStartTime = Convert.ToDateTime(startTime);
                                TimeSpan ts = nextStartTime - DateTime.Now;
                                int days = ts.Days;
                                if (days <= 7)
                                {
                                    //智能提示下次节假日请假的时间不足七天 是否提前社会去向统计事宜
                                    MessageModel model = new MessageModel();
                                    model.title = "节假日智能提醒：</br>";
                                    model.content = "距离下次节假日安排以不足七天！需设置节假日去向统计事宜！</br>";
                                    messageList.Add(model);
                                }
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region 晚点名信息查询
                DateTime nightDeadLine = (DateTime)((from T_Deadline in db.T_Deadline where (T_Deadline.TeacherID == UserID && T_Deadline.TypeID == 2) select T_Deadline.Time).ToList().First());
                if (nightDeadLine <= DateTime.Now && nightDeadLine.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    //已过晚点名时间，建议导出晚点名名单
                    MessageModel model = new MessageModel();
                    model.title = "晚点名提醒：</br>";
                    model.content = "晚点名请假统计已结束！可以导出晚点名请假情况名单！</br>";
                    messageList.Add(model);
                }
                #endregion

                #region 请假情况查询
                var LL_List = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.StateBack == "0") select vw_LeaveList;
                if (LL_List.Any())
                {
                    int num = LL_List.Count();
                    //已过晚点名时间，建议导出晚点名名单
                    MessageModel model = new MessageModel();
                    model.title = "请假销假提醒：：</br>";
                    model.content = "当前有！" + num + "条请假记录需处理！</br>";
                    messageList.Add(model);
                }
                #endregion

                result.Data = messageList;
            }
            if (type == "message")
            {
                result.Data = "1";
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;//允许使用GET方式获取，否则用GET获取是会报错
            return result;
        }

        #region 管理员页面
        public ActionResult addTeacher()
        {
            return View("addTeacher");
        }

        public string btnAddClick()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var teacherJsonModel = js.Deserialize<TeacherJsonModel>(stream);

            string ID = teacherJsonModel.teacherID.ToString();
            T_Teacher teacher = db.T_Teacher.Find(ID);
            if (teacher != null)
            {
                return "2";//账号已存在
            }
            else
            {
                T_Teacher teacherModel = new T_Teacher();
                teacherModel.ID = teacherJsonModel.teacherID;
                teacherModel.Name = teacherJsonModel.teacherName;
                teacherModel.Sex = teacherJsonModel.teacherSex;
                teacherModel.Grade = teacherJsonModel.teacherGrade.ToString().Substring(0,4);
                teacherModel.Tel = teacherJsonModel.teacherTel;
                teacherModel.Email = teacherJsonModel.teacherEmail;
                db.T_Teacher.Add(teacherModel);
                if (db.SaveChanges() == 1)
                {
                    return "1";//插入成功
                }
                return "0";//插入失败
            }
        }

        public int btnDeleteClick()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var teacherJsonModel = js.Deserialize<List<string>>(stream);

            foreach (string teacherID in teacherJsonModel)
            {
                T_Teacher teacher = db.T_Teacher.Find(teacherID);
                if (teacher != null)
                {
                    db.T_Teacher.Remove(teacher);
                }
            }
            return db.SaveChanges();
        }

        public JsonResult GetTeacherInfo()
        {
            var result = new JsonResult();

            string teacherID = Request["teacherID"].ToString();
            T_Teacher teacher = db.T_Teacher.Find(teacherID);

            teacher.ID = ((teacher.ID == null) ? "" : (teacher.ID == "NULL") ? "" : teacher.ID);
            teacher.Name = ((teacher.Name == null) ? "" : (teacher.Name == "NULL") ? "" : teacher.Name);
            teacher.Grade = ((teacher.Grade == null) ? "" : (teacher.Grade == "NULL") ? "" : teacher.Grade);
            teacher.Tel = ((teacher.Tel == null) ? "" : (teacher.Tel == "NULL") ? "" : teacher.Tel);
            teacher.Email = ((teacher.Email == null) ? "" : (teacher.Email == "NULL") ? "" : teacher.Email);
            teacher.Sex = ((teacher.Sex == null) ? "" : (teacher.Sex == "NULL") ? "" : teacher.Sex);

            result.Data = teacher;
            return result;
        }

        public ActionResult editTeacher()
        {
            string teacherID = Request["teacherID"].ToString();
            string teacherName = Request["teacherName"].ToString();
            string teacherTel = Request["teacherTel"].ToString();
            string teacherGrade = Request["teacherGrade"].ToString();
            string teacherEmail = Request["teacherEmail"].ToString();
            string teacherSex = Request["teacherSex"].ToString();

            ViewBag.teacherID = teacherID;
            ViewBag.teacherName = teacherName;
            ViewBag.teacherTel = teacherTel;
            ViewBag.teacherGrade = teacherGrade.ToString().Trim() + "级";
            ViewBag.teacherEmail = teacherEmail;
            ViewBag.teacherSex = teacherSex;
            ViewBag.Edit = "1";

            return addTeacher();
        }

        public string btnEditClick()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var teacherJsonModel = js.Deserialize<TeacherJsonModel>(stream);

            string ID = teacherJsonModel.teacherID.ToString();
            T_Teacher teacher = db.T_Teacher.Find(ID);
            if (teacher != null)
            {
                teacher.Name = teacherJsonModel.teacherName;
                teacher.Sex = teacherJsonModel.teacherSex;
                teacher.Grade = teacherJsonModel.teacherGrade.ToString().Substring(0, 4);
                teacher.Tel = teacherJsonModel.teacherTel;
                teacher.Email = teacherJsonModel.teacherEmail;

                if (db.SaveChanges() == 1)
                {
                    return "1";//插入成功
                }
                return "0";//插入失败
            }
            else
            {
                return "2";//插入失败
            }
        }
        #endregion

    }
}