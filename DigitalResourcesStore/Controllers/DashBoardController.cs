using DigitalResourcesStore.Models.DashBoardDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Controllers
{
    [Route("dashboard")]
    [ApiController]
    public class DashBoardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashBoardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // Chỉ định rõ ràng phương thức Http để Swagger có thể nhận diện dễ dàng hơn
        [HttpGet("Admin/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            //string role = HttpContext.Session.GetString("Role");
            //if (role == "User" || role == null)
            //{
            //    return RedirectToAction("AccessDenied", "Home");
            //}
                var viewModel = await _dashboardService.GetDashboardDataAsync();
                return Ok(viewModel);
           
        }
    }
}
