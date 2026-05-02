namespace Ycheb_practice.DatabaseModel
{
    public class Subscription
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int? VisitsLimit { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();
    }
}
