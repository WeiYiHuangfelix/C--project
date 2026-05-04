using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingProject.Data;
using ShoppingProject.Models; // 替換成你的 Models 命名空間
using ShoppingProject.ViewModels; // 替換成你的 ViewModels 命名空間
using System.Linq;

namespace ShoppingProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        //透過建構子注入資料庫
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound(); //如果找不到訂單編號，回傳404 Not Found
            }

            // 去資料庫把 主檔、客戶、明細、產品 全部一起撈出來
            var order = await _context.Orders
                .Include(o => o.Customer)                     // 關聯客戶資料表
                .Include(o => o.OrderDetails)                 // 關聯銷售明細資料表
                    .ThenInclude(od => od.Product)           // 關聯產品資料表
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if(order == null)
            {
                return NotFound(); //如果找不到訂單，回傳404 Not Found
            }

            // 撈出來的資料 (Model)，對應並塞入我們要傳給前端的 ViewModel
            var viewModel = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerName = order.Customer?.Name,
                TotalAmount = order.TotalAmount,

                // 使用 LINQ 的 Select 將 OrderDetails 轉換成 OrderDetailItem 集合
                Items = order.OrderDetails.Select(od => new OrderDetailItem
                {
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.Quantity * od.UnitPrice
                }).ToList()
            };
            // ViewModel 傳給 View，讓前端可以使用
            return View(viewModel);
            
        }
    }
}
