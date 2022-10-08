using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationTypeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _dbContext.ApplicationType;
            return View(objList);
        }

        //Get-Create
        public IActionResult Create()
        {
            return View();
        }
        //POST-CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            _dbContext.ApplicationType.Add(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET-EDIT
        public IActionResult Edit(int? id)
        {
            if (id is null) return NotFound();
            var obj = _dbContext.ApplicationType.Find(id);
            return View(obj);
        }
        //POST-CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            _dbContext.ApplicationType.Update(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id is null) return NotFound();
            var obj = _dbContext.ApplicationType.Find(id);
            if (obj is null) return NotFound();
            _dbContext.ApplicationType.Remove(obj);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
