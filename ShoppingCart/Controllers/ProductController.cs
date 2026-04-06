using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    /// <summary>
    /// 商品維護控制器
    /// </summary>
    public class ProductController
    {
        // 商品清單
        public List<Product> GetAllProducts()
        {
            return AppData.AllProducts;
        }

        // 新增商品
        public (bool Success, string Message) AddProduct(Product addProduct)
        {
            // 防呆檢查：編號不能重複
            if (AppData.AllProducts.Any(p => p.Id == addProduct.Id))
            {
                return (false, "新增失敗：商品編號已存在！");
            }
            // 防呆檢查：價格與庫存不能為負數
            if (addProduct.Price < 0 || addProduct.Stock < 0)
            {
                return (false, "新增失敗：價格與庫存不能小於零！");
            }

            AppData.AllProducts.Add(addProduct);
            return (true, "商品新增成功！");
        }

        //更新商品
        public (bool Success, string Message) UpdateProduct(Product upDataProduct)
        {
            var product = AppData.AllProducts.FirstOrDefault(p => p.Id == upDataProduct.Id);
            if (product == null)
            {
                return (false, "更新失敗：找不到該商品！");
            }

            product.Name = upDataProduct.Name;
            product.Price = upDataProduct.Price;
            product.Stock = upDataProduct.Stock;
            product.ImagePath = upDataProduct.ImagePath;

            return (true, "商品更新成功！");
        }

        //刪除商品
        public (bool Success, string Message) DeleteProduct(string productId)
        {
            var product = AppData.AllProducts.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return (false, "刪除失敗：找不到該商品！");
            }

            if (AppData.ShoppingCart.Any(c => c.ProductId == productId))
            {
                return (false, "刪除失敗：該商品目前有客人在購物車中，無法刪除！");
            }

            AppData.AllProducts.Remove(product);
            return (true, "商品刪除成功！");
        }

        //查詢商品
        public List<Product> SearchProducts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAllProducts();
            }

            return AppData.AllProducts.Where(p => p.Name.Contains(keyword)).ToList();
        }

        //排序商品
        public List<Product> SortProducts(string sortBy, bool isAscending = true)
        {
            IEnumerable<Product> query = AppData.AllProducts;

            switch (sortBy.ToLower())
            {
                case "price":
                    query = isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                    break;
                case "stock":
                    query = isAscending ? query.OrderBy(p => p.Stock) : query.OrderByDescending(p => p.Stock);
                    break;
                case "id":
                default:
                    query = isAscending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id);
                    break;
            }
            return query.ToList();
        }
    }
}
