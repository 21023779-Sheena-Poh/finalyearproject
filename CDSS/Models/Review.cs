using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int AppointmentId { get; set; }

    public DateTime ReviewDateTime { get; set; }

    public string? ReviewText { get; set; }

    public virtual Appointments Appointment { get; set; } = null!;
}
