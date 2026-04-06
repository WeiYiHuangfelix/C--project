using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    /// <summary>
    /// 購物車
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// 商品編號
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 儲存名稱
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 購物車單價
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 計算金額
        /// </summary>
        public decimal SubTotal
        {
            get { return UnitPrice * Quantity; }
        }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImagePath { get; set; }

        public CartItem(Product product, int quantity)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            UnitPrice = product.Price;
            Quantity = quantity;
            ImagePath = product.ImagePath;
        }
    }
}
