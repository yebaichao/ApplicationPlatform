using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ApplicationPlatform.IBLL
{
    public interface IBaseServiceRepository<TEntity>
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Add(TEntity entity);
        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> where);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete(TEntity entity);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Exist(Expression<Func<TEntity, bool>> where);
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        TEntity Find(Expression<Func<TEntity, bool>> where);
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> where);
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="SEntity"></typeparam>
        /// <param name="where"></param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="orderlanbda">排序表达式</param>
        /// <returns></returns>
        IQueryable<TEntity> FindAll<SEntity>(Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, SEntity>> orderlanbda);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="SEntity"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecord"></param>
        /// <param name="pageCount"></param>
        /// <param name="where"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderLambda"></param>
        /// <returns></returns>
        IQueryable<TEntity> FindPaged<SEntity>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, SEntity>> orderLambda);
        /// <summary>
        /// 包含实体的分页查询
        /// </summary>
        /// <typeparam name="SEntity"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecord"></param>
        /// <param name="pageCount"></param>
        /// <param name="where"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderLambda"></param>
        /// <returns></returns>
        IQueryable<TEntity> FindPaged<SEntity>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, SEntity>> orderLambda, string[] includes);
        /// <summary>
        /// 将实体列表进行分页
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalRecord"></param>
        /// <param name="pageCount"></param>
        /// <param name="TEntities"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderLambda"></param>
        /// <returns></returns>
        IQueryable<TEntity> FindPaged<S>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, IQueryable<TEntity> TEntities, bool isAsc, Expression<Func<TEntity, S>> orderLambda);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}