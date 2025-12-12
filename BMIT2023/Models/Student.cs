using System.ComponentModel.DataAnnotations;

namespace BMIT2023.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string StudentId { get; set; } // e.g., "S001"
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}