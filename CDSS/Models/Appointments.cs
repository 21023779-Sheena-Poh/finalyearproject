using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class Appointments
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? PurposeOfVisit { get; set; }

    public string? AdditionalNotes { get; set; }

    public virtual Patients Patient { get; set; } = null!;

    public string FullName => Patient.FullName;

    public virtual ICollection<Review> Review { get; set; } = new List<Review>();

    public virtual ICollection<MedicalStaff> MedicalStaff { get; set; } = new List<MedicalStaff>();

}
