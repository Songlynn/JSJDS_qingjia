//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace qingjia_MVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_BatchLeave
    {
        public string ID { get; set; }
        public string TypeID { get; set; }
        public string TypeChildID { get; set; }
        public string Reason { get; set; }
        public Nullable<System.DateTime> SubmitTime { get; set; }
        public string StateLeave { get; set; }
        public string StateBack { get; set; }
        public Nullable<System.DateTime> TimeLeave { get; set; }
        public Nullable<System.DateTime> TimeBack { get; set; }
        public string AuditTeacherID { get; set; }
        public string Notes { get; set; }
        public string StudentsID { get; set; }
        public string LeaveInfo { get; set; }
    }
}