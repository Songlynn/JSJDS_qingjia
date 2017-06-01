using FineUIMvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qingjia_MVC.Models;
using System.Web.UI;
using qingjia_MVC.Common;
using qingjia_MVC.Controllers;
using ShortMessage;

namespace qingjia_MVC.Areas.AddressList.Controllers
{
    public class GradeAddressController : BaseController
    {
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        // GET: AddressList/GradeAddress
        public ActionResult Index()
        {
            string RoleID = Session["RoleID"].ToString();
            ViewBag.RoleID = RoleID;
            return View();
        }

        public ActionResult _LiClass()
        {
            //获取当前用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();

            if (RoleID == "3")
            {
                List<T_Class> classList = new List<T_Class>();
                classList = GetClass(UserID);

                if (classList == null)
                {
                    ViewBag.Exist = 0;
                }
                else
                {
                    ViewBag.Exist = 1;
                }
                return PartialView("_LiClass", classList);
            }
            else
            {
                return null;
            }
        }

        public ActionResult _TableClass()
        {
            //获取当前用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            ViewBag.RoleID = RoleID;


            List<vw_Student> ClassInfoList = new List<vw_Student>();
            ClassInfoList = GetClassInfo();

            if (Request["search"] == null)
            {
                ClassInfoList = GetClassInfo();
            }
            else
            {
                ViewBag.SearchText = Request["search"].ToString();
                ClassInfoList = GetClassInfo(Request["search"].ToString());
            }

            if (ClassInfoList == null)
            {
                ViewBag.Exist = 0;
            }
            else
            {
                ViewBag.Exist = 1;
            }

            if (Request["classname"] == null)
            {
                ViewBag.ClassName = "所有学生";
            }
            else
            {
                ViewBag.ClassName = Request["classname"].ToString();
            }

            return PartialView("_TableClass", ClassInfoList);
        }

        protected List<T_Class> GetClass(string UserID)
        {
            List<T_Class> ClassList = new List<T_Class>();

            var classList = from T_Class in db.T_Class where (T_Class.TeacherID == UserID) select T_Class;

            if (classList.Any())
            {
                ClassList = classList.ToList();
                return ClassList;
            }
            else
            {
                return null;
            }
        }

        protected List<vw_Student> GetClassInfo()
        {
            //获取当前用户信息
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();
            string Grade = Session["Grade"].ToString();

            List<vw_Student> classInfo = new List<vw_Student>();

            if (RoleID == "1")//学生
            {
                T_Student student = db.T_Student.Find(UserID);
                string className = student.ClassName;
                var infoList = from vw_Student in db.vw_Student where (vw_Student.ST_Class == className) orderby vw_Student.ST_Class select vw_Student;
                if (infoList.Any())
                {
                    classInfo = infoList.ToList();
                }
                else
                {
                    classInfo = null;
                }
            }
            else if (RoleID == "2")//班级
            {
                string className = Session["UserName"].ToString();
                var infoList = from vw_Student in db.vw_Student where (vw_Student.ST_Class == className) orderby vw_Student.ST_Class select vw_Student;
                if (infoList.Any())
                {
                    classInfo = infoList.ToList();
                }
                else
                {
                    classInfo = null;
                }
            }
            else if (RoleID == "3")//辅导员
            {
                if (Request["classname"] == null)
                {
                    var infoList = from vw_Student in db.vw_Student where (vw_Student.ST_TeacherID == UserID && vw_Student.ST_Grade == Grade) orderby vw_Student.ST_Class select vw_Student;
                    if (infoList.Any())
                    {
                        classInfo = infoList.ToList();
                    }
                    else
                    {
                        classInfo = null;
                    }
                }
                else
                {
                    string classname = Request["classname"].ToString();
                    var infoList = from vw_Student in db.vw_Student where (vw_Student.ST_Class == classname) orderby vw_Student.ST_Class select vw_Student;
                    if (infoList.Any())
                    {
                        classInfo = infoList.ToList();
                    }
                    else
                    {
                        classInfo = null;
                    }
                }
            }
            else
            {
                return null;
            }

            if (classInfo != null)
            {
                foreach (vw_Student student in classInfo)
                {
                    student.ST_Num = (student.ST_Num == null) ? "" : student.ST_Num;
                    student.ST_Name = (student.ST_Name == null) ? "" : student.ST_Name;
                    student.ST_Tel = (student.ST_Tel == null) ? "" : student.ST_Tel;
                    student.ST_Email = (student.ST_Email == null) ? "" : student.ST_Email;
                    student.ST_QQ = (student.ST_QQ == null) ? "" : student.ST_QQ;
                    student.ContactOne = (student.ContactOne == null) ? "" : student.ContactOne;
                    student.OneTel = (student.OneTel == null) ? "" : student.OneTel;
                    student.ContactTwo = (student.ContactTwo == null) ? "" : student.ContactTwo;
                    student.TwoTel = (student.TwoTel == null) ? "" : student.TwoTel;
                    student.ContactThree = (student.ContactThree == null) ? "" : student.ContactThree;
                    student.ThreeTel = (student.ThreeTel == null) ? "" : student.ThreeTel;
                    student.ST_Sex = (student.ST_Sex == null) ? "" : student.ST_Sex;
                    student.ST_Dor = (student.ST_Dor == null) ? "" : student.ST_Dor;
                    student.ST_Class = (student.ST_Class == null) ? "" : student.ST_Class;
                    student.ST_Grade = (student.ST_Grade == null) ? "" : student.ST_Grade;
                    student.MonitorID = (student.MonitorID == null) ? "" : student.MonitorID;
                    student.ST_Teacher = (student.ST_Teacher == null) ? "" : student.ST_Teacher;
                    student.ST_TeacherID = (student.ST_TeacherID == null) ? "" : student.ST_TeacherID;
                }
            }
            return classInfo;
        }

        protected List<vw_Student> GetClassInfo(string search)
        {
            List<vw_Student> list = GetClassInfo();
            List<vw_Student> newlist = new List<vw_Student>();

            if (list == null)
            {
                return null;
            }

            foreach (vw_Student student in list)
            {
                bool flag = false;
                if (student.ST_Num.ToString().Contains(search))
                {
                    flag = true;
                }
                if (student.ST_Name.ToString().Contains(search))
                {
                    flag = true;
                }
                if (student.ST_Class.ToString().Contains(search))
                {
                    flag = true;
                }
                if (student.ST_Sex.ToString().Contains(search))
                {
                    flag = true;
                }
                if (student.ST_Dor.ToString().Contains(search))
                {
                    flag = true;
                }

                if (flag)
                {
                    newlist.Add(student);
                }
            }
            return newlist;
        }

        public string SendMessage()
        {
            string UserID = Session["UserID"].ToString();
            T_Teacher teacher = db.T_Teacher.Find(UserID);

            string text = Request["text"].ToString();
            string tel = Request["tel"].ToString();

            if (ShortMessageClass.SendShortMessage(teacher.Name, text, tel))
            {
                return "1";
            }
            return "0";
        }

    }
}