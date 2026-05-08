using System.Windows;
using System.Windows.Controls;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Windows
{
    public partial class AttendanceMarkWindow : Window
    {
        public AttendanceMarkWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                cbClient.ItemsSource = ManagerService.DataService.Get<Client>(
                    new QueryParameters<Client>
                    {
                        SortBy = "FullName", IsAscending = true
                    });

                cbTraining.ItemsSource = ManagerService.DataService.Get<Training>(
                    new QueryParameters<Training>
                    {
                        SortBy = "Name", IsAscending = true
                    });

                dpDate.SelectedDate = DateTime.Today;
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbClient.SelectedItem   == null) { MessageBox.Show("Выберите клиента.");    return; }
            if (cbTraining.SelectedItem == null) { MessageBox.Show("Выберите тренировку."); return; }
            if (!dpDate.SelectedDate.HasValue)   { MessageBox.Show("Укажите дату.");        return; }

            bool isPresent = (cbPresent.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Да";
            int  clientId  = (int)cbClient.SelectedValue;
            int  trainId   = (int)cbTraining.SelectedValue;
            var  date      = DateOnly.FromDateTime(dpDate.SelectedDate.Value);

            var attendance = new Attendance
            {
                ClientId       = clientId,
                TrainingId     = trainId,
                AttendanceDate = date,
                IsPresent      = isPresent,
                MarkTime       = DateTime.Now,
                UserId         = ManagerService.GetAuth().Id > 0
                                    ? ManagerService.GetAuth().Id : null
            };

            ManagerService.DataService.Add(attendance);
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e) { DialogResult = false; Close(); }
    }
}
