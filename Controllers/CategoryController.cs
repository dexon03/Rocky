using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _dbContext.Category;

            return View(objList);
        }

        //Get-Create
        public IActionResult Create()
        {
            return View();
        }

        //Post-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //Get-Edit
        public IActionResult Edit(int? Id)
        {
            if (Id is null)
            {
                return NotFound();
            }

            var obj = _dbContext.Category.FirstOrDefault(c => c.Id == Id);
            if (obj is null)
            {
                return NotFound();
            }            

            return View(obj);
        }

        //POST-EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Category.Update(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //POST-DELETE
        public IActionResult Delete(int? Id)
        {
            if (Id is null)
            {
                return NotFound();
            }
            Category obj = _dbContext.Category.FirstOrDefault(c => c.Id == Id);
            if (obj is null)
            {
                return NotFound();
            }
            _dbContext.Category.Remove(obj);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}