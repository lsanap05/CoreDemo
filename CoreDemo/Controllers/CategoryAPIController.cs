using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreDemo.Controllers
{
    [Route("api/[controller]")]
     [ApiController]
    public class CategoryAPIController : ControllerBase
    {
        private readonly IRepository<Category, int> catRepo;
        public CategoryAPIController(IRepository<Category, int> catRepo)
        {
            this.catRepo = catRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var cats = await catRepo.GetAsync();
            return Ok(cats);
        }
        // id is passed through the HTTP Header
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var cat = await catRepo.GetAsync(id);
            return Ok(cat);
        }
        // CLR types are always passed throuhg the HTTP Request Body in POST and PUT requests 
        //[HttpPost("{categoryId}/{categoryName}/{basePrice}")]
        // public async Task<IActionResult> PostAsync(Category category)
        // public async Task<IActionResult> PostAsync([FromQuery]Category category)
        //public async Task<IActionResult> PostAsync([FromBody] Category category)
        [HttpPost]
        public async Task<IActionResult> PostAsync(Category category)
        {
            //try
            //{
                //Category category = new Category()
                //{ 
                //  CategoryId = categoryId,
                //  CategoeyName = categoryName,
                //  BasePrice = basePrice
                //}
                    ;

                if (ModelState.IsValid)
                {
                    if (category.BasePrice < 0) throw new Exception("Base Price cannot be -ve");
                    category = await catRepo.CreateAsync(category);
                    return Ok(category);
                }
                return BadRequest(ModelState);
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message);
            //}
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                category = await catRepo.UpdateAsync(id, category);
                return Ok(category); 
            }
            return BadRequest(ModelState);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var res = await catRepo.DeleteAsync(id);
            if (res)
            {
                return Ok($"Recored deleted successfully the result is {res}");
            }
            return NotFound($"Recored not found so the result of deletion is {res}");
        }
    }
}
