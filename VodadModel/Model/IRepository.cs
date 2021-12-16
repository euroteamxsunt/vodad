using System;
using System.Linq;

namespace VodadModel.Repository
{
    interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        void Delete(TEntity entity);
        void Add(TEntity entity);
    }
}
