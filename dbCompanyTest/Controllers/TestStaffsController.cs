using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using System.Collections;

namespace dbCompanyTest.Controllers
{
    public class TestStaffsController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();


        // GET: TestStaffs
        public async Task<IActionResult> Index()
        {
              return View(await _context.TestStaffs.ToListAsync());
        }

        // GET: TestStaffs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TestStaffs == null)
            {
                return NotFound();
            }

            var testStaff = await _context.TestStaffs
                .FirstOrDefaultAsync(m => m.員工編號 == id);
            if (testStaff == null)
            {
                return NotFound();
            }

            return View(testStaff);
        }

        // GET: TestStaffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TestStaffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("員工編號,員工姓名,員工電話,身分證字號,縣市,區,地址,Email,緊急聯絡人,聯絡人關係,聯絡人電話,部門,主管,職稱,密碼,薪資,權限,在職")] TestStaff testStaff)
        {
            var count = _context.TestStaffs.Count() + 1;
            testStaff.員工編號 = $"ST{testStaff.身分證字號.Substring(1, 1)}-{count.ToString("0000")}{testStaff.身分證字號.Substring(6, 3)}";

            if (ModelState.IsValid)
            {
                _context.Add(testStaff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(testStaff);
        }

        // GET: TestStaffs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TestStaffs == null)
            {
                return NotFound();
            }

            var testStaff = await _context.TestStaffs.FindAsync(id);
            if (testStaff == null)
            {
                return NotFound();
            }
            return View(testStaff);
        }

        // POST: TestStaffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("員工編號,員工姓名,員工電話,身分證字號,縣市,區,地址,Email,緊急聯絡人,聯絡人關係,聯絡人電話,部門,主管,職稱,密碼,薪資,權限,在職")] TestStaff testStaff)
        {
            if (id != testStaff.員工編號)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testStaff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestStaffExists(testStaff.員工編號))
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
            return View(testStaff);
        }

        // GET: TestStaffs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TestStaffs == null)
            {
                return NotFound();
            }

            var testStaff = await _context.TestStaffs
                .FirstOrDefaultAsync(m => m.員工編號 == id);
            if (testStaff == null)
            {
                return NotFound();
            }

            return View(testStaff);
        }

        // POST: TestStaffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TestStaffs == null)
            {
                return Problem("Entity set 'dbCompanyTestContext.TestStaffs'  is null.");
            }
            var testStaff = await _context.TestStaffs.FindAsync(id);
            if (testStaff != null)
            {
                _context.TestStaffs.Remove(testStaff);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestStaffExists(string id)
        {
          return _context.TestStaffs.Any(e => e.員工編號 == id);
        }
    }
}
