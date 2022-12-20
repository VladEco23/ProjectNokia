using Microsoft.AspNetCore.Identity;
namespace ProjectNokia.Models
{
    public class AppUser:IdentityUser
    {
        public List<Ticket>? Tickets { get; set; }
    }
}
