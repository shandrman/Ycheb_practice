using System.Windows;
using Ycheb_practice.DatabaseModel;

namespace Ycheb_practice
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (DatabaseContext db = new DatabaseContext())
            {
                // Создаёт БД если не существует.
                // Возвращает true — если БД только что создана (первый запуск).
                if (db.Database.EnsureCreated())
                {
                    SeedDefaultData(db);
                }

                // Загружаем данные клиентов в DataGrid
                dgClients.ItemsSource = db.Clients.ToList();
                tbStatus.Text = $"Загружено клиентов: {db.Clients.Count()}  |  " +
                                $"База данных: FitnessManager.db";
            }
        }

        /// <summary>
        /// Заполняет БД начальными тестовыми данными при первом запуске.
        /// </summary>
        private void SeedDefaultData(DatabaseContext db)
        {
            // Роли
            var roleAdmin   = new Role { RoleName = "Администратор", Description = "Полный доступ" };
            var roleTrainer = new Role { RoleName = "Тренер",        Description = "Доступ к тренировкам и посещаемости" };
            db.Roles.Add(roleAdmin);
            db.Roles.Add(roleTrainer);
            db.SaveChanges();

            // Пользователи
            var userAdmin = new User
            {
                Login        = "admin",
                PasswordHash = "admin123",
                FullName     = "Администратор Системы",
                RoleId       = roleAdmin.Id,
                IsActive     = true,
                CreatedAt    = DateTime.Now
            };
            var userTrainer = new User
            {
                Login        = "trainer",
                PasswordHash = "trainer123",
                FullName     = "Иванов Иван Иванович",
                RoleId       = roleTrainer.Id,
                IsActive     = true,
                CreatedAt    = DateTime.Now
            };
            db.Users.Add(userAdmin);
            db.Users.Add(userTrainer);
            db.SaveChanges();

            // Сотрудник-тренер
            var trainer = new Employee
            {
                FullName       = "Иванов Иван Иванович",
                Position       = "Тренер",
                Phone          = "+7-900-111-22-33",
                Email          = "ivanov@gym.ru",
                HireDate       = new DateOnly(2022, 9, 1),
                Specialization = "Групповые тренировки, йога",
                IsActive       = true,
                UserId         = userTrainer.Id
            };
            db.Employees.Add(trainer);
            db.SaveChanges();

            // Тренировки
            var training1 = new Training
            {
                Name            = "Йога",
                DayOfWeek       = 1, // Понедельник
                StartTime       = new TimeOnly(9, 0),
                EndTime         = new TimeOnly(10, 0),
                MaxParticipants = 15,
                Hall            = "Зал №1",
                IsActive        = true,
                EmployeeId      = trainer.Id
            };
            var training2 = new Training
            {
                Name            = "Силовая тренировка",
                DayOfWeek       = 3, // Среда
                StartTime       = new TimeOnly(18, 0),
                EndTime         = new TimeOnly(19, 30),
                MaxParticipants = 20,
                Hall            = "Тренажёрный зал",
                IsActive        = true,
                EmployeeId      = trainer.Id
            };
            db.Trainings.Add(training1);
            db.Trainings.Add(training2);
            db.SaveChanges();

            // Типы абонементов
            var subMonth = new Subscription
            {
                TypeName     = "Месячный",
                Price        = 3500.00m,
                DurationDays = 30,
                VisitsLimit  = null,
                Description  = "Безлимитные посещения на 30 дней",
                IsActive     = true
            };
            var subVisit = new Subscription
            {
                TypeName     = "Разовый",
                Price        = 400.00m,
                DurationDays = 1,
                VisitsLimit  = 1,
                Description  = "Одно посещение",
                IsActive     = true
            };
            var subYear = new Subscription
            {
                TypeName     = "Годовой",
                Price        = 35000.00m,
                DurationDays = 365,
                VisitsLimit  = null,
                Description  = "Безлимитные посещения на год",
                IsActive     = true
            };
            db.Subscriptions.Add(subMonth);
            db.Subscriptions.Add(subVisit);
            db.Subscriptions.Add(subYear);
            db.SaveChanges();

            // Клиенты
            var clients = new List<Client>
            {
                new Client { FullName = "Петрова Анна Сергеевна",   Phone = "+7-900-123-45-67", Email = "petrova@mail.ru",  RegistrationDate = new DateOnly(2024, 1, 15), Status = "active" },
                new Client { FullName = "Сидоров Михаил Петрович",  Phone = "+7-901-234-56-78", Email = "sidorov@mail.ru",  RegistrationDate = new DateOnly(2024, 3, 20), Status = "active" },
                new Client { FullName = "Козлова Елена Ивановна",   Phone = "+7-902-345-67-89", Email = "kozlova@mail.ru",  RegistrationDate = new DateOnly(2024, 5, 10), Status = "active" },
                new Client { FullName = "Новиков Дмитрий Алексеевич",Phone = "+7-903-456-78-90", Email = null,              RegistrationDate = new DateOnly(2024, 7,  5), Status = "active" },
                new Client { FullName = "Морозова Ольга Николаевна",Phone = "+7-904-567-89-01", Email = "morozova@mail.ru", RegistrationDate = new DateOnly(2024, 9,  1), Status = "inactive"},
            };
            db.Clients.AddRange(clients);
            db.SaveChanges();

            // Абонементы клиентов
            db.ClientSubscriptions.Add(new ClientSubscription
            {
                ClientId       = clients[0].Id,
                SubscriptionId = subMonth.Id,
                PurchaseDate   = new DateOnly(2026, 4, 1),
                StartDate      = new DateOnly(2026, 4, 1),
                EndDate        = new DateOnly(2026, 4, 30),
                Status         = "active"
            });
            db.ClientSubscriptions.Add(new ClientSubscription
            {
                ClientId       = clients[1].Id,
                SubscriptionId = subYear.Id,
                PurchaseDate   = new DateOnly(2026, 1, 1),
                StartDate      = new DateOnly(2026, 1, 1),
                EndDate        = new DateOnly(2026, 12, 31),
                Status         = "active"
            });
            db.SaveChanges();

            // Посещения
            db.Attendances.Add(new Attendance
            {
                ClientId       = clients[0].Id,
                TrainingId     = training1.Id,
                AttendanceDate = new DateOnly(2026, 4, 28),
                IsPresent      = true,
                MarkTime       = DateTime.Now,
                UserId         = userAdmin.Id
            });
            db.Attendances.Add(new Attendance
            {
                ClientId       = clients[1].Id,
                TrainingId     = training2.Id,
                AttendanceDate = new DateOnly(2026, 4, 30),
                IsPresent      = true,
                MarkTime       = DateTime.Now,
                UserId         = userAdmin.Id
            });
            db.SaveChanges();

            // Услуги
            var service = new Service
            {
                Name        = "Массаж спортивный",
                Price       = 1500.00m,
                Category    = "wellness",
                Description = "60 минут спортивного массажа",
                IsActive    = true
            };
            db.Services.Add(service);
            db.SaveChanges();

            // Заказ услуги
            var order = new ServiceOrder
            {
                ClientId      = clients[0].Id,
                EmployeeId    = trainer.Id,
                OrderDate     = DateTime.Now,
                TotalAmount   = 1500.00m,
                PaymentStatus = "paid",
                PaymentMethod = "card"
            };
            db.ServiceOrders.Add(order);
            db.SaveChanges();

            db.ServiceOrderItems.Add(new ServiceOrderItem
            {
                ServiceOrderId = order.Id,
                ServiceId      = service.Id,
                Quantity       = 1,
                UnitPrice      = 1500.00m
            });
            db.SaveChanges();
        }
    }
}
