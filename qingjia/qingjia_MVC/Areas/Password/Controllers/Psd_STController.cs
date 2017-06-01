using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qingjia_MVC.Models;
using qingjia_MVC.Controllers;

namespace qingjia_MVC.Areas.Password.Controllers
{
    public class Psd_STController : BaseController
    {
        //实例化数据库
        imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        // GET: Password/Psd_ST
        public ActionResult Index()
        {
            return View();
        }

        public string ChangePsd()
        {
            string ST_NUM = Session["UserID"].ToString();
            string oldPsd = "";
            string newPsd = "";
            if (Request["oldPsd"] != null)
            {
                oldPsd = Request["oldPsd"].ToString();
            }
            if (Request["newPsd"] != null)
            {
                newPsd = Request["newPsd"].ToString();
            }

            T_Account account = db.T_Account.Find(ST_NUM);
            if (account != null)
            {
                if (account.Psd.ToString() == oldPsd)
                {
                    account.Psd = newPsd;
                    db.SaveChanges();
                    return "修改密码成功！";//保存成功
                }
                else
                {
                    return "原密码输入错误，请重新输入！";//密码错误
                }
            }
            else
            {
                return "未知错误，请联系管理员!";//未知错误，请联系管理员
            }
        }

        public string SecuritySet()
        {
            string ST_NUM = Session["UserID"].ToString();
            string Q1 = Request["Q1"].ToString();
            string A1 = Request["A1"].ToString();
            string Q2 = Request["Q2"].ToString();
            string A2 = Request["A2"].ToString();
            string Q3 = Request["Q3"].ToString();
            string A3 = Request["A3"].ToString();
            T_Account account = db.T_Account.Find(ST_NUM);
            if (Q1 != "" && A1 != "")
            {
                if (account.Q1 == "")
                {
                    account.Q1 = Q1;
                    account.A1 = A1;
                }
                else if (account.Q2 == "")
                {
                    account.Q2 = Q1;
                    account.A2 = A1;
                }
                else if (account.Q3 == "")
                {
                    account.Q3 = Q1;
                    account.A3 = A1;
                }
                else
                {
                    return "";
                }
            }
            if (Q2 != "" && A2 != "")
            {
                if (account.Q1 == "")
                {
                    account.Q1 = Q2;
                    account.A1 = A2;
                }
                else if (account.Q2 == "")
                {
                    account.Q2 = Q2;
                    account.A2 = A2;
                }
                else if (account.Q3 == "")
                {
                    account.Q3 = Q2;
                    account.A3 = A2;
                }
                else
                {
                    return "";
                }
            }
            if (Q3 != "" && A3 != "")
            {
                if (account.Q1 == "")
                {
                    account.Q1 = Q3;
                    account.A1 = A3;
                }
                else if (account.Q2 == "")
                {
                    account.Q2 = Q3;
                    account.A2 = A3;
                }
                else if (account.Q3 == "")
                {
                    account.Q3 = Q3;
                    account.A3 = A3;
                }
                else
                {
                    return "";
                }
            }
            db.SaveChanges();
            return "添加密保成功";
        }
    }
}