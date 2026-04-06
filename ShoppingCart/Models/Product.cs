using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    /// <summary>
    /// 商品維護資料
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 商品編號
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 單價
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImagePath { get; set; }

      
        public Product(string id, string name, decimal price, int stock, string imagePath)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
            ImagePath = imagePath;
        }

        /// <summary>
        /// 覆寫 ToString 方便在除錯或簡單列表顯示時使用
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} - ${Price}";
        }
    }
}
