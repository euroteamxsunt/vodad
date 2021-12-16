using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using VodadModel.Utilities;

namespace VodadModel.Model
{
    public class QueryableRepository : IQueryableRepository
    {
        private ObjectContext context = new VodadEntities();
        private object locker = new object();

        public QueryableRepository()
        {
            context.CommandTimeout = 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
        {
            return context.CreateQuery<TEntity>(GetEntityName<TEntity>());
        }

        public IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return GetQuery<TEntity>().ToList();
        }

        public IList<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetQuery<TEntity>().Where(predicate).ToList();
        }

        public TEntity Single<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetQuery<TEntity>().Single(predicate);
        }

        public TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetQuery<TEntity>().Single(predicate);
        }

        public TEntity First<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var res = Find(predicate);
            return res.Count == 0 ? null : res.First();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            context.AddObject(GetEntityName<TEntity>(), entity);
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetQuery<TEntity>().Any(predicate);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            context.DeleteObject(entity);
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : class
        {
            context.Detach(entity);
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            context.AttachTo(GetEntityName<TEntity>(), entity);
        }

        public void ApplyCurrentValues<TEntity>(TEntity entity) where TEntity : class
        {
            context.ApplyCurrentValues(GetEntityName<TEntity>(), entity);
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public void SaveChanges()
        {
            try
            {
                lock (locker)
                    context.SaveChanges();
            }
            catch (Exception)
            {
            }

        }

        public void SaveChanges(SaveOptions options)
        {
            lock (locker)
                context.SaveChanges(options);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }

        private string GetEntityName<TEntity>() where TEntity : class
        {
            return string.Format("VodadEntities.{0}", context.GetTableName<TEntity>());
        }
    }
}
