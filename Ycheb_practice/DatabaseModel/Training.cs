namespace Ycheb_practice.DatabaseModel
{
    public class Training
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxParticipants { get; set; }
        public string Hall { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        // Внешний ключ → Employee (тренер)
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
