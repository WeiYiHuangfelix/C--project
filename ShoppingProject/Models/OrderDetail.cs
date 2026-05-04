namespace ShoppingProject.Models
{
    /// <summary>
    /// 銷售明細資料表
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// 銷售明細編號
        /// </summary>
        public int OrderDetailId { get; set; }
        /// <summary>
        /// 銷售編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 銷售主檔資料表
        /// </summary>
        public Order Order { get; set; }
        /// <summary>
        /// 產品編號
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 產品資料表
        /// </summary>
        public Product Product { get; set; }
        /// <summary>
        /// 銷售明細數量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 銷售明細單價
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 銷售明細小計
        /// </summary>
        public decimal SubTotal {  get; set; }
    }
}
