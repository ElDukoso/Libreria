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
            var books = _context.Books.ToList();

            ViewBag.BookId = new SelectList(books, "Id", "Title");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetBookPrice(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }
            return Json(new { price = book.Price });
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,BookId,Quantity")] OrderDetail orderDetail)
        {

           var book = await _context.Books.FindAsync(orderDetail.BookId);
                if (book != null)
                {
                    orderDetail.UnitPrice = book.Price;
                    orderDetail.Subtotal = orderDetail.Quantity * orderDetail.UnitPrice;

                    // Genera una nueva orden si no se proporciona una
                    if (orderDetail.OrderId == 0)
                    {
                        var newOrder = new Order
                        {
                            CustomerId = 1, // Cambia esto según tu lógica de negocio
                            Date = DateTime.Now,
                            Total = orderDetail.Subtotal
                        };
                        _context.Orders.Add(newOrder);
                        await _context.SaveChangesAsync();
                        orderDetail.OrderId = newOrder.Id;
                    }

                    _context.Add(orderDetail);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Selected book not found.");
            

            var books = _context.Books.ToList();
            ViewBag.BookId = new SelectList(books, "Id", "Title", orderDetail.BookId);
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

            var books = _context.Books.ToList();

            ViewBag.BookId = new SelectList(books, "Id", "Title", orderDetail.BookId);
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
                    var book = await _context.Books.FindAsync(orderDetail.BookId);
                    if (book != null)
                    {
                        orderDetail.UnitPrice = book.Price;
                        orderDetail.Subtotal = orderDetail.Quantity * orderDetail.UnitPrice;

                        _context.Update(orderDetail);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Selected book not found.");
                        return View(orderDetail);
                    }
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

            var books = _context.Books.ToList();
            ViewBag.BookId = new SelectList(books, "Id", "Title", orderDetail.BookId);
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
