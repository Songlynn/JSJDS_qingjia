using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using qingjia_MVC.Models;
using System.Web.Script.Serialization;
using System.Data.Entity.Infrastructure;
using qingjia_MVC.Controllers;

namespace qingjia_MVC.Areas.Import.Controllers
{
    public class StudentModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string ClassName { get; set; }
        public string Door { get; set; }
        public string Nation { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string QQ { get; set; }
        public string ContactName { get; set; }
        public string ContactTel { get; set; }
        public string Contact { get; set; }
    }

    public class ClassModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public string TeacherID { get; set; }
        public string TeacherName { get; set; }
        public string MonitorID { get; set; }
        public string MonitorName { get; set; }
        public string Batch { get; set; }
    }

    public class DeleteDataModel
    {
        public string type { get; set; }
        public List<string> data { get; set; }
    }

    public class ReturnJsonModel
    {
        public int successNum { get; set; }
        public int failNum { get; set; }
        public List<StudentModel> FailedInfo { get; set; }
    }

    public class StuList
    {
        public string ID { get; set; }
        public string Name { get; set; }

    }

    public class ImportController : BaseController
    {
        //实例化数据库
        imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        public void Login()
        {
            Session["UserID"] = "1214001";
            Session["Grade"] = "2014";
        }

        // GET: Import/Import/Import_T
        public ActionResult Import_T()
        {
            //Login();
            return View();
        }

        public ActionResult ImportHtml()
        {
            //Login();
            return View("ImportHtml");
        }

        public ActionResult ClassInfo()
        {
            string UserID = Session["UserID"].ToString();
            string Grade = Session["Grade"].ToString();

            List<ClassModel> classModelList = new List<ClassModel>();

            var classList = from vw_Class in db.vw_Class where (vw_Class.TeacherID == UserID && vw_Class.Grade == Grade) select vw_Class;
            if (classList.Any())
            {
                foreach (vw_Class item in classList)
                {
                    ClassModel _class = new ClassModel();
                    _class.ID = item.ID;
                    _class.Name = item.ClassName;
                    _class.Grade = item.Grade;
                    _class.TeacherID = (item.TeacherID != null) ? item.TeacherID : "";
                    _class.TeacherName = (item.TeacherName != null) ? item.TeacherName : "未设定";
                    _class.MonitorID = (item.MonitorID != null) ? item.MonitorID : "";
                    _class.MonitorName = (item.MonitorName != null) ? item.MonitorName : "未设定";
                    _class.Batch = (item.Batch != null) ? "1" : "0";//1代表已经设定晚点名批次   0代表未设定晚点名批次

                    classModelList.Add(_class);
                }
            }
            else
            {
                classModelList = null;
            }

            return PartialView("_ClassInfo", classModelList);
        }

        public ActionResult StudentInfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<List<StudentModel>>(stream);
            if (list != null)
            {
                return PartialView("_StudentInfo", list.ToList());
            }

            string UserID = Session["UserID"].ToString();
            string Grade = Session["Grade"].ToString();

            List<StudentModel> studentModelList = new List<StudentModel>();

            var studentList = from vw_Student in db.vw_Student where (vw_Student.ST_TeacherID == UserID && vw_Student.ST_Grade == Grade) orderby vw_Student.ST_Class select vw_Student;
            if (studentList.Any())
            {
                foreach (vw_Student item in studentList)
                {
                    StudentModel _student = new StudentModel();
                    _student.ID = item.ST_Num;
                    _student.Name = item.ST_Name;
                    _student.ClassName = item.ST_Class;
                    _student.Sex = (item.ST_Sex != null) ? item.ST_Sex : "";

                    studentModelList.Add(_student);
                }
            }
            else
            {
                studentModelList = null;
            }

            return PartialView("_StudentInfo", studentModelList);
        }

        public ActionResult DownLoad()
        {
            //获取图片路径
            string picPath = ConfigurationManager.AppSettings["picPath"].ToString();

            string fullFilePath = picPath + @"\Areas\Import\res\UserInfo.xls";
            FileStream fileStream = new FileStream(fullFilePath, FileMode.Open);
            var fileName = Path.GetFileName(fullFilePath);
            string fileDownLoadName = fileName;
            if (Request.Browser.Browser == "IE")
            {
                fileDownLoadName = Server.UrlPathEncode(fileName);
            }
            return File(fileStream, "application/octet-stream", fileDownLoadName);
        }

        public ActionResult ImportExcel(FormCollection form)
        {
            string UserID = Session["UserID"].ToString();

            string size = System.Web.HttpContext.Current.Request.Files[0].ContentLength.ToString();//文件大小
            string type = System.Web.HttpContext.Current.Request.Files[0].ContentType;//文件类型
            string _name = System.Web.HttpContext.Current.Request.Files[0].FileName;//原文件名
            string name = UserID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + _name.Substring(_name.LastIndexOf("."));//文件名字
            string path = Server.MapPath("~/Areas/Import/res/Import/") + name; //服务器端保存路径
            if (Convert.ToInt32(size) > 2097152)
            {
                ViewBag.msg = "上传失败文件大于2m";
                return View();//上传失败页面
            }

            if (type == "application/vnd.ms-excel" || type == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                HttpPostedFileBase files = Request.Files[0];
                files.SaveAs(path);

                //处理Excel 返回DateTable
                //ImportExcelToDataTable(name);
                return PartialView("_StudentInfo", DateTableToList(ImportExcelToDataTable(name)));
            }
            else
            {
                return null;
            }
        }

        public JsonResult SaveStudentInfo()
        {
            var res = new JsonResult();
            ReturnJsonModel model = new ReturnJsonModel();

            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<List<StudentModel>>(stream);

            int successNum = 0;
            int failedNum = 0;

            List<StudentModel> failedList = new List<StudentModel>();

            foreach (StudentModel studnetmodel in list)
            {
                string studentID = studnetmodel.ID;
                string studentclass = studnetmodel.ClassName;
                T_Student student = db.T_Student.Find(studentID);
                if (student == null)
                {
                    student = new T_Student();
                    student.ID = studnetmodel.ID;
                    student.Name = studnetmodel.Name;
                    student.ClassName = studnetmodel.ClassName;
                    student.Tel = studnetmodel.Tel;
                    student.Email = studnetmodel.Email;
                    student.QQ = studnetmodel.QQ;
                    student.Sex = studnetmodel.Sex;
                    student.Room = studnetmodel.Door;
                    student.ContactOne = studnetmodel.Contact + "-" + studnetmodel.ContactName;
                    student.OneTel = studnetmodel.ContactTel;
                    student.ContactTwo = null;
                    student.ContactThree = null;
                    student.ThreeTel = null;

                    db.T_Student.Add(student);

                    T_Class classModel = (from T_Class in db.T_Class where (T_Class.ClassName == studentclass) select T_Class).ToList().First();
                    classModel.Total = classModel.Total + 1;

                    db.SaveChanges();
                    successNum++;
                }
                else
                {
                    failedList.Add(studnetmodel);
                    failedNum++;
                }
            }

            model.successNum = successNum;
            model.failNum = failedNum;
            model.FailedInfo = failedList;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;//允许使用GET方式获取，否则用GET获取是会报错
            res.Data = model;
            return res;
        }

        public ActionResult Delete()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<List<DeleteDataModel>>(stream);

            DeleteDataModel deleteModel = list.ToList().First();
            if (deleteModel.type == "#Class")
            {
                foreach (string itemID in deleteModel.data)
                {
                    //用存储过程删除班级？
                    T_Class _class = db.T_Class.Find(itemID);
                    db.T_Class.Remove(_class);
                }
                db.SaveChanges();
                return ClassInfo();
            }
            if (deleteModel.type == "#Student")
            {
                foreach (string itemID in deleteModel.data)
                {
                    //用存储过程删除班级？
                    T_Student _student = db.T_Student.Find(itemID);
                    db.T_Student.Remove(_student);
                }
                db.SaveChanges();
                return StudentInfo();
            }
            return null;
        }

        public List<StudentModel> DateTableToList(DataTable dtSource)
        {
            List<StudentModel> list = new List<StudentModel>();

            if (dtSource.Rows.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                StudentModel model = new StudentModel();
                model.ID = dtSource.Rows[i][0].ToString();
                model.Name = dtSource.Rows[i][1].ToString();
                model.Sex = dtSource.Rows[i][2].ToString();
                model.ClassName = dtSource.Rows[i][3].ToString();
                model.Door = dtSource.Rows[i][4].ToString();
                model.Nation = dtSource.Rows[i][5].ToString();
                model.Tel = dtSource.Rows[i][6].ToString();
                model.Email = dtSource.Rows[i][7].ToString();
                model.QQ = dtSource.Rows[i][8].ToString();
                model.ContactTel = dtSource.Rows[i][9].ToString();
                model.ContactName = dtSource.Rows[i][10].ToString();
                model.Contact = dtSource.Rows[i][11].ToString();

                list.Add(model);
            }
            return list;
        }

        public DataTable ImportExcelToDataTable(string fileName)
        {
            //获取图片路径
            string picPath = ConfigurationManager.AppSettings["picPath"].ToString();
            string fullFilePath = picPath + @"\Areas\Import\res\Import\" + fileName;

            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            bool isColumnName = true;
            int startRow = 1;//标题行 第一行 Index
            int datastartRow = 2;//数据行 第一行 Index
            try
            {
                using (fs = new FileStream(fullFilePath, FileMode.Open))
                {
                    // 2007版本  
                    if (fullFilePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (fullFilePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(startRow);//第一行 :标题行 
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = datastartRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;
                                    if (row.Cells[0].CellType == CellType.Blank) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        public ActionResult addPeople()
        {
            string Grade = Session["Grade"].ToString();
            ViewBag.Grade = Grade;
            return View();
        }

        public ActionResult saveClass()
        {
            string teacherid = Session["UserID"].ToString();
            string classid = Request["classid"].ToString();
            string classname = Request["classname"].ToString();
            string grade = Request["grade"].ToString().Substring(0, 4);


            try
            {
                T_Class newclass = new T_Class();
                newclass.ID = classid;
                newclass.ClassName = classname;
                newclass.Grade = grade;
                newclass.Total = 0;
                newclass.TeacherID = teacherid;
                newclass.MonitorID = null;
                newclass.Batch = null;
                db.T_Class.Add(newclass);
                db.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                //.Data.Entity.Infrastructure.DbUpdateException
            }
            return ClassInfo();
        }

        public ActionResult editView()
        {
            string classid = Request["id"].ToString();
            T_Class c = db.T_Class.Find(classid);
            string classname = c.ClassName;
            ViewData["classinfo"] = c;
            var stulist = from stu in db.vw_Student
                          where stu.ST_Class == classname
                          select new
                          {
                              ID = stu.ST_Num,
                              Name = stu.ST_Name
                          };
            List<string> ddlStu = new List<string>();
            foreach (var stu in stulist)
            {
                ddlStu.Add(stu.Name + '-' + stu.ID);
            }
            ViewBag.ddlStu = ddlStu;
            return View();
        }

        public ActionResult editClass()
        {
            string classid = Request["classid"].ToString();
            string stuid = Request["stuid"].ToString();
            try
            {
                T_Class c = db.T_Class.Find(classid);
                c.MonitorID = stuid;
                db.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                //.Data.Entity.Infrastructure.DbUpdateException
            }

            return ClassInfo();
        }
    }
}