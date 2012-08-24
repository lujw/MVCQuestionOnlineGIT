using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc3QA.Models;
using Common;
using res = Resource.Controllers.Admin.Admin;
using Mvc3QA.Models.Services;
using System.Web.Security;
using System.Web.Profile;
using Mvc3QA.General;
namespace Mvc3QA.Controllers
{
    [Authorize(Roles = "BusinessUsers")]
    public class ProblemsController : Controller
    {
        private questiononlineContext db = new questiononlineContext();
        GeneralConfigInfo configinfo = Mvc3QA.General.WebUtils.configinfo;
        //问题列表
        // GET: /Problems/

        public ViewResult Index(int? pageNo, int? cid, string sort, string order)
        {
            Problems clsProblems = new Problems();
            Category clsCategory = new Category();
            Pager pager = new Pager();
            int cateid = cid ?? 0;
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = 20;
            if (cateid > 0)
                pager = clsProblems.GetReplyPaging(pager, clsCategory.GetCategoryIds(cateid), sort, string.IsNullOrWhiteSpace(order) ? "desc" : order, User.Identity.Name);
            else
                pager = clsProblems.GetReplyPaging(pager, cateid, sort, string.IsNullOrWhiteSpace(order) ? "desc" : order, User.Identity.Name);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数            
            ViewBag.Cid = cateid;
            ViewBag.Sort = sort;
            ViewBag.Order = order;
            List<CategoryModel> catelist = clsCategory.getFCategoryList("1", "", " -- ");
            catelist.Insert(0, new CategoryModel
            {
                CateId = "0",
                CateName = res.SelectSubcategories
            });
            ViewData["CateId"] = new SelectList(catelist, "CateId", "CateName", cateid);
            return View(pager.Entity);
        }
        //
        // GET: /Problems/Details/5

        public ViewResult Details(Guid id)
        {
            var NewRecords = (from d in db.Pts_Records
                              where d.ProblemID == id
                              orderby d.CreateTime descending
                              select d).First();
            Category clsCategory = new Category();
            UserProfileModel userprofile = GetMyProfile();
            List<CategoryModel> catelist = clsCategory.getFCategoryList("1", "", " -- ");
            Pts_Problems clsPts_Problems = db.Pts_Problems.Find(id);
            ViewBag.CategoryID = catelist.Find(c => c.CateId.ToString() == clsPts_Problems.CategoryID.ToString()).CateName.ToString();
            Department clsDepartment = new Department();
            ViewBag.AssignTo = new SelectList(clsDepartment.GetDepartmentList(), "DeptID", "DeptName");
            ViewBag.Department = userprofile.Department;
            //显示修改按钮相关
            Problems clsProblems = new Problems();
            ViewBag.Edit = NewRecords.ProblemStateID.ToString() == "9c3f96c9-1869-41c4-92f5-f81c5f3e6e5c" ? "1" : "0";
            return View(clsPts_Problems);
        }

        //
        // GET: /Problems/Create

        public ActionResult Create()
        {
            string tid = "1";
            string cid = "1";
            UserProfileModel userprofile = GetMyProfile();
            ViewBag.showAssignTo = userprofile.Department;
            Department clsDepartment = new Department();
            Category clsCategory = new Category();
            ViewData["CategoryID"] = new SelectList(clsCategory.getFCategoryList(tid, "", " -- "), "CateId", "CateName", cid);
            ViewBag.AssignedTo = new SelectList(clsDepartment.GetDepartmentList(userprofile.Department), "DeptID", "DeptName");
            ViewBag.CurrentItem = "t" + tid;
            if (tid == "4")
                return View("AdminAlbumAdd");

            return View();
        }

        //
        // POST: /Problems/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Pts_Problems model)
        {
            //添加[ValidateInput(false)]特性，否则提交内容有html代码会报错
            if (ModelState.IsValid)
            {
                UserProfileModel userprofile = GetMyProfile();

                Pts_Problems obj = new Pts_Problems();
                Pts_Records modelPts_Records = new Pts_Records();
                Department clsDepartment = new Department();
                obj.ProblemID = Guid.NewGuid();
                obj.CreateTime = DateTime.Now;
                obj.CategoryID = model.CategoryID;
                obj.Title = Utils.FileterStr(model.Title);
                obj.Description = Utils.NoHTML(string.IsNullOrWhiteSpace(model.Description) ? "" : model.Description);
                obj.Content = Server.UrlDecode(Utils.DownloadImages(model.Content, "/Content/Upload/", configinfo.Weburl));
                //obj.ip = Utils.GetIP();               
                obj.CreateUser = User.Identity.Name;
                obj.StartTime = model.StartTime;
                obj.CloseTime = model.CloseTime;
                obj.AssignedTo = model.AssignedTo;
                obj.AssignedToUser = model.AssignedToUser;
                obj.IsStart = true;
                obj.IsClosed = false;
                obj.HandlingUser = model.AssignedToUser;
                obj.CreatUserName = userprofile.NickName;
                //回复表中添加记录                  
                modelPts_Records.RecordID = Guid.NewGuid();
                modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                modelPts_Records.AssignTo = model.AssignedTo;
                modelPts_Records.AssignToObjectID = model.AssignedToUser;
                modelPts_Records.AssignType = 0;
                modelPts_Records.Content = "新提交问题";
                //modelPts_Records.Title = pts_Records.Title;
                modelPts_Records.CreateUser = User.Identity.Name;
                modelPts_Records.CreateTime = DateTime.Now;
                modelPts_Records.ProblemID = obj.ProblemID;
                modelPts_Records.SrcStateID = new Guid("00000000-0000-0000-0000-000000000000");
                modelPts_Records.SrcUserID = User.Identity.Name;
                modelPts_Records.Describe = "新提交问题";
                modelPts_Records.ListOrder = 1;
                //提交问题记录相关信息
                if (model.AssignedToUser == "0")
                {

                    modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(model.AssignedTo) + "(部门所有人)";

                }
                else
                {
                    modelPts_Records.Course = userprofile.NickName + "-->" + clsDepartment.GetDepartmentName(model.AssignedTo) + "(" + GetUserProfile(model.AssignedToUser) + ")";

                }
                //提交问题后状态
                modelPts_Records.ProblemStateID = clsDepartment.GetProblemState(model.AssignedTo);
                obj.ProblemStateID = clsDepartment.GetProblemState(model.AssignedTo);//问题表中状态               
                db.Pts_Problems.Add(obj);
                db.Pts_Records.Add(modelPts_Records);
                db.SaveChanges();


                return RedirectToAction("Index", new { tid = 1, cid = obj.CategoryID });
            }
            return View(model);
        }

        //
        // GET: /Problems/Edit/5

        public ActionResult Edit(int id)
        {
            Pts_ProblemCategory pts_problemcategory = db.Pts_ProblemCategory.Find(id);
            return View(pts_problemcategory);
        }

        //
        // POST: /Problems/Edit/5

        [HttpPost]
        public ActionResult Edit(Pts_ProblemCategory pts_problemcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pts_problemcategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pts_problemcategory);
        }

        //
        // GET: /Problems/Delete/5

        public ActionResult Delete(int id)
        {
            Pts_ProblemCategory pts_problemcategory = db.Pts_ProblemCategory.Find(id);
            return View(pts_problemcategory);
        }

        //
        // POST: /Problems/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Pts_ProblemCategory pts_problemcategory = db.Pts_ProblemCategory.Find(id);
            db.Pts_ProblemCategory.Remove(pts_problemcategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 根据部门获取用户列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDeptUser(string id)
        {
            try
            {
                if (Request.IsAjaxRequest())
                {
                    List<SelectListItem> ListS = new List<SelectListItem>();
                    SelectListItem sl = new SelectListItem();
                    sl.Text = "部门所有人可见";
                    sl.Value = "0";
                    ListS.Add(sl);
                    MembershipUserCollection users = Membership.GetAllUsers();
                    foreach (MembershipUser item in users)
                    {
                        sl = new SelectListItem();
                        ProfileBase objProfile = System.Web.Profile.ProfileBase.Create(item.UserName);
                        if (GetUserProfileItem(objProfile, "Department") == id)
                        {
                            sl.Text = GetUserProfileItem(objProfile, "NickName");
                            sl.Value = item.UserName;
                            ListS.Add(sl);
                        }

                    }
                    //.OrderBy(a => System.Web.Profile.ProfileBase.Create(a.UserName).GetPropertyValue("FirstName"));
                    return Json(ListS);
                }
                else
                {
                    return Content("-1");
                }
            }
            catch
            {
                return Content("-1");
            }
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
        /// 获取指定用户的信息
        /// </summary>
        /// <returns></returns>
        private string GetUserProfile(string UserID)
        {
            ProfileBase objProfile = System.Web.Profile.ProfileBase.Create(UserID);
            return GetUserProfileItem(objProfile, "NickName");
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
        public ActionResult QuestionCount()
        {
            Problems clsProblems = new Problems();
            ViewBag.MyReplayCount = clsProblems.MyReplayQuestionCount(User.Identity.Name);
            ViewBag.NewQuestion = clsProblems.NewQuestionsCount();
            ViewBag.QuestionCount = clsProblems.QuestionCount();
            ViewBag.SolvedQuestionCount = clsProblems.SolvedQuestionCount();
            ViewBag.MySolvedQuestionCount = clsProblems.MYSolvedQuestionCount(User.Identity.Name);
            return PartialView("_Console");
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}