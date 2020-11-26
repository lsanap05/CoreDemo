using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAL.Models;
using CoreDemo.CustomeSession;
using DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoreDemo.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IRepository<Product, int> ProRepo;
        private readonly IRepository<Category, int> catRepo;
        /// <summary>
        /// Inject the Category Repository
        /// </summary>
        public ProductController(IRepository<Product, int> ProRepo, IRepository<Category, int> CatRepo)
        {
            this.ProRepo = ProRepo;
            this.catRepo = CatRepo;
        }
        /// <summary>
        /// Http Get methdo to return Index view with List of Categories
        /// </summary>
        /// <returns></returns>
        //public async Task<IActionResult> Index()
        //{
        //    var products = await ProRepo.GetAsync();
        //    ViewBag.CategoryRowId = new SelectList(await catRepo.GetAsync(), "CategoryRowId", "CategoeyName");
        //    return View(products);
        //}

        [Authorize(Policy = "readonlypolicy")]
       // [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Index()
        {
            // read data of category from the session
            Category cat = HttpContext.Session.GetSessionData<Category>("cat");
            List<Product> prds = new List<Product>();
            if(cat!=null)
            {
                var catRowId = cat.CategoryRowId;
                if (catRowId > 0)
                {
                    prds = ProRepo.GetAsync()
                        .Result.ToList()
                        .Where(c => c.CategoryRowId == catRowId).ToList();
                }
                else
                {
                    prds = ProRepo.GetAsync().Result.ToList();
                }
            }
            else
            {
                prds = ProRepo.GetAsync().Result.ToList();
            }

            ViewBag.CategoryRowId = new SelectList(await catRepo.GetAsync(), "CategoryRowId", "CategoeyName");

            return View(prds);
        }

        /// <summary>
        /// Http Get method that will return empty View for accepting Cateogry Data 
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Administration,Seller")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            var products = new Product();
            ViewBag.CategoryRowId = new SelectList(await catRepo.GetAsync(), "CategoryRowId", "CategoeyName");

            return View(products);
        }

        /// <summary>
        /// Http Post method to accept Category data from View request 
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Administration,Seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product products)
        {
            // validate the model
            if (ModelState.IsValid)
            {
                if (products.Price < 0)
                    throw new Exception("Price cannot be -ve");
                products = await ProRepo.CreateAsync(products);
                // return the Index action methods from
                // the current controller
                return RedirectToAction("Index");
            }
            return View(products); // stey on same page and show error messages
        }
        /// <summary>
        /// Get ID and shows category for edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Administration, Seller")]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> edit(int id)
        {
            var products = await ProRepo.GetAsync(id);
            ViewBag.CategoryRowId = new SelectList(await catRepo.GetAsync(), "CategoryRowId", "CategoeyName");
            return View(products);
        }
        /// <summary>
        ///  http post method to Update Category data from view request
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Administration,Seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> edit(Product products)
        {
            if (ModelState.IsValid)
            {
                products = await ProRepo.UpdateAsync(Convert.ToInt16(products.ProductId), products);
                // return the Index action methods from
                // the current controller
                return RedirectToAction("Index");
            }
            return View(products);
        }
        /// <summary>
        /// Get ID and shows details of Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id)
        {
            var products = await ProRepo.GetAsync(id);
            ViewBag.CategoryRowId = new SelectList(await catRepo.GetAsync(), "CategoryRowId", "CategoeyName");
            return View(products);
        }
        /// <summary>
        /// get ID From Main page and Delete The record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Administration, Seller")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var products = await ProRepo.GetAsync(id);
            return View(products);
        }
        [Authorize(Roles = "Administration, Seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id,string a="a")
        {
            var cats = await ProRepo.DeleteAsync(Convert.ToInt32(id));
            return RedirectToAction("Index");

        }
    }
}
