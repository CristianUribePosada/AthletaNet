namespace AthletaNet.Models
{
    public class Turno
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString().Substring(0, 6);
        public System.DateTime Fecha { get; set; } = System.DateTime.Today;
        public string ClienteUsername { get; set; } = string.Empty;
        public string? InstructorUsername { get; set; } // Puede ser nulo [cite: 14]
        public string? RutinaAsociada { get; set; } // Puede ser nulo [cite: 15]

        public Turno() { }

        public Turno(string clienteUsername, string? instructorUsername = null)
        {
            ClienteUsername = clienteUsername;
            InstructorUsername = instructorUsername;
            Fecha = System.DateTime.Today; // Solo se puede tener un turno por día 
        }
    }
}