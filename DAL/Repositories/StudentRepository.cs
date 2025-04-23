using BAL.Models;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ShoppingDbContext _context;

        public StudentRepository(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAsync()
        {
            return await _context.Students.ToListAsync(); // Assumes DbSet name is Students
        }

        public async Task<Student> GetAsync(int id)
        {
            return await _context.Students.FindAsync(id); // Assumes DbSet name is Students
        }

        public async Task<Student> CreateAsync(Student entity)
        {
            _context.Students.Add(entity); // Assumes DbSet name is Students
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Student> UpdateAsync(int id, Student entity)
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
                if (!await StudentExists(id))
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
            var student = await _context.Students.FindAsync(id); // Assumes DbSet name is Students
            if (student == null)
            {
                return false;
            }
            _context.Students.Remove(student); // Assumes DbSet name is Students
            await _context.SaveChangesAsync();
            return true;
        }

         private async Task<bool> StudentExists(int id)
        {
            return await _context.Students.AnyAsync(e => e.Id == id); // Assumes DbSet name is Students
        }
    }
}
