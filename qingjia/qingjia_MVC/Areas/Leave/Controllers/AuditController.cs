using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using qingjia_MVC.Models;
using qingjia_MVC.Controllers;
using qingjia_MVC.Content;
using System.IO;
using System.Web.Script.Serialization;

namespace qingjia_MVC.Areas.Leave.Controllers
{
    /// <summary>
    /// table 数据模型
    /// </summary>
    public class LL_Table
    {
        public string ID { get; set; }
        public string Reason { get; set; }
        public string StateLeave { get; set; }
        public string StateBack { get; set; }
        public string Notes { get; set; }
        public string TypeID { get; set; }
        public string SubmitTime { get; set; }
        public string TimeLeave { get; set; }
        public string TimeBack { get; set; }
        public string LeaveWay { get; set; }
        public string BackWay { get; set; }
        public string Address { get; set; }
        public string TypeChildID { get; set; }
        public string Teacher { get; set; }
        public string ST_Name { get; set; }
        public string ST_Tel { get; set; }
        public string ST_Grade { get; set; }
        public string ST_Class { get; set; }
        public string ST_Teacher { get; set; }
        public string StudentID { get; set; }
        public string LeaveType { get; set; }
        public string AuditName { get; set; }
        public string AuditState { get; set; }
        public string Lesson { get; set; }
        public string Type { get; set; }
    }

    /// <summary>
    /// 请假记录ID json数据格式
    /// </summary>
    public class LL_Num_JsonModel
    {
        public string AudtitType { get; set; }
        public string LL_Num { get; set; }
    }


    public class AuditController : BaseController
    {
        //实例化数据库
        imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        // GET: Leave/Audit
        public ActionResult AuditLeave()
        {
            //login();
            Session["LeaveType"] = "AllLeave";
            Session["AuditType"] = "leave";
            if (Request["LeaveType"] != null)
            {
                Session["LeaveType"] = Request["LeaveType"].ToString();
            }
            return View();
        }

        public ActionResult AuditBack()
        {
            //login();
            Session["LeaveType"] = "AllLeave";
            Session["AuditType"] = "back";
            if (Request["LeaveType"] != null)
            {
                Session["LeaveType"] = Request["LeaveType"].ToString();
            }
            return View();
        }

        /// <summary>
        /// 模拟登陆
        /// </summary>
        protected void login()
        {
            //Session["UserID"] = "1214001";
            //Session["RoleID"] = "3";
            //Session["Grade"] = "2014";
            //Session["UserName"] = "梁导";

            //Session["UserID"] = "14034901";
            //Session["RoleID"] = "034901";
            //Session["Grade"] = "2014";
            //Session["UserName"] = "信管1401";
        }

        public ActionResult GetTable(string condition)
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();

            if (condition == "" || condition == null)
            {
                ViewBag.LeaveType = Session["LeaveType"].ToString();
            }
            else
            {
                ViewBag.LeaveType = condition;
            }

            string AuditType = Session["AuditType"].ToString();
            if (AuditType == "leave")
            {
                return PartialView("_tableleave", Get_LL(UserID, RoleID, condition));
            }
            else if (AuditType == "back")
            {
                return PartialView("_tableback", Get_LL(UserID, RoleID, condition));
            }
            else
            {
                //未知错误
                return null;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="RoleID"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<LL_Table> Get_LL(string UserID, string RoleID, string condition)
        {
            List<vw_LeaveList> LL_List = new List<vw_LeaveList>();

            //请假销假状态
            string AuditType = Session["AuditType"].ToString();

            if (RoleID == "1")//学生
            {
                if (AuditType == "leave")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID && vw_LeaveList.StateLeave == "0" && vw_LeaveList.StateBack == "0") select vw_LeaveList;
                    LL_List = LL.ToList();
                }
                else if (AuditType == "back")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.StudentID == UserID && vw_LeaveList.StateLeave == "1" && vw_LeaveList.StateBack == "0") select vw_LeaveList;
                    LL_List = LL.ToList();
                }
                else
                {
                    //未知错误
                    return null;
                }
            }
            else if (RoleID == "2")//班级
            {
                //获取班级账号名称
                string className = Session["UserName"].ToString();

                if (AuditType == "leave")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_Class == className && vw_LeaveList.StateLeave == "0" && vw_LeaveList.StateBack == "0") select vw_LeaveList;
                    LL_List = LL.ToList();
                }
                else if (AuditType == "back")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_Class == className && vw_LeaveList.StateLeave == "1" && vw_LeaveList.StateBack == "0") select vw_LeaveList;
                    LL_List = LL.ToList();
                }
                else
                {
                    //未知错误
                    return null;
                }
            }
            else if (RoleID == "3")//辅导员
            {
                if (AuditType == "leave")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.StateLeave == "0" && vw_LeaveList.StateBack == "0") select vw_LeaveList;
                    LL_List = LL.ToList();
                }
                else if (AuditType == "back")
                {
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.StateLeave == "1" && vw_LeaveList.StateBack == "0") select vw_LeaveList;
                    LL_List = LL.ToList();
                }
                else
                {
                    //未知错误
                    return null;
                }
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
                table.SubmitTime = ((DateTime)vw_LL.SubmitTime).ToString("yyyy-MM-dd hh:mm:ss");
                table.TimeLeave = ((DateTime)vw_LL.TimeLeave).ToString("yyyy-MM-dd hh:mm:ss");
                table.TimeBack = ((DateTime)vw_LL.TimeBack).ToString("yyyy-MM-dd hh:mm:ss");
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
            string grade = Session["Grade"].ToString();

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
                rowNew["SubmitTime"] = ((DateTime)row["SubmitTime"]).ToString("yyyy-MM-dd hh:mm:ss");//按指定格式输出
                rowNew["TimeLeave"] = ((DateTime)row["TimeLeave"]).ToString("yyyy-MM-dd hh:mm:ss");
                rowNew["TimeBack"] = ((DateTime)row["TimeBack"]).ToString("yyyy-MM-dd hh:mm:ss");
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
        /// 同意请假操作、同意销假操作
        /// </summary>
        /// <returns></returns>
        public ActionResult AgreeCLick()
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
            if (Session["AuditType"].ToString() == "leave")
            {
                T_LeaveList LL = db.T_LeaveList.Find(LL_Num);
                LL.StateLeave = "1";
                db.SaveChanges();
            }
            else if (Session["AuditType"].ToString() == "back")
            {
                T_LeaveList LL = db.T_LeaveList.Find(LL_Num);
                LL.StateBack = "1";
                db.SaveChanges();
            }
            else
            {
                //未知错误
            }

            return GetTable(condition);
        }

        /// <summary>
        /// 批量同意请假、销假
        /// </summary>
        /// <returns></returns>
        public ActionResult AgreeMoreClick()
        {
            string condition = "";
            var result = new JsonResult();
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<List<LL_Num_JsonModel>>(stream);
            if (list.Any())
            {
                int num = list.Count();
                int leaveNum = 0;
                int backNum = 0;

                foreach (LL_Num_JsonModel item in list)
                {
                    T_LeaveList LL_Model = db.T_LeaveList.Find(item.LL_Num);
                    if (item.AudtitType == "Leave")
                    {
                        LL_Model.StateLeave = "1";
                        leaveNum++;
                    }
                    if (item.AudtitType == "Back")
                    {
                        LL_Model.StateBack = "1";
                        backNum++;
                    }
                }

                if (Request["LeaveType"] != null)
                {
                    condition = Request["LeaveType"].ToString();
                }

                try
                {
                    db.SaveChanges();
                    return GetTable(condition);
                }
                catch
                {
                    return GetTable(condition);
                }

            }
            return GetTable(condition);
        }


        /// <summary>
        /// 驳回请假操作
        /// </summary>
        /// <returns></returns>
        public ActionResult CancleClick()
        {

            string LL_Num = "";//请假单号
            string condition = "";//前台请假类型状态
            string Reason = "";//驳回理由
            if (Request["LL_id"] != null)
            {
                LL_Num = Request["LL_id"].ToString();
            }
            if (Request["condition"] != null)
            {
                condition = Request["condition"].ToString();
            }
            if (Request["reason"] != null)
            {
                Reason = Request["reason"].ToString();
            }
            T_LeaveList LL = db.T_LeaveList.Find(LL_Num);
            LL.StateLeave = "2";
            LL.StateBack = "1";
            LL.Notes = Reason;
            db.SaveChanges();
            return GetTable(condition);
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
            string AuditType = Session["AuditType"].ToString();

            List<LL_Table> _LL = new List<LL_Table>();

            string search = "";
            if (Request["search"] != null)
            {
                search = Request["search"].ToString();
                ViewBag.Search = search;
                List<LL_Table> LL = Get_LL(UserID, RoleID, search);
                foreach (LL_Table item in LL)
                {
                    #region 检索关键字
                    bool flag = false;
                    if (item.ID != null && item.ID.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.Reason != null && item.Reason.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.StateLeave != null && item.StateLeave.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.StateBack != null && item.StateBack.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.Notes != null && item.Notes.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.TypeID != null && item.TypeID.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.SubmitTime != null && item.SubmitTime.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.TimeLeave != null && item.TimeLeave.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.TimeBack != null && item.TimeBack.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.LeaveWay != null && item.LeaveWay.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.BackWay != null && item.BackWay.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.Address != null && item.Address.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.TypeChildID != null && item.TypeChildID.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.Teacher != null && item.Teacher.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.ST_Name != null && item.ST_Name.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.ST_Tel != null && item.ST_Tel.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.ST_Grade != null && item.ST_Grade.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.ST_Class != null && item.ST_Class.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.ST_Teacher != null && item.ST_Teacher.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.StudentID != null && item.StudentID.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.LeaveType != null && item.LeaveType.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.AuditName != null && item.AuditName.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.AuditState != null && item.AuditState.ToString().Contains(search))
                    {
                        flag = true;
                    }
                    if (item.Lesson != null && item.Lesson.ToString().Contains(search))
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
                    ViewBag.LeaveType = Session["LeaveType"].ToString();
                }
                else
                {
                    ViewBag.LeaveType = Request["condition"].ToString();
                }


                if (AuditType == "leave")
                {
                    return PartialView("_tableleave", _LL);
                }
                else if (AuditType == "back")
                {
                    return PartialView("_tableback", _LL);
                }
                else
                {
                    //未知错误
                    return null;
                }
            }
            else
            {
                List<LL_Table> LL = Get_LL(UserID, RoleID, search);

                //统计各类请假类型的数量
                LL_Count(LL);

                //返回前台选定的请假类型状态
                if (Request["condition"] == null)
                {
                    ViewBag.LeaveType = "AllLeave";
                }
                else
                {
                    ViewBag.LeaveType = Request["condition"].ToString();
                }


                if (AuditType == "leave")
                {
                    return PartialView("_tableleave", LL);
                }
                else if (AuditType == "back")
                {
                    return PartialView("_tableback", LL);
                }
                else
                {
                    //未知错误
                    return null;
                }
            }
        }
    }
}