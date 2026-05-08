using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Pages
{
    public partial class TrainingsPage : Page
    {
        private static readonly string[] DayNames =
            { "Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб" };
        private static readonly string[] DayNamesFull =
            { "Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };

        private List<TrainingVM> _all = new();
        private bool _loaded = false;

        public TrainingsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => { _loaded = true; Load(); };
        }

        private void Load()
        {
            var data = ManagerService.DataService.Get<Training>(
                new QueryParameters<Training>
                {
                    Includes    = new List<string> { "Employee" },
                    SortBy      = "DayOfWeek",
                    IsAscending = true
                });

            _all = data.Select(t => new TrainingVM
            {
                Id              = t.Id,
                Name            = t.Name,
                TrainerName     = t.Employee?.FullName ?? "—",
                DayName         = t.DayOfWeek < DayNamesFull.Length ? DayNamesFull[t.DayOfWeek] : "—",
                DayOfWeek       = t.DayOfWeek,
                StartStr        = t.StartTime.ToString(@"HH\:mm"),
                EndStr          = t.EndTime.ToString(@"HH\:mm"),
                Hall            = t.Hall,
                MaxParticipants = t.MaxParticipants,
                IsActiveStr     = t.IsActive == true ? "Да" : "Нет",
                Source          = t
            }).ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (!_loaded) return;
            string search = tbSearch.Text.Trim().ToLower();
            var dayItem   = cbDay.SelectedItem as ComboBoxItem;
            string dayTag = dayItem?.Tag?.ToString() ?? "";

            var filtered = _all.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(t =>
                    t.Name.ToLower().Contains(search) ||
                    t.TrainerName.ToLower().Contains(search));
            if (!string.IsNullOrEmpty(dayTag) && byte.TryParse(dayTag, out byte d))
                filtered = filtered.Where(t => t.DayOfWeek == d);

            var list = filtered.ToList();
            dgTrainings.ItemsSource = list;
            tbStatus.Text = $"Тренировок: {list.Count} из {_all.Count}";
        }

        private void TbSearch_TextChanged(object s, TextChangedEventArgs e) => ApplyFilter();
        private void CbDay_Changed(object s, SelectionChangedEventArgs e)    => ApplyFilter();

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            var win = new Windows.TrainingEditWindow(null);
            if (win.ShowDialog() == true) Load();
        }

        private void Dg_DoubleClick(object s, MouseButtonEventArgs e)
        {
            if (dgTrainings.SelectedItem is TrainingVM vm)
            {
                var win = new Windows.TrainingEditWindow(vm.Source);
                if (win.ShowDialog() == true) Load();
            }
        }
    }

    public class TrainingVM
    {
        public int      Id              { get; set; }
        public string   Name            { get; set; } = "";
        public string   TrainerName     { get; set; } = "";
        public string   DayName         { get; set; } = "";
        public byte     DayOfWeek       { get; set; }
        public string   StartStr        { get; set; } = "";
        public string   EndStr          { get; set; } = "";
        public string   Hall            { get; set; } = "";
        public int      MaxParticipants { get; set; }
        public string   IsActiveStr     { get; set; } = "";
        public Training Source          { get; set; } = null!;
    }
}
