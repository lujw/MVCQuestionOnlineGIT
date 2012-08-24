using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc3QA.Models;
using System.Web.Routing;
using Common;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using Mvc3QA.General;
using res = Resource.Controllers.Admin.Admin;
using System.Web.Security;
using Mvc3QA.Models.Services;
using System.Web.Profile;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;

namespace Mvc3QA.Areas.QAAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private questiononlineContext db = new questiononlineContext();
        //[Inject]
        //public IServices myService { get; set; }

        GeneralConfigInfo configinfo = Mvc3QA.General.WebUtils.configinfo;

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 问题列表
        /// </summary>
        public ActionResult AdminProblems(int? pageNo, int? tid, int? layer, int? cid, string sort, string order)
        {
            Problems clsProblems = new Problems();
            Category clsCategory = new Category();
            Pager pager = new Pager();
            int typeid = tid ?? 0;
            int layerid = layer ?? 0;
            int cateid = cid ?? 0;
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = 10;
            if (cateid > 0)
                pager = clsProblems.GetReplyPaging(pager,  clsCategory.GetCategoryIds(cateid),  sort, string.IsNullOrWhiteSpace(order) ? "desc" : order);
            else
                pager = clsProblems.GetReplyPaging(pager,  cateid, sort, string.IsNullOrWhiteSpace(order) ? "desc" : order);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            ViewBag.TypeId = typeid;
            ViewBag.LayerId = layerid;
            ViewBag.Cid = cateid;
            ViewBag.Sort = sort;
            ViewBag.Order = order;
            List<CategoryModel> catelist = clsCategory.getFCategoryList(typeid.ToString(), "", " -- ");
            catelist.Insert(0, new CategoryModel
          {
              CateId = "0",
              CateName = res.SelectSubcategories
          });
            ViewBag.CurrentItem = layerid > 0 ? "l" + layerid.ToString() : "t" + typeid.ToString();
            ViewData["CateId"] = new SelectList(catelist, "CateId", "CateName", cateid);
            return View(pager.Entity);
        }
        public ViewResult Details(Guid id)
        {
            Category clsCategory = new Category();
            List<CategoryModel> catelist = clsCategory.getFCategoryList("1", "", " -- ");
            Pts_Problems clsPts_Problems = db.Pts_Problems.Find(id);
            ViewBag.CategoryID = catelist.Find(c => c.CateId.ToString() == clsPts_Problems.CategoryID.ToString()).CateName.ToString();
            Department clsDepartment = new Department();
            ViewBag.AssignTo = new SelectList(clsDepartment.GetDepartmentList(), "DeptID", "DeptName");

            return View(clsPts_Problems);
        }
        /// <summary>
        /// 保存回复信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Details(FormCollection pts_Records)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //SqlParameter[] parms = new SqlParameter[1];
                    //parms[0] = new SqlParameter("@ProblemID", pts_Records["ProblemID"]);
                    //int results = db.Database.ExecuteSqlCommand("exec SP_Problems_IsStart_Update @ProblemID", parms);

                    Pts_Records modelPts_Records = new Pts_Records();
                    //  EntityState statebefore = db.Entry(modelPts_Recordsmodel).State;
                    modelPts_Records.RecordID = Guid.NewGuid();
                    modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                    modelPts_Records.AssignTo = pts_Records["AssignTo"];
                    modelPts_Records.AssignToObjectID = "bbbbbbbb";
                    modelPts_Records.AssignType = Convert.ToInt32(pts_Records["AssignType"]);
                    modelPts_Records.Content = pts_Records["Content"];
                    //modelPts_Records.Title = pts_Records.Title;
                    modelPts_Records.CreateUser = User.Identity.Name;
                    modelPts_Records.CreateTime = DateTime.Now;
                    modelPts_Records.ProblemID = new Guid(pts_Records["ProblemID"]);
                    modelPts_Records.SrcStateID = new Guid("00000000-0000-0000-0000-000000000000");
                    modelPts_Records.SrcUserID = pts_Records["LastUser"] == null ? "000" : pts_Records["LastUser"]; ;
                    modelPts_Records.ProblemStateID = new Guid("9d536318-d184-40e0-b896-fcea3512286e");
                    db.Pts_Records.Add(modelPts_Records);
                    var upDateProblems = (from d in db.Pts_Problems
                                          where d.ProblemID == modelPts_Records.ProblemID
                                          select d).Single();
                    upDateProblems.IsStart = false;
                    int result = db.SaveChanges();
                    //  EntityState stateafter = db.Entry(modelPts_Recordsmodel).State;
                    return RedirectToAction("Details", "Admin", new { id = modelPts_Records.ProblemID });

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
        /// <summary>
        /// 回复信息
        /// </summary>
        /// <param name="problems"></param>
        /// <returns></returns>
        public ActionResult AdminRecordsAdd(Guid id)
        {
            Pts_Records modelRecords = new Pts_Records();
            modelRecords.ProblemID = id;
            Department clsDepartment = new Department();
            UserProfileModel userprofile = GetMyProfile();
            ViewBag.AssignTo = new SelectList(clsDepartment.GetDepartmentList(userprofile.Department), "DeptID", "DeptName");
            return View(modelRecords);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminRecordsAdd(Pts_Records pts_Records)
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
                    Department clsDepartment = new Department();
                    UserProfileModel userprofile = GetMyProfile();
                    if ((pts_Records.AssignType == 1) || (pts_Records.AssignType == 3))//直接解决或者不不解决转交上一个人
                    {
                        modelPts_Records.AssignTo = "1";
                        modelPts_Records.AssignToObjectID = NewRecords.CreateUser;
                        modelPts_Records.AssignType = 0;
                        modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                        if (LastRecords.CreateUser == User.Identity.Name)//第一条记录创建人和当前用户，判断是否是问题提交人
                        {
                            modelPts_Records.ProblemStateID = new Guid("aa36df43-a68b-45e2-8230-21b2f6f5fce2");//问题已解决
                        }
                        else
                        {
                            //上一个部门
                         Guid preDPID=clsDepartment.GetPreDeparmentID(userprofile.Department);
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
                    }
                    modelPts_Records.RecordID = Guid.NewGuid();
                    modelPts_Records.Content = pts_Records.Content;
                    modelPts_Records.CreateUser = User.Identity.Name;
                    modelPts_Records.CreateTime = DateTime.Now;
                    modelPts_Records.ProblemID = pts_Records.ProblemID;
                    db.Pts_Records.Add(modelPts_Records);
                    var upDateProblems = (from d in db.Pts_Problems
                                          where d.ProblemID == modelPts_Records.ProblemID
                                          select d).Single();
                    upDateProblems.IsStart = false;
                    int result = db.SaveChanges();
                    return RedirectToAction("Details", "Admin", new { id = modelPts_Records.ProblemID });
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
        /// <summary>
        /// 获取相关的回复信息
        /// </summary>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public Pts_Records geRecordsProcessing(Guid problemId)
        {
            Pts_Records modelRecords = new Pts_Records();
            var problems = (from d in db.Pts_Problems
                            where d.ProblemID == problemId
                            select d).Single();
            if (problems.IsStart == true)
            {
                modelRecords.SrcUserID = problems.CreateUser;
            }
            else
            {
                var records = (from d in db.Pts_Records
                               where d.ProblemID == problemId
                               orderby d.CreateTime descending
                               select d).First();
                modelRecords.SrcUserID = records.SrcUserID;
            }
            return modelRecords;
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
        /// 新增文章
        /// </summary>
        public ActionResult AdminAddProblems()
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

        //<summary>
        //新增文章（提交）
        //</summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminAddProblems(Pts_Problems model)
        {
            //添加[ValidateInput(false)]特性，否则提交内容有html代码会报错
            if (ModelState.IsValid)
            {
                UserProfileModel userprofile = GetMyProfile();
                Pts_Problems obj = new Pts_Problems();
                Pts_Records modelPts_Records = new Pts_Records();
                Department clsDepartment = new Department();
                obj.ProblemID = Guid.NewGuid();
                if (userprofile.Department == "6b0500be-2b97-4dc3-8e22-7d34fa9d0acf")//如果是客户提交问题
                {
                    //   CategoryModel category = myService.GetCategoryByID(model.CateId);    
                    obj.CreateTime = DateTime.Now;
                    obj.CategoryID = model.CategoryID;
                    obj.Title = Utils.FileterStr(model.Title);
                    obj.Description = Utils.NoHTML(string.IsNullOrWhiteSpace(model.Description) ? "" : model.Description);
                    obj.Content = Server.UrlDecode(Utils.DownloadImages(model.Content, "/Content/Upload/", configinfo.Weburl));
                    //obj.ip = Utils.GetIP();               
                    obj.CreateUser = User.Identity.Name;
                    obj.StartTime = model.StartTime;
                    obj.CloseTime = model.CloseTime;
                    obj.AssignedTo = "0";
                    obj.AssignedToUser = "0";
                    obj.IsStart = true;
                    obj.IsClosed = false;
                    //回复表中添加记录               
                    modelPts_Records.RecordID = Guid.NewGuid();
                    modelPts_Records.AssignStateID = new Guid("00000000-0000-0000-0000-000000000000");
                    modelPts_Records.AssignTo = "0";
                    modelPts_Records.AssignToObjectID = "0";
                    modelPts_Records.AssignType = 0;
                    modelPts_Records.Content = "新提交问题";
                    //modelPts_Records.Title = pts_Records.Title;
                    modelPts_Records.CreateUser = User.Identity.Name;
                    modelPts_Records.CreateTime = DateTime.Now;
                    modelPts_Records.ProblemID = obj.ProblemID;
                    modelPts_Records.SrcStateID = new Guid("00000000-0000-0000-0000-000000000000");
                    modelPts_Records.SrcUserID = User.Identity.Name;
                    modelPts_Records.Course = userprofile.NickName + "-->";
                    modelPts_Records.ProblemStateID = new Guid("9d536318-d184-40e0-b896-fcea3512286e");//待回复

                }
                else//其他用户提交问题
                {

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
                    modelPts_Records.Course = userprofile.NickName+"-->";
                    //提交问题后状态
                    modelPts_Records.ProblemStateID = clsDepartment.GetProblemState(model.AssignedTo);
                }
                db.Pts_Problems.Add(obj);
                db.Pts_Records.Add(modelPts_Records);
                db.SaveChanges();


                return RedirectToAction("AdminProblems", new { tid = 1, cid = obj.CategoryID });
            }
            return View(model);
        }

        /// <summary>
        /// 问题修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdminEdit(Guid id)
        {
            Category clsCategory = new Category();
            Department clsDepartment = new Department();
            Pts_Problems clsPts_Problems = db.Pts_Problems.Find(id);
            ViewBag.AssignedTo = new SelectList(clsDepartment.GetDepartmentList(), "DeptID", "DeptName", clsPts_Problems.AssignedTo);
            ViewData["CategoryID"] = new SelectList(clsCategory.getFCategoryList("1", "", " -- "), "CateId", "CateName", clsPts_Problems.CategoryID);
            return View(clsPts_Problems);
        }

        //<summary>
        //提交修改
        //</summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminEdit(Pts_Problems model)
        {
            if (ModelState.IsValid)
            {
                Category clsCategory = new Category();
                CategoryModel category = clsCategory.GetCategoryByID(model.CategoryID);
                Pts_Problems obj = new Pts_Problems();
                obj.ProblemID = model.ProblemID;
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
                obj.IsStart = model.IsStart;
                obj.IsClosed = model.IsClosed;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("AdminProblems", new { tid = 1, cid = obj.CategoryID });
            }
            return View(model);
        }

        ///// <summary>
        ///// 文章删除
        ///// </summary>
        [HttpPost]
        public ActionResult AdminDel(Guid id)
        {
            try
            {
                if (Request.IsAjaxRequest())
                {
                    Pts_Problems ModelPts_Problems = db.Pts_Problems.Find(id);
                    db.Pts_Problems.Remove(ModelPts_Problems);
                    EntityState stateafter = db.Entry(ModelPts_Problems).State;//Deleted状态
                    int result = db.SaveChanges();
                    return Content(result.ToString());
                }
                else
                {
                    return Content("-1");
                }
            }
            catch (Exception)
            {
                return Content("-1");
            }

        }
        /// <summary>
        /// 根据问题ID获取处理过程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult AdminRecords(Guid id)
        {
            var pts_recores = from r in db.Pts_Records.Include("Pts_ProblemState")
                              where r.ProblemID == id
                              select r;

            return PartialView("AdminRecords", pts_recores.ToList());
        }
        /// <summary>
        /// 回复问题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult AdminRecordsCreate(Guid id)
        {
            Pts_Records modelPts_Records = new Pts_Records();
            modelPts_Records.ProblemID = id;
            return PartialView("AdminRecordsCreate", modelPts_Records);
        }


        /// <summary>
        /// 分类配置管理
        /// </summary>
        public ActionResult AdminCategory()
        {
            Category ca = new Category();
            ViewBag.Content = ca.GetCategoryStr();
            return View();
        }

        /// <summary>
        /// 提交分类配置（手动修改）
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminCategory(string content)
        {
            try
            {
                List<CategoryModel> lst = Utils.ParseFromJson<List<CategoryModel>>(content);
                List<CategoryModel> newlst = RefreshCateList(lst);
                SaveCateInfo(newlst);
            }
            catch (Exception)
            {
                return Content(res.ModificationFailed + "<a href=\"/Mvc3QA/admin/AdminCategory\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
            }
            return Content(res.ModifiedSuccessfully + " <a href=\"/Mvc3QA/admin/AdminCategory\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 刷新catelist，更新subcuont,path字段
        /// </summary>
        public List<CategoryModel> RefreshCateList(List<CategoryModel> lst)
        {
            List<CategoryModel> newlst = new List<CategoryModel>();
            foreach (CategoryModel c in lst)
            {
                CategoryModel category = new CategoryModel();
                category = c;
                category.Path = GetCategoryPath(lst, category);
                category.SubCount = GetSubCount(lst, category.CateId).ToString();
                newlst.Add(category);
            }
            return newlst;
        }

        /// <summary>
        /// 保存分类信息到json
        /// </summary>
        public void SaveCateInfo(List<CategoryModel> lst)
        {
            List<CategoryModel> newlst = new List<CategoryModel>();
            List<CategoryLangModel> langlst = new List<CategoryLangModel>();
            if (Resource.Models.Web.Web.Lang != "")
            {
                string jsonnavlang = Utils.GetFileSource("/Content/js/Category.Lang.js").Replace("var categorylang =", "").Trim().Replace("\n", "");
                if (jsonnavlang != "")
                {
                    langlst = Utils.ParseFromJson<List<CategoryLangModel>>(jsonnavlang);
                }

                newlst = (from a in lst
                          join b in langlst
                          on a.CateId equals b.CateId into temp
                          from t in temp.DefaultIfEmpty()
                          select new CategoryModel
                          {
                              CateId = a.CateId,
                              CateName = t == null ? a.CateName : t.CateName,
                              Type = a.Type,
                              ListNum = a.ListNum,
                              ReplyPermit = a.ReplyPermit,
                              ParentId = a.ParentId,
                              IsNav = a.IsNav,
                              IsIndex = a.IsIndex,
                              Status = a.Status,
                              ReName = a.ReName,
                              CustomView = t == null ? a.CustomView : t.CustomView,
                              SubCount = a.SubCount,
                              OrderId = a.OrderId,
                              Path = a.Path
                          }).ToList();
            }
            else
                newlst = lst;

            string jsonstr = Utils.GetJson<List<CategoryModel>>(newlst);
            string file = System.Web.HttpContext.Current.Server.MapPath("/Content/js/Category.js");

            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.Write("var category = " + jsonstr);
            }
            SaveCateLangInfo(lst);
            RefreshConfigVersionNo();
            //DataCache.SetCache("Json-Category", jsonstr, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
        }

        /// <summary>
        /// 保存分类语言信息到json
        /// </summary>
        public void SaveCateLangInfo(List<CategoryModel> lst)
        {
            List<CategoryLangModel> lstlang = (from p in lst select new CategoryLangModel { CateId = p.CateId, CateName = p.CateName, CustomView = p.CustomView }).ToList();
            string jsonstr = Utils.GetJson<List<CategoryLangModel>>(lstlang);
            string file = System.Web.HttpContext.Current.Server.MapPath("/Content/js/" + WebUtils.GetCategoryLangName());

            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.Write("var categorylang = " + jsonstr);
            }
        }

        /// <summary>
        /// 计算当前分类的子分类数
        /// </summary>
        private int GetSubCount(List<CategoryModel> lst, string parent)
        {
            int total = 0;
            foreach (CategoryModel c in lst)
            {
                if (c.ParentId == parent)
                    total++;
            }
            return total;
        }

        /// <summary>
        /// 计算当前分类的path
        /// </summary>
        private string GetCategoryPath(List<CategoryModel> lst, CategoryModel category)
        {
            CategoryModel c = category;
            string path = GetCurrentCategoryUrl(c);
            while (c.ParentId != "0")
            {
                foreach (CategoryModel cc in lst)
                {
                    if (cc.CateId == c.ParentId)
                    {
                        path = GetCurrentCategoryUrl(cc) + path;
                        c = cc;
                        continue;
                    }
                }
            }
            return path.Trim(',');
        }

        private string GetCurrentCategoryUrl(CategoryModel category)
        {
            return "," + category.CateId;
        }

        public ActionResult RestoreCategory()
        {
            try
            {
                string file = System.Web.HttpContext.Current.Server.MapPath("/Content/js/Category.js");
                System.IO.File.Copy(System.Web.HttpContext.Current.Server.MapPath("/Content/js/Categorybak.js"), file, true);
            }
            catch (Exception)
            { }
            return RedirectToAction("AdminCategory");
        }

        /// <summary>
        /// 论坛json配置修改
        /// </summary>
        //public ActionResult AdminBBSConfig()
        //{
        //    ViewBag.Content = myService.GetBBSExtendedStr();
        //    return View();
        //}

        /// <summary>
        /// 提交论坛json配置修改
        /// </summary>
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult AdminBBSConfig(string content)
        //{
        //    try
        //    {
        //        List<BbsExtendedModel> lst = Utils.ParseFromJson<List<BbsExtendedModel>>(content);
        //        string jsonstr = Utils.GetJson<List<BbsExtendedModel>>(lst);
        //        string file = System.Web.HttpContext.Current.Server.MapPath("/Content/BBS.js");
        //        using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
        //        {
        //            writer.Write(jsonstr);
        //        }
        //        DataCache.SetCache("Json-BBS", jsonstr, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
        //    }
        //    catch (Exception)
        //    {
        //        return Content(res.ModificationFailed + "<a href=\"/BlogAdmin/admin/AdminBBSConfig\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
        //    }
        //    return Content(res.ModifiedSuccessfully + " <a href=\"/BlogAdmin/admin/AdminBBSConfig\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
        //}

        /// <summary>
        /// 转载网站json配置修改
        /// </summary>
        //public ActionResult AdminPublishWebConfig()
        //{
        //    ViewBag.Content = myService.GetPublishWebStr();
        //    return View();
        //}

        /// <summary>
        /// 提交转载网站json配置修改
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminPublishWebConfig(string content)
        {
            try
            {
                List<PublishWebModel> lst = Utils.ParseFromJson<List<PublishWebModel>>(content);
                string jsonstr = Utils.GetJson<List<PublishWebModel>>(lst);
                string file = System.Web.HttpContext.Current.Server.MapPath("/Content/js/PublishWeb.js");
                using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    writer.Write(jsonstr);
                }
                DataCache.SetCache("Json-PublishWeb", jsonstr, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
                RefreshConfigVersionNo();
            }
            catch (Exception)
            {
                return Content(res.ModificationFailed + "<a href=\"/Mvc3QA/admin/AdminPublishWebConfig\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
            }
            return Content(res.ModifiedSuccessfully + " <a href=\"/Mvc3QA/admin/AdminPublishWebConfig\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 站点基础设置
        /// </summary>
        public ActionResult AdminBaseConfig()
        {
            List<SelectItem> themelst = new List<SelectItem>();
            themelst.Add(new SelectItem { Key = res.Default, Value = "" });
            DirectoryInfo di = new DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath("/Themes"));
            DirectoryInfo[] dirs = di.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                themelst.Add(new SelectItem { Key = dirs[i].Name, Value = dirs[i].Name });
            }
            ViewData["DefaultLang"] = new SelectList(WebUtils.GetLangList(), "Value", "Key", configinfo.DefaultLang);
            ViewData["Theme"] = new SelectList(themelst, "Value", "Key", configinfo.Theme);
            return View(configinfo);
        }

        /// <summary>
        /// 提交站点基础设置
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminBaseConfig(GeneralConfigInfo model)
        {
            try
            {
                GeneralConfigInfo config = configinfo;
                config.Weburl = model.Weburl;
                config.Webtitle = model.Webtitle;
                //config.WebPath = string.IsNullOrEmpty(model.WebPath) ? "/" : model.WebPath;
                config.Icp = model.Icp;
                config.IndexPagerCount = model.IndexPagerCount;
                config.CatePagerCount = model.CatePagerCount;
                config.CommentPagerCount = model.CommentPagerCount;
                config.NotePagerCount = model.NotePagerCount;
                config.WebDescription = model.WebDescription;
                config.ThumbnailInfo = model.ThumbnailInfo;
                config.Theme = model.Theme;
                config.DefaultLang = model.DefaultLang;
                config.ContributorCateIds = model.ContributorCateIds;
                config.MaxSummaryCharCount = model.MaxSummaryCharCount;
                config.AdminEmail = model.AdminEmail;
                config.SmtpServer = model.SmtpServer;
                config.SmtpUser = model.SmtpUser;
                config.SmtpPass = model.SmtpPass;
                config.SmtpPort = model.SmtpPort;
                config.IfSendReplyEmail = model.IfSendReplyEmail;

                WebUtils.ChangeTheme(model.Theme);
                CultureInfo cultureinfo = new CultureInfo(model.DefaultLang == "zh-cn" ? "" : configinfo.DefaultLang);
                System.Web.HttpContext.Current.Session["CurrentLanguage"] = cultureinfo;
                GeneralConfigs.Serialiaze(config, Server.MapPath(WebUtils.GetWebConfigPath()));
            }
            catch (Exception)
            {
                return Content(res.ModifyFailed + " <a href=\"/QAAdmin/admin/AdminBaseConfig\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
            }
            return Content(res.ModifiedSuccessfully + " <a href=\"/QAAdmin/admin/AdminBaseConfig\">" + res.ContinueModify + "</a>", "text/html;charset=UTF-8");
        }

        //更新js版本号
        [NonAction]
        public void RefreshConfigVersionNo()
        {
            GeneralConfigInfo config = configinfo;
            config.VersionNo = DateTime.Now.ToString("yyyyMMddhhmmss");
            GeneralConfigs.Serialiaze(config, Server.MapPath(WebUtils.GetWebConfigPath()));
        }


        //【上传图片】
        //KindEditor 返回JSON格式说明： 
        //格式：{"error":0,"message":".....","url":"/img/1111.gif"} 
        //error：0成功，1失败，成功需要指定url值为图片/文件保存后的URL地址，如果error值不为0，则设置message值为错误提示信息 
        /// <summary>
        /// KE文件上传
        /// </summary>
        [HttpPost]
        public ActionResult UploadFile(string dir)
        {
            //定义一个返回提示Hashtable对象
            Hashtable hash = new Hashtable();
            //文件保存路径
            string savePath = "/Content/Attached/";
            //文件Url
            string saveUrl = configinfo.Weburl + "/Content/Attached/";
            //文件大小限制
            int maxSize = 1000000;
            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

            //针对不同的上传文件类型做差异化设置
            switch (dir)
            {
                case "image":
                    savePath = "/Content/Upload/";
                    saveUrl = configinfo.Weburl + "/Content/Upload/";
                    break;
                case "flash":
                    break;
                case "media":
                    break;
                case "file":
                    break;
            }

            //获得上传文件
            HttpPostedFileBase file = Request.Files["imgFile"];
            //文件保存磁盘路径
            string dirPath = Server.MapPath(savePath);
            //文件名及后缀
            string fileName = file.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();

            //文件是否为空在前端也判断过一次
            if (file == null)
                return UploadJsonRe(1, res.UploadFile_Tip1, "");

            if (!Directory.Exists(dirPath))
                return UploadJsonRe(1, res.UploadFile_Tip2, "");

            if (file.InputStream == null || file.InputStream.Length > maxSize)
                return UploadJsonRe(1, res.UploadFile_Tip3, "");

            if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dir]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                return UploadJsonRe(1, res.UploadFile_Tip4, "");

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
            string filePath = dirPath + newFileName;
            file.SaveAs(filePath);
            string fileUrl = saveUrl + newFileName;

            return UploadJsonRe(0, "", fileUrl);
        }

        /// <summary>
        /// 上传返回提示
        /// </summary>
        private JsonResult UploadJsonRe(int error, string message, string url)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = error;
            hash["message"] = message;
            hash["url"] = url;
            return Json(hash, "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 分类排序
        /// </summary>
        public ActionResult AdminCategorySort()
        {
            Category clsCategory = new Category();
            var lst = clsCategory.getFCategoryList(" -- ");
            return View(lst);

        }

        //<summary>
        //保存分类排序
        //</summary>
        [HttpPost]
        public ActionResult AdminCategorySort(string ids)
        {
            Category clsCategory = new Category();
            List<CategoryModel> orderlst = new List<CategoryModel>();
            List<CategoryModel> newlst = new List<CategoryModel>();
            string[] arrId = ids.Trim(',').Split(',');
            for (int i = 0; i < arrId.Length; i++)
            {
                int orderid = i + 1;
                CategoryModel category = new CategoryModel();
                category = clsCategory.GetCategoryByID(Utils.StrToInt(arrId[i]));
                category.OrderId = orderid.ToString();
                orderlst.Add(category);
            }
            GetNewCategoryList(orderlst, ref newlst, "0");
            SaveCateInfo(newlst);

            string re = "";
            var lst = clsCategory.getFCategoryList(" -- ");
            foreach (CategoryModel c in lst)
            {
                string rootClass = c.ParentId == "0" ? " class=\"cl_root\"" : "";
                re += "<li id=\"" + c.CateId + "\"" + rootClass + ">" + c.CateName + " (" + c.CateId + ")</li>";
            }
            return Content(re, "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 重置分类OrderStr
        /// </summary>
        private void GetNewCategoryList(List<CategoryModel> orderlst, ref List<CategoryModel> newlst, string parentId)
        {
            List<CategoryModel> tmplist = new List<CategoryModel>();
            foreach (CategoryModel c in orderlst)
            {
                if (c.ParentId == parentId)
                {
                    CategoryModel category = c;
                    category.OrderId = (newlst.Count() + 1).ToString();
                    newlst.Add(category);

                    if (Utils.StrToInt(c.SubCount) > 0)
                    {
                        GetNewCategoryList(orderlst, ref newlst, c.CateId);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// 新增分类
        /// </summary>
        public ActionResult AdminCategoryAdd(int id)
        {
            Category clsCategory = new Category();
            ViewBag.CateId = id;
            ViewBag.CateName = id > 0 ? clsCategory.GetCategoryByID(id).CateName : "";
            ViewData["Type"] = new SelectList(WebUtils.GetTypeList(), "TypeId", "TypeName", 1);
            CategoryModel category = new CategoryModel();
            category.ParentId = id.ToString();
            if (id > 0)
                category.Type = clsCategory.GetCategoryByID(id).Type;
            return View(category);
        }

        /// <summary>
        /// 新增分类
        /// </summary>
        [HttpPost]
        public ActionResult AdminCategoryAdd(CategoryModel model)
        {
            Category clsCategory = new Category();
            List<CategoryModel> newlst = new List<CategoryModel>();
            List<CategoryModel> catelist = clsCategory.getFCategoryList();
            CategoryModel category = new CategoryModel();
            category.CateId = (clsCategory.GetMaxCategoryID() + 1).ToString();
            category.CateName = model.CateName;
            category.IsIndex = model.IsIndex;
            category.IsNav = model.IsNav;
            category.ListNum = string.IsNullOrWhiteSpace(model.ListNum) ? "0" : model.ListNum;
            category.ParentId = model.ParentId;
            category.ReName = string.IsNullOrWhiteSpace(model.ReName) ? "" : model.ReName;
            category.CustomView = string.IsNullOrWhiteSpace(model.CustomView) ? "" : model.CustomView;
            category.ReplyPermit = model.ReplyPermit;
            category.Status = model.Status;
            category.Type = model.Type;
            category.OrderId = (catelist.Count() + 1).ToString();
            category.SubCount = "0";
            category.Path = category.CateId;
            if (category.ParentId == "0")
            {
                catelist.Add(category);
                newlst = catelist;
            }
            else
            {
                catelist.Add(category);
                //刷新subcount,path值
                catelist = RefreshCateList(catelist);
                //重置顺序orderid
                GetNewCategoryList(catelist, ref newlst, "0");
            }
            //保存为json
            SaveCateInfo(newlst);
            return Content(res.AddedSuccessfully + " <a href=\"/QAAdmin/admin/AdminCategorySort\">" + res.View + "</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        public ActionResult AdminCategoryEdit(int id)
        {
            Category clsCategory = new Category();
            CategoryModel category = clsCategory.GetCategoryByID(id);
            ViewData["Type"] = new SelectList(WebUtils.GetTypeList(), "TypeId", "TypeName", category.Type);
            List<CategoryModel> list = clsCategory.getFCategoryList("", "", " -- ").Where(m => m.CateId != category.CateId).ToList();
            list.Add(new CategoryModel { CateId = "0", CateName = "Root" });
            ViewData["ParentId"] = new SelectList(list, "CateId", "CateName", category.ParentId);
            return View(category);
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        [HttpPost]
        public ActionResult AdminCategoryEdit(CategoryModel model)
        {
            Category clsCategory = new Category();
            List<CategoryModel> catelist = clsCategory.getFCategoryList();
            List<CategoryModel> newlst = new List<CategoryModel>();
            List<CategoryModel> newlst2 = new List<CategoryModel>();
            CategoryModel category = new CategoryModel();
            category.CateId = model.CateId;
            category.CateName = model.CateName;
            category.IsIndex = model.IsIndex;
            category.IsNav = model.IsNav;
            category.ListNum = string.IsNullOrWhiteSpace(model.ListNum) ? "0" : model.ListNum;
            category.ParentId = model.ParentId;
            category.ReName = string.IsNullOrWhiteSpace(model.ReName) ? "" : model.ReName;
            category.CustomView = string.IsNullOrWhiteSpace(model.CustomView) ? "" : model.CustomView;
            category.ReplyPermit = model.ReplyPermit;
            category.Status = model.Status;
            category.Type = model.Type;

            category.OrderId = (catelist.Count() + 1).ToString();
            category.SubCount = "0";
            category.Path = "0";
            bool isPathChange = false;

            foreach (CategoryModel c in catelist)
            {
                if (category.CateId == c.CateId)
                {
                    if (category.ParentId == c.ParentId)
                    {
                        category.OrderId = c.OrderId;
                        category.SubCount = c.SubCount;
                        category.Path = c.Path;
                    }
                    newlst.Add(category);
                }
                else
                    newlst.Add(c);
            }

            if (category.Path == "0")
            {
                isPathChange = true;
                //刷新subcount,path值
                newlst = RefreshCateList(newlst);
                //重置顺序orderid
                GetNewCategoryList(newlst, ref newlst2, "0");
            }
            else
            { newlst2 = newlst; }

            //保存为json
            SaveCateInfo(newlst2);

            //分类层级关系改变时更新文章表里path信息
            if (isPathChange)
            {

                CategoryModel newcategory = clsCategory.GetCategoryByID(Utils.StrToInt(model.CateId));
                //  clsCategory.BatchUpdateArticlePath(Utils.StrToInt(model.CateId), newcategory.Path);
            }
            return Content(res.AddedSuccessfully + " <a href=\"/QAAdmin/admin/AdminCategorySort\">" + res.View + "</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        [HttpPost]
        public ActionResult AdminCategoryDel(int id)
        {
            Category clsCategory = new Category();
            List<CategoryModel> catelist = clsCategory.getFCategoryList();
            List<CategoryModel> newlst = new List<CategoryModel>();
            List<CategoryModel> newlst2 = new List<CategoryModel>();
            foreach (CategoryModel c in catelist)
            {
                if (id.ToString() != c.CateId)
                { newlst.Add(c); }
            }
            GetNewCategoryList(newlst, ref newlst2, "0");
            newlst2 = RefreshCateList(newlst2);
            //保存为json
            SaveCateInfo(newlst2);
            return Content(res.DeletedSuccessfully + " <a href=\"/QAAdmin/admin/AdminCategorySort\">" + res.View + "</a>", "text/html;charset=UTF-8");
        }


        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminUserList(int? pageNo)
        {
            int pageSize = 20;
            int totalRecords = 0;
            int pageno = pageNo ?? 1;
            ViewBag.PageNo = pageno;
            var user = Membership.GetAllUsers(pageno - 1, pageSize, out totalRecords).Cast<MembershipUser>();
            ViewBag.PageCount = totalRecords / pageSize + (totalRecords % pageSize > 0 ? 1 : 0);
            MembershipUserCollection users = Membership.GetAllUsers();
            totalRecords = users.Count;
            ViewBag.PageCount = totalRecords / pageSize + (totalRecords % pageSize > 0 ? 1 : 0);
            var sortedList = users.Cast<MembershipUser>().ToList().OrderBy(a => System.Web.Profile.ProfileBase.Create(a.UserName).GetPropertyValue("FirstName"));
            var page = users.Cast<MembershipUser>().ToList().OrderByDescending(a => a.CreationDate).Skip(pageSize * (pageno - 1)).Take(pageSize);

            return View(page);
        }

        /// <summary>
        /// 管理列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminManagerList()
        {
            var user = Roles.GetUsersInRole("Admin");
            return View(user);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpPost]
        public ActionResult AdminDelUser(string user)
        {
            try
            {
                Membership.DeleteUser(user);
            }
            catch (Exception)
            {
                return Content(res.DeleteFailed, "text/html;charset=UTF-8");
            }
            return Content(res.DeletedSuccessfully, "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 用户从角色中删除
        /// </summary>
        [HttpPost]
        public ActionResult AdminRemoveUserFromRole(string user, string role = "Admin")
        {
            try
            {
                Roles.RemoveUserFromRole(user, role);
            }
            catch (Exception)
            {
                return Content(res.DeleteFailed, "text/html;charset=UTF-8");
            }
            return Content(res.DeletedSuccessfully, "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 用户加入到角色
        /// </summary>
        [HttpPost]
        public ActionResult AdminAddUserToRole(string user, string role = "Admin")
        {
            try
            {
                Roles.AddUserToRole(user, role);
            }
            catch (Exception)
            {
                return Content(res.OperationFailed, "text/html;charset=UTF-8");
            }
            return Content(res.OperationSuccessful, "text/html;charset=UTF-8");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
