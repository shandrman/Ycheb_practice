namespace Ycheb_practice.Service
{
    public class QueryParameters<T>
    {
        public List<FilterCondition> Filters { get; set; } = new();
        public List<string> Includes { get; set; } = new();
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
    }

    public record FilterCondition(string PropertyName, string Operation, object Value);
}
