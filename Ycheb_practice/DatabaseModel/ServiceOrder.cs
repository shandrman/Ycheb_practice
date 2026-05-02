namespace Ycheb_practice.DatabaseModel
{
    public class ServiceOrder
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }

        // Внешний ключ → Client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        // Внешний ключ → Employee (nullable)
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public ICollection<ServiceOrderItem> ServiceOrderItems { get; set; } = new List<ServiceOrderItem>();
    }
}
