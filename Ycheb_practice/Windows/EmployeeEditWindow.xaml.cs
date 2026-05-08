using System.Windows;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Windows
{
    public partial class EmployeeEditWindow : Window
    {
        private readonly Employee? _emp;
        private readonly bool      _isNew;

        public EmployeeEditWindow(Employee? emp)
        {
            InitializeComponent();
            _emp   = emp;
            _isNew = emp == null;
            tbTitle.Text         = _isNew ? "Добавление сотрудника" : "Редактирование сотрудника";
            btnDelete.Visibility = _isNew ? Visibility.Collapsed : Visibility.Visible;

            if (_isNew)
                dpHire.SelectedDate = DateTime.Today;
            else
            {
                tbName.Text     = emp!.FullName;
                tbPosition.Text = emp.Position;
                tbSpec.Text     = emp.Specialization;
                tbPhone.Text    = emp.Phone;
                tbEmail.Text    = emp.Email;
                dpHire.SelectedDate  = emp.HireDate.ToDateTime(TimeOnly.MinValue);
                chkActive.IsChecked  = emp.IsActive ?? true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))     { MessageBox.Show("Введите ФИО.");       return; }
            if (string.IsNullOrWhiteSpace(tbPosition.Text)) { MessageBox.Show("Введите должность."); return; }
            if (!dpHire.SelectedDate.HasValue)               { MessageBox.Show("Укажите дату приёма."); return; }

            var hireDate = DateOnly.FromDateTime(dpHire.SelectedDate.Value);

            if (_isNew)
            {
                ManagerService.DataService.Add(new Employee
                {
                    FullName       = tbName.Text.Trim(),
                    Position       = tbPosition.Text.Trim(),
                    Specialization = tbSpec.Text.Trim(),
                    Phone          = tbPhone.Text.Trim(),
                    Email          = tbEmail.Text.Trim(),
                    HireDate       = hireDate,
                    IsActive       = chkActive.IsChecked
                });
            }
            else
            {
                _emp!.FullName       = tbName.Text.Trim();
                _emp.Position        = tbPosition.Text.Trim();
                _emp.Specialization  = tbSpec.Text.Trim();
                _emp.Phone           = tbPhone.Text.Trim();
                _emp.Email           = tbEmail.Text.Trim();
                _emp.HireDate        = hireDate;
                _emp.IsActive        = chkActive.IsChecked;
                ManagerService.DataService.Edit(_emp);
            }
            DialogResult = true; Close();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e) { DialogResult = false; Close(); }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить сотрудника «{_emp!.FullName}»?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ManagerService.DataService.Delete(_emp);
                DialogResult = true; Close();
            }
        }
    }
}
