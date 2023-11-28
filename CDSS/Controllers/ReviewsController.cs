using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int AppointmentId { get; set; }

    public DateTime ReviewDateTime { get; set; }

    public string? ReviewText { get; set; }

    [ValidateNever]
    public virtual Appointments Appointment { get; set; } = null!;
}