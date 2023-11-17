using System;
using System.Collections.Generic;

namespace CDSS.Models;

public partial class Medication
{
    public int MedicationId { get; set; }

    public string MedicationName { get; set; } = null!;

    public string? Class { get; set; }
}
