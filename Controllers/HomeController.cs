using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreApp.Models;

namespace StoreApp.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;
        if (!String.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString = searchString;
            products = products.Where(p => p.Name!.ToLower().Contains(searchString)).ToList();
        }

        if (!String.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();
        }


        var model = new ProductViewModel
        {
            Products = products,
            Categories = Repository.Categories,
            SelectedCategory = category,
        };
        return View(model);
    }
    [HttpGet]
    public ActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View();
    }
    [HttpPost]
    public async Task<ActionResult> Create(Product p, IFormFile imageFile)
    {
        var extension="";
        if (imageFile != null)
        {
            var allowExtensions = new[] { ".jpg", ".jpeg", ".png" };
            extension = Path.GetExtension(imageFile.FileName);
            if (!allowExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Geçerli bir resim seçiniz!");
            }
        }

        if (ModelState.IsValid)
        {
            var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
            if (imageFile != null)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile!.CopyToAsync(stream);
                }
                p.Image = randomFileName;
                p.ProductId = Repository.Products.Count + 1;
                Repository.CreateProduct(p);
            }

            
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(p);
    }
    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if (entity == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(entity);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product p, IFormFile? imageFile)
    {
        if (id != p.ProductId)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            if (imageFile != null)
            {
                var allowExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(imageFile.FileName);
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile!.CopyToAsync(stream);
                }
                p.Image=randomFileName;
            }
            Repository.EditProduct(p);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(p);

    }
    [HttpGet]
    public IActionResult Delete(int? id)
    { 
        if (id == null) 
            return NotFound();
        var entity = Repository.Products.FirstOrDefault(w=>w.ProductId==id);
        if (entity == null)
            return NotFound();
        return View("DeleteConfirm",entity);
    }
    [HttpPost]
    public IActionResult Delete(int? id,int productID)
    {
        if (id != productID) return NotFound();
        if (id == null) return NotFound();
        var entity = Repository.Products.FirstOrDefault(w => w.ProductId == productID);
        if (entity == null) return NotFound();

        Repository.Products.Remove(entity);
        return RedirectToAction("Index");
    }
    public IActionResult EditProducts(List<Product> products)
    {
        foreach (var product in products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
    }
}
