using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AthletaNet.Services;

namespace AthletaNet.Pages
{
    public class LoginModel : PageModel
    {
        private readonly GymService _gymService;

        public LoginModel(GymService gymService)
        {
            _gymService = gymService;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMensaje { get; set; } = string.Empty;

        public void OnGet()
        {
            // Limpiar la sesión al entrar al Login (Logout automático)
            HttpContext.Session.Clear();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMensaje = "Por favor, llene todos los campos.";
                return Page();
            }

            // Validar las credenciales usando nuestro servicio y estructuras de datos
            var usuario = _gymService.ValidarLogin(Username, Password);

            if (usuario == null)
            {
                ErrorMensaje = "Usuario o contraseña incorrectos, o usuario deshabilitado.";
                return Page();
            }

            // Guardar el usuario en la sesión para saber quién ingresó
            HttpContext.Session.SetString("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("Username", usuario.Username);
            HttpContext.Session.SetString("Nombre", usuario.NombreCompleto);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            // Redireccionar al panel correspondiente según su Rol
            return RedirectToPage("/Index");
        }
    }
}