using System.ComponentModel.DataAnnotations;

namespace BMIT2023.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        public string AdminId { get; set; } // e.g., "A001"
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}