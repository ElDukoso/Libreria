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
    public class OrderDetailsController : Controller
    {
        private readonly LibreriaContext _context;

        public OrderDetailsController(LibreriaContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var libreriaContext = _context.OrderDetails.Include(o => o.Book).Include(o => o.Order);
            return View(await libreriaContext.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Book)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            var orders = _context.Orders.ToList();
            var books = _context.Books.ToList();

            ViewBag.OrderId = orders.Any() ? new SelectList(orders, "Id", "Id") : new SelectList(Enumerable.Empty<Order>());
            ViewBag.BookId = books.Any() ? new SelectList(books, "Id", "Title") : new SelectList(Enumerable.Empty<Book>());
            return View();
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,BookId,Quantity,UnitPrice,Subtotal")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var orders = _context.Orders.ToList();
            var books = _context.Books.ToList();

            ViewBag.OrderId = orders.Any() ? new SelectList(orders, "Id", "Id", orderDetail.OrderId) : new SelectList(Enumerable.Empty<Order>());
            ViewBag.BookId = books.Any() ? new SelectList(books, "Id", "Title", orderDetail.BookId) : new SelectList(Enumerable.Empty<Book>());
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            var orders = _context.Orders.ToList();
            var books = _context.Books.ToList();

            ViewBag.OrderId = orders.Any() ? new SelectList(orders, "Id", "Id", orderDetail.OrderId) : new SelectList(Enumerable.Empty<Order>());
            ViewBag.BookId = books.Any() ? new SelectList(books, "Id", "Title", orderDetail.BookId) : new SelectList(Enumerable.Empty<Book>());
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,BookId,Quantity,UnitPrice,Subtotal")] OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.Id))
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

            var orders = _context.Orders.ToList();
            var books = _context.Books.ToList();

            ViewBag.OrderId = orders.Any() ? new SelectList(orders, "Id", "Id", orderDetail.OrderId) : new SelectList(Enumerable.Empty<Order>());
            ViewBag.BookId = books.Any() ? new SelectList(books, "Id", "Title", orderDetail.BookId) : new SelectList(Enumerable.Empty<Book>());
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Book)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
