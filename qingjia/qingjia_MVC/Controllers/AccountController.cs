using FineUIMvc;
using qingjia_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ShortMessage;

namespace qingjia_MVC.Controllers
{
    public class AccountController : Controller
    {
        //实例化数据库
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Forget()
        {
            return View();
        }

        /// <summary>
        /// 登录 Ajax登录请求
        /// </summary>
        /// <returns></returns>
        public string AjaxLogin()
        {
            string UserID = Request["UserID"].ToString();
            string Psd = Request["Psd"].ToString();

            T_Account account = db.T_Account.Find(UserID.ToString().Trim());
            if (account != null)
            {
                if (account.Psd.ToString().Trim() == Psd.ToString().Trim())
                {
                    return "1";
                }
                else
                {
                    //密码错误
                    return "2";
                }
            }
            else
            {
                //账号不存在
                return "0";
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult login()
        {
            string txbAccount = Request["UserID"].ToString();
            string txbAccountPass = Request["Psd"].ToString();

            var user = from T_Account in db.T_Account where (T_Account.ID == txbAccount.ToString().Trim()) && (T_Account.Psd == txbAccountPass.ToString().Trim()) select T_Account;

            if (user.Any() && user.Count() == 1)
            {

                #region 登录成功、将用户信息写入Session
                //从数据集中提取
                Session["UserID"] = user.First().ID;
                Session["UserPsd"] = user.First().Psd;
                Session["RoleID"] = user.First().RoleID;

                //设置Session
                SetSession();

                #endregion

                #region 检查学生是否完善个人信息
                if (Session["RoleID"].ToString() == "1")//学生
                {
                    T_Student student = db.T_Student.Find(Session["UserID"].ToString());
                    if (student.ContactOne == "" || student.ContactOne == null || student.OneTel == "" || student.OneTel == null)//信息不完善
                    {
                        //FineUI登陆成功提示框、完善个人信息
                        ShowNotify("成功登陆！请完善个人信息", MessageBoxIcon.Success);
                        return RedirectToAction("Index", "UserInfo", new { area = "UserInfo" });
                        //此处需要以Areas的ID作为参数才能实现从Controller到Areas中的Controller的跳转
                    }
                    else//信息已完善
                    {
                        //FineUI登陆成功提示框
                        ShowNotify("成功登录！", MessageBoxIcon.Success);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    //FineUI登陆成功提示框
                    ShowNotify("成功登录！", MessageBoxIcon.Success);
                    return RedirectToAction("Index", "Home");
                }
                #endregion
            }
            else if (!user.Any())
            {
                alertInfo("登录提示", "用户名或密码错误！", "Information");
                return RedirectToAction("Index", "Account");
            }
            else
            {
                alertInfo("登录提示", "用户名或密码错误！", "Information");
                return RedirectToAction("Index", "Account");
            }
        }

        

        /// <summary>
        /// 设置将账号相关信息存入Session
        /// </summary>
        protected void SetSession()
        {
            string UserID = Session["UserID"].ToString();
            string RoleID = Session["RoleID"].ToString();

            if (RoleID == "1")//学生
            {
                vw_Student modelStudent = (vw_Student)(from vw_Student in db.vw_Student where (vw_Student.ST_Num == UserID) select vw_Student).First();
                Session["Grade"] = modelStudent.ST_Grade;
                Session["UserName"] = modelStudent.ST_Name;
            }
            if (RoleID == "2")//班级
            {
                T_Class modelClass = db.T_Class.Find(UserID);
                Session["Grade"] = modelClass.Grade;
                Session["UserName"] = modelClass.ClassName;
            }
            if (RoleID == "3")//辅导员
            {
                T_Teacher modelTeacher = db.T_Teacher.Find(UserID);
                Session["Grade"] = modelTeacher.Grade;
                Session["UserName"] = modelTeacher.Name;
            }
        }

        #region 忘记密码模块
        public string ST_NumCheck_message()
        {
            string ST_NUM = "";
            string Type = "";
            if (Request["ST_NUM"] != null)
            {
                ST_NUM = Request["ST_NUM"].ToString();
            }
            if (Request["Type"] != null)
            {
                Type = Request["Type"].ToString();
            }

            T_Student student = db.T_Student.Find(ST_NUM);
            if (student != null)
            {
                if (Type == "message")
                {
                    if (student.Tel.ToString().Trim().Length != 11)
                    {
                        return "";
                    }
                    string Tel = student.Tel.ToString().Substring(0, 3) + "****" + student.Tel.ToString().Substring(7, 4);
                    return Tel;
                }
                else if (Type == "question")
                {
                    if (student.Tel.ToString().Trim().Length != 11)
                    {
                        return "";
                    }
                    string Tel = student.Tel.ToString().Substring(0, 3) + "****" + student.Tel.ToString().Substring(7, 4);
                    return Tel;
                }
                else
                {
                    //未知错误
                    return "发生未知错误";
                }
            }
            else
            {
                T_Teacher teacher = db.T_Teacher.Find(ST_NUM);
                if (teacher != null)
                {
                    string Tel = teacher.Tel.ToString().Substring(0, 3) + "****" + teacher.Tel.ToString().Substring(7, 4);
                    return Tel;
                }
                else
                {
                    return "";
                }
            }
        }

        public ActionResult ST_NumCheck_question()
        {
            string ST_NUM = "";
            string Type = "";
            if (Request["ST_NUM"] != null)
            {
                ST_NUM = Request["ST_NUM"].ToString();
            }
            if (Request["Type"] != null)
            {
                Type = Request["Type"].ToString();
            }

            T_Student student = db.T_Student.Find(ST_NUM);
            if (student != null)
            {
                int num = 0;
                List<string> qlist = new List<string>();
                T_Account account = db.T_Account.Find(ST_NUM);

                if (account.Q1 != "")
                {
                    num++;
                    qlist.Add(account.Q1.ToString());
                }
                if (account.Q2 != "")
                {
                    num++;
                    qlist.Add(account.Q2.ToString());
                }
                if (account.Q3 != "")
                {
                    num++;
                    qlist.Add(account.Q3.ToString());
                }
                ViewBag.Question = num;
                return PartialView("_SecurityForm", qlist);
            }
            else
            {
                T_Teacher teacher = db.T_Teacher.Find(ST_NUM);
                if (teacher != null)
                {
                    ViewBag.Teacher = "1";
                    return PartialView("_SecurityForm");
                }
                else
                {
                    return null;
                }
            }
        }

        public string GetCode()
        {
            string code = "";
            if (Request["ST_NUM"] != null)
            {
                string ST_Num = Request["ST_NUM"].ToString();

                System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
                Random rd = new Random();
                for (int i = 0; i < 6; i++)
                {
                    int n = rd.Next(10);
                    code = code + n.ToString();
                }
                Session["code"] = code;

                T_Student student = db.T_Student.Find(ST_Num);
                if (student != null)
                {
                    if (ShortMessageClass.SendShortMessage(student.Name, student.ID, code, student.Tel, "FindPsd"))
                    {
                        return code;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    T_Teacher teacher = db.T_Teacher.Find(ST_Num);
                    if (teacher != null)
                    {
                        if (ShortMessageClass.SendShortMessage(teacher.Name, teacher.ID, code, teacher.Tel, "FindPsd"))
                        {
                            return code;
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            else
            {
                return "";
            }
        }

        public string codeCheck()
        {
            string code = "";
            string ST_Num = "";
            if (Request["code"] != null)
            {
                code = Request["code"].ToString();
            }
            if (Request["ST_NUM"] != null)
            {
                ST_Num = Request["ST_NUM"].ToString();
            }
            if (code == Session["code"].ToString())
            {
                T_Account account = db.T_Account.Find(ST_Num);
                if (account != null)
                {
                    return account.Psd.ToString();
                }
                return "";
            }
            else
            {
                return "";
            }
        }

        public string questionCheck()
        {
            string UserID = Request["ST_NUM"].ToString();

            string A1 = "";
            string A2 = "";
            string A3 = "";
            if (Request["A1"] != null)
            {
                A1 = Request["A1"].ToString();
            }
            if (Request["A2"] != null)
            {
                A2 = Request["A2"].ToString();
            }
            if (Request["A3"] != null)
            {
                A3 = Request["A3"].ToString();
            }

            T_Account account = db.T_Account.Find(UserID);
            if (account.A1 != "" && account.A1 != null)
            {
                if (account.A1 != A1)
                {
                    return "";
                }
            }
            if (account.A2 != "" && account.A2 != null)
            {
                if (account.A2 != A2)
                {
                    return "";
                }
            }
            if (account.A3 != "" && account.A3 != null)
            {
                if (account.A3 != A3)
                {
                    return "";
                }
            }

            return account.Psd;
        }
        #endregion

        #region 实用函数

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="fields"></param>
        public virtual void ShowNotify(FormCollection values)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("表单字段值：");
            sb.Append("<ul class=\"result\">");
            foreach (string key in values.Keys)
            {
                sb.AppendFormat("<li>{0}: {1}</li>", key, values[key]);
            }
            sb.Append("</ul>");

            ShowNotify(sb.ToString());
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public virtual void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            ShowNotify(message, messageIcon, Target.Top);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        /// <param name="target"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        {
            Notify n = new Notify();
            n.Target = target;
            n.Message = message;
            n.MessageBoxIcon = messageIcon;
            n.PositionX = Position.Center;
            n.PositionY = Position.Top;
            n.DisplayMilliseconds = 3000;
            n.ShowHeader = false;

            n.Show();
        }

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

        //暂时未使用到
        /// <summary>
        /// 获取网址的完整路径
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public string GetAbsoluteUrl(string virtualPath)
        {
            // http://benjii.me/2015/05/get-the-absolute-uri-from-asp-net-mvc-content-or-action/
            var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Content(virtualPath),
                Query = null,
            };

            return urlBuilder.ToString();
        }

        #endregion
    }
}