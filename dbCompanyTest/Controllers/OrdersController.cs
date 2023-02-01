using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dbCompanyTest.Models;
using dbCompanyTest.Hubs;

namespace dbCompanyTest.Controllers
{
    public class OrdersController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();
        //private readonly dbCompanyTestContext _context;

        //public OrdersController(dbCompanyTestContext context)
        //{
        //    _context = context;
        //}

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var dbCompanyTestContext = _context.Orders.Include(o => o.客戶編號Navigation);
            return View(await dbCompanyTestContext.ToListAsync());
        }

        public IActionResult getOrderDetails()
        {
            var datas = from c in _context.Orders
                        join o in _context.OrderDetails on c.訂單編號 equals o.訂單編號
                        join a in _context.ProductDetails on o.Id equals a.Id
                        join b in _context.ProductsSizeDetails on a.商品尺寸id equals b.商品尺寸id
                        join d in _context.ProductsColorDetails on a.商品顏色id equals d.商品顏色id
                        join e in _context.Products on a.商品編號id equals e.商品編號id
                        select new ViewModels.OrderDetail_List
                        {
                            訂單編號 = c.訂單編號,
                            送貨地址 = c.送貨地址,
                            商品名稱 = e.商品名稱,
                            尺寸種類 = b.尺寸種類,
                            色碼 = d.色碼,
                            商品數量 = (int)o.商品數量,
                            商品價格=(int)o.商品價格,
                            總金額= (int)o.商品價格* (int)o.商品數量,
                        };
            var test = datas.ToList();
            return Json(datas);
        }
        //public IActionResult getOrderDetails(string id)
        //{
        //    List < OrderDetail > orderDetail = null;
        //    var order = _context.OrderDetails.Select(x => x).Where(c => c.訂單編號 == id).ToList();
        //    foreach (OrderDetail x in order)
        //    {
        //        orderDetail.Add(x);
        //    }
        //    return Json(orderDetail);
        //}

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            var order = _context.OrderDetails.Select(x => x).Where(c => c.訂單編號 == id ).ToList();
            //var order = await _context.OrderDetails
            //    //.Include(o => o.客戶編號Navigation)
            //    .FirstOrDefaultAsync(m => m.訂單編號 == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("訂單編號,客戶編號,付款方式,送貨地址,總金額,下單時間,訂單狀態,付款狀態,收件人名稱,收件人電話,收件人email")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號", order.客戶編號);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號", order.客戶編號);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("訂單編號,客戶編號,付款方式,送貨地址,總金額,下單時間,訂單狀態,付款狀態,收件人名稱,收件人電話,收件人email")] Order order)
        {
            if (id != order.訂單編號)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.訂單編號))
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
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號", order.客戶編號);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.客戶編號Navigation)
                .FirstOrDefaultAsync(m => m.訂單編號 == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'dbCompanyTestContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(string id)
        {
          return _context.Orders.Any(e => e.訂單編號 == id);
        }
    }
}
