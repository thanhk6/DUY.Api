namespace C.Tracking.API.Model
{
    public class SearchBase
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? keyword { get; set; }
        public DateTime? start_date { set; get; }
        public DateTime? end_date { set; get; }
    }
}
