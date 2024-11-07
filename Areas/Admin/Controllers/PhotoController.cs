using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Molla.Areas.Admin.Models.Product;
using Molla.Models;
using MyApplication.Data;
using Molla.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Molla.Areas_Admin_Controllers
{
    [Area("Admin")]
    [Route("Admin/Photo/[action]/{id?}")]
    public class PhotoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhotoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class UploadOneFile{
            public required IFormFile fileUpload {get; set;}
        }
        // GET: Photo
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Photos.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Photo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        public async Task<IActionResult> ListPhoto(int? ProductId)
        {
            if (ProductId == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .Where(m => m.ProductId == ProductId).ToListAsync();

            var json = new {
                success = true,
                photos = photo,

            };
            return Json(json);
        }

        // GET: Photo/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: Photo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Thumb")] ProductPhoto photo)
        {
            List<Photo> photos = await _context.Photos.Where(x => x.ProductId == photo.ProductId).ToListAsync();
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == photo.ProductId);
            if (existingProduct == null)
            {
                return NotFound();
            }
            Console.WriteLine("upload phto", photo.Thumb);

            if (ModelState.IsValid)
            {
                if (photo.Thumb != null)
                {
                    string extension = Path.GetExtension(photo.Thumb.FileName);
                    string fileName = Utilities.Utilities.GenerateSlug(existingProduct.ProductName + "-" + (photos.Count + 1)) + extension;
                    string? name = await Utilities.Utilities.UploadFile(photo.Thumb, fileName);
                    if (name != null)
                    {
                        Photo newPhoto = new Photo()
                        {
                            FileName = name,
                            ProductId = photo.ProductId
                        };
                        _context.Add(newPhoto);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                }
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", photo.ProductId);
            return View(photo);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int ProductId, [Bind("fileUpload")] UploadOneFile file)
        {
            var existingProduct = await _context.Products
                                    .Include(p => p.Photos)
                                    .FirstOrDefaultAsync(p => p.ProductId == ProductId);
            if (existingProduct == null)
            {
                return NotFound();
            }
            Console.WriteLine("upload phto", file.fileUpload.FileName);
            if (file != null)
            {
                Console.WriteLine("name");
                string extension = Path.GetExtension(file.fileUpload.FileName);
                string fileName = Utilities.Utilities.GenerateSlug(existingProduct.ProductName + "-" + (existingProduct.Photos.Count + 1)) + extension;
                string? name = await Utilities.Utilities.UploadFile(file.fileUpload, fileName);
                if (name != null)
                {
                    Photo newPhoto = new Photo()
                    {
                        FileName = name,
                        ProductId = ProductId
                    };
                    _context.Add(newPhoto);
                    await _context.SaveChangesAsync();
                }

            }
            return Ok();
        }

        // GET: Photo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", photo.ProductId);
            return View(photo);
        }

        // POST: Photo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,ProductId")] Photo photo)
        {
            if (id != photo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(photo.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", photo.ProductId);
            return View(photo);
        }

        // GET: Photo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo != null)
            {
                _context.Photos.Remove(photo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }
    }
}
