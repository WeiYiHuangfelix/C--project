namespace ShoppingProject.ViewModels
{
    /// <summary>
    /// 銷售主檔與銷售明細表的ViewModel
    /// </summary>
    public class OrderDetailsViewModel
    {
        /// <summary>
        /// 銷售主檔與銷售明細編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 銷售主檔與銷售明細日期
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 銷售主檔與銷售明細客戶名稱
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 銷售主檔與銷售明細總金額
        /// </summary>
        public decimal TotalAmount { get; set; }
        public List<OrderDetailItem> Items { get; set; } = new List<OrderDetailItem>();
    }

    public class OrderDetailItem
    {
        /// <summary>
        /// 銷售主檔與銷售明細產品名稱
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 銷售主檔與銷售明細書量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 銷售主檔與銷售明細單價
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 銷售主檔與銷售明細小計
        /// </summary>
        public decimal SubTotal { get; set; }
    }
}
