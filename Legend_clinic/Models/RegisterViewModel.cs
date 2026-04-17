using System.ComponentModel.DataAnnotations;

namespace Legend_clinic.Models
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3 to 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscore")]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public DateOnly Dob { get; set; }

        [Required]
        public string Gender { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        // Custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Dob >= DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "Date of Birth must be in the past.",
                    new[] { nameof(Dob) }
                );
            }
        }
    }
}