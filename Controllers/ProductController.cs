using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _dbContext.Product.Include(u => u.Category).Include(u => u.ApplicationType);

            //foreach (var obj in objList)
            //{
            //    obj.Category = _dbContext.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            //    obj.ApplicationType = _dbContext.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            //}
            return View(objList);
        }

        //Get-Upsert
        public IActionResult Upsert(int? Id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _dbContext.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            //ViewBag.CategoryDropDown = CategoryDropDown;
            //Product product = new Product();
            ProductVM productVm = new ProductVM
            {
                Product = new Product(),
                CategorySelectList = _dbContext.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationTypeSelectList = _dbContext.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (Id is null)
            {
                return View(productVm);
            }

            productVm.Product = _dbContext.Product.Find(Id);
            if (productVm.Product is null) return NotFound();
            return View(productVm);
        }

        //Post-Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVm)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (productVm.Product.Id == 0)
                {
                    //Create
                    var upload = webRootPath + WC.ImagePath;
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVm.Product.Image = fileName + extension;
                    _dbContext.Product.Add(productVm.Product);
                    
                }
                else
                {
                    //Update
                    var objFromDb = _dbContext.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVm.Product.Id);
                    if (files.Count > 0)
                    {
                        var upload = webRootPath + WC.ImagePath;
                        var fileName = Guid.NewGuid().ToString();
                        var extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVm.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVm.Product.Image = objFromDb.Image;
                    }

                    _dbContext.Product.Update(productVm.Product);
                }
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            productVm.CategorySelectList = _dbContext.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVm.ApplicationTypeSelectList = _dbContext.ApplicationType.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(productVm);
        }

        public IActionResult Delete(int? Id)
        {
            if (Id is null) return NotFound();
            var product = _dbContext.Product.Find(Id);
            if (product is null) return NotFound();
            var upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, product.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _dbContext.Product.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
