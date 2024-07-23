using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Libreria.Data;
using Libreria.Models;

namespace Libreria.Controllers
{    
    [Authorize]
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
            var libreriaContext = _context.OrderDetails.Include(o => o.Book).Include(o => o.Order).Include(o => o.User);
            return View(await libreriaContext.ToListAsync());
        }
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Book)
                .Include(o => o.Order)
                .Include(o => o.User) // Incluye el User
                .Include(o => o.Order.Customer) // Incluye el Customer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        [Authorize]
        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            var books = _context.Books.ToList();
            var customers = _context.Customers.ToList();

            ViewBag.BookId = new SelectList(books, "Id", "Title");
            ViewBag.CustomerId = new SelectList(customers, "Id", "Name");
            return View();
        }

        [Authorize]
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

        [Authorize]
        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,BookId,Quantity,CustomerId")] OrderDetail orderDetail)
        {
            var book = await _context.Books.FindAsync(orderDetail.BookId);
            if (book != null)
            {
                if (book.Stock < orderDetail.Quantity)
                {
                    ModelState.AddModelError("", "Not enough stock available.");
                    var books = _context.Books.ToList();
                    var customers = _context.Customers.ToList();
                    ViewBag.BookId = new SelectList(books, "Id", "Title", orderDetail.BookId);
                    ViewBag.CustomerId = new SelectList(customers, "Id", "Name", orderDetail.CustomerId);
                    return View(orderDetail);
                }

                orderDetail.UnitPrice = book.Price;

                var customer = await _context.Customers.FindAsync(orderDetail.CustomerId);
                var discount = customer?.DiscountApplied == true ? customer.DiscountPercentage : 0;

                orderDetail.Subtotal = (int)(orderDetail.Quantity * orderDetail.UnitPrice * (1 - discount / 100m));

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                orderDetail.UserId = userId;

                if (orderDetail.OrderId == 0)
                {
                    var newOrder = new Order
                    {
                        CustomerId = customer?.Id,
                        Date = DateTime.Now,
                        Total = orderDetail.Subtotal
                    };
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();
                    orderDetail.OrderId = newOrder.Id;
                }

                // Update the book stock
                book.Stock -= orderDetail.Quantity;

                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Selected book not found.");

            var booksList = _context.Books.ToList();
            var customersList = _context.Customers.ToList();
            ViewBag.BookId = new SelectList(booksList, "Id", "Title", orderDetail.BookId);
            ViewBag.CustomerId = new SelectList(customersList, "Id", "Name", orderDetail.CustomerId);
            return View(orderDetail);
        }

        [Authorize]
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
            var customers = _context.Customers.ToList();

            ViewBag.BookId = new SelectList(books, "Id", "Title", orderDetail.BookId);
            ViewBag.CustomerId = new SelectList(customers, "Id", "Name", orderDetail.Order.CustomerId);
            return View(orderDetail);
        }

        [Authorize]
        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,BookId,Quantity,UnitPrice,Subtotal,CustomerId")] OrderDetail orderDetail)
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
                        var customer = await _context.Customers.FindAsync(orderDetail.CustomerId);
                        var discount = customer?.DiscountApplied == true ? customer.DiscountPercentage : 0;
                        orderDetail.Subtotal = orderDetail.Quantity * orderDetail.UnitPrice * (1 - (int)discount / 100);

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
            var customers = _context.Customers.ToList();
            ViewBag.BookId = new SelectList(books, "Id", "Title", orderDetail.BookId);
            ViewBag.CustomerId = new SelectList(customers, "Id", "Name", orderDetail.CustomerId);
            return View(orderDetail);
        }

        [Authorize]
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
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }
        
        [Authorize]
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
