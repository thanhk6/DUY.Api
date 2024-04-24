namespace C.Tracking.API.Model.Customer
{
    public class CustomerAddModel
    {
        public string phone { get; set; }
        public string name { get; set; } = "";
        public string password { get; set; }
        public string? referral_code { get; set; }
    }
}
