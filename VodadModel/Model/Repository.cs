using System;
using System.Linq;
using System.Data.Objects;
using System.Linq.Expressions;
using VodadModel.Helpers;

namespace VodadModel.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected ObjectContext context = null;

        public Repository(ObjectContext context)
        {
            this.context = context;
            this.context.CommandTimeout = 0; // Бесконечное время ожидания ответа базы
        }

        protected ObjectSet<TEntity> ObjectSet
        {
            get
            {
                return context.CreateObjectSet<TEntity>();
            }
        }

        public void Add(TEntity entity)
        {
            ObjectSet.AddObject(entity);
        }

        public void Delete(TEntity entity)
        {
            ObjectSet.DeleteObject(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return ObjectSet.AsQueryable();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> whereCondition)
        {
            return ObjectSet.Where(whereCondition).AsQueryable();
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> whereCondition)
        {
            return ObjectSet.Where(whereCondition).FirstOrDefault<TEntity>();
        }

        public void Attach(TEntity entity)
        {
            ObjectSet.Attach(entity);
        }

        public int Count(TEntity entity)
        {
            return ObjectSet.Count<TEntity>();
        }

        public long Count(Expression<Func<TEntity, bool>> whereCondition)
        {
            return ObjectSet.Where(whereCondition).LongCount<TEntity>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        public int Save()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (Exception e)
            {
                NLogWrapper.Logger.ErrorException("Database save exception: ", e);

                // Отправить письмо об удалении админ
                string messageBody = "Database save exception: " + e;
                const string messageSubject = "Database save excerption";
                UserHelper.SendMessage(Utilities.Constants.AdminConstants.Email, "Kostya", messageBody, messageSubject);

                return 0;
            }
        }

        public bool Update()
        {
            return true;
        }
    }
}