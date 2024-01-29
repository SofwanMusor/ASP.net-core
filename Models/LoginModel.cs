using System.ComponentModel.DataAnnotations;

namespace LoginModel
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
        public string? Role { get; set; }

        // [Required]
        // [Display(Name = "Role")]
        // public string? Role { get; set; }

    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current Password is required.")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required.")]
        [Compare("NewPassword", ErrorMessage = "The New Password and Confirm New Password must match.")]
        public string? ConfirmNewPassword { get; set; }
    }
    public class Subdata
    {
        public string? RoleName { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        // เพิ่ม properties อื่น ๆ ตามต้องการ
    }

}