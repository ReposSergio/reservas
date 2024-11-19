namespace BookingReservationsAPI.Models
{
    public class Service
{
    public int Id { get; set; }
    
    // Inicializar la propiedad en el constructor
    public string Name { get; set; }

    // Constructor que asegura que 'Name' siempre tenga un valor
    public Service(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name)); // Garantiza que 'Name' no sea nulo
    }
}
}