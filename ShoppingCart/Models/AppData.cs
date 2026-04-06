using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    /// <summary>
    /// 模擬資料庫
    /// </summary>
    public static class AppData
    {
        /// <summary>
        /// 管理商品
        /// </summary>
        public static List<Product> AllProducts = new List<Product>();

        /// <summary>
        /// 管理購物車
        /// </summary>
        public static List<CartItem> ShoppingCart = new List<CartItem>();

        // 商品資料
        public static void InitializeData()
        {
            AllProducts.Clear();
            AllProducts.Add(new Product("P001", "Apple iPhone 13", 799.99m, 10, @"D:\Users\user\source\repos\ShoppingCart\ShoppingCart\Image\iphone13.jpg"));
            //AllProducts.Add(new Product("P002", "Samsung Galaxy S21", 699.99m, 15, "galaxy_s21.jpg"));
        }

        // 全部數量
        public static int TotalQuantity => ShoppingCart.Sum(item => item.Quantity);

        // 全部金額
        public static decimal TotalAmount => ShoppingCart.Sum(item => item.SubTotal);
    }
}
