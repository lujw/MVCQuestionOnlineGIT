using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Mvc3QA.Models;
using Common;
using System.Web.Profile;
using Mvc3QA.General;
using Mvc3QA.Models.Services;


namespace Mvc3QA.Controllers
{
    public class AccountController : Controller
    {
        //[Inject]
        //public IServices myService { get; set; }
        GeneralConfigInfo configinfo = Mvc3QA.General.WebUtils.configinfo;
        public ActionResult Manage()
        {
            return Redirect("/QAAdmin/Admin/Index/");
        }
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {                     
                      
                        if (Roles.Provider.IsUserInRole(model.UserName, "admin"))
                        {
                            return RedirectToAction("Index", "QAAdmin/Admin");

                        }
                        if (Roles.Provider.IsUserInRole(model.UserName, "BusinessUsers"))
                        {
                            return RedirectToAction("Index", "Problems");

                        }
                        if (Roles.Provider.IsUserInRole(model.UserName, "RegisteredUsers"))
                        {
                            return RedirectToAction("Index", "UserProblems");
                        }


                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "提供的用户名或密码不正确。");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // 在某些出错情况下，ChangePassword 将引发异常，
                // 而不是返回 false。
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "当前密码不正确或新密码无效。");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        /// <summary>
        ///  用户信息
        /// </summary>
        public ActionResult UView(string user)
        {
            string Localhost;
            string uid;

            int port = Request.Url.Port;
            string ApplicationPath = Request.ApplicationPath != "/" ? Request.ApplicationPath : string.Empty;
            uid = user;
            Localhost = string.Format("{0}://{1}{2}{3}",
                                 Request.Url.Scheme,
                                 Request.Url.Host,
                                 (port == 80 || port == 0) ? "" : ":" + port,
                                 ApplicationPath);
            ViewBag.Localhost = Localhost;
            ViewBag.uid = uid;
            UserProfileModel userprofile = GetUserProfile(uid);
            // ViewBag.GenderInfo = WebUtils.GetGenderList().Find(delegate(SelectItem item) { return item.Value == userprofile.Gender; }).Key;
            return View(userprofile);
        }

        /// <summary>
        ///  用户中心
        /// </summary>
        [Authorize]
        public ActionResult UCenter()
        {
            Department clsDepartment = new Department();
            string avatarFlashParam;
            string EncodeLocalhost;
            string Localhost;
            string uid;

            int port = Request.Url.Port;
            string ApplicationPath = Request.ApplicationPath != "/" ? Request.ApplicationPath : string.Empty;
            uid = User.Identity.Name;
            Localhost = string.Format("{0}://{1}{2}{3}",
                                 Request.Url.Scheme,
                                 Request.Url.Host,
                                 (port == 80 || port == 0) ? "" : ":" + port,
                                 ApplicationPath);
            EncodeLocalhost = HttpUtility.UrlEncode(Localhost);
            avatarFlashParam = string.Format("{0}/Content/Avatar/common/camera.swf?nt=1&inajax=1&appid=1&input={1}&ucapi={2}/AjaxAvatar.ashx", Localhost, uid, EncodeLocalhost);

            ViewBag.avatarFlashParam = avatarFlashParam;
            ViewBag.Localhost = Localhost;
            ViewBag.uid = uid;
            UserProfileModel userprofile = GetMyProfile();
            ViewBag.GenderInfo = WebUtils.GetGenderList().Find(delegate(SelectItem item) { return item.Value == userprofile.Gender; }).Key;
            ViewBag.Department = clsDepartment.GetDepartmentName(userprofile.Department);
            return View(userprofile);
        }


        /// <summary>
        /// 用户Profile
        /// </summary>
        [Authorize]
        public ActionResult MyProfile()
        {
            Department clsDepartment = new Department();
            ViewBag.CI = configinfo;
            ViewData["Gender"] = new SelectList(WebUtils.GetGenderList(), "Value", "Key", GetProfileItem("gender", "1"));
            UserProfileModel userprofile = GetMyProfile();
            userprofile.Intro = Utils.RemoveHtml(WebUtils.HtmlToUBB(userprofile.Intro));
            userprofile.Signature = Utils.RemoveHtml(WebUtils.HtmlToUBB(userprofile.Signature));
            ViewBag.Department = new SelectList(clsDepartment.GetDepartmentList(), "DeptID", "DeptName");
            return PartialView("_UserProfile", userprofile);
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
        /// 获取用户Profile
        /// </summary>
        private UserProfileModel GetUserProfile(string user)
        {
            UserProfileModel userprofile = new UserProfileModel();
            try
            {
                ProfileBase objProfile = System.Web.Profile.ProfileBase.Create(user);

                userprofile.NickName = GetUserProfileItem(objProfile, "nickname");
                userprofile.Signature = GetUserProfileItem(objProfile, "signature");
                userprofile.Intro = GetUserProfileItem(objProfile, "intro");
                userprofile.Gender = GetUserProfileItem(objProfile, "gender", "1");
                userprofile.Department = GetUserProfileItem(objProfile, "Department", "00000000-0000-0000-0000-000000000000");
                userprofile.Birth = GetUserProfileItem(objProfile, "birth");
                userprofile.Location = GetUserProfileItem(objProfile, "location");
                userprofile.Website = GetUserProfileItem(objProfile, "website");
                userprofile.QQ = GetUserProfileItem(objProfile, "qq");
                userprofile.Sina = GetUserProfileItem(objProfile, "sina");
                userprofile.Facebook = GetUserProfileItem(objProfile, "facebook");
                userprofile.Twitter = GetUserProfileItem(objProfile, "twitter");
                userprofile.Medals = GetUserProfileItem(objProfile, "medals");
                userprofile.Phone = GetUserProfileItem(objProfile, "phone");

                string email = GetUserProfileItem(objProfile, "email");
                if (string.IsNullOrEmpty(email))
                {
                    MembershipUser currentUser = Membership.GetUser(user.Trim(), false /* userIsOnline */);
                    userprofile.Email = currentUser.Email;
                }
                else
                {
                    userprofile.Email = email;
                }
                userprofile.IsSendEmail = GetUserProfileItem(objProfile, "isSendEmail", "1");
            }
            catch
            { }
            return userprofile;
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
        /// 获取当前用户ProfileItem
        /// </summary>
        private string GetProfileItem(string key, string defaultvalue = "")
        {
            string value = (String)HttpContext.Profile.GetPropertyValue(key);
            return string.IsNullOrEmpty(value) ? defaultvalue : value;
        }

        /// <summary>
        /// 当前用户Profile更新
        /// </summary>
        [HttpPost]
        [Authorize]
        public ActionResult AjaxUserProfile(UserProfileModel model)
        {
            try
            {
                HttpContext.Profile["nickname"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.NickName) ? "" : model.NickName);
                HttpContext.Profile["signature"] = WebUtils.ubb2html(string.IsNullOrWhiteSpace(model.Signature) ? "" : model.Signature);
                HttpContext.Profile["intro"] = WebUtils.ubb2html(string.IsNullOrWhiteSpace(model.Intro) ? "" : model.Intro);
                HttpContext.Profile["gender"] = model.Gender;
                HttpContext.Profile["Department"] = model.Department.ToString();
                HttpContext.Profile["birth"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Birth) ? "" : model.Birth);
                HttpContext.Profile["location"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Location) ? "" : model.Location);
                HttpContext.Profile["website"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Website) ? "" : model.Website);
                HttpContext.Profile["qq"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.QQ) ? "" : model.QQ);
                HttpContext.Profile["sina"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Sina) ? "" : model.Sina);
                HttpContext.Profile["facebook"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Facebook) ? "" : model.Facebook);
                HttpContext.Profile["twitter"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Twitter) ? "" : model.Twitter);
                HttpContext.Profile["medals"] = string.IsNullOrWhiteSpace(model.Medals) ? "" : model.Medals;
                HttpContext.Profile["phone"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Phone) ? "" : model.Phone);
                HttpContext.Profile["email"] = Utils.RemoveHtml(string.IsNullOrWhiteSpace(model.Email) ? "" : model.Email);
                HttpContext.Profile["isSendEmail"] = model.IsSendEmail;
                return Content("0", "text/html;charset=UTF-8");
            }
            catch
            {
                return Content("1", "text/html;charset=UTF-8");
            }
        }

        /// <summary>
        ///  用户评论列表(异步)
        /// </summary>
        [Authorize]
        public ActionResult UserCommentList(int pageNo)
        {
            Pager pager = new Pager();
            pager.PageNo = pageNo;
            pager.PageSize = configinfo.CommentPagerCount;
            //  pager = myService.GetReplyPaging(pager, 1, 0, user: User.Identity.Name);
            ViewBag.PageNo = pageNo;
            ViewBag.PageCount = pager.PageCount;
            AjaxPager ajaxpager = new AjaxPager(3, pageNo, pager.PageCount);
            ViewBag.AjaxPager = ajaxpager.getPageInfoHtml();
            return PartialView("_UserComment", pager.Entity);
        }


        /// <summary>
        ///  用户留言列表(异步)
        /// </summary>
        [Authorize]
        public ActionResult UserNoteList(int pageNo)
        {
            Pager pager = new Pager();
            pager.PageNo = pageNo;
            pager.PageSize = configinfo.CommentPagerCount;
            // pager = myService.GetReplyPaging(pager, 6, 0, 0, user: User.Identity.Name);
            ViewBag.PageNo = pageNo;
            ViewBag.PageCount = pager.PageCount;
            AjaxPager ajaxpager = new AjaxPager(3, pageNo, pager.PageCount);
            ViewBag.AjaxPager = ajaxpager.getPageInfoHtml();
            return PartialView("_UserNote", pager.Entity);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请输入不同的用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "该电子邮件地址的用户名已存在。请输入不同的电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
        #endregion
    }
}
