using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ycheb_practice.DatabaseModel;
using Ycheb_practice.Service;

namespace Ycheb_practice.Pages
{
    public partial class SubscriptionsPage : Page
    {
        private bool _loaded = false;

        public SubscriptionsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => { _loaded = true; Load(); };
        }

        private void Load()
        {
            var items = ManagerService.DataService.Get<Subscription>(
                new QueryParameters<Subscription>
                {
                    SortBy      = "TypeName",
                    IsAscending = true
                })
                .Select(s => new SubVM
                {
                    Id           = s.Id,
                    TypeName     = s.TypeName,
                    Price        = s.Price,
                    DurationDays = s.DurationDays,
                    VisitsLimit  = s.VisitsLimit,
                    IsActiveStr  = s.IsActive == true ? "Да" : "Нет",
                    Source       = s
                }).ToList();

            dgSubs.ItemsSource = items;
            tbStatus.Text = $"Типов абонементов: {items.Count}";
        }

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            var win = new Windows.SubscriptionEditWindow(null);
            if (win.ShowDialog() == true) Load();
        }

        private void Dg_DoubleClick(object s, MouseButtonEventArgs e)
        {
            if (dgSubs.SelectedItem is SubVM vm)
            {
                var win = new Windows.SubscriptionEditWindow(vm.Source);
                if (win.ShowDialog() == true) Load();
            }
        }
    }

    public class SubVM
    {
        public int         Id           { get; set; }
        public string      TypeName     { get; set; } = "";
        public decimal     Price        { get; set; }
        public int         DurationDays { get; set; }
        public int?        VisitsLimit  { get; set; }
        public string      IsActiveStr  { get; set; } = "";
        public Subscription Source      { get; set; } = null!;
    }
}
