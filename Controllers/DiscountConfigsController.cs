using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Libreria.Data;
using Libreria.Models;

namespace Libreria.Controllers
{
    public class DiscountConfigsController : Controller
    {
        private readonly LibreriaContext _context;

        public DiscountConfigsController(LibreriaContext context)
        {
            _context = context;
        }

        // GET: DiscountConfigs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DiscountConfigs.ToListAsync());
        }

        // GET: DiscountConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountConfig = await _context.DiscountConfigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discountConfig == null)
            {
                return NotFound();
            }

            return View(discountConfig);
        }

        // GET: DiscountConfigs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiscountConfigs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DiscountPercentage")] DiscountConfig discountConfig)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discountConfig);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discountConfig);
        }

        // GET: DiscountConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountConfig = await _context.DiscountConfigs.FindAsync(id);
            if (discountConfig == null)
            {
                return NotFound();
            }
            return View(discountConfig);
        }

        // POST: DiscountConfigs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DiscountPercentage")] DiscountConfig discountConfig)
        {
            if (id != discountConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discountConfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountConfigExists(discountConfig.Id))
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
            return View(discountConfig);
        }

        // GET: DiscountConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountConfig = await _context.DiscountConfigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discountConfig == null)
            {
                return NotFound();
            }

            return View(discountConfig);
        }

        // POST: DiscountConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discountConfig = await _context.DiscountConfigs.FindAsync(id);
            if (discountConfig != null)
            {
                _context.DiscountConfigs.Remove(discountConfig);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountConfigExists(int id)
        {
            return _context.DiscountConfigs.Any(e => e.Id == id);
        }
    }
}
