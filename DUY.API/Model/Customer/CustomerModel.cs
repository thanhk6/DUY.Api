
using DUY.API.Entities;

namespace DUY.API.Model.Customer
{
    public class CustomerModel : IAuditableEntity
    {

        public string name { set; get; } = string.Empty;
        public string code { set; get; } = string.Empty;//code     
        public string address { set; get; }
        public string phone { set; get; } = string.Empty;
        public string mail { get; set; } = string.Empty;
        public long facebookId { get;set;}
        public string username { set; get; } = string.Empty;
        public string password { set; get; } = string.Empty;
        public string Google { get; set; } = string.Empty;
        public Entities.File? image { get; set; }
    }
    public class CustomerLoginOTPModel
    {
        public string full_name { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;

        public long customer_id { set; get; }
        public int checkLogin { set; get; } //1 otp đã hết hạn,2 OTP không chính xác, 3 tài khoản chưa tồn tại, 4 thanh cong
    }

    public class CustomerClaimModel
    {
        public long id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;
        public byte type { set; get; }

    }
}
