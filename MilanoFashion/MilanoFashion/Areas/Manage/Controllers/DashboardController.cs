using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MilanoFashion.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Superadmin,admin")]

    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
