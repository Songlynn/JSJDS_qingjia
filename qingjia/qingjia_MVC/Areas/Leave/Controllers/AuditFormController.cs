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

namespace qingjia_MVC.Areas.Leave.Controllers
{
    public class AuditFormController : BaseController
    {
        //实例化数据库
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();
        private string staticLeaveType = "total";
        private string staticST_Class = "total";//代表全部班级

        // GET: Leave/AuditForm
        public ActionResult AuditLeave()
        {
            //测试使用
            //Session["AuditState"] = "leave";
            //Get_LL_DataTable("total");
            return View();
        }

        public ActionResult AuditBack()
        {
            Session["AuditState"] = "back";
            Get_LL_DataTable("total");
            return View();
        }

        #region AuditLeaveList

        /// <summary>
        /// 获取请假记录DataTable格式
        /// </summary>
        /// <param name="ST_NUM">账号</param>
        /// <param name="type">请假类型</param>
        /// <returns></returns>
        public DataTable Get_LL_DataTable(string type)
        {
            if (Session["AuditState"].ToString() == "leave")
            {
                //此处将list转为DataTable FineUI的Grid绑定时间类型数据时会发生错误，尚未找到原因。
                //解决办法：将list转为DataTable绑定到Grid，并且将DataTable中值类型为DateTime的列转为字符串类型

                #region 获取LeaveList、转换为DataTable格式
                DataTable dtSource = new DataTable();
                var leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;

                if (type == "btnShort")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType == "短期请假") && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnLong")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType == "长期请假") && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnHoliday")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType == "节假日请假") && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnCall")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType.StartsWith("晚点名请假")) && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnClass")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType.StartsWith("上课请假")) && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                //List 转换为 DataTable
                dtSource = leavelist.ToDataTable(rec => new object[] { leavelist });
                #endregion

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
                        rowNew["Lesson"] = "第三大节（14:00~15:30）";
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

                //绑定数据源
                ViewBag.leavetable = dtClone;

                return dtClone;
            }
            else if (Session["AuditState"].ToString() == "back")
            {
                //此处将list转为DataTable FineUI的Grid绑定时间类型数据时会发生错误，尚未找到原因。
                //解决办法：将list转为DataTable绑定到Grid，并且将DataTable中值类型为DateTime的列转为字符串类型

                #region 获取LeaveList、转换为DataTable格式
                DataTable dtSource = new DataTable();
                var leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;

                if (type == "btnShort")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType == "短期请假") && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnLong")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType == "长期请假") && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnHoliday")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType == "节假日请假") && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnCall")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType.StartsWith("晚点名请假")) && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnClass")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.LeaveType.StartsWith("上课请假")) && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                //List 转换为 DataTable
                dtSource = leavelist.ToDataTable(rec => new object[] { leavelist });
                #endregion

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
                        rowNew["Lesson"] = "第三大节（14:00~15:30）";
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

                //绑定数据源
                ViewBag.leavetable = dtClone;

                return dtClone;
            }
            else
            {
                //未知错误、此处代码退出到登录界面
                return null;
            }
        }

        /// <summary>
        /// 根据学号查找请假记录
        /// </summary>
        /// <param name="ST_NUM"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable Get_LL_DataTable_BY_ST_Num(string ST_NUM, string type)
        {
            if (Session["AuditState"].ToString() == "leave")
            {
                //此处将list转为DataTable FineUI的Grid绑定时间类型数据时会发生错误，尚未找到原因。
                //解决办法：将list转为DataTable绑定到Grid，并且将DataTable中值类型为DateTime的列转为字符串类型

                #region 获取LeaveList、转换为DataTable格式
                DataTable dtSource = new DataTable();
                var leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;

                if (type == "btnShort")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType == "短期请假") && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnLong")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType == "长期请假") && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnHoliday")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType == "节假日请假") && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnCall")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType.StartsWith("晚点名请假")) && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnClass")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType.StartsWith("上课请假")) && (vw_LeaveList.StateLeave == "0") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                //List 转换为 DataTable
                dtSource = leavelist.ToDataTable(rec => new object[] { leavelist });
                #endregion

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
                        rowNew["Lesson"] = "第三大节（14:00~15:30）";
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

                //绑定数据源
                ViewBag.leavetable = dtClone;

                return dtClone;
            }
            else if (Session["AuditState"].ToString() == "back")
            {
                //此处将list转为DataTable FineUI的Grid绑定时间类型数据时会发生错误，尚未找到原因。
                //解决办法：将list转为DataTable绑定到Grid，并且将DataTable中值类型为DateTime的列转为字符串类型

                #region 获取LeaveList、转换为DataTable格式
                DataTable dtSource = new DataTable();
                var leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;

                if (type == "btnShort")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType == "短期请假") && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnLong")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType == "长期请假") && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnHoliday")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType == "节假日请假") && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnCall")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType.StartsWith("晚点名请假")) && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                if (type == "btnClass")
                {
                    leavelist = from vw_LeaveList in db.vw_LeaveList where ((vw_LeaveList.StudentID == ST_NUM) && (vw_LeaveList.LeaveType.StartsWith("上课请假")) && (vw_LeaveList.StateLeave == "1") && (vw_LeaveList.StateBack == "0")) orderby vw_LeaveList.ID descending select vw_LeaveList;
                }
                //List 转换为 DataTable
                dtSource = leavelist.ToDataTable(rec => new object[] { leavelist });
                #endregion

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
                        rowNew["Lesson"] = "第三大节（14:00~15:30）";
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

                //绑定数据源
                ViewBag.leavetable = dtClone;

                return dtClone;
            }
            else
            {
                //未知错误、此处代码退出到登录界面
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LL_ID">请假单号</param>
        /// <returns></returns>
        public DataTable Get_LeaveList(string LL_ID)
        {
            if (Session["AuditState"].ToString() == "leave")
            {
                //此处将list转为DataTable FineUI的Grid绑定时间类型数据时会发生错误，尚未找到原因。
                //解决办法：将list转为DataTable绑定到Grid，并且将DataTable中值类型为DateTime的列转为字符串类型

                #region 获取LeaveList、转换为DataTable格式
                DataTable dtSource = new DataTable();
                var leavelist = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ID == LL_ID) orderby vw_LeaveList.ID descending select vw_LeaveList;

                //List 转换为 DataTable
                dtSource = leavelist.ToDataTable(rec => new object[] { leavelist });
                #endregion

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
                        rowNew["Lesson"] = "第三大节（14:00~15:30）";
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

                //绑定数据源
                ViewBag.leavetable = dtClone;

                return dtClone;
            }
            else if (Session["AuditState"].ToString() == "back")
            {
                //此处将list转为DataTable FineUI的Grid绑定时间类型数据时会发生错误，尚未找到原因。
                //解决办法：将list转为DataTable绑定到Grid，并且将DataTable中值类型为DateTime的列转为字符串类型

                #region 获取LeaveList、转换为DataTable格式
                DataTable dtSource = new DataTable();
                var leavelist = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ID == LL_ID) orderby vw_LeaveList.ID descending select vw_LeaveList;

                //List 转换为 DataTable
                dtSource = leavelist.ToDataTable(rec => new object[] { leavelist });
                #endregion

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
                        rowNew["Lesson"] = "第三大节（14:00~15:30）";
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

                //绑定数据源
                ViewBag.leavetable = dtClone;

                return dtClone;
            }
            else
            {
                //未知错误、此处代码退出到登录界面
                return null;
            }
        }

        //同意请假操作
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnAgreeClick(JArray selectedRows, JArray gridLeaveList_fields)
        {
            if (Session["AuditState"].ToString() == "leave")
            {
                #region 同意请假操作
                foreach (string rowId in selectedRows)
                {
                    //vw_LeaveList vw_LL = db.vw_LeaveList.Find(rowId);
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ID == rowId) select vw_LeaveList;
                    vw_LeaveList vw_LL = LL.ToList().First();
                    if (vw_LL.TypeID == 1 || vw_LL.LeaveType.ToString().Substring(0, 3) == "晚点名")
                    {
                        //离校请假和晚点名请假需要进行销假
                        T_LeaveList T_LL = db.T_LeaveList.Find(rowId);
                        //将请假记录状态修改为待销假状态
                        T_LL.StateLeave = "1";
                        T_LL.StateBack = "0";
                        //将审核人修改为辅导员姓名
                        T_LL.AuditTeacherID = Session["UserID"].ToString();
                    }
                    else
                    {
                        //上课请假备案和早晚自习请假不需要销假
                        T_LeaveList T_LL = db.T_LeaveList.Find(rowId);
                        //将请假记录状态修改为已销假状态
                        T_LL.StateLeave = "1";
                        T_LL.StateBack = "1";
                    }
                }
                db.SaveChanges();

                UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), gridLeaveList_fields);
                //Alert.Show("已同意请假！");
                ShowNotify(String.Format("已同意请假！"));
                return UIHelper.Result();
                #endregion
            }
            else if (Session["AuditState"].ToString() == "back")
            {
                #region 同意销假操作
                foreach (string rowId in selectedRows)
                {
                    //vw_LeaveList vw_LL = db.vw_LeaveList.Find(rowId);
                    var LL = from vw_LeaveList in db.vw_LeaveList where (vw_LeaveList.ID == rowId) select vw_LeaveList;
                    vw_LeaveList vw_LL = LL.ToList().First();
                    if (vw_LL.TypeID == 1 || vw_LL.LeaveType.ToString().Substring(0, 3) == "晚点名")
                    {
                        //离校请假和晚点名请假需要进行销假
                        T_LeaveList T_LL = db.T_LeaveList.Find(rowId);
                        //将请假记录状态修改为已销假状态
                        T_LL.StateLeave = "1";
                        T_LL.StateBack = "1";
                    }
                    else
                    {
                        //上课请假备案和早晚自习请假不需要销假
                        T_LeaveList T_LL = db.T_LeaveList.Find(rowId);
                        //将请假记录状态修改为已销假状态
                        T_LL.StateLeave = "1";
                        T_LL.StateBack = "1";
                    }
                }
                db.SaveChanges();

                UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), gridLeaveList_fields);
                //Alert.Show("已同意请假！");
                ShowNotify(String.Format("已同意销假！"));
                return UIHelper.Result();
                #endregion
            }
            else
            {
                //未知错误，报错，退回到登录界面
                return UIHelper.Result();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnCancelClick(FormCollection formInfo, JArray fields)
        {
            string reason = formInfo["Reason"].ToString();
            T_LeaveList LL = db.T_LeaveList.Find(Session["LL_NUM"].ToString());
            LL.Reason = reason;
            LL.StateLeave = "2";
            LL.StateBack = "1";
            db.SaveChanges();
            UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), fields);
            UIHelper.Window("cancelWindow").Close();
            //PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference());
            ShowNotify(String.Format("驳回请假成功！"));
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult cancelWindow_Show(JArray selectedRows, JArray gridLeaveList_fields)
        {
            string LL_NUM = selectedRows.ToList().First().ToString();
            Session["LL_NUM"] = LL_NUM;
            UIHelper.Window("cancelWindow").Title("驳回 - " + LL_NUM);
            UIHelper.Window("cancelWindow").Show();
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult printWindow_Close()
        {
            Alert.Show("触发了窗体的关闭事件！");
            return UIHelper.Result();
        }

        public ActionResult btnLeave_Click(Grid gridLeaveList)
        {
            //DataTable dt = gridLeaveList.ToDataTable();

            return UIHelper.Result();
        }


        #region 根据按钮名称检索请假记录
        //更新Grid数据
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnTotal_ReloadData(JArray fields)
        {
            staticLeaveType = "total";
            UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), fields);
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnShort_ReloadData(JArray fields)
        {
            staticLeaveType = "btnShort";
            UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), fields);
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnLong_ReloadData(JArray fields)
        {
            staticLeaveType = "btnLong";
            UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), fields);
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnHoliday_ReloadData(JArray fields)
        {
            staticLeaveType = "btnHoliday";
            UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), fields);
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnCall_ReloadData(JArray fields)
        {
            staticLeaveType = "btnCall";
            UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), fields);
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnClass_ReloadData(JArray fields)
        {
            staticLeaveType = "btnClass";
            UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable(staticLeaveType), fields);
            return UIHelper.Result();
        }
        #endregion

        #region 根据搜索条件检索请假记录

        //搜索框一 学生姓名搜索搜索
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TwinTriggerBox1_Trigger2Click(string text, JArray fields)
        {
            // 点击 TwinTriggerBox 的搜索按钮
            var TwinTriggerBox1 = UIHelper.TwinTriggerBox("TwinTriggerBox1");
            string ST_NUM = "";

            if (!String.IsNullOrEmpty(text))
            {
                // 执行搜索动作
                var ST_Num_List = from vw_Student in db.vw_Student where (vw_Student.ST_Name == text) select vw_Student.ST_Num;
                if (ST_Num_List.Any())
                {
                    if (ST_Num_List.ToList().Count == 1)
                    {
                        ST_NUM = ST_Num_List.ToList().First().ToString();
                        ShowNotify(String.Format("检索完成！", text));
                        UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable_BY_ST_Num(ST_NUM, staticLeaveType), fields);
                    }
                    else
                    {
                        ShowNotify(String.Format("姓名为{0}的学生不唯一，请根据其他信息检索！", text));
                    }
                }
                else
                {
                    ShowNotify(String.Format("姓名为{0}的学生不存在，请重新输入！", text));
                }

                TwinTriggerBox1.ShowTrigger1(true);
            }
            else
            {
                ShowNotify("请输入你要搜索的关键词！");
            }
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TwinTriggerBox1_Trigger1Click(string content)
        {
            // 点击 TwinTriggerBox 的取消按钮
            var TwinTriggerBox1 = UIHelper.TwinTriggerBox("TwinTriggerBox1");

            ShowNotify("取消搜索！");

            // 执行清空动作
            TwinTriggerBox1.Text("");
            TwinTriggerBox1.ShowTrigger1(false);

            return UIHelper.Result();
        }

        //搜索框二 学号搜索
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TwinTriggerBox2_Trigger2Click(string text, JArray fields)
        {
            // 点击 TwinTriggerBox 的搜索按钮
            var TwinTriggerBox2 = UIHelper.TwinTriggerBox("TwinTriggerBox2");

            if (!String.IsNullOrEmpty(text))
            {
                // 执行搜索动作
                var ST_Info_List = from vw_Student in db.vw_Student where (vw_Student.ST_Num == text) select vw_Student;
                if (ST_Info_List.Any())
                {
                    ShowNotify(String.Format("检索完成！"));
                    UIHelper.Grid("gridLeaveList").DataSource(Get_LL_DataTable_BY_ST_Num(text, staticLeaveType), fields);
                }
                else
                {
                    ShowNotify(String.Format("学号为{0}的学生不存在，请检查后重新检索！", text));
                }
                TwinTriggerBox2.ShowTrigger1(true);
            }
            else
            {
                ShowNotify("请输入你要搜索的关键词！");
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TwinTriggerBox2_Trigger1Click(string content)
        {
            // 点击 TwinTriggerBox 的取消按钮
            var TwinTriggerBox2 = UIHelper.TwinTriggerBox("TwinTriggerBox2");


            ShowNotify("取消搜索！");

            // 执行清空动作
            TwinTriggerBox2.Text("");
            TwinTriggerBox2.ShowTrigger1(false);

            return UIHelper.Result();
        }

        //搜索框三 请假单号搜索
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TwinTriggerBox3_Trigger2Click(string text, JArray fields)
        {
            // 点击 TwinTriggerBox 的搜索按钮
            var TwinTriggerBox3 = UIHelper.TwinTriggerBox("TwinTriggerBox3");

            if (!String.IsNullOrEmpty(text))
            {
                // 执行搜索动作
                DataTable dt = Get_LeaveList(text);
                if (dt.Rows.Count == 1)
                {
                    ShowNotify(String.Format("检索完成！"));
                    UIHelper.Grid("gridLeaveList").DataSource(dt, fields);
                }
                else
                {
                    ShowNotify(String.Format("请假单号为{0}的请假记录不存在！", text));
                }
                TwinTriggerBox3.ShowTrigger1(true);
            }
            else
            {
                ShowNotify("请输入你要搜索的关键词！");
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TwinTriggerBox3_Trigger1Click(string content)
        {
            // 点击 TwinTriggerBox 的取消按钮
            var TwinTriggerBox3 = UIHelper.TwinTriggerBox("TwinTriggerBox3");


            ShowNotify("取消搜索！");

            // 执行清空动作
            TwinTriggerBox3.Text("");
            TwinTriggerBox3.ShowTrigger1(false);

            return UIHelper.Result();
        }
        #endregion

        #region 按班级查找  尚未完成
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ddlST_Class_SelectedIndexChanged(string ddlST_Class, string ddlST_ClassDropDownList1_text, JArray fields)
        {
            //按班级、请假类型查找
            //尚未完成

            return UIHelper.Result();
        }
        #endregion


        #endregion

    }
}