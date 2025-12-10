using System.ComponentModel.DataAnnotations;

namespace BMIT2023.Models;

#nullable disable warnings

// ------------------------------------------------------------------------
// Login VM
// ------------------------------------------------------------------------
public class LoginVM
{
    [Required(ErrorMessage = "ID is required")]
    [MaxLength(10)]
    [Display(Name = "ID")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [MaxLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}

// ------------------------------------------------------------------------
// Register VM
// ------------------------------------------------------------------------
public class RegisterVM
{
    [Required(ErrorMessage = "Full Name is required")]
    [MaxLength(50)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    public string UserId { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(50)]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [MaxLength(15)]
    [MinLength(10, ErrorMessage = "Phone number must be at least 10 digits")]
    [Display(Name = "Phone Number")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [MaxLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
    [MaxLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Please select a gender")]
    [MaxLength(1)]
    [Display(Name = "Gender")]
    public string Gender { get; set; }
}