using System.ComponentModel.DataAnnotations;

namespace BMIT2023.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public string TeacherId { get; set; } // e.g., "T001"
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}