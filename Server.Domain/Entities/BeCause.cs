using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities
{
    public class BeCause : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Address { get; set; }
        public int? PostalCode { get; set; }
        public bool IsCertified { get; set; }
        public string? IssueDate { get; set; }
        public string? ExpirationDate { get; set; }
        public string? Website { get; set; }
    }
}
