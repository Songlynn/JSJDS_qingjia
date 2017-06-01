using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FineUIMvc;
using System.Text;
using System.Xml;

namespace qingjia_MVC.Controllers
{
    public class BaseController : Controller
    {
        ///// <summary>
        ///// 显示通知对话框
        ///// </summary>
        ///// <param name="message"></param>
        //public virtual void ShowNotify(string message)
        //{
        //    ShowNotify(message, MessageBoxIcon.Information);
        //}

        ///// <summary>
        ///// 显示通知对话框
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="messageIcon"></param>
        //public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        //{
        //    ShowNotify(message, messageIcon, Target.Top);
        //}

        ///// <summary>
        ///// 显示通知对话框
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="messageIcon"></param>
        ///// <param name="target"></param>
        //public virtual void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        //{
        //    Notify n = new Notify();
        //    n.Target = target;
        //    n.Message = message;
        //    n.MessageBoxIcon = messageIcon;
        //    n.PositionX = Position.Center;
        //    n.PositionY = Position.Top;
        //    n.DisplayMilliseconds = 3000;
        //    n.ShowHeader = false;

        //    n.Show();
        //}

        #region static readonly


        #endregion

        #region OnActionExecuting

        /// <summary>
        /// 动作方法调用之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //if (Session["UserID"] == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Index" }));
            //}
            if (filterContext.HttpContext.Session != null)
            {
                //if (filterContext.HttpContext.Session.IsNewSession)
                //{
                //    var sessionCookie = filterContext.HttpContext.Request.Headers["Cookie"];
                //    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId", StringComparison.OrdinalIgnoreCase) >= 0))
                //    {
                //        //string url = filterContext.RequestContext.HttpContext.Request.ApplicationPath.ToString();
                //        string Url = filterContext.HttpContext.Request.RawUrl.ToString();
                //        if (Url != "/Account/Index")
                //        {
                //            filterContext.Result = new RedirectResult("/Account/Index");
                //        }
                //        //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Account", Action = "Index", returnUrl = filterContext.HttpContext.Request.RawUrl }));//这里是跳转到Account下的LogOff,自己定义
                //    }
                //}

                var loginUser = Session["UserID"];
                if (loginUser == null)
                {
                    string Url = filterContext.HttpContext.Request.RawUrl.ToString();
                    if (Url != "/Account/Index")
                    {
                        filterContext.Result = new RedirectResult("/Account/Index");
                    }
                }
            }
        }


        #endregion

        #region 上传文件类型判断

        protected readonly static List<string> VALID_FILE_TYPES = new List<string> { "jpg", "bmp", "gif", "jpeg", "png" };

        protected static bool ValidateFileType(string fileName)
        {
            string fileType = String.Empty;
            int lastDotIndex = fileName.LastIndexOf(".");
            if (lastDotIndex >= 0)
            {
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();
            }

            if (VALID_FILE_TYPES.Contains(fileType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region 表格相关

        /// <summary>
        /// 选中了哪些行
        /// </summary>
        /// <param name="grid">表格对象</param>
        /// <returns>选中行的描述信息</returns>
        protected string HowManyRowsAreSelected(Grid grid)
        {
            StringBuilder sb = new StringBuilder();
            int selectedCount = grid.SelectedRowIndexArray.Length;
            if (selectedCount > 0)
            {
                sb.AppendFormat("<p><strong>共选中了 {0} 行：</strong></p>", selectedCount);
                sb.Append("<table class=\"result\">");

                sb.Append("<tr><th>行号</th>");
                //foreach (string datakey in grid.DataKeyNames)
                //{
                //    sb.AppendFormat("<th>{0}</th>", datakey);
                //}
                sb.Append("</tr>");


                for (int i = 0; i < selectedCount; i++)
                {
                    int rowIndex = grid.SelectedRowIndexArray[i];
                    sb.Append("<tr>");

                    int rownumber = rowIndex + 1;
                    if (grid.AllowPaging)
                    {
                        rownumber += grid.PageIndex * grid.PageSize;
                    }
                    sb.AppendFormat("<td>{0}</td>", rownumber);


                    // 如果是内存分页，所有分页的数据都存在，rowIndex 就是在全部数据中的顺序，而不是当前页的顺序
                    if (grid.AllowPaging && !grid.IsDatabasePaging)
                    {
                        rowIndex = grid.PageIndex * grid.PageSize + rowIndex;
                    }

                    //object[] dataKeys = grid.DataKeys[rowIndex];
                    //for (int j = 0; j < dataKeys.Length; j++)
                    //{
                    //    sb.AppendFormat("<td>{0}</td>", dataKeys[j]);
                    //}

                    sb.Append("</tr>");
                }
                sb.Append("</table>");
            }
            else
            {
                sb.Append("<strong>没有选中任何一行！</strong>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取性别的字面值，在 ASPX 中调用
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        protected string GetGender(object gender)
        {
            if (Convert.ToInt32(gender) == 1)
            {
                return "男";
            }
            else
            {
                return "女";
            }
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


        /// <summary>
        /// 加载XML文件
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        protected XmlDocument LoadXml(string xmlPath)
        {
            // 加载XML配置文件
            xmlPath = Server.MapPath(xmlPath);
            string xmlContent = String.Empty;
            using (StreamReader sr = new StreamReader(xmlPath))
            {
                xmlContent = sr.ReadToEnd();
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            return doc;
        }

        #endregion

        #region

        public string getKey(string type, string key)
        {
            string result = "Error";
            switch (type)
            {
                case "leavetype":
                    if (key == "00")
                    {
                        result = "待审核";
                    }
                    else if (key == "10")
                    {
                        result = "待销假";
                    }
                    else if (key == "11")
                    {
                        result = "已销假";
                    }
                    if (key == "21")
                    {
                        result = "已驳回";
                    }
                    break;
            }
            return result;
        }

        #endregion
    }
}