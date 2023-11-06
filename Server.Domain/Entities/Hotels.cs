using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities
{
    public class Hotels : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string? Country { get; set; }
        public int? ZipCode { get; set; }
        public string? Address { get; set; }
        public int? BeCauseId { get; set; }
    }
}
