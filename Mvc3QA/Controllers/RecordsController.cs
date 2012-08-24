using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc3QA.Models;
using Mvc3QA.Models.Services;
using System.Web.Profile;
using System.Web.Security;
using System.Data.Entity.Validation;
using Common;

namespace Mvc3QA.Controllers
{
    //[Authorize(Roles = "BusinessUsers")]
    public class RecordsController : Controller
    {
        private questiononlineContext db = new questiononlineContext();

        //
        // GET: /RecordS/

        public ViewResult Index()
        {
            var pts_records = db.Pts_Records.Include(p => p.Pts_Problems).Include(p => p.Pts_ProblemState);
            return View(pts_records.ToList());
        }

        //
        // GET: /RecordS/Details/5

        public ViewResult Details(Guid id)
        {
            Pts_Records pts_records = db.Pts_Records.Find(id);
            return View(pts_records);
        }

        //其他部门人员回复问题
        // GET: /RecordS/Create

        public ActionResult Create(Guid id)
        {
            Pts_Records modelRecords = new Pts_Records();
            modelRecords.ProblemID = id;
            Department clsDepartment = new Department();
            UserProfileModel userprofile = GetMyProfile();
            ViewBag.AssignTo = new SelectList(clsDepartment.GetDepartmentList(userprofile.Department), "DeptID", "DeptName");
            return View(modelRecords);
        }

        //
        // POST: /RecordS/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Pts_Records pts_Records)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Pts_Records modelPts_Records = new Pts_Records();
                    var NewRecords = (from d in db.Pts_Records
                                      where d.ProblemID == pts_Records.ProblemID
                                      orderby d.CreateTime descending
                                      select d).First();
                    var LastRecords = (from d in db.Pts_Records
                                       where d.ProblemID == pts_Records.ProblemID
                                       orderby d.CreateTime ascending
                                       select d).First();
                    var upDateProblems = (from d in db.Pts_Problems
                                          where d.ProblemID == pts_Records.ProblemID
                                          select d).Single();
                    Department clsDepartment = new Department();
                    UserProfileModel userprofile = GetMyProfile();
                    if ((pts_Records.AssignType == 1) || (pts_Records.AssignType == 3))//直接解决或者不不解决转交上一个人
                    {
                        modelPts_Records.AssignTo = "1";
                        modelPts_Records.AssignType = 0;
                        modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");

                        //根据问题最终处理人
                        if (NewRecords.Pts_ProblemState.ListOrder==1)
                        {
                            upDateProblems.SolveUser = User.Identity.Name;
                        }

                        if (LastRecords.CreateUser == User.Identity.Name || (GetUserDP(User.Identity.Name) == "bbf977d8-aeab-4bec-b070-a3d1c1b41cb5"))//第一条记录创建人和当前用户，判断是否是问题提交人GetUserDP(ReplayRecords.CreateUser)
                        {
                            modelPts_Records.AssignToObjectID = userprofile.NickName;
                            modelPts_Records.ProblemStateID = new Guid("aa36df43-a68b-45e2-8230-21b2f6f5fce2");//问题已解决
                            modelPts_Records.Course = "-->" + userprofile.NickName;
                            upDateProblems.IsClosed = true;
                        }
                        else
                        {
                            //上一个部门
                            Guid preDPID = clsDepartment.GetPreDeparmentID(userprofile.Department);

                            if (NewRecords.Pts_ProblemState.IsState == 2)//最新一条记录状态为待确定的
                            {
                                //获取当前记录的隔一条记录
                                var ReplayRecords = (from d in db.Pts_Records
                                                     where d.ProblemID == pts_Records.ProblemID && (d.ListOrder == NewRecords.ListOrder - 2)
                                                     orderby d.CreateTime descending
                                                     select d).First();
                                if (ReplayRecords.Pts_ProblemState.IsState == 2)
                                {//解决打回问题
                                    var ReplayRecordsDH = (from d in db.Pts_Records
                                                           where d.ProblemID == pts_Records.ProblemID && (d.ListOrder == ReplayRecords.ListOrder - 2)
                                                           orderby d.CreateTime descending
                                                           select d).First();
                                    modelPts_Records.AssignToObjectID = ReplayRecordsDH.CreateUser;
                                    modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(GetUserDP(ReplayRecordsDH.CreateUser)) + "(" + GetUserProfile(ReplayRecordsDH.CreateUser) + ")";
                                }
                                else
                                {
                                    modelPts_Records.AssignToObjectID = ReplayRecords.CreateUser;
                                    modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(GetUserDP(ReplayRecords.CreateUser)) + "(" + GetUserProfile(ReplayRecords.CreateUser) + ")";
                                }

                            }
                            else
                            {
                                modelPts_Records.AssignToObjectID = NewRecords.CreateUser;
                                modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(preDPID.ToString()) + "(" + GetUserProfile(NewRecords.CreateUser) + ")";

                            }

                            modelPts_Records.ProblemStateID = clsDepartment.GetProblemStateReplay(preDPID);
                        }

                        modelPts_Records.SrcUserID = NewRecords.CreateUser;
                    }
                    if (pts_Records.AssignType == 2)//转交其他人
                    {
                        modelPts_Records.AssignTo = pts_Records.AssignTo;
                        modelPts_Records.AssignToObjectID = pts_Records.AssignToObjectID;
                        modelPts_Records.AssignType = 2;
                        modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                        modelPts_Records.ProblemStateID = clsDepartment.GetProblemState(pts_Records.AssignTo);
                        modelPts_Records.SrcUserID = NewRecords.CreateUser;
                        //提交问题记录相关信息
                        if (pts_Records.AssignToObjectID == "0")
                        {

                            modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(pts_Records.AssignTo) + "(部门所有人)";

                        }
                        else
                        {
                            modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(pts_Records.AssignTo) + "(" + GetUserProfile(pts_Records.AssignToObjectID) + ")";

                        }
                    }
                    modelPts_Records.RecordID = Guid.NewGuid();
                    modelPts_Records.Content = pts_Records.Content;
                    modelPts_Records.Describe = Utils.NoHTML(string.IsNullOrWhiteSpace(pts_Records.Content) ? "" : pts_Records.Content);
                    modelPts_Records.CreateUser = User.Identity.Name;
                    modelPts_Records.CreateTime = DateTime.Now;
                    modelPts_Records.ProblemID = pts_Records.ProblemID;
                    modelPts_Records.ListOrder = NewRecords.ListOrder + 1;
                    modelPts_Records.ResponseTotalTime = Utils.TimeDifference(NewRecords.CreateTime);
                    db.Pts_Records.Add(modelPts_Records);
                    //更新问题表待调整

                    upDateProblems.IsStart = false;
                    upDateProblems.HandlingUser = modelPts_Records.AssignToObjectID;
                    upDateProblems.ProblemStateID = modelPts_Records.ProblemStateID;
                    int result = db.SaveChanges();
                    return RedirectToAction("Details", "Problems", new { id = modelPts_Records.ProblemID });
                }
                else
                {
                    return Content("-1");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                return Content("-1");
            }
        }
        //研发人员回复问题
        // GET: /RecordS/Create

        public ActionResult YFCreate(Guid id)
        {
            Pts_Records modelRecords = new Pts_Records();
            modelRecords.ProblemID = id;
            Department clsDepartment = new Department();
            UserProfileModel userprofile = GetMyProfile();
            ViewBag.AssignTo = new SelectList(clsDepartment.GetDepartmentList(userprofile.Department), "DeptID", "DeptName");
            return View(modelRecords);
        }

        /// <summary>
        /// 研发回复问题
        /// </summary>
        /// <param name="pts_Records"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult YFCreate(Pts_Records pts_Records)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Pts_Records modelPts_Records = new Pts_Records();
                    var NewRecords = (from d in db.Pts_Records
                                      where d.ProblemID == pts_Records.ProblemID
                                      orderby d.CreateTime descending
                                      select d).First();
                    var LastRecords = (from d in db.Pts_Records
                                       where d.ProblemID == pts_Records.ProblemID
                                       orderby d.CreateTime ascending
                                       select d).First();
                    var upDateProblems = (from d in db.Pts_Problems
                                          where d.ProblemID == pts_Records.ProblemID
                                          select d).Single();
                    Department clsDepartment = new Department();
                    UserProfileModel userprofile = GetMyProfile();
                    if ((pts_Records.AssignType == 1) || (pts_Records.AssignType == 3))//直接解决或者不不解决转交上一个人
                    {
                        modelPts_Records.AssignTo = "1";
                        modelPts_Records.AssignType = 0;

                        //根据问题最终处理人                       
                         upDateProblems.SolveUser = User.Identity.Name;
                    

                        modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                        if (LastRecords.CreateUser == User.Identity.Name)//第一条记录创建人和当前用户，判断是否是问题提交人
                        {
                            modelPts_Records.AssignToObjectID = NewRecords.CreateUser;
                            modelPts_Records.ProblemStateID = new Guid("aa36df43-a68b-45e2-8230-21b2f6f5fce2");//问题已解决
                            modelPts_Records.Course = "-->" + userprofile.NickName;
                            upDateProblems.IsClosed = true;
                        }
                        else
                        {
                            //上一个部门
                            Guid preDPID = clsDepartment.GetPreDeparmentID(userprofile.Department);

                            if (NewRecords.Pts_ProblemState.IsState == 2)//最新一条记录状态为待确定的
                            {
                                //获取当前记录的隔一条记录
                                var ReplayRecords = (from d in db.Pts_Records
                                                     where d.ProblemID == pts_Records.ProblemID && (d.ListOrder == NewRecords.ListOrder - 1)
                                                     orderby d.CreateTime descending
                                                     select d).First();
                                //不存在此情况
                                if (ReplayRecords.Pts_ProblemState.IsState == 2)
                                {//解决打回问题
                                    var ReplayRecordsDH = (from d in db.Pts_Records
                                                           where d.ProblemID == pts_Records.ProblemID && (d.ListOrder == ReplayRecords.ListOrder - 2)
                                                           orderby d.CreateTime descending
                                                           select d).First();
                                    modelPts_Records.AssignToObjectID = ReplayRecordsDH.CreateUser;
                                    modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(GetUserDP(ReplayRecordsDH.CreateUser)) + "(" + GetUserProfile(ReplayRecordsDH.CreateUser) + ")";
                                }
                                else
                                {
                                    modelPts_Records.AssignToObjectID = ReplayRecords.CreateUser;
                                    modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(GetUserDP(ReplayRecords.CreateUser)) + "(" + GetUserProfile(ReplayRecords.CreateUser) + ")";
                                }

                            }
                            else
                            {
                                modelPts_Records.AssignToObjectID = NewRecords.CreateUser;
                                modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(preDPID.ToString()) + "(" + GetUserProfile(NewRecords.CreateUser) + ")";

                            }

                            modelPts_Records.ProblemStateID = clsDepartment.GetProblemStateReplay(preDPID);
                        }

                        modelPts_Records.SrcUserID = NewRecords.CreateUser;
                        modelPts_Records.ListOrder = NewRecords.ListOrder +1;
                    }
                    if (pts_Records.AssignType == 2)//转交其他人
                    {
                        modelPts_Records.AssignTo = "2ba34c68-9c2c-4e06-9e17-ecd2e1156438";//部门研发
                        modelPts_Records.AssignToObjectID = User.Identity.Name;
                        modelPts_Records.AssignType = 2;
                        modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                        modelPts_Records.ProblemStateID = new Guid("9c3f96c9-1869-41c4-92f5-f81c5f3e6e5c");//研发中
                        modelPts_Records.SrcUserID = NewRecords.CreateUser;
                        modelPts_Records.YFWay = pts_Records.YFWay;
                        modelPts_Records.YFTime = pts_Records.YFTime;
                        //提交问题记录相关信息                      
                        modelPts_Records.Course = "--研发中：预计时间（" + string.Format("{0:d}", pts_Records.YFTime) + "）--";
                     //研发状态特殊处理
                        modelPts_Records.ListOrder = NewRecords.ListOrder +1;

                    }
                    modelPts_Records.RecordID = Guid.NewGuid();
                    modelPts_Records.Content = pts_Records.Content;
                    modelPts_Records.Describe = Utils.NoHTML(string.IsNullOrWhiteSpace(pts_Records.Content) ? "" : pts_Records.Content);
                    modelPts_Records.CreateUser = User.Identity.Name;
                    modelPts_Records.CreateTime = DateTime.Now;
                    modelPts_Records.ProblemID = pts_Records.ProblemID;
                   
                    modelPts_Records.ResponseTotalTime = Utils.TimeDifference(NewRecords.CreateTime);
                    db.Pts_Records.Add(modelPts_Records);
                    //更新问题表待调整

                    upDateProblems.IsStart = false;
                    upDateProblems.HandlingUser = modelPts_Records.AssignToObjectID;
                    upDateProblems.ProblemStateID = modelPts_Records.ProblemStateID;
                    int result = db.SaveChanges();
                    return RedirectToAction("Details", "Problems", new { id = modelPts_Records.ProblemID });
                }
                else
                {
                    return Content("-1");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                return Content("-1");
            }
        }
        //
        // GET: /RecordS/Edit/5

        public ActionResult Edit(Guid id)
        {
            Pts_Records pts_records = db.Pts_Records.Find(id);
            ViewBag.ProblemID = new SelectList(db.Pts_Problems, "ProblemID", "Title", pts_records.ProblemID);
            ViewBag.ProblemStateID = new SelectList(db.Pts_ProblemState, "ProblemStateID", "StateName", pts_records.ProblemStateID);
            return View(pts_records);
        }

        //
        // POST: /RecordS/Edit/5

        [HttpPost]
        public ActionResult Edit(Pts_Records pts_records)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pts_records).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProblemID = new SelectList(db.Pts_Problems, "ProblemID", "Title", pts_records.ProblemID);
            ViewBag.ProblemStateID = new SelectList(db.Pts_ProblemState, "ProblemStateID", "StateName", pts_records.ProblemStateID);
            return View(pts_records);
        }

        //
        // GET: /RecordS/Delete/5

        public ActionResult Delete(Guid id)
        {
            Pts_Records pts_records = db.Pts_Records.Find(id);
            return View(pts_records);
        }

        //
        // POST: /RecordS/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Pts_Records pts_records = db.Pts_Records.Find(id);
            db.Pts_Records.Remove(pts_records);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateTime(string pid, string times)
        {
            Guid probleID = new Guid(pid);
            var NewRecords = (from d in db.Pts_Records
                              where d.ProblemID == probleID
                              orderby d.CreateTime descending
                              select d).First();

            var upDateProblems = (from d in db.Pts_Problems
                                  where d.ProblemID == probleID
                                  select d).Single();
            //比较期望解决时间如果，大于则不更新
            if (DateTime.Compare(upDateProblems.CloseTime, Convert.ToDateTime(times))  < 0)
            {
                upDateProblems.CloseTime = Convert.ToDateTime(times);

            }
            NewRecords.Course = "--研发中：预计时间（" + times + "）--";
            NewRecords.YFTime = Convert.ToDateTime(times);
            db.SaveChanges();
            return Content("1");
        }
        /// <summary>
        /// 获取指定用户的信息
        /// </summary>
        /// <returns></returns>
        private string GetUserProfile(string UserID)
        {
            ProfileBase objProfile = System.Web.Profile.ProfileBase.Create(UserID);
            return GetUserProfileItem(objProfile, "NickName");
        }
        private string GetUserDP(string UserID)
        {
            ProfileBase objProfile = System.Web.Profile.ProfileBase.Create(UserID);
            return GetUserProfileItem(objProfile, "Department");
        }
        /// <summary>
        /// 获取用户ProfileItem
        /// </summary>
        private string GetUserProfileItem(ProfileBase objProfile, string key, string defaultvalue = "")
        {
            string value = objProfile.GetPropertyValue(key).ToString();
            return string.IsNullOrEmpty(value) ? defaultvalue : value;
        }
        /// <summary>
        /// 获取当前用户Profile
        /// </summary>
        private UserProfileModel GetMyProfile()
        {
            UserProfileModel userprofile = new UserProfileModel();
            try
            {
                userprofile.NickName = GetProfileItem("nickname");
                userprofile.Signature = GetProfileItem("signature");
                userprofile.Intro = GetProfileItem("intro");
                userprofile.Gender = GetProfileItem("gender", "1");
                userprofile.Department = GetProfileItem("Department", "00000000-0000-0000-0000-000000000000");
                userprofile.Birth = GetProfileItem("birth");
                userprofile.Location = GetProfileItem("location");
                userprofile.Website = GetProfileItem("website");
                userprofile.QQ = GetProfileItem("qq");
                userprofile.Sina = GetProfileItem("sina");
                userprofile.Facebook = GetProfileItem("facebook");
                userprofile.Twitter = GetProfileItem("twitter");
                userprofile.Medals = GetProfileItem("medals");
                userprofile.Phone = GetProfileItem("phone");

                string email = GetProfileItem("email");
                if (string.IsNullOrEmpty(email))
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, false /* userIsOnline */);
                    userprofile.Email = currentUser.Email;
                }
                else
                {
                    userprofile.Email = email;
                }
                userprofile.IsSendEmail = GetProfileItem("isSendEmail", "1");
            }
            catch
            { }
            return userprofile;
        }
        /// <summary>
        /// 获取当前用户ProfileItem
        /// </summary>
        private string GetProfileItem(string key, string defaultvalue = "")
        {
            string value = (String)HttpContext.Profile.GetPropertyValue(key);
            return string.IsNullOrEmpty(value) ? defaultvalue : value;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}