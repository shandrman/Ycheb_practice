using System.Windows;
using System.Windows.Controls;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Windows
{
    public partial class TrainingEditWindow : Window
    {
        private readonly Training? _training;
        private readonly bool      _isNew;

        public TrainingEditWindow(Training? training)
        {
            InitializeComponent();
            _training = training;
            _isNew    = training == null;

            // Загружаем список тренеров (сотрудников)
            cbEmployee.ItemsSource = ManagerService.DataService.Get<Employee>(
                new QueryParameters<Employee>
                {
                    SortBy      = "FullName",
                    IsAscending = true
                });

            tbTitle.Text         = _isNew ? "Добавление тренировки" : "Редактирование";
            btnDelete.Visibility = _isNew ? Visibility.Collapsed : Visibility.Visible;

            if (!_isNew)
            {
                tbName.Text  = training!.Name;
                tbStart.Text = training.StartTime.ToString(@"HH\:mm");
                tbEnd.Text   = training.EndTime.ToString(@"HH\:mm");
                tbHall.Text  = training.Hall;
                tbMax.Text   = training.MaxParticipants.ToString();
                chkActive.IsChecked     = training.IsActive ?? true;
                cbEmployee.SelectedValue = training.EmployeeId;

                foreach (ComboBoxItem item in cbDay.Items)
                    if (item.Tag?.ToString() == training.DayOfWeek.ToString())
                    { cbDay.SelectedItem = item; break; }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            { MessageBox.Show("Введите название."); return; }
            if (cbEmployee.SelectedItem == null)
            { MessageBox.Show("Выберите тренера."); return; }
            if (!TimeOnly.TryParseExact(tbStart.Text, @"HH\:mm", out TimeOnly start))
            { MessageBox.Show("Некорректное время начала (ЧЧ:ММ)."); return; }
            if (!TimeOnly.TryParseExact(tbEnd.Text, @"HH\:mm", out TimeOnly end))
            { MessageBox.Show("Некорректное время окончания (ЧЧ:ММ)."); return; }
            if (!int.TryParse(tbMax.Text, out int maxP) || maxP <= 0)
            { MessageBox.Show("Некорректное количество участников."); return; }
            if (string.IsNullOrWhiteSpace(tbHall.Text))
            { MessageBox.Show("Введите зал."); return; }

            byte day = byte.Parse(((ComboBoxItem)cbDay.SelectedItem).Tag.ToString()!);
            int empId = (int)cbEmployee.SelectedValue;

            if (_isNew)
            {
                ManagerService.DataService.Add(new Training
                {
                    Name            = tbName.Text.Trim(),
                    EmployeeId      = empId,
                    DayOfWeek       = day,
                    StartTime       = start,
                    EndTime         = end,
                    Hall            = tbHall.Text.Trim(),
                    MaxParticipants = maxP,
                    IsActive        = chkActive.IsChecked
                });
            }
            else
            {
                _training!.Name            = tbName.Text.Trim();
                _training.EmployeeId       = empId;
                _training.DayOfWeek        = day;
                _training.StartTime        = start;
                _training.EndTime          = end;
                _training.Hall             = tbHall.Text.Trim();
                _training.MaxParticipants  = maxP;
                _training.IsActive         = chkActive.IsChecked;
                ManagerService.DataService.Edit(_training);
            }
            DialogResult = true; Close();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e) { DialogResult = false; Close(); }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить тренировку «{_training!.Name}»?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ManagerService.DataService.Delete(_training);
                DialogResult = true; Close();
            }
        }
    }
}
