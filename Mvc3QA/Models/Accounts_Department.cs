using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Accounts_Department
    {
        public System.Guid DeptID { get; set; }
        public string DeptName { get; set; }
        public int ListOrder { get; set; }
        public bool IsSubmit { get; set; }
    }
}
