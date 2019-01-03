using ApplicationPlatform.IDAL;
using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace ApplicationPlatform.DAL
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected AppContext appContext = ContextFactory.GetDbContext() as AppContext;
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Add(TEntity entity)
        {

            appContext.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Added;
            return entity;
        }
        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Count(Expression<Func<TEntity, bool>> where)
        {
            return appContext.Set<TEntity>().Count(where);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(TEntity entity)
        {
            appContext.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Deleted;
            return this.SaveChanges() > 0;
        }

        public void Dispose()
        {
            if (appContext != null)
            {
                appContext.Dispose();
                GC.SuppressFinalize(appContext);
            }

        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Exist(Expression<Func<TEntity, bool>> where)
        {
            return appContext.Set<TEntity>().Any(where);
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public TEntity Find(Expression<Func<TEntity, bool>> where)
        {
            return appContext.Set<TEntity>().FirstOrDefault(where);
        }
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> where)
        {
            return appContext.Set<TEntity>().Where(where);
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="SEntity"></typeparam>
        /// <param name="where"></param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="orderlanbda">排序表达式</param>
        /// <returns></returns>
        public IQueryable<TEntity> FindAll<SEntity>(Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, SEntity>> orderlanbda)
        {
            var lst = appContext.Set<TEntity>().Where<TEntity>(where);
            if (!isAsc)
            {
                lst = lst.OrderByDescending<TEntity, SEntity>(orderlanbda);
            }
            return lst;
        }

        /// <summary>
        /// 分页查询
        //// </summary>
        /// <typeparam name="SEntity"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalRecord"></param>
        /// <param name="pageCount"></param>
        /// <param name="where"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderLambda"></param>
        /// <returns></returns>
        public IQueryable<TEntity> FindPaged<S>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, S>> orderLambda)
        {

            var _list = appContext.Set<TEntity>().Where<TEntity>(where);
            totalRecord = 0;
            pageCount = 0;
            if (_list != null)
            {

                totalRecord = _list.Count<TEntity>();
                if (totalRecord==0)
                {   return _list;}
                if (totalRecord % pageSize == 0)
                {
                    pageCount = totalRecord / pageSize;
                }
                else
                {
                    pageCount = totalRecord / pageSize + 1;
                }

                if (pageNum <= 1)
                {
                    pageNum = 1;
                }
                if (pageNum >= pageCount)
                {
                    pageNum = pageCount;
                }

                if (isAsc)
                {
                    _list = _list.OrderBy<TEntity, S>(orderLambda).Skip<TEntity>((pageNum - 1) * pageSize).Take<TEntity>(pageSize);
                }
                else
                {
                    _list = _list.OrderByDescending<TEntity, S>(orderLambda).Skip<TEntity>((pageNum - 1) * pageSize).Take<TEntity>(pageSize);
                }
            }
            return _list;
        }
        /// <summary>
        /// 包含实体的分页查询
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalRecord"></param>
        /// <param name="pageCount"></param>
        /// <param name="where"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderLambda"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public IQueryable<TEntity> FindPaged<S>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, S>> orderLambda, string[] includes)
        {

            var _list = appContext.Set<TEntity>().Where<TEntity>(where);
            if (includes.Count() > 0)
            {
                foreach (string item in includes)
                {
                    _list = appContext.Set<TEntity>().Include(item).Where<TEntity>(where);
                }
            }
            totalRecord = 0;
            pageCount = 0;
            if (_list != null)
            {

                totalRecord = _list.Count<TEntity>();
                if (totalRecord == 0)
                { return _list; }
                if (totalRecord % pageSize == 0)
                {
                    pageCount = totalRecord / pageSize;
                }
                else
                {
                    pageCount = totalRecord / pageSize + 1;
                }

                if (pageNum <= 1)
                {
                    pageNum = 1;
                }
                if (pageNum >= pageCount)
                {
                    pageNum = pageCount;
                }

                if (isAsc)
                {
                    _list = _list.OrderBy<TEntity, S>(orderLambda).Skip<TEntity>((pageNum - 1) * pageSize).Take<TEntity>(pageSize);
                }
                else
                {
                    _list = _list.OrderByDescending<TEntity, S>(orderLambda).Skip<TEntity>((pageNum - 1) * pageSize).Take<TEntity>(pageSize);
                }
            }
            return _list;
        }
        public IQueryable<TEntity> FindPaged<S>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, IQueryable<TEntity> TEntities, bool isAsc, Expression<Func<TEntity, S>> orderLambda)
        {

            var _list = TEntities;
            totalRecord = 0;
            pageCount = 0;
            if (_list != null)
            {

                totalRecord = _list.Count<TEntity>();
                if (totalRecord == 0)
                { return _list; }
                if (totalRecord % pageSize == 0)
                {
                    pageCount = totalRecord / pageSize;
                }
                else
                {
                    pageCount = totalRecord / pageSize + 1;
                }

                if (pageNum <= 1)
                {
                    pageNum = 1;
                }
                if (pageNum >= pageCount)
                {
                    pageNum = pageCount;
                }

                if (isAsc)
                {
                    _list = _list.OrderBy<TEntity, S>(orderLambda).Skip<TEntity>((pageNum - 1) * pageSize).Take<TEntity>(pageSize);
                }
                else
                {
                    _list = _list.OrderByDescending<TEntity, S>(orderLambda).Skip<TEntity>((pageNum - 1) * pageSize).Take<TEntity>(pageSize);
                }
            }
            return _list;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return appContext.SaveChanges();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Update(TEntity entity)
        {
            TEntity tentity = appContext.Set<TEntity>().Attach(entity);
            appContext.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Modified;
            return tentity;
        }
    }
}