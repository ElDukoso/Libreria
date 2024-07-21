using Microsoft.AspNetCore.Identity;

namespace Libreria.Models
{
    public class User : IdentityUser
    {
        public string Rut { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } 
        public string Email { get; set; } 
    }
}

