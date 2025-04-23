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
    public class StudentRepositoryTests
    {
        private DbContextOptions<ShoppingDbContext> _options;
        private ShoppingDbContext _context;
        private StudentRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ShoppingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ShoppingDbContext(_options);
            _repository = new StudentRepository(_context);

            _context.Students.AddRange(new List<Student>
            {
                new Student { Id = 1, FirstName = "Eve", LastName = "Davis", DateOfBirth = new DateTime(2005, 5, 5) },
                new Student { Id = 2, FirstName = "Frank", LastName = "Miller", DateOfBirth = new DateTime(2006, 6, 6) }
            });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsAllStudents()
        {
            var students = await _repository.GetAsync();
            Assert.AreEqual(2, students.Count());
        }

        [TestMethod]
        public async Task GetAsync_ById_ReturnsCorrectStudent()
        {
            var student = await _repository.GetAsync(1);
            Assert.IsNotNull(student);
            Assert.AreEqual("Eve", student.FirstName);
        }

        [TestMethod]
        public async Task CreateAsync_AddsNewStudent()
        {
            var newStudent = new Student { FirstName = "Grace", LastName = "Wilson", DateOfBirth = new DateTime(2007, 7, 7) };
            var createdStudent = await _repository.CreateAsync(newStudent);

            Assert.IsNotNull(createdStudent);
            Assert.IsTrue(createdStudent.Id > 0);
            Assert.AreEqual(3, await _context.Students.CountAsync());
        }

        [TestMethod]
        public async Task UpdateAsync_ModifiesExistingStudent()
        {
            var studentToUpdate = await _context.Students.FindAsync(1);
            studentToUpdate.LastName = "Davis-Smith";
            var updatedStudent = await _repository.UpdateAsync(1, studentToUpdate);

            Assert.IsNotNull(updatedStudent);
            Assert.AreEqual("Davis-Smith", updatedStudent.LastName);
            var studentFromDb = await _context.Students.FindAsync(1);
             Assert.AreEqual("Davis-Smith", studentFromDb.LastName);
        }

        [TestMethod]
        public async Task DeleteAsync_RemovesStudent()
        {
            bool result = await _repository.DeleteAsync(1);
            Assert.IsTrue(result);
            Assert.AreEqual(1, await _context.Students.CountAsync());
            Assert.IsNull(await _context.Students.FindAsync(1));
        }
    }
}
