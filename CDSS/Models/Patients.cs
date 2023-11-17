using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class Patients
{
    public int PatientId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? Birthdate { get; set; }

    public string? Ward { get; set; }

    public string? Bed { get; set; }

    public decimal? Weight { get; set; }

    public string? BloodType { get; set; }

    public string? MedicalCondition { get; set; }

    public virtual ICollection<Appointments> Appointments { get; set; } = new List<Appointments>();
}
