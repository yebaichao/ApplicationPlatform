using ApplicationPlatform.IBLL;
using ApplicationPlatform.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ApplicationPlatform.BLL
{
    public class BaseServiceRepository<TEntity> : IBaseServiceRepository<TEntity> where TEntity : class,new()
    {
        public IBaseRepository<TEntity> currentRepository { set; get; }
        public BaseServiceRepository(IBaseRepository<TEntity> currentRepository)
        {
            this.currentRepository = currentRepository;
        }
        public TEntity Add(TEntity entity)
        {
            return currentRepository.Add(entity);
        }

        public int Count(Expression<Func<TEntity, bool>> where)
        {
            return currentRepository.Count(where);
        }

        public bool Delete(TEntity entity)
        {
            return currentRepository.Delete(entity);
        }

        public bool Exist(Expression<Func<TEntity, bool>> where)
        {
            return currentRepository.Exist(where);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> where)
        {
            return currentRepository.Find(where);
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> where)
        {
            return currentRepository.FindAll(where);
        }

        public IQueryable<TEntity> FindAll<SEntity>(Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, SEntity>> orderlanbda)
        {
            return currentRepository.FindAll<SEntity>(where, isAsc, orderlanbda);
        }

        public IQueryable<TEntity> FindPaged<SEntity>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, SEntity>> orderLambda)
        {
            return currentRepository.FindPaged<SEntity>(pageSize, ref pageNum, out totalRecord,out pageCount, where, isAsc, orderLambda);
        }
        public IQueryable<TEntity> FindPaged<SEntity>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, Expression<Func<TEntity, bool>> where, bool isAsc, Expression<Func<TEntity, SEntity>> orderLambda,string[] includes)
        {
            return currentRepository.FindPaged<SEntity>(pageSize, ref pageNum, out totalRecord, out pageCount, where, isAsc, orderLambda, includes);
        }
        public IQueryable<TEntity> FindPaged<S>(int pageSize, ref int pageNum, out int totalRecord, out int pageCount, IQueryable<TEntity> TEntities, bool isAsc, Expression<Func<TEntity, S>> orderLambda)
        { 
            return currentRepository.FindPaged<S>(pageSize, ref pageNum, out totalRecord, out pageCount,  TEntities,isAsc, orderLambda);
        }

        public int SaveChanges()
        {
            return currentRepository.SaveChanges();
        }

        public TEntity Update(TEntity entity)
        {
            return currentRepository.Update(entity);
        }
    }
}