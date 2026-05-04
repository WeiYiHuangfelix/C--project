namespace ShoppingProject.Models
{
    /// <summary>
    /// 銷售主檔
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 銷售主檔編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 銷售主檔日期
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 銷售主檔客戶編號
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// 銷售主檔客戶資料表
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// 銷售主檔總金額
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 銷售主檔對應的銷售明細資料表
        /// </summary>
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
