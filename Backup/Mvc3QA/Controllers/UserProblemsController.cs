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
    [Authorize(Roles = "RegisteredUsers")]
    public class UserProblemsController : Controller
    {
        private questiononlineContext db = new questiononlineContext();
        GeneralConfigInfo configinfo = Mvc3QA.General.WebUtils.configinfo;
        /// <summary>
        /// 用户登录界面
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="cid"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ViewResult Index(int? pageNo, int? cid, string sort, string order)
        {
            Problems clsProblems = new Problems();
            Category clsCategory = new Category();
            Pager pager = new Pager();
            int cateid = cid ?? 0;
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = 10;
            if (cateid > 0)
                pager = clsProblems.GetUserReplyPaging(pager, clsCategory.GetCategoryIds(cateid), sort, string.IsNullOrWhiteSpace(order) ? "desc" : order, User.Identity.Name);
            else
                pager = clsProblems.GetUserReplyPaging(pager, cateid, sort, string.IsNullOrWhiteSpace(order) ? "desc" : order, User.Identity.Name);
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
            ViewBag.UserNewQuestion = clsProblems.UserNewQuestion(User.Identity.Name);
            ViewBag.UserSolveQuestion = clsProblems.UserSolveQuestion(User.Identity.Name);
            ViewData["CateId"] = new SelectList(catelist, "CateId", "CateName", cateid);
            return View(pager.Entity);
        }

        //
        // GET: /UserProblems/Details/5

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
            ViewBag.UserNewQuestion = clsProblems.UserNewQuestion(User.Identity.Name);
            ViewBag.UserSolveQuestion = clsProblems.UserSolveQuestion(User.Identity.Name);
            return View(clsPts_Problems);
        }

        //
        // GET: /UserProblems/Create
        public ActionResult Create()
        {
            string tid = "1";
            string cid = "1";
            UserProfileModel userprofile = GetMyProfile();
            ViewBag.showAssignTo = userprofile.Department;
            Department clsDepartment = new Department();
            Category clsCategory = new Category();
            ViewData["CategoryID"] = new SelectList(clsCategory.getFCategoryList(tid, "", " -- "), "CateId", "CateName", cid);
            Problems clsProblems = new Problems();
            ViewBag.UserNewQuestion = clsProblems.UserNewQuestion(User.Identity.Name);
            ViewBag.UserSolveQuestion = clsProblems.UserSolveQuestion(User.Identity.Name);
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
                //   CategoryModel category = myService.GetCategoryByID(model.CateId);    
                obj.CreateTime = DateTime.Now;
                obj.CategoryID = model.CategoryID;
                obj.Title = Utils.FileterStr(model.Title);
                obj.Description = Utils.NoHTML(string.IsNullOrWhiteSpace(model.Content) ? "" : model.Content);
                obj.Content = Server.UrlDecode(Utils.DownloadImages(model.Content, "/Content/Upload/", configinfo.Weburl));
                //obj.ip = Utils.GetIP();               
                obj.CreateUser = User.Identity.Name;
                obj.StartTime = model.StartTime;
                obj.CloseTime = model.CloseTime;
                obj.AssignedTo = "0";
                obj.AssignedToUser = "0";
                obj.IsStart = true;
                obj.IsClosed = false;
                obj.CreatUserName = userprofile.NickName;
                //回复表中添加记录               
                modelPts_Records.RecordID = Guid.NewGuid();
                modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                modelPts_Records.AssignTo = "0";
                modelPts_Records.AssignToObjectID = "0";
                modelPts_Records.AssignType = 0;
                modelPts_Records.Content = "新提交问题";
                modelPts_Records.Describe = "新提交问题";
                //modelPts_Records.Title = pts_Records.Title;
                modelPts_Records.CreateUser = User.Identity.Name;
                modelPts_Records.CreateTime = DateTime.Now;
                modelPts_Records.ProblemID = obj.ProblemID;
                modelPts_Records.SrcStateID = new Guid("00000000-0000-0000-0000-000000000000");
                modelPts_Records.SrcUserID = User.Identity.Name;
                modelPts_Records.Course = userprofile.NickName + "(客户)-->";
                modelPts_Records.ProblemStateID = new Guid("9d536318-d184-40e0-b896-fcea3512286e");//待回复
                modelPts_Records.ListOrder = 1;
                db.Pts_Problems.Add(obj);
                db.Pts_Records.Add(modelPts_Records);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //
        // GET: /UserProblems/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /UserProblems/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /UserProblems/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /UserProblems/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
