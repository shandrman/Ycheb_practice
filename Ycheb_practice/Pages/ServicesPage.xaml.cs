using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

// Алиас решает конфликт: папка Service/ — это namespace Ycheb_practice.Service,
// а SvcModel — это класс DatabaseModel.Service
using SvcModel = Ycheb_practice.DatabaseModel.Service;

namespace Ycheb_practice.Pages
{
    public partial class ServicesPage : Page
    {
        private List<SvcVM> _all = new();
        private bool _loaded = false;

        public ServicesPage()
        {
            InitializeComponent();
            Loaded += (s, e) => { _loaded = true; Load(); };
        }

        private void Load()
        {
            _all = ManagerService.DataService.Get<SvcModel>(
                new QueryParameters<SvcModel>
                {
                    SortBy = "Name", IsAscending = true
                })
                .Select(sv => new SvcVM
                {
                    Name        = sv.Name,
                    Category    = sv.Category,
                    Price       = sv.Price,
                    Description = sv.Description ?? "—",
                    ActiveStr   = sv.IsActive == true ? "Да" : "Нет",
                    Source      = sv
                }).ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (!_loaded) return;
            string search = tbSearch.Text.Trim().ToLower();
            string cat    = (cbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            var filtered = _all.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(s => s.Name.ToLower().Contains(search));
            if (cat != "Все категории" && !string.IsNullOrEmpty(cat))
                filtered = filtered.Where(s => s.Category == cat);

            var list = filtered.ToList();
            dgServices.ItemsSource = list;
            tbStatus.Text = $"Услуг: {list.Count} из {_all.Count}";
        }

        private void TbSearch_TextChanged(object s, TextChangedEventArgs e) => ApplyFilter();
        private void CbCat_Changed(object s, SelectionChangedEventArgs e)    => ApplyFilter();

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            var win = new Windows.ServiceEditWindow(null);
            if (win.ShowDialog() == true) Load();
        }

        private void Dg_DoubleClick(object s, MouseButtonEventArgs e)
        {
            if (dgServices.SelectedItem is SvcVM vm)
            {
                var win = new Windows.ServiceEditWindow(vm.Source);
                if (win.ShowDialog() == true) Load();
            }
        }
    }

    public class SvcVM
    {
        public string   Name        { get; set; } = "";
        public string   Category    { get; set; } = "";
        public decimal  Price       { get; set; }
        public string   Description { get; set; } = "";
        public string   ActiveStr   { get; set; } = "";
        public SvcModel Source      { get; set; } = null!;
    }
}
