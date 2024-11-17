using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.ViewModels;

namespace ProyectoIdentity.Controllers
{
    public class CuentasController : Controller
    {
        // GET: CuentasController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            RegistroViewModel registroVM = new RegistroViewModel();

            return View(registroVM);
        }   

    }
}
