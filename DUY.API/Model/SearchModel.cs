namespace C.Tracking.API.Model
{
    public class SearchModel
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? keyword { get; set; }
        public long? customer_id { get; set; }
        public long? contract_id { get; set; }
        public long? status_id { get; set; }
    }
}
