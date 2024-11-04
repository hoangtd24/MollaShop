using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Molla.Areas.Admin.Models.Product;
using Molla.Models;
using Molla.Utilities;
using MyApplication.Data;

namespace MollaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Product/[action]/{id?}")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? CateId = 0)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CatName", CateId);
            var applicationDbContext = _context.Products
                                            .Include(p => p.Category).ToList();
            if (CateId != 0)
            {
                applicationDbContext = applicationDbContext.Where(c => c.CategoryId == CateId).ToList();
            }
            return View(applicationDbContext);
        }
        // GET: Product
        public IActionResult SortProduct([FromQuery] int? CateId = 0)
        {
            string redirectUrl = $"/Admin/Product/Index?CateId={CateId}";
            if (CateId == 0)
            {
                redirectUrl = "/Admin/Product/Index";
            }
            return Json(new { status = "success", url = redirectUrl });
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CatName");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ThumbFile)
        {
            if (ModelState.IsValid)
            {
                if (ThumbFile != null)
                {
                    string extension = Path.GetExtension(ThumbFile.FileName);
                    string fileName = Utilities.GenerateSlug(product.ProductName) + extension;
                    string? name = await Utilities.UploadFile(ThumbFile, fileName);
                    if (name != null)
                    {
                        product.Thumb = name;
                        product.DateCreated = DateTime.Now;
                        product.DateModify = DateTime.Now;
                        product.Alias = Utilities.GenerateSlug(product.ProductName);
                    }

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CatName", product.CategoryId);
            return View();
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CatName", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? Thumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            // Lấy sản phẩm hiện tại từ cơ sở dữ liệu
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra nếu có tệp mới được tải lên để cập nhật ảnh thumb
                    if (Thumb != null)
                    {
                        Console.WriteLine("aaaa", Thumb.FileName);
                        string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", existingProduct.Thumb);

                        // Delete the old image file if it exists
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        string extension = Path.GetExtension(Thumb.FileName);
                        string fileName = Utilities.GenerateSlug(product.ProductName) + extension;
                        string? name = await Utilities.UploadFile(Thumb, fileName);
                        if (name != null)
                        {
                            product.Thumb = name;
                        }
                    }
                    else
                    {
                        product.Thumb = existingProduct.Thumb;
                    }

                    // Cập nhật các thuộc tính khác của sản phẩm
                    product.DateModify = DateTime.Now;
                    // Cập nhật vào DbContext và lưu thay đổi
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu cập nhật thất bại, hiển thị lại trang chỉnh sửa
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CatName", product.CategoryId);
            return View(product);
        }


        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
