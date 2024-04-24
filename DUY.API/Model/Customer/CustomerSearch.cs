
namespace C.Tracking.API.Model.Customer
{
    public class CustomerSearch
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? keyword { get; set; }
    }
}
