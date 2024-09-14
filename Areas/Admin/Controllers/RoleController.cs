using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Molla.Areas.Admin.Models.Roles;
using Molla.Models;
using MyApplication.Data;

namespace MollaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleController> _logger;

        public RoleController(ILogger<RoleController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            List<ApplicationRoles> roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([Bind("RoleName", "Description")] CreateRoleViewModel role)
        {
            if(ModelState.IsValid){
                var newRole = new ApplicationRoles(){
                    Name = role.RoleName,
                    Description = role.Description?? "",
                };
                await _context.Roles.AddAsync(newRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(string? id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            return View(role);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(string id, [Bind("Id", "Name", "Description")] ApplicationRoles role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

                _context.Roles.Update(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}