using BAL.Models;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ParentRepository : IParentRepository
    {
        private readonly ShoppingDbContext _context;

        public ParentRepository(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Parent>> GetAsync()
        {
             return await _context.Parents.ToListAsync(); // Assumes DbSet name is Parents
        }

        public async Task<Parent> GetAsync(int id)
        {
            return await _context.Parents.FindAsync(id); // Assumes DbSet name is Parents
        }

        public async Task<Parent> CreateAsync(Parent entity)
        {
             _context.Parents.Add(entity); // Assumes DbSet name is Parents
            await _context.SaveChangesAsync();
            return entity;
        }

         public async Task<Parent> UpdateAsync(int id, Parent entity)
        {
             if (id != entity.Id)
            {
               return null;
            }
            _context.Entry(entity).State = EntityState.Modified;
             try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ParentExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var parent = await _context.Parents.FindAsync(id); // Assumes DbSet name is Parents
            if (parent == null)
            {
                return false;
            }
             _context.Parents.Remove(parent); // Assumes DbSet name is Parents
            await _context.SaveChangesAsync();
            return true;
        }

         private async Task<bool> ParentExists(int id)
        {
            return await _context.Parents.AnyAsync(e => e.Id == id); // Assumes DbSet name is Parents
        }
    }
}
