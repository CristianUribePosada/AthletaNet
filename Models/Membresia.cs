namespace AthletaNet.Models
{
    public class Membresia
    {
        public string Tipo { get; set; } = "Mensual"; // Mensual, Trimestral, Anual
        public System.DateTime FechaInicio { get; set; } = System.DateTime.Now;
        public System.DateTime FechaVencimiento { get; set; } = System.DateTime.Now.AddMonths(1);
        public bool Activa => System.DateTime.Now <= FechaVencimiento;
    }
}