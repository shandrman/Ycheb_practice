using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Pages
{
    public partial class ClientsPage : Page
    {
        private List<Client> _all = new();
        private bool _loaded = false;

        public ClientsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => { _loaded = true; Load(); };
        }

        private void Load()
        {
            // Получаем всех клиентов через слой доступа к данным
            var request = new QueryParameters<Client>
            {
                SortBy      = "FullName",
                IsAscending = true
            };
            _all = ManagerService.DataService.Get(request);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (!_loaded) return;

            string search = tbSearch.Text.Trim().ToLower();
            string status = (cbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            var filtered = _all.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(c =>
                    c.FullName.ToLower().Contains(search) ||
                    c.Phone.Contains(search) ||
                    (c.Email?.ToLower().Contains(search) == true));

            if (status != "Все статусы" && !string.IsNullOrEmpty(status))
                filtered = filtered.Where(c => c.Status == status);

            var list = filtered.ToList();
            dgClients.ItemsSource = list;
            tbStatus.Text = $"Найдено: {list.Count} из {_all.Count} клиентов";
        }

        private void TbSearch_TextChanged(object s, TextChangedEventArgs e) => ApplyFilter();
        private void CbStatus_Changed(object s, SelectionChangedEventArgs e)  => ApplyFilter();

        private void BtnReset_Click(object s, RoutedEventArgs e)
        {
            tbSearch.Clear();
            cbStatus.SelectedIndex = 0;
            ApplyFilter();
        }

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            var win = new Windows.ClientEditWindow(null);
            if (win.ShowDialog() == true) Load();
        }

        private void Dg_DoubleClick(object s, MouseButtonEventArgs e)
        {
            if (dgClients.SelectedItem is Client c)
            {
                var win = new Windows.ClientEditWindow(c);
                if (win.ShowDialog() == true) Load();
            }
        }
    }
}
