namespace ShoppingProject.Models
{
    /// <summary>
    /// 產品資料表
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 產品編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 產品名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 產品價格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
