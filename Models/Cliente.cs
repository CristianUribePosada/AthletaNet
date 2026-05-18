namespace AthletaNet.Models
{
    // Cliente hereda de Usuario todas sus propiedades básicas (Nombre, Username, Password)
    public class Cliente : Usuario
    {
        public Membresia MembresiaActual { get; set; } = new Membresia();

        // El taller pide poder ver su historial de turnos
        public DataStructures.ListaEnlazada<Turno> HistorialTurnos { get; set; } = new DataStructures.ListaEnlazada<Turno>();

        public Cliente() : base() { }

        public Cliente(string nombre, string username, string password)
            : base(nombre, username, password, "Cliente")
        {
        }
    }
}