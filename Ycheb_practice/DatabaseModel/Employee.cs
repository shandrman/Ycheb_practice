namespace Ycheb_practice.DatabaseModel
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateOnly HireDate { get; set; }
        public string? Specialization { get; set; }
        public bool? IsActive { get; set; }

        // Внешний ключ → User (nullable — сотрудник может не иметь аккаунта)
        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Training> Trainings { get; set; } = new List<Training>();
        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    }
}
