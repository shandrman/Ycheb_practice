namespace Ycheb_practice.DatabaseModel
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        // Внешний ключ → Role
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Attendance> MarkedAttendances { get; set; } = new List<Attendance>();
    }
}
