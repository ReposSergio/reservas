namespace BookingReservationsAPI.Models
{
    public class Service
    {
        public int Id { get; set; }

        // Inicializar la propiedad 'Name' en el constructor
        public string Name { get; set; }

        // Propiedad adicional para describir el servicio
        public string Description { get; set; }

        // Constructor que asegura que 'Name' siempre tenga un valor
        public Service(string name, string description = "")
        {
            Name = name ?? throw new ArgumentNullException(nameof(name)); // Garantiza que 'Name' no sea nulo
            Description = description; // Permite una descripción opcional
        }
    }
}