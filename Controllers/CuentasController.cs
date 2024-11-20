using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProyectoIdentity.Models;
using ProyectoIdentity.Models.ViewModels;

namespace ProyectoIdentity.Controllers
{
    public class CuentasController : Controller
    {
        //Inyectando las dependencias necesarias
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET: CuentasController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Registro(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            RegistroViewModel registroVM = new RegistroViewModel();
            return View(registroVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Para los ataques XSS
        public async Task<IActionResult> Registro(RegistroViewModel registroVM, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var usuario = new AppUsuario{
                    UserName = registroVM.Email,
                    Email = registroVM.Email,
                    Nombre = registroVM.Nombre,
                    Url = registroVM.Url,
                    CodigoPais = registroVM.CodigoPais,
                    Telefono = registroVM.Telefono,
                    Pais = registroVM.Pais,
                    Ciudad = registroVM.Ciudad,
                    Direccion = registroVM.Direccion,
                    FechaNacimiento = registroVM.FechaNacimiento,
                    Estado = registroVM.Estado
                };
                // Creando en usuario
                var resultado = await _userManager.CreateAsync(usuario, registroVM.Password);

                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(usuario, isPersistent: false);

                    return LocalRedirect(returnurl);
                }

                ValidarErrores(resultado);
            }
            return View(registroVM);
        }

        //Manejador de errores.
        private void ValidarErrores(IdentityResult resultado){
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }

        //Método para mostrar formulario de acceso
        [HttpGet]
        public IActionResult Acceso(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;    
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Para los ataques XSS
        public async Task<IActionResult> Acceso(AccesoViewModel accesoVM, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;  
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                
                // Creando en usuario
                var resultado = await _signInManager.PasswordSignInAsync(accesoVM.Email,accesoVM.Password,accesoVM.RememberMe, lockoutOnFailure: false);

                if (resultado.Succeeded)
                {
                    //return RedirectToAction("Index", "Home");
                    return LocalRedirect(returnurl);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Acceso inválido.");
                    return View(accesoVM);
                }

            }
            return View(accesoVM);
        }

        //Cerrar sesión de la aplicación
        [HttpPost]
        [ValidateAntiForgeryToken] //Para los ataques XSS
        public async Task<IActionResult> SalirAplicacion()
        {
            //Destruyendo las cookies del navegador
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}
