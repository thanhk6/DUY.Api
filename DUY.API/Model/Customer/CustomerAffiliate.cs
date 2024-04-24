namespace C.Tracking.API.Model.Customer
{
    public class CustomerAffiliate
    {
        public long id { get; set; }
        public string name { get; set; }
        public double investment_money { get; set; }
        public double amount_transferred { get; set; }
        public DateTime date_join { get; set; }
    }
    public class CustomerAffiliateContract
    {
        public long user_id { get; set; }
        public long contract_id { get; set; }
        public double investment_money { get; set; }
        public DateTime date_confirm { get; set; }
        public int status { get; set; }
    }

}
