using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AthletaNet.Services;
using AthletaNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace AthletaNet.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly GymService _gymService;

        public AdminDashboardModel(GymService gymService)
        {
            _gymService = gymService;
        }

        // Propiedades para listar en la interfaz
        public IEnumerable<Cliente> ListaClientes => _gymService.Clientes.ObtenerTodos();
        public IEnumerable<Instructor> ListaInstructores => _gymService.Instructores.ObtenerTodos();
        public IEnumerable<Turno> ListaTurnosGlobales => _gymService.TurnosDelDia.ObtenerTodos();

        // Propiedades vinculadas a los formularios de creación
        [BindProperty]
        public string NuevoNombre { get; set; } = string.Empty;
        [BindProperty]
        public string NuevoUsername { get; set; } = string.Empty;
        [BindProperty]
        public string NuevoPassword { get; set; } = string.Empty;
        [BindProperty]
        public string TipoMembresia { get; set; } = "Mensual";

        [TempData]
        public string MensajeExito { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Seguridad: Solo el Administrador puede ver esta página
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        // Handler para Registrar un Cliente
        public IActionResult OnPostCrearCliente()
        {
            if (string.IsNullOrEmpty(NuevoUsername) || string.IsNullOrEmpty(NuevoPassword)) return Page();

            Cliente nuevo = new Cliente(NuevoNombre, NuevoUsername, NuevoPassword);
            nuevo.MembresiaActual.Tipo = TipoMembresia;

            // Si elige una membresía más larga, le extendemos la fecha de vencimiento
            if (TipoMembresia == "Trimestral") nuevo.MembresiaActual.FechaVencimiento = System.DateTime.Now.AddMonths(3);
            if (TipoMembresia == "Anual") nuevo.MembresiaActual.FechaVencimiento = System.DateTime.Now.AddYears(1);

            _gymService.RegistrarCliente(nuevo);
            MensajeExito = "Cliente registrado correctamente.";

            return RedirectToPage();
        }

        // Handler para Registrar un Instructor
        public IActionResult OnPostCrearInstructor()
        {
            if (string.IsNullOrEmpty(NuevoUsername) || string.IsNullOrEmpty(NuevoPassword)) return Page();

            Instructor nuevo = new Instructor(NuevoNombre, NuevoUsername, NuevoPassword);
            _gymService.RegistrarInstructor(nuevo);
            MensajeExito = "Instructor registrado correctamente.";

            return RedirectToPage();
        }

        // Handler para Eliminar un Cliente
        public IActionResult OnPostEliminarCliente(string username)
        {
            _gymService.EliminarCliente(username);
            MensajeExito = "Cliente eliminado del sistema.";
            return RedirectToPage();
        }

        // Handler para Deshabilitar/Habilitar un Instructor (Requisito: No se borran)
        public IActionResult OnPostAlternarInstructor(string username, bool estadoActual)
        {
            _gymService.CambiarEstadoInstructor(username, !estadoActual);
            MensajeExito = "Estado del instructor actualizado.";
            return RedirectToPage();
        }
    }
}