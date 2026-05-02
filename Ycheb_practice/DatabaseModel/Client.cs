namespace Ycheb_practice.DatabaseModel
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateOnly? RegistrationDate { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }

        // Навигационные свойства
        public ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    }
}
