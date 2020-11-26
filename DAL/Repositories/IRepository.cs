using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Generic  repository interface to have common CRUD methods for all
    /// Enriry classes e.g. Categoey, Product.
    /// The 'in' means input parameter
    /// TEntity is a Gneeric type which will always be a class
    /// thats why it has constraints e.g. where TEntity : class
    /// </summary>
    /// <typeparam name="TEntity">The Entity Class</typeparam>
    /// <typeparam name="TPk">The Primary Key Proeprty that will be always input parameter</typeparam>
    public interface IRepository<TEntity, in TPk> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync(); // read all to return list of data
        Task<TEntity> GetAsync(TPk id); // read single record based on P.K.
        Task<TEntity> CreateAsync(TEntity entity); // create new
        Task<TEntity> UpdateAsync(TPk id, TEntity entity); // update based ob P.K.
        Task<bool> DeleteAsync(TPk id); // delete based on P.K.

    }
}
