using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qingjia_MVC.Models;
using qingjia_MVC.Controllers;
using System.IO;
using System.Web.Script.Serialization;

namespace qingjia_MVC.Areas.Statistics.Controllers
{
    #region 数据模型

    public class JsonModel
    {
        public ArrayList indexNameArr { get; set; }

        public Dictionary<string, ArrayList> data { get; set; }
    }

    public class GeoJsonModel
    {
        public List<string> data { get; set; }
    }

    public class MessageModel
    {
        public string title { get; set; }
        public string content { get; set; }
        public string population { get; set; }
        public string reason { get; set; }
    }

    public class SimulationData
    {
        public string className{get;set;}
        public Dictionary<string,int> data{get;set;}
    }

    #endregion

    public class LeaveListController : BaseController
    {
        //实例化数据库
        imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        // GET: Statistics/LeaveList
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Num()
        {
            return PartialView("_Num");
        }

        public JsonResult GetData()
        {
            var res = new JsonResult();
            List<JsonModel> model = new List<JsonModel>();

            string type = Request["type"].ToString();
            if (type == "leavelist")
            {
                JsonModel leavelistJsonModel = new JsonModel();

                #region 请假记录统计
                var statisticData = from T_Statistic in db.T_Statistic select T_Statistic;
                if (statisticData.Any())
                {
                    ArrayList indexNameArr = new ArrayList();
                    Dictionary<string, ArrayList> data = new Dictionary<string, ArrayList>();

                    foreach (T_Statistic sta in statisticData)
                    {
                        ArrayList value = new ArrayList();
                        value.Add(sta.TotalNum);
                        value.Add(sta.ShortNum);
                        value.Add(sta.LongNum);
                        value.Add(sta.VacationNum);
                        value.Add(sta.NightNum);
                        value.Add(sta.SelfStudyNum);
                        value.Add(sta.ClassNum);
                        data.Add(ChangeID(sta.ID), value);
                    }

                    indexNameArr.Add("全部请假");
                    indexNameArr.Add("短期请假");
                    indexNameArr.Add("长期请假");
                    indexNameArr.Add("节假日请假");
                    indexNameArr.Add("晚点名请假");
                    indexNameArr.Add("自习请假");
                    indexNameArr.Add("上课请假");

                    leavelistJsonModel.indexNameArr = indexNameArr;
                    leavelistJsonModel.data = data;
                }
                else
                {
                    //暂无数据处理
                }
                #endregion

                model.Add(leavelistJsonModel);
            }
            if (type == "student")
            {
                JsonModel classJsonModel = new JsonModel();

                #region 学生信息统计
                string grade = Session["Grade"].ToString();

                var classList = from T_Class in db.T_Class where (T_Class.Grade == grade) select T_Class;
                if (classList.Any())
                {
                    ArrayList indexNameArr = new ArrayList();
                    Dictionary<string, ArrayList> data = new Dictionary<string, ArrayList>();

                    foreach (T_Class classModel in classList)
                    {
                        ArrayList value = new ArrayList();

                        //统计个各个班级男女比例
                        var studentlist = from vw_Student in db.vw_Student where (vw_Student.ST_Class.Contains(classModel.ClassName)) select vw_Student;
                        int maleNum = (studentlist.ToList()).Where(s => s.ST_Sex.Contains("男")).Count();
                        int femaleNum = (studentlist.ToList()).Where(s => s.ST_Sex.Contains("女")).Count();
                        value.Add(classModel.Total);
                        value.Add(maleNum);
                        value.Add(femaleNum);
                        data.Add(classModel.ClassName, value);
                    }
                    indexNameArr.Add("班级人数");
                    indexNameArr.Add("男生人数");
                    indexNameArr.Add("女生人数");

                    classJsonModel.indexNameArr = indexNameArr;
                    classJsonModel.data = data;
                }
                else
                {
                    //无查询数据，如何处理
                }
                #endregion

                model.Add(classJsonModel);
            }
            if (type == "atSchool")
            {
                JsonModel AtSchool = new JsonModel();

                #region 学生在校情况统计
                string UserID = Session["UserID"].ToString();
                string Grade = Session["Grade"].ToString();
                DateTime now = DateTime.Now;

                var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.TypeID == 1 && vw_LeaveList.TimeLeave <= now && vw_LeaveList.TimeBack >= now) select vw_LeaveList;
                var classList = from T_Class in db.T_Class where (T_Class.TeacherID == UserID && T_Class.Grade == Grade) select T_Class;

                if (LL.Any())
                {
                    if (classList.Any())
                    {
                        ArrayList indexNameArr = new ArrayList();
                        Dictionary<string, ArrayList> data = new Dictionary<string, ArrayList>();

                        foreach (T_Class item in classList)
                        {
                            ArrayList value = new ArrayList();
                            int num = (LL.ToList()).Where(i => i.ST_Class.Contains(item.ClassName)).Count();
                            value.Add(item.Total - num);
                            value.Add(num);
                            data.Add(item.ClassName, value);
                        }
                        indexNameArr.Add("在校人数");
                        indexNameArr.Add("离校学生");
                        AtSchool.indexNameArr = indexNameArr;
                        AtSchool.data = data;
                    }
                    else
                    {
                        //班级为空，尚未分配管理班级
                        return null;
                    }
                }
                else
                {
                    //无离校同学
                    ArrayList indexNameArr = new ArrayList();
                    Dictionary<string, ArrayList> data = new Dictionary<string, ArrayList>();

                    foreach (T_Class item in classList)
                    {
                        ArrayList value = new ArrayList();
                        int num = 0;
                        value.Add(item.Total - num);
                        value.Add(num);
                        data.Add(item.ClassName, value);
                    }
                    indexNameArr.Add("在校人数");
                    indexNameArr.Add("离校学生");
                    AtSchool.indexNameArr = indexNameArr;
                    AtSchool.data = data;
                }
                #endregion

                model.Add(AtSchool);
            }
            if (type == "vacation")
            {
                //string StartTime = "170102";
                //string EndTime = "170105";

                //var sr = new StreamReader(Request.InputStream);
                //var stream = sr.ReadToEnd();
                //JavaScriptSerializer js = new JavaScriptSerializer();
                //var list = js.Deserialize<GeoJsonModel>(stream);

                //var LL_Address = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ID.CompareTo(StartTime) >= 0 && vw_LeaveList.ID.CompareTo(EndTime) <= 0 && vw_LeaveList.TypeChildID == 6) select vw_LeaveList.Address;

                //Dictionary<string, int> returnData = new Dictionary<string, int>();

                //foreach (string geo in list.data)
                //{
                //    int n = 0;
                //    foreach (string address in LL_Address)
                //    {
                //        if (address.Contains(geo))
                //        {
                //            n++;
                //        }
                //    }
                //    if (n != 0)
                //    {
                //        returnData.Add(geo, n);
                //    }
                //}

                //res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;//允许使用GET方式获取，否则用GET获取是会报错
                //res.Data = returnData;

                #region 模拟数据
                SimulationData simulationData_01 = new SimulationData();
                simulationData_01.className = "信管1401班";
                Dictionary<string, int> data = new Dictionary<string, int>();
                data.Add("上海", 95);
                data.Add("广州", 90);
                data.Add("大连", 70);
                data.Add("南宁", 60);
                data.Add("南昌", 50);
                data.Add("南京", 40);
                data.Add("长春", 40);
                data.Add("哈尔滨", 35);
                data.Add("重庆", 34);
                data.Add("包头", 30);
                data.Add("常州", 10);
                simulationData_01.data = data;

                SimulationData simulationData_02 = new SimulationData();
                simulationData_02.className = "信管1402班";
                Dictionary<string, int> data_02 = new Dictionary<string, int>();
                data_02.Add("包头", 95);
                data_02.Add("长春", 90);
                data_02.Add("昆明", 70);
                data_02.Add("郑州", 60);
                data_02.Add("南昌", 50);
                data_02.Add("长沙", 40);
                data_02.Add("丹东", 40);
                data_02.Add("大连", 35);
                data_02.Add("拉萨", 34);
                data_02.Add("太原", 30);
                data_02.Add("常州", 10);
                simulationData_02.data = data_02;

                SimulationData simulationData_03 = new SimulationData();
                simulationData_03.className = "信管1403班";
                Dictionary<string, int> data_03 = new Dictionary<string, int>();
                data_03.Add("福州", 95);
                data_03.Add("太原", 90);
                data_03.Add("长春", 70);
                data_03.Add("重庆", 60);
                data_03.Add("西安", 50);
                data_03.Add("成都", 40);
                data_03.Add("北京", 40);
                data_03.Add("天津", 35);
                data_03.Add("海口", 34);
                data_03.Add("北海", 30);
                data_03.Add("沈阳", 10);
                simulationData_03.data = data_03;

                List<SimulationData> simulationDataList = new List<SimulationData>();
                simulationDataList.Add(simulationData_01);
                simulationDataList.Add(simulationData_02);
                simulationDataList.Add(simulationData_03);

                #endregion

                res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                res.Data = simulationDataList;//模拟数据
                return res;
            }

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;//允许使用GET方式获取，否则用GET获取是会报错
            res.Data = model;
            return res;
        }

        private string ChangeID(string ID)
        {
            return "20" + ID.Substring(0, 2) + "年 " + ID.Substring(2, 2) + "月" + ID.Substring(4, 2) + "日";
        }

        public JsonResult Analysis()
        {
            JsonResult result = new JsonResult();

            string UserID = Session["UserID"].ToString();
            string Grade = Session["Grade"].ToString();

            var classList = from T_Class in db.T_Class where (T_Class.TeacherID == UserID && T_Class.Grade == Grade) select T_Class.ClassName;
            List<vw_LeaveList> LL_List = new List<vw_LeaveList>();
            if (classList.Any())
            {
                foreach (string className in classList)
                {
                    LL_List.AddRange((from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ST_TeacherID == UserID && vw_LeaveList.ST_Grade == Grade && vw_LeaveList.ST_Class == className && vw_LeaveList.TypeChildID != 6) orderby vw_LeaveList.SubmitTime descending select vw_LeaveList).Take(20).ToList());
                }
            }

            List<MessageModel> modelList = new List<MessageModel>();

            #region 病假检测
            foreach (string className in classList)
            {
                List<vw_LeaveList> _LL = new List<vw_LeaveList>();//某班级的病假数据
                foreach (vw_LeaveList item in LL_List)
                {
                    if (item.ST_Class == className && item.LeaveType.Contains("病假"))
                    {
                        _LL.Add(item);
                    }
                }

                if (_LL.Any())
                {
                    DateTime startTime = _LL[_LL.Count() - 1].SubmitTime;
                    DateTime endTime = _LL[0].SubmitTime;
                    TimeSpan ts = endTime - startTime;
                    int days = ts.Days;
                    if (days <= 7 && _LL.Count() >= 5)
                    {
                        //在 一段时间内  某个班级有多人生病
                        MessageModel model = new MessageModel();
                        model.title = "病假情况异常";
                        model.content = className + "班存在病假异常现象， " + startTime.ToString("yyyy-MM-dd") + "-" + endTime.ToString("yyyy-MM-dd") + "期间：" + _LL.Count() + "人次病假。";
                        model.reason = "流行性疾病导致多人次、集中性病假";
                        model.population = "className";
                        modelList.Add(model);
                    }
                }
            }
            #endregion

            #region 寝室检测
            Dictionary<string, int> DoorData = new Dictionary<string, int>();
            List<string> DoorListTotal = new List<string>();
            foreach (vw_LeaveList item in LL_List)
            {
                DoorListTotal.Add(item.ST_Dor.ToString().Trim());
            }
            var DoorNum = from p in DoorListTotal group p by p into g select new { Key = g.Count(), BranchName = g.Key };
            if (DoorNum.Any())
            {
                foreach (var v in DoorNum)
                {
                    if (v.Key >= 4)
                    {
                        DoorData.Add(v.BranchName, v.Key);
                    }
                }
                if (DoorData.Any())
                {
                    foreach (var item in DoorData)
                    {
                        List<string> _LL = new List<string>();
                        foreach (var itemLL in LL_List)
                        {
                            if (itemLL.ST_Dor.ToString().Trim() == item.Key)
                            {
                                _LL.Add(itemLL.ID.ToString().Substring(0, 6));
                            }
                        }
                        //计数统计
                        var DayNum = from p in _LL group p by p into g select new { Key = g.Count(), BranchName = g.Key };
                        foreach (var d in DayNum)
                        {
                            if (d.Key >= 4)
                            {
                                //某寝室 四人在同一天请假
                                //在 一段时间内  某个班级有多人生病
                                MessageModel model = new MessageModel();
                                model.title = "寝室请假异常";
                                model.content = item.Key + "存在请假异常现象， " + "20" + d.BranchName.Substring(0, 2) + "-" + d.BranchName.Substring(2, 2) + "-" + d.BranchName.Substring(4, 2) + "-" + "4人次请假。";
                                model.reason = "寝室多人集中请假！";
                                model.population = "寝室" + item.Key;
                                modelList.Add(model);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 民族检测
            Dictionary<string, int> NationData = new Dictionary<string, int>();
            List<vw_LeaveList> nationLL_List = new List<vw_LeaveList>();
            foreach (vw_LeaveList item in LL_List)
            {
                if (item.Nation != null && item.Nation != "汉族")
                {
                    nationLL_List.Add(item);
                }
            }

            if (nationLL_List.Any())
            {
                List<String> nationList = new List<string>();
                foreach (vw_LeaveList item in nationLL_List)
                {
                    nationList.Add(item.Nation);
                }

                //计数
                var NationNum = from p in nationList group p by p into g select new { Key = g.Count(), BranchName = g.Key };
                if (NationNum.Any())
                {
                    foreach (var v in NationNum)
                    {
                        if (v.Key >= 4)
                        {
                            NationData.Add(v.BranchName, v.Key);
                        }
                    }
                    if (NationData.Any())
                    {
                        foreach (var item in NationData)
                        {
                            List<string> _LL = new List<string>();
                            foreach (var itemLL in LL_List)
                            {
                                if (itemLL.Nation != null && itemLL.Nation.ToString() == item.Key)
                                {
                                    _LL.Add(itemLL.ID.ToString().Substring(0, 6));
                                }
                            }
                            //计数统计
                            var DayNum = from p in _LL group p by p into g select new { Key = g.Count(), BranchName = g.Key };
                            foreach (var d in DayNum)
                            {
                                if (d.Key >= 4)
                                {
                                    //某寝室 四人在同一天请假
                                    //在 一段时间内  某个班级有多人生病
                                    MessageModel model = new MessageModel();
                                    model.title = "少数民族请假异常";
                                    model.content = item.Key.ToString().Trim() + "存在请假异常现象， " + "20" + d.BranchName.Substring(0, 2) + "-" + d.BranchName.Substring(2, 2) + "-" + d.BranchName.Substring(4, 2) + "-" + d.Key + "人次请假。";
                                    model.reason = "同一天内，多名少数名族学生同时请假，原因可能是少数民族节假日、或其他！";
                                    model.population = item.Key.ToString().Trim();
                                    modelList.Add(model);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            result.Data = modelList;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;//允许使用GET方式获取，否则用GET获取是会报错
            return result;
        }
    }
}