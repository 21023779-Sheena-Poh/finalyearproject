using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class Patients
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
}
