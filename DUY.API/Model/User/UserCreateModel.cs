using System.ComponentModel.DataAnnotations;

namespace C.Tracking.API.Model.User
{
    public class UserCreateModel
    {
        public long id { set; get; }

        public string code { set; get; }
        [StringLength(50)]
        public string username { get; set; } = string.Empty;
        [StringLength(50)]
        public string password { get; set; } = string.Empty;
     
        [StringLength(50)]
        public string email { get; set; } = string.Empty;
        [StringLength(12)]
        public string phone_number { get; set; } = string.Empty;
        [StringLength(15)]
        public string landline_number { get; set; } = string.Empty;
        [StringLength(50)]
        public string full_name { get; set; } = string.Empty;
        [StringLength(250)]
        public string address { get; set; } = string.Empty;
        public DateTime birthday { set; get; }
        public long province_id { set; get; }
        public long district_id { set; get; }
        public long ward_id { set; get; }
        public byte sex { set; get; }
        public bool is_active { set; get; }
        public byte type { set; get; }
         
    }  
}
