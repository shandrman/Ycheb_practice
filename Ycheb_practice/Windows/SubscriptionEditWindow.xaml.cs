using System.Windows;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Windows
{
    public partial class SubscriptionEditWindow : Window
    {
        private readonly Subscription? _sub;
        private readonly bool          _isNew;

        public SubscriptionEditWindow(Subscription? sub)
        {
            InitializeComponent();
            _sub   = sub;
            _isNew = sub == null;
            tbTitle.Text         = _isNew ? "Добавление типа абонемента" : "Редактирование";
            btnDelete.Visibility = _isNew ? Visibility.Collapsed : Visibility.Visible;
            if (!_isNew)
            {
                tbName.Text    = sub!.TypeName;
                tbPrice.Text   = sub.Price.ToString("F2");
                tbDays.Text    = sub.DurationDays.ToString();
                tbVisits.Text  = sub.VisitsLimit?.ToString() ?? "";
                tbDesc.Text    = sub.Description;
                chkActive.IsChecked = sub.IsActive ?? true;
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
            if (!int.TryParse(tbDays.Text, out int days) || days <= 0)
            { MessageBox.Show("Некорректный срок."); return; }

            int? visits = null;
            if (!string.IsNullOrWhiteSpace(tbVisits.Text))
            {
                if (!int.TryParse(tbVisits.Text, out int v))
                { MessageBox.Show("Некорректный лимит визитов."); return; }
                visits = v;
            }

            if (_isNew)
            {
                ManagerService.DataService.Add(new Subscription
                {
                    TypeName     = tbName.Text.Trim(),
                    Price        = price,
                    DurationDays = days,
                    VisitsLimit  = visits,
                    Description  = tbDesc.Text.Trim(),
                    IsActive     = chkActive.IsChecked
                });
            }
            else
            {
                _sub!.TypeName     = tbName.Text.Trim();
                _sub.Price         = price;
                _sub.DurationDays  = days;
                _sub.VisitsLimit   = visits;
                _sub.Description   = tbDesc.Text.Trim();
                _sub.IsActive      = chkActive.IsChecked;
                ManagerService.DataService.Edit(_sub);
            }
            DialogResult = true; Close();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e) { DialogResult = false; Close(); }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить тип «{_sub!.TypeName}»?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ManagerService.DataService.Delete(_sub);
                DialogResult = true; Close();
            }
        }
    }
}
