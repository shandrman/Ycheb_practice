using System.Windows;
using System.Windows.Controls;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Windows
{
    public partial class ClientEditWindow : Window
    {
        private readonly Client? _client;
        private readonly bool    _isNew;

        public ClientEditWindow(Client? client)
        {
            InitializeComponent();
            _client = client;
            _isNew  = client == null;

            tbTitle.Text          = _isNew ? "Добавление клиента" : "Редактирование клиента";
            btnDelete.Visibility  = _isNew ? Visibility.Collapsed : Visibility.Visible;

            if (!_isNew)
            {
                tbFullName.Text    = client!.FullName;
                tbPhone.Text       = client.Phone;
                tbEmail.Text       = client.Email;
                dpBirth.SelectedDate = client.BirthDate.HasValue
                    ? client.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
                    : null;
                tbNotes.Text = client.Notes;

                foreach (ComboBoxItem item in cbStatus.Items)
                    if (item.Content?.ToString() == client.Status)
                    { cbStatus.SelectedItem = item; break; }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFullName.Text))
            { MessageBox.Show("Введите ФИО."); return; }
            if (string.IsNullOrWhiteSpace(tbPhone.Text))
            { MessageBox.Show("Введите телефон."); return; }

            string status = (cbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "active";

            if (_isNew)
            {
                var c = new Client
                {
                    FullName         = tbFullName.Text.Trim(),
                    Phone            = tbPhone.Text.Trim(),
                    Email            = tbEmail.Text.Trim(),
                    BirthDate        = dpBirth.SelectedDate.HasValue
                                        ? DateOnly.FromDateTime(dpBirth.SelectedDate.Value) : null,
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                    Status           = status,
                    Notes            = tbNotes.Text.Trim()
                };
                // Добавляем через слой доступа к данным
                ManagerService.DataService.Add(c);
            }
            else
            {
                _client!.FullName  = tbFullName.Text.Trim();
                _client.Phone      = tbPhone.Text.Trim();
                _client.Email      = tbEmail.Text.Trim();
                _client.BirthDate  = dpBirth.SelectedDate.HasValue
                                      ? DateOnly.FromDateTime(dpBirth.SelectedDate.Value) : null;
                _client.Status     = status;
                _client.Notes      = tbNotes.Text.Trim();
                // Редактируем через слой доступа к данным
                ManagerService.DataService.Edit(_client);
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        { DialogResult = false; Close(); }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить клиента «{_client!.FullName}»?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ManagerService.DataService.Delete(_client);
                DialogResult = true;
                Close();
            }
        }
    }
}
