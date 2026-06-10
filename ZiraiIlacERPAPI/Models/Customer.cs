using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZiraiIlacERPAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Email { get; set; }

        [StringLength(600)]
        public string? Address { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Order>? Orders { get; set; }
    }
}
