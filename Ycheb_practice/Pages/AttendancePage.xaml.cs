using System.Windows;
using System.Windows.Controls;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Pages
{
    public partial class AttendancePage : Page
    {
        private List<AttVM> _all = new();
        private bool _loaded = false;

        public AttendancePage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                _loaded = true;
                dpFrom.SelectedDate = DateTime.Today.AddDays(-7);
                dpTo.SelectedDate   = DateTime.Today;
                Load();
            };
        }

        private void Load()
        {
            var data = ManagerService.DataService.Get<Attendance>(
                new QueryParameters<Attendance>
                {
                    Includes    = new List<string> { "Client", "Training" },
                    SortBy      = "AttendanceDate",
                    IsAscending = false
                });

            _all = data.Select(a => new AttVM
            {
                ClientName   = a.Client?.FullName   ?? "—",
                TrainingName = a.Training?.Name     ?? "—",
                DateStr      = a.AttendanceDate.ToString("dd.MM.yyyy"),
                Date         = a.AttendanceDate,
                PresentStr   = a.IsPresent == true ? "✅ Да" : "❌ Нет",
                MarkStr      = a.MarkTime?.ToString("HH:mm") ?? "—"
            }).ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (!_loaded) return;

            string search  = tbSearch.Text.Trim().ToLower();
            DateOnly? from = dpFrom.SelectedDate.HasValue
                ? DateOnly.FromDateTime(dpFrom.SelectedDate.Value) : null;
            DateOnly? to   = dpTo.SelectedDate.HasValue
                ? DateOnly.FromDateTime(dpTo.SelectedDate.Value) : null;

            var f = _all.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
                f = f.Where(a => a.ClientName.ToLower().Contains(search) ||
                                 a.TrainingName.ToLower().Contains(search));
            if (from.HasValue) f = f.Where(a => a.Date >= from.Value);
            if (to.HasValue)   f = f.Where(a => a.Date <= to.Value);

            var list = f.ToList();
            dgAttendance.ItemsSource = list;
            tbStatus.Text = $"Записей: {list.Count}";
        }

        private void Filter_Changed(object s, EventArgs e) => ApplyFilter();

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            var win = new Windows.AttendanceMarkWindow();
            if (win.ShowDialog() == true) Load();
        }
    }

    public class AttVM
    {
        public string   ClientName   { get; set; } = "";
        public string   TrainingName { get; set; } = "";
        public string   DateStr      { get; set; } = "";
        public DateOnly Date         { get; set; }
        public string   PresentStr   { get; set; } = "";
        public string   MarkStr      { get; set; } = "";
    }
}
