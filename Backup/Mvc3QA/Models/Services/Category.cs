using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using System.Configuration;
using Mvc3QA.General;

namespace Mvc3QA.Models
{
    /// <summary>
    /// 分类信息相关数据操作实现
    /// </summary>
    public partial class Category 
    {
        GeneralConfigInfo configinfo = Mvc3QA.General.WebUtils.configinfo;
        /// <summary>
        /// 获取指定Id分类信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public CategoryModel GetCategoryByID(int cid)
        {
            CategoryModel category = new CategoryModel();
            try
            {
                List<CategoryModel> lst = GetCategoryList();
                category = lst.Find((CategoryModel p) => { return p.CateId == cid.ToString(); });
                return category;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取最大分类id
        /// </summary>
        public int GetMaxCategoryID()
        {
            int re = 0;
            try
            {
                List<CategoryModel> lst = GetCategoryList();
                re = lst.Select(p => Utils.StrToInt(p.CateId)).Max();
                return re;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取指定Rename分类信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public CategoryModel GetCategoryByReName(string rename)
        {
            CategoryModel category = new CategoryModel();
            try
            {
                List<CategoryModel> lst = GetCategoryList();
                category = lst.Find((CategoryModel p) => { return p.ReName == rename; });
                return category;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获得格式化后的所有分类列表
        /// </summary>
        /// <returns></returns>
        public List<CategoryModel> getFCategoryList(string space = "")
        {
            return getFCategoryList("", "", space);
        }

        /// <summary>
        /// 获得格式化后的分类列表
        /// </summary>
        /// <returns></returns>
        public List<CategoryModel> getFCategoryList(string tids, string cids, string space = "")
        {
            string[] arrcid = cids.Split(',');
            string[] arrtid = tids.Split(',');
            var list = GetCategories();
            if (cids.Trim()!="")
                list = list.Where(m => arrcid.Contains(m.CateId)); 
            else if (tids.Trim() != "")
                list = list.Where(m => arrtid.Contains(m.Type));
            list = list.OrderBy(m => Utils.StrToInt(m.OrderId));
            List<CategoryModel> newlst = new List<CategoryModel>();
            CategoryModel category = new CategoryModel();
            foreach (CategoryModel c in list)
            {
                category = c;
                category.CateName = (space == "" ? "" : Utils.GetSpace(c.Path.Split(',').Count(), space)) + c.CateName;
                newlst.Add(c);
            }
            return newlst;
        }

        /// <summary>
        /// 获取分类路径url
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public string GetCategoryPathUrl(string path)
        {
            string str = "";
            CategoryModel category = new CategoryModel();
            try
            {
                List<CategoryModel> lst = GetCategoryList();
                foreach (string s in path.Split(','))
                {
                    category = lst.Find((CategoryModel p) => { return p.CateId == s; });
                    str += "\\ <a href=\"" + WebUtils.GetCateUrl(category) + "\">" + category.CateName + "</a> ";
                }

                return str.Trim('\\').Trim();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取分类路径url2（不显示根目录，1级目录显示文字，往后显示为链接）
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public string GetCategoryPathUrl2(string path)
        {
            CategoryModel category = new CategoryModel();
            string str = "";
            string[] arrpath = path.Split(',');
            if (arrpath.Count() < 2)
                return "";
            List<CategoryModel> lst = GetCategoryList();
            if (arrpath.Count() == 2)
            {
                category = lst.Find((CategoryModel p) => { return p.CateId == arrpath[1]; });
                str=category.CateName;
            }
            else
            {
                string newPath = "";
                for (int i = 1; i < arrpath.Count(); i++)
                { newPath += arrpath[i] + ','; }
                str = GetCategoryPathUrl(newPath.Trim(','));
            }
            return str;
        }

        /// <summary>
        /// 返回所有数据
        /// 从iqueryable想list转换用iqueryable.ToList()
        /// 反向转换使用list.asQueryable()
        /// </summary>
        /// <returns></returns>
        public IQueryable<CategoryModel> GetCategories()
        {
            return GetCategoryList().AsQueryable<CategoryModel>();
        }

        /// <summary>
        /// 返回需要显示在首页的分类信息
        /// </summary>
        /// <returns></returns>
        public List<CategoryModel> GetIndexCategoryList()
        {
            List<CategoryModel> lst = GetCategoryList();
            try
            {
                List<CategoryModel> newlst = new List<CategoryModel>();
                foreach (CategoryModel category in lst)
                {
                    if (category.IsIndex=="1")
                        newlst.Add(category);
                }
                return newlst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定Id连同其所有子分类信息的id字符串集合
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public string GetCategoryIds(int cid)
        {
            try
            {
                return String.Concat(GetCategoryList().Where(c =>
                                      ("," + c.Path + ",").Contains("," + cid.ToString() + ",")).Select(c => c.CateId.ToString()+",")).Trim(',');
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取指定Id所有子分类信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public IQueryable<CategoryModel> GetSubCategoryList(int cid)
        {
            try
            {
                return GetCategories().Where(c =>
                                      ("," + c.Path + ",").Contains("," + cid.ToString() + ",")).Where(c => c.CateId != cid.ToString()).OrderBy(c => Utils.StrToInt(c.OrderId));
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 获取指定Id下一级子分类信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public IQueryable<CategoryModel> GetSonCategoryList(int cid)
        {
            try
            {
                return GetCategories().Where(c => c.ParentId == cid.ToString()).OrderBy(c => Utils.StrToInt(c.OrderId));
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 读取json,反序列化为分类列表
        /// </summary>
        /// <returns></returns>
        public List<CategoryModel> GetCategoryList()
        {
            List<CategoryModel> lst = new List<CategoryModel>();
            List<CategoryLangModel> langlst = new List<CategoryLangModel>();
            try
            {
                string jsonnav = GetCategoryStr().Replace("\n", "");
                if (jsonnav != "")
                {
                    lst = Utils.ParseFromJson<List<CategoryModel>>(jsonnav);
                }

                if (Resource.Models.Web.Web.Lang != "")
                {
                    string jsonnavlang = GetCategoryLangStr().Replace("\n", "");
                    if (jsonnavlang != "")
                    {
                        langlst = Utils.ParseFromJson<List<CategoryLangModel>>(jsonnavlang);
                    }

                    lst = (from a in lst
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
            }
            catch (Exception) { }
            return lst;
        }

        /// <summary>
        /// 读取json文件,返回字符串
        /// </summary>
        /// <returns></returns>
        public string GetCategoryStr()
        {
            //string cacheKey = "Json-Category";
            //object cache = DataCache.GetCache(cacheKey);
            //if (cache == null)
            //{
            //    try
            //    {
            //        cache = Utils.GetFileSource("/Content/js/Category.js").Replace("var category =", "").Trim();
            //        if (cache != null)
            //        {
            //            DataCache.SetCache(cacheKey, cache, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
            //        }
            //    }
            //    catch (Exception) { }
            //}
            //return cache.ToString();
            string re = "";
            try
            {
                re = Utils.GetFileSource("/Content/js/Category.js").Replace("var category =", "").Trim();
            }
            catch (Exception) { re = "";}
            return re;
        }

        /// <summary>
        /// 读取json lang文件,返回字符串
        /// </summary>
        /// <returns></returns>
        public string GetCategoryLangStr()
        {
            string re = "";
            try
            {
                string weblang = Resource.Models.Web.Web.Lang != "" ? Resource.Models.Web.Web.Lang + "." : "";
                re = Utils.GetFileSource("/Content/js/Category.Lang." + weblang + "js").Replace("var categorylang =", "").Trim();
            }
            catch (Exception) { re = ""; }
            return re;
        }
    }
}