using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using work_01_Session.Models;

namespace work_01_Session.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ECommerceDbContext _context;
        private readonly IWebHostEnvironment _he;
        public ProductsController(ECommerceDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return _context.Products != null ?
                          View(await _context.Products.ToListAsync()) :
                          Problem("Entity set 'ECommerceDbContext.Products'  is null.");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ImagePath != null)
                {
                    // Image handling
                    string newFileName = Guid.NewGuid().ToString() + "_" + vm.ImagePath.FileName;
                    string newFilePath = Path.Combine("Images", newFileName);
                    string file = Path.Combine(_he.WebRootPath, newFilePath);
                    vm.ImagePath.CopyTo(new FileStream(file, FileMode.Create));

                    // Create product and save to database
                    Product product = new Product
                    {
                        Name = vm.Name,
                        Unit = vm.Unit,
                        Price = vm.Price,
                        Quantity = vm.Quantity,
                        Image = newFileName
                    };

                    // Save product to the database
                    _context.Products.Add(product);
                    _context.SaveChanges();

                    return PartialView("_success");
                }
            }
            else
            {
                return PartialView("_error");
            }

            return View(vm);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Unit,Price,Quantity,Image")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ECommerceDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
