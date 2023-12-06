using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class MedicalStaff
{
    [ValidateNever]
    public int MedicalStaffId { get; set; }
    [ValidateNever]
    public string Username { get; set; } = null!;
    [ValidateNever]
    public string Password { get; set; } = null!;
    [ValidateNever]
    public string? FirstName { get; set; }
    [ValidateNever]
    public string? LastName { get; set; }
    [ValidateNever]
    public string Role { get; set; } = null!;
    [ValidateNever]
    public virtual ICollection<Appointments> Appointment { get; set; } = new List<Appointments>();
}
