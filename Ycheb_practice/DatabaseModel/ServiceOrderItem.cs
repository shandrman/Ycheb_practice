namespace Ycheb_practice.DatabaseModel
{
    public class ServiceOrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Вычисляемое поле
        public decimal Subtotal => Quantity * UnitPrice;

        // Внешний ключ → ServiceOrder
        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; } = null!;

        // Внешний ключ → Service
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
