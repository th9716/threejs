using dbCompanyTest.Hubs;
using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;

namespace dbCompanyTest.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
                return View();
            else
                return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult checkLogin(string account, string password)
        {
            dbCompanyTestContext db = new dbCompanyTestContext();
            if (password == null)
            {
                return Content("Nopassword");
            }
            var a = db.TestClients.FirstOrDefault(c => c.Email == account && c.密碼 == password);
            if (a != null)
            {
                useSession(a);
                return Content("成功");
            }
            else
            {
                return Content("失敗");
            }
        }

        public IActionResult loginSussess()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
            if (payload == null)
            {
                // 驗證失敗
                //ViewData["Msg"] = "驗證 Google 授權失敗";
                return RedirectToAction("Login");
            }
            else
            {
                dbCompanyTestContext db = new dbCompanyTestContext();
                TestClient? loggingUser = db.TestClients.FirstOrDefault(c => c.Email == payload.Email);
                if (loggingUser == null)
                {
                    TestClient newClient = new TestClient();
                    newClient.客戶編號 = $"CLG-{payload.JwtId.Substring(0, 7)}";
                    newClient.Email = payload.Email;
                    newClient.客戶姓名 = payload.Name;
                    db.TestClients.Add(newClient);
                    db.SaveChanges();
                    useSession(newClient);
                }
                else
                    useSession(loggingUser);
                //驗證成功，取使用者資訊內容
                //ViewData["Msg"] = "驗證 Google 授權成功" + "<br>";
                //ViewData["Msg"] += "Email:" + payload.Email + "<br>";
                //ViewData["Msg"] += "Name:" + payload.Name + "<br>";
                //ViewData["Msg"] += "Picture:" + payload.Picture;
                return RedirectToAction("index", "Home");
            }
        }
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? formCredential, string? formToken, string? cookiesToken)
        {
            // 檢查空值
            if (formCredential == null || formToken == null && cookiesToken == null)
            {
                return null;
            }

            GoogleJsonWebSignature.Payload? payload;
            try
            {
                // 驗證 token
                if (formToken != cookiesToken)
                {
                    return null;
                }

                // 驗證憑證
                IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                string GoogleClientId = Config.GetSection("GoogleClientId").Value;
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { GoogleClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(formCredential, settings);
                if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
                {
                    return null;
                }
                if (payload.ExpirationTimeSeconds == null)
                {
                    return null;
                }
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
                    if (now > expiration)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return payload;
        }

        public void useSession(TestClient x)
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
            {
                string json = JsonSerializer.Serialize(x);
                HttpContext.Session.SetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION, json);
            }
        }

        public IActionResult Logout()
        {
            //Gary
            MyLoveAndSoppingCar(CDittionary.SK_USE_FOR_MYLOVE_SESSION, CDittionary.SK_USE_FOR_LOGIN_USER_SESSION, false);
            //--------------------------------------------------------------
            //---購物車Logout--LU--感謝Gary<3
            MyLoveAndSoppingCar(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, CDittionary.SK_USE_FOR_LOGIN_USER_SESSION, true);
            //---購物車Logout結束--LU
            HttpContext.Session.Remove(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
            return RedirectToAction("Index", "Home");
        }

        private void MyLoveAndSoppingCar(string session, string usersession, bool MyloveOrShoppingcar)
        {
            if (HttpContext.Session.Keys.Contains(session))
            {
                dbCompanyTestContext _context = new dbCompanyTestContext();
                string userjson = HttpContext.Session.GetString(usersession);
                var userdata = JsonSerializer.Deserialize<TestClient>(userjson);
                var del = _context.會員商品暫存s.Select(x => x).Where(y => y.客戶編號 == userdata.客戶編號 && y.購物車或我的最愛 == MyloveOrShoppingcar);
                _context.會員商品暫存s.RemoveRange(del);
                _context.SaveChanges();
                //讀取我的最愛Session
                string lovejson = HttpContext.Session.GetString(session);
                var lovedata = JsonSerializer.Deserialize<List<會員商品暫存>>(lovejson).ToArray();
                foreach (var item in lovedata)
                {
                    item.Id = 0;
                }
                _context.會員商品暫存s.AddRange(lovedata);
                _context.SaveChanges();
                HttpContext.Session.Remove(session);
            }
        }

        public IActionResult getUser()
        {
            string? json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
            string userName = "";
            if (json == null)
                userName = "訪客";
            else
            {
                TestClient? x = JsonSerializer.Deserialize<TestClient>(json);
                userName = x.客戶姓名;
            }
            return Content(userName);
        }
        public IActionResult CreateClient()
        {
            return PartialView();
        }

        public IActionResult CheckClient(TestClient x)
        {
            if (x == null)
                return Content("請輸入資料");
            else
            {
                dbCompanyTestContext _context = new dbCompanyTestContext();
                if (!_context.TestClients.Any(c => c.Email == x.Email || c.客戶電話 == x.客戶電話 || c.身分證字號 == x.身分證字號))
                {
                    int count = _context.TestClients.Count() + 1;
                    x.客戶編號 = $"CL{x.身分證字號.Substring(1, 1)}-{count.ToString("0000")}{x.身分證字號.Substring(7, 3)}";
                    _context.TestClients.Add(x);
                    _context.SaveChanges();
                    useSession(x);
                    return Content("OK");
                }
                else
                    return Content("Email,電話或身分證字號已被使用");
            }
        }

        public IActionResult forgetPassword(string Email)
        {
            dbCompanyTestContext _context = new dbCompanyTestContext();
            if (_context.TestClients.Any(c => c.Email == Email))
            {//zlazqafpmuwxkxvo
                var mail = new MailMessage();
                mail.To.Add(Email);
                mail.Subject = "SheoseGift忘記密碼變更";
                mail.Body = $"<h1>ShoeSpace密碼變更</h1><a href=`https://localhost:7100/Login/RePassword?Email={Email}`><h2>點選這裡變更密碼</h2></a><hr/><h6>此訊息為系統自動寄出請勿直接回覆</h6>";
                //mail.Body = $"<form action = 'https://localhost:7100/Login/ResetPassword'><input type='hidden' value='{Email}' id='account' name='Email' /><input type='text' id='newPassword' name='Password'/><br/><input type='text' id='dblnewPassword' /><br/><input type='submit' value='確認'/></form>";
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                mail.From = new MailAddress("msit145finalpj@gmail.com", "SheoseGift");
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("msit145finalpj@gmail.com", "zlazqafpmuwxkxvo"),
                    EnableSsl = true
                };
                smtp.Send(mail);
                mail.Dispose();
                return Content("請至信箱接收密碼更改信件");
            }
            else
                return Content("沒有這個帳號");
        }
        [HttpGet]
        public IActionResult RePassword(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        public IActionResult ResetPassword(string Email, string Password)
        {
            dbCompanyTestContext _context = new dbCompanyTestContext();
            TestClient? client = _context.TestClients.FirstOrDefault(c => c.Email == Email);
            client.密碼 = Password;
            _context.SaveChanges();
            return Content("");
        }
    }
}
