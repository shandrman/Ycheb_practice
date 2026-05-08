using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ycheb_practice.Service;

namespace Ycheb_practice
{
    public partial class MainWindow : Window
    {
        // Ищем элементы через FindName() в Loaded —
        // это 100% рабочий способ независимо от генерации g.cs
        private Button?    _btnClients;
        private Button?    _btnSubscriptions;
        private Button?    _btnTrainings;
        private Button?    _btnAttendance;
        private Button?    _btnEmployees;
        private Button?    _btnServices;
        private TextBlock? _tbUserName;
        private TextBlock? _tbUserRole;
        private TextBlock? _tbPageTitle;
        private Frame?     _mainFrame;

        private Dictionary<string, Button> _navBtns = new();

        private static readonly SolidColorBrush BgActive   = new(Color.FromRgb(45, 62, 85));
        private static readonly SolidColorBrush BgInactive = Brushes.Transparent;
        private static readonly SolidColorBrush FgActive   = Brushes.White;
        private static readonly SolidColorBrush FgInactive = new(Color.FromRgb(168, 184, 204));

        public MainWindow()
        {
            InitializeComponent();
        }

        // Все FindName() — только после того как XAML полностью загружен
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Находим элементы по имени
            _btnClients       = FindName("btnClients")       as Button;
            _btnSubscriptions = FindName("btnSubscriptions") as Button;
            _btnTrainings     = FindName("btnTrainings")     as Button;
            _btnAttendance    = FindName("btnAttendance")    as Button;
            _btnEmployees     = FindName("btnEmployees")     as Button;
            _btnServices      = FindName("btnServices")      as Button;
            _tbUserName       = FindName("tbUserName")       as TextBlock;
            _tbUserRole       = FindName("tbUserRole")       as TextBlock;
            _tbPageTitle      = FindName("tbPageTitle")      as TextBlock;
            _mainFrame        = FindName("MainFrame")        as Frame;

            // Заполняем словарь навигации
            _navBtns = new Dictionary<string, Button>();
            if (_btnClients       != null) _navBtns["Clients"]       = _btnClients;
            if (_btnSubscriptions != null) _navBtns["Subscriptions"] = _btnSubscriptions;
            if (_btnTrainings     != null) _navBtns["Trainings"]     = _btnTrainings;
            if (_btnAttendance    != null) _navBtns["Attendance"]    = _btnAttendance;
            if (_btnEmployees     != null) _navBtns["Employees"]     = _btnEmployees;
            if (_btnServices      != null) _navBtns["Services"]      = _btnServices;

            // Начальный стиль всех кнопок — неактивный
            foreach (var btn in _navBtns.Values)
                SetInactive(btn);

            // Данные авторизованного пользователя
            var user = ManagerService.GetAuth();
            if (_tbUserName != null) _tbUserName.Text = user.FullName;
            if (_tbUserRole != null) _tbUserRole.Text = user.Role?.RoleName ?? "—";

            // Открываем первую страницу
            NavigateTo("Clients");
        }

        private void NavBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
                NavigateTo(btn.Tag?.ToString() ?? "Clients");
        }

        public void NavigateTo(string page)
        {
            // Сбрасываем стиль всех кнопок
            foreach (var kv in _navBtns)
                SetInactive(kv.Value);

            // Активируем нужную
            if (_navBtns.TryGetValue(page, out var activeBtn))
                SetActive(activeBtn);

            // Создаём страницу
            Page p = page switch
            {
                "Clients"       => new Pages.ClientsPage(),
                "Subscriptions" => new Pages.SubscriptionsPage(),
                "Trainings"     => new Pages.TrainingsPage(),
                "Attendance"    => new Pages.AttendancePage(),
                "Employees"     => new Pages.EmployeesPage(),
                "Services"      => new Pages.ServicesPage(),
                _               => new Pages.ClientsPage()
            };

            if (_tbPageTitle != null)
                _tbPageTitle.Text = page switch
                {
                    "Clients"       => "Клиенты",
                    "Subscriptions" => "Абонементы",
                    "Trainings"     => "Расписание тренировок",
                    "Attendance"    => "Посещаемость",
                    "Employees"     => "Персонал",
                    "Services"      => "Услуги",
                    _               => page
                };

            _mainFrame?.Navigate(p);
        }

        private static void SetActive(Button btn)
        {
            btn.Background = BgActive;
            btn.Foreground = FgActive;
        }

        private static void SetInactive(Button btn)
        {
            btn.Background = BgInactive;
            btn.Foreground = FgInactive;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("Выйти из системы?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                ManagerService.DataService.Logout();
                new Windows.LoginWindow().Show();
                this.Close();
            }
        }
    }
}
