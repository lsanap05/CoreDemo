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
    public class ParentRepositoryTests
    {
        private DbContextOptions<ShoppingDbContext> _options;
        private ShoppingDbContext _context;
        private ParentRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ShoppingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ShoppingDbContext(_options);
            _repository = new ParentRepository(_context);

             _context.Parents.AddRange(new List<Parent>
            {
                new Parent { Id = 1, FirstName = "Heidi", LastName = "Taylor" },
                new Parent { Id = 2, FirstName = "Ivan", LastName = "Moore" }
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
        public async Task GetAsync_ReturnsAllParents()
        {
            var parents = await _repository.GetAsync();
            Assert.AreEqual(2, parents.Count());
        }

        [TestMethod]
        public async Task GetAsync_ById_ReturnsCorrectParent()
        {
            var parent = await _repository.GetAsync(1);
            Assert.IsNotNull(parent);
            Assert.AreEqual("Heidi", parent.FirstName);
        }

        [TestMethod]
        public async Task CreateAsync_AddsNewParent()
        {
            var newParent = new Parent { FirstName = "Judy", LastName = "Anderson" };
            var createdParent = await _repository.CreateAsync(newParent);

            Assert.IsNotNull(createdParent);
            Assert.IsTrue(createdParent.Id > 0);
            Assert.AreEqual(3, await _context.Parents.CountAsync());
        }

        [TestMethod]
        public async Task UpdateAsync_ModifiesExistingParent()
        {
           var parentToUpdate = await _context.Parents.FindAsync(1);
            parentToUpdate.LastName = "Taylor-Smith";
            var updatedParent = await _repository.UpdateAsync(1, parentToUpdate);

            Assert.IsNotNull(updatedParent);
            Assert.AreEqual("Taylor-Smith", updatedParent.LastName);
             var parentFromDb = await _context.Parents.FindAsync(1);
             Assert.AreEqual("Taylor-Smith", parentFromDb.LastName);
        }

        [TestMethod]
        public async Task DeleteAsync_RemovesParent()
        {
            bool result = await _repository.DeleteAsync(1);
            Assert.IsTrue(result);
            Assert.AreEqual(1, await _context.Parents.CountAsync());
            Assert.IsNull(await _context.Parents.FindAsync(1));
        }
    }
}
