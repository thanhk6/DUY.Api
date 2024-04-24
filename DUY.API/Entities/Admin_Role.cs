using System.ComponentModel.DataAnnotations.Schema;

namespace DUY.API.Entities
{
    [Table("admin_role")]
    public class Admin_Role : IAuditableEntity
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string note { get; set; } = string.Empty;
    }
}
