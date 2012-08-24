using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc3QA.Models.Services
{
    public class Department
    {
        private questiononlineContext db = new questiononlineContext();
        public List<Accounts_Department> GetDepartmentList()
        {
            return db.Accounts_Department.ToList();
        }
        public List<Accounts_Department> GetDepartmentList(string currentDpID)
        {
            Guid GuidDpID = new Guid(currentDpID);
            Accounts_Department acc = db.Accounts_Department.Find(GuidDpID);
          int listOrder =  acc.ListOrder+1;
          var query = from d in db.Accounts_Department
                      where d.ListOrder == listOrder
                      select d;

          return query.ToList();
        }
        /// <summary>
        /// 获取当前用户部门的上一个部门
        /// </summary>
        /// <param name="curDPID"></param>
        /// <returns></returns>
        public Guid GetPreDeparmentID(string curDPID)
        {
            Guid GDPID = new Guid(curDPID);
            Accounts_Department acc = db.Accounts_Department.Find(GDPID);
            int listOrder = acc.ListOrder - 1;
            var query =( from d in db.Accounts_Department
                        where d.ListOrder == listOrder
                        select d).Single();
            return query.DeptID;
        }


        /// <summary>
        /// 根据状态获取回复状态GUID
        /// </summary>
        /// <returns></returns>
        public Guid GetProblemState(string dpID)
        {
            Guid GuDpID=new Guid(dpID);
            var query =( from s in db.Pts_ProblemState
                        where s.DeptID == GuDpID && s.IsState == 1
                        select s).Single();
            return query.ProblemStateID;
        }
/// <summary>
/// 确定问题状态
/// </summary>
/// <param name="dpID"></param>
/// <returns></returns>
        public Guid GetProblemStateReplay(Guid dpID)
        {          
            var query = (from s in db.Pts_ProblemState
                         where s.DeptID == dpID && s.IsState == 2
                         select s).Single();
            return query.ProblemStateID;
        }
        /// <summary>
        /// 根据部门ID获取名称
        /// </summary>
        /// <param name="DepID"></param>
        /// <returns></returns>
        public string GetDepartmentName(string DepID)
        {
            string DepartmentName = "";
            DepartmentName = db.Accounts_Department.ToList().First(d => d.DeptID.Equals(new Guid(DepID))).DeptName;
           return DepartmentName;
        }
       
    }
}