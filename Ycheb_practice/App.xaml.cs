using System.Windows;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаём БД при первом запуске
            using (var db = new DatabaseContext())
            {
                if (db.Database.EnsureCreated())
                    ManagerService.DataService.SetDefaultData(db);
            }

            // Открываем окно входа
            new Windows.LoginWindow().Show();
        }
    }
}
