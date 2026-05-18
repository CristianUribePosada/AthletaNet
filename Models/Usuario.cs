using System.Numerics;

namespace AthletaNet.Models
{
    public class Usuario
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString().Substring(0, 8);
        public string NombreCompleto { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; // "Administrador", "Instructor", "Cliente"

        public Usuario() { }

        public Usuario(string nombre, string username, string password, string rol)
        {
            NombreCompleto = nombre;
            Username = username;
            Password = password;
            Rol = rol;
        }
    }
}