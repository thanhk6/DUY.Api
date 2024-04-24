using System.ComponentModel.DataAnnotations.Schema;

namespace DUY.API.Entities
{
    [Table("admin_role_group")]
    public class Admin_Role_Group:IAuditableEntity
    {
        public long group_id { get; set; }
        public long role_id { get; set; }
    }

}
