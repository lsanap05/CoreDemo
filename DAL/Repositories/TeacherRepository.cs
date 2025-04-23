using BAL.Models;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ShoppingDbContext _context;

        public TeacherRepository(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAsync()
        {
            return await _context.Teachers.ToListAsync(); // Assumes DbSet name is Teachers
        }

        public async Task<Teacher> GetAsync(int id)
        {
            return await _context.Teachers.FindAsync(id); // Assumes DbSet name is Teachers
        }

        public async Task<Teacher> CreateAsync(Teacher entity)
        {
            _context.Teachers.Add(entity); // Assumes DbSet name is Teachers
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Teacher> UpdateAsync(int id, Teacher entity)
        {
            if (id != entity.Id)
            {
                // Handle mismatch if necessary, or rely on caller validation
                return null; // Or throw an exception
            }
            _context.Entry(entity).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TeacherExists(id))
                {
                    return null; // Or handle not found case
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
            var teacher = await _context.Teachers.FindAsync(id); // Assumes DbSet name is Teachers
            if (teacher == null)
            {
                return false;
            }
            _context.Teachers.Remove(teacher); // Assumes DbSet name is Teachers
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> TeacherExists(int id)
        {
            return await _context.Teachers.AnyAsync(e => e.Id == id); // Assumes DbSet name is Teachers
        }
    }
}
