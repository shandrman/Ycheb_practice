using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Linq.Dynamic.Core;
using Ycheb_practice.DatabaseModel;

namespace Ycheb_practice.Service
{
    public class DataService
    {
        // Хранит авторизованного пользователя после успешного Login
        public User? AuthUser;

        public DataService()
        {
            AuthUser = null;
        }

        // ─── GET ──────────────────────────────────────────────────────────────
        /// <summary>
        /// Универсальный метод получения данных из любой таблицы.
        /// Поддерживает Include (связи), фильтры и сортировку.
        /// </summary>
        public List<T> Get<T>(QueryParameters<T>? parameters = null) where T : class
        {
            parameters ??= new QueryParameters<T>();

            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    IQueryable<T> query = db.Set<T>();

                    // 1. Include — подгрузка связанных таблиц
                    foreach (var include in parameters.Includes)
                        query = query.Include(include);

                    // 2. Фильтры через Dynamic LINQ
                    // Пример: new FilterCondition("Status", "==", "active")
                    foreach (var filter in parameters.Filters)
                        query = query.Where(
                            $"{filter.PropertyName} {filter.Operation} @0",
                            filter.Value);

                    // 3. Сортировка
                    if (!string.IsNullOrEmpty(parameters.SortBy))
                    {
                        string dir = parameters.IsAscending ? "ascending" : "descending";
                        query = query.OrderBy($"{parameters.SortBy} {dir}");
                    }

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения данных: " + ex.Message);
                return new List<T>();
            }
        }

        // ─── LOGIN ────────────────────────────────────────────────────────────
        /// <summary>
        /// Авторизация: ищет пользователя по логину и паролю.
        /// При успехе записывает пользователя в AuthUser и возвращает true.
        /// </summary>
        public bool Login(string login, string password)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    // В нашей модели поле называется PasswordHash
                    var userFind = db.Users
                        .Include(u => u.Role)
                        .FirstOrDefault(u => u.Login == login
                                          && u.PasswordHash == password
                                          && u.IsActive == true);

                    if (userFind != null)
                    {
                        // Обновляем время последнего входа
                        userFind.LastLogin = DateTime.Now;
                        db.SaveChanges();

                        AuthUser = userFind;
                        return true;
                    }
                    else
                    {
                        AuthUser = null;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка авторизации: " + ex.Message);
                AuthUser = null;
                return false;
            }
        }

        public void Logout()
        {
            AuthUser = null;
        }

        // ─── ADD ──────────────────────────────────────────────────────────────
        /// <summary>
        /// Добавляет новую запись в таблицу.
        /// Возвращает количество сохранённых строк (1 при успехе, -1 при ошибке).
        /// </summary>
        public int Add<T>(T obj) where T : class
        {
            try
            {
                int count;
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Set<T>().Add(obj);
                    count = db.SaveChanges();
                }

                if (count > 0)
                    MessageBox.Show($"Данные сохранены. Добавлено записей: {count}");
                else
                    MessageBox.Show("Данные не сохранены.");

                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
                return -1;
            }
        }

        // ─── EDIT ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Обновляет существующую запись в таблице.
        /// Объект должен иметь корректный первичный ключ.
        /// </summary>
        public int Edit<T>(T obj) where T : class
        {
            try
            {
                int count;
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Set<T>().Attach(obj);
                    db.Entry(obj).State = EntityState.Modified;
                    count = db.SaveChanges();
                }

                if (count > 0)
                    MessageBox.Show($"Данные сохранены. Изменено записей: {count}");
                else
                    MessageBox.Show("Данные не сохранены.");

                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка редактирования: " + ex.Message);
                return -1;
            }
        }

        // ─── DELETE ───────────────────────────────────────────────────────────
        /// <summary>
        /// Удаляет запись по объекту.
        /// </summary>
        public int Delete<T>(T obj) where T : class
        {
            try
            {
                int count;
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Set<T>().Attach(obj);
                    db.Set<T>().Remove(obj);
                    count = db.SaveChanges();
                }

                if (count > 0)
                    MessageBox.Show($"Запись удалена.");
                else
                    MessageBox.Show("Удаление не выполнено.");

                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
                return -1;
            }
        }

        // ─── RECREATE DATABASE ────────────────────────────────────────────────
        /// <summary>
        /// Полностью пересоздаёт БД и заполняет начальными данными.
        /// ВНИМАНИЕ: все существующие данные будут удалены!
        /// </summary>
        public void RecreateDatabase()
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    // Удаляем БД только если она существует
                    if (db.Database.CanConnect())
                        db.Database.EnsureDeleted();

                    // Создаём заново
                    if (db.Database.EnsureCreated())
                        SetDefaultData(db);
                }

                MessageBox.Show("База данных успешно пересоздана.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка пересоздания БД: " + ex.Message);
            }
        }

        // ─── SEED DATA ────────────────────────────────────────────────────────
        /// <summary>
        /// Заполняет БД начальными тестовыми данными.
        /// </summary>
        public void SetDefaultData(DatabaseContext db)
        {
            // Роли
            var roleAdmin   = new Role { RoleName = "Администратор", Description = "Полный доступ" };
            var roleTrainer = new Role { RoleName = "Тренер",        Description = "Доступ к тренировкам" };
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
                Specialization = "Групповые тренировки",
                IsActive       = true,
                UserId         = userTrainer.Id
            };
            db.Employees.Add(trainer);
            db.SaveChanges();

            // Тренировки
            db.Trainings.Add(new Training
            {
                Name            = "Йога",
                DayOfWeek       = 1,
                StartTime       = new TimeOnly(9, 0),
                EndTime         = new TimeOnly(10, 0),
                MaxParticipants = 15,
                Hall            = "Зал №1",
                IsActive        = true,
                EmployeeId      = trainer.Id
            });
            db.Trainings.Add(new Training
            {
                Name            = "Силовая тренировка",
                DayOfWeek       = 3,
                StartTime       = new TimeOnly(18, 0),
                EndTime         = new TimeOnly(19, 30),
                MaxParticipants = 20,
                Hall            = "Тренажёрный зал",
                IsActive        = true,
                EmployeeId      = trainer.Id
            });
            db.SaveChanges();

            // Абонементы
            var subMonth = new Subscription
            {
                TypeName     = "Месячный",
                Price        = 3500m,
                DurationDays = 30,
                Description  = "Безлимит на 30 дней",
                IsActive     = true
            };
            var subYear = new Subscription
            {
                TypeName     = "Годовой",
                Price        = 35000m,
                DurationDays = 365,
                Description  = "Безлимит на год",
                IsActive     = true
            };
            db.Subscriptions.Add(subMonth);
            db.Subscriptions.Add(subYear);
            db.SaveChanges();

            // Клиенты
            var clients = new List<Client>
            {
                new() { FullName = "Петрова Анна Сергеевна",    Phone = "+7-900-123-45-67", Email = "petrova@mail.ru",  RegistrationDate = new DateOnly(2024, 1, 15), Status = "active"   },
                new() { FullName = "Сидоров Михаил Петрович",   Phone = "+7-901-234-56-78", Email = "sidorov@mail.ru",  RegistrationDate = new DateOnly(2024, 3, 20), Status = "active"   },
                new() { FullName = "Козлова Елена Ивановна",    Phone = "+7-902-345-67-89", Email = "kozlova@mail.ru",  RegistrationDate = new DateOnly(2024, 5, 10), Status = "active"   },
                new() { FullName = "Новиков Дмитрий Алексеевич",Phone = "+7-903-456-78-90", Email = null,               RegistrationDate = new DateOnly(2024, 7,  5), Status = "active"   },
                new() { FullName = "Морозова Ольга Николаевна", Phone = "+7-904-567-89-01", Email = "morozova@mail.ru", RegistrationDate = new DateOnly(2024, 9,  1), Status = "inactive" },
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
        }
    }
}
