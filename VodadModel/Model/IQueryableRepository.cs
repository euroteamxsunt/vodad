using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace VodadModel.Model
{
    public interface IQueryableRepository : IDisposable
    {
        IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class;
        IList<TEntity> GetAll<TEntity>() where TEntity : class;
        IList<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        TEntity Single<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        TEntity First<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        void Add<TEntity>(TEntity entity) where TEntity : class;
        bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        void Detach<TEntity>(TEntity entity) where TEntity : class;
        void Attach<TEntity>(TEntity entity) where TEntity : class;
        void ApplyCurrentValues<TEntity>(TEntity entity) where TEntity : class;
        int Save();
        void SaveChanges();
        void SaveChanges(SaveOptions options);
    }
}
