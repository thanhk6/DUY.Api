using DUY.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUY.API.Entities
{
    [Table("song")]
    public class Song : IAuditableEntity
    {
        public string name {get;set;}
        public string description { get;set;}
    }
}
