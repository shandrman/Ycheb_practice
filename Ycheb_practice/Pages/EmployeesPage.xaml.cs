using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Pages
{
    public partial class EmployeesPage : Page
    {
        private List<EmpVM> _all = new();
        private bool _loaded = false;

        public EmployeesPage()
        {
            InitializeComponent();
            Loaded += (s, e) => { _loaded = true; Load(); };
        }

        private void Load()
        {
            _all = ManagerService.DataService.Get<Employee>(
                new QueryParameters<Employee>
                {
                    SortBy = "FullName", IsAscending = true
                })
                .Select(emp => new EmpVM
                {
                    FullName       = emp.FullName,
                    Position       = emp.Position,
                    Specialization = emp.Specialization ?? "—",
                    Phone          = emp.Phone ?? "—",
                    HireDate       = emp.HireDate.ToString("dd.MM.yyyy"),
                    ActiveStr      = emp.IsActive == true ? "Да" : "Нет",
                    Source         = emp
                }).ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (!_loaded) return;
            string s = tbSearch.Text.Trim().ToLower();
            var filtered = string.IsNullOrEmpty(s) ? _all :
                _all.Where(e => e.FullName.ToLower().Contains(s) ||
                                e.Position.ToLower().Contains(s)).ToList();
            dgEmployees.ItemsSource = filtered;
            tbStatus.Text = $"Сотрудников: {filtered.Count} из {_all.Count}";
        }

        private void TbSearch_TextChanged(object s, TextChangedEventArgs e) => ApplyFilter();

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            var win = new Windows.EmployeeEditWindow(null);
            if (win.ShowDialog() == true) Load();
        }

        private void Dg_DoubleClick(object s, MouseButtonEventArgs e)
        {
            if (dgEmployees.SelectedItem is EmpVM vm)
            {
                var win = new Windows.EmployeeEditWindow(vm.Source);
                if (win.ShowDialog() == true) Load();
            }
        }
    }

    public class EmpVM
    {
        public string   FullName       { get; set; } = "";
        public string   Position       { get; set; } = "";
        public string   Specialization { get; set; } = "";
        public string   Phone          { get; set; } = "";
        public string   HireDate       { get; set; } = "";
        public string   ActiveStr      { get; set; } = "";
        public Employee Source         { get; set; } = null!;
    }
}
