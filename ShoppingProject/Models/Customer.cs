namespace ShoppingProject.Models
{
    /// <summary>
    /// 客戶資料表
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 客戶編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地區
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentMethods { get; set; }
    }
}
