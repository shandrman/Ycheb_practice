using System.Windows;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Первый запуск: создать БД если не существует
            using (DatabaseContext db = new DatabaseContext())
            {
                if (db.Database.EnsureCreated())
                    ManagerService.DataService.SetDefaultData(db);
            }

            // Загружаем данные через слой доступа к данным (ManagerService)
            LoadClients();
        }

        /// <summary>
        /// Загружает клиентов через ManagerService.DataService.Get — 
        /// именно так требует задание 2.
        /// </summary>
        private void LoadClients()
        {
            // Простой вызов без параметров — получить все записи
            dgClients.ItemsSource = ManagerService.DataService.Get<Client>();

            int count = (dgClients.ItemsSource as List<Client>)?.Count ?? 0;
            tbStatus.Text = $"Загружено клиентов: {count}  |  " +
                            $"База данных: FitnessManager.db  |  " +
                            $"Метод: ManagerService.DataService.Get<Client>()";
        }

        /// <summary>
        /// Пример Get с фильтром — только активные клиенты, сортировка по ФИО.
        /// Раскомментировать для проверки фильтрации.
        /// </summary>
        private void LoadClientsFiltered()
        {
            var parameters = new QueryParameters<Client>
            {
                Filters = new List<FilterCondition>
                {
                    // Только активные клиенты
                    new FilterCondition("Status", "==", "active")
                },
                SortBy      = "FullName",
                IsAscending = true
            };

            dgClients.ItemsSource = ManagerService.DataService.Get(parameters);
        }

        /// <summary>
        /// Кнопка «Пересоздать БД» — вызывает RecreateDatabase через ManagerService.
        /// </summary>
        private void BtnRecreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Все данные будут удалены и база данных будет пересоздана.\nПродолжить?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                ManagerService.DataService.RecreateDatabase();
                LoadClients();   // Обновляем DataGrid после пересоздания
            }
        }
    }
}
