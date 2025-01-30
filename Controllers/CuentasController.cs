using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProyectoIdentity.Dtos;
using ProyectoIdentity.Models;
using ProyectoIdentity.Models.ViewModels;
using ProyectoIdentity.Servicios;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace ProyectoIdentity.Controllers
{
    public class CuentasController : Controller
    {
        //Inyectando las dependencias necesarias
        private readonly IMessage _message;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IMessage message)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _message = message;
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
        public async Task<IActionResult> Registro(RegistroViewModel registroVM,SendEmailRequest request, string returnurl = null)
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
                    //Implementación de confirmación de Email en el registro
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                    var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas",
                    new
                    {
                        userId = usuario.Id,
                        code = code
                    },
                    protocol: HttpContext.Request.Scheme);

                    //Contruyecdo email para confirmación
                    request.Subject = "Confirmar su cuenta - Proyecto Identity";
                    request.Body = $@"
                    <p>Porfavor confirme su cuenta dando click en el siguiente enlace:</p>
                    <a href={urlRetorno}>Confirmar Email</a>";
                    request.To = registroVM.Email;
                    await _message.SendEmail(request.Subject, request.Body, request.To);

                    //Registrando usuario
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
                var resultado = await _signInManager.PasswordSignInAsync(accesoVM.Email,accesoVM.Password,accesoVM.RememberMe, lockoutOnFailure: true);

                if (resultado.Succeeded)
                {
                    return LocalRedirect(returnurl);
                }
                if (resultado.IsLockedOut)
                {
                    return View("Bloqueado");
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
            return LocalRedirect("~/Cuentas/Acceso");
        }

        //Método para olvido de contraseña
        [HttpGet]
        public IActionResult OlvidoPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OlvidoPassword(SendEmailRequest request, OlvidoPasswordViewModel pro)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(pro.Email);

                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionOlvidoPassword");
                }
                var codigo = await _userManager.GeneratePasswordResetTokenAsync(usuario);

                var urlRetorno = Url.Action("ResetPassword", "Cuentas", 
                    new {
                        userId = usuario.Id,
                        code = codigo
                    },
                    protocol: HttpContext.Request.Scheme
                );

                request.Subject = "Recuperar contraseña - Proyecto Identity";
                request.Body = $@"
                    <h1>Restablecer contraseña</h1>
                    <p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p>
                    <a href={urlRetorno}>Restablecer contraseña</a>
                    <p><em>Este enlace expirará en 5 minutos</em></p>";
                request.To = pro.Email;
                await _message.SendEmail(request.Subject, request.Body,request.To);
                return RedirectToAction("ConfirmacionOlvidoPassword");
            }
            return View(pro);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionOlvidoPassword()
        {
            return View();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code=null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(RecuperarPasswordViewModel repViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(repViewModel.Email);

                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }


                var resultado = await _userManager.ResetPasswordAsync(usuario,repViewModel.Code,repViewModel.Password);
                if (resultado.Succeeded) 
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }
                
                ValidarErrores(resultado);
            }
            return View(repViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionRecuperaPassword()
        {
            return View();
        }
        //Método para confirmación en el registro
        [HttpGet]
        public async Task<IActionResult> ConfirmarEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var usuario = await _userManager.FindByIdAsync(userId);

            if (usuario == null)
            {
                return View("Error");
            }

            //Confirmando el usuario en la BD
            var resultado = await _userManager.ConfirmEmailAsync(usuario,code);
            return View(resultado.Succeeded ? "ConfirmarEmail" : "Error");
        }

        //Configuración de acceso externo: facebook, google, etc.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult AccesoExterno(string proveedor, string returnurl = null)
        {
            var urlRedireccion = Url.Action("AccesoExternoCallback", "Cuentas", new { ReturnUrl = returnurl });
            var propiedades = _signInManager.ConfigureExternalAuthenticationProperties(proveedor,urlRedireccion);

            return Challenge(propiedades, proveedor);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AccesoExternoCallback(string returnurl = null, string error = null)
        {
            returnurl = returnurl ?? Url.Content("~/");
            if (error != null) 
            {
                ModelState.AddModelError(string.Empty, $"Error en el acceso externo {error}");
                return View(nameof(Acceso));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) 
            {
                return RedirectToAction(nameof(Acceso));
            }

            //Acceder con el usuario en el proveedor externo
            var resultado = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey, isPersistent: false);

            if (resultado.Succeeded)
            {
                //Actualizar los tokens de acceso
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnurl);
            }
            else
            {
                //Si el usuario no tiene cuenta pregunta si quiere crear una
                ViewData["ReturnUrl"] = returnurl;
                ViewData["NombreAMostrarProveedor"] = info.ProviderDisplayName;

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var nombre = info.Principal.FindFirstValue(ClaimTypes.Name);
                return View("ConfirmacionAccesoExterno", 
                    new ConfirmacionAccesoExternoViewModel
                    {
                        Email = email,
                        Name = nombre
                    }
                );
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmacionAccesoExterno(ConfirmacionAccesoExternoViewModel caeViewModel, string returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                //Obtener la información del usuario del proveedor externo
                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null)
                {
                    return View("Error");
                }

                var usuario = new AppUsuario { UserName = caeViewModel.Email, Email = caeViewModel.Email, Nombre = caeViewModel.Name};
                var resultado = await _userManager.CreateAsync(usuario);

                if (resultado.Succeeded)
                {
                    resultado = await _userManager.AddLoginAsync(usuario,info);
                    if (resultado.Succeeded)
                    {
                        await _signInManager.SignInAsync(usuario,isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                        return LocalRedirect(returnurl);
                    }
                }
                ValidarErrores(resultado);
            }
            ViewData["ReturnUrl"] = returnurl;
            return View(caeViewModel);
        }


    }
}
