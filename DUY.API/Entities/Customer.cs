using System.ComponentModel.DataAnnotations.Schema;
namespace DUY.API.Entities
{
    [Table("customer")]
    public class Customer:IAuditableEntity
    {
        public string name { set; get; } = string.Empty;//tên 
        public string code   { set; get; } = string.Empty;//code     
        public string address { set; get; } = string.Empty;//địa chỉ
        public string phone { set; get; } = string.Empty;// số điện thoại
        public string email { set; get; } = string.Empty; //email
        public string username { set; get; } = string.Empty;
        public string password { set; get; } = string.Empty;
        public string pass_code { set; get; } = string.Empty;
        public long facebookId { get; set; }
        public string Google { get; set; } =string.Empty;
        public bool is_active { set; get; } = true;
    }
}
