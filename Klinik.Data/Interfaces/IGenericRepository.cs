using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Klinik.Data
{
    /// <summary>
    /// Interface of generic repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGenericRepository<TEntity>
    {
        List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

        TEntity GetById(object id);

        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes);

        TEntity FindBy(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(object id);
    }
}
