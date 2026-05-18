using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AthletaNet.Services;
using AthletaNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace AthletaNet.Pages
{
    public class ClienteDashboardModel : PageModel
    {
        private readonly GymService _gymService;

        public ClienteDashboardModel(GymService gymService)
        {
            _gymService = gymService;
        }

        // Datos del cliente logueado
        public Cliente? ClienteActual { get; private set; }
        public IEnumerable<Instructor> InstructoresDisponibles => _gymService.Instructores.ObtenerTodos().Where(i => i.Activo);
        public IEnumerable<Turno> MiHistorialTurnos => ClienteActual?.HistorialTurnos.ObtenerTodos() ?? Enumerable.Empty<Turno>();

        [BindProperty]
        public string? InstructorSeleccionado { get; set; }

        [TempData]
        public string MensajeTurno { get; set; } = string.Empty;
        [TempData]
        public string TipoMensaje { get; set; } = "success"; // success o danger

        public IActionResult OnGet()
        {
            // Seguridad: Validar que sea un Cliente
            var rol = HttpContext.Session.GetString("Rol");
            var username = HttpContext.Session.GetString("Username");

            if (rol != "Cliente" || string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Login");
            }

            // Buscar la información completa del cliente en nuestra lista enlazada
            ClienteActual = _gymService.Clientes.ObtenerTodos()
                .FirstOrDefault(c => c.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));

            if (ClienteActual == null)
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }

        public IActionResult OnPostSolicitarTurno()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("/Login");

            // Intentar agendar el turno usando la lógica del servicio web
            bool exito = _gymService.SolicitarTurno(username, InstructorSeleccionado);

            if (exito)
            {
                MensajeTurno = "¡Turno asignado con éxito! Ya estás en la fila de espera del día.";
                TipoMensaje = "success";
            }
            else
            {
                MensajeTurno = "No se pudo generar el turno. Recuerda que debes tener una membresía ACTIVA y solo puedes pedir UN turno por día.";
                TipoMensaje = "danger";
            }

            return RedirectToPage();
        }
    }
}