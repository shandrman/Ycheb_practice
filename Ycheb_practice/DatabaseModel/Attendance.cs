namespace Ycheb_practice.DatabaseModel
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public bool? IsPresent { get; set; }
        public DateTime? MarkTime { get; set; }

        // Внешний ключ → Client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        // Внешний ключ → Training
        public int TrainingId { get; set; }
        public Training Training { get; set; } = null!;

        // Внешний ключ → User (кто отметил, nullable)
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
