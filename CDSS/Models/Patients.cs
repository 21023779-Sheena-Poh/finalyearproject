using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDSS.Models
{
    public partial class Patients : IValidatableObject
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please enter the first name.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the last name.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the birthdate.")]
        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public Patients()
        {
            Birthdate = DateTime.MinValue.Date;
        }

        [Required(ErrorMessage = "Please enter the ward.")]
        public string? Ward { get; set; }

        [Required(ErrorMessage = "Please enter the bed.")]
        public string? Bed { get; set; }

        [Required(ErrorMessage = "Please enter the weight.")]
        public decimal? Weight { get; set; }

        [Required(ErrorMessage = "Please enter the blood type.")]
        public string? BloodType { get; set; }

        [Required(ErrorMessage = "Please select at least one medical condition.")]
        public string? MedicalCondition { get; set; }

        public virtual ICollection<Appointments> Appointments { get; set; } = new List<Appointments>();

        public string? GetFormattedBirthdate()
        {
            return Birthdate?.ToString("yyyy-MM-dd"); // Change the format as needed
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Birthdate.HasValue && Birthdate > DateTime.Now.Date)
            {
                yield return new ValidationResult("Birthdate cannot be in the future.", new[] { nameof(Birthdate) });
            }
        }
    }
}
