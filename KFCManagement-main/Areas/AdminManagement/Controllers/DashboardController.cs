using KFCManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KFCManagement.Areas.AdminManagement.Controllers {

    public class DashboardController : BaseController
    {
        private readonly RestaurantManagementContext _context;

        public DashboardController(RestaurantManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }

}
