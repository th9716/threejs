using dbCompanyTest.Models;

namespace dbCompanyTest.ViewModels
{
    public class ProductDetailViewModels
    {
        public int pro商品編號 { get; set; }
        public int prodetailID { get; set; }
        public int 商品顏色ID { get; set; }
        public int 商品尺寸ID { get; set; }
        public string? pro商品名稱 { get; set; }
        public string? pro商品金額 { get; set; }
        public string? pro商品顏色 { get; set; }
        public string? pro商品顏色圖片 { get; set; }
        public string? pro商品分類 { get; set; }
        public string? pro商品材質 { get; set; }
        public string? pro商品介紹 { get; set; }
        public string? pro商品圖片1 { get; set; }
        public string? pro商品圖片2 { get; set; }
        public string? pro商品圖片3 { get; set; }

        public List<string>? pro商品顏色圖片list { get; set; }
        public List<string>? pro商品尺寸list { get; set; }
        public List<int>? pro商品DetailIDlist { get; set; }
        public List<int>? pro商品顏色idlist { get; set; }
        public List<int>? pro商品尺寸idlist { get; set; }

    }
}
