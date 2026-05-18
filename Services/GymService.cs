using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using AthletaNet.DataStructures;
using AthletaNet.Models;

namespace AthletaNet.Services
{
    public class GymService
    {
        // Rutas físicas de los archivos planos en la carpeta Data
        private readonly string _folderPath;
        private readonly string _clientesFile;
        private readonly string _instructoresFile;
        private readonly string _turnosFile;

        // Nuestras Estructuras de Datos personalizadas en memoria RAM
        public ListaEnlazada<Cliente> Clientes { get; private set; } = new ListaEnlazada<Cliente>();
        public ListaEnlazada<Instructor> Instructores { get; private set; } = new ListaEnlazada<Instructor>();
        public Cola<Turno> TurnosDelDia { get; private set; } = new Cola<Turno>();

        // Credenciales fijas del Administrador único solicitado por el taller
        public readonly Usuario Administrador = new Usuario("Administrador General", "admin", "admin123", "Administrador");

        public GymService()
        {
            // Apuntar a la carpeta Data creada en la raíz del proyecto
            _folderPath = Path.Combine(AppContext.BaseDirectory, "Data");
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            _clientesFile = Path.Combine(_folderPath, "clientes.json");
            _instructoresFile = Path.Combine(_folderPath, "instructores.json");
            _turnosFile = Path.Combine(_folderPath, "turnos.json");

            // Cargar los archivos guardados al inicializar la aplicación
            CargarDatosDesdeArchivos();
        }

        // ==========================================
        // LÓGICA DE AUTENTICACIÓN (LOGIN)
        // ==========================================
        public Usuario? ValidarLogin(string username, string password)
        {
            // 1. Validar si es el administrador único
            if (Administrador.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && Administrador.Password == password)
            {
                return Administrador;
            }

            // 2. Buscar en la lista enlazada de clientes
            foreach (var cliente in Clientes.ObtenerTodos())
            {
                if (cliente.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && cliente.Password == password)
                {
                    return cliente;
                }
            }

            // 3. Buscar en la lista enlazada de instructores (siempre que esté activo)
            foreach (var instructor in Instructores.ObtenerTodos())
            {
                if (instructor.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && instructor.Password == password && instructor.Activo)
                {
                    return instructor;
                }
            }

            return null; // Credenciales inválidas
        }

        // ==========================================
        // LÓGICA DEL ADMINISTRADOR (CRUD)
        // ==========================================
        public void RegistrarCliente(Cliente nuevoCliente)
        {
            Clientes.Agregar(nuevoCliente);
            GuardarDatosEnArchivos();
        }

        public void EliminarCliente(string username)
        {
            // Al eliminar un cliente se borra toda su actividad (membresía, turnos, etc) según el taller
            Clientes.Eliminar(c => c.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            // Re-filtrar la cola de turnos para remover los turnos que pertenecían a ese cliente
            var turnosRestantes = TurnosDelDia.ObtenerTodos().Where(t => !t.ClienteUsername.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();
            TurnosDelDia = new Cola<Turno>();
            foreach (var t in turnosRestantes)
            {
                TurnosDelDia.Encolar(t);
            }

            GuardarDatosEnArchivos();
        }

        public void RegistrarInstructor(Instructor nuevoInstructor)
        {
            Instructores.Agregar(nuevoInstructor);
            GuardarDatosEnArchivos();
        }

        public void CambiarEstadoInstructor(string username, bool activo)
        {
            // Los instructores solo se pueden deshabilitar o actualizar, NO borrar
            foreach (var inst in Instructores.ObtenerTodos())
            {
                if (inst.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    inst.Activo = activo;
                    break;
                }
            }
            GuardarDatosEnArchivos();
        }

        // ==========================================
        // LÓGICA DE CLIENTES (GESTIÓN DE TURNOS)
        // ==========================================
        public bool SolicitarTurno(string clienteUsername, string? instructorUsername = null)
        {
            // Buscar al cliente para validar membresía activa
            Cliente? cliente = Clientes.ObtenerTodos().FirstOrDefault(c => c.Username.Equals(clienteUsername, StringComparison.OrdinalIgnoreCase));

            if (cliente == null || !cliente.MembresiaActual.Activa)
                return false; // No tiene membresía activa para ingresar

            // El taller exige: "solo se puede tener un turno por día"
            bool yaTieneTurno = TurnosDelDia.ObtenerTodos().Any(t => t.ClienteUsername.Equals(clienteUsername, StringComparison.OrdinalIgnoreCase)) ||
                               cliente.HistorialTurnos.ObtenerTodos().Any(t => t.Fecha.Date == DateTime.Today);

            if (yaTieneTurno)
                return false;

            // Generar el turno
            Turno nuevoTurno = new Turno(clienteUsername, instructorUsername);

            // Si el turno tiene asociado un instructor, el instructor puede ver sus clientes
            TurnosDelDia.Encolar(nuevoTurno);
            cliente.HistorialTurnos.Agregar(nuevoTurno);

            GuardarDatosEnArchivos();
            return true;
        }

        // ==========================================
        // PERSISTENCIA DE DATOS (Manejo de Archivos Locales)
        // ==========================================
        private void GuardarDatosEnArchivos()
        {
            // Convertimos nuestras listas enlazadas y colas a listas serializables para guardarlas en formato JSON plano (.txt/.json)
            var options = new JsonSerializerOptions { WriteIndented = true };

            string clientesJson = JsonSerializer.Serialize(Clientes.ObtenerTodos().ToList(), options);
            string instructoresJson = JsonSerializer.Serialize(Instructores.ObtenerTodos().ToList(), options);
            string turnosJson = JsonSerializer.Serialize(TurnosDelDia.ObtenerTodos().ToList(), options);

            File.WriteAllText(_clientesFile, clientesJson);
            File.WriteAllText(_instructoresFile, instructoresJson);
            File.WriteAllText(_turnosFile, turnosJson);
        }

        private void CargarDatosDesdeArchivos()
        {
            if (File.Exists(_clientesFile))
            {
                string json = File.ReadAllText(_clientesFile);
                var lista = JsonSerializer.Deserialize<System.Collections.Generic.List<Cliente>>(json);
                if (lista != null)
                {
                    Clientes.Limpiar();
                    foreach (var item in lista) Clientes.Agregar(item);
                }
            }

            if (File.Exists(_instructoresFile))
            {
                string json = File.ReadAllText(_instructoresFile);
                var lista = JsonSerializer.Deserialize<System.Collections.Generic.List<Instructor>>(json);
                if (lista != null)
                {
                    Instructores.Limpiar();
                    foreach (var item in lista) Instructores.Agregar(item);
                }
            }

            if (File.Exists(_turnosFile))
            {
                string json = File.ReadAllText(_turnosFile);
                var lista = JsonSerializer.Deserialize<System.Collections.Generic.List<Turno>>(json);
                if (lista != null)
                {
                    TurnosDelDia = new Cola<Turno>();
                    foreach (var item in lista) TurnosDelDia.Encolar(item);
                }
            }
        }
    }
}