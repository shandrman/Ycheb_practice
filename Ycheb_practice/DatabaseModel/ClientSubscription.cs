namespace Ycheb_practice.DatabaseModel
{
    public class ClientSubscription
    {
        public int Id { get; set; }

        // Внешний ключ → Client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        // Внешний ключ → Subscription
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;

        public DateOnly PurchaseDate { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int? RemainingVisits { get; set; }
        public string? Status { get; set; }
        public DateOnly? FreezeUntil { get; set; }
    }
}
