using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AthletaNet.Services;
using AthletaNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace AthletaNet.Pages
{
    public class InstructorDashboardModel : PageModel
    {
        private readonly GymService _gymService;

        public InstructorDashboardModel(GymService gymService)
        {
            _gymService = gymService;
        }

        public string UsernameInstructor { get; set; } = string.Empty;

        // Mostrar los turnos en la cola global que eligieron a este instructor o entrenamiento libre
        public IEnumerable<Turno> MisTurnosAsignados => _gymService.TurnosDelDia.ObtenerTodos()
            .Where(t => string.IsNullOrEmpty(t.InstructorUsername) || t.InstructorUsername.Equals(UsernameInstructor, System.StringComparison.OrdinalIgnoreCase));

        [BindProperty]
        public string TurnoIdAtendiendo { get; set; } = string.Empty;

        [BindProperty]
        public string RedaccionRutina { get; set; } = string.Empty;

        [TempData]
        public string MensajeInstructor { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Seguridad: Validar que sea un Instructor
            var rol = HttpContext.Session.GetString("Rol");
            UsernameInstructor = HttpContext.Session.GetString("Username") ?? string.Empty;

            if (rol != "Instructor" || string.IsNullOrEmpty(UsernameInstructor))
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }

        // Handler para despachar al cliente al principio de la cola y asignarle su rutina
        public IActionResult OnPostAsignarRutina()
        {
            if (string.IsNullOrEmpty(TurnoIdAtendiendo) || string.IsNullOrEmpty(RedaccionRutina))
            {
                return RedirectToPage();
            }

            // 1. Buscamos el turno en la cola para aplicarle la rutina
            var turnoActual = _gymService.TurnosDelDia.ObtenerTodos().FirstOrDefault(t => t.Id == TurnoIdAtendiendo);

            if (turnoActual != null)
            {
                // Asignar la rutina al turno
                turnoActual.RutinaAsociada = RedaccionRutina;

                // También actualizarla en el historial del cliente para que le quede guardada
                var cliente = _gymService.Clientes.ObtenerTodos()
                    .FirstOrDefault(c => c.Username.Equals(turnoActual.ClienteUsername, System.StringComparison.OrdinalIgnoreCase));

                if (cliente != null)
                {
                    var turnoHistorial = cliente.HistorialTurnos.ObtenerTodos().FirstOrDefault(t => t.Id == TurnoIdAtendiendo);
                    if (turnoHistorial != null) turnoHistorial.RutinaAsociada = RedaccionRutina;
                }

                // 2. Aplicamos FIFO: Desencolamos el primer elemento de la fila diaria
                _gymService.TurnosDelDia.Desencolar();

                // Forzar el guardado en los archivos planos JSON con los cambios de la cola
                // Usamos un truco limpio: invocamos indirectamente la actualización registrando un cambio vacío
                _gymService.CambiarEstadoInstructor(HttpContext.Session.GetString("Username")!, true);

                MensajeInstructor = $"📋 Rutina asignada con éxito al atleta {turnoActual.ClienteUsername}. ¡Siguiente en la fila!";
            }

            return RedirectToPage();
        }
    }
}