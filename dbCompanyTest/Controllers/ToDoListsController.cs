using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dbCompanyTest.Models;

namespace dbCompanyTest.Controllers
{
    public class ToDoListsController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();
        //private readonly dbCompanyTestContext _context;

        //public ToDoListsController(dbCompanyTestContext context)
        //{
        //    _context = context;
        //}

        // GET: ToDoLists
        //public async Task<IActionResult> Index()
        //{
        //    var dbCompanyTestContext = _context.ToDoLists.Include(t => t.員工編號Navigation);
        //    return View(await dbCompanyTestContext.ToListAsync());
        //}

        // GET: ToDoLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ToDoLists == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists
                .Include(t => t.員工編號Navigation)
                .FirstOrDefaultAsync(m => m.交辦事項id == id);
            if (toDoList == null)
            {
                return NotFound();
            }

            return View(toDoList);
        }

        // GET: ToDoLists/Create
        public IActionResult Create()
        {
            //ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號");
            string stfNum = HttpContext.Session.GetString("Account");
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);

            ViewBag.acc = $"{stf.員工姓名} {stfNum} {stf.部門}";
            ViewBag.dep = stf.部門;
            return View();
        }

        // POST: ToDoLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("交辦事項id,員工編號,表單類型,表單內容,回覆,表單狀態,起單時間,起單人,部門主管,部門主管簽核,部門主管簽核意見,部門主管簽核時間,協辦部門,協辦部門簽核,協辦部門簽核人員,協辦部門簽核意見,協辦部門簽核時間,老闆簽核,老闆簽核意見,老闆簽核時間,執行人,執行時間,執行人簽核,附件,附件path")] ToDoList toDoList)
        {
            if (/*ModelState.IsValid*/true)
            {
                
                _context.Add(toDoList);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Staff_Home");
            }
            ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號", toDoList.員工編號);
            return View(toDoList);
        }

        // GET: ToDoLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ToDoLists == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists.FindAsync(id);
            if (toDoList == null)
            {
                return NotFound();
            }
            ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號", toDoList.員工編號);
            return View(toDoList);
        }

        // POST: ToDoLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("交辦事項id,員工編號,表單類型,表單內容,回覆,表單狀態,起單時間,起單人,部門主管,部門主管簽核,部門主管簽核意見,部門主管簽核時間,協辦部門,協辦部門簽核,協辦部門簽核人員,協辦部門簽核意見,協辦部門簽核時間,老闆簽核,老闆簽核意見,老闆簽核時間,執行人,執行時間,執行人簽核,附件,附件path")] ToDoList toDoList)
        {
            if (id != toDoList.交辦事項id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDoList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoListExists(toDoList.交辦事項id))
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
            ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號", toDoList.員工編號);
            return View(toDoList);
        }

        // GET: ToDoLists/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.ToDoLists == null)
        //    {
        //        return NotFound();
        //    }

        //    var toDoList = await _context.ToDoLists
        //        .Include(t => t.員工編號Navigation)
        //        .FirstOrDefaultAsync(m => m.交辦事項id == id);
        //    if (toDoList == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(toDoList);
        //}

        // POST: ToDoLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDoLists == null)
            {
                return Problem("Entity set 'dbCompanyTestContext.ToDoLists'  is null.");
            }
            var toDoList = await _context.ToDoLists.FindAsync(id);
            if (toDoList != null)
            {
                _context.ToDoLists.Remove(toDoList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoListExists(int id)
        {
          return _context.ToDoLists.Any(e => e.交辦事項id == id);
        }
    }
}
