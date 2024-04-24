using System.ComponentModel.DataAnnotations;

namespace DUY.API.Entities
{
    public class IAuditableEntity
    {
        [Key]
        public long id { set; get; }
        public long userAdded { set; get; }
        public long? userUpdated { set; get; }
        public DateTime dateAdded { get; set; }
        public DateTime? dateUpdated { get; set; }
        public bool is_delete { get; set; } = false;
    }

}
