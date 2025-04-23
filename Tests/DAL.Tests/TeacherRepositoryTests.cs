using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL.Repositories;
using DAL.DBContext;
using BAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace DAL.Tests
{
    [TestClass]
    public class TeacherRepositoryTests
    {
        private DbContextOptions<ShoppingDbContext> _options;
        private ShoppingDbContext _context;
        private TeacherRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Use InMemory database for testing
            _options = new DbContextOptionsBuilder<ShoppingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name per test run
                .Options;
            _context = new ShoppingDbContext(_options);
            _repository = new TeacherRepository(_context);

            // Seed initial data
            _context.Teachers.AddRange(new List<Teacher>
            {
                new Teacher { Id = 1, FirstName = "Alice", LastName = "Smith", Subject = "Math" },
                new Teacher { Id = 2, FirstName = "Bob", LastName = "Jones", Subject = "History" }
            });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Teardown()
        {
            _context.Database.EnsureDeleted(); // Clean up the in-memory database
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsAllTeachers()
        {
            var teachers = await _repository.GetAsync();
            Assert.AreEqual(2, teachers.Count());
        }

        [TestMethod]
        public async Task GetAsync_ById_ReturnsCorrectTeacher()
        {
            var teacher = await _repository.GetAsync(1);
            Assert.IsNotNull(teacher);
            Assert.AreEqual("Alice", teacher.FirstName);
        }

        [TestMethod]
        public async Task GetAsync_ById_ReturnsNullForInvalidId()
        {
            var teacher = await _repository.GetAsync(99);
            Assert.IsNull(teacher);
        }

        [TestMethod]
        public async Task CreateAsync_AddsNewTeacher()
        {
            var newTeacher = new Teacher { FirstName = "Charlie", LastName = "Brown", Subject = "Art" };
            var createdTeacher = await _repository.CreateAsync(newTeacher);

            Assert.IsNotNull(createdTeacher);
            Assert.IsTrue(createdTeacher.Id > 0); // EF Core InMemory usually assigns IDs
            Assert.AreEqual(3, await _context.Teachers.CountAsync());
        }

        [TestMethod]
        public async Task UpdateAsync_ModifiesExistingTeacher()
        {
            var teacherToUpdate = await _context.Teachers.FindAsync(1);
            teacherToUpdate.Subject = "Advanced Math";
            var updatedTeacher = await _repository.UpdateAsync(1, teacherToUpdate);

            Assert.IsNotNull(updatedTeacher);
            Assert.AreEqual("Advanced Math", updatedTeacher.Subject);
            var teacherFromDb = await _context.Teachers.FindAsync(1);
            Assert.AreEqual("Advanced Math", teacherFromDb.Subject);
        }

         [TestMethod]
        public async Task UpdateAsync_ReturnsNullForMismatchedId()
        {
            var teacherToUpdate = new Teacher { Id = 1, FirstName = "Test", LastName="Test", Subject = "Physics" };
            var result = await _repository.UpdateAsync(2, teacherToUpdate); // Mismatched ID
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_RemovesTeacher()
        {
            bool result = await _repository.DeleteAsync(1);
            Assert.IsTrue(result);
            Assert.AreEqual(1, await _context.Teachers.CountAsync());
            Assert.IsNull(await _context.Teachers.FindAsync(1));
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsFalseForInvalidId()
        {
            bool result = await _repository.DeleteAsync(99);
            Assert.IsFalse(result);
            Assert.AreEqual(2, await _context.Teachers.CountAsync());
        }
    }
}
