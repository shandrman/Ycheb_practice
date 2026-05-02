using Microsoft.EntityFrameworkCore;

namespace Ycheb_practice.DatabaseModel
{
    public class DatabaseContext : DbContext
    {
        // Таблицы БД
        public DbSet<Client> Clients { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<ClientSubscription> ClientSubscriptions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<ServiceOrderItem> ServiceOrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=FitnessManager.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Subtotal — вычисляемое свойство, не сохраняем в БД
            modelBuilder.Entity<ServiceOrderItem>()
                .Ignore(i => i.Subtotal);

            // Уникальный составной ключ для Attendance
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.ClientId, a.TrainingId, a.AttendanceDate })
                .IsUnique();

            // Уникальный логин
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Уникальное название роли
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            // Связь Attendance → User (marked_by) — без каскадного удаления
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.User)
                .WithMany(u => u.MarkedAttendances)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
