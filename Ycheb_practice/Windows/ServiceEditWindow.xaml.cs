using System.Windows;
using System.Windows.Controls;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

// Алиас решает конфликт имён: namespace Ycheb_practice.Service vs класс DatabaseModel.Service
using SvcModel = Ycheb_practice.DatabaseModel.Service;

namespace Ycheb_practice.Windows
{
    public partial class ServiceEditWindow : Window
    {
        private readonly SvcModel? _svc;
        private readonly bool      _isNew;

        public ServiceEditWindow(SvcModel? svc)
        {
            InitializeComponent();
            _svc   = svc;
            _isNew = svc == null;
            tbTitle.Text         = _isNew ? "Добавление услуги" : "Редактирование услуги";
            btnDelete.Visibility = _isNew ? Visibility.Collapsed : Visibility.Visible;

            if (!_isNew)
            {
                tbName.Text  = svc!.Name;
                tbPrice.Text = svc.Price.ToString("F2");
                tbDesc.Text  = svc.Description;
                chkActive.IsChecked = svc.IsActive ?? true;
                foreach (ComboBoxItem item in cbCat.Items)
                    if (item.Content?.ToString() == svc.Category)
                    { cbCat.SelectedItem = item; break; }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            { MessageBox.Show("Введите название."); return; }
            if (!decimal.TryParse(tbPrice.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal price))
            { MessageBox.Show("Некорректная цена."); return; }

            string cat = (cbCat.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "SPA";

            if (_isNew)
            {
                ManagerService.DataService.Add(new SvcModel
                {
                    Name        = tbName.Text.Trim(),
                    Category    = cat,
                    Price       = price,
                    Description = tbDesc.Text.Trim(),
                    IsActive    = chkActive.IsChecked
                });
            }
            else
            {
                _svc!.Name        = tbName.Text.Trim();
                _svc.Category     = cat;
                _svc.Price        = price;
                _svc.Description  = tbDesc.Text.Trim();
                _svc.IsActive     = chkActive.IsChecked;
                ManagerService.DataService.Edit(_svc);
            }
            DialogResult = true; Close();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e) { DialogResult = false; Close(); }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить услугу «{_svc!.Name}»?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ManagerService.DataService.Delete(_svc);
                DialogResult = true; Close();
            }
        }
    }
}
