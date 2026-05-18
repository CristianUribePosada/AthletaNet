using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AthletaNet.Services;

namespace AthletaNet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly GymService _gymService;

        public IndexModel(GymService gymService)
        {
            _gymService = gymService;
        }

        // Propiedades para exponer los datos a la vista HTML
        public string NombreUsuario { get; set; } = string.Empty;
        public string RolUsuario { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Validar si el usuario pasó por el Login leyendo la sesión
            NombreUsuario = HttpContext.Session.GetString("Nombre") ?? string.Empty;
            RolUsuario = HttpContext.Session.GetString("Rol") ?? string.Empty;

            // Si nadie ha iniciado sesión, lo patitas a la calle (al Login)
            if (string.IsNullOrEmpty(RolUsuario))
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }
    }
}