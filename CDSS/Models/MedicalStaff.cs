using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class MedicalStaff
{

    public int MedicalStaffId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Appointments> Appointment { get; set; } = new List<Appointments>();
}
