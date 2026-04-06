using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    //購物車控制器
    internal class CartController
    {
        //購物車清單
        public List<CartItem> GetCartItems()
        {
            return AppData.ShoppingCart;
        }

        //加入購物車
        public (bool Success, string Message) AddToCart(string productId, int quantity)
        {
            if(quantity <= 0) return (false, "加入失敗：數量必須大於零！");

            var product = AppData.AllProducts.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return (false, "加入失敗：找不到該商品！");
            }

            //檢查購物車中是否已經有該商品
            var existingItem = AppData.ShoppingCart.FirstOrDefault(i => i.ProductId == productId);
            int currentCartQty = existingItem != null ? existingItem.Quantity : 0;

            //購物車原數量 + 這次想加入的數量，不能超過總庫存
            if (currentCartQty + quantity > product.Stock)
            {
                return (false, $"加入失敗：庫存不足！目前庫存：{product.Stock}");
            }

            if(existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                AppData.ShoppingCart.Add(new CartItem(product, quantity));
            }
            return (true, "加入成功！");
        }

        //更正購物車
        public (bool Success, string Message) UpdateCart(string productId, int newQuantity)
        {
            if (newQuantity < 0) return (false, "更新失敗：數量不能小於零！");

            // 1. 尋找購物車中的該項商品 (這才是我們要修改對象)
            var cartItem = AppData.ShoppingCart.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null) return (false, "更新失敗：購物車中找不到該商品！");

            if (newQuantity == 0)
            {
                return RemoveFromCart(productId);
            }

            // 2. 檢查商品主檔的庫存 (確保修改後的數量沒有超過庫存)
            var product = AppData.AllProducts.FirstOrDefault(p => p.Id == productId);
            if (product != null && newQuantity > product.Stock)
            {
                return (false, $"更新失敗：庫存不足！目前庫存：{product.Stock}");
            }

            // 3. 修改的是購物車內「購買數量」，而不是商品主檔的庫存
            cartItem.Quantity = newQuantity;
            return (true, "更新成功！");
        }

        //刪除購物車
        public (bool Success, string Message) RemoveFromCart(string productId)
        {
            var item = AppData.ShoppingCart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                AppData.ShoppingCart.Remove(item);
                return (true, "商品已從購物車移除！");
            }
            return (false, "移除失敗：找不到該商品！");
        }

        // 在 CartController.cs 內
        public (bool Success, string Message) Checkout()
        {
            var cartItems = GetCartItems();
            if (cartItems.Count == 0) return (false, "購物車是空的！");

            // 🌟 核心邏輯：扣除商品庫存
            foreach (var item in cartItems)
            {
                // 從 AppData (或透過 ProductController) 找到那件商品
                var product = AppData.AllProducts.Find(p => p.Id == item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity; // 真正減掉庫存
                }
            }

            // 扣完庫存後，再清空購物車
            AppData.ShoppingCart.Clear();

            return (true, "結帳成功，庫存已自動扣除！");
        }

        //清空購物車
        public void ClearCart()
        {
            AppData.ShoppingCart.Clear();
        }

        public (int TotalItems, int TotalQuantity, decimal TotalAmount) GetCheckoutSummary()
        {
            int itemsCount = AppData.ShoppingCart.Count;            // 品項數 (有幾種商品)
            int totalQty = AppData.ShoppingCart.Sum(x => x.Quantity); // 總數量 (買了幾個)
            decimal totalAmt = AppData.ShoppingCart.Sum(x => x.SubTotal); // 總金額 (總共多少錢)

            return (itemsCount, totalQty, totalAmt);
        }
    }
}
