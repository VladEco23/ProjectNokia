using System.ComponentModel.DataAnnotations;

namespace ProjectNokia.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LimitDate { get; set; }
        public string? State { get; set; }
        public string Description { get; set; }
        public string? Priority { get; set; }
        public string? Reporter { get; set; }
        public string? Assigne { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public Ticket() => CreatedDate = DateTime.Now;
    }
}
