using BAL.Models;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProductRepository : IRepository<Product, int>
    {
        private readonly ShoppingDbContext context;
        public ProductRepository(ShoppingDbContext context)
        {
            this.context = context;
        }
        public async Task<Product> CreateAsync(Product entity)
        {
            var res = await context.products.AddAsync(entity);
            await context.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await context.products.FindAsync(id);
            if (cat == null) return false;
            context.products.Remove(cat);
            context.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<Product>> GetAsync()
        {
            return await context.products.ToListAsync();
        }

        public async Task<Product> GetAsync(int id)
        {
            return await context.products.FindAsync(id);
        }

        public async Task<Product> UpdateAsync(int id, Product entity)
        {

            var cat = await context.products.FindAsync(id);
            if (cat != null)
            {
                cat.ProductId = entity.ProductId;
                cat.ProductName = entity.ProductName;
                cat.Price = entity.Price;
                cat.Manufacturer = entity.Manufacturer;
                cat.CategoryRowId = entity.CategoryRowId;
                //cat.ProductRowId = entity.ProductRowId;
                await context.SaveChangesAsync();
                return cat;
            }
            return entity;
        }
    }
}
