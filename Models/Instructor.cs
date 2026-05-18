namespace AthletaNet.Models
{
    // Instructor también hereda de Usuario
    public class Instructor : Usuario
    {
        // El taller dice que se pueden deshabilitar pero no borrar, por eso agregamos este flag
        public bool Activo { get; set; } = true;

        public Instructor() : base() { }

        public Instructor(string nombre, string username, string password)
            : base(nombre, username, password, "Instructor")
        {
        }
    }
}