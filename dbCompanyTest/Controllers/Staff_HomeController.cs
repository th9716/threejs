using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Net.Mail;
using System.Security.Policy;
using System.Security.Principal;
using static NuGet.Packaging.PackagingConstants;

namespace dbCompanyTest.Controllers
{
    public class Staff_HomeController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();
        public IActionResult Index()
        {
            string stfNum = HttpContext.Session.GetString("Account");
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);

            ViewBag.acc = $"{stfNum} {stf.員工姓名} {stf.部門}";
            return View();
        }
        public async Task<IActionResult> Index_HR()
        {
            string stfNum = HttpContext.Session.GetString("Account");
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);

            ViewBag.HR = $"{stfNum} {stf.員工姓名} {stf.部門}";
            return View(await _context.TestStaffs.ToListAsync());
        }

        public IActionResult login()
        {
            //HttpContext.Session.Remove("Account");
            string stfNum = HttpContext.Session.GetString("Account");
            if (HttpContext.Session.Keys.Contains("Account"))
            {
                TestStaff x = _context.TestStaffs.FirstOrDefault(T => T.員工編號 == stfNum);
                if (x.部門 == "行政" || x.部門 == "執行長室")
                {
                    return RedirectToAction("Index");
                }
                else if (x.部門 == "人事")
                {
                    return RedirectToAction("Index_HR");
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult login(/*CLoginViewModels vm, */string account, string password)
        {
            TestStaff x = _context.TestStaffs.FirstOrDefault(T => T.員工編號.Equals(account) && T.密碼.Equals(password));
            if (x != null)
            {
                if (x.密碼.Equals(password) && x.員工編號.Equals(account))
                {
                    if (!HttpContext.Session.Keys.Contains("Account"))
                        HttpContext.Session.SetString("Account", account);
                    return Content("success");
                }
            }
            else
            {
                if (account == null || password == null)
                {
                    return Content("CantNull");
                }
                return Content("false");
            }
            return View();
        }
        public IActionResult logout()
        {
            HttpContext.Session.Remove("Account");
            return RedirectToAction("login");
        }
        public IActionResult PartialSheeplist()
        {
            return PartialView();
        }
        public IActionResult PartialToDoList()
        {
            return PartialView();
        }
        public IActionResult Create_TDL()
        {
            string stfNum = HttpContext.Session.GetString("Account");
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);

            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            return View();
        }

        public IActionResult DT_TDL(int listNum)
        {
            string stfNum = HttpContext.Session.GetString("Account");
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            ViewBag.dep = stf.部門;
            var data = _context.ToDoLists.FirstOrDefault(c => c.交辦事項id == listNum);

            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DT_TDL(int id, [Bind("交辦事項id,員工編號,表單類型,表單內容,回覆,表單狀態,起單時間,起單人,部門主管,部門主管簽核,部門主管簽核意見,部門主管簽核時間,協辦部門,協辦部門簽核,協辦部門簽核人員,協辦部門簽核意見,協辦部門簽核時間,老闆簽核,老闆簽核意見,老闆簽核時間,執行人,執行時間,執行人簽核,附件,附件path")] ToDoList toDoList)
        {


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
            return RedirectToAction("Index_HR");
        }
        private bool ToDoListExists(int id)
        {
            return _context.ToDoLists.Any(e => e.交辦事項id == id);
        }

        public IActionResult PartialSchedule_HR()
        {
            return PartialView();
        }
        public IActionResult PartialSchedule_RG()
        {
            return PartialView();
        }


        //==================================================
        public IActionResult LoadSheeplist()
        {
            var datas = from c in _context.Orders
                        join o in _context.OrderDetails on c.訂單編號 equals o.訂單編號
                        join a in _context.ProductDetails on o.Id equals a.Id
                        join b in _context.ProductsSizeDetails on a.商品尺寸id equals b.商品尺寸id
                        join d in _context.ProductsColorDetails on a.商品顏色id equals d.商品顏色id
                        join e in _context.Products on a.商品編號id equals e.商品編號id
                        where c.訂單狀態 == "待出貨"
                        select new ViewModels.Cback_order_list
                        {
                            訂單編號 = c.訂單編號,
                            客戶編號 = c.客戶編號,
                            送貨地址 = c.送貨地址,
                            商品名稱 = e.商品名稱,
                            尺寸種類 = b.尺寸種類,
                            色碼 = d.色碼,
                            商品數量 = (int)o.商品數量
                        };
            var test = datas.ToList();
            return Json(datas);
        }

        public IActionResult LoadToDoList(string stf)
        {
            if (stf == "ST2-0010170")
            {
                var datas = from c in _context.ToDoLists
                            join o in _context.TestStaffs on c.員工編號 equals o.員工編號
                            where c.員工編號 == stf || c.起單人 == stf || c.執行人 == stf || c.表單類型 == "人事表單"
                            select c;
                return Json(datas);
            }
            else
            {
                var datas = from c in _context.ToDoLists
                            join o in _context.TestStaffs on c.員工編號 equals o.員工編號
                            where c.員工編號 == stf || c.協辦部門簽核人員 == stf || c.部門主管 == stf || c.起單人 == stf || c.執行人 == stf
                            select c;
                return Json(datas);
            }


        }
        public IActionResult stf_info(string stf)
        {
            TestStaff datas = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stf);
            return Json(datas);
        }
        public IActionResult all_stf_info()
        {
            var datas = from c in _context.TestStaffs select c;
            return Json(datas);
        }
        public IActionResult stf_info_dep(string dep)
        {
            var datas = from c in _context.TestStaffs
                        where c.部門 == dep
                        select c;
            var data = (from t in datas
                        orderby Guid.NewGuid()
                        select t).Take(2);
            return Json(data);
        }

        public IActionResult stf_info1(string i_whostart, string s_executor, string i_spvs, string i_co, string i_boss)
        {
            List<TestStaff> data = new List<TestStaff>();
            var datas = from c in _context.TestStaffs select c;
            var data1 = from t in datas where t.員工編號 == i_whostart select t;
            var data2 = from b in datas where b.員工編號 == i_spvs select b;
            var data3 = from d in datas where d.員工編號 == i_co select d;
            var data4 = from f in datas where f.員工編號 == i_boss select f;
            var data5 = from a in datas where a.員工編號 == s_executor select a;
            data.AddRange(data1); data.AddRange(data2); data.AddRange(data3);
            data.AddRange(data4); data.AddRange(data5);


            return Json(data);
        }
        [HttpPost]
        public IActionResult forgetPassword(string account)
        {
            //var x = from t in _context.TestStaffs where t.員工編號 == account select t; 
            TestStaff x = _context.TestStaffs.FirstOrDefault(T => T.員工編號 == account);
            if (x != null)
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.To.Add(x.Email);
                msg.From = new MailAddress("msit145finalpj@gmail.com", "Shoespace", System.Text.Encoding.UTF8);
                msg.Subject = "員工忘記密碼";
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//主旨編碼
                msg.Body = $"<h5 id=\"stf_info\">{x.員工編號} {x.員工姓名} 您好!</h5>";
                msg.Body += $"<a href=`https://localhost:7100/Staff_Home/ResetPassword?account={account}`>點選此連結變更密碼</a>";
                msg.BodyEncoding = System.Text.Encoding.UTF8;//內文編碼
                msg.IsBodyHtml = true; //!!!

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new System.Net.NetworkCredential("msit145finalpj@gmail.com", "zlazqafpmuwxkxvo");
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(msg);
                msg.Dispose();

                return Json("請至信箱接收密碼更改信件");
            }
            else
                return Json("沒有這個帳號");
        }
        public IActionResult ResetPassword(string account)
        {
            ViewBag.account = account;
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(string stf_info, string Password_F)
        {
            TestStaff datas = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stf_info);
            if (datas.密碼 == Password_F)
            {
                return Content("repeat");
            }
            else
            {
                datas.密碼 = Password_F;
                _context.SaveChanges();
                return Content("success");
            }
        }


        public IActionResult StaffNum()
        {
            string stfNum = "";
            if (HttpContext.Session.Keys.Contains("Account"))
                stfNum = HttpContext.Session.GetString("Account");
            else
                stfNum = "fales";
            return Content(stfNum);
        }

        public IActionResult TDLCount()
        {
            var datas = (from c in _context.ToDoLists select c).OrderByDescending(d=>d.交辦事項id).FirstOrDefault().交辦事項id;
            var count = Convert.ToInt32(datas) + 1;
            return Content(count.ToString());
        }


    }
}
