using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManager.API.Controllers
{
    public class MaincopyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
