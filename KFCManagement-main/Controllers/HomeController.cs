﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCManagement.Models;
using System.Diagnostics;

namespace KFCManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly RestaurantManagementContext _context;
        private readonly ILogger<HomeController> _logger;

        // Constructor
        public HomeController(RestaurantManagementContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Action method để xử lý trang chủ
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách món ăn từ cơ sở dữ liệu
            var menuItems = await _context.MenuItems.ToListAsync();

            // Lấy danh sách bàn trống từ cơ sở dữ liệu
            var availableTables = await _context.Tables.Where(t => t.IsAvailable==true).ToListAsync();

            // Truyền dữ liệu vào View thông qua ViewBag
            ViewBag.MenuItems = menuItems;
            ViewBag.AvailableTables = availableTables;

            return View();
        }

        public async Task<IActionResult> Menu()
        {
            var menuItems = await _context.MenuItems.ToListAsync();

            // Lấy danh sách bàn trống từ cơ sở dữ liệu
            

            // Truyền dữ liệu vào View thông qua ViewBag
            ViewBag.MenuItems = menuItems;
            return View();
        }
        public IActionResult Service()
        {
            return View();
        }
        public IActionResult Restaurant()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
