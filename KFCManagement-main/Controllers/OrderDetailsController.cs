using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KFCManagement.Models;
using Microsoft.AspNetCore.Http;

namespace KFCManagement.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly RestaurantManagementContext _context;

        public OrderDetailsController(RestaurantManagementContext context)
        {
            _context = context;
        }

        public IActionResult AddToCart(int itemId, int quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            // Check if CustomerId exists in session
            if (customerId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để sử dụng chức năng này.";
                return RedirectToAction("Index", "LoginC");
            }

            if (quantity <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            if (userId == 0)
            {
                return RedirectToAction("Index", "LoginC");
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.Orders
                                    .FirstOrDefault(o => o.CustomerId == userId && o.Status == "pending");

                if (order == null)
                {
                    order = new Order
                    {
                        CustomerId = userId,
                        Status = "pending",
                        OrderDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                }

                var menuItem = _context.MenuItems.AsNoTracking().FirstOrDefault(m => m.ItemId == itemId);
                if (menuItem == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var existingItem = _context.OrderDetails
                                           .FirstOrDefault(od => od.OrderId == order.OrderId && od.ItemId == itemId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.UpdatedAt = DateTime.Now;
                }
                else
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ItemId = menuItem.ItemId,
                        Quantity = quantity,
                        Price = menuItem.Price,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("Index", "OrderDetails");
        }
    
        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("CustomerId") ?? 0; // Giả sử bạn đang sử dụng tên người dùng (hoặc user ID)

            if (userId == 0)
            {
                // Trường hợp không có CustomerId trong Session, yêu cầu người dùng đăng nhập
                return RedirectToAction("Index", "LoginC");
            }

            // Lấy giỏ hàng của người dùng từ cơ sở dữ liệu
            var order = _context.Orders
                                .FirstOrDefault(o => o.CustomerId == userId && o.Status == "pending");

            if (order == null)
            {
                return View(new List<OrderDetail>()); // Nếu không có giỏ hàng, trả về danh sách rỗng
            }

            var cartItems = _context.OrderDetails
                         .Include(od => od.Item)  // Đảm bảo bao gồm MenuItem (được liên kết với OrderDetail)
                         .Where(od => od.OrderId == order.OrderId)
                         .ToList();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int orderDetailId)
        {
            var orderDetail = await _context.OrderDetails
                                             .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);

            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng." });
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
        }

        public async Task<IActionResult> Checkout()
        {
            // Lấy CustomerId từ Session
            var userId = HttpContext.Session.GetInt32("CustomerId") ?? 0;

            if (userId == 0)
            {
                // Trường hợp không có CustomerId trong Session, yêu cầu người dùng đăng nhập
                return RedirectToAction("Index", "LoginC");
            }

            // Lấy đơn hàng của người dùng có trạng thái "pending"
            var order = _context.Orders
                .FirstOrDefault(o => o.CustomerId == userId && o.Status == "pending");

            // Kiểm tra nếu đơn hàng tồn tại
            if (order != null)
            {
                // Cập nhật trạng thái đơn hàng
                order.Status = "completed";  // Thay đổi trạng thái thành "completed"

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                // Chuyển hướng đến trang khác (hoặc trở về trang giỏ hàng, trang chi tiết đơn hàng, v.v.)
                return RedirectToAction("OrderConfirmation", "OrderDetails");  // Bạn có thể điều chỉnh tên Action tùy vào yêu cầu của bạn
            }
            else
            {
                // Trường hợp không tìm thấy đơn hàng, có thể redirect về trang lỗi hoặc giỏ hàng
                return RedirectToAction("Index", "OrderDetails");
            }
        }

        public IActionResult OrderConfirmation()
        {
            // Lấy CustomerId từ Session
            var userId = HttpContext.Session.GetInt32("CustomerId") ?? 0;

            if (userId == 0)
            {
                // Trường hợp không có CustomerId trong Session, yêu cầu người dùng đăng nhập
                return RedirectToAction("Index", "LoginC");
            }

            // Lấy đơn hàng đã hoàn thành
            var order = _context.Orders
                .FirstOrDefault(o => o.CustomerId == userId && o.Status == "completed");

            if (order != null)
            {
                return View(order);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailId == id);
        }
    }
}
