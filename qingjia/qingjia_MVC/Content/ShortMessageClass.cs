using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Sms.Model.V20160927;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ShortMessage
{
    public class ShortMessageClass
    {

        /// <summary>
        /// 发送通知短信
        /// </summary>
        /// <param name="ST_Name">学生姓名</param>
        /// <param name="LV_Num">请假单号</param>
        /// <param name="ST_Tel">电话号码</param>
        /// <param name="MessageType">短信类型</param>
        /// <returns></returns>
        public static bool SendShortMessage(string ST_Name, string ST_NUM, string LV_Num, string ST_Tel, string MessageType)
        {
            //AccessKey 和 AccessKeyCode
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAI7W5SRT92SGZD", "F7Gv1zZvwHYHLbkSIXnn1Dx9HUIi0K");
            IAcsClient client = new DefaultAcsClient(profile);
            SingleSendSmsRequest request = new SingleSendSmsRequest();
            try
            {
                //短信签名  【请假系统】
                request.SignName = "请假系统";
                if (MessageType == "go")
                {
                    //请加成功模板
                    request.TemplateCode = "SMS_27325377";
                }
                else if (MessageType == "back")
                {
                    //销假成功模板
                    request.TemplateCode = "SMS_27495348";
                }
                else if (MessageType == "failed")
                {
                    //驳回请假模板
                    request.TemplateCode = "SMS_27620081";
                }
                else if (MessageType == "FindPsd")
                {
                    //短信验证找回密码
                    request.TemplateCode = "SMS_60140885";
                }
                else
                {
                    return false;
                }
                request.RecNum = ST_Tel;
                request.ParamString = "{\"name\":\"" + ST_Name + "\",\"lvnum\":\"" + LV_Num + "\"}";
                SingleSendSmsResponse httpResponse = client.GetAcsResponse(request);

                //SaveMessageList(ST_NUM, LV_Num, ST_Tel, MessageType);

                return true;
            }
            catch (ServerException e)
            {
                return false;
            }
            catch (ClientException e)
            {
                return false;
            }
        }

        /// <summary>
        /// 将发送的短信内容保存至数据库
        /// </summary>
        /// <param name="ST_NUM"></param>
        /// <param name="LV_Num"></param>
        /// <param name="ST_Tel"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        private static bool SaveMessageList(string ST_NUM, string LV_Num, string ST_Tel, string MessageType)
        {
            string connString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
            string timeString = DateTime.Now.ToString();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string cmdString = "INSERT INTO SendList VALUES (' " + LV_Num + "','" + ST_NUM + "','" + MessageType + "','" + ST_Tel + "','" + timeString + "')";
                int flag = 0;

                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    flag = (int)cmd.ExecuteNonQuery();
                }

                if (flag == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="teacherName">教室姓名</param>
        /// <param name="text">短信内容</param>
        /// <param name="ST_Tel">学生电话</param>
        /// <returns></returns>
        public static bool SendShortMessage(string teacherName, string text, string ST_Tel)
        {
            //AccessKey 和 AccessKeyCode
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAI7W5SRT92SGZD", "F7Gv1zZvwHYHLbkSIXnn1Dx9HUIi0K");
            IAcsClient client = new DefaultAcsClient(profile);
            SingleSendSmsRequest request = new SingleSendSmsRequest();
            try
            {

                //短信签名  【请假系统】
                request.SignName = "请假系统";
                request.TemplateCode = "SMS_63430002";

                request.RecNum = ST_Tel;
                request.ParamString = "{\"name\":\"" + teacherName + "\",\"text\":\"" + text + "\"}";
                SingleSendSmsResponse httpResponse = client.GetAcsResponse(request);

                return true;
            }
            catch (ServerException e)
            {
                return false;
            }
        }
    }
}