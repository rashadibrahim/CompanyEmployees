using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;

namespace CompanyEmployees.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext;
        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
        public IQueryable<T> FindAll(bool trackChanges)
        {
            if (trackChanges)
            {
                return RepositoryContext.Set<T>();
            }
            else
            {
                return RepositoryContext.Set<T>().AsNoTracking();
            }
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            if (trackChanges)
            {
                return RepositoryContext.Set<T>().Where(expression);
            }
            else
            {
                return RepositoryContext.Set<T>().Where(expression).AsNoTracking();
            }
        }
        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
    }
}
