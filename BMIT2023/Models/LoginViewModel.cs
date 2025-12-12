using System.ComponentModel.DataAnnotations;

namespace BMIT2023.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User ID is required.")]
        [Display(Name = "User ID (e.g. S001, T001, A001)")]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}