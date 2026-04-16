using System.ComponentModel.DataAnnotations;

namespace Legend_clinic.Models
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
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
        public string Phone { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        // ✅ Custom validation here
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