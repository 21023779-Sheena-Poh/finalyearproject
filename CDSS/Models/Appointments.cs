using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CDSS.Models;

public partial class Appointments
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string PurposeOfVisit { get; set; } = null!;

    public string? AdditionalNotes { get; set; }

    [ValidateNever]
    public virtual Patients Patient { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<Review> Review { get; set; } = new List<Review>();

    [ValidateNever]
    public virtual ICollection<MedicalStaff> MedicalStaff { get; set; } = new List<MedicalStaff>();


}