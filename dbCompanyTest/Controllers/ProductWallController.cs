using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.Xml;
using X.PagedList;


namespace dbCompanyTest.Controllers
{
    public class ProductWallController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();

        public IActionResult Index(int? id, int page = 1) 
        {

            if (id == null)
                return NotFound();
            else
            {
                var datas = from c in _context.Products
                            join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                            join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                            join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                            join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                            join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                            join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                            where c.商品分類id == id
                            select new ViewModels.ProductWallViewModel
                            {
                                鞋種名稱 = b.鞋種,
                                商品id = (int)d.商品編號id,
                                商品分類id = (int)id,
                                商品鞋種id = (int)c.商品鞋種id,
                                商品名稱 = c.商品名稱,
                                商品價格 = (decimal)c.商品價格,
                                產品圖片1 = f.商品圖片1,
                                商品分類名稱 = e.商品分類名稱,
                                商品顏色id=(int)d.商品顏色id,
                                顏色名稱=g.商品顏色種類,
                                尺寸名稱=h.尺寸種類,
                                材質名稱=c.商品材質
                            };

                return View(datas.ToPagedList(page, 8));
            }
        }
        [HttpPost]
        public IActionResult pro(string word, int page = 1)
        {
            
            var datas = from c in _context.Products
                            join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                            join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                            join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                            join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                            join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                            join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                            where h.尺寸種類 == word || g.商品顏色種類 == word || c.商品材質 == word
                            select new ViewModels.ProductWallViewModel
                            {
                                鞋種名稱 = b.鞋種,
                                商品id = (int)d.商品編號id,
                                商品鞋種id = (int)c.商品鞋種id,
                                商品名稱 = c.商品名稱,
                                商品價格 = (decimal)c.商品價格,
                                產品圖片1 = f.商品圖片1,
                                商品分類名稱 = e.商品分類名稱,
                                商品顏色id = (int)d.商品顏色id,
                                顏色名稱 = g.商品顏色種類,
                                尺寸名稱 = h.尺寸種類,
                                材質名稱 = c.商品材質
                            };

                return PartialView(datas.ToPagedList(page, 8));
            
        }
        public IActionResult typeNav(int? id,string? type)
        {
            var datas = from c in _context.商品鞋種s
                        select new ViewModels.ProductWallViewModel
                        {
                            鞋種名稱 = c.鞋種,
                            商品鞋種id = c.商品鞋種id,
                            商品分類id=(int)id,
                            商品分類名稱=type,
                        };

            return PartialView(datas);
        }

        public IActionResult type(int? id,int? tid,string? type, int page = 1)
        {
            if (id == null)
                return NotFound();
            else
            {
                var datas = from c in _context.Products
                            join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                            join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                            join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                            join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                            join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                            join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                            where c.商品鞋種id == id
                            select new ViewModels.ProductWallViewModel
                            {
                                鞋種名稱 = b.鞋種,
                                商品id = (int)d.商品編號id,
                                商品分類id = (int)tid,
                                商品鞋種id = (int)c.商品鞋種id,
                                商品名稱 = c.商品名稱,
                                商品價格 = (decimal)c.商品價格,
                                產品圖片1 = f.商品圖片1,
                                商品分類名稱 = type,
                                商品顏色id = (int)d.商品顏色id,
                                顏色名稱 = g.商品顏色種類,
                                尺寸名稱 = h.尺寸種類,
                                材質名稱 = c.商品材質
                            };
                        ViewBag.tid = tid;
                        ViewBag.type=type;
                return View(datas.ToPagedList(page, 8));
            }
        }

        public IActionResult search(string? keyword, int page = 1)
        {
            
            var datas = from c in _context.Products
                        join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                        join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                        join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                        join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                        join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                        where c.商品名稱.Contains(keyword)
                        select new ViewModels.ProductWallViewModel
                        {
                            商品id = (int)d.商品編號id,
                            商品鞋種id = (int)c.商品鞋種id,
                            商品名稱 = c.商品名稱,
                            商品價格 = (decimal)c.商品價格,
                            產品圖片1 = f.商品圖片1,
                            keyword = keyword,
                            商品顏色id = (int)d.商品顏色id,
                            顏色名稱 = g.商品顏色種類
                        };
                        ViewBag.keyword=keyword;
            return View(datas.ToPagedList(page, 8));
            
        }

        public IActionResult selectview()
        {
            ProductWallViewModel pwv = new ProductWallViewModel();
            var datas = (from c in _context.Products
                         join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                         join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                         join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                         select new ViewModels.ProductWallViewModel
                         {
                             顏色名稱 = g.商品顏色種類,
                             尺寸名稱 = h.尺寸種類,
                             材質名稱 = c.商品材質
                         }).Distinct();

            foreach (var item in datas.Distinct())
            {
                pwv.顏色名稱 = item.顏色名稱;
                pwv.尺寸名稱 = item.尺寸名稱;
                pwv.材質名稱 = item.材質名稱;
            }

                return PartialView(datas);
        }

       

        //---------------------- Gary產品頁 ----------------------------
        public IActionResult Details(int? id, int? colorID)
        {
            //測試用 productDetail ID
            if (id == null)
            {
                id = 1;
                colorID = 1;
            }
            ProductDetailViewModels pdm = new ProductDetailViewModels();


            //透過productID找出全部的顏色圖片，商品尺寸，ProductDetail，商品顏色ID
            pdm.pro商品顏色圖片list = new List<string>();
            pdm.pro商品尺寸list = new List<string>();
            pdm.pro商品尺寸idlist = new List<int>();
            //pdm.pro商品DetailIDlist = new List<int>();//可以拿掉
            pdm.pro商品顏色idlist = new List<int>();
            int Key = 0;
            if (id == null)
                return NotFound();
            else
            {
                //productDetailID所找出的單一商品詳細資訊
                var productdetail = from product in _context.Products
                                    join prodetail in _context.ProductDetails on product.商品編號id equals prodetail.商品編號id
                                    join pro分類 in _context.ProductsTypeDetails on product.商品分類id equals pro分類.商品分類id
                                    join prophoto in _context.圖片位置s on prodetail.圖片位置id equals prophoto.圖片位置id
                                    join procolor in _context.ProductsColorDetails on prodetail.商品顏色id equals procolor.商品顏色id
                                    where product.商品編號id == id && prodetail.商品顏色id == colorID
                                    select new
                                    {
                                        //prodetailID = prodetail.Id,
                                        pro商品編號 = id,
                                        pro商品商品顏色ID = colorID,
                                        //pro商品尺寸ID = prodetail.商品尺寸id,
                                        pro商品金額 = product.商品價格,
                                        pro商品顏色 = procolor.商品顏色種類,
                                        //pro商品顏色圖片 = procolor.商品顏色圖片,
                                        pro商品分類 = pro分類.商品分類名稱,
                                        pro商品名稱 = product.商品名稱,
                                        pro商品介紹 = product.商品介紹,
                                        pro商品材質 = product.商品材質,
                                        pro商品圖片1 = prophoto.商品圖片1,
                                        pro商品圖片2 = prophoto.商品圖片2,
                                        pro商品圖片3 = prophoto.商品圖片3,
                                    };
                foreach (var item in productdetail.Distinct())
                {
                    Key = (int)item.pro商品編號;
                    pdm.pro商品編號 = (int)item.pro商品編號;
                    //pdm.商品尺寸ID =(int)item.pro商品尺寸ID;
                    pdm.商品顏色ID = (int)item.pro商品商品顏色ID;
                    //pdm.prodetailID = item.prodetailID;
                    pdm.pro商品顏色 = item.pro商品顏色;
                    pdm.pro商品金額 = item.pro商品金額.ToString();
                    //pdm.pro商品顏色圖片 = item.pro商品顏色圖片;
                    pdm.pro商品分類 = item.pro商品分類;
                    pdm.pro商品名稱 = item.pro商品名稱;
                    pdm.pro商品介紹 = item.pro商品介紹;
                    pdm.pro商品材質 = item.pro商品材質;
                    pdm.pro商品圖片1 = item.pro商品圖片1;
                    pdm.pro商品圖片2 = item.pro商品圖片2;
                    pdm.pro商品圖片3 = item.pro商品圖片3;
                }

                //取出商品的顏色及顏色圖片
                var totallist = from item in _context.Products
                                join prodetail in _context.ProductDetails on item.商品編號id equals prodetail.商品編號id
                                join procolor in _context.ProductsColorDetails on prodetail.商品顏色id equals procolor.商品顏色id
                                where item.商品編號id == Key
                                select new
                                {
                                    pro商品顏色圖片list = procolor.商品顏色圖片,
                                    pro商品顏色idlist = prodetail.商品顏色id,
                                    //pro商品DetailIDlist = prodetail.Id
                                };
                foreach (var CC in totallist)
                {

                    pdm.pro商品顏色圖片list.Add(CC.pro商品顏色圖片list);
                    //pdm.pro商品DetailIDlist.Add(CC.pro商品DetailIDlist);
                    pdm.pro商品顏色idlist.Add((int)(CC.pro商品顏色idlist));
                    //pdm.pro商品DetailIDlist = pdm.pro商品DetailIDlist.Distinct().ToList();
                    pdm.pro商品顏色圖片list = pdm.pro商品顏色圖片list.Distinct().ToList();
                    pdm.pro商品顏色idlist = pdm.pro商品顏色idlist.Distinct().ToList();
                }

                //取出此商品顏色有幾種size
                var listsize = from item in _context.Products
                               join prodetail in _context.ProductDetails on item.商品編號id equals prodetail.商品編號id
                               join prosize in _context.ProductsSizeDetails on prodetail.商品尺寸id equals prosize.商品尺寸id
                               join procolor in _context.ProductsColorDetails on prodetail.商品顏色id equals procolor.商品顏色id
                               where item.商品編號id == Key && procolor.商品顏色id == pdm.商品顏色ID
                               select new
                               {
                                   pro商品尺寸list = prosize.尺寸種類,
                                   pro商品尺寸idlist = prosize.商品尺寸id
                               };
                foreach (var SS in listsize)
                {
                    pdm.pro商品尺寸list.Add(SS.pro商品尺寸list);
                    pdm.pro商品尺寸list = pdm.pro商品尺寸list.Distinct().ToList();
                    pdm.pro商品尺寸idlist.Add(SS.pro商品尺寸idlist);
                    pdm.pro商品尺寸idlist = pdm.pro商品尺寸idlist.Distinct().ToList();
                    //pdm.pro商品DetailIDlist.Add(CC.pro商品DetailIDlist);

                    //pdm.pro商品DetailIDlist = pdm.pro商品DetailIDlist.Distinct().ToList();
                }


                return View(pdm);

            }

        }
        }
    }

