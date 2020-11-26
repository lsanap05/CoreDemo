using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BAL.Models;
using DAL.DBContext;

namespace DAL.Repositories
{
    public class CategoryRepository : IRepository<Category, int>
    {
         private readonly ShoppingDbContext context;
        /// <summary>
        ///  Inject the ShoppingDbContext
        /// </summary>
        public CategoryRepository(ShoppingDbContext context)
        {
            this.context = context;
        }
        public async Task<Category> CreateAsync(Category entity)
        {
            var res = await context.Categories.AddAsync(entity);
            await context.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await context.Categories.FindAsync(id);
            if (cat == null) return false;

            context.Categories.Remove(cat);
            context.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<Category>> GetAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category> GetAsync(int id)
        {
            return await context.Categories.FindAsync(id);
        }

        public async Task<Category> UpdateAsync(int id, Category entity)
        {
            var cat = await context.Categories.FindAsync(id);
            if (cat != null)
            {
                cat.CategoryId = entity.CategoryId;
                cat.CategoeyName = entity.CategoeyName;
                cat.BasePrice = entity.BasePrice;
                await context.SaveChangesAsync();
                return cat;
            }
            return entity;

        }
    }
}
