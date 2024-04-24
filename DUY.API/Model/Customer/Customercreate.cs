using DUY.API.Entities;

namespace DUY.API.Model.Customer
{
    public class Customercreate
    {
        public long id { set; get; }
        public string name { set; get; } = string.Empty;
        public string code { set; get; } = string.Empty;//code     
        public string address { set; get; }
        public string phone { set; get; } = string.Empty;
      public string password { set; get; }
    }
}
