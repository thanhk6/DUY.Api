using System.ComponentModel.DataAnnotations.Schema;
namespace DUY.API.Entities
{
    [Table("comment")]
    public class ComMent: IAuditableEntity
    {
        public string Name { get; set; }
        public long ParentID { get; set; }
        public long UserID { get; set; }
        public long song_Id { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public bool Activity { get; set; } = true;


    }
}
