using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using System.Data;
using System.Data.Entity.Validation;

namespace Mvc3QA.Models.Services
{
    public class Problems
    {
        private questiononlineContext db = new questiononlineContext();
        /// <summary>
        /// 获取问题记录
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="tid"></param>
        /// <param name="cids"></param>
        /// <param name="layer"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Pager GetReplyPaging(Pager pager, string cids, string sort = "", string order = "desc", string user = "")
        {
           
            IQueryable<Pts_Problems> query = db.Pts_Problems;        
            if (cids != "")
            {
                List<int> listids = new List<string>(cids.Split(',')).ConvertAll(i => int.Parse(i));
                query = query.Where(m => listids.Contains(m.CategoryID));
            }
            switch (sort)
            {
                case "NewQuestion":
                    query = query.Where(m => m.IsStart == true);
                    break;
                case "MyReplay":
                    query = query.Where(m => m.HandlingUser == user);
                    break;
                case "SolvedQuestion":
                    query = query.Where(m => m.IsClosed == true);
                    break;
                case "MySolvedQuestion":
                    query = query.Where(m => m.SolveUser == user);
                    break;
                default:
                    break;
            }
            pager.Amount = query.Count();

            if (order == "desc")
                query = query.OrderByDescending(m => m.ProblemID);
            else
                query = query.OrderBy(m => m.ProblemID);

            query = query.Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }
        public Pager GetReplyPaging(Pager pager, int cid, string sort = "", string order = "desc", string user = "")
        {
            IQueryable<Pts_Problems> query = db.Pts_Problems ;
            if (cid > 0)
            {
                query = query.Where(m => m.CategoryID == cid);
            }  
            switch (sort)
            {                
                case "NewQuestion":
                    query = query.Where(m => m.IsStart==true);
                    break;
                case "MyReplay":
                    query = query.Where(m => m.HandlingUser == user);
                    break;
                case "SolvedQuestion":
                    query = query.Where(m => m.IsClosed == true);
                    break;
                case "MySolvedQuestion":
                    query = query.Where(m => m.SolveUser == user);
                    break;               
                default :
                    break;
            }
            pager.Amount = query.Count();

            if (order == "desc")
                query = query.OrderByDescending(m => m.CreateTime);
            else
                query = query.OrderBy(m => m.CreateTime);

            query = query.Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }
        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="cids"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Pager GetUserReplyPaging(Pager pager, string cids, string sort = "", string order = "desc", string user = "")
        {

            IQueryable<Pts_Problems> query = db.Pts_Problems;
            query = query.Where(m => m.CreateUser == user);
            if (cids != "")
            {
                List<int> listids = new List<string>(cids.Split(',')).ConvertAll(i => int.Parse(i));
                query = query.Where(m => listids.Contains(m.CategoryID));
            }
            switch (sort)
            {
                case "UserNewQuestion":
                    query = query.Where(q => q.CreateUser == user);
                    break;
                case "UserSolveQuestion":
                    query = query.Where(q => q.CreateUser == user && q.IsClosed == true);
                    break;
                default:
                    break;
            }     
            pager.Amount = query.Count();

            if (order == "desc")
                query = query.OrderByDescending(m => m.ProblemID);
            else
                query = query.OrderBy(m => m.ProblemID);

            query = query.Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }
        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="cid"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Pager GetUserReplyPaging(Pager pager, int cid, string sort = "", string order = "desc", string user = "")
        {
            IQueryable<Pts_Problems> query = db.Pts_Problems;
            if (cid>0)
            {
                query = query.Where(m => m.CategoryID == cid);
            }          
            query = query.Where(m => m.CreateUser == user);
            switch (sort)
            {
                case "UserNewQuestion":
                    query = query.Where(q => q.CreateUser == user);
                    break;
                case "UserSolveQuestion":
                    query = query.Where(q => q.CreateUser == user && q.IsClosed == true);
                    break;                
                default:
                    break;
            }          
            pager.Amount = query.Count();

            if (order == "desc")
                query = query.OrderByDescending(m => m.CreateTime);
            else
                query = query.OrderBy(m => m.CreateTime);           
            query = query.Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 录入问题
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal int AddProblems(Pts_Problems obj)
        {
            int id = 0;
            try
            {
                //   EntityState statebefore = db.Entry(obj).State;
                db.Pts_Problems.Add(obj);
                id = db.SaveChanges();
                //  EntityState stateafter = db.Entry(obj).State;
            }
            catch (Exception dbEx)
            {

                throw;
            }

            return id;
        }
        /// <summary>
        /// 返回新提交问题总数
        /// </summary>
        /// <returns></returns>
        public int NewQuestionsCount()
        {
            return db.Pts_Problems.Where(q => q.IsStart == true).Count();

        }
        /// <summary>
        /// 当前用户的待处理问题数
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int MyReplayQuestionCount(string UserID)
        {
            Guid stateID = new Guid("aa36df43-a68b-45e2-8230-21b2f6f5fce2");
            var query = from q in db.Pts_Records
                        where q.ProblemStateID != stateID && q.AssignToObjectID == UserID
                        orderby q.CreateTime descending
                        select q;
            return query.Count();
        }
        /// <summary>
        /// 问题总数
        /// </summary>
        /// <returns></returns>
        public int QuestionCount()
        {
            return db.Pts_Problems.Count();
        }
        /// <summary>
        /// 已解决问题数
        /// </summary>
        /// <returns></returns>
        public int SolvedQuestionCount()
        {
            return db.Pts_Problems.Where(q => q.IsClosed).Count();
        }
        /// <summary>
        /// 当前用户解决问题数
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public int MYSolvedQuestionCount(string currentUser)
        {
            return db.Pts_Problems.Where(q => q.SolveUser==currentUser).Count();
        }

        internal int UserNewQuestion(string userID)
        {
            return db.Pts_Problems.Where(q => q.CreateUser == userID).Count();
        }

        internal int UserSolveQuestion(string userID)
        {
            return db.Pts_Problems.Where(q => q.CreateUser == userID && q.IsClosed==true).Count();
        }
    }
}