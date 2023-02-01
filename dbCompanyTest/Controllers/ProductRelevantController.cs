using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.SS.UserModel;
using System.Text;

namespace dbCompanyTest.Controllers
{
    public class ProductRelevantController : Controller
    {
        dbCompanyTestContext db = new dbCompanyTestContext();
        private IWebHostEnvironment _eviroment; //宣告取域 環境變數
        private readonly ILogger<ProductController> _logger;  //設定成紀錄類型
        Back_Product_library BP = new Back_Product_library();
        public ProductRelevantController(IWebHostEnvironment eviroment, ILogger<ProductController> logger)  //建構子，將環境變數注入
        {
            _eviroment = eviroment;
            _logger = logger; //注入，才能用session
        }
        //存放Product的資料，搜尋請改用Session
        //static List<Product> searchData = new List<Product>();

        public IActionResult Index()
        {
            return View();
        }

        //顏色
        #region 顏色
        public IActionResult ColorList()
        {
            var data = db.ProductsColorDetails.ToList();
            return Json(new { data });
        }

        public IActionResult ColorDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);            
            var data = db.ProductsColorDetails.FirstOrDefault(pd => pd.商品顏色id == _id);
            if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
            var ProData = db.ProductDetails.Where(p => p.商品顏色id == _id);
            if(ProData.Count()>0)return Content($"錯誤_還有ProductDetail使用此顏色，不能刪除", "text/plain", Encoding.UTF8);
            //刪除圖片
            if (data.商品顏色圖片 == null)return Content($"錯誤_沒有此圖片資料!", "text/plain", Encoding.UTF8);

            string Path = _eviroment.WebRootPath + "/images/colorimg/" + data.商品顏色圖片;
            (new Back_Product_library()).DeleteImg(Path);

            db.ProductsColorDetails.Remove(data);
            db.SaveChanges();
            return Content($"編號 {data.商品顏色id} 刪除成功", "text/plain", Encoding.UTF8);
        }



        public IActionResult _ColorEdit(string id)
        {
            //因用javascript ajax傳輸 請用 Content("錯誤_請輸入圖片!", "text/plain", Encoding.UTF8);          
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);                 
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);          
            var data = db.ProductsColorDetails.FirstOrDefault(pd => pd.商品顏色id == Int32.Parse(id));
            return PartialView(data);
        }
        [HttpPost]
        public IActionResult ColorEdit(ProductsColorDetail data, IFormFile 商品顏色圖片)
        {

              var PC = db.ProductsColorDetails.FirstOrDefault(pd => pd.商品顏色id == data.商品顏色id);
            if (PC == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            //先更改圖片位置
            //建立8位數亂碼
            string cold = (new Back_Product_library()).RandomString(8);
            string imgeName = $"{data.商品顏色id}_{cold}";

            //先建立圖片資料
            if (商品顏色圖片 != null)
            {
                BP.SavePhotoMethod(商品顏色圖片, $"{imgeName}", PC.商品顏色圖片, _eviroment.WebRootPath + "/images/colorimg/");
                PC.商品顏色圖片 = $"{imgeName}.jpg";
            }
            PC.商品顏色種類 = data.商品顏色種類;
            PC.色碼 = data.色碼;
            db.SaveChanges();

            return Content($"編號 {data.商品顏色id} 修改成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult _ColorCreate()
        {
            return PartialView();
        }

        public IActionResult ColorCreate(ProductsColorDetail data, IFormFile 商品顏色圖片)
        {
            //先更改圖片位置
            //建立8位數亂碼           
            string cold = (new Back_Product_library()).RandomString(8);
            int num = db.ProductsColorDetails.Count();
            var N_id = db.ProductsColorDetails.Take(num).Skip(num - 1).FirstOrDefault().商品顏色id;     
            if(N_id==null)
                return Content($"資料讀取錯誤", "text/plain", Encoding.UTF8);
            string imgeName = $"{N_id+1}_{cold}";
            ProductsColorDetail CC = new ProductsColorDetail();
            //先建立圖片資料
            if (商品顏色圖片 != null)
            {
                BP.SavePhotoMethod(商品顏色圖片, $"{imgeName}", "", _eviroment.WebRootPath + "/images/colorimg/");
                CC.商品顏色圖片 = $"{imgeName}.jpg";
            }
            CC.商品顏色id = data.商品顏色id;
            CC.商品顏色種類 = data.商品顏色種類;
            CC.色碼 = data.色碼;
            db.ProductsColorDetails.Add(CC);
            db.SaveChanges();
            // return PartialView();
            return Content($"新增成功", "text/plain", Encoding.UTF8);
        }
        #endregion

        //尺寸
        #region 尺寸
        public IActionResult SizeList()
        {
            var data = db.ProductsSizeDetails.ToList();
            return Json(new { data });
        }

        public IActionResult SizeDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsSizeDetails.FirstOrDefault(pd => pd.商品尺寸id == _id);
            if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
            var PDdata = db.ProductDetails.Where(p => p.商品尺寸id == _id);
            if(PDdata.Count()>0) return Content($"錯誤_還有ProductDetail使用此尺寸，不能刪除", "text/plain", Encoding.UTF8);
            db.ProductsSizeDetails.Remove(data);
            db.SaveChanges();
            return Content($"編號 {data.商品尺寸id} 刪除成功", "text/plain", Encoding.UTF8);
        }

       
        public IActionResult _SizeEdit(string id)
        {
            //因用javascript ajax傳輸 請用 Content("錯誤_請輸入圖片!", "text/plain", Encoding.UTF8);          
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsSizeDetails.FirstOrDefault(pd => pd.商品尺寸id == Int32.Parse(id));
            return PartialView(data);
        }
        public IActionResult SizeEdit(ProductsSizeDetail data)
        {
            
            var SE = db.ProductsSizeDetails.FirstOrDefault(pd => pd.商品尺寸id == data.商品尺寸id);
            if (SE == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            SE.商品尺寸id = data.商品尺寸id;
            SE.尺寸種類 = data.尺寸種類;
            db.SaveChanges();

            return Content($"編號 {data.商品尺寸id} 修改成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult _SizeCreate()
        {
            return PartialView();
        }

        public IActionResult SizeCreate(ProductsSizeDetail data)
        {
            ProductsSizeDetail SE = new ProductsSizeDetail();            
            SE.尺寸種類 = data.尺寸種類;
            db.ProductsSizeDetails.Add(SE);
            db.SaveChanges();

            return Content($"新建成功", "text/plain", Encoding.UTF8);
        }

        #endregion

        //類別
        #region 類別
        public IActionResult TypeList()
        {
            var data = db.ProductsTypeDetails.ToList();
            return Json(new { data });
        }

        public IActionResult TypeDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsTypeDetails.FirstOrDefault(pd => pd.商品分類id == _id);
            if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
            var Tdata = db.Products.Where(p => p.商品分類id == _id);
            if (Tdata.Count() > 0) return Content($"錯誤_還有Product使用此分類，不能刪除", "text/plain", Encoding.UTF8);
            db.ProductsTypeDetails.Remove(data);
            db.SaveChanges();
            return Content($"編號 {data.商品分類id} 刪除成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult _TypeEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsTypeDetails.FirstOrDefault(pd => pd.商品分類id == _id);
            return PartialView(data);
        }

        public IActionResult TypeEdit(ProductsTypeDetail data)
        {

            var PT = db.ProductsTypeDetails.FirstOrDefault(pd => pd.商品分類id == data.商品分類id);
            if (PT == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            //PT.商品分類id = data.商品分類id;
            PT.商品分類名稱 = data.商品分類名稱;
            db.SaveChanges();

            return Content($"編號 {data.商品分類id} 修改成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult _TypeCreate()
        {
            return PartialView();
        }

        public IActionResult TypeCreate(ProductsTypeDetail data)
        {

            ProductsTypeDetail PT = new ProductsTypeDetail();

            PT.商品分類id = data.商品分類id;
            PT.商品分類名稱 = data.商品分類名稱;
            db.ProductsTypeDetails.Add(PT);
            db.SaveChanges();

            return Content($"新建成功", "text/plain", Encoding.UTF8);
        }
        #endregion

        //鞋種
        #region 鞋種
        public IActionResult ShoesList()
        {
            var data = db.商品鞋種s.ToList();
            return Json(new { data });
        }

        public IActionResult _ShoesCreate()
        {
            return PartialView();
        }

        public IActionResult ShoesCreate(商品鞋種 data)
        {

            商品鞋種 SD= new 商品鞋種();

            //SD.商品鞋種id = data.商品鞋種id;
            SD.鞋種 = data.鞋種;
            db.商品鞋種s.Add(SD);
            db.SaveChanges();

            return Content($"新建成功", "text/plain", Encoding.UTF8);
        }


        public IActionResult _ShoesEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.商品鞋種s.FirstOrDefault(pd => pd.商品鞋種id == _id);
            return PartialView(data);
        }

        public IActionResult ShoesEdit(商品鞋種 data)
        {

            var SD = db.商品鞋種s.FirstOrDefault(pd => pd.商品鞋種id == data.商品鞋種id);
            if (SD == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            //SD.商品鞋種id = data.商品鞋種id;
            SD.鞋種 = data.鞋種;
            db.SaveChanges();

            return Content($"編號 {data.商品鞋種id} 修改成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult ShoesDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.商品鞋種s.FirstOrDefault(pd => pd.商品鞋種id == _id);
            if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
            var SHdata = db.Products.Where(p => p.商品鞋種id == _id);
            if (SHdata.Count() > 0) return Content($"錯誤_還有Product使用此鞋種，不能刪除", "text/plain", Encoding.UTF8);
            db.商品鞋種s.Remove(data);
            db.SaveChanges();
            return Content($"編號 {data.商品鞋種id} 刪除成功", "text/plain", Encoding.UTF8);
        }

        #endregion
    }
}
