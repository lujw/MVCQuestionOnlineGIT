using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using res = Resource.Models.Web.Web;
using System.Text.RegularExpressions;
using System.IO;
using Common;
using System.Web.Helpers;
using System.Web.Security;
using System.Net.Mail;
using System.Text;
using System.Net;
using Mvc3QA.Models;


namespace Mvc3QA.General
{
    public class WebUtils
    {
        /// <summary>
        /// 网站配置信息
        /// </summary>
        public static GeneralConfigInfo configinfo = GeneralConfigs.GetConfig();

        /// <summary>
        /// 分类链接url
        /// </summary>
        public static string GetCateUrl(CategoryModel category)
        {
            return GetCurrentLangPath() + (!string.IsNullOrWhiteSpace(category.ReName) ? "/" + category.ReName + "/" : "/cate/" + category.CateId);
        }

        /// <summary>
        /// 文章链接url
        /// </summary>
        public static string GetArticleUrl(int id, string reName)
        {
            return !string.IsNullOrWhiteSpace(reName) ? "/article/" + reName : "/archive/" + id.ToString();
        }

        /// <summary>
        /// 文章链接url
        /// </summary>
        //public static string GetArticleUrl(blog_varticle varticle)
        //{
        //    return !string.IsNullOrWhiteSpace(varticle.rename) ? "/article/" + varticle.rename : "/archive/" + varticle.id.ToString();
        //}

        /// <summary>
        /// 获得对应视图名称
        /// </summary>
        /// <param name="customView"></param>
        /// <param name="defaultView"></param>
        /// <returns></returns>
        public static string GetViewName(string customView, string defaultView)
        {
            return string.IsNullOrWhiteSpace(customView) ? defaultView : customView;
        }

        /// <summary>
        /// 获得配置文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetWebConfigPath()
        {
            return ConfigurationManager.AppSettings["QAConfig"].ToString();
        }

        /// <summary>
        /// 获得当前语言下分类语言文件名
        /// </summary>
        /// <returns></returns>
        public static string GetCategoryLangName()
        {
            string cateName = "Category.Lang.js";
            string weblang = Resource.Models.Web.Web.Lang != "" ? "." + Resource.Models.Web.Web.Lang : "";
            if (weblang != "")
                cateName = cateName.Replace(".js", weblang + ".js");
            return cateName;
        }

        /// <summary>
        /// 视图切换，记录session,cookie(用于缓存定制)以及更新视图引擎路径
        /// </summary>
        /// <param name="path"></param>
        public static void ChangeTheme(string key)
        {
            string theme = string.IsNullOrEmpty(key) ? "" : key;

            HttpCookie themeCookie = new HttpCookie("web");
            themeCookie["theme"] = theme;
            themeCookie.Expires = DateTime.Now.AddDays(7d);
            HttpContext.Current.Response.Cookies.Add(themeCookie);

            DataCache.SetCache("DecorateCount", CurrentDecorateCount(theme), DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new BlogViewEngine(theme));
        }

        /// <summary>
        /// 当前语言路径
        /// </summary>
        public static string GetCurrentLangPath()
        {
            string currentlang = string.IsNullOrWhiteSpace(res.Lang) ? "zh-cn" : res.Lang;
            return currentlang != configinfo.DefaultLang ? "/" + currentlang : "";
        }

        /// <summary>
        /// 当前风格
        /// </summary>
        public static string GetCurrentTheme()
        {
            return HttpContext.Current.Request.Cookies["web"]["theme"] != null ? HttpContext.Current.Request.Cookies["web"]["theme"].ToString() : Mvc3QA.General.WebUtils.configinfo.Theme;
        }

        /// <summary>
        /// 获得页面类型下拉列表
        /// </summary>
        public static List<PageType> GetTypeList()
        {
            List<PageType> items = new List<PageType>();
            items.Add(new PageType
            {
                TypeName = Resource.Views.Admin.Admin.Article,
                TypeId = 1
            });
            items.Add(new PageType
            {
                TypeName = Resource.Views.Admin.Admin.SinglePage,
                TypeId = 2
            });
            items.Add(new PageType
            {
                TypeName = Resource.Views.Admin.Admin.Contributors,
                TypeId = 3
            });
            items.Add(new PageType
            {
                TypeName = Resource.Views.Admin.Admin.Album,
                TypeId = 4
            });
            //items.Add(new PageType
            //{
            //    TypeName = "论坛",
            //    TypeId = 5
            //});
            items.Add(new PageType
            {
                TypeName = Resource.Views.Admin.Admin.Message,
                TypeId = 6
            });
            items.Add(new PageType
            {
                TypeName = Resource.Views.Admin.Admin.CustomArea,
                TypeId = 7
            });
            items.Add(new PageType
            {
                TypeName = Resource.Views.Admin.Admin.CustomGlobalArea,
                TypeId = 8
            });
            return items;
        }

        /// <summary>
        /// 获得站点语言下拉列表
        /// </summary>
        public static List<PageLang> GetLangList()
        {
            List<PageLang> items = new List<PageLang>();
            items.Add(new PageLang
            {
                Key = "中(简体)",
                Value = "zh-cn"
            });
            items.Add(new PageLang
            {
                Key = "中(繁体)",
                Value = "zh-tw"
            });
            items.Add(new PageLang
            {
                Key = "English",
                Value = "en-us"
            });

            return items;


        }

        public static readonly string[] Langs = new[] { "/en-us", "/zh-tw", "/zh-cn" };

        /// <summary>
        /// 获得性别下拉列表
        /// </summary>
        public static List<SelectItem> GetGenderList()
        {
            List<SelectItem> items = new List<SelectItem>();
            items.Add(new SelectItem
            {
                Key = res.Secrecy,
                Value = "0"
            });
            items.Add(new SelectItem
            {
                Key = res.Male,
                Value = "1"
            });
            items.Add(new SelectItem
            {
                Key = res.Female,
                Value = "2"
            });
            return items;
        }



        /// <summary>
        /// ubb转html
        /// </summary>
        /// <param name="argString"></param>
        /// <returns></returns>
        public static string ubb2html(string argString)
        {
            string tString = argString;
            if (tString != "")
            {
                Regex tRegex;
                bool tState = true;
                tString = tString.Replace("&", "&amp;");
                tString = tString.Replace(">", "&gt;");
                tString = tString.Replace("<", "&lt;");
                tString = tString.Replace("\"", "&quot;");
                tString = tString.Replace("&amp;#91;", "&#91;");
                tString = tString.Replace("&amp;#93;", "&#93;");
                tString = tString.Replace("\r\n", "<br/>");
                tString = Regex.Replace(tString, @"\[br\]", "<br/>", RegexOptions.IgnoreCase);
                string[,] tRegexAry = {
                  {@"\[p\]([^\[]*?)\[\/p\]", "$1<br/>"},
                  {@"\[b\]([^\[]*?)\[\/b\]", "<b>$1</b>"},
                  {@"\[i\]([^\[]*?)\[\/i\]", "<i>$1</i>"},
                  {@"\[u\]([^\[]*?)\[\/u\]", "<u>$1</u>"},
                  {@"\[ol\]([^\[]*?)\[\/ol\]", "<ol>$1</ol>"},
                  {@"\[ul\]([^\[]*?)\[\/ul\]", "<ul>$1</ul>"},
                  {@"\[li\]([^\[]*?)\[\/li\]", "<li>$1</li>"},
                  {@"\[code\]([^\[]*?)\[\/code\]", "<div class=\"ubb_code\">$1</div>"},
                  {@"\[quote\]([^\[]*?)\[\/quote\]", "<fieldset class=\"comment_quote\"><legend> "+Resource.Views.Home.Home.Quote+" </legend>$1</fieldset>"},
                  //{@"\[quote\]([^\[]*?)\[\/quote\]", "<div class=\"ubb_quote\">$1</div>"},
                  {@"\[color=([^\]]*)\]([^\[]*?)\[\/color\]", "<font style=\"color: $1\">$2</font>"},
                  {@"\[hilitecolor=([^\]]*)\]([^\[]*?)\[\/hilitecolor\]", "<font style=\"background-color: $1\">$2</font>"},
                  {@"\[align=([^\]]*)\]([^\[]*?)\[\/align\]", "<div style=\"text-align: $1\">$2</div>"},
                  {@"\[url=([^\]]*)\]([^\[]*?)\[\/url\]", "<a href=\"$1\">$2</a>"},
                  {@"\[img\]([^\[]*?)\[\/img\]", "<img src=\"$1\" />"}
                };
                while (tState)
                {
                    tState = false;
                    for (int ti = 0; ti < tRegexAry.GetLength(0); ti++)
                    {
                        tRegex = new Regex(tRegexAry[ti, 0], RegexOptions.IgnoreCase);
                        if (tRegex.Match(tString).Success)
                        {
                            tState = true;
                            tString = Regex.Replace(tString, tRegexAry[ti, 0], tRegexAry[ti, 1], RegexOptions.IgnoreCase);
                        }
                    }
                }
            }
            return tString;
        }


        /// <summary>
        /// html转ubb
        /// </summary>
        public static string HtmlToUBB(string _Html)
        {
            _Html = Regex.Replace(_Html, "<br[^>]*>", "\n");
            _Html = Regex.Replace(_Html, @"<p[^>\/]*\/>", "\n");
            _Html = Regex.Replace(_Html, "<hr[^>]*>", "[hr]");
            _Html = Regex.Replace(_Html, "<(\\/)?blockquote([^>]*)>", "[$1blockquote]");
            _Html = Regex.Replace(_Html, "<img[^>]*smile=\"(\\d+)\"[^>]*>", "‘[s:$1]");
            _Html = Regex.Replace(_Html, "<img[^>]*src=[\'\"\\s]*([^\\s\'\"]+)[^>]*>", "[img]$1[/img]");
            _Html = Regex.Replace(_Html, "<a[^>]*href=[\'\"\\s]*([^\\s\'\"]*)[^>]*>(.+?)<\\/a>", "[url=$1]$2[/url]");
            _Html = Regex.Replace(_Html, "<b>(.+?)</b>", @"\[b\]$1\[/b\]");
            _Html = Regex.Replace(_Html, "<[^>]*?>", "");
            _Html = Regex.Replace(_Html, "&amp;", "&");
            _Html = Regex.Replace(_Html, "&nbsp;", " ");
            _Html = Regex.Replace(_Html, "&lt;", "<");
            _Html = Regex.Replace(_Html, "&gt;", ">");
            return _Html;
        }

        /// <summary>
        /// 获得插图总数
        /// </summary>
        /// <returns></returns>
        public static int GetDecorateCount(string theme)
        {

            string cacheKey = "DecorateCount";
            object cache = DataCache.GetCache(cacheKey);
            if (cache == null)
            {
                try
                {
                    cache = CurrentDecorateCount(theme);
                    if (cache != null)
                    {
                        DataCache.SetCache(cacheKey, cache, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
                    }
                }
                catch (Exception) { }
            }
            return Utils.ObjectToInt(cache);
        }

        /// <summary>
        /// 当前风格下插图总数
        /// </summary>
        /// <returns></returns>
        private static int CurrentDecorateCount(string theme)
        {
            try
            {
                string decoratepath = theme == "" ? "/Content/image/decorate" : "/Themes/" + theme + "/Content/image/decorate";
                return Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath(decoratepath), "*.jpg").Length - 1;
            }
            catch
            {
                return 0;
            }
        }


        /// <summary>
        /// 从文本中提取用户集合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> GetUsersFromTxt(string str)
        {
            List<string> lst = new List<string>();
            Regex regstr = new Regex(@"@(.*?)\s", RegexOptions.IgnoreCase);
            MatchCollection mc = regstr.Matches(str);
            foreach (Match match in mc)
            {
                lst.Add(match.Groups[1].Value);
            }
            lst = lst.GroupBy(x => x).Select(x => x.Key).ToList();

            return lst;
        }

        /// <summary>
        /// WebMail邮件发送
        /// </summary>
        public static void SendWebMail(string emailTo, string emailTitle, string emailContent, string[] filePath = null, string[] additionalHeaders = null)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            //WebMail.SmtpServer = ConfigurationManager.AppSettings["smtpServer"];//获取或设置要用于发送电子邮件的 SMTP 中继邮件服务器的名称。            
            //WebMail.SmtpPort = Utils.StrToInt(ConfigurationManager.AppSettings["smtpPort"]);//发送端口                    
            //WebMail.UserName = ConfigurationManager.AppSettings["smtpUser"];//账号名            
            //WebMail.From = ConfigurationManager.AppSettings["adminEmail"];//邮箱名            
            //WebMail.Password = ConfigurationManager.AppSettings["smtpPass"];//密码   
            WebMail.SmtpServer = configinfo.SmtpServer;//获取或设置要用于发送电子邮件的 SMTP 中继邮件服务器的名称。            
            WebMail.SmtpPort = configinfo.SmtpPort;//发送端口                    
            WebMail.UserName = configinfo.SmtpUser;//账号名            
            WebMail.From = configinfo.AdminEmail;//邮箱名            
            WebMail.Password = configinfo.SmtpPass;//密码   
            WebMail.EnableSsl = true;//是否启用 SSL GMAIL 需要 而其他都不需要 具体看你在邮箱中的配置    
            WebMail.SmtpUseDefaultCredentials = true;//是否使用默认配置                    
            try
            {
                if (reg.IsMatch(emailTo))
                {
                    WebMail.Send(to: emailTo, subject: emailTitle, body: emailContent, isBodyHtml: true, filesToAttach: filePath, additionalHeaders: additionalHeaders);
                }
            }
            catch { }
        }

        /// <summary>
        /// WebMail邮件发送
        /// </summary>
        public static void SendWebMail(string emailTo, string emailTitle, string emailContent)
        {
            SendWebMail(emailTo, emailTitle, emailContent);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        public static void SendSysMail(string to, string subject, string body)
        {
            //string from = ConfigurationManager.AppSettings["adminEmail"];
            //string host=ConfigurationManager.AppSettings["smtpServer"];
            //string userName=ConfigurationManager.AppSettings["smtpUser"];
            //string password=ConfigurationManager.AppSettings["smtpPass"];
            //int port = Utils.StrToInt(ConfigurationManager.AppSettings["smtpPort"]);

            string from = configinfo.AdminEmail;
            string host = configinfo.SmtpServer;
            string userName = configinfo.SmtpUser;
            string password = configinfo.SmtpPass;
            int port = configinfo.SmtpPort;
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                SendMail(from, to, subject, body, host, port, userName, password);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="from">发件人</param>
        /// <param name="to">收件人</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="host">发送服务地址(smtp.gmail.com)</param>
        /// <param name="port">发送邮件服务器端口</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public static void SendMail(string from, string to, string subject, string body, string host, int port, string userName, string password)
        {
            MailMessage message = new MailMessage(from, to, subject, body);
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            SmtpClient client = new SmtpClient(host);
            client.Credentials = new NetworkCredential(userName, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Port = port;
            client.Send(message);
        }
    }
}