using Ycheb_practice.DatabaseModel;

namespace Ycheb_practice.Service
{
    /// <summary>
    /// Статический класс-посредник — единая точка доступа к слою данных.
    /// Вызывается из любого окна без передачи экземпляра:
    ///   ManagerService.DataService.Get&lt;Client&gt;()
    ///   ManagerService.DataService.Add(obj)
    ///   ManagerService.DataService.Login("admin", "admin123")
    /// </summary>
    public static class ManagerService
    {
        public static DataService DataService { get; } = new DataService();

        /// <summary>
        /// Возвращает авторизованного пользователя.
        /// Если никто не авторизован — возвращает пустого пользователя.
        /// </summary>
        public static User GetAuth()
        {
            return DataService.AuthUser ?? new User();
        }
    }
}
