using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qingjia_MVC.Models;
using qingjia_MVC.Controllers;
using FineUIMvc;
using System.Data.Entity.Validation;


namespace qingjia_MVC.Areas.UserInfo.Controllers
{

    public class TeacherDataModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
        public List<string> classList { get; set; }
    }

    public class UserInfoController : BaseController
    {
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        // GET: UserInfo/UserInfo
        public ActionResult Index()
        {
            LoadData_ST(Session["UserID"].ToString());
            return View("personal_ST");
        }

        public ActionResult personal_T()
        {
            return View(LoadData_T(Session["UserID"].ToString()));
        }

        #region 学生端个人信息修改
        protected void LoadData_ST(string ST_NUM)
        {
            string ST_Num = Session["UserID"].ToString();
            vw_Student modelStudent = new vw_Student();
            try
            {
                var vw_stu = from vw_Student in db.vw_Student where (vw_Student.ST_Num == ST_Num) select vw_Student;
                modelStudent = vw_stu.ToList().First();
            }
            catch (DbEntityValidationException dbEx)
            {

            }

            //绑定数据
            //先判断是否存在，再进行复制，防止报错
            ViewBag.ST_NUM = (modelStudent.ST_Num == null) ? "" : modelStudent.ST_Num.ToString();
            ViewBag.ST_Name = (modelStudent.ST_Name == null) ? "" : modelStudent.ST_Name.ToString();
            ViewBag.ST_SEX = (modelStudent.ST_Sex == null) ? "" : modelStudent.ST_Sex.ToString();
            ViewBag.ST_CLASS = (modelStudent.ST_Class == null) ? "" : modelStudent.ST_Class.ToString();
            ViewBag.Teacher = (modelStudent.ST_Teacher == null) ? "" : modelStudent.ST_Teacher.ToString();
            ViewBag.ST_Tel = (modelStudent.ST_Tel == null) ? "" : modelStudent.ST_Tel.ToString();
            ViewBag.ST_QQ = (modelStudent.ST_QQ == null) ? "" : modelStudent.ST_QQ.ToString();
            ViewBag.ST_Email = (modelStudent.ST_Email == null) ? "" : modelStudent.ST_Email.ToString();
            ViewBag.ST_Door = (modelStudent.ST_Dor == null) ? "" : modelStudent.ST_Dor.ToString();

            string teacherID = modelStudent.ST_TeacherID.ToString();
            T_Teacher teacher =  db.T_Teacher.Find(teacherID);
            if (teacher.Tel != null && teacher.Tel != "NULL" && teacher.Tel != "")
            {
                ViewBag.Teacher = (modelStudent.ST_Teacher == null) ? "" : modelStudent.ST_Teacher.ToString() + "-" + teacher.Tel;
            }

            if (modelStudent.ContactOne == null || modelStudent.ContactOne.ToString() == "")
            {
                ViewBag.Relation1 = "";
                ViewBag.RelationName1 = "";
            }
            else
            {
                ViewBag.Relation1 = modelStudent.ContactOne.ToString().Substring(0, 2);
                ViewBag.RelationName1 = modelStudent.ContactOne.ToString().Substring(3, modelStudent.ContactOne.ToString().Length - 3);
            }
            if (modelStudent.ContactTwo == null || modelStudent.ContactTwo.ToString() == "")
            {
                ViewBag.Relation2 = "";
                ViewBag.RelationName2 = "";
            }
            else
            {
                ViewBag.Relation2 = modelStudent.ContactTwo.ToString().Substring(0, 2);
                ViewBag.RelationName2 = modelStudent.ContactTwo.ToString().Substring(3, modelStudent.ContactTwo.ToString().Length - 3);
            }
            if (modelStudent.ContactThree == null || modelStudent.ContactThree.ToString() == "")
            {
                ViewBag.Relation3 = "";
                ViewBag.RelationName3 = "";
            }
            else
            {
                ViewBag.Relation3 = modelStudent.ContactThree.ToString().Substring(0, 2);
                ViewBag.RelationName3 = modelStudent.ContactThree.ToString().Substring(3, modelStudent.ContactThree.ToString().Length - 3);
            }
            ViewBag.RelationTel1 = ((modelStudent.OneTel == "") || (modelStudent.OneTel == null)) ? "" : modelStudent.OneTel.ToString();
            ViewBag.RelationTel2 = ((modelStudent.TwoTel == "") || (modelStudent.TwoTel == null)) ? "" : modelStudent.TwoTel.ToString();
            ViewBag.RelationTel3 = ((modelStudent.ThreeTel == "") || (modelStudent.ThreeTel == null)) ? "" : modelStudent.ThreeTel.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserInfo(string ST_Tel, string ST_QQ, string ST_Email, string ST_ContactName1, string ST_ContactRelation1, string ST_ContactTel1, string ST_ContactName2, string ST_ContactRelation2, string ST_ContactTel2, string ST_ContactName3, string ST_ContactRelation3, string ST_ContactTel3)
        {

            string ST_Num = Session["UserID"].ToString();
            T_Student student = db.T_Student.Find(ST_Num);

            //联系人信息全部为空，说明是从登录界面跳转到信息修改界面，修改完成后跳转回主页面
            if (student.ContactOne == "" || student.ContactOne == null || student.OneTel == "" || student.OneTel == null)
            {
                #region 登陆之后自动跳转，完善个人信息，联系人方式等信息为空
                //缺少验证处理

                if (ST_Tel == "" && ST_QQ == "" && ST_Email == "" && ST_ContactName1 == "" && ST_ContactRelation1 == "" && ST_ContactTel1 == "")
                {
                    LoadData_ST(ST_Num);
                    ShowNotify("未做任何修改！");
                    return View("Index");
                }
                if (ST_Tel != "")
                {
                    student.Tel = ST_Tel.ToString();
                }
                if (ST_QQ != "")
                {
                    student.QQ = ST_QQ.ToString();
                }
                if (ST_Email != "")
                {
                    student.Email = ST_Email.ToString();
                }
                if (ST_ContactName1 != "")
                {
                    //此处需要判断原数据是否为null
                    if (student.ContactOne != null && student.ContactOne.ToString() != "")
                    {
                        student.ContactOne = student.ContactOne.ToString().Substring(0, 3) + ST_ContactName1.ToString();
                    }
                    else
                    {
                        student.ContactOne = ST_ContactName1.ToString();
                    }
                }
                if (ST_ContactRelation1 != "")
                {
                    if (ST_ContactRelation1 != "父亲" && ST_ContactRelation1 != "母亲" && ST_ContactRelation1 != "其他")
                    {
                        LoadData_ST(ST_Num);
                        ShowNotify("联系人关系类型为父亲、母亲或其他，请重新填写！");
                        return View("Index");
                    }
                    else
                    {
                        //检查是否包含“-”字符
                        if (student.ContactOne.Contains("-"))
                        {
                            //联系人信息已存在
                            student.ContactOne = ST_ContactRelation1.ToString() + "-" + student.ContactOne.ToString().Substring(3, student.ContactOne.ToString().Length - 3);
                        }
                        else
                        {
                            //联系人信息不存在
                            student.ContactOne = ST_ContactRelation1.ToString() + "-" + student.ContactOne.ToString();
                        }
                    }
                }
                if (ST_ContactTel1 != "")
                {
                    student.OneTel = ST_ContactTel1.ToString();
                }
                db.SaveChanges();
                LoadData_ST(ST_Num);
                ViewBag.Changed = true;
                ShowNotify("修改成功");
                //return View("Index");
                return RedirectToAction("Index", "Home", new { area = "" });
                #endregion
            }

            //非登陆跳转,则至少包含一个联系人信息
            else
            {
                //联系人一
                #region 修改个人信息，联系人方式等信息为空

                //提取联系人用作比较
                string contact = student.ContactOne.ToString();
                if (ST_Tel == student.Tel && ST_QQ == student.QQ && ST_Email == student.Email && ST_ContactName1 == contact.Substring(3, contact.Length - 3) && ST_ContactRelation1 == contact.Substring(0, 2) && ST_ContactTel1 == student.OneTel)
                {
                    LoadData_ST(ST_Num);
                    ShowNotify("未做任何修改！");
                    return View("personal_ST");
                }
                if (ST_Tel != "")
                {
                    student.Tel = ST_Tel.ToString();
                }
                if (ST_QQ != "")
                {
                    student.QQ = ST_QQ.ToString();
                }
                if (ST_Email != "")
                {
                    student.Email = ST_Email.ToString();
                }
                if (ST_ContactName1 != "")
                {
                    //此处需要判断原数据是否为null
                    if (student.ContactOne != null && student.ContactOne.ToString() != "")
                    {
                        student.ContactOne = student.ContactOne.ToString().Substring(0, 3) + ST_ContactName1.ToString();
                    }
                    else
                    {
                        student.ContactOne = ST_ContactName1.ToString();
                    }
                }
                if (ST_ContactRelation1 != "")
                {
                    if (ST_ContactRelation1 != "父亲" && ST_ContactRelation1 != "母亲" && ST_ContactRelation1 != "其他")
                    {
                        LoadData_ST(ST_Num);
                        ShowNotify("联系人关系类型为父亲、母亲或其他，请重新填写！");
                        return View("personal_ST");
                    }
                    else
                    {
                        //检查是否包含“-”字符
                        if (student.ContactOne.Contains("-"))
                        {
                            //联系人信息已存在
                            student.ContactOne = ST_ContactRelation1.ToString() + "-" + student.ContactOne.ToString().Substring(3, student.ContactOne.ToString().Length - 3);
                        }
                        else
                        {
                            //联系人信息不存在
                            student.ContactOne = ST_ContactRelation1.ToString() + "-" + student.ContactOne.ToString();
                        }
                    }
                }
                if (ST_ContactTel1 != "")
                {
                    student.OneTel = ST_ContactTel1.ToString();
                }
                db.SaveChanges();
                LoadData_ST(ST_Num);
                ViewBag.Changed = true;
                ShowNotify("修改成功");
                return View("personal_ST");
                #endregion

                //联系人二、三
                #region

                #endregion
            }
        }

        #endregion

        #region 辅导员端个人信息修改
        public TeacherDataModel LoadData_T(string UserID)
        {
            TeacherDataModel teacherModel = new TeacherDataModel();
            T_Teacher teacher = db.T_Teacher.Find(UserID);

            string Grade = Session["Grade"].ToString();
            var classList = from T_Class in db.T_Class where (T_Class.TeacherID == UserID && T_Class.Grade == Grade) select T_Class.ClassName;

            teacherModel.ID = teacher.ID;
            teacherModel.Name = teacher.Name;
            teacherModel.Grade = teacher.Grade + "级";
            teacherModel.Tel = teacher.Tel;
            teacherModel.Email = teacher.Email;
            teacherModel.Sex = teacher.Sex;
            if (classList.Any())
            {
                teacherModel.classList = classList.ToList();
            }
            else
            {
                teacherModel = null;
            }

            return teacherModel;
        }

        public string changeInfo_T()
        {
            string UserID = Session["UserID"].ToString();
            string Tel = Request["Tel"].ToString();
            string Email = Request["Email"].ToString();

            try
            {
                T_Teacher teacher = db.T_Teacher.Find(UserID);
                teacher.Tel = Tel;
                teacher.Email = Email;
                db.SaveChanges();
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        #endregion
    }
}