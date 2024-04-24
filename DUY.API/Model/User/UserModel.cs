using DUY.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace DUY.API.Model.User
{
    public class UserModel
    {
        public long id { set; get; }

        public string code { set; get; }
        [StringLength(50)]
        public string username { get; set; } = string.Empty;
        [StringLength(50)]
        public string email { get; set; } = string.Empty;
        [StringLength(12)]
        public string phone_number { get; set; } = string.Empty;
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
        public long userAdded { set; get; }
        public long? userUpdated { set; get; }
        public Entities.File? image { get; set; }

    }
}
