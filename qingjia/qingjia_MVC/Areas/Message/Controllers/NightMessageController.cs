using FineUIMvc;
using Newtonsoft.Json.Linq;
using qingjia_MVC.Common;
using qingjia_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;
using qingjia_MVC.Controllers;

namespace qingjia_MVC.Areas.Message.Controllers
{
    public class checkInfo
    {
        public string item_batch { get; set; }
        public string item_name { get; set; }
        public string item_time { get; set; }
        public string item_location { get; set; }
    }

    public class BatchClassModel
    {
        public List<vw_ClassBatch> BatchClassList { get; set; }
    }

    public class NightMessageController : BaseController
    {
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        /// <summary>
        /// 测试使用
        /// </summary>
        protected void Login()
        {
            Session["UserID"] = "1214001";
            Session["UserName"] = "梁导";
            Session["Grade"] = "2014";
        }

        // GET: Message/NightMessage
        public ActionResult Index()
        {
            //Login();//测试使用

            return View();
        }

        public ActionResult _FormContent()
        {
            //获取用户信息
            string UserID = Session["UserID"].ToString();
            string UserName = Session["UserName"].ToString();
            string Grade = Session["Grade"].ToString();

            #region 加载基本信息
            ViewBag.TeacherName = UserName;
            ViewBag.Grade = Grade + "级";

            var batchList = from T_Batch in db.T_Batch where (T_Batch.TeacherID == UserID) orderby T_Batch.Batch select T_Batch;
            if (batchList.Any())
            {
                List<T_Batch> BatchModel = new List<T_Batch>();
                BatchModel = batchList.ToList();
                ViewBag.Date = (BatchModel.First().Datetime == null) ? "未设定" : BatchModel.First().Datetime.ToString("yyyy-MM-dd");
                foreach (T_Batch batch in BatchModel)
                {
                    if (batch.Batch == 1)
                    {
                        ViewBag.First = (batch.Datetime == null) ? "未设定" : batch.Datetime.ToString("HH:mm");
                        ViewBag.FirstLoacation = (batch.Location == null) ? "未设定" : batch.Location.ToString();
                    }
                    if (batch.Batch == 2)
                    {
                        ViewBag.Second = (batch.Datetime == null) ? "未设定" : batch.Datetime.ToString("HH:mm");
                        ViewBag.SecondLoacation = (batch.Location == null) ? "未设定" : batch.Location.ToString();
                    }
                    if (batch.Batch == 3)
                    {
                        ViewBag.Third = (batch.Datetime == null) ? "未设定" : batch.Datetime.ToString("HH:mm");
                        ViewBag.ThirdLoacation = (batch.Location == null) ? "未设定" : batch.Location.ToString();
                    }
                }
            }
            else
            {
                ViewBag.Date = "未设定";
                ViewBag.First = "未设定";
                ViewBag.Second = "未设定";
                ViewBag.Third = "未设定";
            }
            #endregion

            return PartialView("_FormContent", GetClassInfo(UserID));
        }

        protected List<BatchClassModel> GetClassInfo(string UserID)
        {
            var classList = from vw_ClassBatch in db.vw_ClassBatch where (vw_ClassBatch.TeacherID == UserID) orderby vw_ClassBatch.ClassName select vw_ClassBatch;

            List<BatchClassModel> data = new List<BatchClassModel>();
            if (classList.Any())
            {
                List<vw_ClassBatch> batchInfoList = new List<vw_ClassBatch>();
                batchInfoList = classList.ToList();

                List<int> numList = new List<int>();
                numList = CountNum(classList.Count(), 3);//后一个参数代表行数  返回值为每行所对应的个数

                int index = 0;

                foreach (int num in numList)
                {
                    List<vw_ClassBatch> batchInfo = new List<vw_ClassBatch>();
                    for (int n = 0; n < num; n++)
                    {
                        batchInfo.Add(batchInfoList[index++]);
                    }
                    BatchClassModel list = new BatchClassModel();
                    list.BatchClassList = batchInfo;
                    data.Add(list);
                }
                return data;
            }
            else
            {
                return null;
            }
        }

        protected List<int> CountNum(int totalNum, int trNum)
        {
            List<int> numList = new List<int>();

            int a = totalNum / trNum;
            int b = totalNum % trNum;

            for (int n = 0; n < trNum; n++)
            {
                numList.Add(a);
            }

            for (int n = 0; n < b; n++)
            {
                numList[n] = numList[n] + 1;
            }

            return numList;
        }

        public string NightSet()
        {
            var result = new JsonResult();


            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<List<checkInfo>>(stream);
            if (list.Any())
            {
                SetBatch(list);
                return "修改成功";
            }
            return "修改失败";
        }

        protected void SetBatch(List<checkInfo> list)
        {
            string UserID = Session["UserID"].ToString();

            int maxbatch = 0;
            foreach (checkInfo info in list)
            {
                if (Convert.ToInt32(info.item_batch) > maxbatch)
                {
                    maxbatch = Convert.ToInt32(info.item_batch);
                }
            }

            var oldbatchlist = from T_Batch in db.T_Batch where T_Batch.TeacherID == UserID orderby T_Batch.Batch select T_Batch;
            List<System.Guid> IDlist = new List<System.Guid>();
            List<string> batchlist = new List<string>();
            foreach (T_Batch batch in oldbatchlist)
            {
                IDlist.Add(batch.ID);
                batchlist.Add(batch.Batch.ToString());
            }

            if (maxbatch <= IDlist.Count)
            {
                foreach (System.Guid id in IDlist)
                {
                    List<string> classID = new List<string>();
                    T_Batch batch = db.T_Batch.Find(id);
                    foreach (checkInfo info in list)
                    {
                        if (info.item_batch == batch.Batch.ToString())
                        {
                            batch.Datetime = Convert.ToDateTime(info.item_time);
                            batch.Location = info.item_location;
                            classID.Add(info.item_name);
                        }
                    }

                    foreach (string classNum in classID)
                    {
                        T_Class classmodel = db.T_Class.Find(classNum);
                        classmodel.Batch = id;
                    }
                    db.SaveChanges();
                }
            }
            if (maxbatch > IDlist.Count)
            {
                foreach (System.Guid id in IDlist)
                {
                    T_Batch batch = db.T_Batch.Find(id);
                    db.T_Batch.Remove(batch);
                }
                //db.SaveChanges();
                for (int i = 1; i <= 3; i++)
                {
                    T_Batch batchModel = new T_Batch();
                    List<string> classList = new List<string>();
                    System.Guid newID = System.Guid.NewGuid();

                    foreach (checkInfo info in list)
                    {
                        if (info.item_batch == i.ToString())
                        {
                            batchModel.Batch = Convert.ToInt32(info.item_batch);
                            batchModel.TeacherID = UserID;
                            batchModel.Datetime = Convert.ToDateTime(info.item_time);
                            batchModel.ID = newID;
                            batchModel.Location = info.item_location;
                            classList.Add(info.item_name);
                        }
                    }
                    db.T_Batch.Add(batchModel);
                    foreach (string classID in classList)
                    {
                        T_Class classModel = db.T_Class.Find(classID);
                        classModel.Batch = newID;
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}